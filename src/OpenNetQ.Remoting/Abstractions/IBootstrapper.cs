using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Remoting.Abstractions
{
    public interface IBootstrapper
    {
        Task StartAsync();
        Task StopAsync();
        void RegisterRPCHook(IRPCHook? hook);
    }
}
