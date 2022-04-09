using System;
using OpenNetQ.Broker.MqTrace;
using OpenNetQ.Remoting.Netty.Abstractions;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Broker.Processor
{
    public interface IReplyMessageProcessor:IAsyncNettyRequestProcessor
    {
        void RegisterSendMessageHook(ICollection<ISendMessageHook> sendMessageHooks);
    }
}