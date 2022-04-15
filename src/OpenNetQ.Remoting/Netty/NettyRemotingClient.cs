using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Common.Concurrency;
using DotNetty.Handlers.Timeout;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using J2N.Threading.Atomic;
using KhaosLog.NettyProvider.Handlers;
using Microsoft.Extensions.Logging;
using OpenNetQ.Extensions;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Exceptions;
using OpenNetQ.Remoting.Netty.Abstractions;
using OpenNetQ.Remoting.Netty.Handlers;
using OpenNetQ.Remoting.Protocol;
using OpenNetQ.TaskSchedulers;
using OpenNetQ.Utils;
using Timer = System.Timers.Timer;

namespace OpenNetQ.Remoting.Netty
{
    /// <summary>
    /// time out 后续替换成 task delay
    /// </summary>
    public class NettyRemotingClient : AbstractNettyRemoting, IRemotingClient
    {
        private static readonly ILogger<NettyRemotingClient> _logger = OpenNetQLoggerFactory.CreateLogger<NettyRemotingClient>();

        private readonly RemotingClientOption _option;
        private readonly IPEndPoint _ipEndPoint;
        private readonly bool _useTls;
        //private readonly OpenNetQTaskScheduler _publicSchedluer;
        private readonly OpenNetQTaskScheduler _clientCallbackSchedluer;
        private readonly int LOCK_TIMEOUT_MILLIS = 3000;
        private readonly object _lockChannelTables = new();
        private readonly object _nameServerChannelLock = new();
        private readonly System.Timers.Timer _timer = new Timer(3000)
        {
            AutoReset = true,
            Enabled = true
        };

        private readonly ConcurrentDictionary<string /* addr */, ChannelWrapper> _channelTables = new ();

        private readonly AtomicReference<List<string>> _namesrvAddrList = new AtomicReference<List<String>>();
        private readonly AtomicReference<string> _namesrvAddrChoosed = new AtomicReference<String>();
        private readonly AtomicInt32 _namesrvIndex = new AtomicInt32(InitValueIndex());
        private MultithreadEventLoopGroup group;

        private Bootstrap bootstrap;
        private MessagePackEncoder _encoder;
        private MessagePackDecoder _decoder;
        private NettyClientHandler _nettyClientHandler;
        private NettyClientConnectManagerHandler _nettyClientConnectManagerHandler;
        public NettyRemotingClient(RemotingClientOption option) : base(option.PermitsOneway, option.PermitsAsync)
        {
            _option = option;
            _ipEndPoint = new IPEndPoint(_option.Host, _option.Port);
            _useTls = option.UseTls();
           
            var threads = Math.Max(4, _option.ClientCallbackExecutorThreads);
            _clientCallbackSchedluer = new OpenNetQTaskScheduler(threads, threads, "NettyRemotingClientCallback_");
        }

        public override OpenNetQTaskScheduler GetCallbackExecutor()
        {
            return _clientCallbackSchedluer;
        }

        private static int InitValueIndex()
        {
            var random = new Random();
            return Math.Abs(random.Next() % 999) % 999;
        }

