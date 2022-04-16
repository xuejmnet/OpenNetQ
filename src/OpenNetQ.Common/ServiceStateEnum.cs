using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 14:31:39
    /// Email: 326308290@qq.com
    public enum ServiceStateEnum
    {
        /**
         * Service just created,not start
         */
        CREATE_JUST,
        /**
         * Service Running
         */
        RUNNING,
        /**
         * Service shutdown
         */
        SHUTDOWN_ALREADY,
        /**
         * Service Start failure
         */
        START_FAILED;
    }
}
