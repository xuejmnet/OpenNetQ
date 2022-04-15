using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Client.Exceptions;
using OpenNetQ.Common.Messages;
using OpenNetQ.Remoting.Exceptions;

namespace OpenNetQ.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/15 10:50:42
    /// Email: 326308290@qq.com
    public interface IMQAdmin
    {
        /// <summary>
        /// 创建一个主题
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newTopic"></param>
        /// <param name="queueNum"></param>
        /// <param name="topicSysFlag"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task CreateTopicAsync(string key, string newTopic, int queueNum, int topicSysFlag);
        /// <summary>
        /// 搜索某个时间的队列偏移量
        /// 注意调用小心 IO开销很大
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task<long> SearchOffsetAsync(MessageQueue queue, long timestamp);
        /// <summary>
        /// 获取最大偏移量
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task<long> MaxOffsetAsync(MessageQueue queue);
        /// <summary>
        /// 获取最小偏移量
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task<long> MinOffsetAsync(MessageQueue queue);

        /// <summary>
        /// 获取最早的消息存储时间
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        Task<long> EarliestMsgStoreTimeAsync(MessageQueue queue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetMsgId"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<MessageExt> ViewMessageAsync(string offsetMsgId);
        /// <summary>
        /// 查询消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="key"></param>
        /// <param name="maxNum"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task<QueryResult> QueryMessage(string topic,string key,int maxNum,long begin,long end);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="msgId"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        /// <exception cref="RemotingException"></exception>
        /// <exception cref="MQBrokerException"></exception>
        Task<MessageExt> ViewMessage(string topic, string msgId);
    }
}
