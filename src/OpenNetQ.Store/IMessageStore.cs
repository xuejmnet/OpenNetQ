using System;

namespace OpenNetQ.Store;

/*
* @Author: xjm
* @Description:
* @Date: Monday, 27 December 2021 22:26:39
* @Email: 326308290@qq.com
*/
public interface IMessageStore:IDisposable
{
    /// <summary>
    /// 加载存储消息
    /// </summary>
    /// <returns></returns>
    bool Load();
    /// <summary>
    /// 启动
    /// </summary>
    void Start();
    /// <summary>
    /// 停止
    /// </summary>
    void Stop();

    Task AddMessageAsync();


    LinkedList<ICommitLogDispatcher> GetDispatcherList();
}