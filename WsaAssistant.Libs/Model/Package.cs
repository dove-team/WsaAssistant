using System;

namespace WsaAssistant.Libs.Model
{
    public sealed class Package
    {
        public Package(string package)
        {
            PackageName = package;
        }
        public string PackageIcon { get; set; }
        public string PackageName { get; set; }
        internal void Init()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Init", ex);
            }
        }
    }
}