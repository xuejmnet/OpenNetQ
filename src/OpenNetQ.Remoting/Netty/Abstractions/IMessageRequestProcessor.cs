using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Remoting.Netty.Abstractions
{
    public interface IMessageRequestProcessor
    {
        RemotingCommand ProcessRequest(IChannelHandlerContext ctx, RemotingCommand request);
        /// <summary>
        /// 拒绝请求
        /// </summary>
        /// <returns></returns>
        bool IsRejectRequest();
    }
}
