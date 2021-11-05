using System;
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
            Downloader.ProcessChange += Downloader_ProcessChange;
            Task.Factory.StartNew(() =>
            {
                ShowLoading();
                var result = WSA.State();
                buttonVM.Enabled = !result.VM;
                labelVM.Text = result.VM ? "已安装" : "未安装";
                buttonWSA.Enabled = !result.WSA;
                labelWSA.Text = result.WSA ? "已安装" : "未安装";
                if (result.VM && result.WSA)
                    buttonRemove.Enabled = true;
                HideLoading();
            });
        }
        private void Downloader_ProcessChange(int receiveSize, long totalSize)
        {
            labelProgress.Text = $"下载进度：{receiveSize / totalSize * 100}%";
        }
        private async Task InitWSA()
        {
            if (!WSA.Pepair())
            {
                labelWSA.Text = "未安装";
                buttonWSA.Enabled = true;
                WSAList list = new WSAList();
                if (list.ShowDialog(this) == DialogResult.OK)
                {
                    var result = WSA.Pepair();
                    if (result)
                    {
                        labelWSA.Text = "已安装";
                        buttonWSA.Enabled = false;
                        await LinkWSA();
                        MessageBox.Show("恭喜你，看起来WSA环境已经准备好了！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        labelWSA.Text = "未安装";
                        buttonWSA.Enabled = true;
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
                labelWSA.Text = "已安装";
                buttonWSA.Enabled = false;
                await LinkWSA();
                MessageBox.Show("恭喜你，看起来现在的WSA环境很好！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void buttonVM_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                ShowLoading();
                var idx = WSA.Init();
                if (idx == 1)
                {
                    labelVM.Text = "已安装";
                    buttonVM.Enabled = false;
                    buttonRemove.Enabled = true;
                    await InitWSA();
                }
                else
                {
                    labelVM.Text = "未安装";
                    buttonVM.Enabled = true;
                    buttonRemove.Enabled = false;
                }
                HideLoading();
            });
        }
        private void buttonWSA_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                ShowLoading();
                await InitWSA();
                HideLoading();
            });
        }
        private void buttonApkInstall_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                buttonApkInstall.Enabled = false;
                string path = Path.Combine(Environment.CurrentDirectory, "APKInstaller.zip"),
                targetDirectory = Path.Combine(Environment.CurrentDirectory, "APKInstaller");
                labelProgress.Visible = true;
                if (await Downloader.Create("https://github.com/michael-eddy/WSATools/releases/download/v1.0.3/APKInstaller.zip", path, 60)
                && Zipper.UnZip(path, targetDirectory))
                {
                    labelProgress.Visible = false;
                    Command.Instance.Shell(Path.Combine(targetDirectory, "Install.ps1"), out _);
                    Command.Instance.Shell("Get-AppxPackage|findstr AndroidAppInstaller", out string message);
                    var msg = !string.IsNullOrEmpty(message) ? "安装成功！" : "安装失败，请稍后重试！";
                    Directory.Delete(targetDirectory, true);
                    MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    labelProgress.Visible = false;
                    MessageBox.Show("初始化APKInstaller安装包失败，请稍后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                buttonApkInstall.Enabled = true;
            });
        }
        private async Task LinkWSA(string condition = "")
        {
            if (await Adb.Instance.Pepair())
            {
                Adb.Instance.Reload();
                var list = Adb.Instance.GetAll(condition);
                listView1.BeginUpdate();
                listView1.Items.Clear();
                foreach (var item in list)
                    listView1.Items.Add(item);
                listView1.EndUpdate();
                HideLoading();
            }
            else
            {
                MessageBox.Show("初始化ADB环境失败，请稍后重试！或者直接使用APKInstall进行管理！", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                HideLoading();
            }
        }
        private void buttonInstall_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (Adb.Instance.Install(openFileDialog1.FileName))
                {
                    MessageBox.Show("安装成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Task.Factory.StartNew(async () =>
                    {
                        ShowLoading();
                        await LinkWSA();
                        HideLoading();
                    });
                }
                else
                    MessageBox.Show("安装失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonUninstall_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var packageName = listView1.SelectedItems[0].ToString();
                if (Adb.Instance.Remove(packageName))
                {
                    MessageBox.Show("卸载成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Task.Factory.StartNew(async () =>
                    {
                        ShowLoading();
                        await LinkWSA();
                        HideLoading();
                    });
                }
                else
                    MessageBox.Show("卸载失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonDowngrade_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 && openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (Adb.Instance.Downgrade(openFileDialog1.FileName))
                    MessageBox.Show("降级安装成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("降级安装失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                ShowLoading();
                await LinkWSA();
                HideLoading();
            });
        }
        private void buttonQuery_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                ShowLoading();
                await LinkWSA(textBoxCondition.Text);
                HideLoading();
            });
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Downloader.ProcessChange -= Downloader_ProcessChange;
            if (MessageBox.Show("是否清除下载的文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Downloader.Clear();
        }
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ShowLoading();
                WSA.Clear();
                if (MessageBox.Show("需要重启系统以完成操作！(确定后10s内重启系统，请保存好你的数据后进行重启！！！)", "提示",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    Command.Instance.Excute("shutdown -r -t 10", out _);
                HideLoading();
            });
        }
    }
}