using Downloader;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WSATools.Libs
{
    public sealed class AppX
    {
        private static AppX instance;
        public static AppX Instance
        {
            get
            {
                if (instance == null)
                    instance = new AppX();
                return instance;
            }
        }
        private readonly string[] array;
        public event BooleanHandler DownloadComplete;
        public Node<string, string, bool?, DownloadPackage> PackageList { get; }
        private AppX()
        {
            array = new string[2];
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                case Architecture.Arm64:
                    array[0] = "x86";
                    array[1] = "x64";
                    break;
                case Architecture.X64:
                    array[0] = "arm";
                    array[1] = "x86";
                    break;
            }
            PackageList = new Node<string, string, bool?, DownloadPackage>();
            DownloadManager.Instance.ProgressComplete += DownloadManager_ProgressComplete;
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
                                var list = failedList.Select(x => x.Item2).ToArray();
                                await DownloadManager.Instance.Create(list);
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
        private void DownloadManager_ProgressComplete(object sender, bool hasError, string address)
        {
            var item = PackageList.FindItem(address);
            PackageList.AddOrUpdate(item.Item1, item.Item2, !hasError, default);
            if (PackageList.Count == PackageList.GetCount(x => x.Item3.HasValue))
            {
                var count = PackageList.GetCount(x => x.Item3 == true);
                DownloadComplete?.Invoke(this, count == PackageList.Count);
            }
        }
        public async Task<Node<string, string, bool?, DownloadPackage>> GetFilePath()
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
                        if (a != null)
                        {
                            var key = a.InnerHtml.ToString();
                            if (IsSupport(key))
                            {
                                var value = a.Attributes["href"].Value;
                                PackageList.AddOrUpdate(key, value);
                            }
                        }
                    }
                }
            }
            return PackageList;
        }
        private bool IsSupport(string key)
        {
            if (key.EndsWith("appx", StringComparison.CurrentCultureIgnoreCase) ||
                key.EndsWith("msixbundle", StringComparison.CurrentCultureIgnoreCase))
            {
                var count = 0;
                foreach (var type in array)
                {
                    if (key.Contains(type))
                        count++;
                }
                return count == 0;
            }
            return false;
        }
        public async Task<bool> PepairAsync()
        {
            int count = 0;
            try
            {
                for (var idx = 0; idx < PackageList.Count; idx++)
                {
                    var url = PackageList.GetIndex(idx);
                    var path = Path.Combine(Environment.CurrentDirectory, url.Item1);
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
    }
}