using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Netty;

namespace OpenNetQ.Remoting.Abstractions
{
    public class NettyEventExecutor : AbstractServiceThread
    {
        private readonly ChannelEventListener _listener;
        private static readonly IInternalNetQLogger _log = InternalNetQLoggerFactory.GetLogger<NettyEventExecutor>();
        private readonly BlockingCollection<NettyEvent> eventQueue =
            new BlockingCollection<NettyEvent>();
        private readonly int maxSize = 10000;

        public NettyEventExecutor(ChannelEventListener listener)
        {
            _listener = listener;
        }
        public void PutNettyEvent(NettyEvent @event)
        {
            if (this.eventQueue.Count <= maxSize)
            {
                this.eventQueue.Add(@event);
            }
            else
            {
                _log.Warn($"event queue size[{this.eventQueue.Count}] enough, so drop this event {@event}");
            }
        }
        public override string GetServiceName()
        {
            return nameof(NettyEventExecutor);
        }

        public override void Run()
        {

            _log.Info(this.GetServiceName() + " service started");


            while (!this.IsStopped())
            {
                try
                {
                    if (this.eventQueue.TryTake(out var @event, TimeSpan.FromSeconds(3)))
                    {
                        switch (@event.GetNettyEventType())
                        {
                            case NettyEventTypeEnum.IDLE:
                                _listener.OnChannelIdle(@event.GetRemoteAddr(), @event.GetChannel());
                                break;
                            case NettyEventTypeEnum.CLOSE:
                                _listener.OnChannelClose(@event.GetRemoteAddr(), @event.GetChannel());
                                break;
                            case NettyEventTypeEnum.CONNECT:
                                _listener.OnChannelConnect(@event.GetRemoteAddr(), @event.GetChannel());
                                break;
                            case NettyEventTypeEnum.EXCEPTION:
                                _listener.OnChannelException(@event.GetRemoteAddr(), @event.GetChannel());
                                break;
                            default:
                                break;

                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Warn(this.GetServiceName() + " service has exception. ", e);
                }
            }

            _log.Warn(this.GetServiceName() + " service end");
        }
    }
}
