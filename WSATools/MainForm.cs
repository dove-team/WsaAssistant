using System;
using System.Windows.Forms;
using WSATools.Libs;

namespace WSATools
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void ShowLoading()
        {
            panelLoading.Visible = true;
            panelLoading.BringToFront();
        }
        private void HideLoading()
        {
            panelLoading.Visible = false;
            panelLoading.SendToBack();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowLoading();
            var idx = WSA.Init();
            if (idx == 1)
            {
                label4.Text = "已安装";
                if (!WSA.Pepair())
                {
                    label5.Text = "未安装";
                    WSAList list = new WSAList();
                    if (list.ShowDialog(this) == DialogResult.OK)
                    {

                    }
                    else
                    {
                        MessageBox.Show("未安装WSA无法进行操作，程序即将退出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                    }
                }
                else
                    label5.Text = "已安装";
            }
            else if (idx == -1)
                Application.Exit();
            else
                label4.Text = "未安装";
            HideLoading();
        }
    }
}