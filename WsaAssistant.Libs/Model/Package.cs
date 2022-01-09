using System;
using System.Collections.Generic;
using System.Drawing;

namespace WsaAssistant.Libs.Model
{
    public sealed class Package
    {
        internal Package(KeyValuePair<string, string> package)
        {
            Icons.Instance.Init();
            PackageName = package.Key;
            IsSystem = !package.Value.Contains("/data/app/", StringComparison.CurrentCultureIgnoreCase);
        }
        public bool IsSystem { get; set; }
        public Bitmap PackageIcon { get; set; }
        public string DisplayName { get; set; }
        public string PackageName { get; set; }
        internal void Init()
        {
            try
            {
                PackageIcon = Icons.Instance.GetDisplayIcon(PackageName);
                if (PackageIcon == null)
                    PackageIcon = Icons.Instance.Default;
                DisplayName = Icons.Instance.GetDisplayName(PackageName);
                if (string.IsNullOrEmpty(DisplayName))
                    DisplayName = PackageName;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Init", ex);
            }
        }
    }
}