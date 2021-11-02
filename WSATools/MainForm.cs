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
            if (WSA.Init())
            {
                if (!WSA.Pepair())
                {
                    WSAList list = new WSAList();
                    if (list.ShowDialog(this) == DialogResult.OK)
                    {

                    }
                    else
                    {
                        MessageBox.Show("提示", "未安装WSA无法进行操作，程序即将退出！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                    }
                }
            }
            HideLoading();
        }
    }
}