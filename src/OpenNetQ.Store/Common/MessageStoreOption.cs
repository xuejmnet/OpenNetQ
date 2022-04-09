using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ.Store.Common
{
    public class MessageStoreOption
    {
        public int HAPort { get; set; }=10912;
        public bool EnableDLegerCommitLog { get; set; }=false;
    }
}
