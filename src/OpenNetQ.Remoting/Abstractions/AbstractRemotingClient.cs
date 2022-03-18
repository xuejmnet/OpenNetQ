using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using OpenNetQ.Extensions;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Netty;
using OpenNetQ.Remoting.Netty.Abstractions;
using OpenNetQ.Remoting.Protocol;
using OpenNetQ.TaskSchedulers;

namespace OpenNetQ.Remoting.Abstractions
{
    public abstract class AbstractRemotingClient
    {
        private static IInternalNetQLogger _log = InternalNetQLoggerFactory.GetLogger<AbstractRemotingClient>();
        private readonly SemaphoreSlim _semaphoreOneway;
        private readonly SemaphoreSlim _semaphoreAsync;
        protected readonly Dictionary<int, (IMessageRequestProcessor, OpenNetQTaskScheduler)> ProcessorTables = new(64);

        protected readonly ConcurrentDictionary<int, ResponseTask> ResponseTables = new(31, 256);

        protected (IMessageRequestProcessor, OpenNetQTaskScheduler) DefaultRequestProcessor { get; set; }

        /**
         * custom rpc hooks
         */
        protected List<IRPCHook> RpcHooks = new List<IRPCHook>();
        public AbstractRemotingClient(int permitsOneway, int permitsAsync)
        {

            _semaphoreAsync = new SemaphoreSlim(Math.Max(1, permitsAsync));
            _semaphoreOneway = new SemaphoreSlim(Math.Max(1, permitsOneway));
        }
        /// <summary>
        /// 处理收到的消息
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cmd"></param>
        public void ProcessMessageReceived(IChannelHandlerContext ctx, RemotingCommand? cmd)
        {
            if (cmd != null)
            {

                switch (cmd.GetCommandType())
                {
                    case RemotingCommandType.REQUEST_COMMAND:
                        ProcessRequestCommand(ctx, cmd);
                        break;
                    case RemotingCommandType.RESPONSE_COMMAND:
                        ProcessResponseCommand(ctx, cmd);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 处理收到的请求消息
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cmd"></param>
        public void ProcessRequestCommand(IChannelHandlerContext ctx, RemotingCommand cmd)
        {
            (IMessageRequestProcessor messageRequestProcessor, OpenNetQTaskScheduler openNetQTaskScheduler) pair = default;
            if (!ProcessorTables.TryGetValue(cmd.Code, out (IMessageRequestProcessor, OpenNetQTaskScheduler) matched))
            {
                pair = DefaultRequestProcessor;
            }
            else
            {
                pair = default == matched ? DefaultRequestProcessor : matched;
            }

            var opaque = cmd.Opaque;
            if (pair != default)
            {
                Action run = () =>
                {
                    try
                    {
                        String remoteAddr = RemotingHelper.ParseChannelRemoteAddr(ctx.Channel);
                        DoBeforeRpcHooks(remoteAddr, cmd);
                        Action<RemotingCommand?> callback = response =>
                        {
                            DoAfterRpcHooks(remoteAddr, cmd, response);
                            if (!cmd.IsOnewayRPC())
                            {
                                if (response != null)
                                {
                                    response.Opaque = opaque;
                                    response.MarkResponseType();
                                    try
                                    {
                                        ctx.WriteAndFlushAsync(response);
                                    }
                                    catch (Exception e)
                                    {
                                        _log.Error("process request over, but response failed", e);
                                        _log.Error($"{cmd}");
                                        _log.Error($"{response}");
                                    }
                                }
                                else
                                {
                                }
                            }
                        };
                        if (pair.messageRequestProcessor is AbstractAsyncMessageRequestProcessor
                            asyncMessageRequestProcessor)
                        {
                            asyncMessageRequestProcessor.AsyncProcessRequest(ctx, cmd, callback);
                        }
                        else
                        {
                            RemotingCommand response = pair.messageRequestProcessor.ProcessRequest(ctx, cmd);
                            callback(response);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error("process request exception", e);
                        _log.Error($"{cmd}");

                        if (!cmd.IsOnewayRPC())
                        {
                            RemotingCommand response = RemotingCommand.CreateResponseCommand(RemotingSysResponseCode.SYSTEM_ERROR, RemotingHelper.ExceptionSimpleDesc(e));

                            response.Opaque = opaque;
                            ctx.WriteAndFlushAsync(response);
                        }
                    }

                };


                if (pair.messageRequestProcessor.IsRejectRequest())
                {
                    RemotingCommand response = RemotingCommand.CreateResponseCommand(RemotingSysResponseCode.SYSTEM_BUSY,
                        "[REJECTREQUEST]system busy, start flow control for a while");
                    response.Opaque = opaque;
                    ctx.WriteAndFlushAsync(response);
                    return;
                }

                try
                {
                    pair.openNetQTaskScheduler.Run(run);
                }
                catch (Exception e)
                {
                    _log.Error($"task scheduler run error,{e}");
                    if (!cmd.IsOnewayRPC())
                    {
                        RemotingCommand response = RemotingCommand.CreateResponseCommand(RemotingSysResponseCode.SYSTEM_BUSY,
                             "[OVERLOAD]system busy, start flow control for a while");
                        response.Opaque = opaque;
                        ctx.WriteAndFlushAsync(response);
                    }
                }
            }
            else
            {
                var error = $" request type  {cmd.Code} not supported";
                RemotingCommand? response = RemotingCommand.CreateResponseCommand(RemotingSysResponseCode.REQUEST_CODE_NOT_SUPPORTED, error);

                response.Opaque = opaque;
                ctx.WriteAndFlushAsync(response);
                _log.Error(RemotingHelper.ParseChannelRemoteAddr(ctx.Channel) + error);
            }
        }


        public void ProcessResponseCommand(IChannelHandlerContext ctx, RemotingCommand cmd)
        {
            int opaque = cmd.Opaque;
            if (ResponseTables.Remove(opaque, out var responseTask))
            {
                responseTask.SetResponseCommand(cmd);
                if (responseTask.IsAsyncRequest())
                {
                    ExecuteInvokeCallback(responseTask);
                }
                else
                {
                    responseTask.AddResponse(cmd);
                    responseTask.Release();
                }
            }
            else
            {
                _log.Warn($"receive response, but not matched any request, {RemotingHelper.ParseChannelRemoteAddr(ctx.Channel)}");
                _log.Warn($"{cmd}");
            }
        }
        public abstract OpenNetQTaskScheduler? GetCallbackExecutor();
        private void ExecuteInvokeCallback(ResponseTask responseTask)
        {
            bool runInThisThread = false;
            var executor = this.GetCallbackExecutor();
            if (executor != null)
            {
                try
                {
                    executor.Run(() =>
                    {
                        try
                        {
                            responseTask.ExecuteInvokeCallback();
                        }
                        catch (Exception e)
                        {
                            _log.Warn("execute callback in executor exception, and callback throw", e);
                        }
                        finally
                        {
                            responseTask.Release();
                        }

                    });
                }
                catch (Exception e)
                {
                    runInThisThread = true;
                    _log.Warn("execute callback in executor exception, maybe executor busy", e);
                }
            }
            else
            {
                runInThisThread = true;
            }

            if (runInThisThread)
            {
                try
                {
                    responseTask.ExecuteInvokeCallback();
                }
                catch (Exception e)
                {
                    _log.Warn("executeInvokeCallback Exception", e);
                }
                finally
                {
                    responseTask.Release();
                }
            }
        }


        protected void DoBeforeRpcHooks(String addr, RemotingCommand request)
        {
            if (RpcHooks.Count > 0)
            {
                foreach (var rpcHook in RpcHooks)
                {
                    rpcHook.DoBeforeRequest(addr, request);
                }
            }
        }

        protected void DoAfterRpcHooks(String addr, RemotingCommand request, RemotingCommand? response)
        {
            foreach (var rpcHook in RpcHooks)
            {
                rpcHook.DoAfterResponse(addr, request, response);
            }
        }
    }
}
