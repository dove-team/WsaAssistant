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
            GetList();
        }
        private async void buttonInstall_Click(object sender, EventArgs e)
        {
            ShowLoading();
            Dictionary<string, string> urls = new Dictionary<string, string>();
            foreach (var item in checkedListBox.CheckedItems)
            {
                var url = List.FirstOrDefault(x => x.Key == item.ToString());
                urls.Add(url.Key, url.Value);
            }
            if (await AppX.PepairAsync(urls))
            {
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
                DialogResult = DialogResult.Cancel;
                MessageBox.Show("获取WSA环境包到本地失败，请重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            Command.Instance.Shell(Path.Combine(Environment.CurrentDirectory, file), out _);
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
            e.Cancel = DialogResult == DialogResult.None;
        }
    }
}