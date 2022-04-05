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
using OpenNetQ.Broker.MqTrace;
using OpenNetQ.Broker.Offset;
using OpenNetQ.Broker.Out;
using OpenNetQ.Broker.Processor;
using OpenNetQ.Broker.Slave;
using OpenNetQ.Broker.Stats;
using OpenNetQ.Broker.Subscription;
using OpenNetQ.Broker.Topic;
using OpenNetQ.Common.Constant;
using OpenNetQ.Common.Options;
using OpenNetQ.Common.Protocol;
using OpenNetQ.Remoting.Abstractions;
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
        private  readonly IRemotingServer _remotingServer;
        private readonly FixedSchedule _registerBrokerAllFixedSchedule = new FixedSchedule("BrokerControllerFixedSchedule", TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));

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


        private readonly ICollection<ISendMessageHook> sendMessageHooks = new List<ISendMessageHook>();
        private readonly ICollection<IConsumeMessageHook> consumeMessageHooks = new List<IConsumeMessageHook>();

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
            _remotingServer = GetRequiredService<IRemotingServer>();
        }

        public void Initialize()
        {
            if (_brokerOption.EnableDLegerCommitLog)
            {
                throw new NotImplementedException("DLegerCommitLog");
            }
            //TODO  MESSAGE STORE load plugin
            _messageStore.GetDispatcherList().AddFirst(GetRequiredService<CommitLogDispatcherCalcBitMap>());

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

        public void RegisterProcessor()
        {
            //SendMessageProcessor
            var sendMessageProcessor = GetRequiredService<ISendMessageProcessor>();
            sendMessageProcessor.RegisterSendMessageHook(sendMessageHooks);
            sendMessageProcessor.RegisterConsumeMessageHook(consumeMessageHooks);

            _remotingServer.RegisterProcessor(RequestCode.SEND_MESSAGE, sendMessageProcessor, _sendMessageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.SEND_MESSAGE_V2, sendMessageProcessor, this._sendMessageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.SEND_BATCH_MESSAGE, sendMessageProcessor, this._sendMessageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.CONSUMER_SEND_MSG_BACK, sendMessageProcessor, this._sendMessageScheduler);

           //PullMessageProcessor
            _remotingServer.RegisterProcessor(RequestCode.PULL_MESSAGE, _pullMessageProcessor, _pullMessageScheduler);
            _pullMessageProcessor.registerConsumeMessageHook(consumeMessageHookList);

            /**
             * ReplyMessageProcessor
             */
            ReplyMessageProcessor replyMessageProcessor = new ReplyMessageProcessor(this);
            replyMessageProcessor.registerSendMessageHook(sendMessageHookList);

            this.remotingServer.registerProcessor(RequestCode.SEND_REPLY_MESSAGE, replyMessageProcessor, replyMessageExecutor);
            this.remotingServer.registerProcessor(RequestCode.SEND_REPLY_MESSAGE_V2, replyMessageProcessor, replyMessageExecutor);
            this.fastRemotingServer.registerProcessor(RequestCode.SEND_REPLY_MESSAGE, replyMessageProcessor, replyMessageExecutor);
            this.fastRemotingServer.registerProcessor(RequestCode.SEND_REPLY_MESSAGE_V2, replyMessageProcessor, replyMessageExecutor);

            /**
             * QueryMessageProcessor
             */
            NettyRequestProcessor queryProcessor = new QueryMessageProcessor(this);
            this.remotingServer.registerProcessor(RequestCode.QUERY_MESSAGE, queryProcessor, this.queryMessageExecutor);
            this.remotingServer.registerProcessor(RequestCode.VIEW_MESSAGE_BY_ID, queryProcessor, this.queryMessageExecutor);

            this.fastRemotingServer.registerProcessor(RequestCode.QUERY_MESSAGE, queryProcessor, this.queryMessageExecutor);
            this.fastRemotingServer.registerProcessor(RequestCode.VIEW_MESSAGE_BY_ID, queryProcessor, this.queryMessageExecutor);

            /**
             * ClientManageProcessor
             */
            ClientManageProcessor clientProcessor = new ClientManageProcessor(this);
            this.remotingServer.registerProcessor(RequestCode.HEART_BEAT, clientProcessor, this.heartbeatExecutor);
            this.remotingServer.registerProcessor(RequestCode.UNREGISTER_CLIENT, clientProcessor, this.clientManageExecutor);
            this.remotingServer.registerProcessor(RequestCode.CHECK_CLIENT_CONFIG, clientProcessor, this.clientManageExecutor);

            this.fastRemotingServer.registerProcessor(RequestCode.HEART_BEAT, clientProcessor, this.heartbeatExecutor);
            this.fastRemotingServer.registerProcessor(RequestCode.UNREGISTER_CLIENT, clientProcessor, this.clientManageExecutor);
            this.fastRemotingServer.registerProcessor(RequestCode.CHECK_CLIENT_CONFIG, clientProcessor, this.clientManageExecutor);

            /**
             * ConsumerManageProcessor
             */
            ConsumerManageProcessor consumerManageProcessor = new ConsumerManageProcessor(this);
            this.remotingServer.registerProcessor(RequestCode.GET_CONSUMER_LIST_BY_GROUP, consumerManageProcessor, this.consumerManageExecutor);
            this.remotingServer.registerProcessor(RequestCode.UPDATE_CONSUMER_OFFSET, consumerManageProcessor, this.consumerManageExecutor);
            this.remotingServer.registerProcessor(RequestCode.QUERY_CONSUMER_OFFSET, consumerManageProcessor, this.consumerManageExecutor);

            this.fastRemotingServer.registerProcessor(RequestCode.GET_CONSUMER_LIST_BY_GROUP, consumerManageProcessor, this.consumerManageExecutor);
            this.fastRemotingServer.registerProcessor(RequestCode.UPDATE_CONSUMER_OFFSET, consumerManageProcessor, this.consumerManageExecutor);
            this.fastRemotingServer.registerProcessor(RequestCode.QUERY_CONSUMER_OFFSET, consumerManageProcessor, this.consumerManageExecutor);

            /**
             * EndTransactionProcessor
             */
            this.remotingServer.registerProcessor(RequestCode.END_TRANSACTION, new EndTransactionProcessor(this), this.endTransactionExecutor);
            this.fastRemotingServer.registerProcessor(RequestCode.END_TRANSACTION, new EndTransactionProcessor(this), this.endTransactionExecutor);

            /**
             * Default
             */
            AdminBrokerProcessor adminProcessor = new AdminBrokerProcessor(this);
            this.remotingServer.registerDefaultProcessor(adminProcessor, this.adminBrokerExecutor);
            this.fastRemotingServer.registerDefaultProcessor(adminProcessor, this.adminBrokerExecutor);
            
            
            _registerBrokerAllFixedSchedule.StartAsync()
        }

        public void RegisterBrokerAll(bool checkOrderConfig, bool oneway, bool forceRegister)
        {
            var topicConfigWrapper = _topicConfigManager.BuildTopicConfigSerializeWrapper();
            if(!PermName.IsWriteable(_brokerOption))
        }
    }
}
