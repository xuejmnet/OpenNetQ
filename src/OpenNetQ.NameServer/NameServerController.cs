using System;
using Microsoft.Extensions.Logging;
using OpenNetQ.Remoting.Common;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.NameServer
{
    public class NameServerController:InjectService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly RemotingServerOption _remotingServerOption;

        public NameServerController(IServiceProvider serviceProvider):base(serviceProvider)
        {
            _loggerFactory = GetRequiredService<ILoggerFactory>();
            _remotingServerOption = GetRequiredService<RemotingServerOption>();
        }
    }
}