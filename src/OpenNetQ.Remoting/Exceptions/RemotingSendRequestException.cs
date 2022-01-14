using System;
using System.Runtime.Serialization;

namespace OpenNetQ.Remoting.Exceptions;

/*
* @Author: xjm
* @Description:
* @Date: Friday, 14 January 2022 22:17:52
* @Email: 326308290@qq.com
*/
public class RemotingSendRequestException:RemotingException
{
    public RemotingSendRequestException()
    {
    }

    protected RemotingSendRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public RemotingSendRequestException(string? message) : base(message)
    {
    }

    public RemotingSendRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}