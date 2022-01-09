using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace WsaAssistant.Libs
{
    public sealed class Icons
    {
        private static Icons instance;
        public static Icons Instance
        {
            get
            {
                if (instance == null)
                    instance = new Icons();
                return instance;
            }
        }
        private bool init = false;
        public Bitmap Default { get; }
        private string ProgramFolder { get; }
        private IEnumerable<string> InkInfos { get; }
        private Dictionary<string, string> PackageInfos { get; }
        private const string DEFAULT_ICON_BASE64_URL = "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";
        private Icons()
        {
            ProgramFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");
            InkInfos = Directory.GetFiles(ProgramFolder, "*.lnk");
            PackageInfos = new Dictionary<string, string>();
            Default = DEFAULT_ICON_BASE64_URL.ToBitmap();
        }
        public void Init()
        {
            if (!init)
            {
                init = true;
                foreach (var file in InkInfos)
                {
                    var shellType = Type.GetTypeFromProgID("WScript.Shell");
                    dynamic shell = Activator.CreateInstance(shellType);
                    var shortcut = shell.CreateShortcut(file);
                    string args = shortcut.Arguments;
                    PackageInfos.Add(Path.GetFileNameWithoutExtension(file), args);
                }
            }
        }
        public string GetDisplayName(string argValue)
        {
            try
            {
                var item = PackageInfos.FirstOrDefault(x => x.Value.Contains(argValue));
                return item.Key;
            }
            catch { }
            return string.Empty;
        }
        public Bitmap GetDisplayIcon(string argValue)
        {
            try
            {
                var item = PackageInfos.FirstOrDefault(x => x.Value.Contains(argValue));
                string filePath = Path.Combine(ProgramFolder, $"{item.Key}.lnk");
                if (File.Exists(filePath))
                    return Icon.ExtractAssociatedIcon(filePath).ToBitmap();
            }
            catch { }
            return null;
        }
    }
}