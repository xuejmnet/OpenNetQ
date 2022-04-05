using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OpenNetQ.Core;
using OpenNetQ.Extensions;

namespace OpenNetQ.Remoting.Protocol
{
    public abstract class RemotingSerializable
    {
        public static byte[] Encode<T>(T obj)
        {
            var json = ToJson(obj);
            
           return json.GetBytes();
        }
        public static string ToJson<T>(T obj)
        {
            return ServerContainer.GetRequiredService<IJsonSerializer>().ToJson(obj);
        }
        public static T Decode<T>(byte[] data)
        {
            var json = data.GetString();
            return FromJson<T>(json);
        }

        public static T FromJson<T>(string json)
        {
            return ServerContainer.GetRequiredService<IJsonSerializer>().FromJson<T>(json);
        }

        public byte[] Encode()
        {
            return Encode(this);
        }

        public string ToJson()
        {
            return ToJson(this);
        }
    }
}
