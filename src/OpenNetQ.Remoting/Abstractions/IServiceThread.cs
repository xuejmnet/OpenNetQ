using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Remoting.Abstractions
{
    public interface IServiceThread
    {
        void Start();
        void Shutdown(bool interrupt=false);
        void Run();
    }
}
