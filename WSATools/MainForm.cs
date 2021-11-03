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
                button1.Enabled = false;
                if (!WSA.Pepair())
                {
                    label5.Text = "未安装";
                    button2.Enabled = true;
                    WSAList list = new WSAList();
                    if (list.ShowDialog(this) == DialogResult.OK)
                    {
                        var result = WSA.Pepair();
                        if (result)
                        {
                            label5.Text = "已安装";
                            button2.Enabled = false;
                            MessageBox.Show("恭喜你，看起来WSA环境已经准备好了！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            label5.Text = "未安装";
                            button2.Enabled = true;
                            MessageBox.Show("很无语，看起来WSA环境安装失败了，请稍后试试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("未安装WSA无法进行操作，程序即将退出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                    }
                }
                else
                {
                    label5.Text = "已安装";
                    button2.Enabled = false;
                    MessageBox.Show("恭喜你，看起来现在的WSA环境很好！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (idx == -1)
                Application.Exit();
            else
            {
                label4.Text = "未安装";
                button1.Enabled = true;
            }
            HideLoading();
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}