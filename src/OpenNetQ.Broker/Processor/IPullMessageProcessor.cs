using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Broker.MqTrace;
using OpenNetQ.Remoting.Netty.Abstractions;

namespace OpenNetQ.Broker.Processor
{
    public interface IPullMessageProcessor: IAsyncNettyRequestProcessor
    {
        void RegisterConsumeMessageHook(ICollection<IConsumeMessageHook> consumeMessageHooks);
    }
}
