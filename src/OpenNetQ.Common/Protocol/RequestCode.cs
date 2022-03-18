using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Common.Protocol
{
    public class RequestCode
    {
        public const int SEND_MESSAGE = 10;

        public const int PULL_MESSAGE = 11;

        public const int QUERY_MESSAGE = 12;
        public const int QUERY_BROKER_OFFSET = 13;
        public const int QUERY_CONSUMER_OFFSET = 14;
        public const int UPDATE_CONSUMER_OFFSET = 15;
        public const int UPDATE_AND_CREATE_TOPIC = 17;
        public const int GET_ALL_TOPIC_CONFIG = 21;
        public const int GET_TOPIC_CONFIG_LIST = 22;

        public const int GET_TOPIC_NAME_LIST = 23;

        public const int UPDATE_BROKER_CONFIG = 25;

        public const int GET_BROKER_CONFIG = 26;

        public const int TRIGGER_DELETE_FILES = 27;

        public const int GET_BROKER_RUNTIME_INFO = 28;
        public const int SEARCH_OFFSET_BY_TIMESTAMP = 29;
        public const int GET_MAX_OFFSET = 30;
        public const int GET_MIN_OFFSET = 31;

        public const int GET_EARLIEST_MSG_STORETIME = 32;

        public const int VIEW_MESSAGE_BY_ID = 33;

        public const int HEART_BEAT = 34;

        public const int UNREGISTER_CLIENT = 35;

        public const int CONSUMER_SEND_MSG_BACK = 36;

        public const int END_TRANSACTION = 37;
        public const int GET_CONSUMER_LIST_BY_GROUP = 38;

        public const int CHECK_TRANSACTION_STATE = 39;

        public const int NOTIFY_CONSUMER_IDS_CHANGED = 40;

        public const int LOCK_BATCH_MQ = 41;

        public const int UNLOCK_BATCH_MQ = 42;
        public const int GET_ALL_CONSUMER_OFFSET = 43;

        public const int GET_ALL_DELAY_OFFSET = 45;

        public const int CHECK_CLIENT_CONFIG = 46;

        public const int UPDATE_AND_CREATE_ACL_CONFIG = 50;

        public const int DELETE_ACL_CONFIG = 51;

        public const int GET_BROKER_CLUSTER_ACL_INFO = 52;

        public const int UPDATE_GLOBAL_WHITE_ADDRS_CONFIG = 53;

        public const int GET_BROKER_CLUSTER_ACL_CONFIG = 54;

        public const int PUT_KV_CONFIG = 100;

        public const int GET_KV_CONFIG = 101;

        public const int DELETE_KV_CONFIG = 102;

        public const int REGISTER_BROKER = 103;

        public const int UNREGISTER_BROKER = 104;
        public const int GET_ROUTEINFO_BY_TOPIC = 105;

        public const int GET_BROKER_CLUSTER_INFO = 106;
        public const int UPDATE_AND_CREATE_SUBSCRIPTIONGROUP = 200;
        public const int GET_ALL_SUBSCRIPTIONGROUP_CONFIG = 201;
        public const int GET_TOPIC_STATS_INFO = 202;
        public const int GET_CONSUMER_CONNECTION_LIST = 203;
        public const int GET_PRODUCER_CONNECTION_LIST = 204;
        public const int WIPE_WRITE_PERM_OF_BROKER = 205;

        public const int GET_ALL_TOPIC_LIST_FROM_NAMESERVER = 206;

        public const int DELETE_SUBSCRIPTIONGROUP = 207;
        public const int GET_CONSUME_STATS = 208;

        public const int SUSPEND_CONSUMER = 209;

        public const int RESUME_CONSUMER = 210;
        public const int RESET_CONSUMER_OFFSET_IN_CONSUMER = 211;
        public const int RESET_CONSUMER_OFFSET_IN_BROKER = 212;

        public const int ADJUST_CONSUMER_THREAD_POOL = 213;

        public const int WHO_CONSUME_THE_MESSAGE = 214;

        public const int DELETE_TOPIC_IN_BROKER = 215;

        public const int DELETE_TOPIC_IN_NAMESRV = 216;
        public const int GET_KVLIST_BY_NAMESPACE = 219;

        public const int RESET_CONSUMER_CLIENT_OFFSET = 220;

        public const int GET_CONSUMER_STATUS_FROM_CLIENT = 221;

        public const int INVOKE_BROKER_TO_RESET_OFFSET = 222;

        public const int INVOKE_BROKER_TO_GET_CONSUMER_STATUS = 223;

        public const int QUERY_TOPIC_CONSUME_BY_WHO = 300;

        public const int GET_TOPICS_BY_CLUSTER = 224;

        public const int REGISTER_FILTER_SERVER = 301;
        public const int REGISTER_MESSAGE_FILTER_CLASS = 302;

        public const int QUERY_CONSUME_TIME_SPAN = 303;

        public const int GET_SYSTEM_TOPIC_LIST_FROM_NS = 304;
        public const int GET_SYSTEM_TOPIC_LIST_FROM_BROKER = 305;

        public const int CLEAN_EXPIRED_CONSUMEQUEUE = 306;

        public const int GET_CONSUMER_RUNNING_INFO = 307;

        public const int QUERY_CORRECTION_OFFSET = 308;
        public const int CONSUME_MESSAGE_DIRECTLY = 309;

        public const int SEND_MESSAGE_V2 = 310;

        public const int GET_UNIT_TOPIC_LIST = 311;

        public const int GET_HAS_UNIT_SUB_TOPIC_LIST = 312;

        public const int GET_HAS_UNIT_SUB_UNUNIT_TOPIC_LIST = 313;

        public const int CLONE_GROUP_OFFSET = 314;

        public const int VIEW_BROKER_STATS_DATA = 315;

        public const int CLEAN_UNUSED_TOPIC = 316;

        public const int GET_BROKER_CONSUME_STATS = 317;

        /**
         * update the config of name server
         */
        public const int UPDATE_NAMESRV_CONFIG = 318;

        /**
         * get config from name server
         */
        public const int GET_NAMESRV_CONFIG = 319;

        public const int SEND_BATCH_MESSAGE = 320;

        public const int QUERY_CONSUME_QUEUE = 321;

        public const int QUERY_DATA_VERSION = 322;

        /**
         * resume logic of checking half messages that have been put in TRANS_CHECK_MAXTIME_TOPIC before
         */
        public const int RESUME_CHECK_HALF_MESSAGE = 323;

        public const int SEND_REPLY_MESSAGE = 324;

        public const int SEND_REPLY_MESSAGE_V2 = 325;

        public const int PUSH_REPLY_MESSAGE_TO_CLIENT = 326;

        public const int ADD_WRITE_PERM_OF_BROKER = 327;
    }
}
