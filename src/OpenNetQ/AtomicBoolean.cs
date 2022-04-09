using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetQ
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/4/9 9:12:43
    /// Email: 326308290@qq.com

    public class AtomicBoolean
    {
        private int value;
        internal const int True = 1;
        internal const int False = 0;

        public AtomicBoolean()
            : this(false)
        {
        }

        public AtomicBoolean(bool value) => this.value = value ? 1 : 0;

        public bool Value
        {
            get => Interlocked.CompareExchange(ref this.value, 0, 0) == 1;
            set => Interlocked.Exchange(ref this.value, value ? 1 : 0);
        }

        public bool CompareAndSet(bool expect, bool update)
        {
            int comparand = expect ? 1 : 0;
            return Interlocked.CompareExchange(ref this.value, update ? 1 : 0, comparand) == comparand;
        }

        public bool GetAndSet(bool newValue) => Interlocked.Exchange(ref this.value, newValue ? 1 : 0) == 1;
    }
}
