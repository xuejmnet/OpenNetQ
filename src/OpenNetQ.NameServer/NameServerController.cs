using System;
using Microsoft.Extensions.Logging;
using OpenNetQ.Common.NameSrv;
using OpenNetQ.Extensions;
using OpenNetQ.NameServer.KvConfig;
using OpenNetQ.NameServer.Processor;
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
        private readonly IKvConfigManager _kvConfigManager;
        private readonly IBrokerHousekeepingService _brokerHousekeepingService;

        private readonly IRemotingServer _remotingServer;
        private readonly OpenNetQTaskScheduler _remoteExecutor;
        private readonly OpenNetQTaskScheduler _scheduledExecutorService = new OpenNetQTaskScheduler(1, "NSScheduledThread");
        public NameServerController(
            NameServerOption nameServerOption,
            RemotingServerOption remotingServerOption,
            IRouteInfoManager routeInfoManager,
            IKvConfigManager kvConfigManager,
            IRemotingServer remotingServer,
            IServiceProvider serviceProvider):base(serviceProvider)
        {
            _nameServerOption = nameServerOption;
            _remotingServerOption = remotingServerOption;
            _routeInfoManager = routeInfoManager;
            _kvConfigManager = kvConfigManager;
            _remotingServer = remotingServer;
            _remoteExecutor=new OpenNetQTaskScheduler(_remotingServerOption.ServerWorkerThreads, "RemotingExecutorThread_");
        }

        public async Task Initialize()
        {
            RegisterProcessor();
            await _scheduledExecutorService.RunFixedRate( () =>
            {
                _routeInfoManager.ScanNotActiveBroker();
            },TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(10) );
             
            await _scheduledExecutorService.RunFixedRate( () =>
            {
                _kvConfigManager.PrintAllPeriodically();
            },TimeSpan.FromMinutes(1),TimeSpan.FromMinutes(10) );
        }

        private void RegisterProcessor()
        {
            //TODO 
            var defaultProcessor = GetRequiredService<IDefaultProcessor>();
            _remotingServer.RegisterDefaultProcessor(defaultProcessor, _remoteExecutor);
        }

        public async Task StartAsync()
        {
            await _remotingServer.StartAsync();
        }

        public async Task StopAsync()
        {
            await _remotingServer.StopAsync();
            _remoteExecutor.Dispose();
            _scheduledExecutorService.Dispose();
        }
    }
}