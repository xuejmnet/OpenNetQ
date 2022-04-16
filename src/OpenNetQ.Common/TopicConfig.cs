using System;
using System.Text;
using OpenNetQ.Common.Constant;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common
{
    public class TopicConfig
    {
        private const string SEPARATOR = " ";
        public const int DefaultReadQueueNums = 16;
        public const int DefaultWriteQueueNums = 16;
        public string TopicName { get; set; }
        public int ReadQueueNums { get; set; } = DefaultReadQueueNums;
        public int WriteQueueNums { get; set; } = DefaultWriteQueueNums;
        public PermissionEnum Perm { get; set; } =PermissionEnum.READ|PermissionEnum.WRITE;
        public TopicFilterTypeEnum TopicFilterType { get; set; } = TopicFilterTypeEnum.SINGLE_TAG;
        public int TopicSysFlag { get; set; } = 0;
        public bool Order { get; set; } = false;

        public TopicConfig()
        {

        }
        public TopicConfig(string topicName)
        {
            this.TopicName = topicName;
        }

        public TopicConfig(string topicName,int readQueueNums,int writeQueueNums,PermissionEnum perm)
        {
            TopicName = topicName;
            ReadQueueNums = readQueueNums;
            WriteQueueNums = writeQueueNums;
            Perm = perm;
        }

        public string Encode()
        {
            var sb = new StringBuilder();
            sb.Append(TopicName);
            sb.Append(SEPARATOR);
            sb.Append(ReadQueueNums);
            sb.Append(SEPARATOR);
            sb.Append(WriteQueueNums);
            sb.Append(SEPARATOR);
            sb.Append(Perm);
            sb.Append(SEPARATOR);
            sb.Append(TopicFilterType);
            return sb.ToString();
        }

        public bool Decode(string value)
        {
            var strs = value.Split(SEPARATOR);
            if (strs.Length!= 5)
            {
                TopicName = strs[0];
                ReadQueueNums = int.Parse(strs[1]);
                WriteQueueNums = int.Parse(strs[2]);
                Perm = (PermissionEnum)int.Parse(strs[3]);
                TopicFilterType = (TopicFilterTypeEnum)int.Parse(strs[4]);

                return true;
            }

            return false;
        }

        protected bool Equals(TopicConfig other)
        {
            return TopicName == other.TopicName && ReadQueueNums == other.ReadQueueNums && WriteQueueNums == other.WriteQueueNums && Perm == other.Perm && TopicFilterType == other.TopicFilterType && TopicSysFlag == other.TopicSysFlag && Order == other.Order;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TopicConfig)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TopicName, ReadQueueNums, WriteQueueNums, Perm, (int)TopicFilterType, TopicSysFlag, Order);
        }

        public override string ToString()
        {
            return $"{nameof(TopicName)}: {TopicName}, {nameof(ReadQueueNums)}: {ReadQueueNums}, {nameof(WriteQueueNums)}: {WriteQueueNums}, {nameof(Perm)}: {Perm}, {nameof(TopicFilterType)}: {TopicFilterType}, {nameof(TopicSysFlag)}: {TopicSysFlag}, {nameof(Order)}: {Order}";
        }
    }
}