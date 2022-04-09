using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.TaskSchedulers;

namespace OpenNetQ.Extensions
{
    public static class TaskSchedulerExtension
    {
        public static Task RunFixedRate(this OpenNetQTaskScheduler scheduler,Action action,TimeSpan initialDelay ,TimeSpan period, Action<Exception>? errorAction = null)
        {
            var fixedRateSchedule = Task.Factory.StartNew(async () => {
                {
                    var initialDelayTotalMilliseconds = initialDelay.TotalMilliseconds;
                    if (initialDelayTotalMilliseconds > 0)
                    {
                        await Task.Delay(initialDelay);
                    }
                    while (true)
                    {
                        try
                        {
                            action();
                            await Task.Delay(period);
                        }
                        catch (Exception ex)
                        {
                            if (errorAction != null) errorAction(ex);
                            throw;
                        }
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.None, scheduler);
            return fixedRateSchedule;
        }
        #region 线程中执行
        /// <summary>
        /// 线程中执行
        /// </summary>
        public static Task Run(this TaskScheduler scheduler, Action<object?> doWork, object? arg = null, Action<Exception>? errorAction = null)
        {
            return Task.Factory.StartNew((obj) =>
            {
                try
                {
                    doWork(obj);
                }
                catch (Exception ex)
                {
                    if (errorAction != null) errorAction(ex);
                    //LogUtil.Error(ex, "RunHelper.Run错误");
                }
            }, arg, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        #endregion

        #region 线程中执行
        /// <summary>
        /// 线程中执行
        /// </summary>
        public static Task Run(this TaskScheduler scheduler, Action doWork, Action<Exception>? errorAction = null)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    doWork();
                }
                catch (Exception ex)
                {
                    if (errorAction != null) errorAction(ex);
                    //LogUtil.Error(ex, "RunHelper.Run错误");
                }
            }, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        #endregion

        #region 线程中执行
        /// <summary>
        /// 线程中执行
        /// </summary>
        public static Task<T> Run<T>(this TaskScheduler scheduler, Func<object?, T> doWork, object? arg = null, Action<Exception>? errorAction = null)
        {
            return Task.Factory.StartNew<T>((obj) =>
            {
                try
                {
                    return doWork(obj);
                }
                catch (Exception ex)
                {
                    if (errorAction != null) errorAction(ex);
                    //LogUtil.Error(ex, "RunHelper.Run错误");
                    throw;
                }
            }, arg, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        #endregion

        #region 线程中执行
        /// <summary>
        /// 线程中执行
        /// </summary>
        public static Task<T> Run<T>(this TaskScheduler scheduler, Func<T> doWork, Action<Exception>? errorAction = null)
        {
            return Task.Factory.StartNew<T>(() =>
            {
                try
                {
                    return doWork();
                }
                catch (Exception ex)
                {
                    if (errorAction != null) errorAction(ex);
                    //LogUtil.Error(ex, "RunHelper.Run错误");
                    throw;
                }
            }, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        #endregion

        #region 线程中执行
        /// <summary>
        /// 线程中执行
        /// </summary>
        public static async Task<T> RunAsync<T>(this TaskScheduler scheduler, Func<object?, T> doWork, object? arg = null, Action<Exception>? errorAction = null)
        {
            return await Task.Factory.StartNew<T>((obj) =>
            {
                try
                {
                    return doWork(obj);
                }
                catch (Exception ex)
                {
                    if (errorAction != null) errorAction(ex);
                    //LogUtil.Error(ex, "RunHelper.RunAsync错误");
                    throw;
                }
            }, arg, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        #endregion

        #region 线程中执行
        /// <summary>
        /// 线程中执行
        /// </summary>
        public static async Task<T> RunAsync<T>(this TaskScheduler scheduler, Func<T> doWork, Action<Exception>? errorAction = null)
        {
            return await Task.Factory.StartNew<T>(() =>
            {
                try
                {
                    return doWork();
                }
                catch (Exception ex)
                {
                    if (errorAction != null) errorAction(ex);
                    //LogUtil.Error(ex, "RunHelper.RunAsync错误");
                    throw;
                }
            }, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        #endregion

        #region 线程中执行
        /// <summary>
        /// 线程中执行
        /// </summary>
        public static async Task RunAsync(this TaskScheduler scheduler, Action<object?> doWork, object? arg = null, Action<Exception>? errorAction = null)
        {
            await Task.Factory.StartNew((obj) =>
            {
                try
                {
                    doWork(obj);
                }
                catch (Exception ex)
                {
                    if (errorAction != null) errorAction(ex);
                    throw;
                }
            }, arg, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        #endregion

        #region 线程中执行
        /// <summary>
        /// 线程中执行
        /// </summary>
        public static async Task RunAsync(this TaskScheduler scheduler, Action doWork, Action<Exception>? errorAction = null)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    doWork();
                }
                catch (Exception ex)
                {
                    if (errorAction != null) errorAction(ex);
                    //LogUtil.Error(ex, "RunHelper.RunAsync错误");
                    throw;
                }
            }, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        #endregion

    }
}
