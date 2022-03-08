using System;
using System.Text;
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
            if (extFieldsBytes.IsNotEmpty())
            {
                
            }
        }
    }
}