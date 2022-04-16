using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Client.Exceptions;
using OpenNetQ.Client.Impls.Factory;
using OpenNetQ.Client.Producers;
using OpenNetQ.Common;
using OpenNetQ.Common.Helper;
using OpenNetQ.Common.Messages;
using OpenNetQ.Remoting.Abstractions;

namespace OpenNetQ.Client.Impls.Producers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 13:02:45
    /// Email: 326308290@qq.com
    public class DefaultMQProducerImpl : IMQProducerInner
    {
        private readonly IMQProducer _mqProducer;
        private readonly IRPCHook? _rpcHook;
        private ServiceStateEnum _serviceState = ServiceStateEnum.CREATE_JUST;
        private MQClientInstance _mqClientFactory;
        public DefaultMQProducerImpl(IMQProducer mqProducer) : this(mqProducer, null)
        {

        }
        public DefaultMQProducerImpl(IMQProducer mqProducer, IRPCHook? rpcHook)
        {
            _mqProducer = mqProducer;
            _rpcHook = rpcHook;
        }
        public ISet<string> GetPublishTopicList()
        {
            throw new NotImplementedException();
        }

        public object GetCheckListener()
        {
            throw new NotImplementedException();
        }

        public void CheckTransactionState(string addr, MessageExt msg, object checkRequestHeader)
        {
            throw new NotImplementedException();
        }

        public void UpdateTopicPublishInfo(string topic, TopicPublishInfo info)
        {
            throw new NotImplementedException();
        }

        public bool IsUnitMode()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        private void MakeSureStateOK()
        {
            if (this._serviceState != ServiceStateEnum.RUNNING)
            {
                throw new MQClientException($"The producer service state not OK, {_serviceState}", null);
            }
        }
        public async Task<List<MessageQueue>> FetchPublishMessageQueue(string topic)
        {
            MakeSureStateOK();
        }
    }
}
