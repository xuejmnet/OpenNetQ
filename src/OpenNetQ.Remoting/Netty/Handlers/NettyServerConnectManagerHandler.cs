using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using OpenNetQ.Remoting.Common;

namespace OpenNetQ.Remoting.Netty.Handlers
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: Friday, 13 November 2020 17:07:38
    * @Email: 326308290@qq.com
    */
    /// <summary>
    /// 同时处理出入栈事件
    /// </summary>
    public class NettyServerConnectManagerHandler : ChannelDuplexHandler
    {
        private readonly ILogger<NettyServerConnectManagerHandler> _logger;
        public NettyServerConnectManagerHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NettyServerConnectManagerHandler>();
        }
        public override bool IsSharable => true;

        public event EventHandler<NettyEventArg> OnNettyEventTrigger;
        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            string remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.LogInformation($"NETTY SERVER PIPELINE: ChannelRegistered {remoteAddress}");
            base.ChannelRegistered(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            string remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.LogInformation($"NETTY SERVER PIPELINE: ChannelUnregistered, the channel[{remoteAddress}]");
            base.ChannelUnregistered(context);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            string remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.LogInformation($"NETTY SERVER PIPELINE: ChannelActive, the channel[{remoteAddress}]");
            base.ChannelActive(context);
            OnNettyEventTrigger.Invoke(this,new NettyEventArg(NettyEventTypeEnum.CONNECT,remoteAddress,context.Channel));
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            string remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.LogInformation($"NETTY SERVER PIPELINE: channelInactive, the channel[{remoteAddress}]");
            base.ChannelInactive(context);
            OnNettyEventTrigger?.Invoke(this,new NettyEventArg(NettyEventTypeEnum.CLOSE,remoteAddress,context.Channel));
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent @event)
            {
                if (@event.State == IdleState.AllIdle)
                {
                    var remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
                    _logger.LogInformation($"NETTY SERVER PIPELINE: IDLE exception [{remoteAddress}]");
                    RemotingUtil.CloseChannel(context.Channel, _logger);
                    OnNettyEventTrigger?.Invoke(this,new NettyEventArg(NettyEventTypeEnum.IDLE,remoteAddress,context.Channel));
                    
                }
            }
            context.FireUserEventTriggered(evt);
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            string remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.LogError($"NETTY SERVER PIPELINE: exceptionCaught {remoteAddress}");
            _logger.LogError(exception, "NETTY SERVER PIPELINE: exceptionCaught exception.");
            OnNettyEventTrigger?.Invoke(this,new NettyEventArg(NettyEventTypeEnum.EXCEPTION,remoteAddress,context.Channel));
            RemotingUtil.CloseChannel(context.Channel,_logger);
        }
    }
}