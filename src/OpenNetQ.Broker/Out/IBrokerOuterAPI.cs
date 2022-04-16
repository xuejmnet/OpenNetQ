using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Common.NameSrv;
using OpenNetQ.Common.Protocol.Body;

namespace OpenNetQ.Broker.Out
{
    public interface IBrokerOuterAPI
    {
        ICollection<bool> NeedRegister(string clusterName, string brokerAddr, string brokerName, long brokerId,
            TopicConfigSerializeWrapper topicConfigWrapper, int timeoutMillis);

        ICollection<RegisterBrokerResult> RegisterBrokerAll(
            string clusterName,
            string brokerAddr,
            string brokerName,
            long brokerId,
            string haServerAddr,
            TopicConfigSerializeWrapper topicConfigWrapper,
            ICollection<string> filterServerList,
            bool oneway,
            int timeoutMillis,
            bool compressed
        );

        void UpdateNameServerAddressList(string addrs);

        string FetchNameServerAddr();
    }
}
