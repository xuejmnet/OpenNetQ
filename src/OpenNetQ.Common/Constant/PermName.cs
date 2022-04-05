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
        public const int PERM_PRIORITY = 0X1 << 3;
        public const int PERM_READ = 0X1 << 2;
        public const int PERM_WRITE = 0X1 << 1;
        public const int PERM_INHERIT = 0X1 << 0;

        public static string Perm2String(int perm)
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
        public static bool IsReadable(int perm)
        {
            return (perm & PERM_READ) == PERM_READ;
        }
        public static bool IsWriteable(int perm)
        {
            return (perm & PERM_WRITE) == PERM_WRITE;
        }
        public static bool IsInherited(int perm)
        {
            return (perm & PERM_INHERIT) == PERM_INHERIT;
        }

    }
}