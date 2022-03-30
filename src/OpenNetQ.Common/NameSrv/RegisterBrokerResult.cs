using System;
using OpenNetQ.Common.Protocol.Body;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.NameSrv
{
    public class RegisterBrokerResult
    {
        public string? HaServerAddr { get; set; }
        public string? MasterAddr { get; set; }
        public KVTable? KVTable { get; set; }
    }
}