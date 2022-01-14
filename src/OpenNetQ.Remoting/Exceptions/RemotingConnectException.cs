using System;
using System.Runtime.Serialization;

namespace OpenNetQ.Remoting.Exceptions;

/*
* @Author: xjm
* @Description:
* @Date: Friday, 14 January 2022 22:16:26
* @Email: 326308290@qq.com
*/
public class RemotingConnectException:RemotingException
{
    public RemotingConnectException()
    {
    }

    protected RemotingConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public RemotingConnectException(string? message) : base(message)
    {
    }

    public RemotingConnectException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}