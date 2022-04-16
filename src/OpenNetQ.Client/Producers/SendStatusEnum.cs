namespace OpenNetQ.Client.Producers
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 11:06:07
    /// Email: 326308290@qq.com
    public enum SendStatusEnum
    {
        SEND_OK,
        FLUSH_DISK_TIMEOUT,
        FLUSH_SLAVE_TIMEOUT,
        SLAVE_NOT_AVAILABLE,
    }
}
