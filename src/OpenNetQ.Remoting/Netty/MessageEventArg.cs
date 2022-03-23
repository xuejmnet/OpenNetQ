using System;
using DotNetty.Transport.Channels;
using OpenNetQ.Remoting.Protocol;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Remoting.Netty
{
    public class MessageEventArg
    {
        public IChannelHandlerContext ChannelHandlerContext { get; }
        public RemotingCommand? RemotingCommand { get; }

        public MessageEventArg(IChannelHandlerContext channelHandlerContext,RemotingCommand? remotingCommand)
        {
            ChannelHandlerContext = channelHandlerContext;
            RemotingCommand = remotingCommand;
        }
    }
}