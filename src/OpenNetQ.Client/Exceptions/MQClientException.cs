using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Common.Helper;
using OpenNetQ.Exceptions;

namespace OpenNetQ.Client.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/15 11:01:16
    /// Email: 326308290@qq.com
    public class MQClientException: OpenNetQException
    {
        public int ResponseCode { get; set; } = -1;
        public string? ErrorMessage { get; set; }
        public MQClientException(string? errorMessage,Exception? exception):base(FAQUrl.AttachDefaultURL(errorMessage), exception)
        {
            ResponseCode = -1;
            ErrorMessage = errorMessage;
        }

        public MQClientException(int responseCode,string? errorMessage):base(FAQUrl.AttachDefaultURL($"CODE: {responseCode} DESC: {errorMessage}"))
        {
            ResponseCode = responseCode;
            ErrorMessage = errorMessage;
        }
    }
}
