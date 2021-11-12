using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using WSATools.Libs;

namespace WSATools.Update
{
    class Program
    {
        public static string UpgradeFile { get; private set; } = string.Empty;
        static void Main(string[] args)
        {
            try
            {
                using Mutex mutex = new Mutex(true, Application.ProductName, out bool flag);
                if (flag)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.DoEvents();
                    Application.Run(new HostForm());
                    mutex.ReleaseMutex();
                    CheckUpdate();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Start Host", ex);
            }
        }
        static void CheckUpdate()
        {
            try
            {
                var stringContent = Client.Instance.GetContent("https://michael-eddy.github.io/config/wsa-tools.json");
                var model = JsonConvert.DeserializeObject<VersionInfo>(stringContent);
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                if (version != null && model != null)
                {
                    if (version.Major < model.Main || version.Minor < model.Second || version.Build < model.Fix)
                    {
                        var url = Client.Instance.DownloadPath(model, out Uri uri);
                        if (!string.IsNullOrEmpty(url))
                        {
                            UpgradeFile = Path.Combine(Environment.CurrentDirectory, $"update.{uri.Ext}");
                            if (!string.IsNullOrEmpty(UpgradeFile))
                                Downloader.Create(url, UpgradeFile).GetAwaiter().GetResult();
                        }
                    }
                }
            }
            catch { }
        }
    }
}