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
using OpenNetQ.Broker.Subscription;
using OpenNetQ.Broker.Topic;
using OpenNetQ.Broker.Transaction;
using OpenNetQ.Common;
using OpenNetQ.Common.Constant;
using OpenNetQ.Common.Options;
using OpenNetQ.Common.Protocol;
using OpenNetQ.Common.Protocol.Body;
using OpenNetQ.Extensions;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Store;
using OpenNetQ.Store.Common;
using OpenNetQ.Store.Stats;
using OpenNetQ.TaskSchedulers;

namespace OpenNetQ.Broker
{
    public class BrokerController : InjectService
    {
        private static readonly ILogger _logger = OpenNetQLoggerFactory.CreateLogger<BrokerController>();
        private readonly IServiceProvider _serviceProvider;
        private readonly BrokerOption _brokerOption;
        private readonly RemotingServerOption _remotingServerOption;
        private readonly RemotingClientOption _remotingClientOption;
        private readonly MessageStoreOption _messageStoreOption;
        private readonly IConsumerOffsetManager _consumerOffsetManager;
        private readonly ITopicConfigManager _topicConfigManager;
        private readonly IPullMessageProcessor _pullMessageProcessor;
        private readonly IReplyMessageProcessor _replyMessageProcessor;
        private readonly IQueryMessageProcessor _queryMessageProcessor;
        private readonly IClientManageProcessor _clientManageProcessor;
        private readonly IConsumerManageProcessor _consumerManageProcessor;
        private readonly IEndTransactionProcessor _endTransactionProcessor;
        private readonly IAdminBrokerProcessor _adminBrokerProcessor;
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
        private bool _updateMasterHAServerAddrPeriodically = false;
        private readonly IBrokerStats _brokerStats;
        private readonly IBrokerFastFailure _brokerFastFailure;
        private readonly IMessageStore _messageStore;
        private readonly IRemotingServer _remotingServer;
        private readonly OpenNetQTaskScheduler _scheduler = new OpenNetQTaskScheduler(1, "BrokerControllerScheduledTask");
        private TransactionalMessageCheckService _transactionalMessageCheckService;

        #region alone thread pool

        private OpenNetQTaskScheduler _sendMessageScheduler;
        private OpenNetQTaskScheduler _pushMessageScheduler;
        private OpenNetQTaskScheduler _pullMessageScheduler;
        private OpenNetQTaskScheduler _processReplyMessageScheduler;
        private OpenNetQTaskScheduler _queryMessageScheduler;
        private OpenNetQTaskScheduler _adminBrokerScheduler;
        private OpenNetQTaskScheduler _clientManageScheduler;
        private OpenNetQTaskScheduler _heartbeatScheduler;
        private OpenNetQTaskScheduler _endTransactionScheduler;
        private OpenNetQTaskScheduler _consumerManageScheduler;
        #endregion





        private readonly ICollection<ISendMessageHook> sendMessageHooks = new List<ISendMessageHook>();
        private readonly ICollection<IConsumeMessageHook> consumeMessageHooks = new List<IConsumeMessageHook>();

        public BrokerController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _brokerOption = GetRequiredService<BrokerOption>();
            _remotingServerOption = GetRequiredService<RemotingServerOption>();
            _remotingClientOption = GetRequiredService<RemotingClientOption>();
            _messageStoreOption = GetRequiredService<MessageStoreOption>();
            _consumerOffsetManager = GetRequiredService<IConsumerOffsetManager>();
            _topicConfigManager = GetRequiredService<ITopicConfigManager>();
            _pullMessageProcessor = GetRequiredService<IPullMessageProcessor>();
            _replyMessageProcessor = GetRequiredService<IReplyMessageProcessor>();
            _queryMessageProcessor = GetRequiredService<IQueryMessageProcessor>();
            _clientManageProcessor = GetRequiredService<IClientManageProcessor>();
            _consumerManageProcessor = GetRequiredService<IConsumerManageProcessor>();
            _endTransactionProcessor = GetRequiredService<IEndTransactionProcessor>();
            _adminBrokerProcessor = GetRequiredService<IAdminBrokerProcessor>();
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
            _brokerStats = GetRequiredService<IBrokerStats>();
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

