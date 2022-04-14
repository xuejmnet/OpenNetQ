using System.Net;
using System.Text;
using DotNetty.Transport.Channels;
using OpenNetQ.Logging;

namespace OpenNetQ.Remoting.Common
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 13 November 2020 16:56:12
* @Email: 326308290@qq.com
*/
    public class RemotingHelper
    {
        private static IInternalNetQLogger _log = InternalNetQLoggerFactory.GetLogger<RemotingHelper>();
        private RemotingHelper(){ }

        public static IPEndPoint String2IpEndPoint(string addr)
        {
            var split = addr.LastIndexOf(":");
            var host = addr.Substring(0,split);
            var port = addr.Substring(split+1);
            return new IPEndPoint(IPAddress.Parse(host), int.Parse(port));
        }
        /// <summary>
        /// 通过channel解析Ip地址
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string ParseChannelRemoteAddr(IChannel? channel) {
            if (null == channel) {
                return string.Empty;
            }
            //获取Ip
            IPEndPoint iPEndPoint = (IPEndPoint)channel.RemoteAddress;
            
            string addr = iPEndPoint.Address.ToString();

            if (!string.IsNullOrWhiteSpace(addr)) {
                int index = addr.LastIndexOf("/", StringComparison.Ordinal);
                if (index >= 0) {
                    return addr.Substring(index + 1);
                }
                return addr;
            }

            return string.Empty;
        }
        /// <summary>
        /// 解析地址
        /// </summary>
        /// <param name="socketAddress"></param>
        /// <returns></returns>
        public static string ParseSocketAddress(EndPoint? socketAddress) {
            if (socketAddress != null) {
                return socketAddress.ToString()??string.Empty;
            }
            return string.Empty;
        }
    }
}