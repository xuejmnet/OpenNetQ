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
        private static readonly IInternalNetQLogger _log = InternalNetQLoggerFactory.GetLogger<NettyEventExecutor>();
        private readonly BlockingCollection<NettyEventArg> eventQueue =
            new BlockingCollection<NettyEventArg>(new ConcurrentQueue<NettyEventArg>());
        private readonly int maxSize = 10000;

        public void PutNettyEvent(NettyEventArg eventArg)
        {
            if (this.eventQueue.Count <= maxSize)
            {
                this.eventQueue.Add(eventArg);
            }
            else
            {
                _log.Warn($"event queue size[{this.eventQueue.Count}] enough, so drop this event {eventArg}");
            }
        }

        public event EventHandler<NettyEventArg> OnChannelIdle;
        public event EventHandler<NettyEventArg> OnChannelClose;
        public event EventHandler<NettyEventArg> OnChannelConnect;
        public event EventHandler<NettyEventArg> OnChannelException;
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
                                OnChannelIdle?.Invoke(this,@event);
                                break;
                            case NettyEventTypeEnum.CLOSE:
                                OnChannelClose?.Invoke(this,@event);
                                break;
                            case NettyEventTypeEnum.CONNECT:
                                OnChannelConnect?.Invoke(this,@event);
                                break;
                            case NettyEventTypeEnum.EXCEPTION:
                                OnChannelException.Invoke(this,@event);
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
