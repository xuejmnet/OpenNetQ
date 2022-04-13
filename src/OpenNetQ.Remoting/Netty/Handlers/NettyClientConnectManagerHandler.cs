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
        public override bool IsSharable => true;
        public override Task ConnectAsync(IChannelHandlerContext context, EndPoint remoteAddress, EndPoint localAddress)
        {
            var local = localAddress == null ? "UNKNOWN" : RemotingHelper.ParseSocketAddressAddr(localAddress);
            var remote = remoteAddress == null ? "UNKNOWN" : RemotingHelper.ParseSocketAddressAddr(remoteAddress);
            _logger.Info($"NETTY CLIENT PIPELINE: CONNECT  {local} => {remote}");
            return base.ConnectAsync(context, remoteAddress, localAddress);
        }

        public override Task DisconnectAsync(IChannelHandlerContext context)
        {
            var remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.Info($"NETTY CLIENT PIPELINE: DISCONNECT {remoteAddress}");
            RemotingUtil.CloseChannel(context.Channel);
            return base.DisconnectAsync(context);
        }

        public override Task CloseAsync(IChannelHandlerContext context)
        {
            var remoteAddress = RemotingHelper.ParseChannelRemoteAddr(context.Channel);
            _logger.Info($"NETTY CLIENT PIPELINE: CLOSE {remoteAddress}");
            return base.CloseAsync(context);
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent @event)
            {
                if (@event.State == IdleState.AllIdle)
                {
                    context.WriteAndFlushAsync(BuildHeartBeatCommand());
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
            _logger.Error($"NETTY CLIENT PIPELINE: exceptionCaught {remoteAddress}");
            _logger.Error("NETTY CLIENT PIPELINE: exceptionCaught exception.",exception);
            RemotingUtil.CloseChannel(context.Channel);
        }


        /// <summary>
        /// build heart beat command
        /// </summary>
        /// <returns></returns>
        private RemotingCommand BuildHeartBeatCommand()
        {
            var cmd = RemotingCommand.CreateRequestCommand(MessageCodeEnum.心跳请求);
            if (cmd.ExtFields == null)
                cmd.ExtFields = new Dictionary<string, string>();
            cmd.ExtFields.Add(ConstantKey.USER_ID, _clientOption.UserId);
            cmd.ExtFields.Add(ConstantKey.APP_ID, _clientOption.AppId);
            // Content
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            foreach (var requestExtField in cmd.ExtFields)
            {
                if (requestExtField.Key != ConstantKey.SIGNATURE)
                {
                    dic.Add(requestExtField.Key, requestExtField.Value);
                }
            }
            var content = AclUtil.CombineRequestContent(cmd, dic);
            var sign = AclUtil.CalSignature(content, _clientOption.Secret);
            cmd.ExtFields.Add(ConstantKey.SIGNATURE, sign);
            return cmd;
        }
    }
}