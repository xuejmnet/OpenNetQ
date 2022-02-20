using System;
using System.Runtime.InteropServices;

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
}