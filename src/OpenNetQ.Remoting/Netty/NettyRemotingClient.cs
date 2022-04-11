using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using J2N.Threading.Atomic;
using Microsoft.Extensions.Logging;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Netty.Abstractions;
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

        // 主工作线程组，设置为1个线程
        private IEventLoopGroup bossGroup;

        // 工作线程组，默认为内核数*2的线程数
        private IEventLoopGroup workerGroup;
        public NettyRemotingClient(RemotingClientOption option) : base(option.PermitsOneway, option.PermitsAsync)
        {
            _option = option;
            _useTls=option.UseTls();
            //libuv
            var useLibuv = true;
            if (useLibuv)
            {
                var dispatcher = new DispatcherEventLoopGroup();
                bossGroup = dispatcher;
                workerGroup = new WorkerEventLoopGroup(dispatcher);
            }
            else
            {
                bossGroup = new MultithreadEventLoopGroup(1);
                workerGroup = new MultithreadEventLoopGroup();
            }
            var threads = Math.Max(4, _option.ClientCallbackExecutorThreads);
            _clientCallbackSchedluer = new OpenNetQTaskScheduler(threads, threads,"NettyRemotingClientCallback_");
        }

        public override OpenNetQTaskScheduler? GetCallbackExecutor()
        {
            return _clientCallbackSchedluer;
        }

        public Task StartAsync()
        {
            throw new NotImplementedException();
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
