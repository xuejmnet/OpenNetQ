using Microsoft.Extensions.Logging;
using OpenNetQ.Client.Impls.Producers;
using OpenNetQ.Client.Traces;
using OpenNetQ.Common;
using OpenNetQ.Common.Messages;
using OpenNetQ.Common.Protocol;
using OpenNetQ.Common.Topic;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Abstractions;

namespace OpenNetQ.Client.Producers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 10:58:55
    /// Email: 326308290@qq.com
    public class DefaultMQProducer:ClientConfig,IMQProducer
    {
        private static ILogger<DefaultMQProducer> _logger = OpenNetQLoggerFactory.CreateLogger<DefaultMQProducer>();
        private readonly DefaultMQProducerImpl _defaultMQProducerImpl;

        private readonly ISet<int> _retryResponseCodes = new HashSet<int>()
        {
            ResponseCode.TOPIC_NOT_EXIST,
            ResponseCode.SERVICE_NOT_AVAILABLE,
            ResponseCode.SYSTEM_ERROR,
            ResponseCode.NO_PERMISSION,
            ResponseCode.NO_BUYER_ID,
            ResponseCode.NOT_IN_CURRENT_UNIT
        };

        private string _producerGroup;
        private string _createTopicKey = TopicValidator.AUTO_CREATE_TOPIC_KEY_TOPIC;
        private volatile int _defaultTopicQueueNums = 4;
        private int _sendMsgTimeout = 3000;
        private int _compressMsgBodyOverHowmuch = 1024 * 4;
        private int _retryTimesWhenSendFailed = 2;
        private int _retryTimesWhenSendAsyncFailed = 2;
        private bool _retryAnotherBrokerWhenNotStoreOk = false;
        private int _maxMessageSize = 1024 * 1024 * 4;

        private ITraceDispatcher? _traceDispatcher;

        public DefaultMQProducer():this(null,MixAll.DEFAULT_PRODUCER_GROUP,null)
        {
            
        }

        public DefaultMQProducer(IRPCHook rpcHook):this(null, MixAll.DEFAULT_PRODUCER_GROUP, rpcHook)
        {
            
        }

        public DefaultMQProducer(string producerGroup): this(null, producerGroup, null)
        {
            
        }

        public DefaultMQProducer(string? @namespace, string producerGroup, IRPCHook? rpcHook)
        {
            Namespace = @namespace;
            _producerGroup = producerGroup;
            _defaultMQProducerImpl = new DefaultMQProducerImpl(this, rpcHook);
        }

        public DefaultMQProducer(string producerGroup,bool enableMsgTrace):this(null,producerGroup,null,enableMsgTrace,null)
        {

        }
        public DefaultMQProducer(string producerGroup, bool enableMsgTrace, string customizedTraceTopic) : this(null, producerGroup, null, enableMsgTrace, customizedTraceTopic)
        {

        }
        public DefaultMQProducer(string producerGroup,IRPCHook rpcHook,bool enableMsgTrace,string customizedTraceTopic):this(null, producerGroup, rpcHook,enableMsgTrace, customizedTraceTopic)
        {
            
        }

        public DefaultMQProducer(string? @namespace, string producerGroup, IRPCHook? rpcHook, bool enableMsgTrace,
            string? customizedTraceTopic)
        {
            Namespace = @namespace;
            _producerGroup = producerGroup;
            _defaultMQProducerImpl = new DefaultMQProducerImpl(this, rpcHook);
            if (enableMsgTrace)
            {
                try
                {

                }
                catch (Exception ex)
                {

                }
            }

        }

        public override void SetUseTLS(bool useTLS)
        {
            base.SetUseTLS(useTLS);
            //TODO
            if (_traceDispatcher != null)
            {

            }
        }
        public async Task StartAsync()
        {
            _producerGroup = WithNamespace(_producerGroup);
           _defaultMQProducerImpl.Start();
           if (null != _traceDispatcher)
           {

           }
        }
        public async Task StopAsync()
        {
            _defaultMQProducerImpl.Stop();
            if (null != _traceDispatcher)
            {

            }
        }

        public Task<List<MessageQueue>> FetchPublishMessageQueuesAsync(string topic)
        {
            return _defaultMQProducerImpl.FetchPublishMessageQueue(topic);
        }


        public async Task CreateTopicAsync(string key, string newTopic, int queueNum, int topicSysFlag)
        {
            return _defaultMQProducerImpl.
        }

        public Task<long> SearchOffsetAsync(MessageQueue queue, long timestamp)
        {
            throw new NotImplementedException();
        }

        public Task<long> MaxOffsetAsync(MessageQueue queue)
        {
            throw new NotImplementedException();
        }

        public Task<long> MinOffsetAsync(MessageQueue queue)
        {
            throw new NotImplementedException();
        }

        public Task<long> EarliestMsgStoreTimeAsync(MessageQueue queue)
        {
            throw new NotImplementedException();
        }

        public Task<MessageExt> ViewMessageAsync(string offsetMsgId)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult> QueryMessage(string topic, string key, int maxNum, long begin, long end)
        {
            throw new NotImplementedException();
        }

        public Task<MessageExt> ViewMessage(string topic, string msgId)
        {
            throw new NotImplementedException();
        }


        public Task<SendResult> SendAsync(Message msg)
        {
            throw new NotImplementedException();
        }

        public Task<SendResult> SendAsync(MessageQueue msg, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task<SendResult> SendAsync(Message msg, MessageQueue mq)
        {
            throw new NotImplementedException();
        }

        public Task<SendResult> SendAsync(Message msg, MessageQueue mq, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task SendCallbackAsync(Message msg, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException)
        {
            throw new NotImplementedException();
        }

        public Task SendCallbackAsync(Message msg, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task SendCallbackAsync(Message msg, Func<List<MessageQueue>, Message, object, MessageQueue> messageQueueSelector, object arg, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException)
        {
            throw new NotImplementedException();
        }

        public Task SendCallbackAsync(Message msg, Func<List<MessageQueue>, Message, object, MessageQueue> messageQueueSelector, object arg, long timeoutMillis, Func<SendResult, Task>? onSuccess,
            Func<Exception, Task>? onException)
        {
            throw new NotImplementedException();
        }

        public Task SendOnewayAsync(MessageQueue msg)
        {
            throw new NotImplementedException();
        }

        public Task SendOnewayAsync(Message msg, Func<List<MessageQueue>, Message, object, MessageQueue> messageQueueSelector, object arg)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionSendResult> SendMessageInTransactionAsync(Message msg, Func<Message, object, LocalTransactionStateEnum>? executeLocalTransaction, Func<MessageExt, LocalTransactionStateEnum>? checkLocalTransaction, object arg)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionSendResult> SendMessageInTransaction(Message msg, object args)
        {
            throw new NotImplementedException();
        }

        public Task<SendResult> SendAsync(ICollection<Message> msgs)
        {
            throw new NotImplementedException();
        }

        public Task<SendResult> SendAsync(ICollection<Message> msgs, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task<SendResult> SendAsync(ICollection<Message> msgs, MessageQueue mq)
        {
            throw new NotImplementedException();
        }

        public Task<SendResult> SendAsync(ICollection<Message> msgs, MessageQueue mq, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(ICollection<Message> msgs, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(ICollection<Message> msgs, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(ICollection<Message> msgs, MessageQueue mq, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(ICollection<Message> msgs, MessageQueue mq, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task<Message> RequestAsync(Message msg, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task RequestAsync(Message msg, Func<Message, Task>? onSuccess, Func<Exception, Task>? onException, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task RequestAsync(Message msg, Func<List<MessageQueue>, Message, object, MessageQueue> messageQueueSelector, object arg, Func<Message, Task>? onSuccess, Func<Exception, Task>? onException,
            long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task<Message> RequestAsync(Message msg, MessageQueue mq, long timeoutMillis)
        {
            throw new NotImplementedException();
        }

        public Task RequestAsync(Message msg, MessageQueue mq, Func<Message, Task>? onSuccess, Func<Exception, Task>? onException, long timeoutMillis)
        {
            throw new NotImplementedException();
        }
    }
}
