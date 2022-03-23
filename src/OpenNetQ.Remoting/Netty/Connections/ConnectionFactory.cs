using KhaosLog.NettyClientProvider.Abstractions;
using KhaosLog.NettyClientProvider.Abstractions.Connections;
using KhaosLog.NettyClientProvider.Common;
using OpenNetQ.Remoting.Netty.Connections;

namespace KhaosLog.NettyClientProvider.Impls
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 17 November 2020 10:42:18
* @Email: 326308290@qq.com
*/
    public class ConnectionFactory:IConnectionFactory
    {
        private readonly RemotingClientOption _option;

        public ConnectionFactory(RemotingClientOption option)
        {
            _option = option;
        }
        public IConnection CreateConnection()
        {
            return new Connection(_option);
        }
    }
}