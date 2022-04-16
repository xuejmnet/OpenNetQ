using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Remoting.Netty;
using OpenNetQ.Remoting.Netty.Abstractions;
using OpenNetQ.Remoting.Protocol;
using OpenNetQ.TaskSchedulers;

namespace OpenNetQ.Remoting.Abstractions
{
    public interface IRemotingClient: IBootstrapper
    {
        void UpdateNameServerAddressList(List<string> addrs);

        List<string>? GetNameServerAddressList();
        Task<RemotingCommand> InvokeAsync(string? addr, RemotingCommand request, long timeoutMillis);
        Task InvokeCallbackAsync(string addr, RemotingCommand request, long timeoutMillis,Action<ResponseTask> callback);
        Task InvokeOnewayAsync(string addr, RemotingCommand request, long timeoutMillis);

        void RegisterProcessor(int requestCode, INettyRequestProcessor processor,OpenNetQTaskScheduler? scheduler);
        bool IsChannelWritable(string addr);
    }
}
