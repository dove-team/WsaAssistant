using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WSATools.Libs
{
    public sealed class AppX
    {
        public static async Task<Dictionary<string, string>> GetFilePath()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            var url = "https://store.rg-adguard.net/api/GetFiles";
            var content = "type=ProductId&url=9p3395vx91nr&ring=WIF&lang=zh-CN";
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            HttpClient httpClient = new HttpClient(handler);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
            var respone = await httpClient.PostAsync(url, stringContent);
            if (respone.StatusCode == HttpStatusCode.OK)
            {
                var html = await respone.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var table = doc.DocumentNode.SelectSingleNode("table");
                foreach (var tr in table.SelectNodes("tr"))
                {
                    var a = tr.SelectSingleNode("td").SelectSingleNode("a");
                    if (a != null)
                    {
                        var key = a.InnerHtml.ToString();
                        var value = a.Attributes["href"].Value;
                        list.Add(key, value);
                    }
                }
            }
            return list;
        }
        public async Task<bool> PepairAsync(Dictionary<string, string> urls)
        {
            try
            {
                int count = 0, total = urls.Count;
                foreach (var url in urls)
                {
                    var path = Path.Combine(Environment.CurrentDirectory, url.Key);
                    if (await Downloader.Create(url.Value, path))
                        count++;
                }
                return count == total;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}