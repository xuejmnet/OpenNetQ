using System;
using System.Reflection;
using System.Text.Json.Serialization;
using J2N.IO;
using OpenNetQ.Exceptions;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Remoting.Protocol
{
    public class RemotingCommand
    {
        private static readonly int RPC_TYPE = 0; // 0, REQUEST_COMMAND
        private static readonly int RPC_ONEWAY = 1; // 0, RPC
        private static AtomicSequence requestId = new AtomicSequence(0);
        private static SerializeTypeEnum SerializeTypeConfigInThisServer = SerializeTypeEnum.JSON;
        private static readonly IDictionary<Type, PropertyInfo[]> CLASS_HASH_MAP = new Dictionary<Type, PropertyInfo[]>();
        private static readonly IDictionary<Type, string> CANONICAL_NAME_CACHE = new Dictionary<Type, string>();

        static RemotingCommand()
        {
            //TODO
            //读取环境变量或者配置文件
            SerializeTypeConfigInThisServer = SerializeTypeEnum.JSON;
        }

        public short Code { get; set; }
        public LanguageCodeEnum Language { get; set; } = LanguageCodeEnum.DOTNET;
        public short Version { get; set; } = 0;
        public int Opaque { get; set; } = requestId.GetAndIncrement();
        public int Flag { get; set; }
        public string? Remark { get; set; }
        public IDictionary<string, string?>? ExtFields { get; set; }
        private ICommandCustomHeader? CustomHeader { get; set; }

        public SerializeTypeEnum SerializeTypeCurrentRPC = SerializeTypeConfigInThisServer;
        public byte[]? Body { get; set; }
        public RemotingCommand()
        {

        }

        public static RemotingCommand CreateRequestCommand(int code, ICommandCustomHeader customHeader)
        {
            var remotingCommand = new RemotingCommand()
            {
                Code = code,
                CustomHeader = customHeader
            };
            return remotingCommand;
        }
        /// <summary>
        /// 创建响应命令
        /// </summary>
        /// <param name="customHeaderType"></param>
        /// <returns></returns>
        public static RemotingCommand? CreateResponseCommand(Type customHeaderType)
        {
            return CreateResponseCommand(RemotingSysResponseCode.SYSTEM_ERROR, "not set ant response code", customHeaderType);
        }
        /// <summary>
        /// 创建响应命令
        /// </summary>
        /// <param name="code"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static RemotingCommand CreateResponseCommand(int code, string remark)
        {
            return CreateResponseCommand(code, remark, null)!;
        }
        /// <summary>
        /// 创建响应命令
        /// </summary>
        /// <param name="code"></param>
        /// <param name="remark"></param>
        /// <param name="customHeaderType"></param>
        /// <returns></returns>
        /// <exception cref="OpenNetQException"></exception>
        public static RemotingCommand? CreateResponseCommand(int code, string? remark, Type? customHeaderType)
        {

            var remotingCommand = new RemotingCommand()
            {
                Code = code,
                Remark = remark
            };
            remotingCommand.MarkResponseType();
            //TODO 需要重新实现
            //SetCmdVersion(remotingCommand);
            if (customHeaderType != null)
            {

                if (customHeaderType.IsAssignableFrom(typeof(ICommandCustomHeader)))
                {
                    throw new OpenNetQException($"{customHeaderType} not impl {typeof(ICommandCustomHeader)}");
                }

                try
                {
                    var customHeader = (ICommandCustomHeader)Activator.CreateInstance(customHeaderType)!;
                    remotingCommand.CustomHeader = customHeader;
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            return remotingCommand;
        }
        //private static void SetCmdVersion(RemotingCommand cmd)
        //{
        //    if (configVersion >= 0)
        //    {
        //        cmd.setVersion(configVersion);
        //    }
        //    else
        //    {
        //        String v = System.getProperty(REMOTING_VERSION_KEY);
        //        if (v != null)
        //        {
        //            int value = Integer.parseInt(v);
        //            cmd.setVersion(value);
        //            configVersion = value;
        //        }
        //    }
        //}





        /// <summary>
        /// 是否是响应请求
        /// </summary>
        /// <returns></returns>
        public bool IsResponseType()
        {
            int bits = 1 << RPC_TYPE;
            return (Flag & bits) == bits;
        }

        /// <summary>
        /// 标记为响应请求
        /// </summary>
        public void MarkResponseType()
        {
            int bits = 1 << RPC_TYPE;
            Flag |= bits;
        }

        /// <summary>
        /// 标记消息为单向
        /// </summary>
        public void MarkOnewayRPC()
        {
            int bits = 1 << RPC_ONEWAY;
            Flag |= bits;
        }

        /// <summary>
        /// 判断消息是否是单向消息
        /// </summary>
        /// <returns></returns>
        public bool IsOnewayRPC()
        {
            int bits = 1 << RPC_ONEWAY;
            return (Flag & bits) == bits;
        }

        /// <summary>
        /// get current command type
        /// 获取当前命令的类型
        /// </summary>
        /// <returns></returns>
        public RemotingCommandType GetCommandType()
        {
            if (IsResponseType())
            {
                return RemotingCommandType.RESPONSE_COMMAND;
            }

            return RemotingCommandType.REQUEST_COMMAND;
        }
        public static RemotingCommand Decode(byte[] array)
        {
            ByteBuffer byteBuffer = ByteBuffer.Wrap(array);
            return Decode(byteBuffer);
        }

        public static int GetHeaderLength(int length)
        {
            return length & 0xFFFFFF;
        }
        public static SerializeTypeEnum GetProtocolType(int source)
        {
            return (SerializeTypeEnum)((byte)((source >> 24) & 0xFF));
        }
        private static RemotingCommand HeaderDecode(byte[] headerData, SerializeTypeEnum type)
        {
            RemotingCommand resultONQ = OpenNetQSerializable.OpenNetQProtocolDecode(headerData);
            resultONQ.SerializeTypeCurrentRPC = SerializeTypeEnum.OPENNETQ;
            return resultONQ;
        }
        public static RemotingCommand Decode(ByteBuffer byteBuffer)
        {
            int length = byteBuffer.Limit;
            int oriHeaderLen = byteBuffer.GetInt32();
            int headerLength = GetHeaderLength(oriHeaderLen);

            byte[] headerData = new byte[headerLength];
            byteBuffer.Get(headerData);

            RemotingCommand cmd = HeaderDecode(headerData, GetProtocolType(oriHeaderLen));

            int bodyLength = length - 4 - headerLength;
            byte[]? bodyData = null;
            if (bodyLength > 0)
            {
                bodyData = new byte[bodyLength];
                byteBuffer.Get(bodyData);
            }
            cmd.Body = bodyData;

            return cmd;
        }

        public ByteBuffer Encode()
        {
            // 1> header length size
            int length = 4;

            // 2> header data length
            byte[] headerData = this.HeaderEncode();
            length += headerData.Length;

            // 3> body data length
            if (this.Body != null)
            {
                length += Body.Length;
            }

            ByteBuffer result = ByteBuffer.Allocate(4 + length);

            // length
            result.PutInt32(length);

            // header length
            result.Put(MarkProtocolType(headerData.Length, SerializeTypeCurrentRPC));

            // header data
            result.Put(headerData);

            // body data;
            if (this.Body != null)
            {
                result.Put(this.Body);
            }

            result.Flip();
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] HeaderEncode()
        {
            MakeCustomHeaderToNet();
            if (SerializeTypeEnum.OPENNETQ == SerializeTypeCurrentRPC)
            {
                return OpenNetQSerializable.OpenNetQProtocolEncode(this);
            }
            else
            {
                return RemotingSerializable.Encode(this);
            }

        }

        public void MakeCustomHeaderToNet()
        {
            if (this.CustomHeader != null)
            {
                if (ExtFields == null)
                {
                    ExtFields = new Dictionary<string, string?>();
                }
            }
        }

        public ByteBuffer EncodeHeader()
        {
            return EncodeHeader(Body?.Length ?? 0);
        }
        public ByteBuffer EncodeHeader(int bodyLength)
        {
            // 1> header length size
            int length = 4;

            // 2> header data length
            byte[] headerData = HeaderEncode();

            // 3> body data length
            if (this.Body != null)
            {
                length += Body.Length;
            }
            ByteBuffer result = ByteBuffer.Allocate(4 + length);

            // length
            result.PutInt32(length);

            // header length
            result.Put(MarkProtocolType(headerData.Length, SerializeTypeCurrentRPC));

            // header data
            result.Put(headerData);

            // body data;
            if (this.Body != null)
            {
                result.Put(this.Body);
            }

            result.Flip();

            return result;
        }

        public static byte[] MarkProtocolType(int source, SerializeTypeEnum type)
        {
            byte[] result = new byte[4];

            result[0] = (byte)type;
            result[1] = (byte)((source >> 16) & 0xFF);
            result[2] = (byte)((source >> 8) & 0xFF);
            result[3] = (byte)(source & 0xFF);
            return result;
        }
    }
}