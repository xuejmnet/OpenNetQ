using System;
using System.Runtime.Serialization;

namespace OpenNetQ.Remoting.Exceptions;

/*
* @Author: xjm
* @Description:
* @Date: Friday, 14 January 2022 22:18:25
* @Email: 326308290@qq.com
*/
public class RemotingTimeoutException:RemotingException
{
    public RemotingTimeoutException(string addr, long timeoutMillis, Exception exception) : base($"wait response on the channel <{addr}> timeout, {timeoutMillis}(ms)", exception)
    {
    }
    public RemotingTimeoutException(string addr, long timeoutMillis) : base($"wait response on the channel <{addr}> timeout, {timeoutMillis}(ms)", null)
    {
    }
    public RemotingTimeoutException(string message) : base(message)
    {
    }
}