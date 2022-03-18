using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Common.Protocol
{
    public class ResponseCode:RemotingSysResponseCode
    {
        public const int FLUSH_DISK_TIMEOUT = 10;

        public const int SLAVE_NOT_AVAILABLE = 11;

        public const int FLUSH_SLAVE_TIMEOUT = 12;

        public const int MESSAGE_ILLEGAL = 13;

        public const int SERVICE_NOT_AVAILABLE = 14;

        public const int VERSION_NOT_SUPPORTED = 15;

        public const int NO_PERMISSION = 16;

        public const int TOPIC_NOT_EXIST = 17;
        public const int TOPIC_EXIST_ALREADY = 18;
        public const int PULL_NOT_FOUND = 19;

        public const int PULL_RETRY_IMMEDIATELY = 20;

        public const int PULL_OFFSET_MOVED = 21;

        public const int QUERY_NOT_FOUND = 22;

        public const int SUBSCRIPTION_PARSE_FAILED = 23;

        public const int SUBSCRIPTION_NOT_EXIST = 24;

        public const int SUBSCRIPTION_NOT_LATEST = 25;

        public const int SUBSCRIPTION_GROUP_NOT_EXIST = 26;

        public const int FILTER_DATA_NOT_EXIST = 27;

        public const int FILTER_DATA_NOT_LATEST = 28;

        public const int TRANSACTION_SHOULD_COMMIT = 200;

        public const int TRANSACTION_SHOULD_ROLLBACK = 201;

        public const int TRANSACTION_STATE_UNKNOW = 202;

        public const int TRANSACTION_STATE_GROUP_WRONG = 203;
        public const int NO_BUYER_ID = 204;

        public const int NOT_IN_CURRENT_UNIT = 205;

        public const int CONSUMER_NOT_ONLINE = 206;

        public const int CONSUME_MSG_TIMEOUT = 207;

        public const int NO_MESSAGE = 208;

        public const int UPDATE_AND_CREATE_ACL_CONFIG_FAILED = 209;

        public const int DELETE_ACL_CONFIG_FAILED = 210;

        public const int UPDATE_GLOBAL_WHITE_ADDRS_CONFIG_FAILED = 211;
    }
}