            RegisterProcessor();

            var initialDelay = DateTime.Now.AddDays(1).Date.Subtract(DateTime.Now);

            _scheduler.RunFixedRate(() =>
            {
                try
                {
                    _brokerStats.Record();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, " schedule record error.");
                }
            }, initialDelay, TimeSpan.FromDays(1));

            _scheduler.RunFixedRate(() =>
            {
                try
                {
                    _consumerOffsetManager.Persist();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "schedule persist consumerOffset error.");
                }
            }, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(_brokerOption.FlushConsumerOffsetInterval));


            _scheduler.RunFixedRate(() =>
            {
                try
                {
                    _consumerFilterManager.Persist();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "schedule persist consumer filter error.");
                }
            }, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

            _scheduler.RunFixedRate(() =>
            {
                try
                {
                    ProtectBroker();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ProtectBroker error.");
                }
            }, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3));
            _scheduler.RunFixedRate(() =>
            {
                try
                {
                    PrintWaterMark();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "PrintWaterMark error.");
                }
            }, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1)); 
            _scheduler.RunFixedRate(() =>
            {
                try
                {
                    _logger.LogInformation($"dispatch behind commit log {_messageStore.DispatchBehindBytes()} bytes");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "schedule dispatchBehindBytes error.");
                }
            }, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(60));

            if (_brokerOption.NameServerAddress != null)
            {
                _brokerOuterApi.UpdateNameServerAddressList(_brokerOption.NameServerAddress);
                _logger.LogInformation($"Set user specified name server address: {_brokerOption.NameServerAddress}");
            }
            else if(_brokerOption.IsFetchNamesrvAddrByAddressServer)
            {
                _scheduler.RunFixedRate(() =>
                {
                    try
                    {
                        _brokerOuterApi.FetchNameServerAddr();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "cheduledTask fetchNameServerAddr exception");
                    }
                }, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(2));
            }

            if (_messageStoreOption.EnableDLegerCommitLog)
            {
                //TODO
            }

            InitialTransaction();
            InitialAcl();
            InitialRpcHooks();
        }

        public void ProtectBroker()
        {
            //TODO
        }

        public void PrintWaterMark()
        {
            //TODO
        }
        public Task StartAsync()
        {
            return Task.CompletedTask;
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
            _pullMessageProcessor.RegisterConsumeMessageHook(consumeMessageHooks);

            //ReplyMessageProcessor
            _replyMessageProcessor.RegisterSendMessageHook(sendMessageHooks);
            _remotingServer.RegisterProcessor(RequestCode.SEND_REPLY_MESSAGE, _replyMessageProcessor, _processReplyMessageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.SEND_REPLY_MESSAGE_V2, _replyMessageProcessor, _processReplyMessageScheduler);

            //QueryMessageProcessor
            _remotingServer.RegisterProcessor(RequestCode.QUERY_MESSAGE, _queryMessageProcessor, _queryMessageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.VIEW_MESSAGE_BY_ID, _queryMessageProcessor, _queryMessageScheduler);

            //ClientManageProcessor
            _remotingServer.RegisterProcessor(RequestCode.HEART_BEAT, _clientManageProcessor, _clientManageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.UNREGISTER_CLIENT, _clientManageProcessor, _clientManageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.CHECK_CLIENT_CONFIG, _clientManageProcessor, _clientManageScheduler);


            //ConsumerManageProcessor
            _remotingServer.RegisterProcessor(RequestCode.GET_CONSUMER_LIST_BY_GROUP, _consumerManageProcessor, _consumerManageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.UPDATE_CONSUMER_OFFSET, _consumerManageProcessor, _consumerManageScheduler);
            _remotingServer.RegisterProcessor(RequestCode.QUERY_CONSUMER_OFFSET, _consumerManageProcessor, _consumerManageScheduler);

            //EndTransactionProcessor
            _remotingServer.RegisterProcessor(RequestCode.END_TRANSACTION, _endTransactionProcessor, _endTransactionScheduler);

            //Default
            _remotingServer.RegisterDefaultProcessor(_adminBrokerProcessor, _adminBrokerScheduler);


            _scheduler.RunFixedRate(() =>
            {
                try
                {
                    Console.WriteLine("RegisterBrokerAll run fixed rate start");
                    RegisterBrokerAll(true, false, _brokerOption.IsForceRegister);
                    Console.WriteLine("RegisterBrokerAll run fixed rate end");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "RegisterBrokerAll Exception");
                }
            }, TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(Math.Max(10000, Math.Min(_brokerOption.RegisterNameServerPeriod, 60000))));

        }

        private readonly object _registerBrokerAllLock = new();
        public void RegisterBrokerAll(bool checkOrderConfig, bool oneway, bool forceRegister)
        {
            lock (_registerBrokerAllLock)
            {
                var topicConfigWrapper = _topicConfigManager.BuildTopicConfigSerializeWrapper();
                if (!PermName.IsWriteable(_brokerOption.BrokerPermission)
                    || !PermName.IsReadable(_brokerOption.BrokerPermission))
                {
                    ConcurrentDictionary<string, TopicConfig> topicConfigTable = new ConcurrentDictionary<string, TopicConfig>();
                    foreach (var topicConfig in topicConfigWrapper.TopicConfigTable.Values)
                    {
                        var tc = new TopicConfig(topicConfig.TopicName, topicConfig.ReadQueueNums, topicConfig.WriteQueueNums, _brokerOption.BrokerPermission);
                        topicConfigTable.TryAdd(tc.TopicName, tc);
                    }

                    topicConfigWrapper.TopicConfigTable = topicConfigTable;
                }

                if (forceRegister || NeedRegister(_brokerOption.BrokerClusterName, GetBrokerAddr(),
                        _brokerOption.BrokerName,
                        _brokerOption.BrokerId, _brokerOption.RegisterBrokerTimeoutMills))
                {
                    DoRegisterBrokerAll(checkOrderConfig, oneway, topicConfigWrapper);
                }
            }
        }

        private void DoRegisterBrokerAll(bool checkOrderConfig, bool oneway,
            TopicConfigSerializeWrapper topicConfigWrapper)
        {
            //TODO
            var registerBrokerResultList = _brokerOuterApi.RegisterBrokerAll(
                _brokerOption.BrokerClusterName,
                GetBrokerAddr(),
                _brokerOption.BrokerName,
                _brokerOption.BrokerId,
                GetHAServerAddr(),
                topicConfigWrapper,
                _filterServerManager.BuildNewFilterServerList(),
                oneway,
                _brokerOption.RegisterBrokerTimeoutMills,
                _brokerOption.IsCompressedRegister
            );
            if (registerBrokerResultList.Any())
            {
                var registerBrokerResult = registerBrokerResultList.First();
                if (_updateMasterHAServerAddrPeriodically && registerBrokerResult.HAServerAddr is not null)
                {
                    _messageStore.UpdateHaMasterAddress(registerBrokerResult.HAServerAddr);
                }

                _slaveSynchronize.SetMasterAddr(registerBrokerResult.MasterAddr);
                if (checkOrderConfig)
                {
                    _topicConfigManager.UpdateOrderTopicConfig(registerBrokerResult.KVTable);
                }
            }
        }

        private bool NeedRegister(string clusterName, string brokerAddr, string brokerName, long brokerId,
            int timeoutMills)
        {
            var topicConfigWrapper = _topicConfigManager.BuildTopicConfigSerializeWrapper();
            var needRegister = _brokerOuterApi.NeedRegister(clusterName, brokerAddr, brokerName, brokerId, topicConfigWrapper, timeoutMills);
            return needRegister.Any(o => o);
        }

        public string GetBrokerAddr()
        {
            return $"{_brokerOption.BrokerIP1}:{_remotingServerOption.Port}";
        }

        public string GetHAServerAddr()
        {
            return $"{_brokerOption.BrokerIP2}:{_messageStoreOption.HAPort}";
        }

        private void InitialTransaction()
        {
            //TODO InitialTransaction
        }

        private void InitialAcl()
        {
            //TODO InitialAcl
        }

        private void InitialRpcHooks()
        {
            //TODO InitialRpcHooks
        }
    }
}
