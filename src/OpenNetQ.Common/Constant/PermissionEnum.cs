using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.Constant
{
    [Flags]
    public enum PermissionEnum
    {
        INHERIT=1,
        WRITE=1<<1,
        READ=1<<2,
        PRIORITY=1<<3
    }
}