namespace OpenNetQ.Client.Producers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 12:04:31
    /// Email: 326308290@qq.com
    public enum LocalTransactionStateEnum
    {
        COMMIT_MESSAGE,
        ROLLBACK_MESSAGE,
        UNKNOW,
    }
}
