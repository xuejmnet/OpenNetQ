using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace OpenNetQ.Remoting
{
    public interface ChannelEventListener
    {
        void OnChannelConnect(string remoteAddr, IChannel channel);

        void OnChannelClose(string remoteAddr, IChannel channel);

        void OnChannelException(string remoteAddr, IChannel channel);

        void OnChannelIdle(string remoteAddr, IChannel channel);
    }
}
