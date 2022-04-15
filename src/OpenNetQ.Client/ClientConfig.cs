using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/15 17:20:43
    /// Email: 326308290@qq.com
    public class ClientConfig
    {
        private string _namesrvAddr;
        private string _clientIP;
        private string _instanceName = "DEFAULT";
        private int _clientCallbackExecutorThreads = Environment.ProcessorCount;
        protected string Namespace { get; set; }
        private bool _namespaceInitialized = false;
        protected AccessChannelEnum AccessChannel = AccessChannelEnum.LOCAL;
        private int _pullNameServerInterval = 1000 * 30;
    }
}
