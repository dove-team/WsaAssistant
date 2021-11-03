using System;
using System.Threading;
using System.Windows.Forms;

namespace WSATools
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using Mutex mutex = new Mutex(true, Application.ProductName, out bool ret);
            if (ret)
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("有一个和本程序相同的应用程序已经在运行，请不要同时运行多个本程序。\n\n这个程序即将退出。",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }
    }
}