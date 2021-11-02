using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private async void WSAList_Load(object sender, EventArgs e)
        {
            await GetList();
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
                foreach (var url in urls)
                {
                    string message = string.Empty;
                    var path = Path.Combine(Environment.CurrentDirectory, url.Key);
                    PS.Excute($"Add-AppxPackage {path} -ForceApplicationShutdown", ref message);
                }
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
                MessageBox.Show("获取WSA环境包到本地失败，请重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            HideLoading();
        }
        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            await GetList();
        }
        private async Task GetList()
        {
            ShowLoading();
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
            HideLoading();
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}