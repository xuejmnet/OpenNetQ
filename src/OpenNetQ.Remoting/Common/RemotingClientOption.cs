using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Remoting.Common
{
    public class RemotingClientOption
    {
        public int ClientCallbackExecutorThreads { get; set; } = Environment.ProcessorCount;
        /// <summary>
        /// 单向发送的并发值
        /// </summary>
        public int PermitsOneway { get; set; } = 65536;

        /// <summary>
        /// 异步发送的并发值
        /// </summary>
        public int PermitsAsync { get; set; } = 65536;

        /// <summary>
        /// tsl证书
        /// </summary>
        public X509Certificate2? TlsCertificate { get; set; }
        /// <summary>
        /// 心跳间隔秒
        /// </summary>
        public int AllIdleTime { get; set; } = 30;
        public bool UseTls()
        {
            return TlsCertificate != null;
        }
    }
}
