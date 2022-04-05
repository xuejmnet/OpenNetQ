using System;
using Microsoft.Extensions.Logging;
using OpenNetQ.Common.NameSrv;
using OpenNetQ.Extensions;
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
        private readonly IBrokerHousekeepingService _brokerHousekeepingService;

        private IRemotingServer _remotingServer;
        private OpenNetQTaskScheduler _remoteExecutor;
        private readonly OpenNetQTaskScheduler scheduledExecutorService = new OpenNetQTaskScheduler(1, "NSScheduledThread");
        private readonly FixedSchedule _scanNotActiveBrokerSchedule = new FixedSchedule("NameServerScanNotActiveBrokerSchedule", TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(10));
        //private readonly FixedSchedule _scanNotActiveBrokerSchedule1 = new FixedSchedule("NameServerFixedSchedule",TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(10));
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

        public async Task Initialize()
        {
            _remotingServer = new NettyRemotingServer(_loggerFactory, _remotingServerOption);
            _remoteExecutor = new OpenNetQTaskScheduler(_remotingServerOption.ServerWorkerThreads, "RemotingExecutorThread_");
            RegisterProcessor();
            await _scanNotActiveBrokerSchedule.StartAsync(async token =>
            {
                this._routeInfoManager.ScanNotActiveBroker();
            });
            //TODO printAllPeriodically
            //await _scanNotActiveBrokerSchedule1.StartAsync(async token =>
            //{

            //});
            //await _fixedSchedule.StartAsync(async token =>
            //{
            //    this._routeInfoManager.ScanNotActiveBroker();
            //});
        }

        private void RegisterProcessor()
        {
            //TODO 
            //if (_nameServerOption.ClusterTest)
            //{

            //}
            //else
            //{

            //}
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
            scheduledExecutorService.Dispose();
        }
    }
}