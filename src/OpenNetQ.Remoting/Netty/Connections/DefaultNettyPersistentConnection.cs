using System;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using KhaosLog.LoggingProvider;
using KhaosLog.NettyClientProvider.Abstractions.Connections;
using KhaosLog.NettyClientProvider.Common;

namespace KhaosLog.NettyClientProvider.Impls
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 17 November 2020 11:14:34
* @Email: 326308290@qq.com
*/
    public class DefaultNettyPersistentConnection : INettyPersistentConnection
    {
        private static readonly IInternalKhaosLogger _logger = InternalKhaosLoggerFactory.GetLogger<DefaultNettyPersistentConnection>();
        private readonly IConnectionFactory _connectionFactory;
        IConnection _connection;
        bool _disposed;

        object sync_root = new object();

        public DefaultNettyPersistentConnection(RemotingClientOption option,IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public bool IsConnectCreated => _connection != null;

        public bool TryConnect()
        {
            lock (sync_root)
            {
                if (!IsConnectCreated)
                {
                    _connection = _connectionFactory
                        .CreateConnection();
                }

                return true;
            }
        }

        public IChannel CreateChannel()
        {
            if (!IsConnectCreated)
            {
                TryConnect();
            }

            return _connection.CreateChannel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection?.Dispose();
                _connection = null;
            }
            catch (Exception ex)
            {
                _logger.Error("PersistentConnection Dispose error", ex);
                //_logger.LogCritical(ex.ToString());
            }
        }
    }
}