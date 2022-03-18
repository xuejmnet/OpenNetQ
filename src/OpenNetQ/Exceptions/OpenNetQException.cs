using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Exceptions
{
    public class OpenNetQException: Exception
    {
        public OpenNetQException()
        {
        }

        protected OpenNetQException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public OpenNetQException(string? message) : base(message)
        {
        }

        public OpenNetQException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
