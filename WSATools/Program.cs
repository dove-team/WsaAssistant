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
                MessageBox.Show("��һ���ͱ�������ͬ��Ӧ�ó����Ѿ������У��벻Ҫͬʱ���ж��������\n\n������򼴽��˳���",
                    "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }
    }
}