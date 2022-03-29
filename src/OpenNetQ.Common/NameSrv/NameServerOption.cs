using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.NameSrv
{
    public class NameServerOption
    {
        public bool IsOrderMessageEnable { get; set; } = false;
        
        public bool ClusterTest{ get; set; } = false;
        public string ProductEnvName { get; set; } = "center";
        public string ConfigStorePath { get; set; } = Path.Combine("NameServer", "NameServer.Text");
        public string KvConfigPath { get; set; } = Path.Combine("NameServer", "KvConfig.Text");
    }
}