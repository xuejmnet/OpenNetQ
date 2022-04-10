using System;
using DotNetty.Transport.Channels;
using OpenNetQ.Remoting.Protocol;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.NameServer.Processor
{
    public class ClusterTestRequestProcessor:IDefaultProcessor
    {
        public RemotingCommand ProcessRequest(IChannelHandlerContext ctx, RemotingCommand request)
        {
            throw new NotImplementedException();
        }

        public bool IsRejectRequest()
        {
            throw new NotImplementedException();
        }

        public void AsyncProcessRequest(IChannelHandlerContext ctx, RemotingCommand request, Action<RemotingCommand?> callback)
        {
            throw new NotImplementedException();
        }
    }
}