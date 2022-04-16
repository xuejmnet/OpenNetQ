using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Remoting.Abstractions
{
    public interface IRPCHook
    {
        Task DoBeforeRequest(string? remoteAddr, RemotingCommand request);
        Task DoAfterResponse(string? remoteAddr, RemotingCommand request, RemotingCommand? response);
    }
}
