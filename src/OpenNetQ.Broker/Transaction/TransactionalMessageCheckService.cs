using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenNetQ.Common.Options;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.TaskSchedulers;

namespace OpenNetQ.Broker.Transaction
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/9 9:02:36
    /// Email: 326308290@qq.com
    public class TransactionalMessageCheckService: AbstractServiceThread
    {
        private readonly BrokerOption _brokerOption;

        private static readonly ILogger<TransactionalMessageCheckService>
            _logger = OpenNetQLoggerFactory.CreateLogger<TransactionalMessageCheckService>();

        public TransactionalMessageCheckService(BrokerOption brokerOption)
        {
            _brokerOption = brokerOption;
        }
        public override string GetServiceName()
        {
            return nameof(TransactionalMessageCheckService);
        }

        public override void Run()
        {
            _logger.LogInformation($"Start transaction check service thread!");
            var interval = _brokerOption.TransactionCheckInterval;
            while (!IsStopped())
            {
                WaitForRunning(interval);
            }
            _logger.LogInformation("End transaction check service thread!");

        }

        public override void OnWaitEnd()
        {
            //TODO
        }
    }
}
