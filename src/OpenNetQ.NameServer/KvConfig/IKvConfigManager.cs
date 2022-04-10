using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.NameServer.KvConfig
{
    public interface IKvConfigManager
    {
        void Load();
        void AdKvConfig(string @namespace, string key, string value);
        void Persist();
        void DeleteKvConfig(string @namespace, string key);
        byte[] GetKvListByNamespace(string @namespace);
        string GetKvConfig(string @namespace, string key);
        void PrintAllPeriodically();
    }
}