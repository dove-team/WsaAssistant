using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WSATools.Libs;

namespace WSATools
{
    public partial class WSAList : Form
    {
        private Dictionary<string, string> List { get; set; }
        public WSAList()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private void Downloader_ProcessChange(int receiveSize, long totalSize)
        {
            label2.Text = $"下载进度：{receiveSize / totalSize * 100}%";
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
        private void WSAList_Load(object sender, EventArgs e)
        {
            DialogResult = DialogResult.None;
            Downloader.ProcessChange += Downloader_ProcessChange;
            GetList();
        }
        private async void buttonInstall_Click(object sender, EventArgs e)
        {
            ShowLoading();
            textBox1.ReadOnly = true;
            Dictionary<string, string> urls = new Dictionary<string, string>();
            foreach (var item in checkedListBox.CheckedItems)
            {
                var url = List.FirstOrDefault(x => x.Key == item.ToString());
                urls.Add(url.Key, url.Value);
            }
            label2.Visible = true;
            var timeout = int.Parse(textBox1.Text);
            if (await AppX.PepairAsync(urls, timeout))
            {
                label2.Visible = false;
                StringBuilder shellBuilder = new StringBuilder();
                foreach (var url in urls)
                {
                    var path = Path.Combine(Environment.CurrentDirectory, url.Key);
                    shellBuilder.AppendLine($"Add-AppxPackage {path} -ForceApplicationShutdown");
                }
                ExcuteCommand(shellBuilder);
                DialogResult = DialogResult.OK;
            }
            else
            {
                label2.Visible = false;
                DialogResult = DialogResult.Cancel;
                MessageBox.Show("获取WSA环境包到本地失败，请重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            textBox1.ReadOnly = false;
            HideLoading();
        }
        private static void ExcuteCommand(StringBuilder shellBuilder)
        {
            Command.Instance.Shell("Set-ExecutionPolicy RemoteSigned", out _);
            Command.Instance.Shell("Set-ExecutionPolicy -ExecutionPolicy Unrestricted", out _);
            var file = "install.ps1";
            if (File.Exists(file))
                File.Delete(file);
            File.WriteAllText(file, shellBuilder.ToString());
            var shellFile = Path.Combine(Environment.CurrentDirectory, file);
            Command.Instance.Shell(shellFile, out _);
            File.Delete(shellFile);
        }
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            GetList();
        }
        private void GetList()
        {
            ShowLoading();
            Task.Factory.StartNew(async () =>
            {
                if (List == null || List.Count == 0)
                    List = await AppX.GetFilePath();
                if (List != null && List.Count > 0)
                {
                    var names = List.Select(x => x.Key).ToArray();
                    checkedListBox.Items.AddRange(names);
                }
                else
                {
                    MessageBox.Show("获取WSA环境包失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            HideLoading();
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void WSAList_FormClosing(object sender, FormClosingEventArgs e)
        {
            Downloader.ProcessChange -= Downloader_ProcessChange;
            e.Cancel = DialogResult == DialogResult.None;
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}