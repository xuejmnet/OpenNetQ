using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using OpenNetQ.Concurrent;
using OpenNetQ.Core;
using OpenNetQ.Remoting.Protocol;

namespace OpenNetQ.Remoting.Netty
{
    public class ResponseTask
    {
        private static readonly IInternalLogger _logger = InternalLoggerFactory.GetInstance<ResponseTask>();
        private readonly IChannel _channel;
        private readonly int _opaque;
        private readonly long _timeoutMillis;
        private readonly Action<ResponseTask>? _callback;
        private readonly CountdownEvent _countdownEvent = new CountdownEvent(1);
        private volatile bool _isOk;
        private volatile RemotingCommand _responseCommand;
        private volatile Exception _exception;
        private readonly long _beginTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private readonly SemaphoreReleaseOnlyOnce _once;
        private readonly DoOnlyOnce _callbackDoOnlyOnce = new DoOnlyOnce();

        public ResponseTask(IChannel channel, int opaque, long timeoutMillis, Action<ResponseTask>? callback, SemaphoreReleaseOnlyOnce once)
        {
            _channel = channel;
            _opaque = opaque;
            _timeoutMillis = timeoutMillis;
            _callback = callback;
            _once = once;
        }

        public int GetOpaque()
        {
            return _opaque;
        }
        public bool IsAsyncRequest()
        {
            return _callback != null;
        }

        public RemotingCommand WaitResponse()
        {
            _countdownEvent.Wait((int)_timeoutMillis);
            return _responseCommand;
        }

        public void SetSendResponseOk(bool isOk)
        {
            _isOk = isOk;
        }

        public bool IsSendResponseOk()
        {
            return _isOk;
        }

        public IChannel GetChannel()
        {
            return _channel;
        }

        public void AddResponse(RemotingCommand cmd)
        {
            _responseCommand = cmd;
            _countdownEvent.Signal();
        }

        public RemotingCommand GetResponseCommand()
        {
            return _responseCommand;
        }
        public void SetResponseCommand(RemotingCommand cmd)
        {
            _responseCommand = cmd;
        }

        public void SetException(Exception exception)
        {
            _exception = exception;
        }

        public Exception GetException()
        {
            return _exception;
        }

        public bool IsTimeOut()
        {
            var diff = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _beginTimestamp;
            return diff > _timeoutMillis;
        }

        public void Release()
        {
            _once?.Release();
        }


        public void ExecuteInvokeCallback()
        {
            if (_callback != null)
            {
                if (_callbackDoOnlyOnce.IsUnDo())
                {
                    _callback.Invoke(this);
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(_channel)}: {_channel}, {nameof(_opaque)}: {_opaque}, {nameof(_timeoutMillis)}: {_timeoutMillis}, {nameof(_callback)}: {_callback}, {nameof(_countdownEvent)}: {_countdownEvent}, {nameof(_isOk)}: {_isOk}, {nameof(_responseCommand)}: {_responseCommand}, {nameof(_exception)}: {_exception}, {nameof(_beginTimestamp)}: {_beginTimestamp}, {nameof(_once)}: {_once}, {nameof(_callbackDoOnlyOnce)}: {_callbackDoOnlyOnce}";
        }
    }
}