using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.NameServer.KvConfig
{
    public class KvConfigManager:InjectService,IKvConfigManager
    {
        public KvConfigManager(IServiceProvider serviceProvider):base(serviceProvider)
        {
            
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void AdKvConfig(string @namespace, string key, string value)
        {
            throw new NotImplementedException();
        }

        public void Persist()
        {
            throw new NotImplementedException();
        }

        public void DeleteKvConfig(string @namespace, string key)
        {
            throw new NotImplementedException();
        }

        public byte[] GetKvListByNamespace(string @namespace)
        {
            throw new NotImplementedException();
        }

        public string GetKvConfig(string @namespace, string key)
        {
            throw new NotImplementedException();
        }

        public void PrintAllPeriodically()
        {
            throw new NotImplementedException();
        }
    }
}