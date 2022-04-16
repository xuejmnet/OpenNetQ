using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Common.Protocol;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Common.Topic
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 13:15:53
    /// Email: 326308290@qq.com
    public class TopicValidator
    {
        public static readonly string AUTO_CREATE_TOPIC_KEY_TOPIC = "TBW102"; // Will be created at broker when isAutoCreateTopicEnable
        public static readonly string RMQ_SYS_SCHEDULE_TOPIC = "SCHEDULE_TOPIC_XXXX";
        public static readonly string RMQ_SYS_BENCHMARK_TOPIC = "BenchmarkTest";
        public static readonly string RMQ_SYS_TRANS_HALF_TOPIC = "RMQ_SYS_TRANS_HALF_TOPIC";
        public static readonly string RMQ_SYS_TRACE_TOPIC = "RMQ_SYS_TRACE_TOPIC";
        public static readonly string RMQ_SYS_TRANS_OP_HALF_TOPIC = "RMQ_SYS_TRANS_OP_HALF_TOPIC";
        public static readonly string RMQ_SYS_TRANS_CHECK_MAX_TIME_TOPIC = "TRANS_CHECK_MAX_TIME_TOPIC";
        public static readonly string RMQ_SYS_SELF_TEST_TOPIC = "SELF_TEST_TOPIC";
        public static readonly string RMQ_SYS_OFFSET_MOVED_EVENT = "OFFSET_MOVED_EVENT";

        public static readonly string SYSTEM_TOPIC_PREFIX = "rmq_sys_";
        public static readonly bool[] VALID_CHAR_BIT_MAP = new bool[128];
        private static readonly int TOPIC_MAX_LENGTH = 127;

        private static readonly ISet<string> SYSTEM_TOPIC_SET = new HashSet<string>();

        /**
         * Topics'set which client can not send msg!
         */
        private static readonly ISet<string> NOT_ALLOWED_SEND_TOPIC_SET = new HashSet<string>();

        static TopicValidator()
        {
            SYSTEM_TOPIC_SET.Add(AUTO_CREATE_TOPIC_KEY_TOPIC);
            SYSTEM_TOPIC_SET.Add(RMQ_SYS_SCHEDULE_TOPIC);
            SYSTEM_TOPIC_SET.Add(RMQ_SYS_BENCHMARK_TOPIC);
            SYSTEM_TOPIC_SET.Add(RMQ_SYS_TRANS_HALF_TOPIC);
            SYSTEM_TOPIC_SET.Add(RMQ_SYS_TRACE_TOPIC);
            SYSTEM_TOPIC_SET.Add(RMQ_SYS_TRANS_OP_HALF_TOPIC);
            SYSTEM_TOPIC_SET.Add(RMQ_SYS_TRANS_CHECK_MAX_TIME_TOPIC);
            SYSTEM_TOPIC_SET.Add(RMQ_SYS_SELF_TEST_TOPIC);
            SYSTEM_TOPIC_SET.Add(RMQ_SYS_OFFSET_MOVED_EVENT);

            NOT_ALLOWED_SEND_TOPIC_SET.Add(RMQ_SYS_SCHEDULE_TOPIC);
            NOT_ALLOWED_SEND_TOPIC_SET.Add(RMQ_SYS_TRANS_HALF_TOPIC);
            NOT_ALLOWED_SEND_TOPIC_SET.Add(RMQ_SYS_TRANS_OP_HALF_TOPIC);
            NOT_ALLOWED_SEND_TOPIC_SET.Add(RMQ_SYS_TRANS_CHECK_MAX_TIME_TOPIC);
            NOT_ALLOWED_SEND_TOPIC_SET.Add(RMQ_SYS_SELF_TEST_TOPIC);
            NOT_ALLOWED_SEND_TOPIC_SET.Add(RMQ_SYS_OFFSET_MOVED_EVENT);

            // regex: ^[%|a-zA-Z0-9_-]+$
            // %
            VALID_CHAR_BIT_MAP['%'] = true;
            // -
            VALID_CHAR_BIT_MAP['-'] = true;
            // _
            VALID_CHAR_BIT_MAP['_'] = true;
            // |
            VALID_CHAR_BIT_MAP['|'] = true;
            for (int i = 0; i < VALID_CHAR_BIT_MAP.Length; i++)
            {
                if (i >= '0' && i <= '9')
                {
                    // 0-9
                    VALID_CHAR_BIT_MAP[i] = true;
                }
                else if (i >= 'A' && i <= 'Z')
                {
                    // A-Z
                    VALID_CHAR_BIT_MAP[i] = true;
                }
                else if (i >= 'a' && i <= 'z')
                {
                    // a-z
                    VALID_CHAR_BIT_MAP[i] = true;
                }
            }
        }

        public static bool IsTopicOrGroupIllegal(string str)
        {
            int strLen = str.Length;
            int len = VALID_CHAR_BIT_MAP.Length;
            bool[] bitMap = VALID_CHAR_BIT_MAP;
            for (int i = 0; i < strLen; i++)
            {
                char ch = str[i];
                if (ch >= len || !bitMap[ch])
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ValidateTopic(string topic, RemotingCommand response)
        {

            if (UtilAll.isBlank())
            {
                response.setCode(ResponseCode.SYSTEM_ERROR);
                response.setRemark("The specified topic is blank.");
                return false;
            }

            if (isTopicOrGroupIllegal(topic))
            {
                response.setCode(ResponseCode.SYSTEM_ERROR);
                response.setRemark("The specified topic contains illegal characters, allowing only ^[%|a-zA-Z0-9_-]+$");
                return false;
            }

            if (topic.length() > TOPIC_MAX_LENGTH)
            {
                response.setCode(ResponseCode.SYSTEM_ERROR);
                response.setRemark("The specified topic is longer than topic max length.");
                return false;
            }

            return true;
        }

        public static boolean isSystemTopic(string topic, RemotingCommand response)
        {
            if (isSystemTopic(topic))
            {
                response.setCode(ResponseCode.SYSTEM_ERROR);
                response.setRemark("The topic[" + topic + "] is conflict with system topic.");
                return true;
            }
            return false;
        }

        public static boolean isSystemTopic(string topic)
        {
            return SYSTEM_TOPIC_SET.contains(topic) || topic.startsWith(SYSTEM_TOPIC_PREFIX);
        }

        public static boolean isNotAllowedSendTopic(string topic)
        {
            return NOT_ALLOWED_SEND_TOPIC_SET.contains(topic);
        }

        public static boolean isNotAllowedSendTopic(string topic, RemotingCommand response)
        {
            if (isNotAllowedSendTopic(topic))
            {
                response.setCode(ResponseCode.NO_PERMISSION);
                response.setRemark("Sending message to topic[" + topic + "] is forbidden.");
                return true;
            }
            return false;
        }

        public static void addSystemTopic(string systemTopic)
        {
            SYSTEM_TOPIC_SET.add(systemTopic);
        }

        public static Set<string> getSystemTopicSet()
        {
            return SYSTEM_TOPIC_SET;
        }

        public static Set<string> getNotAllowedSendTopicSet()
        {
            return NOT_ALLOWED_SEND_TOPIC_SET;
        }
    }
}
