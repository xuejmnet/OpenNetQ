using System;
using System.Buffers;
using System.Text;
using J2N;
using J2N.IO;
using OpenNetQ.Extensions;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Remoting.Protocol
{
    public class OpenNetQSerializable
    {
        public static byte[] OpenNetQProtocolEncode(RemotingCommand cmd)
        {
            //string remark
            byte[]? remarkBytes = null;
            int remarkLen = 0;
            if (cmd.Remark.NoNullOrEmpty())
            {
                remarkBytes = cmd.Remark.GetBytes(Encoding.UTF8);
                remarkLen = remarkBytes.Length;
            }
            //hashmap extFields
            byte[]? extFieldsBytes = null;
            int extLen = 0;
            if (cmd.ExtFields.IsNotEmpty())
            {
                extFieldsBytes = MapSerialize(cmd.ExtFields!);
                extLen = extFieldsBytes.Length;
            }

            var totalLen = CalTotalLen(remarkLen,extLen);
            ByteBuffer headerBuffer = ByteBuffer.Allocate(totalLen);
            // int code(~32767)
            headerBuffer.PutInt16(cmd.Code);
            // LanguageCode language
            headerBuffer.Put((byte)cmd.Language);
            // int version(~32767)
            headerBuffer.PutInt16(cmd.Version);
            // int opaque
            headerBuffer.PutInt32(cmd.Opaque);
            // int flag
            headerBuffer.PutInt32(cmd.Flag);
            // String remark
            if (remarkBytes != null)
            {
                headerBuffer.PutInt32(remarkBytes.Length);
                headerBuffer.Put(remarkBytes);
            }
            else
            {
                headerBuffer.PutInt32(0);
            }
            // HashMap<String, String> extFields;
            if (extFieldsBytes != null)
            {
                headerBuffer.PutInt32(extFieldsBytes.Length);
                headerBuffer.Put(extFieldsBytes);
            }
            else
            {
                headerBuffer.PutInt32(0);
            }

            return headerBuffer.Array;
        }
        private static byte[] MapSerialize(IDictionary<string, string?> map)
        {
            // keySize+key+valSize+val

            var totalLength = map.Sum(o =>
            {
                if (o.Value == null)
                {
                    return 0;
                }

                return 2 + Encoding.UTF8.GetByteCount(o.Key.AsSpan()) + 4 + Encoding.UTF8.GetByteCount(o.Value.AsSpan());
            });

            //var arrayPool = ArrayPool<byte>.Shared;
            //var bytes = arrayPool.Rent(totalLength);
            

            ByteBuffer content = ByteBuffer.Allocate(totalLength);
            var enumerator = map.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var (k, v) = enumerator.Current;
                if (v != null)
                {
                    var key = Encoding.UTF8.GetBytes(k);
                    var val = Encoding.UTF8.GetBytes(v);
                    content.PutInt16((short)key.Length);
                    content.Put(key);
                    content.PutInt32(val.Length);
                    content.Put(val);
                }
            }

            return content.Array;
        }

        private static int CalTotalLen(int remark, int ext)
        {
            // int code(~32767)
            int length = 2
                         // LanguageCode language
                         + 1
                         // int version(~32767)
                         + 2
                         // int opaque
                         + 4
                         // int flag
                         + 4
                         // String remark
                         + 4 + remark
                         // HashMap<String, String> extFields
                         + 4 + ext;

            return length;
        }

        public static RemotingCommand OpenNetQProtocolDecode(byte[] headerArray)
        {
            RemotingCommand cmd = new RemotingCommand();
            ByteBuffer headerBuffer = ByteBuffer.Wrap(headerArray);
            // int code(~32767)
            cmd.Code = headerBuffer.GetInt16();
            // LanguageCode language
            cmd.Language = (LanguageCodeEnum)headerBuffer.Get();
            // int version(~32767)
            cmd.Version = headerBuffer.GetInt16();
            // int opaque
            cmd.Opaque = headerBuffer.GetInt32();
            // int flag
            cmd.Flag = headerBuffer.GetInt32();
            // String remark
            int remarkLength = headerBuffer.GetInt32();
            if (remarkLength > 0)
            {
                byte[] remarkContent = new byte[remarkLength];
                headerBuffer.Get(remarkContent);
                cmd.Remark = Encoding.UTF8.GetString(remarkContent);
            }

            // HashMap<String, String> extFields
            int extFieldsLength = headerBuffer.GetInt32();
            if (extFieldsLength > 0)
            {
                byte[] extFieldsBytes = new byte[extFieldsLength];
                headerBuffer.Get(extFieldsBytes);
                cmd.ExtFields = MapDeserialize(extFieldsBytes);
            }
            return cmd;
        }
        public static Dictionary<string, string?>? MapDeserialize(byte[] bytes)
        {
            if (bytes.Length <= 0)
                return null;

            Dictionary<String, String> map = new Dictionary<String, String>();
            ByteBuffer byteBuffer = ByteBuffer.Wrap(bytes);

            short keySize;
            byte[] keyContent;
            int valSize;
            byte[] valContent;
            while (byteBuffer.HasRemaining)
            {
                keySize = byteBuffer.GetInt16();
                keyContent = new byte[keySize];
                byteBuffer.Get(keyContent);

                valSize = byteBuffer.GetInt32();
                valContent = new byte[valSize];
                byteBuffer.Get(valContent);

                map.Add(Encoding.UTF8.GetString(keyContent), Encoding.UTF8.GetString(valContent));
            }
            return map;
        }

        public static bool IsBlank(string? str)
        {
            int strLen;
            if (str == null || (strLen = str.Length) == 0)
            {
                return true;
            }
            for (int i = 0; i < strLen; i++)
            {
                if (!Character.IsWhiteSpace(str[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}