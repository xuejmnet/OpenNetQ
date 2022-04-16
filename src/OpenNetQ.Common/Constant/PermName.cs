using System;
using System.Text;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.Constant
{
    public class PermName
    {

        public static string Perm2String(PermissionEnum perm)
        {
            var stringBuilder = new StringBuilder("---");
            if (IsReadable(perm))
            {
                stringBuilder.Replace("-","R",0,1);
            }
            if (IsWriteable(perm))
            {
                stringBuilder.Replace("-","W",1,1);
            }
            if (IsInherited(perm))
            {
                stringBuilder.Replace("-","X",2,1);
            }

            return stringBuilder.ToString();
        }
        public static bool IsReadable(PermissionEnum perm)
        {
            return perm.HasFlag(PermissionEnum.READ);
        }
        public static bool IsWriteable(PermissionEnum perm)
        {
            return perm.HasFlag(PermissionEnum.WRITE);
        }
        public static bool IsInherited(PermissionEnum perm)
        {
            return perm.HasFlag(PermissionEnum.INHERIT);
        }

    }
}