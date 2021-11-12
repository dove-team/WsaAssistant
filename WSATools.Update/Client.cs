using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using WSATools.Libs;

namespace WSATools.Update
{
    sealed class Client
    {
        public static string DownloadPath(VersionInfo model)
        {
            var current = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? "x64" : "arm64";
            var url = model.Urls.FirstOrDefault(x => x.Platform == current)?.Url;
            url = "https://wwa.lanzoui.com/iDh1Bwcun3a";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(GetContent(url));
            var iframe = doc.DocumentNode.SelectSingleNode("html/body/div[@class='d']/div[@class='d2']/div[@class='ifr']/iframe[@class='ifr2']");
            url += iframe.Attributes["src"].Value;
            doc.LoadHtml(GetContent(url));
            var scripts = doc.DocumentNode.SelectSingleNode("html/body/script").InnerHtml.Splits("\n\t\t");
            return Excute(scripts);
        }
        private static string Excute(string[] scripts)
        {
            AjaxData ajaxData = new AjaxData();
            try
            {
                string sign = string.Empty, signs = string.Empty;
                foreach (var script in scripts)
                {
                    if (!script.StartsWith("//") && script.Replace(" ", "").Contains("data:{"))
                    {
                        int start = script.IndexOf("{"), end = script.LastIndexOf("}");
                        foreach (var data in script.Substring(start + 1, end - start - 1).Splits(","))
                        {
                            string name = data.Splits(":")[0].Replace("'", ""), value = data.Splits(":")[1];
                            if (value.Contains('\''))
                                ajaxData.SetValue(name, value.Replace("\'", ""));
                            else if (int.TryParse(value, out int result))
                                ajaxData.SetValue(name, result);
                        }
                    }
                    else if (script.Contains("ajaxdata", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int start = script.IndexOf("'"), end = script.LastIndexOf("'");
                        signs = script.Substring(start + 1, end - start - 1);
                    }
                    else if (script.Contains("ispostdowns", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int start = script.IndexOf("'"), end = script.LastIndexOf("'");
                        sign = script.Substring(start + 1, end - start - 1);
                    }
                }
                ajaxData.Set(sign, signs);
            }
            catch { }
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            HttpClient httpClient = new HttpClient(handler);
            StringContent stringContent = new StringContent(ajaxData.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
            var respone = httpClient.PostAsync("https://wwa.lanzoui.com/ajaxm.php", stringContent).GetAwaiter().GetResult();
            var content = respone.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var obj = JsonConvert.DeserializeObject<JObject>(content);
            if (Convert.ToUInt32(obj["zt"].ToString()) == 1)
                return $"{obj["dom"]}/file/{obj["url"]}";
            return string.Empty;
        }
        public static string GetContent(string url)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                return httpClient.GetStringAsync(url).GetAwaiter().GetResult();
            }
            catch { }
            return string.Empty;
        }
    }
}