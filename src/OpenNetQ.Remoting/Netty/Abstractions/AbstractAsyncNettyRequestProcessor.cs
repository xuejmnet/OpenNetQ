using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Remoting.Netty.Abstractions
{
    public abstract class AbstractAsyncNettyRequestProcessor : IAsyncNettyRequestProcessor
    {
        public abstract RemotingCommand ProcessRequest(IChannelHandlerContext ctx, RemotingCommand request);
        public abstract bool IsRejectRequest();
        /// <summary>
        /// 处理异步消息
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="request"></param>
        /// <param name="callback">入参response</param>
        public void AsyncProcessRequest(IChannelHandlerContext ctx, RemotingCommand request, Action<RemotingCommand?> callback)
        {
            RemotingCommand response = ProcessRequest(ctx, request);
            callback(response);
        }
    }
}