        private void InitSharableHandlers()
        {
            _encoder = new MessagePackEncoder();
            _decoder = new MessagePackDecoder();
            _nettyClientHandler = new NettyClientHandler();
            _nettyClientHandler.OnProcessMessageReceived += OnProcessMessageReceived;
            _nettyClientConnectManagerHandler = new NettyClientConnectManagerHandler(_option);
            _nettyClientConnectManagerHandler.OnNettyEventTrigger += OnNettyEventTrigger;
            _nettyClientConnectManagerHandler.OnCloseChannel += (sender, arg) => CloseChannel(arg);
        }
        public async Task StartAsync()
        {
            InitSharableHandlers();
            group = new MultithreadEventLoopGroup(Math.Max(1, _option.ClientWorkThreads));
            bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoBacklog, 100) // 看最下面解释
                .Option(ChannelOption.SoKeepalive, true) //保持连接
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(3)) //连接超时
                .Option(ChannelOption.RcvbufAllocator, new AdaptiveRecvByteBufAllocator(1024, 1024, 65536))
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    //工作线程连接器 是设置了一个管道，服务端主线程所有接收到的信息都会通过这个管道一层层往下传输
                    //同时所有出栈的消息 也要这个管道的所有处理器进行一步步处理
                    IChannelPipeline pipeline = channel.Pipeline;
                    if (_useTls)
                    {
                        pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(_targetHost)));
                    }

                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast(_decoder);
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4, false));
                    //实体类编码器,心跳管理器,连接管理器
                    pipeline.AddLast(_encoder
                        , new IdleStateHandler(0, 0, _option.AllIdleTime),
                        _nettyClientConnectManagerHandler,
                        _nettyClientHandler
                    );
                }));
            if (_option.ClientSocketSndBufSize > 0)
            {
                _logger.LogInformation($"client set SO_SNDBUF to {_option.ClientSocketSndBufSize}");
                bootstrap.Option(ChannelOption.SoSndbuf,_option.ClientSocketSndBufSize);
            }
            if (_option.ClientSocketRcvBufSize > 0)
            {
                _logger.LogInformation($"client set SO_RCVBUF to {_option.ClientSocketRcvBufSize}");
                bootstrap.Option(ChannelOption.SoRcvbuf,_option.ClientSocketRcvBufSize);
            }
            if (_option.WriteBufferLowWaterMark > 0)
            {
                _logger.LogInformation($"client set netty WRITE_BUFFER_LOW_WATER_MARK to {_option.WriteBufferLowWaterMark}");
                bootstrap.Option(ChannelOption.WriteBufferLowWaterMark,_option.WriteBufferLowWaterMark);
            }
            if (_option.WriteBufferHighWaterMark > 0)
            {
                _logger.LogInformation($"client set netty WRITE_BUFFER_HEIGH_WATER_MARK to {_option.WriteBufferHighWaterMark}");
                bootstrap.Option(ChannelOption.WriteBufferHighWaterMark,_option.WriteBufferHighWaterMark);
            }

            try
            {
                _logger.LogInformation("OpenNetQ开始启动----------");

                await bootstrap.ConnectAsync(_ipEndPoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"OpenNetQ start error.");
            }

            this._timer.Elapsed += (sender, args) =>
            {
                try
                {
                    ScanResponseTables();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "ScanResponseTables exception");
                }
            };
            _timer.Start();
            NettyEventExecutor.Start();
            _logger.LogInformation($"OpenNetQ启动完成监听端口:{_option.Port}");
        }

        public async  Task StopAsync()
        {
            try
            {
                _logger.LogInformation("NettyRemotingClient 开始停止");
                this._timer.Stop();

                foreach (var channelTablesValue in _channelTables.Values)
                {
                    CloseChannel(null, channelTablesValue.Channel);
                }

                _channelTables.Clear();
                NettyEventExecutor.Shutdown();
                await @group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));

                _logger.LogInformation("NettyRemotingClient 已停止");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"NettyRemotingClient shutdown exception.");
            }

            try
            {
                _clientCallbackSchedluer.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"NettyRemotingClient ClientCallbackSchedluer shutdown exception.");
            }
        }

        public void CloseChannel(string? addr, IChannel? channel)
        {
            if (null == channel)
                return;

            string addrRemote = addr ?? RemotingHelper.ParseChannelRemoteAddr(channel);
            try
            {

                var acquired = Monitor.TryEnter(_lockChannelTables, TimeSpan.FromMilliseconds(LOCK_TIMEOUT_MILLIS));
                if (!acquired)
                {
                    _logger.LogWarning($"closeChannel: try to lock channel table, but timeout, { LOCK_TIMEOUT_MILLIS} ms");
                    return;
                }

                try
                {

                    bool removeItemFromTable = true;
                    this._channelTables.TryGetValue(addrRemote,out var prevCW);

                    _logger.LogInformation($"CloseChannel: begin close the channel[{addrRemote}] Found: {prevCW != null}");

                    if (null == prevCW)
                    {
                        _logger.LogInformation($"CloseChannel: the channel[{addrRemote}] has been removed from the channel table before");
                        removeItemFromTable = false;
                    }
                    else if (!prevCW.Channel.Equals(channel))
                    {
                        _logger.LogInformation($"CloseChannel: the channel[{addrRemote}] has been closed before, and has been created again, nothing to do.");
                        removeItemFromTable = false;
                    }

                    if (removeItemFromTable)
                    {
                        this._channelTables.Remove(addrRemote,out var _);
                        _logger.LogInformation($"CloseChannel: the channel[{addrRemote}] was removed from channel table");
                    }

                    RemotingUtil.CloseChannel(channel);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"CloseChannel: close the channel exception");
                }
                finally
                {
                    Monitor.Exit(_lockChannelTables);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseChannel exception");
            }
        }

        public void CloseChannel(IChannel? channel)
        {
            if (null == channel)
                return;
            
            try
            {
                var acquired = Monitor.TryEnter(_lockChannelTables, TimeSpan.FromMilliseconds(LOCK_TIMEOUT_MILLIS));
                if (acquired)
                {
                    try
                    {
                        bool removeItemFromTable = true;
                        ChannelWrapper preCW = null;
                        string addrRemote = null;
                        foreach (var entry in _channelTables)
                        {
                            var key = entry.Key;
                            var prev = entry.Value;
                            if (prev.Channel.Equals(channel))
                            {
                                preCW = prev;
                                addrRemote = key;
                                break;
                            }
                        }

                        if (preCW is null)
                        {
                            _logger.LogInformation(
                                $"EventCloseChannel: the channel[{addrRemote}] has been removed from the channel table before");
                            removeItemFromTable = false;
                        }

                        if (removeItemFromTable)
                        {
                            _channelTables!.Remove(addrRemote, out _);
                            _logger.LogInformation(
                                $"CloseChannel: the channel[{addrRemote}] was removed from channel table");
                            RemotingUtil.CloseChannel(channel);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "CloseChannel: close the channel exception");
                    }
                    finally
                    {
                        Monitor.Exit(_lockChannelTables);
                    }
                }
                else
                {
                    _logger.LogWarning($"CloseChannel: try to lock channel table, but timeout, {LOCK_TIMEOUT_MILLIS}ms");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"CloseChannel exception");
            }
        }

        public void RegisterRPCHook(IRPCHook? hook)
        {
            if (hook != null && !RpcHooks.Contains(hook))
            {
                RpcHooks.Add(hook);
            }
        }

        public void UpdateNameServerAddressList(List<string> addrs)
        {
            var old = this._namesrvAddrList.Value;
            bool update = false;

            if (!addrs.IsEmpty())
            {
                if (null == old)
                {
                    update = true;
                }
                else if (addrs.Count != old.Count)
                {
                    update = true;
                }
                else
                {
                    for (int i = 0; i < addrs.Count && !update; i++)
                    {
                        if (!old.Contains(addrs[i]))
                        {
                            update = true;
                        }
                    }
                }

                if (update)
                {
                    addrs = addrs.OrderBy(o => Guid.NewGuid()).ToList();
                    _logger.LogInformation($"name server address updated. NEW : {addrs.PrintString()} , OLD: {old.PrintString()}");
                    this._namesrvAddrList.Value= addrs;

                    if (this._namesrvAddrChoosed.Value!=null&&!addrs.Contains(this._namesrvAddrChoosed.Value))
                    {
                        this._namesrvAddrChoosed.Value=null;
                    }
                }
            }
        }

        public List<string>? GetNameServerAddressList()
        {
            return _namesrvAddrList.Value;
        }

        public  Task<RemotingCommand> InvokeAsync(string addr, RemotingCommand request, long timeoutMillis)
        {
            //var delay = Task.Delay(TimeSpan.FromSeconds(timeoutMillis));
            //var invokeAsync0 = InvokeAsync0(addr,request, timeoutMillis,cancellationToken);
            //if (await Task.WhenAny(invokeAsync0, delay) == delay)
            //{
            //    throw new RemotingTimeoutException($"invokeSync call the addr[{addr}] timeout");
            //}

            //return await invokeAsync0;
            return  InvokeAsync0(addr, request, timeoutMillis);
        }

        private async Task<RemotingCommand> InvokeAsync0(string addr, RemotingCommand request, long timeoutMillis)
        {
            var begin = TimeUtil.CurrentTimeMillis();
            var channel = await GetAndCreateChannelAsync(addr);
            if (channel is not null && channel.Active)
            {
                try
                {
                    DoBeforeRpcHooks(addr, request);
                    var costTime = TimeUtil.CurrentTimeMillis() - begin;
                    if (timeoutMillis <= costTime)
                    {
                        throw new RemotingTimeoutException($"invokeSync call the addr[{addr}] timeout");
                    }
                    var response = await InvokeSyncImpl(channel,request,timeoutMillis- costTime);
                    DoAfterRpcHooks(RemotingHelper.ParseChannelRemoteAddr(channel),request,response);
                    return response;
                }
                catch (RemotingSendRequestException e)
                {
                    _logger.LogWarning($"invokeSync: send request exception, so close the channel[{addr}]");
                    this.CloseChannel(addr, channel);
                    throw;
                }
                catch(RemotingTimeoutException e)
                {
                    if (_option.ClientCloseSocketIfTimeout)
                    {
                        this.CloseChannel(addr, channel);
                        _logger.LogWarning($"invokeAsync: close socket because of timeout, {timeoutMillis}ms, {addr}");
                    }
                    _logger.LogWarning($"invokeAsync: wait response timeout exception, the channel[{addr}]");
                    throw;
                }
            }
            else
            {
                CloseChannel(addr,channel);
                throw new RemotingConnectException(addr);
            }
        }

        public async Task InvokeCallbackAsync(string addr, RemotingCommand request, long timeoutMillis, Action<ResponseTask> callback)
        {
            var begin = TimeUtil.CurrentTimeMillis();
            var channel = await GetAndCreateChannelAsync(addr);
            if (channel is not null && channel.Active)
            {
                try
                {
                    DoBeforeRpcHooks(addr,request);
                    var costTime = TimeUtil.CurrentTimeMillis()-begin;
                    if (timeoutMillis <= costTime)
                    {
                        throw new RemotingTooMuchRequestException($"invokeAsync call the addr[{addr}] timeout");
                    }

                    await InvokeAsyncImpl(channel, request, TimeUtil.CurrentTimeMillis() - costTime, callback);
                }
                catch (RemotingSendRequestException e)
                {
                    _logger.LogWarning($"invokeAsync: send request exception, so close the channel[{addr}]");
                    CloseChannel(addr,channel);
                    throw;
                }
                catch (RemotingTimeoutException e)
                {
                    if (_option.ClientCloseSocketIfTimeout)
                    {
                        this.CloseChannel(addr, channel);
                        _logger.LogWarning($"invokeCallbackAsync: close socket because of timeout, {timeoutMillis}ms, {addr}");
                    }
                    _logger.LogWarning($"invokeCallbackAsync: wait response timeout exception, the channel[{addr}]");
                    throw;
                }
            }
            else
            {
                CloseChannel(addr,channel);
                throw new RemotingConnectException(addr);
            }
        }
        /// <summary>
        /// time out 替换成 task delay
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="request"></param>
        /// <param name="timeoutMillis"></param>
        /// <returns></returns>
        /// <exception cref="RemotingConnectException"></exception>
        public async Task InvokeOnewayAsync(string addr, RemotingCommand request, long timeoutMillis)
        {
            var channel = await GetAndCreateChannelAsync(addr);
            if (channel is not null && channel.Active)
            {
                try
                {
                    DoBeforeRpcHooks(addr, request);
                    await InvokeOnewayImpl(channel, request, timeoutMillis);
                }
                catch (RemotingSendRequestException e)
                {
                    _logger.LogWarning($"invokeOneway: send request exception, so close the channel[{addr}]");
                    CloseChannel(addr, channel);
                    throw;
                }
                catch (RemotingTimeoutException e)
                {
                    if (_option.ClientCloseSocketIfTimeout)
                    {
                        this.CloseChannel(addr, channel);
                        _logger.LogWarning($"InvokeOnewayAsync: close socket because of timeout, {timeoutMillis}ms, {addr}");
                    }
                    _logger.LogWarning($"InvokeOnewayAsync: wait response timeout exception, the channel[{addr}]");
                    throw;
                }
            }
            else
            {
                CloseChannel(addr,channel);
                throw new RemotingConnectException(addr);
            }
        }

        private async Task<IChannel?> GetAndCreateChannelAsync(string? addr)
        {
            if (addr is null)
            {
                return await GetAndCreateNameServerChannelAsync();
            }

            if (_channelTables.TryGetValue(addr, out var cw) && cw.IsActive())
            {
                return cw.Channel;
            }

            return await CreateChannelAsync(addr);
        }

        private async Task<IChannel?> GetAndCreateNameServerChannelAsync()
        {
            var addr = this._namesrvAddrChoosed.Value;
            if (addr is not null)
            {
                if (_channelTables.TryGetValue(addr, out var cw)&&cw.IsActive())
                {
                    return cw.Channel;
                }
            }

            var addrList = _namesrvAddrList.Value;
            var acquired = Monitor.TryEnter(_nameServerChannelLock,TimeSpan.FromSeconds(LOCK_TIMEOUT_MILLIS));
            if (acquired)
            {
                try
                {
                    addr = _namesrvAddrChoosed.Value;
                    if (addr is not null)
                    {
                        if (_channelTables.TryGetValue(addr, out var cw) && cw.IsActive())
                        {
                            return cw.Channel;
                        }
                    }

                    if (addrList.IsNotEmpty())
                    {
                        for (int i = 0; i < addrList!.Count; i++)
                        {
                            var index = _namesrvIndex.IncrementAndGet();
                            index= Math.Abs(index);
                            index = index % addrList.Count;
                            var newAddr = addrList[index];
                            _namesrvAddrChoosed.Value = newAddr;
                            _logger.LogInformation($"new name server is chosen. OLD: {addr} , NEW: {newAddr}. namesrvIndex = {_namesrvIndex}");
                            var channelNew = await CreateChannelAsync(newAddr);
                            if (channelNew is not null)
                            {
                                return channelNew;
                            }
                        }

                        throw new RemotingConnectException(addrList.PrintString());
                    }
                    
                }
                finally
                {
                    Monitor.Exit(_nameServerChannelLock);
                }
            }
            else
            {
                _logger.LogWarning($"GetAndCreateNameserverChannel: try to lock name server, but timeout, {LOCK_TIMEOUT_MILLIS}ms");
            }

            return null;
        }

        private async Task<IChannel?> CreateChannelAsync(string addr)
        {
            if (_channelTables.TryGetValue(addr, out var cw) && cw.IsActive())
            {
                return cw.Channel;
            }

            var acquired = Monitor.TryEnter(_lockChannelTables,TimeSpan.FromMilliseconds(LOCK_TIMEOUT_MILLIS));
            if (acquired)
            {
                try
                {
                    bool createNewConnection = false;
                    if (_channelTables.TryGetValue(addr, out  cw))
                    {
                        if (cw.IsActive())
                        {
                            return cw.Channel;
                        }
                        else
                        {
                            _channelTables.Remove(addr, out _);
                            createNewConnection = true;
                        }
                    }
                    else
                    {
                        createNewConnection = true;
                    }

                    if (createNewConnection)
                    {
                        var channel = await this.bootstrap.ConnectAsync(RemotingHelper.String2IpEndPoint(addr));
                        _logger.LogInformation($"CreateChannel: begin to connect remote host[{addr}] asynchronously");
                        cw = new ChannelWrapper(channel);
                        _channelTables.TryAdd(addr, cw);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"CreateChannel: create channel exception");
                }
                finally
                {
                    Monitor.Exit(_lockChannelTables);
                }
            }
            else
            {
                _logger.LogWarning($"CreateChannel: try to lock channel table, but timeout, {LOCK_TIMEOUT_MILLIS}ms");
            }

            if (cw is not null)
            {
                if (cw.IsActive())
                {
                    _logger.LogInformation($"CreateChannel: connect remote host[{addr}] success");
                    return cw.Channel;
                }
                else
                {
                    _logger.LogWarning($"CreateChannel: connect remote host[{addr}] failed");
                }
            }

            return null;
        }

        public void RegisterProcessor(int requestCode, INettyRequestProcessor processor, OpenNetQTaskScheduler? scheduler)
        {
            ProcessorTables.Add(requestCode, (processor, scheduler ?? _clientCallbackSchedluer));
        }

        public bool IsChannelWritable(string addr)
        {
            if (_channelTables.TryGetValue(addr, out var cw) && cw.IsActive())
            {
                return cw.IsWritable();
            }

            return true;
        }
    }

    public class ChannelWrapper
    {
        public IChannel Channel { get; }

        public ChannelWrapper(IChannel channel)
        {
            Channel = channel;
        }

        public bool IsActive()
        {
            return Channel.Active;
        }
        public bool IsWritable()
        {
            return Channel.IsWritable;
        }
    }
}