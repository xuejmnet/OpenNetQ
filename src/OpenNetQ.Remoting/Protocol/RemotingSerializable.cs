using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Core;

namespace OpenNetQ.Remoting.Protocol
{
    public class RemotingSerializable
    {
        private static IOpenNetQSerializer _openNetQSerializer;
        public static void Init(IOpenNetQSerializer openNetQSerializer)
        {
            _openNetQSerializer = openNetQSerializer;
        }

        public static byte[] Encode<T>(T obj)
        {
            return  _openNetQSerializer.Serialize(obj);
        }
        public static T Decode<T>(byte[] data)
        {
            return _openNetQSerializer.Deserialize<T>(data);
        }
    }
}
