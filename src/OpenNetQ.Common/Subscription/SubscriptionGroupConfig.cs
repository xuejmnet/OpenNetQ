using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.Subscription
{
    public class SubscriptionGroupConfig
    {
        public string GroupName { get; set; }
        public bool ConsumeEnable { get; set; } = true;
        public bool ConsumeFromMinEnable { get; set;} =true;
        public int RetryQueueNums { get; set; } = 1;
        public int RetryMaxTimes { get; set; } = 16;
        public long BrokerId { get; set; } = MixAll.MASTER_ID;
        public long WhichBrokerWhenConsumeSlowly { get; set; } = 1;
        public bool NotifyConsumerIdsChangeEnable { get; set; } = true;

        protected bool Equals(SubscriptionGroupConfig other)
        {
            return GroupName == other.GroupName && ConsumeEnable == other.ConsumeEnable && ConsumeFromMinEnable == other.ConsumeFromMinEnable && RetryQueueNums == other.RetryQueueNums && RetryMaxTimes == other.RetryMaxTimes && BrokerId == other.BrokerId && WhichBrokerWhenConsumeSlowly == other.WhichBrokerWhenConsumeSlowly && NotifyConsumerIdsChangeEnable == other.NotifyConsumerIdsChangeEnable;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubscriptionGroupConfig)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GroupName, ConsumeEnable, ConsumeFromMinEnable, RetryQueueNums, RetryMaxTimes, BrokerId, WhichBrokerWhenConsumeSlowly, NotifyConsumerIdsChangeEnable);
        }

        public override string ToString()
        {
            return $"{nameof(GroupName)}: {GroupName}, {nameof(ConsumeEnable)}: {ConsumeEnable}, {nameof(ConsumeFromMinEnable)}: {ConsumeFromMinEnable}, {nameof(RetryQueueNums)}: {RetryQueueNums}, {nameof(RetryMaxTimes)}: {RetryMaxTimes}, {nameof(BrokerId)}: {BrokerId}, {nameof(WhichBrokerWhenConsumeSlowly)}: {WhichBrokerWhenConsumeSlowly}, {nameof(NotifyConsumerIdsChangeEnable)}: {NotifyConsumerIdsChangeEnable}";
        }
    }
}