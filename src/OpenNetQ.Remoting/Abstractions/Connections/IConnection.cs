using DotNetty.Transport.Channels;

namespace OpenNetQ.Remoting.Abstractions.Connections
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 17 November 2020 10:40:58
* @Email: 326308290@qq.com
*/
    public interface IConnection:IDisposable
    {
        bool IsOpen { get; }
        IChannel CreateChannel();
    }
}