using System;
using System.Reflection;
using J2N.IO;
using OpenNetQ.Common;

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
            SerializeTypeConfigInThisServer= SerializeTypeEnum.JSON;
        }
        
        public int Code { get; set; }
        public LanguageCodeEnum Language { get; set; }= LanguageCodeEnum.DOTNET;
        public int Version { get; set; } = 0;
        public int Opaque { get; set; } = requestId.GetAndIncrement();
        public int Flag { get; set; }
        public string Remark { get; set; }
        public IDictionary<string,string> ExtFields { get; set; }
        private ICommandCustomHeader CustomHeader{ get; set; }

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

        public static RemotingCommand CreateResponseCommand<TCommandCustomHeader>() where TCommandCustomHeader : ICommandCustomHeader
        {
            return CreateResponseCommand<TCommandCustomHeader>(RemotingSysResponseCodeConstant.SYSTEM_ERROR, "not set ant response code");
        }

        public static RemotingCommand CreateResponseCommand<TCommandCustomHeader>(int code, string remark) where TCommandCustomHeader : ICommandCustomHeader
        {
            try
            {
                var customHeader = Activator.CreateInstance<TCommandCustomHeader>();
                return CreateResponseCommand(code, remark, customHeader);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static RemotingCommand CreateResponseCommand(int code, string remark,ICommandCustomHeader? customHeader)
        {
            var remotingCommand = new RemotingCommand()
            {
                Code = code,
                Remark = remark
            };
            remotingCommand.MarkResponseType();
            if (customHeader != null)
            {
                remotingCommand.CustomHeader = customHeader;
            }

            return remotingCommand;
        }
        
        
        
        
        
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
        public RemotingCommandType GetType()
        {
            if (IsResponseType())
            {
                return RemotingCommandType.RESPONSE_COMMAND;
            }

            return RemotingCommandType.REQUEST_COMMAND;
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
            byte[] headerData=
            
        }

        public byte[] HeaderEncode()
        {
            MakeCustomHeaderToNet();
            if (SerializeTypeEnum.OPENNETQ == SerializeTypeCurrentRPC)
            {
                
            }

        }

        public void MakeCustomHeaderToNet()
        {
            if (this.CustomHeader != null)
            {
                if (ExtFields == null)
                {
                    ExtFields = new Dictionary<string, string>();
                }
            }
        }
    }
}