using System;
using System.Collections.Generic;
using System.Linq;
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
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Netty.Abstractions;
using OpenNetQ.Remoting.Netty.Handlers;
using OpenNetQ.Remoting.Protocol;
using OpenNetQ.TaskSchedulers;

namespace OpenNetQ.Remoting.Netty
{
    /// <summary>
    /// 
    /// </summary>
    public class NettyRemotingClient : AbstractNettyRemoting, IRemotingClient
    {
        private static readonly ILogger<NettyRemotingClient> _logger = OpenNetQLoggerFactory.CreateLogger<NettyRemotingClient>();

        private readonly RemotingClientOption _option;
        private readonly bool _useTls;
        private readonly OpenNetQTaskScheduler _clientCallbackSchedluer;

        private MultithreadEventLoopGroup group;

        private Bootstrap bootstrap;
        public NettyRemotingClient(RemotingClientOption option) : base(option.PermitsOneway, option.PermitsAsync)
        {
            _option = option;
            _useTls = option.UseTls();
            group = new MultithreadEventLoopGroup();
           
            var threads = Math.Max(4, _option.ClientCallbackExecutorThreads);
            _clientCallbackSchedluer = new OpenNetQTaskScheduler(threads, threads, "NettyRemotingClientCallback_");
        }

        public override OpenNetQTaskScheduler? GetCallbackExecutor()
        {
            return _clientCallbackSchedluer;
        }

        public async Task StartAsync()
        {
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
                    pipeline.AddLast(new MessagePackDecoder());
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4, false));
                    //实体类编码器,心跳管理器,连接管理器
                    pipeline.AddLast(new MessagePackEncoder()
                        , new IdleStateHandler(0, 0, _option.AllIdleTime),
                        new NettyClientConnectManagerHandler(_option),
                        new NettyClientHandler()
                    );
                }));
            try
            {
                _logger.LogInformation("OpenNetQ开始启动----------");
                // bootstrap绑定到指定端口的行为 就是服务端启动服务，同样的Serverbootstrap可以bind到多个端口
                await bootstrap.BindAsync(_option.Port);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"异常:{ex.Message}");
            }

            NettyEventExecutor.Start();
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
            _logger.LogInformation($"OpenNetQ启动完成端口:{_option.Port}----------");
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public void RegisterRPCHook(IRPCHook hook)
        {
            throw new NotImplementedException();
        }

        public void UpdateNameServerAddressList(ICollection<string> addrs)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetNameServerAddressList()
        {
            throw new NotImplementedException();
        }

        public Task<RemotingCommand> InvokeAsync(string addr, RemotingCommand request, long timeoutMillis,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task InvokeCallbackAsync(string addr, RemotingCommand request, long timeoutMillis, Action<ResponseTask> callback,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task InvokeOnewayAsync(string addr, RemotingCommand request, long timeoutMillis,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public void RegisterProcessor(int requestCode, INettyRequestProcessor processor)
        {
            throw new NotImplementedException();
        }

        public bool IsChannelWritable(string addr)
        {
            throw new NotImplementedException();
        }
    }
}