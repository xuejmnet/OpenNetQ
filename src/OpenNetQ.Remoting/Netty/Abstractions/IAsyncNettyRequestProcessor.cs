using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Remoting.Netty.Abstractions
{
    public interface IAsyncNettyRequestProcessor:INettyRequestProcessor
    {
        void AsyncProcessRequest(IChannelHandlerContext ctx, RemotingCommand request,
            Action<RemotingCommand?> callback);
    }
}
