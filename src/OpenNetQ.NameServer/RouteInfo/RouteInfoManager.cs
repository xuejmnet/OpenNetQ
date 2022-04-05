using System;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using OpenNetQ.Common;
using OpenNetQ.Common.NameSrv;
using OpenNetQ.Common.Protocol.Body;
using OpenNetQ.Common.Protocol.Route;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.NameServer.RouteInfo
{
    public class RouteInfoManager : IRouteInfoManager
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RouteInfoManager> _logger;
        private readonly Dictionary<string /*Topic*/, List<QueueData>> _topicQueueTable;
        private readonly Dictionary<string /*BrokerName*/, BrokerData> _brokerAddrTable;
        private readonly Dictionary<string /*ClusterName*/, ISet<string/*BrokerName*/>> _clusterAddrTable;
        private readonly Dictionary<string /*BrokerAddr*/, BrokerLiveInfo> _brokerLiveTable;
        private readonly Dictionary<string /*BrokerAddr*/, List<string /*Filter Server*/>> _filterServerTable;

        public RouteInfoManager(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<RouteInfoManager>();
            _topicQueueTable = new(1024);
            _brokerAddrTable = new(128);
            _clusterAddrTable = new(32);
            _brokerLiveTable= new(256);
            _filterServerTable= new(256);
        }

        public byte[] GetAllClusterInfo()
        {
            throw new NotImplementedException();
        }

        public void DeleteTopic(string topic)
        {
            throw new NotImplementedException();
        }

        public byte[] GetAllTopicList()
        {
            throw new NotImplementedException();
        }

        public RegisterBrokerResult RegisterBroker(string clusterName, string brokerAddr, string brokerName, long brokerId, string haServerAddr, TopicConfigSerializeWrapper topicConfigSerializeWrapper, ICollection<string> filterList, IChannel channel)
        {
            throw new NotImplementedException();
        }

        public bool IsBrokerTopicConfig(string brokerAddr, DataVersion dataVersion)
        {
            throw new NotImplementedException();
        }

        public DataVersion QueryBrokerTopicConfig(string brokerAddr)
        {
            throw new NotImplementedException();
        }

        public void UpdateBrokerInfoUpdateTimestamp(string brokerAddr)
        {
            throw new NotImplementedException();
        }

        public void CreateAndUpdateQueueData(string brokerName, TopicConfig topicConfig)
        {
            throw new NotImplementedException();
        }

        public int WipeWritePermOfBrokerByLock(string brokerName)
        {
            throw new NotImplementedException();
        }

        public int AddWritePermOfBreakerByLock(string brokerName)
        {
            throw new NotImplementedException();
        }

        public int OperateWritePermOfBrokerByLock(string brokerName, int requestCode)
        {
            throw new NotImplementedException();
        }

        public int OperateWritePermOfBroker(string brokerName, string requestCode)
        {
            throw new NotImplementedException();
        }

        public void UnRegisterBroker(string clusterName, string brokerAddr, string brokerName, long brokerId)
        {
            throw new NotImplementedException();
        }

        public void RemoveTopicByBrokerName(string brokerName)
        {
            throw new NotImplementedException();
        }

        public TopicRouteData PickupTopicRouteData(string topic)
        {
            throw new NotImplementedException();
        }

        public void ScanNotActiveBroker()
        {
            throw new NotImplementedException();
        }

        public void OnChannelDestory(string remoteAddr, IChannel channel)
        {
            throw new NotImplementedException();
        }

        public void PrintAllPeriodically()
        {
            throw new NotImplementedException();
        }

        public byte[] GetSystemTopicList()
        {
            throw new NotImplementedException();
        }

        public byte[] GetTopicsByCluster(string cluster)
        {
            throw new NotImplementedException();
        }

        public byte[] GetUnitTopics()
        {
            throw new NotImplementedException();
        }

        public byte[] GetHasUnitSubTopicList()
        {
            throw new NotImplementedException();
        }

        public byte[] GetHasUnitSubUnUnitTopicList()
        {
            throw new NotImplementedException();
        }
    }
}