using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
            var handler = new HttpClientHandler { AllowAutoRedirect = true };
            HttpClient = new HttpClient(handler);
            HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh", .9));
            HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-CN", .9));
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11");
            DownloadManager.Instance.ProgressComplete += DownloadManager_ProgressComplete;
        }
        private void DownloadManager_ProgressComplete(object sender, bool hasError, Uri address, string path)
        {
            SavePath = path;
            DownloadComplete?.Invoke(this, !hasError);
        }
        public void CheckUpdate()
        {
            Task.Factory.StartNew(async () =>
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
                            string current = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? "x64" : "arm64";
                            var uri = model.Urls.FirstOrDefault(x => x.Platform == current);
                            var url = uri?.Url;
                            if (!string.IsNullOrEmpty(url))
                            {
                                var content = GetContent($"https://api.vvhan.com/api/lz?url={url}");
                                if (string.IsNullOrEmpty(content))
                                {
                                    content = await GetLzSteamUrl(url);
                                    if (!string.IsNullOrEmpty(content))
                                        DownloadPath = content;
                                }
                                else
                                {
                                    var info = JsonConvert.DeserializeObject<JObject>(content);
                                    if (info != null && info["success"].ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase))
                                        DownloadPath = info["fileUrl"].ToString();
                                }
                                LogManager.Instance.LogInfo($"DownloadPath:{DownloadPath}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogError("CheckUpdate", ex);
                }
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
        private async Task<string> GetLzSteamUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                string page = GetContent(url);
                if (!new Regex("(?<=<div class=\"off\"><div class=\"off0\"><div class=\"off1\"></div></div>)(.*)(?=</div>)").Match(page).Success)
                {
                    string fn = null;
                    foreach (Match src in new Regex("/fn[^\"]+").Matches(page))
                    {
                        if (src.Length > 10)
                            fn = $"https://{uri.Host}{src.Value}";
                    }
                    string page1 = GetContent(fn);
                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Referrer = new Uri(fn);
                    var sign = new Regex("(?<=sign':')(.*)(?=',)").Match(page1).Value;
                    var stringContent = new StringContent($"action=downprocess&signs=%3Fctdf&sign={sign}&ves=1&websign=&websignkey=5Sq0");
                    stringContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
                    var response = await client.PostAsync("https://www.lanzoux.com/ajaxm.php", stringContent);
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject js = JObject.Parse(responseString);
                    if (js["zt"].ToString() == "1")
                    {
#pragma warning disable SYSLIB0014
                        var request = (HttpWebRequest)WebRequest.Create($"{js["dom"]}/file/{js["url"]}");
                        request.Timeout = 3000;
                        request.AllowAutoRedirect = false;
                        request.Headers[HttpRequestHeader.AcceptLanguage] = "zh-CN,zh;q=0.9";
                        var resopneContent = request.GetResponse().Headers["Location"];
                        request.Abort();
#pragma warning restore SYSLIB0014
                        return resopneContent;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetLzSteamUrl", ex);
            }
            return string.Empty;
        }
    }
}