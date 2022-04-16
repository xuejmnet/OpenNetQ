using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.Remoting.Common;

namespace OpenNetQ.Client.Impls.Factory
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 14:33:38
    /// Email: 326308290@qq.com
    public class MQClientInstance
    {
        private static readonly ILogger<MQClientInstance> _logger = OpenNetQLoggerFactory.CreateLogger<MQClientInstance>();
        private static readonly long LOCK_TIMEOUT_MILLS = 3000;



        private readonly RemotingClientOption _remotingClientOption;
        private readonly ClientConfig _clientConfig;
        private readonly MQAdminImpl _mqAdminImpl;
        private readonly MQClientAPIImpl _mqClientAPIImpl;
        private readonly IClientRemotingProcessor _clientRemotingProcessor;

        public MQClientInstance(RemotingClientOption remotingClientOption,ClientConfig clientConfig, int instanceIndex, string clientId, IRPCHook? rpcHook)
        {
            _remotingClientOption = remotingClientOption;
            _clientConfig = clientConfig;
            _mqClientAPIImpl =
                new MQClientAPIImpl(remotingClientOption, _clientRemotingProcessor, rpcHook, clientConfig);
        }

        public MQClientAPIImpl GetMQClientAPIImpl()
        {
            return _mqClientAPIImpl;
        }
    }
}
