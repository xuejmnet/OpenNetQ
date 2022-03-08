using System;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Common.Extensions
{
    public static class CollectionExtension
    {
        public static bool IsEmpty<TSource>(this IEnumerable<TSource>? collection)
        {
            return collection == null || !collection.Any();
        }
        public static bool IsNotEmpty<TSource>(this IEnumerable<TSource>? collection)
        {
            return !collection.IsEmpty();
        }
    }
}