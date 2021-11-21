using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WSATools.Libs.Model;

namespace WSATools.Libs
{
    public static class Ext
    {
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
                if (registryKey == null)
                {
                    registryKey = Registry.ClassesRoot.CreateSubKey(".apk\\shell\\open");
                    var title = langType == LangType.Chinese ? "使用 WSA助手 安装" : "Use WSATools Install";
                    registryKey.SetValue(string.Empty, title);
                    var commandKey = registryKey.CreateSubKey("Command");
                    commandKey.SetValue(string.Empty, $"{path} %1");
                    RegistryKey iconKey = registryKey.CreateSubKey("DefaultIcon");
                    var iconPath = Path.Combine(path.ProcessPath(), "icon.ico");
                    iconKey.SetValue(string.Empty, iconPath);
                }
                DB.Instance.SetData("menu", DateTime.Now.ToString("yyyyMMddHHmmss"));
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("AddMenu", ex);
                return false;
            }
        }
        public static bool RemoveMenu(this object _)
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(@".apk\shell\wsa");
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("RemoveMenu", ex);
                return false;
            }
        }
        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using var timeoutCancellationTokenSource = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                await task;
            }
            else
                throw new TimeoutException("The operation has timed out.");
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