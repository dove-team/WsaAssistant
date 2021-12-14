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
        private string ProgramFolder { get; }
        private IEnumerable<string> InkInfos { get; }
        private Dictionary<string, string> PackageInfos { get; }
        private Icons()
        {
            ProgramFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");
            InkInfos = Directory.GetFiles(ProgramFolder, "*.lnk");
            PackageInfos = new Dictionary<string, string>();
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
        public Icon GetDisplayIcon(string argValue)
        {
            try
            {
                var item = PackageInfos.FirstOrDefault(x => x.Value.Contains(argValue));
                string filePath = Path.Combine(ProgramFolder, $"{item.Key}.lnk");
                if (File.Exists(filePath))
                    return Icon.ExtractAssociatedIcon(filePath);
            }
            catch { }
            return default;
        }
    }
}