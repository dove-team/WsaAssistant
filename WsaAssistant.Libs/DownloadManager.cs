using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Downloader;
using System.Net;
using System.ComponentModel;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

namespace WsaAssistant.Libs
{
    public sealed class DownloadManager
    {
        private static DownloadManager instance;
        public static DownloadManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new DownloadManager();
                return instance;
            }
        }
        private readonly List<string> array;
        private DownloadService Service { get; set; }
        private DirectoryInfo SaveDirectory { get; set; }
        private readonly DownloadConfiguration configuration;
        public event ProgressHandler ProcessChange;
        public event ProgressCompleteHandler ProgressComplete;
        private DownloadManager()
        {
            configuration = new DownloadConfiguration()
            {
                ChunkCount = 8,
                Timeout = int.MaxValue,
                BufferBlockSize = 10240,
                ParallelDownload = true,
                OnTheFlyDownload = false,
                MaximumBytesPerSecond = 1024 * 1024,
                MaxTryAgainOnFailover = int.MaxValue,
                TempDirectory = Path.Combine(this.ProcessPath(), "temp"),
                RequestConfiguration =
                {
                    Accept = "*/*",
                    KeepAlive = false,
                    UseDefaultCredentials = false,
                    Headers = new WebHeaderCollection(),
                    ProtocolVersion = HttpVersion.Version11,
                    CookieContainer =  new CookieContainer(),
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36 Edg/95.0.1020.44"
                }
            };
            array = new List<string>();
        }
        private void Build()
        {
            if (Service != null)
            {
                Service.Clear();
                Service.DownloadFileCompleted -= OnDownloadFileCompleted;
                Service.DownloadProgressChanged -= DownloadProgressChanged;
            }
            Service = new DownloadService(configuration);
            Service.DownloadFileCompleted += OnDownloadFileCompleted;
            Service.DownloadProgressChanged += DownloadProgressChanged;
        }
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var hasError = e.Cancelled || e.Error != null;
            string address = string.Empty;
            if (e.UserState is DownloadPackage package)
            {
                address = package.Address;
                array.Add(package.FileName);
            }
            if (e.Error is Exception ex)
                LogManager.Instance.LogError("OnDownloadFileCompleted", ex);
            ProgressComplete?.Invoke(sender, hasError, address);
        }
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var progressPercentage = e.ProgressPercentage.ToString("0.00");
            ProcessChange?.Invoke(progressPercentage);
        }
        public void Init(string root)
        {
            SaveDirectory = new DirectoryInfo(root);
        }
        public async Task Create(string url)
        {
            try
            {
                Build();
                await Service.DownloadFileTaskAsync(url, SaveDirectory).ConfigureAwait(false);
            }
            catch { }
        }
        public async Task Create(params string[] urls)
        {
            foreach (var url in urls)
                await Create(url);
        }
        public bool HasClear => array.Count > 0;
        public void Clear()
        {
            Directory.Delete(Path.Combine(this.ProcessPath(), "temp"));
            Service.Clear();
            foreach (var path in array)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}