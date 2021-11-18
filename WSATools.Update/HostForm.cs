using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WSATools.Libs;
using WSATools.Libs.Model;
using Timer = System.Windows.Forms.Timer;

namespace WSATools.Update
{
    public partial class HostForm : Form
    {
        private Timer Timer { get; }
        public HostForm()
        {
            InitializeComponent();
            Timer = new Timer();
            Timer.Interval = 2000;
            Timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Timer.Stop();
            Adb.Instance.Install();
            Process.GetCurrentProcess().Kill();
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MessageHelper.WM_COPYDATA:
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                    switch (cds.lpData)
                    {
                        case "Upgrade":
                            {
                                Thread.Sleep(2000);
                                if (!string.IsNullOrEmpty(Program.UpgradeFile))
                                {
                                    string title = CultureInfo.CurrentCulture.Name.Contains("zh", StringComparison.CurrentCultureIgnoreCase)
                                        ? "WSATools有新版本，是否进行更新？" : "WSATools Has new-version，upgrade now？";
                                    if (MessageBox.Show(Program.UpdateMessage, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        Process.Start(Program.UpgradeFile);
                                }
                                Application.Exit();
                                break;
                            }
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        private void HostForm_Load(object sender, EventArgs e)
        {
            Timer.Start();
        }
    }
}