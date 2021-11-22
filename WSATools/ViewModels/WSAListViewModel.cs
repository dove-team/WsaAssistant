using Downloader;
using System;
using System.IO;
using System.Text;
using WSATools.Libs;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using MessageBox = HandyControl.Controls.MessageBox;
using FolderBrowserDialog = WpfCore.FolderPicker.FolderBrowserDialog;

namespace WSATools.ViewModels
{
    public sealed class WSAListViewModel : ViewModelBase
    {
        public event CloseHandler Close;
        public IAsyncRelayCommand CloseCommand { get; }
        public IAsyncRelayCommand RreshCommand { get; }
        public IAsyncRelayCommand InstallCommand { get; }
        public IAsyncRelayCommand OfflineCommand { get; }
        public WSAListViewModel()
        {
            CloseCommand = new AsyncRelayCommand(CloseAsync);
            RreshCommand = new AsyncRelayCommand(RreshAsync);
            InstallCommand = new AsyncRelayCommand(InstallAsync);
            OfflineCommand = new AsyncRelayCommand(OfflineAsync);
            AppX.Instance.DownloadComplete += Instance_DownloadComplete;
        }
        private async void Instance_DownloadComplete(object sender, bool state)
        {
            if (!state)
            {
                if (MessageBoxResult.Yes == MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.YesNo, MessageBoxImage.Error))
                {
                    LogManager.Instance.LogInfo("下载WSA异常，重试中！");
                    await AppX.Instance.Retry(false);
                }
                else
                {
                    LogManager.Instance.LogInfo("下载WSA异常，退出！");
                    Close?.Invoke(this, false);
                }
            }
            else
            {
                LogManager.Instance.LogInfo("下载WSA完成，开始安装！");
                ExcuteCommand();
                Close?.Invoke(this, true);
            }
            InstallEnable = true;
            TimeoutEnable = true;
            LoadVisable = Visibility.Collapsed;
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
        private bool installEnable = true;
        public bool InstallEnable
        {
            get => installEnable;
            set => SetProperty(ref installEnable, value);
        }
        private Task OfflineAsync()
        {
            RunOnUIThread(() =>
            {
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
                            LoadVisable = Visibility.Collapsed;
                        }
                        catch (Exception ex)
                        {
                            LoadVisable = Visibility.Collapsed;
                            LogManager.Instance.LogError("ExcuteCommand", ex);
                        }
                    }
                    else
                    {
                        LoadVisable = Visibility.Collapsed;
                        LogManager.Instance.LogInfo("Select Empty Folder,Breeak");
                        MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
            return Task.CompletedTask;
        }
        private Task RreshAsync()
        {
            RunOnUIThread(async () =>
            {
                LoadVisable = Visibility.Visible;
                await GetList();
                LoadVisable = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
        private Task CloseAsync()
        {
            LogManager.Instance.LogInfo("User Cancel.");
            LoadVisable = Visibility.Collapsed;
            Close?.Invoke(this, false);
            return Task.CompletedTask;
        }
        private Task InstallAsync()
        {
            RunOnUIThread(async () =>
            {
                InstallEnable = false;
                LoadVisable = Visibility.Visible;
                try
                {
                    TimeoutEnable = false;
                    if (await AppX.Instance.PepairAsync())
                    {
                        LogManager.Instance.LogInfo("Found Local Package to install.");
                        ExcuteCommand();
                        LoadVisable = Visibility.Collapsed;
                    }
                }
                catch (Exception ex)
                {
                    InstallEnable = true;
                    TimeoutEnable = true;
                    LoadVisable = Visibility.Collapsed;
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
                foreach (Tuple<string, string, bool?, DownloadPackage> package in AppX.Instance.PackageList)
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
                MessageBox.Show(FindChar("WsaSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVisable = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("ExcuteCommand", ex);
            }
        }
        public async void LoadAsync(object sender, EventArgs e)
        {
            Dispatcher = (sender as WSAList).Dispatcher;
            DownloadManager.Instance.ProcessChange += Downloader_ProcessChange;
            await GetList();
        }
        private void Downloader_ProcessChange(string progressPercentage)
        {
            ProcessVal = progressPercentage;
        }
        private async Task GetList()
        {
            LoadVisable = Visibility.Visible;
            try
            {
                if (Packages == null || Packages.Count == 0)
                {
                    var pairs = await AppX.Instance.GetFilePath();
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
            LoadVisable = Visibility.Collapsed;
        }
        public override void Dispose()
        {
            DownloadManager.Instance.ProcessChange -= Downloader_ProcessChange;
        }
    }
}