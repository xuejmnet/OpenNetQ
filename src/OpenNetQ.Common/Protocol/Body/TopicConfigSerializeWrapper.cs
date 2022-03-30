using System;
using System.Collections.Concurrent;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.Protocol.Body
{
    public class TopicConfigSerializeWrapper
    {
        public ConcurrentDictionary<string, TopicConfig> TopicConfigTable = new ConcurrentDictionary<string, TopicConfig>();
        public DataVersion DataVersion = new DataVersion();
    }
}