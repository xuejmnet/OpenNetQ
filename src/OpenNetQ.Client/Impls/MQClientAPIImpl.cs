using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenNetQ.Client.Exceptions;
using OpenNetQ.Common;
using OpenNetQ.Common.NameSrv;
using OpenNetQ.Common.Protocol;
using OpenNetQ.Common.Protocol.Header.NameSrerver;
using OpenNetQ.Common.Protocol.Route;
using OpenNetQ.Logging;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Netty;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Client.Impls
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 14:43:05
    /// Email: 326308290@qq.com
    public class MQClientAPIImpl
    {
        private static readonly ILogger<MQClientAPIImpl> _logger = OpenNetQLoggerFactory.CreateLogger<MQClientAPIImpl>();
        private static readonly bool _sendSmartMsg = true;


        private readonly RemotingClientOption _remotingClientOption;
        private readonly IClientRemotingProcessor _clientRemotingProcessor;
        private readonly IRPCHook _rpcHook;
        private readonly ClientConfig _clientConfig;
        private readonly TopAddressing _topAddressing;
        private readonly IRemotingClient _remotingClient;

        public MQClientAPIImpl(RemotingClientOption remotingClientOption,
            IClientRemotingProcessor clientRemotingProcessor,
            IRPCHook? rpcHook,
            ClientConfig clientConfig
            )
        {
            _remotingClientOption = remotingClientOption;
            _clientRemotingProcessor = clientRemotingProcessor;
            _rpcHook = rpcHook;
            _clientConfig = clientConfig;
            _topAddressing = new TopAddressing();
            _remotingClient = new NettyRemotingClient(remotingClientOption);
            _remotingClient.RegisterRPCHook(_rpcHook);
            _remotingClient.RegisterProcessor(RequestCode.CHECK_TRANSACTION_STATE, _clientRemotingProcessor, null);
            _remotingClient.RegisterProcessor(RequestCode.NOTIFY_CONSUMER_IDS_CHANGED, _clientRemotingProcessor, null);
            _remotingClient.RegisterProcessor(RequestCode.RESET_CONSUMER_CLIENT_OFFSET, _clientRemotingProcessor, null);
            _remotingClient.RegisterProcessor(RequestCode.GET_CONSUMER_STATUS_FROM_CLIENT, _clientRemotingProcessor, null);
            _remotingClient.RegisterProcessor(RequestCode.GET_CONSUMER_RUNNING_INFO, _clientRemotingProcessor, null);
            _remotingClient.RegisterProcessor(RequestCode.CONSUME_MESSAGE_DIRECTLY, _clientRemotingProcessor, null);
            _remotingClient.RegisterProcessor(RequestCode.PUSH_REPLY_MESSAGE_TO_CLIENT, _clientRemotingProcessor, null);
        }

        public void createTopic(string addr, string defaultTopic, TopicConfig topicConfig,
        long timeoutMillis)
        {
            CreateTopicRequestHeader requestHeader = new CreateTopicRequestHeader();
            requestHeader.setTopic(topicConfig.getTopicName());
            requestHeader.setDefaultTopic(defaultTopic);
            requestHeader.setReadQueueNums(topicConfig.getReadQueueNums());
            requestHeader.setWriteQueueNums(topicConfig.getWriteQueueNums());
            requestHeader.setPerm(topicConfig.getPerm());
            requestHeader.setTopicFilterType(topicConfig.getTopicFilterType().name());
            requestHeader.setTopicSysFlag(topicConfig.getTopicSysFlag());
            requestHeader.setOrder(topicConfig.isOrder());

            RemotingCommand request = RemotingCommand.createRequestCommand(RequestCode.UPDATE_AND_CREATE_TOPIC, requestHeader);

            RemotingCommand response = this.remotingClient.invokeSync(MixAll.brokerVIPChannel(this.clientConfig.isVipChannelEnabled(), addr),
                request, timeoutMillis);
            assert response != null;
            switch (response.getCode())
            {
                case ResponseCode.SUCCESS:
                    {
                        return;
                    }
                default:
                    break;
            }

            throw new MQClientException(response.getCode(), response.getRemark());
        }

        public async Task<TopicRouteData> GetTopicRouteInfoFromNameServerAsync(string topic, long timeoutMillis)
        {

            return await GetTopicRouteInfoFromNameServerAsync(topic, timeoutMillis, true);
        }
        public async Task<TopicRouteData> GetTopicRouteInfoFromNameServerAsync(string topic, long timeoutMillis,
            bool allowTopicNotExist)
        {
            GetRouteInfoRequestHeader requestHeader = new GetRouteInfoRequestHeader();
            requestHeader.Topic = topic;

            RemotingCommand request = RemotingCommand.CreateRequestCommand(RequestCode.GET_ROUTEINFO_BY_TOPIC, requestHeader);

            RemotingCommand response = (await this._remotingClient.InvokeAsync(null, request, timeoutMillis));
            switch (response.Code)
            {
                case ResponseCode.TOPIC_NOT_EXIST:
                    {
                        if (allowTopicNotExist)
                        {
                            _logger.LogWarning($"get Topic [{topic}] RouteInfoFromNameServer is not exist value");
                        }

                    }
                    break;
                case ResponseCode.SUCCESS:
                    {
                        byte[]? body = response.Body;
                        if (body != null)
                        {
                            return TopicRouteData.Decode<TopicRouteData>(body);
                        }
                    }

                    break;
                default:
                    break;
            }

            throw new MQClientException(response.Code, response.Remark);
        }
    }
}
