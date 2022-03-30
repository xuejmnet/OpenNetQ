using System;
using Microsoft.Extensions.Logging;
using OpenNetQ.Common.NameSrv;
using OpenNetQ.NameServer.RouteInfo;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Netty;
using OpenNetQ.TaskSchedulers;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.NameServer
{
    public class NameServerController:InjectService
    {
        private readonly NameServerOption _nameServerOption;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RemotingServerOption _remotingServerOption;
        private readonly IRouteInfoManager _routeInfoManager;
        private readonly IBrokerHousekeepingService _brokerHousekeepingService;

        private IRemotingServer _remotingServer;
        private OpenNetQTaskScheduler _remoteExecutor;
        public NameServerController(
            ILoggerFactory loggerFactory,
            NameServerOption nameServerOption,
            RemotingServerOption remotingServerOption,
            IRouteInfoManager routeInfoManager,
            IBrokerHousekeepingService brokerHousekeepingService,
            IServiceProvider serviceProvider):base(serviceProvider)
        {
            _loggerFactory = loggerFactory;
            _nameServerOption = nameServerOption;
            _remotingServerOption = remotingServerOption;
            _routeInfoManager = routeInfoManager;
            _brokerHousekeepingService = brokerHousekeepingService;
        }

        public void Initialize()
        {
            _remotingServer = new NettyRemotingServer(_loggerFactory, _remotingServerOption);
            _remoteExecutor = new OpenNetQTaskScheduler(_remotingServerOption.ServerWorkerThreads, "RemotingExecutorThread_");
        }
    }
}