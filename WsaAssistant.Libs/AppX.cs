using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WsaAssistant.Libs
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
        private HttpClient HttpClient { get; }
        private Architecture Architecture { get; }
        private Dictionary<string, Dictionary<string, string>> Cache { get; }
        private bool IsArm => Architecture == Architecture.Arm || Architecture == Architecture.Arm64;
        private AppX()
        {
            array = new string[2];
            Cache = new Dictionary<string, Dictionary<string, string>>();
            Architecture = RuntimeInformation.ProcessArchitecture;
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
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            handler.ServerCertificateCustomValidationCallback += (_, _, _, _) => { return true; };
            HttpClient = new HttpClient(handler);
        }
        public async Task<Dictionary<string, string>> GetPackages(string productId)
        {
            if (!Cache.ContainsKey(productId))
            {
                Dictionary<string, string> directory = new Dictionary<string, string>();
                var stringContent = new StringContent($"type=ProductId&url={productId}&ring=WIS&lang=zh-CN", Encoding.UTF8, "application/x-www-form-urlencoded");
                var respone = await HttpClient.PostAsync("https://store.rg-adguard.net/api/GetFiles", stringContent);
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
                                    var href = a.Attributes["href"].Value;
                                    directory.Add(key, href);
                                }
                            }
                        }
                    }
                }
                var value = SupportPackages(directory);
                Cache.Add(productId, value);
                return value;
            }
            else
            {
                return Cache.FirstOrDefault(x => x.Key == productId).Value;
            }
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
        private Dictionary<string, string> SupportPackages(Dictionary<string, string> directory)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>(directory);
            if (directory != null && directory.Count > 0)
            {
                var names = directory.Select(x => x.Key).ToList();
                if (IsArm)
                {
                    if (names.ItemContainEquals("arm", "arm64"))
                    {
                        keyValuePairs.Clear();
                        foreach (var item in directory.Where(x => x.Key.Contains("arm64")))
                            keyValuePairs.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    if (names.ItemContains("x86"))
                    {
                        keyValuePairs.Clear();
                        foreach (var item in directory.Where(x => x.Key.Contains("arm64")))
                            keyValuePairs.Add(item.Key, item.Value);
                    }
                }
            }
            return keyValuePairs;
        }
    }
}