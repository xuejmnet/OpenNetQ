using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetQ.Remoting;

namespace OpenNetQ.Common.Protocol.Header.NameSrerver
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/16 15:09:10
    /// Email: 326308290@qq.com
    public class GetRouteInfoRequestHeader:ICommandCustomHeader
    {
        public void CheckFields()
        {
            
        }
        public string Topic { get; set; }
    }
}
