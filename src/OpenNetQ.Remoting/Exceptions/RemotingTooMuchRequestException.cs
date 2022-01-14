using System;
using System.Runtime.Serialization;

namespace OpenNetQ.Remoting.Exceptions;

/*
* @Author: xjm
* @Description:
* @Date: Friday, 14 January 2022 22:18:51
* @Email: 326308290@qq.com
*/
public class RemotingTooMuchRequestException:RemotingException
{
    public RemotingTooMuchRequestException()
    {
    }

    protected RemotingTooMuchRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public RemotingTooMuchRequestException(string? message) : base(message)
    {
    }

    public RemotingTooMuchRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}