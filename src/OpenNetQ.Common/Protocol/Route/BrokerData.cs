using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.Protocol.Route
{
    public class BrokerData:IComparer<BrokerData>
    {
        private readonly Random _random = new Random();
        public string? Cluster { get; set; }
        public string? BrokerName { get; set; }
        public Dictionary<long, string>? BrokerAddrs { get; set; }

        public BrokerData()
        {
            
        }

        public BrokerData(string cluster, string brokerName, Dictionary<long, string> brokerAddrs)
        {
            Cluster = cluster;
            BrokerName = brokerName;
            BrokerAddrs = brokerAddrs;
        }

        public string? SelectBrokerAddr()
        {
            if (!BrokerAddrs!.TryGetValue(MixAll.MASTER_ID, out var addr))
            {
                var list = BrokerAddrs.Values.ToList();
                return list[_random.Next(list.Count)];
            }

            return addr;
        }
        public int Compare(BrokerData? x, BrokerData? y)
        {
            throw new NotImplementedException();
        }
    }
}