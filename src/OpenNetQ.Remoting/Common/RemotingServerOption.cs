using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Remoting.Common
{
    /// <summary>
    /// 服务端配置
    /// </summary>
    public class RemotingServerOption
    {
        /// <summary>
        /// 监听端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 心跳间隔秒
        /// </summary>
        public int AllIdleTime { get; set; }

        /// <summary>
        /// 批处理单次处理数量
        /// </summary>
        public int BatchSize { get; set; } = 10000;

        /// <summary>
        /// 提交间隔毫秒
        /// </summary>
        public int CommitIntervalMillis { get; set; } = 500;
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

        public bool UseTls()
        {
            return TlsCertificate != null;
        }
    }
}
