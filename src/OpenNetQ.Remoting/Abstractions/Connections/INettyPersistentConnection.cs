using DotNetty.Transport.Channels;

namespace OpenNetQ.Remoting.Abstractions.Connections
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 17 November 2020 11:09:43
* @Email: 326308290@qq.com
*/
    public interface INettyPersistentConnection:IDisposable
    {
        
        bool IsConnectCreated { get; }

        bool TryConnect();

        IChannel CreateChannel();
    }
}