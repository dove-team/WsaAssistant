using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WsaAssistant.Libs.Model;

namespace WsaAssistant.Libs
{
    public static class Ext
    {
        public static bool ItemContains(this IEnumerable<string> source, string content)
        {
            int count = 0;
            for (var idx = 0; idx < source.Count(); idx++)
                count += source.ElementAt(idx).Contains(content, StringComparison.CurrentCultureIgnoreCase) ? 1 : 0;
            return count > 0;
        }
        public static bool ItemContainEquals(this IEnumerable<string> source, string content1, string content2)
        {
            int count1 = 0, count2 = 0;
            for (var idx = 0; idx < source.Count(); idx++)
                count1 += source.ElementAt(idx).Contains(content1, StringComparison.CurrentCultureIgnoreCase) ? 1 : 0;
            for (var idx = 0; idx < source.Count(); idx++)
                count2 += source.ElementAt(idx).Contains(content2, StringComparison.CurrentCultureIgnoreCase) ? 1 : 0;
            return count1 == count2;
        }
        public static bool NewerThan(this string v1, string v2)
        {
            bool result = false;
            try
            {
                string[] v1s = v1.Splits("."), v2s = v2.Splits(".");
                for (var idx = 0; idx < v1s.Length; idx++)
                {
                    if (int.TryParse(v1s.ElementAt(idx), out int vv1) && int.TryParse(v2s.ElementAt(idx), out int vv2))
                    {
                        if (vv1 > vv2)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("EqualVersion", ex);
            }
            return result;
        }
        public static string ProcessPath<T>(this T _) where T : class
        {
            var path = Environment.ProcessPath;
            return Path.GetDirectoryName(path);
        }
        public static bool AddMenu(this string path, LangType langType)
        {
            try
            {
                RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(".apk\\shell\\open");
                if (registryKey != null)
                    Registry.ClassesRoot.DeleteSubKeyTree(".apk\\shell\\open");
                registryKey = Registry.ClassesRoot.CreateSubKey(".apk\\shell\\open");
                var title = langType == LangType.Chinese ? "使用 WSA助手 安装" : "Use WsaAssistant Install";
                registryKey.SetValue(string.Empty, title);
                var commandKey = registryKey.CreateSubKey("Command");
                commandKey.SetValue(string.Empty, $"{path} %1");
                RegistryKey iconKey = registryKey.CreateSubKey("DefaultIcon");
                var iconPath = Path.Combine(path.ProcessPath(), "icon.ico");
                iconKey.SetValue(string.Empty, iconPath);
                using DB db = new DB();
                db.SetData("menu", DateTime.Now.ToString("yyyyMMddHHmmss"));
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("AddMenu", ex);
                return false;
            }
        }
        public static bool Before(this string str, string start, string end)
        {
            var startIndex = str.IndexOf(start);
            var endIndex = str.IndexOf(end);
            if (startIndex != -1)
                return startIndex < endIndex;
            return false;
        }
        public static string[] Splits(this string str, string separator)
        {
            List<string> results = new List<string>();
            var array = str.Split(separator);
            if (array != null)
            {
                foreach (var item in array)
                {
                    var data = item.Trim();
                    if (!string.IsNullOrEmpty(data))
                        results.Add(data);
                }
            }
            return results.ToArray();
        }
        public static string[] Splits(this string str, params char[] separator)
        {
            List<string> results = new List<string>();
            var array = str.Split(separator);
            if (array != null)
            {
                foreach (var item in array)
                {
                    var data = item.Trim();
                    if (!string.IsNullOrEmpty(data))
                        results.Add(data);
                }
            }
            return results.ToArray();
        }
        public static string Substring(this string str, string startChars, string endChars = null)
        {
            var startIndex = str.IndexOf(startChars, StringComparison.CurrentCultureIgnoreCase);
            int leftPadding = startChars.Length, endIndex = str.Length;
            if (!string.IsNullOrEmpty(endChars))
                endIndex = str.IndexOf(endChars, StringComparison.CurrentCultureIgnoreCase);
            return str.Substring(startIndex + leftPadding, endIndex - startIndex - leftPadding);
        }
    }
}