using OpenNetQ.Common.Messages;

namespace OpenNetQ.Client.Producers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 11:05:46
    /// Email: 326308290@qq.com
    public class SendResult
    {
        public SendStatusEnum? SendStatus { get; set; }
        public string? MsgId { get; set; }
        public MessageQueue? MessageQueue { get; set; }
        public long QueueOffset { get; set; }
        public string? TransactionId { get; set; }
        public string? OffsetMsgId { get; set; }
        public string? RegionId { get; set; }
        public bool TraceOn { get; set; } = true;

        public SendResult()
        {
            
        }

        public SendResult(SendStatusEnum sendStatus,string? msgId,string? offsetMsgId,MessageQueue? messageQueue,long queueOffset)
        :this(sendStatus,msgId,messageQueue,queueOffset,null,offsetMsgId,null)
        {
        }
        public SendResult(SendStatusEnum sendStatus, string? msgId,  MessageQueue? messageQueue, long queueOffset,string? transactionId,string? offsetMsgId,string? regionId)
        {
            SendStatus = sendStatus;
            MsgId = msgId;
            MessageQueue = messageQueue;
            QueueOffset = queueOffset;
            TransactionId=transactionId;
            OffsetMsgId = offsetMsgId;
            RegionId = regionId;
        }

        public override string ToString()
        {
            return $"{nameof(SendStatus)}: {SendStatus}, {nameof(MsgId)}: {MsgId}, {nameof(MessageQueue)}: {MessageQueue}, {nameof(QueueOffset)}: {QueueOffset}, {nameof(TransactionId)}: {TransactionId}, {nameof(OffsetMsgId)}: {OffsetMsgId}, {nameof(RegionId)}: {RegionId}, {nameof(TraceOn)}: {TraceOn}";
        }
    }
}
