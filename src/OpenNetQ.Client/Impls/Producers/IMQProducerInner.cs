using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Common.Messages;

namespace OpenNetQ.Client.Impls.Producers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 12:23:08
    /// Email: 326308290@qq.com
    public interface IMQProducerInner
    {
        ISet<string> GetPublishTopicList();
        object GetCheckListener();

        void CheckTransactionState(string addr,MessageExt msg,object checkRequestHeader);
        void UpdateTopicPublishInfo(string topic, TopicPublishInfo info);
        bool IsUnitMode();
    }
}
