using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace OpenNetQ.Remoting.Netty
{
    public class NettyEvent
    {
        private readonly NettyEventTypeEnum nettyEventType;
        private readonly String remoteAddr;
        private readonly IChannel channel;

        public NettyEvent(NettyEventTypeEnum nettyEventType, String remoteAddr, IChannel channel)
        {
            this.nettyEventType = nettyEventType;
            this.remoteAddr = remoteAddr;
            this.channel = channel;
        }

        public NettyEventTypeEnum GetNettyEventType()
        {
            return nettyEventType;
        }

        public string GetRemoteAddr()
        {
            return remoteAddr;
        }

        public IChannel GetChannel()
        {
            return channel;
        }

        public override string ToString()
        {
            return "NettyEvent [type=" + nettyEventType + ", remoteAddr=" + remoteAddr + ", channel=" + channel + "]";
        }
    }
}
