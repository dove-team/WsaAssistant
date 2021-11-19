using Downloader;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WSATools.Libs;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WSATools.ViewModels
{
    public sealed class WSAListViewModel : ViewModelBase
    {
        public event CloseHandler Close;
        public IAsyncRelayCommand CloseCommand { get; }
        public IAsyncRelayCommand RreshCommand { get; }
        public IAsyncRelayCommand InstallCommand { get; }
        public WSAListViewModel()
        {
            CloseCommand = new AsyncRelayCommand(CloseAsync);
            RreshCommand = new AsyncRelayCommand(RreshAsync);
            InstallCommand = new AsyncRelayCommand(InstallAsync);
            AppX.Instance.DownloadComplete += Instance_DownloadComplete;
        }
        private async void Instance_DownloadComplete(object sender, bool state)
        {
            if (!state)
            {
                if (MessageBoxResult.Yes == MessageBox.Show(FindChar("WsaDownloadFailed"), FindChar("Tips"),
                    MessageBoxButton.YesNo, MessageBoxImage.Error))
                    await AppX.Instance.Retry(false);
                else
                    Close?.Invoke(this, false);
            }
            else
            {
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
        private decimal processVal = 0;
        public decimal ProcessVal
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
                    MessageBox.Show(FindChar("WsaDownloadFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            return Task.CompletedTask;
        }
        private void ExcuteCommand()
        {
            try
            {
                StringBuilder shellBuilder = new StringBuilder();
                foreach (Tuple<string, string, bool> package in AppX.Instance.PackageList)
                    shellBuilder.AppendLine($"Add-AppxPackage {package.Item2} -ForceApplicationShutdown");
                Command.Instance.Shell("Set-ExecutionPolicy RemoteSigned", out _);
                Command.Instance.Shell("Set-ExecutionPolicy -ExecutionPolicy Unrestricted", out _);
                var file = "install.ps1";
                if (File.Exists(file))
                    File.Delete(file);
                File.WriteAllText(file, shellBuilder.ToString());
                var shellFile = Path.Combine(this.ProcessPath(), file);
                Command.Instance.Shell(shellFile, out string message);
                LogManager.Instance.LogInfo("Install WSA Script:" + message);
                File.Delete(shellFile);
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
        private void Downloader_ProcessChange(long receiveSize, long totalSize)
        {
            ProcessVal = Math.Round((decimal)receiveSize / totalSize * 100, 2);
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
                        MessageBox.Show(FindChar("WsaDownloadFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetList", ex);
                MessageBox.Show(FindChar("WsaDownloadFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadVisable = Visibility.Collapsed;
        }
        public override void Dispose()
        {
            DownloadManager.Instance.ProcessChange -= Downloader_ProcessChange;
        }
    }
}