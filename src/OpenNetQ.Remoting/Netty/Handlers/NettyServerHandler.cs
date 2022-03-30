using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenNetQ.Remoting.Exceptions;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Remoting.Netty.Handlers
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 13 November 2020 17:36:51
* @Email: 326308290@qq.com
*/
    /// <summary>
    /// 服务处理执行器
    /// </summary>
    public class NettyServerHandler : SimpleChannelInboundHandler<RemotingCommand>
    {
        private readonly ILogger<NettyServerHandler> _logger;
        public override bool IsSharable => true;

        public NettyServerHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NettyServerHandler>();
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, RemotingCommand msg)
        {
            OnProcessMessageReceived?.Invoke(this,new MessageEventArg(ctx,msg));
        }

        public event EventHandler<MessageEventArg> OnProcessMessageReceived;
    }
}