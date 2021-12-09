using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace WsaAssistant.Libs
{
    public sealed class Adb
    {
        private static Adb instance;
        public static Adb Instance
        {
            get
            {
                if (instance == null)
                    instance = new Adb();
                return instance;
            }
        }
        private DeviceData Device { get; set; }
        private AdvancedAdbClient AdbClient { get; set; }
        private Adb()
        {
            if (!AdbServer.Instance.GetStatus().IsRunning)
            {
                AdbServer server = new AdbServer();
                var path = Path.Combine(this.ProcessPath(), "adb.exe");
                if (!File.Exists(path))
                    throw new FileNotFoundException("ADB文件已丢失，请重新安装程序！");
                StartServerResult result = server.StartServer(path, false);
                if (result != StartServerResult.Started)
                    throw new ApplicationException("Can't start adb server");
            }
        }
        public bool TryConnect()
        {
            try
            {
                if (Device == null)
                {
                    if (!WSA.Instance.Running)
                        WSA.Instance.Start();
                    var count = 0;
                    while (count < 10)
                    {
                        if (!string.IsNullOrEmpty(WsaIp))
                        {
                            AdbClient = new AdvancedAdbClient();
                            AdbClient.Connect(WsaIp);
                            Device = AdbClient.GetDevices().FirstOrDefault(x => x.State == DeviceState.Online);
                            break;
                        }
                        else
                        {
                            count++;
                            Thread.Sleep(5000);
                        }
                    }
                }
                return Device != null;
            }
            catch { }
            return false;
        }
        public void Close()
        {
            try
            {
                if (AdbServer.Instance.GetStatus().IsRunning)
                {
                    foreach (var process in Process.GetProcessesByName("ADB.EXE"))
                        process.Kill();
                }
            }
            catch { }
        }
        public bool Connect()
        {
            try
            {
                if (Device == null)
                {
                    AdbClient = new AdvancedAdbClient();
                    AdbClient.Connect(WsaIp);
                    Device = AdbClient.GetDevices().FirstOrDefault(x => x.State == DeviceState.Online);
                }
                return Device != null;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Connect", ex);
            }
            return false;
        }
        public string WsaIp
        {
            get
            {
                try
                {
                    var find = "arp -a|findstr 00-15-5d";
                    Command.Instance.Excute(find, out string address);
                    address = address.Substring(find + "&exit").Replace("\r\n", "");
                    if (string.IsNullOrEmpty(address))
                        return string.Empty;
                    LogManager.Instance.LogInfo("WsaIp:" + address);
                    return address.Splits(new[] { ' ' }).FirstOrDefault().Trim();
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogError("WsaIp", ex);
                    return string.Empty;
                }
            }
        }
        public bool Install(string packagePath)
        {
            try
            {
                AdbClient.Install(Device, File.OpenRead(packagePath));
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Install", ex);
            }
            return false;
        }
        public bool Downgrade(string packagePath)
        {
            try
            {
                AdbClient.Install(Device, File.OpenRead(packagePath), "-r", "-d");
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Downgrade", ex);
            }
            return false;
        }
        public bool Uninstall(string packageName)
        {
            try
            {
                PackageManager manager = new PackageManager(AdbClient, Device);
                manager.UninstallPackage(packageName);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Uninstall", ex);
            }
            return false;
        }
        public List<string> GetAll(string condition = "")
        {
            try
            {
                PackageManager manager = new PackageManager(AdbClient, Device);
                var packages = manager.Packages.Select(x => x.Key);
                return packages.Where(x => string.IsNullOrEmpty(condition) ||
                   x.Contains(condition, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetAll", ex);
                return new List<string>();
            }
        }
    }
}