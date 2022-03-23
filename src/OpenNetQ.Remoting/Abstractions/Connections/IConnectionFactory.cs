namespace OpenNetQ.Remoting.Abstractions.Connections
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 17 November 2020 10:40:44
* @Email: 326308290@qq.com
*/
    public interface IConnectionFactory
    {
        IConnection CreateConnection();
    }
}