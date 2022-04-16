using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 13:41:12
    /// Email: 326308290@qq.com
    public class MixAll
    {
        public static readonly string ROCKETMQ_HOME_ENV = "ROCKETMQ_HOME";
        public static readonly string ROCKETMQ_HOME_PROPERTY = "rocketmq.home.dir";
        public static readonly string NAMESRV_ADDR_ENV = "NAMESRV_ADDR";
        public static readonly string NAMESRV_ADDR_PROPERTY = "rocketmq.namesrv.addr";
        public static readonly string MESSAGE_COMPRESS_LEVEL = "rocketmq.message.compressLevel";
        public static readonly string DEFAULT_NAMESRV_ADDR_LOOKUP = "jmenv.tbsite.net";
        public static readonly string WS_DOMAIN_NAME = DEFAULT_NAMESRV_ADDR_LOOKUP;
        public static readonly string WS_DOMAIN_SUBGROUP = "nsaddr";
        //http://jmenv.tbsite.net:8080/rocketmq/nsaddr
        //public static readonly string WS_ADDR = "http://" + WS_DOMAIN_NAME + ":8080/rocketmq/" + WS_DOMAIN_SUBGROUP;
        public static readonly string DEFAULT_PRODUCER_GROUP = "DEFAULT_PRODUCER";
        public static readonly string DEFAULT_CONSUMER_GROUP = "DEFAULT_CONSUMER";
        public static readonly string TOOLS_CONSUMER_GROUP = "TOOLS_CONSUMER";
        public static readonly string SCHEDULE_CONSUMER_GROUP = "SCHEDULE_CONSUMER";
        public static readonly string FILTERSRV_CONSUMER_GROUP = "FILTERSRV_CONSUMER";
        public static readonly string MONITOR_CONSUMER_GROUP = "__MONITOR_CONSUMER";
        public static readonly string CLIENT_INNER_PRODUCER_GROUP = "CLIENT_INNER_PRODUCER";
        public static readonly string SELF_TEST_PRODUCER_GROUP = "SELF_TEST_P_GROUP";
        public static readonly string SELF_TEST_CONSUMER_GROUP = "SELF_TEST_C_GROUP";
        public static readonly string ONS_HTTP_PROXY_GROUP = "CID_ONS-HTTP-PROXY";
        public static readonly string CID_ONSAPI_PERMISSION_GROUP = "CID_ONSAPI_PERMISSION";
        public static readonly string CID_ONSAPI_OWNER_GROUP = "CID_ONSAPI_OWNER";
        public static readonly string CID_ONSAPI_PULL_GROUP = "CID_ONSAPI_PULL";
        public static readonly string CID_RMQ_SYS_PREFIX = "CID_RMQ_SYS_";
        public static readonly List<string> LOCAL_INET_ADDRESS = GetLocalInetAddress();
        public static readonly string LOCALHOST = Localhost();
        public static readonly string DEFAULT_CHARSET = "UTF-8";
        public static readonly long MASTER_ID = 0L;
        public static readonly long CURRENT_JVM_PID =Process.GetCurrentProcess().Id;
        public static readonly string RETRY_GROUP_TOPIC_PREFIX = "%RETRY%";
        public static readonly string DLQ_GROUP_TOPIC_PREFIX = "%DLQ%";
        public static readonly string REPLY_TOPIC_POSTFIX = "REPLY_TOPIC";
        public static readonly string UNIQUE_MSG_QUERY_FLAG = "_UNIQUE_KEY_QUERY";
        public static readonly string DEFAULT_TRACE_REGION_ID = "DefaultRegion";
        public static readonly string CONSUME_CONTEXT_TYPE = "ConsumeContextType";
        public static readonly string CID_SYS_RMQ_TRANS = "CID_RMQ_SYS_TRANS";
        public static readonly string ACL_CONF_TOOLS_FILE = "/conf/tools.yml";
        public static readonly string REPLY_MESSAGE_FLAG = "reply";
        public static readonly string LMQ_PREFIX = "%LMQ%";
        public static readonly string MULTI_DISPATCH_QUEUE_SPLITTER = ",";
        private MixAll()
        {

        }
        private static string? Localhost()
        {
            return null;
        }
        public static List<String> GetLocalInetAddress()
        {
            return null;
        }

    }
}
