using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using OpenNetQ.Remoting.Netty.Abstractions;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Broker.Processor
{
    public class PullMessageProcessor:AbstractAsyncNettyRequestProcessor, IPullMessageProcessor
    {
        private readonly BrokerController _brokerController;

        public PullMessageProcessor(BrokerController brokerController)
        {
            _brokerController = brokerController;
        }

        public override RemotingCommand ProcessRequest(IChannelHandlerContext ctx, RemotingCommand request)
        {
            throw new NotImplementedException();
        }

        public override bool IsRejectRequest()
        {
            throw new NotImplementedException();
        }
    }
}
