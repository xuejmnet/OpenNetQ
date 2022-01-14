using System;
using System.Runtime.Serialization;

namespace OpenNetQ.Remoting.Exceptions;

/*
* @Author: xjm
* @Description:
* @Date: Friday, 14 January 2022 22:16:26
* @Email: 326308290@qq.com
*/
public class RemotingCommandException:RemotingException
{
    public RemotingCommandException()
    {
    }

    protected RemotingCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public RemotingCommandException(string? message) : base(message)
    {
    }

    public RemotingCommandException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}