using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenNetQ.Logging;
using OpenNetQ.Utils;

namespace OpenNetQ.Remoting.Abstractions
{
    public abstract class AbstractServiceThread: IServiceThread
    {
        private static readonly ILogger _logger = OpenNetQLoggerFactory.CreateLogger(LoggerName.COMMON_LOGGER_NAME);

        private static readonly long JOIN_TIME = 90 * 1000;
        protected  Thread thread;

        private readonly CountdownEvent _waitPoint = new CountdownEvent(1);
        private  volatile AtomicBoolean _hasNotified = new AtomicBoolean(false);

        protected volatile bool stopped = false;
        private readonly AtomicBoolean started = new AtomicBoolean(false);
        public bool IsBackground { get; set; } = false;

        public AbstractServiceThread()
        {
        }

        public abstract string GetServiceName();

        public void Start()
        {
            _logger.LogInformation($"Try to start service thread:{GetServiceName()} started:{started.Value} lastThread:{thread}");
            if (!started.CompareAndSet(false, true))
            {
                return;
            }
            this.stopped = false;

            this.thread = new Thread(Run);
            thread.Name = GetServiceName();
            thread.IsBackground = IsBackground;
            this.thread.Start();
        }

        public void Shutdown(bool interrupt=false)
        {

            _logger.LogInformation($"Try to shutdown service thread:{GetServiceName()} started:{started.Value} lastThread:{thread}");
            if (!started.CompareAndSet(true, false))
            {
                return;
            }


            this.stopped = true;
            _logger.LogInformation($"shutdown thread {GetServiceName()} interrupt {interrupt}");

            if (_hasNotified.CompareAndSet(false, true))
            {
                _waitPoint.Signal();
            }
            try
            {
                if (interrupt)
                {
                    this.thread.Interrupt();
                }

                long beginTime = TimeUtil.CurrentTimeMillis();

                if (!thread.IsBackground)
                {
                    this.thread.Join(TimeSpan.FromMilliseconds(GetJointime()));
                }

                long elapsedTime = TimeUtil.CurrentTimeMillis() - beginTime;
                _logger.LogInformation($"join thread {GetServiceName()} elapsed time(ms) {elapsedTime} join time {GetJointime()}");
            }
            catch (SecurityException e)
            {
                _logger.LogError(e,"Interrupted");
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

        public void Wakeup()
        {
            if (_hasNotified.CompareAndSet(false, true))
            {
                _waitPoint.Signal();
            }
        }

        public void WaitForRunning(int interval)
        {
            if (_hasNotified.CompareAndSet(true, false))
            {
                OnWaitEnd();
                return;
            }
            _waitPoint.Reset();
            try
            {
                _waitPoint.Wait(TimeSpan.FromMilliseconds(interval));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "wait point");
                throw;
            }
            finally
            {
                _hasNotified.Value = false;
                OnWaitEnd();
            }
        }

        public virtual void OnWaitEnd()
        {

        }
    }
}
