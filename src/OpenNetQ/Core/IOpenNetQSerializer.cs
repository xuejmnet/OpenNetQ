using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Core
{
    /// <summary>
    /// 序列化
    /// </summary>
    public interface IOpenNetQSerializer
    {
        /// <summary>
        /// 对象序列化成字节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Serialize<T>(T data);
        /// <summary>
        /// 字节反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] bytes);
    }
}
