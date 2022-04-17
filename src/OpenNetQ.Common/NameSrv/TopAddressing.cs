using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenNetQ.Logging;

namespace OpenNetQ.Common.NameSrv
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 14:48:15
    /// Email: 326308290@qq.com
    public class TopAddressing
    {
        private static readonly ILogger<TopAddressing> _logger = OpenNetQLoggerFactory.CreateLogger<TopAddressing>();
        private readonly string _wsAddr;
        private readonly string? _unitName;
        public string NSAddr { get; set; }

        public TopAddressing(string wsAddr):this(wsAddr,null)
        {
        }
        public TopAddressing(string wsAddr,string? unitName)
        {
            _wsAddr = wsAddr;
            _unitName = unitName;
        }

        public string FetchNSAddr()
        {
            return FetchNSAddr(true, 3000);
        }
        public string FetchNSAddr(bool verbbose,long timeoutMillis)
        {
            throw new NotImplementedException();
        }
    }
}
