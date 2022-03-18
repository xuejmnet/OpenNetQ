using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Common.Protocol
{
    public class RemotingSysResponseCode
    {
        public const int SUCCESS = 0;
        public const int SYSTEM_ERROR = 1;
        public const int SYSTEM_BUSY = 2;
        public const int REQUEST_CODE_NOT_SUPPORTED = 3;
        public const int TRANSACTION_FAILED = 4;
    }
}
