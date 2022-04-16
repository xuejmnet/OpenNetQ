using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Extensions;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Protocol;
using OpenNetQ.Utils;

namespace OpenNetQ.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/15 17:20:43
    /// Email: 326308290@qq.com
    public class ClientConfig
    {
        private const string DEFAULT_INSTANCE_NAME = "DEFAULT";
        private string _namesrvAddr;
        public string ClientIP { get; set; } = RemotingUtil.GetLocalAddress();
        public string InstanceName { get; set; }= DEFAULT_INSTANCE_NAME;
        private int _clientCallbackExecutorThreads = Environment.ProcessorCount;
        protected string Namespace { get; set; }
        private bool _namespaceInitialized = false;
        protected AccessChannelEnum AccessChannel = AccessChannelEnum.LOCAL;
        private int _pollNameServerInterval = 1000 * 30;
        private int _heartbeatBrokerInterval = 1000 * 30;
        private int _persistConsumerOffsetInterval = 1000 * 5;
        private long _pullTimeDelayMillsWhenException = 1000;
        public bool UnitMode { get; set; }= false;
        public string UnitName { get; set; }
        private bool _vipChannelEnabled = false;
        private bool _useTLS = false;
        private int _mqClientApiTimeout = 3 * 1000;
        private LanguageCodeEnum _language = LanguageCodeEnum.DOTNET;

        public string BuildMQClientId()
        {
            var sb = new StringBuilder();
            sb.Append(ClientIP);
            sb.Append("@");
            sb.Append(InstanceName);
            if (UnitName.NoNullOrWhiteSpace())
            {
                sb.Append("@");
                sb.Append(UnitName);
            }

            return sb.ToString();
        }

        public void ChangeInstanceNameToPID()
        {
            if (DEFAULT_INSTANCE_NAME.Equals(InstanceName))
            {
                InstanceName = $"{Process.GetCurrentProcess().Id}#{TimeUtil.NanoTime()}";
            }
        }

        public virtual void SetUseTLS(bool useTLS)
        {
            this._useTLS = useTLS;
        }

        public string? WithNamespace(string resource)
        {
            //todo
            return null;
        }
    }
}
