using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WSATools.Libs
{
    public sealed class Adb
    {
        private string AdbRoot { get; }
        private string AdbLocation { get; }
        private string deviceCode;
        public string DeviceCode
        {
            get
            {
                if (string.IsNullOrEmpty(deviceCode))
                {
                    ExcuteCommand("adb connect 127.0.0.1:58526", out _);
                    Thread.Sleep(100);
                    if (ExcuteCommand("adb devices", out string message))
                    {
                        var lines = message.Substring("List of devices attached");
                        var device = lines.Split("\r\n").FirstOrDefault(x => x.Contains("172.")||x.Contains(":5555"));
                        if (device != null)
                            deviceCode = device.Split('\t').FirstOrDefault();
                    }
                }
                return deviceCode;
            }
        }
        public bool HasBrige => File.Exists(AdbLocation);
        public static Adb Instance { get; } = new Adb();
        private Adb()
        {
            AdbRoot = Path.Combine(Environment.CurrentDirectory, "platform-tools");
            AdbLocation = Path.Combine(AdbRoot, "adb.exe");
        }
        public async Task<bool> Pepair()
        {
            try
            {
                if (!HasBrige)
                {
                    var url = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip";
                    var path = Path.Combine(Environment.CurrentDirectory, "platform-tools-latest-windows.zip");
                    if (await Downloader.Create(url, path))
                        return Zipper.UnZip(path, Environment.CurrentDirectory);
                    else
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string Reload()
        {
            var processes = Process.GetProcessesByName("ADB.EXE");
            if (processes != null && processes.Length > 0)
            {
                foreach (var process in processes)
                    process.Kill();
            }
            ExcuteCommand("adb devices", out string message);
            return message;
        }
        public List<string> GetAll(string condition = "")
        {
            List<string> packages = new List<string>();
            if (!string.IsNullOrEmpty(DeviceCode))
            {
                string command = string.IsNullOrEmpty(condition) ? $"adb -s {DeviceCode} shell pm list packages" :
                $"adb -s {DeviceCode} shell pm list packages|grep {condition}";
                if (ExcuteCommand(command, out string message))
                {
                    var lines = message.Substring($"{command}&exit");
                    foreach (var item in lines.Split("\r\n"))
                    {
                        if (!string.IsNullOrEmpty(item))
                            packages.Add(item.Split(':').LastOrDefault());
                    }
                }
            }
            return packages.OrderBy(x => x).ToList();
        }
        public bool Install(string packagePath)
        {
            if (string.IsNullOrEmpty(DeviceCode))
                return false;
            else
            {
                string command = $"adb -s{DeviceCode} install {packagePath}";
                if (ExcuteCommand(command, out string message))
                    return message.Substring($"{command}&exit").Contains("success", StringComparison.CurrentCultureIgnoreCase);
                return false;
            }
        }
        public bool Downgrade(string packagePath)
        {
            if (string.IsNullOrEmpty(DeviceCode))
                return false;
            else
            {
                string command = $"adb -s{DeviceCode} -r -d install {packagePath}";
                if (ExcuteCommand(command, out string message))
                    return message.Substring($"{command}&exit").Contains("success", StringComparison.CurrentCultureIgnoreCase);
                return false;
            }
        }
        public bool Remove(string packageName)
        {
            if (string.IsNullOrEmpty(DeviceCode))
                return false;
            else
            {
                string command = $"adb -s {DeviceCode} shell pm uninstall --user 0 {packageName}";
                if (ExcuteCommand(command, out string message))
                    return message.Substring($"{command}&exit").Contains("success", StringComparison.CurrentCultureIgnoreCase);
                return false;
            }
        }
        public bool ExcuteCommand(string cmd, out string message)
        {
            try
            {
                List<string> cmds = new List<string>
                {
                    $"cd \"{AdbRoot}\"",
                    cmd
                };
                return Command.Instance.Excute(cmds, out message);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
    }
}