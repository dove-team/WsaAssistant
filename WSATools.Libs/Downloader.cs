using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WSATools.Libs
{
    public sealed class Downloader
    {
        public static async Task<bool> Create(string url, string path)
        {
            try
            {
                HttpClient client = new HttpClient();
                using var httpResponse = await client.GetAsync(url);
                using var stream = await httpResponse.Content.ReadAsStreamAsync();
                if (File.Exists(path))
                    File.Delete(path);
                using var fs = new FileStream(path, FileMode.CreateNew);
                var buffer = new byte[4096];
                int readLength = 0, length = 0;
                while ((length = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    readLength += length;
                    fs.Write(buffer, 0, length);
                }
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}