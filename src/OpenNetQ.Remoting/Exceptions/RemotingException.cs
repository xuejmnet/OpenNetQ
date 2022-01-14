using System;
using System.Runtime.Serialization;

namespace OpenNetQ.Remoting.Exceptions;

/*
* @Author: xjm
* @Description:
* @Date: Friday, 14 January 2022 22:15:18
* @Email: 326308290@qq.com
*/
public class RemotingException:Exception
{
    public RemotingException()
    {
    }

    protected RemotingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public RemotingException(string? message) : base(message)
    {
    }

    public RemotingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}