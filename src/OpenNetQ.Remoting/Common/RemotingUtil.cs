using System;
using System.Runtime.InteropServices;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;

namespace OpenNetQ.Remoting.Common;

/*
* @Author: xjm
* @Description:
* @Date: Thursday, 13 January 2022 22:46:47
* @Email: 326308290@qq.com
*/
public class RemotingUtil
{
    private static readonly bool isLinuxPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    private static readonly bool isWindowsPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);


    public static void CloseChannel(IChannel? channel,ILogger logger)
    {
        if (channel != null)
        {
            string addrRemote = RemotingHelper.ParseChannelRemoteAddr(channel);
            channel.CloseAsync()
                .ContinueWith((t, c) => logger.LogInformation($"closeChannel: close the connection to remote address[{addrRemote}] result: {t.IsCompleted}"),
                    null, TaskContinuationOptions.ExecuteSynchronously);
        }
    }

    public static string GetLocalAddress()
    {
        throw new NotImplementedException(nameof(GetLocalAddress));
    }
}