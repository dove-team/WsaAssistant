using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WsaAssistant.Libs;
using WsaAssistant.Libs.Model;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WsaAssistant
{
    public sealed class UpdateBackgroundThread
    {
        private static UpdateBackgroundThread instance;
        public static UpdateBackgroundThread Instance
        {
            get
            {
                if (instance == null)
                    instance = new UpdateBackgroundThread();
                return instance;
            }
        }
        private HttpClient HttpClient { get; }
        private string UpgradeFile { get; set; }
        private string UpdateMessage { get; set; }
        private UpdateBackgroundThread()
        {
            HttpClient = new HttpClient();
        }
        private string DownloadPath(VersionInfo model, out VersionUri uri)
        {
            var current = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? "x64" : "arm64";
            uri = model.Urls.FirstOrDefault(x => x.Platform == current);
            var url = uri?.Url;
            if (!string.IsNullOrEmpty(url))
            {
                var content = GetContent($"https://api.pingping6.com/tools/lanzou/?url={url}");
                var info = JsonConvert.DeserializeObject<JObject>(content);
                if (info != null && Convert.ToInt32(info["code"].ToString()) == 1)
                {
                    var file = info["file"].ToString();
                    LogManager.Instance.LogInfo($"DownloadPath:{file}");
                    return file;
                }
            }
            return string.Empty;
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
        public bool ShowUpdate(CancelEventArgs e)
        {
            HttpClient.Dispose();
            var hasUpdate = !string.IsNullOrEmpty(UpgradeFile);
            LogManager.Instance.LogInfo($"ShowUpdate:{(hasUpdate ? "有最新版本更新！" : "没有最新版本更新！")}");
            if (hasUpdate)
            {
                e.Cancel = true;
                string title = LangManager.Instance.Current == LangType.Chinese ? "WsaAssistant有新版本，是否进行更新？" : "WsaAssistant Has new-version，upgrade now？";
                if (MessageBox.Show(UpdateMessage, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Process.Start(UpgradeFile);
                    Thread.Sleep(2000);
                }
            }
            return hasUpdate;
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
                            var url = DownloadPath(model, out VersionUri uri);
                            if (!string.IsNullOrEmpty(url))
                            {
                                UpdateMessage = LangManager.Instance.Current == LangType.Chinese ? model.ChMessage : model.EnMessage;
                                UpgradeFile = Path.Combine(this.ProcessPath(), $"update.{uri.Ext}");
                                if (!string.IsNullOrEmpty(UpgradeFile))
                                {
                                    LogManager.Instance.LogInfo($"UpgradeFile:{UpgradeFile}");
                                    await DownloadManager.Instance.Create(url);
                                }
                            }
                        }
                    }
                }
                catch { }
            });
        }
    }
}