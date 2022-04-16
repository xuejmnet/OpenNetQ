using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Client.Exceptions;
using OpenNetQ.Client.Impls.Factory;
using OpenNetQ.Common;
using OpenNetQ.Common.Messages;
using OpenNetQ.Extensions;

namespace OpenNetQ.Client.Impls
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 14:36:12
    /// Email: 326308290@qq.com
    public class MQAdminImpl:IMQAdmin
    {
        private readonly MQClientInstance _mqClientFactory;
        public long TimeoutMillis { get; set; } = 6000;

        public MQAdminImpl(MQClientInstance mqClientFactory)
        {
            _mqClientFactory = mqClientFactory;
        }

        public Task CreateTopicAsync(string key, string newTopic, int queueNum)
        {
            return CreateTopicAsync(key, newTopic, queueNum, 0);
        }

        public async Task CreateTopicAsync(string key, string newTopic, int queueNum, int topicSysFlag)
        {
            try
            {
                Validators.CheckTopic(newTopic);
                Validators.IsSystemTopic(newTopic);
                var topicRouteData = await _mqClientFactory.GetMQClientAPIImpl().GetTopicRouteInfoFromNameServerAsync(key, TimeoutMillis));
                var brokerDataList = topicRouteData.BrokerDatas;
                if (brokerDataList.IsNotEmpty())
                {
                    brokerDataList = brokerDataList!.OrderBy(o => o).ToList();
                    bool createOKAtLeastOnce = false;
                    MQClientException? exception = null;
                    var orderTopicString = new StringBuilder();
                    foreach (var brokerData in brokerDataList)
                    {
                        if (brokerData.BrokerAddrs!.TryGetValue(MixAll.MASTER_ID, out var addr))
                        {
                            var topicConfig = new TopicConfig(newTopic);
                            topicConfig.ReadQueueNums = queueNum;
                            topicConfig.WriteQueueNums = queueNum;
                            topicConfig.TopicSysFlag = topicSysFlag;
                            bool createOK = false;
                            for (int i = 0; i < 5; i++)
                            {
                                try
                                {
                                    _mqClientFactory.GetMQClientAPIImpl().
                                }
                                catch (Exception e)
                                {
                                    if (4 == i)
                                    {
                                        exception = new MQClientException("create topic to broker exception", e);
                                    }
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
    }
}
