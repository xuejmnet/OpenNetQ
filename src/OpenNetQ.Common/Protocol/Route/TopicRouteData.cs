

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/

using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Common.Protocol.Route
{
    public class TopicRouteData:RemotingSerializable
    {
        public string? OrderTopicConf { get; set; }
        public List<QueueData>? QueueDatas { get; set; }
        public List<BrokerData>? BrokerDatas { get; set; }
        public Dictionary<string /* brokerAddr */, List<string> /* Filter Server */>? FilterServerTables { get; set; }

        public TopicRouteData CloneTopicRouteData()
        {
            var topicRouteData = new TopicRouteData();
            topicRouteData.QueueDatas = new List<QueueData>();
            topicRouteData.BrokerDatas = new List<BrokerData>();
            topicRouteData.FilterServerTables = new Dictionary<string, List<string>>();
            topicRouteData.OrderTopicConf = OrderTopicConf;
            if (QueueDatas!=null)
            {
                topicRouteData.QueueDatas.AddRange(QueueDatas);
            }
            if (BrokerDatas != null)
            {
                topicRouteData.BrokerDatas.AddRange(BrokerDatas);
            }
            if (FilterServerTables != null)
            {
                foreach (var filterServerTable in FilterServerTables)
                {
                    topicRouteData.FilterServerTables.TryAdd(filterServerTable.Key, filterServerTable.Value);
                }
            }

            return topicRouteData;
        }
    }
}