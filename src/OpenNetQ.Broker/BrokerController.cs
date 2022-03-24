using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenNetQ.Broker.Client;
using OpenNetQ.Broker.Client.Net;
using OpenNetQ.Broker.Filter;
using OpenNetQ.Broker.FilterServer;
using OpenNetQ.Broker.Latency;
using OpenNetQ.Broker.Longpolling;
using OpenNetQ.Broker.Offset;
using OpenNetQ.Broker.Out;
using OpenNetQ.Broker.Processor;
using OpenNetQ.Broker.Slave;
using OpenNetQ.Broker.Stats;
using OpenNetQ.Broker.Subscription;
using OpenNetQ.Broker.Topic;
using OpenNetQ.Common.Options;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Store;
using OpenNetQ.Store.Common;
using OpenNetQ.TaskSchedulers;

namespace OpenNetQ.Broker
{
    public class BrokerController: InjectService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BrokerController> _logger;
        private readonly LoggerFactory _loggerFactory;
        private readonly BrokerOption _brokerOption;
        private readonly RemotingServerOption _remotingServerOption;
        private readonly RemotingClientOption _remotingClientOption;
        private readonly MessageStoreOption _messageStoreOption;
        private readonly IConsumerOffsetManager _consumerOffsetManager;
        private readonly ITopicConfigManager _topicConfigManager;
        private readonly IPullMessageProcessor _pullMessageProcessor;
        private readonly IPullRequestHoldService _pullRequestHoldService;
        private readonly INotifyMessageArrivingListener _notifyMessageArrivingListener;
        private readonly IConsumerIdsChangeListener _consumerIdsChangeListener;
        private readonly IConsumerManager _consumerManager;
        private readonly IConsumerFilterManager _consumerFilterManager;
        private readonly IProducerManager _producerManager;
        private readonly IClientHousekeepingService _clientHousekeepingService;
        private readonly IBroker2Client _broker2Client;
        private readonly ISubscriptionGroupManager _subscriptionGroupManager;
        private readonly IBrokerOuterAPI _brokerOuterApi;
        private readonly IFilterServerManager _filterServerManager;
        private readonly ISlaveSynchronize _slaveSynchronize;
        private readonly IBrokerStatsManager _brokerStatsManager;
        private readonly IBrokerFastFailure _brokerFastFailure;
        private  readonly IMessageStore _messageStore;

        #region alone thread pool

        private  OpenNetQTaskScheduler _sendMessageScheduler;
        private  OpenNetQTaskScheduler _pushMessageScheduler;
        private  OpenNetQTaskScheduler _pullMessageScheduler;
        private  OpenNetQTaskScheduler _processReplyMessageScheduler;
        private  OpenNetQTaskScheduler _queryMessageScheduler;
        private  OpenNetQTaskScheduler _adminBrokerScheduler;
        private  OpenNetQTaskScheduler _clientManageScheduler;
        private  OpenNetQTaskScheduler _heartbeatScheduler;
        private  OpenNetQTaskScheduler _endTransactionScheduler;
        private  OpenNetQTaskScheduler _consumerManageScheduler;
        #endregion

        public BrokerController(IServiceProvider serviceProvider):base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _loggerFactory = GetRequiredService<LoggerFactory>();
            _logger = _loggerFactory.CreateLogger<BrokerController>();
            _brokerOption = GetRequiredService<BrokerOption>();
            _remotingServerOption = GetRequiredService<RemotingServerOption>();
            _remotingClientOption = GetRequiredService<RemotingClientOption>();
            _messageStoreOption = GetRequiredService<MessageStoreOption>();
            _consumerOffsetManager = GetRequiredService<IConsumerOffsetManager>();
            _topicConfigManager = GetRequiredService<ITopicConfigManager>();
            _pullMessageProcessor = GetRequiredService<IPullMessageProcessor>();
            _pullRequestHoldService = GetRequiredService<IPullRequestHoldService>();
            _notifyMessageArrivingListener = GetRequiredService<INotifyMessageArrivingListener>();
            _consumerIdsChangeListener = GetRequiredService<IConsumerIdsChangeListener>();
            _consumerManager = GetRequiredService<IConsumerManager>();
            _consumerFilterManager = GetRequiredService<IConsumerFilterManager>();
            _producerManager = GetRequiredService<IProducerManager>();
            _clientHousekeepingService = GetRequiredService<IClientHousekeepingService>();
            _broker2Client = GetRequiredService<IBroker2Client>();
            _subscriptionGroupManager = GetRequiredService<ISubscriptionGroupManager>();
            _brokerOuterApi = GetRequiredService<IBrokerOuterAPI>();
            _filterServerManager = GetRequiredService<IFilterServerManager>();
            _slaveSynchronize = GetRequiredService<ISlaveSynchronize>();
            _brokerStatsManager = GetRequiredService<IBrokerStatsManager>();
            _brokerFastFailure = GetRequiredService<IBrokerFastFailure>();
            _messageStore = GetRequiredService<IMessageStore>();
        }

        public void Initialize()
        {
            _sendMessageScheduler = new OpenNetQTaskScheduler(_brokerOption.SendMessageThreadPoolCount, _brokerOption.SendMessageThreadPoolCount, 60 * 1000, "SendMessageThread_");
            _pushMessageScheduler = new OpenNetQTaskScheduler(_brokerOption.PushMessageThreadPoolCount, _brokerOption.PushMessageThreadPoolCount, 60 * 1000, "PushMessageThread_");
            _pullMessageScheduler = new OpenNetQTaskScheduler(_brokerOption.PullMessageThreadPoolCount, _brokerOption.PullMessageThreadPoolCount, 60 * 1000, "PullMessageThread_");
            _processReplyMessageScheduler = new OpenNetQTaskScheduler(_brokerOption.PullMessageThreadPoolCount, _brokerOption.PullMessageThreadPoolCount, 60 * 1000, "ProcessReplyMessageThread_");
            _queryMessageScheduler = new OpenNetQTaskScheduler(_brokerOption.QueryMessageThreadPoolCount, _brokerOption.QueryMessageThreadPoolCount, 60 * 1000, "QueryMessageThread_");
            _adminBrokerScheduler = new OpenNetQTaskScheduler(_brokerOption.AdminBrokerThreadPoolCount, _brokerOption.AdminBrokerThreadPoolCount, 60 * 1000, "AdminBrokerThread_");
            _clientManageScheduler = new OpenNetQTaskScheduler(_brokerOption.ClientManageThreadPoolCount, _brokerOption.ClientManageThreadPoolCount, 60 * 1000, "ClientManageThread_");
            _heartbeatScheduler = new OpenNetQTaskScheduler(_brokerOption.HeartbeatThreadPoolCount, _brokerOption.HeartbeatThreadPoolCount, 60 * 1000, "HeartbeatThread_");
            _endTransactionScheduler = new OpenNetQTaskScheduler(_brokerOption.EndTransactionThreadPoolCount, _brokerOption.EndTransactionThreadPoolCount, 60 * 1000, "EndTransactionThread_");
            _consumerManageScheduler = new OpenNetQTaskScheduler(_brokerOption.ConsumerManageThreadPoolCount, _brokerOption.ConsumerManageThreadPoolCount, 60 * 1000, "ConsumerManageThread_");

        }

        public Task StartAsync()
        {
            _loggerFactory
        }
    }
}
