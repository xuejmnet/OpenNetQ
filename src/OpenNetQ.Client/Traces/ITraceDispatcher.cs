using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Client.Exceptions;

namespace OpenNetQ.Client.Traces
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 13:21:27
    /// Email: 326308290@qq.com
    public interface ITraceDispatcher
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameSrvAddr"></param>
        /// <param name="accessChannel"></param>
        /// <returns></returns>
        /// <exception cref="MQClientException"></exception>
        Task StartAsync(string nameSrvAddr, AccessChannelEnum accessChannel);

        bool Append(object ctx);
        Task FlushAsync();
        Task StopAsync();
    }
}
