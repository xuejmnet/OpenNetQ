using DotNetty.Transport.Channels;

namespace OpenNetQ.Remoting.Abstractions.Connections
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 17 November 2020 11:26:48
* @Email: 326308290@qq.com
*/
    public interface IConnectionChannelPool
    {
        bool TryGet( out IChannel channel,out int index);
        void CloseChannel(int index,  IChannel channel);
    }
}