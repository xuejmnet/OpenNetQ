using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Protocol;

namespace KhaosLog.NettyProvider.Handlers
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 13 November 2020 17:20:50
* @Email: 326308290@qq.com
*/
    public class MessagePackEncoder : MessageToByteEncoder<RemotingCommand>
    {
        private readonly ILogger<MessagePackEncoder> _logger;
        public override bool IsSharable => true;

        public MessagePackEncoder(LoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MessagePackEncoder>();
        }
        protected override void Encode(IChannelHandlerContext context, RemotingCommand cmd, IByteBuffer output)
        {
            try
            {
                //序列化类
                var messageBytes  = cmd.EncodeHeader().Array;
                output.WriteBytes(messageBytes);
                var body = cmd.Body;
                if (body != null)
                {
                    output.WriteBytes(body);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"message encode error.");
                RemotingUtil.CloseChannel(context.Channel, _logger);
            }
        }
    }
}