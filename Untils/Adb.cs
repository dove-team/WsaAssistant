using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WSATools.Untils
{
    public sealed class Adb
    {
        public string AdbRoot { get; }
        public string AdbLocation { get; }
        public bool HasBrige => File.Exists(AdbLocation);
        public static Adb Instance { get; } = new Adb();
        private Adb()
        {
            AdbRoot = Path.Combine(Environment.CurrentDirectory, "platform-tools");
            AdbLocation = Path.Combine(AdbRoot, "adb.exe");
        }
        public async Task<bool> Pepair()
        {
            try
            {
                if (!HasBrige)
                {
                    HttpClient client = new HttpClient();
                    using var httpResponse = await client.GetAsync("https://dl.google.com/android/repository/platform-tools-latest-windows.zip");
                    using var stream = await httpResponse.Content.ReadAsStreamAsync();
                    var path = Path.Combine(Environment.CurrentDirectory, "platform-tools-latest-windows.zip");
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
                    return Zipper.UnZip(path, Environment.CurrentDirectory);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string Reload()
        {
            var processes = Process.GetProcessesByName("ADB.EXE");
            if (processes != null && processes.Length > 0)
            {
                foreach (var process in processes)
                    process.Kill();
            }
            ExcuteCommand("adb devices", out string message);
            return message;
        }
        public bool ExcuteCommand(string cmd, out string message)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.StandardInput.WriteLine($"cd \"{AdbRoot}\"");
                p.StandardInput.WriteLine($"{cmd}&exit");
                p.StandardInput.AutoFlush = true;
                message = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
    }
}