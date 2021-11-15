using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Downloader;
using System.Net;
using System.ComponentModel;
using WSATools.Libs.Model;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

namespace WSATools.Libs
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
        private DownloadService Service { get; }
        private DirectoryInfo SaveDirectory { get; set; }
        public event ProgressHandler ProcessChange;
        public event ProgressCompleteHandler ProgressComplete;
        private DownloadManager()
        {
            var downloadOpt = new DownloadConfiguration()
            {
                ChunkCount = 8,
                Timeout = int.MaxValue,
                BufferBlockSize = 10240,
                ParallelDownload = true,
                OnTheFlyDownload = false,
                MaximumBytesPerSecond = 1024 * 1024,
                MaxTryAgainOnFailover = int.MaxValue,
                TempDirectory = Path.Combine(Environment.CurrentDirectory, "temp"),
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
            Service = new DownloadService(downloadOpt);
            Service.DownloadFileCompleted += OnDownloadFileCompleted;
            Service.DownloadProgressChanged += DownloadProgressChanged;
        }
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var hasError = e.Cancelled || e.Error != null;
            string fileName = string.Empty;
            if (e.UserState is DownloadPackage package)
                fileName = package.FileName;
            ProgressComplete?.Invoke(sender, hasError, fileName);
        }
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProcessChange?.Invoke(e.ProgressedByteSize, e.TotalBytesToReceive);
        }
        public void Init(string root)
        {
            SaveDirectory = new DirectoryInfo(root);
        }
        public async Task<DownloadResult> Create(string url)
        {
            try
            {
                await Service.DownloadFileTaskAsync(url, SaveDirectory).ConfigureAwait(false);
                return new DownloadResult(true, Service.Package);
            }
            catch { }
            return new DownloadResult();
        }
        public void Cancel()
        {
            Service.CancelAsync();
        }
        public async Task Resume(DownloadPackage package)
        {
            await Service.DownloadFileTaskAsync(package);
        }
        public void Clear()
        {
            foreach (var path in array)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}