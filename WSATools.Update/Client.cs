using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace WSATools.Update
{
    sealed class Client
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
        private HttpClient HttpClient { get; }
        private Client()
        {
            HttpClient = new HttpClient();
        }
        public string DownloadPath(VersionInfo model, out Uri uri)
        {
            var current = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? "x64" : "arm64";
            uri = model.Urls.FirstOrDefault(x => x.Platform == current);
            var url = uri?.Url;
            if (!string.IsNullOrEmpty(url))
            {
                var content = GetContent($"https://api.pingping6.com/tools/lanzou/?url={url}");
                var info = JsonConvert.DeserializeObject<JObject>(content);
                if (info != null && Convert.ToInt32(info["code"].ToString()) == 1)
                    return info["file"].ToString();
            }
            return string.Empty;
        }
        public string GetContent(string url)
        {
            try
            {
                return HttpClient.GetStringAsync(url).GetAwaiter().GetResult();
            }
            catch { }
            return string.Empty;
        }
    }
}