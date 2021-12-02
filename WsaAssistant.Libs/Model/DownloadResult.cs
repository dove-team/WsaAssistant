using Downloader;

namespace WsaAssistant.Libs.Model
{
    public sealed class DownloadResult
    {
        public DownloadResult()
        {
            Package = null;
            CreateStatus = false;
        }
        public DownloadResult(bool status, DownloadPackage package)
        {
            Package = package;
            CreateStatus = status;
        }
        public bool CreateStatus { get; set; }
        public DownloadPackage Package { get; set; }
    }
}