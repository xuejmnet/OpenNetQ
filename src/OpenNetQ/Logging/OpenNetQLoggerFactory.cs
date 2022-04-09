using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OpenNetQ.Logging
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/9 9:22:45
    /// Email: 326308290@qq.com
    public class OpenNetQLoggerFactory
    {
        private OpenNetQLoggerFactory()
        {
            
        }
        public static ILoggerFactory DefaultFactory { get; set; }
        public static ILogger CreateLogger<T>() => DefaultFactory.CreateLogger<T>();
        public static ILogger CreateLogger(string categoryName) => DefaultFactory.CreateLogger(categoryName);
    }
}
