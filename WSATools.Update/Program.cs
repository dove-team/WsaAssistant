using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WSATools.Libs;

namespace WSATools.Update
{
    class Program
    {
        public static string UpgradeFile { get; private set; } = string.Empty;
        public static string UpdateMessage { get; private set; } = string.Empty;
        static void Main(string[] args)
        {
            try
            {
                using Mutex mutex = new Mutex(true, Application.ProductName, out bool flag);
                if (flag)
                {
                    CheckUpdate();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.DoEvents();
                    Application.Run(new HostForm());
                    mutex.ReleaseMutex();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Start Host", ex);
            }
        }
        static void CheckUpdate()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var stringContent = Client.Instance.GetContent("https://michael-eddy.github.io/config/wsa-tools.json");
                    var model = JsonConvert.DeserializeObject<VersionInfo>(stringContent);
                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    if (version != null && model != null)
                    {
                        if (version.Major < model.Major || version.Minor < model.Minor || version.Build < model.Build)
                        {
                            var url = Client.Instance.DownloadPath(model, out Uri uri);
                            if (!string.IsNullOrEmpty(url))
                            {
                                UpdateMessage = CultureInfo.CurrentCulture.Name.Contains("zh", StringComparison.CurrentCultureIgnoreCase)
                                ? model.ChMessage : model.EnMessage;
                                UpgradeFile = Path.Combine(Environment.CurrentDirectory, $"update.{uri.Ext}");
                                if (!string.IsNullOrEmpty(UpgradeFile))
                                    await DownloadManager.Instance.Create(url);
                            }
                        }
                    }
                }
                catch { }
            });
        }
    }
}