using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Remoting.Netty
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 13 November 2020 17:11:03
* @Email: 326308290@qq.com
*/
    public class MessagePackDecoder: ByteToMessageDecoder
    {
        private readonly ILogger<MessagePackDecoder> _logger = OpenNetQLoggerFactory.CreateLogger<MessagePackDecoder>();

        public MessagePackDecoder()
        {
        }
        
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                byte[] array = new byte[input.ReadableBytes];
                input.GetBytes(input.ReaderIndex, array, 0, input.ReadableBytes);
                input.Clear();
                var temp = RemotingCommand.Decode(array);
                output.Add(temp);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"message decoder error");
                RemotingUtil.CloseChannel(context.Channel, _logger);
            }
        }
    }
}