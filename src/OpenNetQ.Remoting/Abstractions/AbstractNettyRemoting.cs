using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using OpenNetQ.Concurrent;
using OpenNetQ.Extensions;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Exceptions;
using OpenNetQ.Remoting.Netty;
using OpenNetQ.Remoting.Netty.Abstractions;
using OpenNetQ.Remoting.Protocol;
using OpenNetQ.TaskSchedulers;
using OpenNetQ.Utils;

namespace OpenNetQ.Remoting.Abstractions
{
    public abstract class AbstractNettyRemoting
    {
        private readonly int _permitsOneway;
        private readonly int _permitsAsync;
        private static IInternalNetQLogger _log = InternalNetQLoggerFactory.GetLogger<AbstractNettyRemoting>();
        private readonly SemaphoreSlim _semaphoreOneway;
        private readonly SemaphoreSlim _semaphoreAsync;
        protected readonly Dictionary<int, (INettyRequestProcessor, OpenNetQTaskScheduler)> ProcessorTables = new(64);

        protected readonly ConcurrentDictionary<int, ResponseTask> ResponseTables = new(31, 256);

        protected (INettyRequestProcessor, OpenNetQTaskScheduler) DefaultRequestProcessor { get; set; }

        /**
         * custom rpc hooks
         */
        protected List<IRPCHook> RpcHooks = new List<IRPCHook>();

        protected NettyEventExecutor NettyEventExecutor = new NettyEventExecutor();

        public AbstractNettyRemoting(int permitsOneway, int permitsAsync)
        {
            _permitsOneway = permitsOneway;
            _permitsAsync = permitsAsync;

            _semaphoreAsync = new SemaphoreSlim(Math.Max(1, permitsAsync));
            _semaphoreOneway = new SemaphoreSlim(Math.Max(1, permitsOneway));
        }

        public void PutNettyEvent(NettyEventArg eventArg)
        {
            this.NettyEventExecutor.PutNettyEvent(eventArg);
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
            (INettyRequestProcessor messageRequestProcessor, OpenNetQTaskScheduler openNetQTaskScheduler) pair =
                default;
            if (!ProcessorTables.TryGetValue(cmd.Code, out (INettyRequestProcessor, OpenNetQTaskScheduler) matched))
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
                        if (pair.messageRequestProcessor is AbstractAsyncNettyRequestProcessor
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
                            RemotingCommand response = RemotingCommand.CreateResponseCommand(
                                RemotingSysResponseCode.SYSTEM_ERROR, RemotingHelper.ExceptionSimpleDesc(e));

                            response.Opaque = opaque;
                            ctx.WriteAndFlushAsync(response);
                        }
                    }

                };


