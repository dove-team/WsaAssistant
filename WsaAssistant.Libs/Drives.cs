using Downloader;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WsaAssistant.Libs.Model;

namespace WsaAssistant.Libs
{
    public sealed class Drives
    {
        private static Drives instance;
        public static Drives Instance
        {
            get
            {
                if (instance == null)
                    instance = new Drives();
                return instance;
            }
        }
        private const string OPENGL_PRODUCE_ID = "9nqpsl29bfff";
        public event BooleanHandler DownloadComplete;
        public Node<string, Uri, bool?, DownloadPackage> PackageList { get; }
        private Drives()
        {
            PackageList = new Node<string, Uri, bool?, DownloadPackage>();
            DownloadManager.Instance.ProgressComplete += Instance_ProgressComplete;
        }
        private void Instance_ProgressComplete(object sender, bool hasError, Uri address, string path)
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
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(path)
                };
                Process.Start(psi);
                DownloadComplete?.Invoke(path, true);
            }
        }
        public bool HasOpenGL
        {
            get
            {
                try
                {
                    Command.Instance.Shell("Get-AppxPackage|findstr Microsoft.D3DMappingLayers", out string message);
                    LogManager.Instance.LogInfo("Pepair OpenGL:" + message);
                    return !string.IsNullOrEmpty(message);
                }
                catch { }
                return false;
            }
        }
        public async Task<bool> InstallOpenGL()
        {
            try
            {
                var packages = await AppX.Instance.GetPackages(OPENGL_PRODUCE_ID);
                for (var idx = 0; idx < packages.Count; idx++)
                {
                    var package = packages.ElementAt(idx);
                    var path = Path.Combine(this.ProcessPath(), package.Key);
                    if (File.Exists(path))
                        PackageList.AddOrUpdate(package.Key, new Uri(package.Value), true, new DownloadPackage { FileName = path });
                    else
                    {
                        PackageList.AddOrUpdate(package.Key, new Uri(package.Value));
                        await DownloadManager.Instance.Create(package.Value).ConfigureAwait(false);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("InstallOpen", ex);
                return false;
            }
        }
        public string GetLink(GPUType type)
        {
            string url = string.Empty;
            switch (type)
            {
                case GPUType.Intel:
                    url = "https://downloadmirror.intel.com/685037/igfx_win_101.1069.exe";
                    break;
                case GPUType.Nvidia:
                    url = "https://developer.nvidia.com/51006_quadro_win11_win10-dch_64bit_international";
                    break;
                case GPUType.AMD:
                    url = "https://drivers.amd.com/drivers/DX12-WSL-Radeon-Software-Adrenalin-20.45.01.31-Dec15.exe";
                    break;
            }
            return url;
        }
        public async Task<bool> Retry(bool reconstruction)
        {
            GC.Collect();
            switch (reconstruction)
            {
                case true:
                    {
                        return await InstallOpenGL();
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
        public async Task WSLDrive(GPUType type)
        {
            try
            {
                string url = GetLink(type);
                HttpHeader headers = new HttpHeader();
                if (type == GPUType.AMD)
                    headers.Referer = "https://www.amd.com/";
                await DownloadManager.Instance.Create(url, headers).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                LogManager.Instance.LogError("WSLDrive", ex);
            }
        }
    }
}