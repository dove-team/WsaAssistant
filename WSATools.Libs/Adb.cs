﻿using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace WSATools.Libs
{
    public sealed class Adb
    {
        private string AdbLocation { get; }
        private List<string> IgnorePackages { get; }
        private string deviceCode;
        public string DeviceCode
        {
            get
            {
                if (string.IsNullOrEmpty(deviceCode))
                {
                    var address = WsaIp;
                    if (!string.IsNullOrEmpty(address))
                    {
                        var wsaIp = address.Splits(new[] { ' ' }).FirstOrDefault();
                        ExcuteCommand("adb connect " + wsaIp, out string connect);
                        LogManager.Instance.LogInfo("adb connect:" + connect);
                        Thread.Sleep(8);
                        if (ExcuteCommand("adb devices", out string message))
                        {
                            LogManager.Instance.LogInfo("adb devices:" + message);
                            var lines = message.Substring("List of devices attached");
                            foreach (var device in lines.Splits("\r\n"))
                            {
                                var code = device.Splits('\t').FirstOrDefault();
                                var cmd = $"adb -s {code} shell getprop ro.product.model";
                                ExcuteCommand(cmd, out string name);
                                LogManager.Instance.LogInfo("adb name:" + name);
                                name = name.Substring(cmd + "&exit");
                                if (name.Contains("Subsystem for Android", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    deviceCode = code;
                                    break;
                                }
                            }
                        }
                    }
                }
                LogManager.Instance.LogInfo("DeviceCode:" + deviceCode);
                return deviceCode;
            }
        }
        public bool HasBrige => File.Exists(AdbLocation);
        public static Adb Instance { get; } = new Adb();
        private Adb()
        {
            AdbLocation = Path.Combine(this.ProcessPath(), "adb.exe");
            IgnorePackages = new List<string>
            {
                "android","com.microsoft.windows.systemapp","com.android.permissioncontroller","com.android.shell","com.android.webview",
                "com.android.packageinstaller","com.android.settings","com.android.systemui","com.microsoft.windows.userapp","com.android.se",
                "android.ext.services","com.android.dynsystem","com.android.providers.calendar","com.android.providers.media","android.ext.shared",
                "com.android.inputdevices","com.android.providers.settings","com.android.keychain","com.android.systemui.auto_generated_rro_vendor__",
                "com.android.settings.auto_generated_rro_vendor__","com.android.certinstaller","com.android.modulemetadata","com.android.providers.downloads",
                "com.android.providers.media.module","com.android.providers.downloads.ui","com.android.companiondevicemanager","com.android.location.fused",
                "com.android.networkstack","com.android.statementservice","com.android.providers.settings.auto_generated_rro_vendor__","com.android.providers.contacts",
                "com.amazon.device.messaging","com.android.networkstack.tethering","com.android.networkstack.permissionconfig","com.android.traceur",
                "android.auto_generated_rro_vendor__","com.android.localtransport","com.android.hotspot2.osulogin"
            };
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
                    foreach (var item in lines.Splits("\r\n"))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var name = item.Splits(':').LastOrDefault();
                            if (!IgnorePackages.Contains(name))
                                packages.Add(name);
                        }
                    }
                }
            }
            return packages.OrderBy(x => x).ToList();
        }
        public bool Connect
        {
            get
            {
                if (string.IsNullOrEmpty(WsaIp))
                {
                    WSA.Instance.Start();
                    int count = 0;
                    while (count < 10)
                    {
                        if (WSA.Instance.Running)
                        {
                            count = 0;
                            Thread.Sleep(3000);
                            while (count < 10)
                            {
                                if (!string.IsNullOrEmpty(WsaIp))
                                    return true;
                                else
                                {
                                    count++;
                                    Thread.Sleep(3000);
                                }
                            }
                            return false;
                        }
                        count++;
                        Thread.Sleep(3000);
                    }
                    return false;
                }
                return true;
            }
        }
        private string WsaIp
        {
            get
            {
                var find = "arp -a|findstr 00-15-5d";
                Command.Instance.Excute(find, out string address);
                address = address.Substring(find + "&exit").Replace("\r\n", "");
                LogManager.Instance.LogInfo("WsaIp:" + address);
                return address;
            }
        }
        public bool Install(string packagePath)
        {
            if (string.IsNullOrEmpty(DeviceCode))
                return false;
            else
            {
                string command = $"adb -s {DeviceCode} install \"{packagePath}\"";
                if (ExcuteCommand(command, out string message))
                {
                    LogManager.Instance.LogInfo(message);
                    return message.Substring($"{command}&exit").Contains("success", StringComparison.CurrentCultureIgnoreCase);
                }
                return false;
            }
        }
        public bool Downgrade(string packagePath)
        {
            if (string.IsNullOrEmpty(DeviceCode))
                return false;
            else
            {
                string command = $"adb -s {DeviceCode} -r -d install \"{packagePath}\"";
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
                string command = $"adb -s {DeviceCode} shell pm uninstall --user 0 \"{packageName}\"";
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
                    $"cd /d \"{this.ProcessPath()}\"",
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