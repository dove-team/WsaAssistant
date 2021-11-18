using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using WSATools.Libs.Model;

namespace WSATools.Libs
{
    public static class Ext
    {
        public static bool AddMenu(this string path, LangType langType)
        {
            try
            {
                var title = langType == LangType.Chinese ? "使用 WSA助手 安装" : "Use WSATools Install";
                Interaction.Shell($"cmd /c echo yes | REG ADD HKEY_CLASSES_ROOT\\.apk\\shell\\wsa /d \"{title}\"", AppWinStyle.Hide);
                Thread.Sleep(100);
                var cmd = Conversions.ToString(Operators.AddObject(Operators.AddObject("cmd /c echo yes | REG ADD HKEY_CLASSES_ROOT\\.apk\\shell\\wsa\\command /d ", path), "\" %1\""));
                Interaction.Shell(cmd, AppWinStyle.Hide);
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
        public static bool UnZip(this string zipFileName, string targetDirectory)
        {
            try
            {
                var zip = new FastZip();
                zip.ExtractZip(zipFileName, targetDirectory, "");
                File.Delete(zipFileName);
                return Adb.Instance.HasBrige;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("UnZip", ex);
            }
            return false;
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