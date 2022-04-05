using System;
using System.Collections.Concurrent;
using OpenNetQ.Common;
using OpenNetQ.Common.Protocol.Body;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Broker.Topic
{
    public class TopicConfigManager:ITopicConfigManager
    {
        private readonly ConcurrentDictionary<string, TopicConfig> _topicConfigTable = new(Environment.ProcessorCount,1024);
        private readonly DataVersion _dataVersion = new();
        
        public TopicConfigSerializeWrapper BuildTopicConfigSerializeWrapper()
        {
            var topicConfigSerializeWrapper = new TopicConfigSerializeWrapper();
            topicConfigSerializeWrapper.TopicConfigTable = _topicConfigTable;
            topicConfigSerializeWrapper.DataVersion = _dataVersion;
            return topicConfigSerializeWrapper;
        }
    }
}