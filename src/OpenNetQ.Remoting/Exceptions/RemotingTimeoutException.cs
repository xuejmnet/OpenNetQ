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
    public RemotingTimeoutException()
    {
    }

    protected RemotingTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public RemotingTimeoutException(string? message) : base(message)
    {
    }

    public RemotingTimeoutException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}