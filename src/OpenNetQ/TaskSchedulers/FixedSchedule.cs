using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.TaskSchedulers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/1 9:06:30
    /// Email: 326308290@qq.com
    public class FixedSchedule
    {
        private readonly string _name;
        private readonly TimeSpan _initDelay;
        private readonly TimeSpan _period;
        public Task? FixedRateSchedule { get; private set; }
        private CancellationTokenSource _cts;
        public event EventHandler<string> OnLog;

        public FixedSchedule(string name,TimeSpan initDelay,TimeSpan period)
        {
            _name = name;
            _initDelay = initDelay;
            _period = period;
            _cts = new CancellationTokenSource();
        }

        //public static FixedSchedule RunFixedRate(Action<object> action)
        //{
        //    var fixedRateSchedule= Task.Factory.StartNew(()=>{
        //    {
        //        RunLoop
        //    }
        //    }, null, TaskCreationOptions.LongRunning);
        //    return new FixedSchedule(fixedRateSchedule);
        //}

        public ValueTask StartAsync(Func<CancellationToken,Task> func)
        {
            FixedRateSchedule = Task.Factory.StartNew(async () =>
            {
                await RunLoop(func);
            }, TaskCreationOptions.LongRunning);
            return ValueTask.CompletedTask;
        }
        private async Task RunLoop(Func<CancellationToken, Task> func)
        {
            await Task.Delay(_initDelay);
            while (!IsStop())
            {

                try
                {
                   await func(_cts.Token);

                }
                catch (Exception e)
                {
                    OnLog?.Invoke(this, Format($"do commit error. {e}"));
                }

                await Task.Delay(_period);
            }
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            catch (Exception e)
            {
                OnLog?.Invoke(this, Format($"WriteMessageService Exception. {e}"));
            }

            OnLog?.Invoke(this, Format($"CommitMessageBufferPoolProcessorV2 RunWrite end."));
        }

        private bool IsStop()
        {
            return _cts.IsCancellationRequested;
        }
        public async ValueTask StopAsync()
        {
            if (IsStop())
            {
                return;
            }
            _cts.Cancel();

            try
            {
                if (FixedRateSchedule != null)
                {
                    await FixedRateSchedule.WaitAsync(TimeSpan.FromMilliseconds(100));
                }
            }
            catch (Exception e)
            {
                OnLog?.Invoke(this, Format($"stop error:{e} "));
            }
            OnLog?.Invoke(this, Format($"CommitMessageBufferPoolProcessorV2 RunWrite end."));
        }

        private string Format(string msg)
        {
            return $"{nameof(FixedRateSchedule)}:{_name},{msg}";
        }
    }
}
