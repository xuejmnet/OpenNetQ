namespace OpenNetQ.Store
{
    /// <summary>
    /// 提交日志派发者
    /// </summary>
    /// Author: xjm
    /// Created: 2022/3/25 9:29:44
    /// Email: 326308290@qq.com
    public interface ICommitLogDispatcher
    {
        /// <summary>
        /// 从存储区发送消息以构建消费队列、索引和筛选数据
        /// </summary>
        /// <param name="request"></param>
        void Dispatch(DispatchRequest request);
    }
}
