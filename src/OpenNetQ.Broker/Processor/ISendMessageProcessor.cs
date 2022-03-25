using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Broker.MqTrace;
using OpenNetQ.Remoting.Netty.Abstractions;

namespace OpenNetQ.Broker.Processor
{
    /// <summary>
    /// 发送消息处理器 单例注册
    /// </summary>
    /// Author: xjm
    /// Created: 2022/3/25 10:28:30
    /// Email: 326308290@qq.com
    public interface ISendMessageProcessor:IAsyncNettyRequestProcessor
    {
        void RegisterSendMessageHook(ICollection<ISendMessageHook> sendMessageHooks);
        void RegisterConsumeMessageHook(ICollection<IConsumeMessageHook> consumeMessageHooks);
    }
}
