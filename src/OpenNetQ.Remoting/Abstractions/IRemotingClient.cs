using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Remoting.Netty;
using OpenNetQ.Remoting.Netty.Abstractions;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Remoting.Abstractions
{
    public interface IRemotingClient: IBootstrapper
    {
        void UpdateNameServerAddressList(ICollection<string> addrs);
       
        ICollection<string> GetNameServerAddressList();
        Task<RemotingCommand> InvokeAsync(string addr, RemotingCommand request, long timeoutMillis, CancellationToken cancellationToken = new CancellationToken());
        Task InvokeCallbackAsync(string addr, RemotingCommand request, long timeoutMillis,Action<ResponseTask> callback, CancellationToken cancellationToken = new CancellationToken());
        Task InvokeOnewayAsync(string addr, RemotingCommand request, long timeoutMillis, CancellationToken cancellationToken = new CancellationToken());

        void RegisterProcessor(int requestCode, INettyRequestProcessor processor);
        bool IsChannelWritable(string addr);
    }
}
