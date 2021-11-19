using System;
using System.IO;
using System.Text;

namespace WSATools.Libs
{
    public sealed class DB
    {
        private static DB instance;
        public static DB Instance
        {
            get
            {
                if (instance == null)
                    instance = new DB();
                return instance;
            }
        }
        private DirectoryInfo Directory { get; }
        private DB()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Directory = new DirectoryInfo(Path.Combine(path, ".wsatools"));
            if (!Directory.Exists)
                Directory.Create();
        }
        public void SetData(string key, string value)
        {
            var filePath = Path.Combine(Directory.FullName, $"{key}.db");
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
                fileInfo.Delete();
            using var fs = fileInfo.Create();
            var bytes = Encoding.UTF8.GetBytes(value);
            fs.Write(bytes);
        }
        public string GetData(string key)
        {
            var filePath = Path.Combine(Directory.FullName, $"{key}.db");
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                using var fs = fileInfo.OpenRead();
                var bytes = new byte[fs.Length];
                fs.Write(bytes);
                return Encoding.UTF8.GetString(bytes);
            }
            return string.Empty;
        }
    }
}