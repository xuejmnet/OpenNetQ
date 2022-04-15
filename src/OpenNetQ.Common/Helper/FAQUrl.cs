using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/15 11:02:19
    /// Email: 326308290@qq.com
    public class FAQUrl
    {
        public static readonly String APPLY_TOPIC_URL =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String NAME_SERVER_ADDR_NOT_EXIST_URL =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String GROUP_NAME_DUPLICATE_URL =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String CLIENT_PARAMETER_CHECK_URL =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String SUBSCRIPTION_GROUP_NOT_EXIST =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String CLIENT_SERVICE_NOT_OK =
        "http://rocketmq.apache.org/docs/faq/";

        // FAQ: No route info of this topic, TopicABC
        public static readonly String NO_TOPIC_ROUTE_INFO =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String LOAD_JSON_EXCEPTION =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String SAME_GROUP_DIFFERENT_TOPIC =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String MQLIST_NOT_EXIST =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String UNEXPECTED_EXCEPTION_URL =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String SEND_MSG_FAILED =
        "http://rocketmq.apache.org/docs/faq/";

        public static readonly String UNKNOWN_HOST_EXCEPTION =
        "http://rocketmq.apache.org/docs/faq/";

        private static readonly string TIP_STRING_BEGIN = "\nSee ";
        private static readonly string TIP_STRING_END = " for further details.";
        private FAQUrl(){}

        public static string? AttachDefaultURL(string? errorMessage)
        {
            if (errorMessage is not null)
            {
                var index = errorMessage.IndexOf(TIP_STRING_BEGIN, StringComparison.Ordinal);
                if (index == -1)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(errorMessage);
                    sb.Append("For more information, please visit the url, ");
                    sb.Append(UNEXPECTED_EXCEPTION_URL);
                    return sb.ToString();
                }
            }

            return errorMessage;
        }
    }
}
