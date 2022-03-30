using System;
using DotNetty.Transport.Channels;
using OpenNetQ.Common;
using OpenNetQ.Common.NameSrv;
using OpenNetQ.Common.Protocol.Body;
using OpenNetQ.Common.Route;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.NameServer.RouteInfo
{
    public interface IRouteInfoManager
    {
        byte[] GetAllClusterInfo();
        void DeleteTopic(string topic);
        byte[] GetAllTopicList();
        RegisterBrokerResult RegisterBroker(
            string clusterName,
            string brokerAddr,
            string brokerName,
            long brokerId,
            string haServerAddr,
            TopicConfigSerializeWrapper topicConfigSerializeWrapper,
            ICollection<string> filterList,
            IChannel channel);

        bool IsBrokerTopicConfig(string brokerAddr, DataVersion dataVersion);
        DataVersion QueryBrokerTopicConfig(string brokerAddr);
        void UpdateBrokerInfoUpdateTimestamp(string brokerAddr);
        void CreateAndUpdateQueueData(string brokerName, TopicConfig topicConfig);
        int WipeWritePermOfBrokerByLock(string brokerName);
        int AddWritePermOfBreakerByLock(string brokerName);
        int OperateWritePermOfBrokerByLock(string brokerName, int requestCode);
        int OperateWritePermOfBroker(string brokerName, string requestCode);
        void UnRegisterBroker(string clusterName, string brokerAddr, string brokerName, long brokerId);
        void RemoveTopicByBrokerName(string brokerName);
        TopicRouteData PickupTopicRouteData(string topic);
        void ScanNotActiveBroker();
        void OnChannelDestory(string remoteAddr, IChannel channel);
        void PrintAllPeriodically();
        byte[] GetSystemTopicList();
        byte[] GetTopicsByCluster(string cluster);
        byte[] GetUnitTopics();
        byte[] GetHasUnitSubTopicList();
        byte[] GetHasUnitSubUnUnitTopicList();
    }
}