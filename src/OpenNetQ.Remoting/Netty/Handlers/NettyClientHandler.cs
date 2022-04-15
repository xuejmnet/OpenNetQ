using System;
using DotNetty.Common.Internal.Logging;
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
    public class NettyClientHandler: SimpleChannelInboundHandler<RemotingCommand>
    {
        private static readonly ILogger<NettyClientHandler> _logger = OpenNetQLoggerFactory.CreateLogger<NettyClientHandler>();

        public override bool IsSharable => true;


        protected override void ChannelRead0(IChannelHandlerContext ctx, RemotingCommand msg)
        {
            OnProcessMessageReceived?.Invoke(this,new MessageEventArg(ctx,msg));
        }
        public event EventHandler<MessageEventArg> OnProcessMessageReceived;

        ///// <summary>
        ///// 进入消息处理接收
        ///// </summary>
        ///// <param name="ctx"></param>
        ///// <param name="msg"></param>
        //private void ProcessMessageReceived(IChannelHandlerContext ctx, RemotingCommand msg)
        //{
        //    RemotingCommand cmd = msg;
        //    if (cmd != null && cmd.GetCommandType() == RemotingCommandType.RESPONSE_COMMAND)
        //    {
        //        ProcessResponseCommand(ctx, cmd);
        //    }
        //}

        ///// <summary>
        ///// 处理请求消息
        ///// </summary>
        ///// <param name="ctx"></param>
        ///// <param name="cmd"></param>
        //private void ProcessResponseCommand(IChannelHandlerContext ctx, RemotingCommand cmd)
        //{
        //    var requestId = cmd.RequestId;
        //    if (ResponseTaskManager.GetInstance().TryRemove(requestId, out var responseTask)&&responseTask!=null)
        //    {
        //        responseTask.PutResponse(cmd);
        //        if (responseTask.IsAsyncRequest())
        //        {
        //            responseTask.ExecuteInvokeCallback();
        //        }
        //        else
        //        {
        //            responseTask.GoOn();
        //            responseTask.Release();
        //        }
        //    }
        //    else
        //    {
        //        if (cmd.Code != MessageCodeEnum.心跳响应)
        //        {
        //            _logger.Warn("receive response, but not matched any request, " + RemotingHelper.ParseChannelRemoteAddr(ctx.Channel));
        //            _logger.Warn($"not found response in responseTaskManager cmd: {cmd}");
        //        }
        //    }

        //}
    }
}