                if (pair.messageRequestProcessor.IsRejectRequest())
                {
                    RemotingCommand response = RemotingCommand.CreateResponseCommand(
                        RemotingSysResponseCode.SYSTEM_BUSY,
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
                        RemotingCommand response = RemotingCommand.CreateResponseCommand(
                            RemotingSysResponseCode.SYSTEM_BUSY,
                            "[OVERLOAD]system busy, start flow control for a while");
                        response.Opaque = opaque;
                        ctx.WriteAndFlushAsync(response);
                    }
                }
            }
            else
            {
                var error = $" request type  {cmd.Code} not supported";
                RemotingCommand? response =
                    RemotingCommand.CreateResponseCommand(RemotingSysResponseCode.REQUEST_CODE_NOT_SUPPORTED, error);

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
                _log.Warn(
                    $"receive response, but not matched any request, {RemotingHelper.ParseChannelRemoteAddr(ctx.Channel)}");
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

        public void ScanResponseTables()
        {
            LinkedList<ResponseTask> rtList = new LinkedList<ResponseTask>();
            var keys = new List<int>(ResponseTables.Keys);
            var enumerator = keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var opaque = enumerator.Current;
                if (ResponseTables.TryGetValue(opaque, out var rep))
                {
                    if (rep.IsTimeOut(TimeSpan.FromMilliseconds(1000)))
                    {
                        rep.Release();
                        ResponseTables.TryRemove(opaque, out var v);
                        rtList.AddLast(rep);
                        _log.Warn($"remove timeout request, {rep}");
                    }
                }
            }
            foreach (var responseTask in rtList)
            {
                try
                {
                    ExecuteInvokeCallback(responseTask);
                }
                catch (Exception e)
                {
                    _log.Warn($"scanResponseTable, operationComplete Exception", e);
                }
            }
        }

        public async Task<RemotingCommand> InvokeSyncImpl(IChannel channel, RemotingCommand request, long timeoutMillis)
        {
            int opaque = request.Opaque;

            var addr = channel.RemoteAddress;
            ResponseTask responseTask = new ResponseTask(channel, opaque, timeoutMillis, null, null);
            ResponseTables.TryAdd(opaque, responseTask);
            try
            {
                _ = channel.WriteAndFlushAsync(request)
                     .ContinueWith((t, c) =>
                     {
                         responseTask.SetSendResponseOk(t.IsCompletedSuccessfully());
                         if (t.IsCompletedSuccessfully())
                         {
                             return;
                         }

                         ResponseTables.TryRemove(opaque, out var v);
                         responseTask.SetException(t.Exception);
                         responseTask.AddResponse(null);
                         _log.Warn($"send a request command to channel <{RemotingHelper.ParseChannelRemoteAddr(channel)}> failed.");
                     }, null);

                var response = responseTask.WaitResponse();
                if (null == response)
                {
                    if (responseTask.IsSendResponseOk())
                    {
                        throw new RemotingTimeoutException(RemotingHelper.ParseSocketAddress(addr), timeoutMillis, responseTask.GetException());
                    }
                    else
                    {
                        throw new RemotingSendRequestException(RemotingHelper.ParseSocketAddress(addr), responseTask.GetException());
                    }
                }

                return response;
            }
            finally
            {
                ResponseTables.TryRemove(opaque, out var v);
            }
        }
        public async Task InvokeAsyncImpl(IChannel channel, RemotingCommand request, long timeoutMillis,
            Action<ResponseTask> callback)
        {
            long beginStartTime = TimeUtil.CurrentTimeMillis();
            int opaque = request.Opaque;
            bool acquired = await this._semaphoreAsync.WaitAsync(TimeSpan.FromMilliseconds(timeoutMillis));
            if (acquired)
            {
                var once = new SemaphoreReleaseOnlyOnce(this._semaphoreAsync);
                long costTime = TimeUtil.CurrentTimeMillis() - beginStartTime;
                if (timeoutMillis < costTime)
                {
                    once.Release();
                    throw new RemotingTimeoutException("invokeAsyncImpl call timeout");
                }

                var responseTask = new ResponseTask(channel, opaque, timeoutMillis - costTime, callback, once);
                ResponseTables.TryAdd(opaque, responseTask);
                try
                {
                    _ = channel.WriteAndFlushAsync(request)
                        .ContinueWith((t, c) =>
                        {
                            if (t.IsCompletedSuccessfully())
                            {
                                responseTask.SetSendResponseOk(true);
                                return;
                            }

                            RequestFail(opaque);
                            _log.Warn($"send a request command to channel <{RemotingHelper.ParseChannelRemoteAddr(channel)}> failed.");
                        }, null);
                }
                catch (Exception e)
                {
                    responseTask.Release();
                    var address = RemotingHelper.ParseChannelRemoteAddr(channel);
                    _log.Warn($"send a request command to channel < {address} > Exception", e);
                    throw new RemotingSendRequestException(address, e);
                }
            }
            else
            {
                if (timeoutMillis <= 0)
                {
                    throw new RemotingTooMuchRequestException("invokeAsyncImpl invoke too fast");
                }
                else
                {
                    string info = $"{nameof(InvokeAsyncImpl)} tryAcquire semaphore timeout, {timeoutMillis}ms, waiting thread nums: {this._semaphoreAsync.CurrentCount} semaphoreAsyncValue: {_permitsAsync}";
                    _log.Warn(info);
                    throw new RemotingTimeoutException(info);
                }
            }
        }
        protected void FailFast(IChannel channel)
        {
            var enumerator = ResponseTables.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var keyValuePair = enumerator.Current;
                if (Equals(keyValuePair.Value.GetChannel(), channel))
                {
                    var opaque = keyValuePair.Key;
                    RequestFail(opaque);
                }
            }
        }

        private void RequestFail(int opaque)
        {
            if (ResponseTables.TryRemove(opaque, out var responseTask))
            {
                responseTask.SetSendResponseOk(false);
                responseTask.AddResponse(null);
                try
                {
                    ExecuteInvokeCallback(responseTask);
                }
                catch (Exception e)
                {
                    _log.Warn($"execute callback in requestFail, and callback throw {e}");
                }
                finally
                {
                    responseTask.Release();
                }
            }
        }



        public async Task InvokeOnewayImpl(IChannel channel, RemotingCommand request, long timeoutMillis)
        {
            request.MarkOnewayRPC();
            bool acquired = await this._semaphoreOneway.WaitAsync(TimeSpan.FromMilliseconds(timeoutMillis));
            if (acquired)
            {
                var once = new SemaphoreReleaseOnlyOnce(this._semaphoreOneway);

                try
                {
                    _ = channel.WriteAndFlushAsync(request)
                         .ContinueWith((t, c) =>
                         {
                             once.Release();
                             if (!t.IsCompletedSuccessfully())
                             {
                                 _log.Warn($"send a request command to channel <{RemotingHelper.ParseChannelRemoteAddr(channel)}> failed.");
                             }
                         }, null);
                }
                catch (Exception e)
                {
                    once.Release();
                    _log.Warn($"write send a request command to channel <{channel.RemoteAddress}> failed.");
                    var address = RemotingHelper.ParseChannelRemoteAddr(channel);
                    throw new RemotingSendRequestException(address, e);
                }
            }
            else
            {
                if (timeoutMillis <= 0)
                {
                    throw new RemotingTooMuchRequestException($"{nameof(InvokeOnewayImpl)} invoke too fast");
                }
                else
                {
                    var info =
                        $"{nameof(InvokeOnewayImpl)} tryAcquire semaphore timeout, {timeoutMillis}ms, waiting thread nums: {this._semaphoreOneway.CurrentCount} semaphoreOnewayValue: {_permitsOneway}";
                    _log.Warn(info);
                    throw new RemotingTimeoutException(info);
                }
            }
        }
    }
}
