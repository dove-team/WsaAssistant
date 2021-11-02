using System.Diagnostics;

namespace WSATools.Libs
{
    public sealed class PS
    {
        public static void Excute(string cmd, ref string message)
        {
            Process p = new Process();
            p.StartInfo.FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.StandardInput.WriteLine($"{cmd}&exit");
            p.StandardInput.AutoFlush = true;
            message = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
        }
    }
}