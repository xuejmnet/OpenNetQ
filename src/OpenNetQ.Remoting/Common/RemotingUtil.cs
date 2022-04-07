using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using OpenNetQ.Extensions;

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


    public static void CloseChannel(IChannel? channel, ILogger logger)
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
        var ipv4Result = new List<string>();
        var ipv6Result = new List<string>();

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        ipv6Result.Add(NormalizeHostAddress(ip.Address));
                    }
                    else if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipv4Result.Add(NormalizeHostAddress(ip.Address));
                    }
                }
            }
        }

        if (ipv4Result.IsNotEmpty())
        {
            var ip = ipv4Result.FirstOrDefault(o => !o.StartsWith("127.0") && !o.StartsWith("192.168"))??ipv4Result.First();
            return ip;
        } else if (!ipv6Result.IsNotEmpty())
        {
            return ipv6Result.First();
        }

        throw new ArgumentException("Failed to obtain local address");

    }

    public static string NormalizeHostAddress(IPAddress ipAddress)
    {
        if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return $"[{ipAddress.ToString()}]";
        }
        else
        {
            return ipAddress.ToString();
        }
    }
}