using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WSATools.Libs
{
    public sealed class Downloader
    {
        public static async Task<bool> Create(string url, string path)
        {
            try
            {
                DateTime startTime = DateTime.UtcNow;
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                if (File.Exists(path))
                    File.Delete(path);
                using Stream responseStream = response.GetResponseStream();
                using Stream fileStream = new FileStream(path, FileMode.CreateNew);
                byte[] buffer = new byte[20480];
                int bytesRead = await responseStream.ReadAsync(buffer, 0, 20480);
                while (bytesRead > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    DateTime nowTime = DateTime.UtcNow;
                    if ((nowTime - startTime).TotalMinutes > 30)
                        return false;
                    bytesRead = await responseStream.ReadAsync(buffer, 0, 20480);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Create", ex);
                return false;
            }
        }
    }
}