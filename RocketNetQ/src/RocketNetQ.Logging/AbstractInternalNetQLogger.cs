// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using KhaosLog.LoggingProvider;

namespace RocketNetQ.Logging
{
    /// <summary>
    /// A skeletal implementation of <see cref="IInternalNetQLogger"/>. This class implements
    /// all methods that have a <see cref="InternalNetQLogLevel"/> parameter by default to call
    /// specific logger methods such as <see cref="Info(string)"/> or <see cref="InfoEnabled"/>.
    /// </summary>
    public abstract class AbstractInternalNetQLogger : IInternalNetQLogger
    {
        static readonly string EXCEPTION_MESSAGE = "Unexpected exception:";

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name">A friendly name for the new logger instance.</param>
        protected AbstractInternalNetQLogger(string name)
        {
            Contract.Requires(name != null);

            this.Name = name;
        }

        public string Name { get; }

        public bool IsEnabled(InternalNetQLogLevel level)
        {
            switch (level)
            {
                case InternalNetQLogLevel.TRACE:
                    return this.TraceEnabled;
                case InternalNetQLogLevel.DEBUG:
                    return this.DebugEnabled;
                case InternalNetQLogLevel.INFO:
                    return this.InfoEnabled;
                case InternalNetQLogLevel.WARN:
                    return this.WarnEnabled;
                case InternalNetQLogLevel.ERROR:
                    return this.ErrorEnabled;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract bool TraceEnabled { get; }

        public abstract void Trace(string msg);

        public abstract void Trace(string format, object arg);

        public abstract void Trace(string format, object argA, object argB);

        public abstract void Trace(string format, params object[] arguments);

        public abstract void Trace(string msg, Exception t);

        public void Trace(Exception t) => this.Trace(EXCEPTION_MESSAGE, t);

        public abstract bool DebugEnabled { get; }

        public abstract void Debug(string msg);

        public abstract void Debug(string format, object arg);

        public abstract void Debug(string format, object argA, object argB);

        public abstract void Debug(string format, params object[] arguments);

        public abstract void Debug(string msg, Exception t);

        public void Debug(Exception t) => this.Debug(EXCEPTION_MESSAGE, t);

        public abstract bool InfoEnabled { get; }

        public abstract void Info(string msg);

        public abstract void Info(string format, object arg);

        public abstract void Info(string format, object argA, object argB);

        public abstract void Info(string format, params object[] arguments);

        public abstract void Info(string msg, Exception t);

        public void Info(Exception t) => this.Info(EXCEPTION_MESSAGE, t);

        public abstract bool WarnEnabled { get; }

        public abstract void Warn(string msg);

        public abstract void Warn(string format, object arg);

        public abstract void Warn(string format, params object[] arguments);

        public abstract void Warn(string format, object argA, object argB);

        public abstract void Warn(string msg, Exception t);

        public void Warn(Exception t) => this.Warn(EXCEPTION_MESSAGE, t);

        public abstract bool ErrorEnabled { get; }

        public abstract void Error(string msg);

        public abstract void Error(string format, object arg);

        public abstract void Error(string format, object argA, object argB);

        public abstract void Error(string format, params object[] arguments);

        public abstract void Error(string msg, Exception t);

        public void Error(Exception t) => this.Error(EXCEPTION_MESSAGE, t);

        public void Log(InternalNetQLogLevel level, string msg, Exception cause)
        {
            switch (level)
            {
                case InternalNetQLogLevel.TRACE:
                    this.Trace(msg, cause);
                    break;
                case InternalNetQLogLevel.DEBUG:
                    this.Debug(msg, cause);
                    break;
                case InternalNetQLogLevel.INFO:
                    this.Info(msg, cause);
                    break;
                case InternalNetQLogLevel.WARN:
                    this.Warn(msg, cause);
                    break;
                case InternalNetQLogLevel.ERROR:
                    this.Error(msg, cause);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Log(InternalNetQLogLevel level, Exception cause)
        {
            switch (level)
            {
                case InternalNetQLogLevel.TRACE:
                    this.Trace(cause);
                    break;
                case InternalNetQLogLevel.DEBUG:
                    this.Debug(cause);
                    break;
                case InternalNetQLogLevel.INFO:
                    this.Info(cause);
                    break;
                case InternalNetQLogLevel.WARN:
                    this.Warn(cause);
                    break;
                case InternalNetQLogLevel.ERROR:
                    this.Error(cause);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Log(InternalNetQLogLevel level, string msg)
        {
            switch (level)
            {
                case InternalNetQLogLevel.TRACE:
                    this.Trace(msg);
                    break;
                case InternalNetQLogLevel.DEBUG:
                    this.Debug(msg);
                    break;
                case InternalNetQLogLevel.INFO:
                    this.Info(msg);
                    break;
                case InternalNetQLogLevel.WARN:
                    this.Warn(msg);
                    break;
                case InternalNetQLogLevel.ERROR:
                    this.Error(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Log(InternalNetQLogLevel level, string format, object arg)
        {
            switch (level)
            {
                case InternalNetQLogLevel.TRACE:
                    this.Trace(format, arg);
                    break;
                case InternalNetQLogLevel.DEBUG:
                    this.Debug(format, arg);
                    break;
                case InternalNetQLogLevel.INFO:
                    this.Info(format, arg);
                    break;
                case InternalNetQLogLevel.WARN:
                    this.Warn(format, arg);
                    break;
                case InternalNetQLogLevel.ERROR:
                    this.Error(format, arg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Log(InternalNetQLogLevel level, string format, object argA, object argB)
        {
            switch (level)
            {
                case InternalNetQLogLevel.TRACE:
                    this.Trace(format, argA, argB);
                    break;
                case InternalNetQLogLevel.DEBUG:
                    this.Debug(format, argA, argB);
                    break;
                case InternalNetQLogLevel.INFO:
                    this.Info(format, argA, argB);
                    break;
                case InternalNetQLogLevel.WARN:
                    this.Warn(format, argA, argB);
                    break;
                case InternalNetQLogLevel.ERROR:
                    this.Error(format, argA, argB);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Log(InternalNetQLogLevel level, string format, params object[] arguments)
        {
            switch (level)
            {
                case InternalNetQLogLevel.TRACE:
                    this.Trace(format, arguments);
                    break;
                case InternalNetQLogLevel.DEBUG:
                    this.Debug(format, arguments);
                    break;
                case InternalNetQLogLevel.INFO:
                    this.Info(format, arguments);
                    break;
                case InternalNetQLogLevel.WARN:
                    this.Warn(format, arguments);
                    break;
                case InternalNetQLogLevel.ERROR:
                    this.Error(format, arguments);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString() => this.GetType().Name + '(' + this.Name + ')';
    }
}