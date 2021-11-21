using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WSATools.Libs
{
    public sealed class WSA
    {
        private static WSA instance;
        public static WSA Instance
        {
            get
            {
                if (instance == null)
                    instance = new WSA();
                return instance;
            }
        }
        public IEnumerable<string> PackageList { get; }
        private WSA()
        {
            PackageList = new[] { "HypervisorPlatform", "VirtualMachinePlatform" };
        }
        public void Start()
        {
            var cmd = @"explorer.exe shell:appsFolder\MicrosoftCorporationII.WindowsSubsystemForAndroid_8wekyb3d8bbwe!App";
            Command.Instance.Excute(cmd, out _);
        }
        public (bool VM, bool WSA, bool Run) State()
        {
            var count = 0;
            foreach (var package in PackageList)
            {
                if (Check(package))
                    count++;
            }
            return (count == PackageList.Count(), Pepair(), Running);
        }
        public async Task<bool> HasUpdate()
        {
            bool hasNew = false;
            try
            {
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                HttpClient httpClient = new HttpClient(handler);
                var stringContent = new StringContent("type=ProductId&url=9p3395vx91nr&ring=WIS&lang=zh-CN", Encoding.UTF8, "application/x-www-form-urlencoded");
                var respone = await httpClient.PostAsync("https://store.rg-adguard.net/api/GetFiles", stringContent);
                if (respone.StatusCode == HttpStatusCode.OK)
                {
                    var html = await respone.Content.ReadAsStringAsync();
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var table = doc.DocumentNode.SelectSingleNode("table");
                    if (table != null)
                    {
                        foreach (var tr in table.SelectNodes("tr"))
                        {
                            var a = tr.SelectSingleNode("td").SelectSingleNode("a");
                            if (a != null && a.InnerText.Contains("WindowsSubsystemForAndroid"))
                            {
                                var version = a.InnerText.Splits("_")?.ElementAt(1);
                                if (!string.IsNullOrEmpty(version))
                                {
                                    Command.Instance.Shell("Get-AppxPackage|findstr WindowsSubsystemForAndroid", out string message);
                                    var packageVersion = message.Split("\r\n").ElementAt(1).Split("_").ElementAt(1).Trim();
                                    hasNew = version.NewerThan(packageVersion);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("HasUpdate", ex);
            }
            return hasNew;
        }
        public bool Init()
        {
            int count = 0;
            foreach (var package in PackageList)
            {
                if (!Check(package))
                    Install(package);
                else
                    count++;
            }
            return count < 3;
        }
        public bool Running
        {
            get
            {
                var ps = Process.GetProcessesByName("vmmemWSA");
                return ps != null && ps.Length > 0;
            }
        }
        private void Install(string packageName)
        {
            Command.Instance.Excute($"DISM /Online /Enable-Feature /All /FeatureName:{packageName} /NoRestart", out string message);
            LogManager.Instance.LogInfo("Install WSA:" + message);
        }
        private bool Check(string packageName)
        {
            Command.Instance.Excute($"DISM /Online /Get-FeatureInfo:{packageName}", out string message);
            LogManager.Instance.LogInfo("Check VM:" + message);
            return message.Before("状态", "已启用");
        }
        public bool Pepair()
        {
            Command.Instance.Shell("Get-AppxPackage|findstr WindowsSubsystemForAndroid", out string message);
            LogManager.Instance.LogInfo("Pepair WSA:" + message);
            return !string.IsNullOrEmpty(message);
        }
        public void Clear()
        {
            Command.Instance.Shell("Get-AppxPackage|findstr WindowsSubsystemForAndroid", out string message);
            var packageName = message.Split("\r\n").ElementAt(1).Split(":").LastOrDefault().Trim();
            Command.Instance.Shell($"Remove-AppxPackage {packageName}", out string packageMessage);
            LogManager.Instance.LogInfo("Clear WSA:" + packageMessage);
            foreach (var package in PackageList)
            {
                Command.Instance.Excute($"DISM /Online /Disable-Feature /All /FeatureName:{package} /NoRestart", out string resultMessage);
                LogManager.Instance.LogInfo("Clear VM WSA:" + resultMessage);
            }
        }
    }
}