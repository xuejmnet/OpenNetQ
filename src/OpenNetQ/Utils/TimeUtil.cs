using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Utils
{
    public class TimeUtil
    {
        private TimeUtil()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMillis()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
