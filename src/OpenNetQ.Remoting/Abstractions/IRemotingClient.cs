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
        RemotingCommand Invoke(string addr, RemotingCommand request, long timeoutMillis);
        Task<RemotingCommand> InvokeAync(string addr, RemotingCommand request, long timeoutMillis);
        void InvokeCallback(string addr, RemotingCommand request, long timeoutMillis,Action<ResponseTask> callback);
        Task InvokeOnewayAsync(string addr, RemotingCommand request, long timeoutMillis);

        void SendOneway(RemotingCommand command, long timeoutMillis);
        void RegisterProcessor(int requestCode, IMessageRequestProcessor processor);
        bool IsChannelWritable(string addr);
    }
}
