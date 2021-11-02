using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WSATools.Libs
{
    public sealed class Command
    {
        private static Command instance;
        public static Command Instance
        {
            get
            {
                if (instance == null)
                    instance = new Command();
                return instance;
            }
        }
        public string DeviceCode { get; set; }
        private Command() { }
        public async Task Init()
        {
            if (!await Adb.Instance.Pepair())
            {
                MessageBox.Show("初始化ADB失败！请确保网络通畅！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        public void ReStart()
        {
            Adb.Instance.Reload();
        }
        public List<string> ConnectDevices()
        {
            List<string> devices = new List<string>();
            if (Adb.Instance.ExcuteCommand("adb devices", out string message))
            {
                var lines = message.Substring("List of devices attached");
                foreach (var item in lines.Split("\r\n"))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var deviceCode = item.Split('\t').FirstOrDefault();
                        var deviceName = DeviceName(deviceCode);
                        devices.Add($"{deviceName}({deviceCode})".Clear());
                    }
                }
            }
            return devices;
        }
        public string SysVersion()
        {
            string version = string.Empty,
                  command = $"adb -s {DeviceCode} shell getprop ro.build.version.release";
            if (Adb.Instance.ExcuteCommand(command, out string message))
                version = message.Substring($"{command}&exit").Clear();
            return version;
        }
        public string DeviceName(string deviceCode)
        {
            string command = $"adb -s {deviceCode} shell getprop ro.product.model";
            return Adb.Instance.ExcuteCommand(command, out string message)
                ? message.Substring($"{command}&exit") : deviceCode;
        }
        public List<string> GetAll(string condition = "")
        {
            List<string> packages = new List<string>();
            string command = string.IsNullOrEmpty(condition) ? $"adb -s {DeviceCode} shell pm list packages" :
                $"adb -s {DeviceCode} shell pm list packages|grep {condition}";
            if (Adb.Instance.ExcuteCommand(command, out string message))
            {
                var lines = message.Substring($"{command}&exit");
                foreach (var item in lines.Split("\r\n"))
                {
                    if (!string.IsNullOrEmpty(item))
                        packages.Add(item.Split(':').LastOrDefault());
                }
            }
            return packages.OrderBy(x => x).ToList();
        }
        public bool Enable(string packageName)
        {
            string command = $"adb -s {DeviceCode} shell pm enable {packageName}";
            if (Adb.Instance.ExcuteCommand(command, out string message))
                return message.Substring($"{command}&exit").Contains("success", StringComparison.CurrentCultureIgnoreCase);
            return false;
        }
        public bool Disable(string packageName)
        {
            string command = $"adb -s {DeviceCode} shell pm disable-user {packageName}";
            if (Adb.Instance.ExcuteCommand(command, out string message))
                return message.Substring($"{command}&exit").Contains("success", StringComparison.CurrentCultureIgnoreCase);
            return false;
        }
        public bool Install(string packagePath)
        {
            string command = $"adb -s{DeviceCode} install {packagePath}";
            if (Adb.Instance.ExcuteCommand(command, out string message))
                return message.Substring($"{command}&exit").Contains("success", StringComparison.CurrentCultureIgnoreCase);
            return false;
        }
        public bool Remove(string packageName)
        {
            string command = $"adb -s {DeviceCode} shell pm uninstall --user 0 {packageName}";
            if (Adb.Instance.ExcuteCommand(command, out string message))
                return message.Substring($"{command}&exit").Contains("success", StringComparison.CurrentCultureIgnoreCase);
            return false;
        }
        public bool Excute(string cmd, out string message)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.StandardInput.WriteLine($"{cmd}&exit");
                p.StandardInput.AutoFlush = true;
                message = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
    }
}