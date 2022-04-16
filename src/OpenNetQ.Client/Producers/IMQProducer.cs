using OpenNetQ.Client.Exceptions;
using OpenNetQ.Common.Messages;
using OpenNetQ.Remoting.Exceptions;

namespace OpenNetQ.Client.Producers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 10:59:18
    /// Email: 326308290@qq.com
    public interface IMQProducer:IMQAdmin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task StartAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task StopAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task<List<MessageQueue>> FetchPublishMessageQueuesAsync(string topic);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<SendResult> SendAsync(Message msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timeoutMillis">毫秒</param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<SendResult> SendAsync(MessageQueue msg, long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="mq"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<SendResult> SendAsync(Message msg, MessageQueue mq);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="mq"></param>
        /// <param name="timeoutMillis"></param>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        /// <returns></returns>
        Task<SendResult> SendAsync(Message msg, MessageQueue mq, long timeoutMillis);
        #region 异步回调
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        Task SendCallbackAsync(Message msg, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        Task SendCallbackAsync(Message msg, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException, long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageQueueSelector"></param>
        /// <param name="arg"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        Task SendCallbackAsync(Message msg, Func<List<MessageQueue>, Message, object, MessageQueue> messageQueueSelector, object arg, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageQueueSelector"></param>
        /// <param name="arg"></param>
        /// <param name="timeoutMillis"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        Task SendCallbackAsync(Message msg, Func<List<MessageQueue>, Message, object, MessageQueue> messageQueueSelector, object arg, long timeoutMillis, Func<SendResult, Task>? onSuccess, Func<Exception, Task>? onException);

        #endregion
        #region 单向
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        Task SendOnewayAsync(MessageQueue msg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageQueueSelector"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        Task SendOnewayAsync(Message msg, Func<List<MessageQueue>, Message, object, MessageQueue> messageQueueSelector,
            object arg);

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="executeLocalTransaction"></param>
        /// <param name="checkLocalTransaction"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task<TransactionSendResult> SendMessageInTransactionAsync(Message msg,
            Func<Message, object, LocalTransactionStateEnum>? executeLocalTransaction,
            Func<MessageExt, LocalTransactionStateEnum>? checkLocalTransaction, object arg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task<TransactionSendResult> SendMessageInTransaction(Message msg, object args);
        #region batch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgs"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<SendResult> SendAsync(ICollection<Message> msgs);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<SendResult> SendAsync(ICollection<Message> msgs,long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="mq"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<SendResult> SendAsync(ICollection<Message> msgs, MessageQueue mq);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="mq"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<SendResult> SendAsync(ICollection<Message> msgs, MessageQueue mq,long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task SendAsync(ICollection<Message> msgs, Func<SendResult, Task>? onSuccess,
            Func<Exception, Task>? onException);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task SendAsync(ICollection<Message> msgs, Func<SendResult, Task>? onSuccess,
            Func<Exception, Task>? onException,long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="mq"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task SendAsync(ICollection<Message> msgs,MessageQueue mq, Func<SendResult, Task>? onSuccess,
            Func<Exception, Task>? onException);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="mq"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task SendAsync(ICollection<Message> msgs, MessageQueue mq, Func<SendResult, Task>? onSuccess,
            Func<Exception, Task>? onException,long timeoutMillis);

        #endregion

        #region rpc
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="RequestTimeoutException"></exception>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<Message> RequestAsync(Message msg,long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="RequestTimeoutException"></exception>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task RequestAsync(Message msg, Func<Message, Task>? onSuccess,
            Func<Exception, Task>? onException, long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageQueueSelector"></param>
        /// <param name="arg"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="RequestTimeoutException"></exception>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task RequestAsync(Message msg, Func<List<MessageQueue>, Message, object, MessageQueue> messageQueueSelector,
            object arg, Func<Message, Task>? onSuccess,
            Func<Exception, Task>? onException, long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="mq"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="RequestTimeoutException"></exception>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<Message> RequestAsync(Message msg,MessageQueue mq,long timeoutMillis);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="mq"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onException"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="RequestTimeoutException"></exception>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task RequestAsync(Message msg, MessageQueue mq, Func<Message, Task>? onSuccess,
            Func<Exception, Task>? onException, long timeoutMillis);

        #endregion
    }
}
