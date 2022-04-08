using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Broker.Slave
{
    public interface ISlaveSynchronize
    {
        void SetMasterAddr(string? masterAddr);
    }
}
