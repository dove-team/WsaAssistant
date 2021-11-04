using System;

namespace WSATools.Libs
{
    public static class Ext
    {
        public static bool Before(this string str, string start, string end)
        {
            var startIndex = str.IndexOf(start);
            var endIndex = str.IndexOf(end);
            if (startIndex != -1)
                return startIndex < endIndex;
            return false;
        }
        public static long ToInt64(this string chars)
        {
            try
            {
                if (long.TryParse(chars, out long result))
                    return result;
                else
                    return Convert.ToInt64(chars);
            }
            catch { }
            return default;
        }
        public static string Substring(this string str, string startChars, string endChars = null)
        {
            var startIndex = str.IndexOf(startChars, StringComparison.CurrentCultureIgnoreCase);
            int leftPadding = startChars.Length, endIndex = str.Length;
            if (!string.IsNullOrEmpty(endChars))
                endIndex = str.IndexOf(endChars, StringComparison.CurrentCultureIgnoreCase);
            return str.Substring(startIndex + leftPadding, endIndex - startIndex - leftPadding);
        }
        public static string Clear(this string str)
        {
            try
            {
                return str.Replace("\t", "").Replace("\r\n", "").Trim(new[] { ' ' });
            }
            catch { }
            return str;
        }
    }
}