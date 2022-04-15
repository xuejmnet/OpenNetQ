using System;
using System.Net;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Protocol;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Remoting.Netty.Handlers
{
    public class NettyClientConnectManagerHandler:ChannelDuplexHandler
    {
        private readonly RemotingClientOption _clientOption;
        private static readonly ILogger<NettyClientHandler> _logger = OpenNetQLoggerFactory.CreateLogger<NettyClientHandler>();

        public NettyClientConnectManagerHandler(RemotingClientOption clientOption)
        {
            _clientOption = clientOption;
        }
        public event EventHandler<NettyEventArg> OnNettyEventTrigger;
        public event EventHandler<IChannel> OnCloseChannel;
        public override bool IsSharable => true;
        public override async Task ConnectAsync(IChannelHandlerContext context, EndPoint? remoteAddress, EndPoint? localAddress)
        {
            var local = localAddress == null ? "UNKNOWN" : RemotingHelper.ParseSocketAddress(localAddress);
            var remote = remoteAddress == null ? "UNKNOWN" : RemotingHelper.ParseSocketAddress(remoteAddress);
            _logger.LogInformation($"NETTY CLIENT PIPELINE: CONNECT  {local} => {remote}");
            await base.ConnectAsync(context, remoteAddress, localAddress);
            OnNettyEventTrigger?.Invoke(this,new NettyEventArg(NettyEventTypeEnum.CONNECT, remote,context.Channel));
        }

        public override async  Task DisconnectAsync(IChannelHandlerContext context)
        {
            var remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.LogInformation($"NETTY CLIENT PIPELINE: DISCONNECT {remoteAddress}");
            OnCloseChannel?.Invoke(this,context.Channel);
            await  base.DisconnectAsync(context);
            OnNettyEventTrigger?.Invoke(this,new NettyEventArg(NettyEventTypeEnum.CLOSE,remoteAddress,context.Channel));
        }

        public override async Task CloseAsync(IChannelHandlerContext context)
        {
            var remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.LogInformation($"NETTY CLIENT PIPELINE: CLOSE {remoteAddress}");
            await base.CloseAsync(context);
            OnNettyEventTrigger?.Invoke(this,new NettyEventArg(NettyEventTypeEnum.CLOSE,remoteAddress,context.Channel));
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent @event)
            {
                if (@event.State == IdleState.AllIdle)
                {
                    var remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
                    _logger.LogWarning($"NETTY CLIENT PIPELINE: IDLE exception [{remoteAddress}]");
                    OnCloseChannel?.Invoke(this,context.Channel);
                    OnNettyEventTrigger?.Invoke(this,new NettyEventArg(NettyEventTypeEnum.IDLE,remoteAddress,context.Channel));
                    //var remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
                    //_logger.Info($"NETTY CLIENT PIPELINE: IDLE exception [{remoteAddress}]");
                    //RemotingUtil.CloseChannel(context.Channel);
                }
            }
            context.FireUserEventTriggered(evt);
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            string remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.LogWarning($"NETTY CLIENT PIPELINE: exceptionCaught {remoteAddress}");
            _logger.LogWarning(exception,"NETTY CLIENT PIPELINE: exceptionCaught exception.");
            OnCloseChannel?.Invoke(this,context.Channel);
            OnNettyEventTrigger?.Invoke(this,new NettyEventArg(NettyEventTypeEnum.EXCEPTION,remoteAddress,context.Channel));
        }


        ///// <summary>
        ///// build heart beat command
        ///// </summary>
        ///// <returns></returns>
        //private RemotingCommand BuildHeartBeatCommand()
        //{
        //    var cmd = RemotingCommand.CreateRequestCommand(MessageCodeEnum.心跳请求);
        //    if (cmd.ExtFields == null)
        //        cmd.ExtFields = new Dictionary<string, string>();
        //    cmd.ExtFields.Add(ConstantKey.USER_ID, _clientOption.UserId);
        //    cmd.ExtFields.Add(ConstantKey.APP_ID, _clientOption.AppId);
        //    // Content
        //    SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
        //    foreach (var requestExtField in cmd.ExtFields)
        //    {
        //        if (requestExtField.Key != ConstantKey.SIGNATURE)
        //        {
        //            dic.Add(requestExtField.Key, requestExtField.Value);
        //        }
        //    }
        //    var content = AclUtil.CombineRequestContent(cmd, dic);
        //    var sign = AclUtil.CalSignature(content, _clientOption.Secret);
        //    cmd.ExtFields.Add(ConstantKey.SIGNATURE, sign);
        //    return cmd;
        //}
    }
}