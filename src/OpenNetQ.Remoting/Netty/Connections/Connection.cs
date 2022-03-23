using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using OpenNetQ.Extensions;
using OpenNetQ.Remoting.Abstractions.Connections;
using OpenNetQ.Remoting.Common;

namespace OpenNetQ.Remoting.Netty.Connections
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: Tuesday, 17 November 2020 10:44:14
    * @Email: 326308290@qq.com
    */
    public class Connection : IConnection
    {
        private readonly RemotingClientOption _option;
        private readonly string _targetHost;
        private readonly IPEndPoint _ipEndPoint;
        private static readonly int LOCK_TIMEOUT_MILLIS = 3000;
        private static readonly object LockObj = new object();
        private static readonly IInternalLogger _logger = InternalLoggerFactory.GetInstance<Connection>();

        /// <summary>
        /// 服务启动
        /// </summary>
        private MultithreadEventLoopGroup group;

        private Bootstrap bootstrap;
        private IChannel clientChannel;

        public Connection(RemotingClientOption option)
        {
            _option = option;
            _targetHost = _option.TlsCertificate.GetNameInfo(X509NameType.DnsName, false);
            _ipEndPoint = new IPEndPoint(_option.Host, _option.Port);
            InitNetty();
        }

        private void InitNetty()
        {
            try
            {
                group = new MultithreadEventLoopGroup();
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
                        pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(_targetHost)));
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常:{ex.Message}");
                throw ex;
            }
        }

        public bool IsOpen => clientChannel != null && clientChannel.Active;

        public IChannel CreateChannel()
        {
            return DoCreateChannel();
        }


        private async Task<IChannel> ConnectAsync()
        {
            return await bootstrap.ConnectAsync(_ipEndPoint);
        }
        //private IChannel ConnectAsync()
        //{
        //    return await bootstrap.ConnectAsync(_ipEndPoint);
        //}

        private IChannel DoCreateChannel()
        {
            if (IsOpen)
                return clientChannel;
            if (Monitor.TryEnter(LockObj, TimeSpan.FromMilliseconds(LOCK_TIMEOUT_MILLIS)))
            {
                try
                {
                    if (IsOpen)
                        return clientChannel;
                    clientChannel = ConnectAsync().WaitAndUnwrapException();
                    //clientChannel = ConnectAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"createChannel: create channel exception:[{e}]");
                    _logger.Error($"createChannel: create channel exception", e);
                }
                finally
                {
                    Monitor.Exit(LockObj);
                }
            }
            else
            {
                Console.WriteLine($"createChannel: try to lock channel , but timeout, {LOCK_TIMEOUT_MILLIS}ms");
                _logger.Warn($"createChannel: try to lock channel , but timeout, {LOCK_TIMEOUT_MILLIS}ms");
            }

            if (clientChannel != null)
            {
                if (clientChannel.Active)
                    return clientChannel;
                Console.WriteLine($"createChannel: connect remote host[{_ipEndPoint.Address}] failed");
                _logger.Warn($"createChannel: connect remote host[{_ipEndPoint.Address}] failed");
            }

            return null;
        }

        public void Dispose()
        {
            try
            {
                RemotingUtil.CloseChannel(clientChannel);

                group?.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine($"connection dispose error. [{e}]");
                _logger.Error("connection dispose error.", e);
            }
        }
    }
}