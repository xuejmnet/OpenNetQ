using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Common.Helper;

namespace OpenNetQ.Client.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/15 11:21:32
    /// Email: 326308290@qq.com
    public class MQBrokerException: Exception
    {
        public  int ResponseCode { get; }
        public string ErrorMessage { get; }
        public  string? BrokerAddress { get; }
        public MQBrokerException(int responseCode,string errorMessage):base(FAQUrl.AttachDefaultURL($"CODE: {responseCode} DESC: {errorMessage}"))
        {
            ResponseCode = responseCode;
            ErrorMessage = errorMessage;
        }

        public MQBrokerException(int responseCode, string errorMessage,string? brokerAddress):base(FAQUrl.AttachDefaultURL($"CODE: {responseCode} DESC: {errorMessage} Broker: {brokerAddress}"))
        {
            ResponseCode = responseCode;
            ErrorMessage = errorMessage;
            BrokerAddress = brokerAddress;
        }
    }
}
