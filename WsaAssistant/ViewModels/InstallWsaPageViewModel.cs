using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WsaAssistant.Libs;
using System.Windows;
using System.Collections.ObjectModel;
using WsaAssistant.Libs.Model;
using System.Windows.Forms;
using System.IO;
using Downloader;
using WsaAssistant.Views;
using MessageBox = HandyControl.Controls.MessageBox;
using FolderBrowserDialog = WpfCore.FolderPicker.FolderBrowserDialog;

namespace WsaAssistant.ViewModels
{
    public sealed class InstallWsaPageViewModel : ViewModelBase
    {
        public IAsyncRelayCommand RreshCommand { get; }
        public IAsyncRelayCommand InstallCommand { get; }
        public IAsyncRelayCommand OfflineCommand { get; }
        public InstallWsaPageViewModel()
        {
            RreshCommand = new AsyncRelayCommand(RreshAsync);
            InstallCommand = new AsyncRelayCommand(InstallAsync);
            OfflineCommand = new AsyncRelayCommand(OfflineAsync);
            WSA.Instance.DownloadComplete += Instance_DownloadComplete;
            DownloadManager.Instance.ProcessChange += Downloader_ProcessChange;
        }
        private async void Instance_DownloadComplete(object sender, bool state)
        {
            if (!state)
            {
                if (MessageBoxResult.Yes == MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.YesNo, MessageBoxImage.Error))
                {
                    LogManager.Instance.LogInfo("下载WSA异常，重试中！");
                    await WSA.Instance.Retry(false);
                }
                else
                {
                    LogManager.Instance.LogInfo("下载WSA异常，退出！");
                    NavigateTo("WsaPage");
                }
            }
            else
            {
                LogManager.Instance.LogInfo("下载WSA完成，开始安装！");
                ExcuteCommand();
                NavigateTo("WsaPage");
            }
            InstallEnable = true;
            TimeoutEnable = true;
            HideLoading();
        }
        private ObservableCollection<ListItem> packages = new ObservableCollection<ListItem>();
        public ObservableCollection<ListItem> Packages
        {
            get => packages;
            set => SetProperty(ref packages, value);
        }
        private bool timeoutEnable = true;
        public bool TimeoutEnable
        {
            get => timeoutEnable;
            set => SetProperty(ref timeoutEnable, value);
        }
        private string processVal = "0.00";
        public string ProcessVal
        {
            get => processVal;
            set => SetProperty(ref processVal, value);
        }
        private bool installEnable = false;
        public bool InstallEnable
        {
            get => installEnable;
            set => SetProperty(ref installEnable, value);
        }
        private Task OfflineAsync()
        {
            RunOnUIThread(() =>
            {
                InstallEnable = false;
                var dialog = new FolderBrowserDialog { InitialFolder = this.ProcessPath() };
                if (dialog.ShowDialog() != DialogResult.Cancel)
                {
                    var directory = new DirectoryInfo(dialog.Folder);
                    List<string> files = new List<string>();
                    files.AddRange(directory.GetFiles("*.appx", SearchOption.AllDirectories).Select(x => x.FullName) ?? Array.Empty<string>());
                    files.AddRange(directory.GetFiles("*.msixbundle", SearchOption.AllDirectories).Select(x => x.FullName) ?? Array.Empty<string>());
                    LogManager.Instance.LogInfo("选择离线安装包：" + string.Join("#", files));
                    if (files.Count > 0)
                    {
                        try
                        {
                            StringBuilder shellBuilder = new StringBuilder();
                            foreach (var f in files)
                                shellBuilder.AppendLine($"Add-AppxPackage {f} -ForceApplicationShutdown");
                            Command.Instance.Shell("Set-ExecutionPolicy RemoteSigned", out _);
                            Command.Instance.Shell("Set-ExecutionPolicy -ExecutionPolicy Unrestricted", out _);
                            var file = "install.ps1";
                            if (File.Exists(file))
                                File.Delete(file);
                            File.WriteAllText(file, shellBuilder.ToString());
                            Command.Instance.Shell(@".\" + file, out string message);
                            LogManager.Instance.LogInfo("Install WSA Script Result:" + message);
                            LogManager.Instance.LogInfo("Install WSA Script Content:" + shellBuilder.ToString());
                            MessageBox.Show(FindChar("WsaSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                            HideLoading();
                        }
                        catch (Exception ex)
                        {
                            HideLoading();
                            LogManager.Instance.LogError("ExcuteCommand", ex);
                        }
                    }
                    else
                    {
                        HideLoading();
                        LogManager.Instance.LogInfo("Select Empty Folder,Breeak");
                        MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                InstallEnable = true;
            });
            return Task.CompletedTask;
        }
        private Task RreshAsync()
        {
            RunOnUIThread(async () =>
            {
                ShowLoading();
                await GetList();
                HideLoading();
            });
            return Task.CompletedTask;
        }
        private Task InstallAsync()
        {
            RunOnUIThread(async () =>
            {
                InstallEnable = false;
                ShowLoading();
                try
                {
                    TimeoutEnable = false;
                    if (await WSA.Instance.PepairAsync())
                    {
                        LogManager.Instance.LogInfo("Found Local Package to install.");
                        ExcuteCommand();
                        HideLoading();
                    }
                }
                catch (Exception ex)
                {
                    InstallEnable = true;
                    TimeoutEnable = true;
                    HideLoading();
                    LogManager.Instance.LogError("InstallAsync", ex);
                    MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            return Task.CompletedTask;
        }
        private void ExcuteCommand()
        {
            try
            {
                StringBuilder shellBuilder = new StringBuilder();
                foreach (Tuple<string, string, bool?, DownloadPackage> package in WSA.Instance.PackageList)
                    shellBuilder.AppendLine($"Add-AppxPackage {package.Item1} -ForceApplicationShutdown");
                Command.Instance.Shell("Set-ExecutionPolicy RemoteSigned", out _);
                Command.Instance.Shell("Set-ExecutionPolicy -ExecutionPolicy Unrestricted", out _);
                var file = "install.ps1";
                if (File.Exists(file))
                    File.Delete(file);
                File.WriteAllText(file, shellBuilder.ToString());
                var shellFile = Path.Combine(this.ProcessPath(), file);
                Command.Instance.Shell(@".\" + file, out string message);
                LogManager.Instance.LogInfo("Install WSA Script Result:" + message);
                LogManager.Instance.LogInfo("Install WSA Script Content:" + shellBuilder.ToString());
                File.Delete(file);
                MessageBox.Show(FindChar("WsaSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                HideLoading();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("ExcuteCommand", ex);
            }
        }
        public async void LoadAsync(object sender, EventArgs e)
        {
            Dispatcher = (sender as InstallWsaPage).Dispatcher;
            await GetList();
        }
        private void Downloader_ProcessChange(string progressPercentage)
        {
            ProcessVal = progressPercentage;
        }
        private async Task GetList()
        {
            InstallEnable = false;
            ShowLoading();
            try
            {
                if (Packages == null || Packages.Count == 0)
                {
                    var pairs = await WSA.Instance.GetFilePath();
                    if (pairs != null && pairs.Count > 0)
                    {
                        foreach (Tuple<string, string, bool?, DownloadPackage> pair in pairs)
                        {
                            var item = new ListItem(pair.Item1, pair.Item2);
                            Dispatcher.Invoke(() =>
                            {
                                Packages.Add(item);
                            });
                        }
                        InstallEnable = true;
                    }
                    else
                    {
                        MessageBox.Show(FindChar("WsaPackageFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetList", ex);
                MessageBox.Show(FindChar("WsaPackageFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            HideLoading();
        }
        public override void Dispose()
        {
            WSA.Instance.DownloadComplete -= Instance_DownloadComplete;
            DownloadManager.Instance.ProcessChange -= Downloader_ProcessChange;
        }
    }
}