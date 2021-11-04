﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WSATools.Libs;

namespace WSATools
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
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
            Task.Factory.StartNew(InitVM);
            HideLoading();
        }
        private void InitVM()
        {
            var idx = WSA.Init();
            if (idx == 1)
            {
                label4.Text = "已安装";
                button1.Enabled = false;
                InitWSA();
            }
            else if (idx == -1)
                Application.Exit();
            else
            {
                label4.Text = "未安装";
                button1.Enabled = true;
            }
        }
        private void InitWSA()
        {
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
                        LoadApks();
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
        private void LoadApks()
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            ShowLoading();
            Task.Factory.StartNew(InitVM);
            HideLoading();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ShowLoading();
            Task.Factory.StartNew(InitWSA);
            HideLoading();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                string path = Path.Combine(Environment.CurrentDirectory, "APKInstaller.zip"),
                targetDirectory = Path.Combine(Environment.CurrentDirectory, "APKInstaller");
                if (await Downloader.Create("https://github.com/michael-eddy/WSATools/releases/download/v1.0.0/APKInstaller.zip", path)
                && Zipper.UnZip(path, targetDirectory))
                {
                    Command.Instance.Shell(Path.Combine(targetDirectory, "Install.ps1"), out _);
                    Command.Instance.Shell("Get-AppxPackage|findstr AndroidAppInstaller", out string message);
                    var msg = !string.IsNullOrEmpty(message) ? "安装成功！" : "安装失败，请稍后重试！";
                    Directory.Delete(targetDirectory, true);
                    MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("初始化APKInstaller安装包失败，请稍后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
        private void button4_Click(object sender, EventArgs e)
        {

        }
        private void button5_Click(object sender, EventArgs e)
        {

        }
        private void button6_Click(object sender, EventArgs e)
        {

        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否清除下载的文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)== DialogResult.Yes)
                Downloader.Clear();
        }
    }
}