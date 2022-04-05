using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Core
{
    public interface IJsonSerializer
    {
        string ToJson<T>(T obj);
        T FromJson<T>(string json);
    }
}