using System;
using OpenNetQ.Common.Constant;
using OpenNetQ.Remoting;
using OpenNetQ.Remoting.Exceptions;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.Protocol.Header.NameSrerver
{
    public class CreateTopicRequestHeader:ICommandCustomHeader
    {
        public string? Topic { get; set; }
        public string? DefaultTopic { get; set; }
        public int? ReadQueueNums { get; set; }
        public int? WriteQueueNums { get; set; }
        public PermissionEnum? Perm { get; set; }
        public string? TopicFilterType { get; set; }
        public int? TopicSysFlag { get; set; }
        public bool? Order { get; set; }=false;
        public void CheckFields()
        {
            try
            {
                if (TopicFilterType == null)
                    throw new RemotingCommandException($"topicFilterType = [{TopicFilterType}] value invalid");
                Enum.Parse<TopicFilterTypeEnum>(TopicFilterType);
            }
            catch (Exception e)
            {
                throw new RemotingCommandException($"topicFilterType = [{TopicFilterType}] value invalid", e);
            }
        }
    }
}