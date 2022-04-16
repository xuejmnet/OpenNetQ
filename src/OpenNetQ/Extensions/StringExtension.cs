using System;
using System.Text;

/*
* @Author: xjm
* @Description:
* @Date: DATE TIME
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }
        public static bool NoNullOrEmpty(this string? value)
        {
            return !value.IsNullOrEmpty();
        }
        public static byte[] GetBytes(this string? value, Encoding encoding)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            return encoding != null ? encoding.GetBytes(value) : throw new ArgumentNullException(nameof (encoding));
        }
        public static byte[] GetBytes(this string? value)
        {
            return value.GetBytes(Encoding.UTF8);
        }
        public static string GetString(this byte[]? data, Encoding encoding)
        {
            if (data == null)
                throw new ArgumentNullException(nameof (data));
            return encoding != null ? encoding.GetString(data) : throw new ArgumentNullException(nameof (encoding));
        }
        public static string GetString(this byte[]? data)
        {
            return data.GetString(Encoding.UTF8);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public static bool NoNullOrWhiteSpace(this string value)
        {
            return !value.IsNullOrWhiteSpace();
        }
    }
}