using System;

/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 08 March 2022 22:11:12
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Remoting.Protocol
{
    public enum RemotingCommandType
    {
        /// <summary>
        /// request command
        /// 请求命令
        /// </summary>
        REQUEST_COMMAND,
        /// <summary>
        /// response command
        /// 响应命令
        /// </summary>
        RESPONSE_COMMAND
    }
}