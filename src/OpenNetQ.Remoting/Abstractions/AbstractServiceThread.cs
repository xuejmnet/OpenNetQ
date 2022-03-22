using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Logging;

namespace OpenNetQ.Remoting.Abstractions
{
    public abstract class AbstractServiceThread: IServiceThread
    {
        private static readonly IInternalNetQLogger _log = InternalNetQLoggerFactory.GetLogger<AbstractNettyRemoting>();

        private static readonly long JOIN_TIME = 90 * 1000;
        protected readonly Thread thread;
        protected volatile bool stopped = false;

        public AbstractServiceThread()
        {
            this.thread = new Thread(Run);
            thread.Name = GetServiceName();
        }

        public abstract string GetServiceName();

        public void Start()
        {
            this.thread.Start();
        }
        public void Shutdown(bool interrupt=false)
        {
            this.stopped = true;
            _log.Info("shutdown thread " + this.GetServiceName() + " interrupt " + interrupt);
         

            try
            {
                if (interrupt)
                {
                    this.thread.Interrupt();
                }

                long beginTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                this.thread.Join(TimeSpan.FromMilliseconds(GetJointime()));
                long elapsedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - beginTime;
                _log.Info("join thread " + this.GetServiceName() + " elapsed time(ms) " + elapsedTime + " "
                         + this.GetJointime());
            }
            catch (SecurityException e)
            {
                _log.Error("Interrupted", e);
            }
        }

        public long GetJointime()
        {
            return JOIN_TIME;
        }

        public bool IsStopped()
        {
            return stopped;
        }
        public abstract void Run();
    }
}
