using Downloader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WsaAssistant.Libs
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
        private const string WSA_PRODUCE_ID = "9p3395vx91nr";
        public const string WSA_DEPENDENCE = "8wekyb3d8bbwe";
        public List<string> FeatureList { get; }
        public event BooleanHandler DownloadComplete;
        public Node<string, Uri, bool?, DownloadPackage> PackageList { get; }
        private WSA()
        {
            PackageList = new Node<string, Uri, bool?, DownloadPackage>();
            FeatureList = new List<string> { "Microsoft-Hyper-V", "VirtualMachinePlatform" };
            DownloadManager.Instance.ProgressComplete += DownloadManager_ProgressComplete;
        }
        public void Start()
        {
            try
            {
                if (!Running)
                {
                    var cmd = @"explorer.exe shell:appsFolder\MicrosoftCorporationII.WindowsSubsystemForAndroid_8wekyb3d8bbwe!App";
                    Command.Instance.Excute(cmd, out _);
                }
            }
            catch { }
        }
        public void Open()
        {
            try
            {
                var cmd = @"explorer.exe shell:appsFolder\MicrosoftCorporationII.WindowsSubsystemForAndroid_8wekyb3d8bbwe!SettingsApp";
                Command.Instance.Excute(cmd, out _);
            }
            catch { }
        }
        public void Reset()
        {
            try
            {
                Command.Instance.Shell("Get-AppxPackage *WindowsSubsystemForAndroid*|Reset-AppxPackage", out _);
            }
            catch { }
        }
        public async Task ReStart()
        {
            await Stop();
            Start();
        }
        public bool HasFeature
        {
            get
            {
                var count = 0;
                foreach (var package in FeatureList)
                {
                    if (CheckFeature(package))
                        count++;
                }
                return count == FeatureList.Count;
            }
        }
        public async Task Stop()
        {
            try
            {
                await Adb.Instance.Excute("shutdown -p", 200);
            }
            catch { }
        }
        public async Task<bool> HasUpdate()
        {
            bool hasNew = false;
            try
            {
                var packages = await AppX.Instance.GetPackages(WSA_PRODUCE_ID);
                foreach (var package in packages)
                {
                    if (package.Key.Contains("windowssubsystemforandroid", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var version = package.Key.Splits("_")?.ElementAt(1);
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
            catch (Exception ex)
            {
                LogManager.Instance.LogError("HasUpdate", ex);
            }
            return hasNew;
        }
        public bool InstallFeature()
        {
            try
            {
                int count = 0;
                foreach (var package in FeatureList)
                {
                    if (!CheckFeature(package))
                        InstallFeature(package);
                    else
                        count++;
                }
                return count != FeatureList.Count;
            }
            catch { }
            return false;
        }
        public bool Running
        {
            get
            {
                try
                {
                    var ps = Process.GetProcessesByName("vmmemWSA");
                    return ps != null && ps.Length > 0;
                }
                catch { }
                return false;
            }
        }
        private void InstallFeature(string packageName)
        {
            try
            {
                Command.Instance.Excute($"DISM /Online /Enable-Feature /All /FeatureName:{packageName} /NoRestart", out string message);
                LogManager.Instance.LogInfo("Install WSA:" + message);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("InstallFeature", ex);
            }
        }
        private bool CheckFeature(string packageName)
        {
            try
            { 
                Command.Instance.Excute($"DISM /Online /Get-FeatureInfo:{packageName} /English", out string message);
                LogManager.Instance.LogInfo("Check VM:" + message);
                return message.Before("State", "Enabled");
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("CheckFeature", ex);
            }
            return false;
        }
        public bool HasWsa
        {
            get
            {
                try
                {
                    Command.Instance.Shell("Get-AppxPackage|findstr WindowsSubsystemForAndroid", out string message);
                    LogManager.Instance.LogInfo("Pepair WSA:" + message);
                    return !string.IsNullOrEmpty(message);
                }
                catch { }
                return false;
            }
        }
        public async Task<bool> Retry(bool reconstruction)
        {
            GC.Collect();
            switch (reconstruction)
            {
                case true:
                    {
                        return await PepairAsync();
                    }
                default:
                    {
                        try
                        {
                            var failedList = PackageList.Where(x => x.Item3 == null || x.Item3 == false);
                            if (failedList != null && failedList.Any())
                            {
                                for (var idx = 0; idx < failedList.Count(); idx++)
                                {
                                    var failed = failedList.ElementAt(idx);
                                    PackageList.AddOrUpdate(failed.Item1, failed.Item2, null, failed.Item4);
                                }
                                var list = failedList.Select(x => x.Item2).ToArray();
                                await DownloadManager.Instance.Create(list).ConfigureAwait(false);
                            }
                            return true;
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.LogError("Retry", ex);
                            return false;
                        }
                    }
            }
        }
        private void DownloadManager_ProgressComplete(object sender, bool hasError, Uri address, string path)
        {
            try
            {
                var item = PackageList.FindItem(address);
                if (item != null)
                {
                    PackageList.AddOrUpdate(item.Item1, item.Item2, !hasError, default);
                    if (PackageList.Count == PackageList.GetCount(x => x.Item3.HasValue))
                    {
                        var count = PackageList.GetCount(x => x.Item3 == true);
                        DownloadComplete?.Invoke(this, count == PackageList.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("DownloadManager_ProgressComplete", ex);
            }
        }
        public List<string> GetDiskList()
        {
            List<string> diskList = new List<string>();
            try
            {
                var disks = DriveInfo.GetDrives();
                foreach (var disk in disks)
                    diskList.Add(disk.Name);
            }
            catch
            {
                diskList.Add("C:\\");
            }
            return diskList;
        }
        public async Task<Node<string, Uri, bool?, DownloadPackage>> GetFilePath()
        {
            try
            {
                var packages = await AppX.Instance.GetPackages(WSA_PRODUCE_ID);
                foreach (var package in packages)
                    PackageList.AddOrUpdate(package.Key, new Uri(package.Value));
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetFilePath", ex);
            }
            return PackageList;
        }
        public async Task<bool> PepairAsync()
        {
            int count = 0;
            try
            {
                for (var idx = 0; idx < PackageList.Count; idx++)
                {
                    var url = PackageList.GetIndex(idx);
                    var path = Path.Combine(this.ProcessPath(), url.Item1);
                    if (File.Exists(path))
                    {
                        count++;
                        PackageList.AddOrUpdate(url.Item1, url.Item2, true, new DownloadPackage { FileName = path });
                    }
                    else
                        await DownloadManager.Instance.Create(url.Item2).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("PepairAsync", ex);
            }
            return count == PackageList.Count;
        }
        public void Clear()
        {
            try
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
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Clear", ex);
            }
        }
    }
}