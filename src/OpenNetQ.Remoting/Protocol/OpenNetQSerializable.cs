using System;
using System.Text;
using J2N.IO;
using OpenNetQ.Common.Extensions;

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

            int totalLen = CalTotalLen(remarkLen, extLen);
            var headerBuffer = ByteBuffer.Allocate(totalLen);
            // int code(~32767)
            headerBuffer.PutInt16((short)cmd.Code);
            // LanguageCode language
            headerBuffer.Put((byte)cmd.Language);
            // int version(~32767)
            headerBuffer.PutInt16((short)cmd.Version);
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
        /// <summary>
        /// –Ú¡–ªØk-v Ù–‘
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static byte[] MapSerialize(IDictionary<string, string?> map)
        {

            int totalLength = 0;
            foreach (var keyValuePair in map)
            {
                if (keyValuePair.Value != null)
                {
                    int kvLength = 2 + keyValuePair.Key.GetBytes(Encoding.UTF8).Length//keySize+key
                                     + 4 + keyValuePair.Value.GetBytes(Encoding.UTF8).Length;//valSize+val
                    totalLength += kvLength;
                }
            }

            var content = ByteBuffer.Allocate(totalLength);
            foreach (var keyValuePair in map)
            {
                if (keyValuePair.Value != null)
                {
                   var key = keyValuePair.Key.GetBytes(Encoding.UTF8);
                   var val = keyValuePair.Value.GetBytes(Encoding.UTF8);
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
    }
}