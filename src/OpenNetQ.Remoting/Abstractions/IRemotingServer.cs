using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using OpenNetQ.Remoting.Netty;
using OpenNetQ.Remoting.Netty.Abstractions;
using OpenNetQ.Remoting.Protocol;
using OpenNetQ.TaskSchedulers;

namespace OpenNetQ.Remoting.Abstractions
{
    public interface IRemotingServer: IBootstrapper
    {
        void RegisterProcessor(int requestCode, INettyRequestProcessor processor, OpenNetQTaskScheduler scheduler);

        void RegisterDefaultProcessor(INettyRequestProcessor processor, OpenNetQTaskScheduler scheduler);

        int LocalListenPort();

        Tuple<INettyRequestProcessor, OpenNetQTaskScheduler>? GetProcessorPair(int requestCode);

        Task<RemotingCommand> InvokeAsync(IChannel channel, RemotingCommand request, long timeoutMillis);
        Task InvokeCallbackAsync(IChannel channel, RemotingCommand request, long timeoutMillis, Action<ResponseTask> callback);
        Task InvokeOnewayAsync(IChannel channel, RemotingCommand request, long timeoutMillis);

    }
}
