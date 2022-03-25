using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Broker.MqTrace
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/3/25 10:59:08
    /// Email: 326308290@qq.com
    public interface IConsumeMessageHook
    {
        string HookName { get; }


        void ConsumeMessageBefore(ConsumeMessageContext context);

        void ConsumeMessageAfter(ConsumeMessageContext context);
    }
}
