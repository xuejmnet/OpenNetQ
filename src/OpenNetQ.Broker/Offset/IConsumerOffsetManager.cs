using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Broker.Offset
{
    public interface IConsumerOffsetManager
    {
        void Persist();
    }
}
