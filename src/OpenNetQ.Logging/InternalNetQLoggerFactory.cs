using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace OpenNetQ.Logging
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 14 December 2020 07:52:45
* @Email: 326308290@qq.com
*/
    public class InternalNetQLoggerFactory
    {
        static ILoggerFactory defaultFactory;


        static ILoggerFactory NewDefaultFactory(string name)
        {
            var f = new LoggerFactory();
            return f;
        }

        /// <summary>
        ///     Gets or sets the default factory.
        /// </summary>
        public static ILoggerFactory DefaultFactory
        {
            get
            {
                ILoggerFactory factory = Volatile.Read(ref defaultFactory);
                if (factory == null)
                {
                    factory = NewDefaultFactory(typeof(InternalNetQLoggerFactory).FullName);
                    ILoggerFactory current = Interlocked.CompareExchange(ref defaultFactory, factory, null);
                    if (current != null)
                    {
                        return current;
                    }
                }
                return factory;
            }
            set
            {
                Contract.Requires(value != null);

                Volatile.Write(ref defaultFactory, value);
            }
        }

        /// <summary>
        ///     Creates a new logger instance with the name of the specified type.
        /// </summary>
        /// <typeparam name="T">type where logger is used</typeparam>
        /// <returns>logger instance</returns>
        public static IInternalNetQLogger GetLogger<T>() => GetLogger(typeof(T));

        /// <summary>
        ///     Creates a new logger instance with the name of the specified type.
        /// </summary>
        /// <param name="type">type where logger is used</param>
        /// <returns>logger instance</returns>
        public static IInternalNetQLogger GetLogger(Type type) => GetLogger(type.FullName);

        /// <summary>
        ///     Creates a new logger instance with the specified name.
        /// </summary>
        /// <param name="name">logger name</param>
        /// <returns>logger instance</returns>
        public static IInternalNetQLogger GetLogger(string name) => new GenericNetQLogger(name, DefaultFactory.CreateLogger(name));
    
    }
}