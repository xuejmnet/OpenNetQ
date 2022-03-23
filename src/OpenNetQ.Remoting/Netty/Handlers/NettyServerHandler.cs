using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
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
        private static readonly IInternalLogger _logger = InternalLoggerFactory.GetInstance<NettyServerHandler>();
        private readonly IServiceProvider _serviceProvider;
        private readonly IRpcServerFilterManager _rpcServerFilterManager;
        private readonly IRequestProcessor _requestProcessor;
        public override bool IsSharable => true;

        public NettyServerHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _rpcServerFilterManager = serviceProvider.GetService<IRpcServerFilterManager>();
            _requestProcessor = serviceProvider.GetService<IRequestProcessor>();
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, RemotingCommand msg)
        {
            ProcessMessageReceived(ctx, msg);
        }

        /// <summary>
        /// 进入消息处理接收
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        private void ProcessMessageReceived(IChannelHandlerContext ctx, RemotingCommand msg)
        {
            RemotingCommand cmd = msg;
            if (cmd != null && cmd.GetCommandType() == RemotingCommandType.REQUEST_COMMAND)
            {
                ProcessRequestCommand(ctx, cmd);
            }
        }

        /// <summary>
        /// 处理请求消息
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cmd"></param>
        private void ProcessRequestCommand(IChannelHandlerContext ctx, RemotingCommand cmd)
        {
            Task.Run(async () =>
            {
                // _logger.Debug($"received msg:{cmd}");
                await DoAsync(ctx, cmd);
            });
            // }
            // else
            // {
            //     String error = " request type " + cmd.Code + " not supported";
            //     RemotingCommand response =
            //         RemotingCommand.CreateResponseCommand(MessageCode.不支持消息, error);
            //     response.RequestId = requestId;
            //     ctx.WriteAndFlushAsync(response);
            //     Console.WriteLine($"Ip:{RemotingHelper.ParseChannelRemoteAddr(ctx.Channel)}:{error}");
            // }
        }

        protected void DoBeforeRpcFilters(RemotingContext context)
        {
            var rpcFilters = _rpcServerFilterManager.GetRpcFilters();
            if (rpcFilters != null && rpcFilters.Any())
            {
                foreach (var rpcFilter in rpcFilters)
                {
                    rpcFilter.DoBeforeRequest(context);
                }
            }
        }

        protected void DoAfterRpcFilters(RemotingContext context)
        {
            var rpcFilters = _rpcServerFilterManager.GetRpcFilters();
            if (rpcFilters != null && rpcFilters.Any())
            {
                foreach (var rpcFilter in rpcFilters)
                {
                    rpcFilter.DoAfterResponse(context);
                }
            }
        }

        private async Task DoAsync(IChannelHandlerContext ctx, RemotingCommand cmd)
        {
            var context = new RemotingContext(cmd, ctx.Channel);
            var requestId = cmd.RequestId;
            try
            {
                //执行前置过滤器
                DoBeforeRpcFilters(context);
                //最终处理的方法
                Func<RemotingCommand, Task> callback = async resp =>
                {
                    //执行后置过滤器
                    DoAfterRpcFilters(context);
                    //如果是不是单向消息
                    if (!cmd.IsOnewayRPC() && resp != null)
                    {
                        resp.RequestId = requestId;
                        resp.MarkResponseType();
                        try
                        {
                            await ctx.Channel.WriteAndFlushAsync(resp);
                        }
                        catch (Exception e)
                        {
                            _logger.Error($"process request over, but response failed.cmd={cmd},resp={resp}");
                            _logger.Error(e);
                        }
                    }
                };
                RemotingCommand response = _requestProcessor.ProcessRequest(context);
                await callback(response);
            }
            catch (AclException e)
            {
                _logger.Error($"unAuthorization:{cmd}");
                _logger.Error(e);
                if (!cmd.IsOnewayRPC())
                {
                    RemotingCommand response = RemotingCommand.CreateResponseCommand(MessageCodeEnum.认证失败, e.Message);
                    response.RequestId = requestId;
                    await ctx.WriteAndFlushAsync(response);
                }
            }
            catch (RemotingException e)
            {
                _logger.Error($"process request remotingException:{cmd}");
                _logger.Error(e);

                if (!cmd.IsOnewayRPC())
                {
                    RemotingCommand response = RemotingCommand.CreateResponseCommand(e.Code, e.Message);
                    response.RequestId = requestId;
                    await ctx.WriteAndFlushAsync(response);
                }
            }
            catch (Exception e)
            {
                _logger.Error($"process request exception:{cmd}");
                _logger.Error(e);

                if (!cmd.IsOnewayRPC())
                {
                    RemotingCommand response = RemotingCommand.CreateResponseCommand(MessageCodeEnum.系统错误,
                        e.Message);
                    response.RequestId = requestId;
                    await ctx.WriteAndFlushAsync(response);
                }
            }
        }
    }
}