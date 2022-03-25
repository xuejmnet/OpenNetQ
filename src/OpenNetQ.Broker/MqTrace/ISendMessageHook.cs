using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Broker.MqTrace
{
    /// <summary>
    /// 发送消息钩子
    /// </summary>
    /// Author: xjm
    /// Created: 2022/3/25 10:31:25
    /// Email: 326308290@qq.com
    public interface ISendMessageHook
    {
        string HookName { get; }
        void SendMessageBefore(SendMessageContext context);

        void SendMessageAfter(SendMessageContext context);
    }
}
