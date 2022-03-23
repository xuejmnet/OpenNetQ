using System;
using System.Collections.Concurrent;
using System.Threading;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using KhaosLog.NettyClientProvider.Abstractions.Connections;
using KhaosLog.NettyClientProvider.Common;
using KhaosLog.NettyProvider.Common;

namespace KhaosLog.NettyClientProvider.Impls
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 17 November 2020 11:27:26
* @Email: 326308290@qq.com
*/
    public class ConnectionChannelPool : IConnectionChannelPool
    {
        private static readonly IInternalLogger _logger = InternalLoggerFactory.GetInstance<ConnectionChannelPool>();
        private static readonly long LOCK_TIMEOUT_MILLIS = 3000;
        private static readonly object LockObj=new object();
        private readonly ConcurrentDictionary<int, IConnection> _pools = new ConcurrentDictionary<int, IConnection>();
        private readonly IConnectionFactory _connectionFactory;
        private const int _defaultPoolSize = 1;
        private int _maxSize;
        private int _currentIndex;

        public ConnectionChannelPool(IConnectionFactory connectionFactory,RemotingClientOption option)
        {
            _connectionFactory = connectionFactory;
            _maxSize = option.MaxPoolSize <= 0 ? _defaultPoolSize : option.MaxPoolSize;
        }


        /// <summary>
        /// 尝试取一个连接
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool TryGet( out IChannel channel,out int index)
        {
            if (_maxSize > 1)
            {
                Interlocked.Increment(ref _currentIndex);
                index = Math.Abs(_currentIndex % _maxSize);
            }
            else
            {
                index = 0;
            }
            if (_pools.TryGetValue(index, out var c) && c != null && c.IsOpen)
            {
                channel = c.CreateChannel();
                return true;
            }

            if (Monitor.TryEnter(LockObj, (int) LOCK_TIMEOUT_MILLIS))
            {
                try
                {
                    if (_pools.TryGetValue(index, out c))
                    {
                        if (c != null)
                        {
                            if (c.IsOpen)
                            {
                                channel = c.CreateChannel();
                                return true;
                            }
                        }
                    }

                    c = _connectionFactory.CreateConnection();
                    _pools.AddOrUpdate(index, k => c, (k, v) => c);
                }
                catch (Exception e)
                {
                    _logger.Error($"createChannel: create channel exception", e);
                    throw e;
                }
                finally
                {
                    Monitor.Exit(LockObj);
                }
            }
            else
            {
                _logger.Warn($"createChannel:try to lock channel table, but timeout, {LOCK_TIMEOUT_MILLIS}ms");
            }

            if (c != null)
            {
                channel = c.CreateChannel();
                return true;
            }

            channel = null;
            return false;
        }

        public void CloseChannel(int index,IChannel channel)
        {
            if (channel == null)
                return;
            var addr = RemotingHelper.ParseChannelRemoteAddr(channel);
            try
            {
                if (Monitor.TryEnter(LockObj, (int) LOCK_TIMEOUT_MILLIS))
                {
                    try
                    {
                        _pools.TryGetValue(index, out var c);
                        _logger.Info($"closeChannel: begin close the channel[{addr}] Found: {c!=null}");
                        if (c == null)
                        {
                            _logger.Info($"closeChannel: the channel[{addr}] has been removed from the channel table before");
                        } else if (c != channel)
                        {
                            _logger.Info($"closeChannel: the channel[{addr}] has been closed before, and has been created again, nothing to do.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("closeChannel: close the channel exception", ex);
                    }
                    finally
                    {
                        Monitor.Exit(LockObj);
                    }
                }
                else
                {
                    _logger.Warn($"closeChannel: try to lock channel table, but timeout, {LOCK_TIMEOUT_MILLIS}ms");
                }
            }
            catch (ThreadInterruptedException e)
            {
                _logger.Error("closeChannel exception",e);
            }

        }
    }
}