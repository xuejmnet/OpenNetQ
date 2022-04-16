using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenNetQ.Common.Constant;
using OpenNetQ.Core;
using OpenNetQ.Remoting.Common;

namespace OpenNetQ.Common.Options
{
    public class BrokerOption
    {
        public string BrokerClusterName { get; set; } = "DefaultCluster";
        public string BrokerIP1 { get; set; } = RemotingUtil.GetLocalAddress();
        public string BrokerIP2 { get; set; } = RemotingUtil.GetLocalAddress();
        public string BrokerName { get; set; } = LocalHostName();
        public long BrokerId { get; set; } = 0L;
        public int RegisterBrokertimeoutMillis { get; set; } = 6000;
        public bool IsCompressedRegister { get; set; } = false;
        public bool IsForceRegister { get; set; } = true;
        public int RegisterNameServerPeriod { get; set; }=30000;



        public static string LocalHostName()
        {
            try
            {
                return  Dns.GetHostName();
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Failed to obtain the host name : {e}");
                return "DEFAULT_BROKER";
            }

        }
        public int BrokerPermission { get; set; } = PermName.PERM_READ | PermName.PERM_WRITE;
        /// <summary>
        /// 是否启用轻量级消息队列
        /// </summary>
        public bool EnableLightMessageQueue { get; set; } = false;

        /// <summary>
        /// 发送消息的线程池数
        /// </summary>
        public int SendMessageThreadPoolCount { get; set; } = Math.Min(Environment.ProcessorCount, 4);
        /// <summary>
        /// 推送消息线程池数目
        /// </summary>
        public int PushMessageThreadPoolCount { get; set; } = Math.Min(Environment.ProcessorCount, 4);
        /// <summary>
        /// 拉去消息线程数
        /// </summary>
        public int PullMessageThreadPoolCount { get; set; } = 16 + Environment.ProcessorCount * 2;
        /// <summary>
        /// 处理消息线程池数
        /// </summary>
        public int ProcessReplyMessageThreadPoolCount { get; set; } = 16 + Environment.ProcessorCount * 2;
        /// <summary>
        /// 查询消息线程池数
        /// </summary>
        public int QueryMessageThreadPoolCount { get; set; } = 8 + Environment.ProcessorCount;

        public int AdminBrokerThreadPoolCount { get; set; } = 16;
        public int ClientManageThreadPoolCount { get; set; } = 32;
        public int ConsumerManageThreadPoolCount { get; set; } = 32;
        public int HeartbeatThreadPoolCount { get; set; } = Math.Min(Environment.ProcessorCount, 32);
        public int EndTransactionThreadPoolCount =>  Math.Max(8 + Environment.ProcessorCount * 2,
            SendMessageThreadPoolCount * 4);
        /// <summary>
        /// 毫秒数
        /// </summary>
        public int FlushConsumerOffsetInterval = 1000 * 5;
        /// <summary>
        /// 毫秒数
        /// </summary>
        public int FlushConsumerOffsetHistoryInterval = 1000 * 60;

        public bool RejectTransactionMessage = false;

        public bool EnableDLegerCommitLog { get; set; } = false;

        public int TransactionCheckInterval { get; set; }= 60 * 1000;
        public string? NameServerAddress { get; set; }
        public bool IsFetchNamesrvAddrByAddressServer { get; set; } = false;
    }
}
