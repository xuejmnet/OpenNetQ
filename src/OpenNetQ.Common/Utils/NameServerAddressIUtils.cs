using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNetQ.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/15 17:21:54
    /// Email: 326308290@qq.com
    public class NameServerAddressIUtils
    {
        public static readonly string INSTANCE_PREFIX = "MQ_INST_";
        public static readonly string INSTANCE_REGEX = INSTANCE_PREFIX + "\\w+_\\w+";
        public static readonly string ENDPOINT_PREFIX = "(\\w+://|)";
        public static readonly Regex NAMESRV_ENDPOINT_PATTERN = new Regex("^http://.*");
        public static readonly Regex INST_ENDPOINT_PATTERN = new Regex("^" + ENDPOINT_PREFIX + INSTANCE_REGEX + "\\..*");



    }
}
