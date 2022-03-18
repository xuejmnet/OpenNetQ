using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ
{
    public class AtomicSequence
    {
        private  int value;
        

        public AtomicSequence(int initValue)
        {
            value = initValue;
        }

        public int GetAndIncrement()
        {
            return Interlocked.Increment(ref value)-1;
        }
    }
}