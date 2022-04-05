using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Common.Protocol.Body;

namespace OpenNetQ.Broker.Topic
{
    public interface ITopicConfigManager
    {
        TopicConfigSerializeWrapper BuildTopicConfigSerializeWrapper();
    }
}
