using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WsaAssistant.Libs.Model;

namespace WsaAssistant.Libs
{
    public sealed class Client
    {
        private static Client instance;
        public static Client Instance
        {
            get
            {
                if (instance == null)
                    instance = new Client();
                return instance;
            }
        }
        private string SavePath { get; set; }
        private HttpClient HttpClient { get; }
        private string DownloadPath { get; set; }
        public event BooleanHandler DownloadComplete;
        public bool HasUpdate => !string.IsNullOrEmpty(DownloadPath);
        private Client()
        {
            HttpClient = new HttpClient();
            DownloadManager.Instance.ProgressComplete += DownloadManager_ProgressComplete;
        }
        private void DownloadManager_ProgressComplete(object sender, bool hasError, Uri address, string path)
        {
            SavePath = path;
            DownloadComplete?.Invoke(this, !hasError);
        }
        public void CheckUpdate()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var stringContent = GetContent("https://michael-eddy.github.io/config/wsa-tools.json");
                    var model = JsonConvert.DeserializeObject<VersionInfo>(stringContent);
                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    if (version != null && model != null)
                    {
                        if (version.Major < model.Major || version.Minor < model.Minor || version.Build < model.Build)
                        {
                            var current = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? "x64" : "arm64";
                            var uri = model.Urls.FirstOrDefault(x => x.Platform == current);
                            var url = uri?.Url;
                            if (!string.IsNullOrEmpty(url))
                            {
                                var content = GetContent($"https://api.pingping6.com/tools/lanzou/?url={url}");
                                var info = JsonConvert.DeserializeObject<JObject>(content);
                                if (info != null && Convert.ToInt32(info["code"].ToString()) == 1)
                                {
                                    DownloadPath = info["file"].ToString();
                                    LogManager.Instance.LogInfo($"DownloadPath:{DownloadPath}");
                                }
                            }
                        }
                    }
                }
                catch { }
            });
        }
        public void Install()
        {
            try
            {
                Process.Start(SavePath);
            }
            catch { }
        }
        public async Task<bool> Start()
        {
            try
            {
                if (HasUpdate)
                    await DownloadManager.Instance.Create(DownloadPath).ConfigureAwait(false);
            }
            catch { }
            return false;
        }
        private string GetContent(string url)
        {
            try
            {
                var stringContent = HttpClient.GetStringAsync(url).GetAwaiter().GetResult();
                LogManager.Instance.LogInfo($"GetContent:{stringContent}");
                return stringContent;
            }
            catch { }
            return string.Empty;
        }
    }
}