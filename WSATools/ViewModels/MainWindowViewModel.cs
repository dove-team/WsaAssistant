using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WSATools.Libs;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WSATools.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public event CloseHandler Close;
        public event BooleanHandler Enable;
        public event BooleanHandler WSAStateHandler;
        public IAsyncRelayCommand CloseCommand { get; }
        public IAsyncRelayCommand SearchCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand UninstallCommand { get; }
        public IAsyncRelayCommand InstallVmCommand { get; }
        public IAsyncRelayCommand InstallApkCommand { get; }
        public IAsyncRelayCommand StartWSACommand { get; }
        public IAsyncRelayCommand InstallWSACommand { get; }
        public IAsyncRelayCommand DowngradeCommand { get; }
        public IAsyncRelayCommand RegisterCommand { get; }        
        public IAsyncRelayCommand UninstallApkCommand { get; }
        public MainWindowViewModel()
        {
            WSAStateHandler += MainWindowViewModel_WSAStateHandler;
               CloseCommand = new AsyncRelayCommand(CloseAsync);
            SearchCommand = new AsyncRelayCommand(SearchAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            StartWSACommand = new AsyncRelayCommand(StartWSAAsync);
            UninstallCommand = new AsyncRelayCommand(UninstallAsync);
            DowngradeCommand = new AsyncRelayCommand(DowngradeAsync);
            InstallVmCommand = new AsyncRelayCommand(InstallVmAsync);
            InstallApkCommand = new AsyncRelayCommand(InstallApkAsync);
            InstallWSACommand = new AsyncRelayCommand(InstallWSAAsync);
            UninstallApkCommand = new AsyncRelayCommand(UninstallApkAsync);
        }
        private void MainWindowViewModel_WSAStateHandler(object sender, bool state)
        {
            Dispatcher.Invoke(() =>
            {
                if (state)
                    StateBrush = new SolidColorBrush(Colors.Green);
                else
                    StateBrush = new SolidColorBrush(Colors.Red);
            });
        }
        private Task RegisterAsync()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "WSATools.Update.exe");
            if (path.AddMenu(LangManager.Instance.Current))
                MessageBox.Show(FindChar("OperaSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(FindChar("OperaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            return Task.CompletedTask;
        }
        private Task StartWSAAsync()
        {
            RunOnUIThread(async () =>
            {
                LoadVisable = Visibility.Visible;
                WSA.Instance.Start();
                await Task.Delay(5000);
                if (WSA.Instance.Running)
                {
                    WSARun = true;
                    WSARunState = FindChar("Running");
                    WSAStateHandler?.Invoke(this, true);
                    WSAStart = Visibility.Collapsed;
                    Adb.Instance.Reload();
                    await LinkWSA();
                }
                else
                {
                    WSARun = false;
                    WSARunState = FindChar("NotRunning");
                    WSAStateHandler?.Invoke(this, false);
                    WSAStart = Visibility.Visible;
                }
                LoadVisable = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
        private Task CloseAsync()
        {
            Application.Current.Shutdown();
            return Task.CompletedTask;
        }
        public void LoadAsync(object sender, EventArgs e)
        {
            Dispatcher = (sender as MainWindow).Dispatcher;
            VMState = FindChar("Checking");
            WSAState = FindChar("Checking");
            WSARunState = FindChar("Checking");
            StateBrush = new SolidColorBrush(Colors.Yellow);
            DownloadManager.Instance.ProcessChange += Downloader_ProcessChange;
            RunOnUIThread(async () =>
            {
                LoadVisable = Visibility.Visible;
                var result = WSA.Instance.State();
                if (result.VM)
                {
                    VMState = FindChar("Installed");
                    VMEnable = false;
                }
                else
                {
                    VMState = FindChar("NotInstall");
                    VMEnable = true;
                }
                if (result.WSA)
                {
                    WSAState = FindChar("Installed");
                    WSAEnable = false;
                    WSARemoveable = true;
                }
                else
                {
                    WSAState = FindChar("NotInstall");
                    WSAEnable = true;
                    WSARemoveable = false;
                }
                if (result.Run)
                {
                    WSARun = true;
                    WSAStateHandler?.Invoke(this, true);
                    WSARunState = FindChar("Running");
                    WSAStart = Visibility.Collapsed;
                    Adb.Instance.Reload();
                    await LinkWSA();
                }
                else
                {
                    WSARun = false;
                    WSAStateHandler?.Invoke(this, false);
                    WSARunState = FindChar("NotRunning");
                    WSAStart = Visibility.Visible;
                }
                LoadVisable = Visibility.Collapsed;
            });
        }
        private ObservableCollection<ListItem> packages = new ObservableCollection<ListItem>();
        public ObservableCollection<ListItem> Packages
        {
            get => packages;
            set => SetProperty(ref packages, value);
        }
        private string selectPackage;
        public string SelectPackage
        {
            get => selectPackage;
            set => SetProperty(ref selectPackage, value);
        }
        private double processVal = 0;
        public double ProcessVal
        {
            get => processVal;
            set => SetProperty(ref processVal, value);
        }
        private string vmState;
        public string VMState
        {
            get => vmState;
            private set => SetProperty(ref vmState, value);
        }
        private bool vmEnable = false;
        public bool VMEnable
        {
            get => vmEnable;
            private set => SetProperty(ref vmEnable, value);
        }
        private string wsaRunState;
        public string WSARunState
        {
            get => wsaRunState;
            set => SetProperty(ref wsaRunState, value);
        }
        private SolidColorBrush stateBrush;
        public SolidColorBrush StateBrush
        {
            get => stateBrush;
            set => SetProperty(ref stateBrush, value);
        }
        private Visibility wsaStart = Visibility.Collapsed;
        public Visibility WSAStart
        {
            get => wsaStart;
            set => SetProperty(ref wsaStart, value);
        }
        private bool wsaRun = false;
        public bool WSARun
        {
            get => wsaRun;
            set => SetProperty(ref wsaRun, value);
        }
        private string wsaState;
        public string WSAState
        {
            get => wsaState;
            private set => SetProperty(ref wsaState, value);
        }
        private bool wsaEnable = false;
        public bool WSAEnable
        {
            get => wsaEnable;
            private set => SetProperty(ref wsaEnable, value);
        }
        private bool wsaRemoveable = false;
        public bool WSARemoveable
        {
            get => wsaRemoveable;
            private set => SetProperty(ref wsaRemoveable, value);
        }
        private string searchKeywords;
        public string SearchKeywords
        {
            get => searchKeywords;
            set => SetProperty(ref searchKeywords, value);
        }
        private Task RefreshAsync()
        {
            RunOnUIThread(async () =>
            {
                LoadVisable = Visibility.Visible;
                await LinkWSA();
                LoadVisable = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
        private Task DowngradeAsync()
        {
            RunOnUIThread(() =>
           {
               LoadVisable = Visibility.Visible;
               OpenFileDialog openFileDialog = new OpenFileDialog
               {
                   FileName = string.Empty,
                   Filter = FindChar("ApkFile")
               };
               if (!string.IsNullOrEmpty(SelectPackage) && openFileDialog.ShowDialog() == true)
               {
                   if (Adb.Instance.Downgrade(openFileDialog.FileName))
                       MessageBox.Show(FindChar("DowngradeSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                   else
                       MessageBox.Show(FindChar("DowngradeFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
               }
               LoadVisable = Visibility.Collapsed;
           });
            return Task.CompletedTask;
        }
        private Task UninstallApkAsync()
        {
            RunOnUIThread(async () =>
            {
                LoadVisable = Visibility.Visible;
                if (!string.IsNullOrEmpty(SelectPackage))
                {
                    if (MessageBox.Show($"{FindChar("UninstallTips")}{SelectPackage}？", FindChar("Tips"), MessageBoxButton.YesNo, MessageBoxImage.Question)
                          == MessageBoxResult.Yes)
                    {
                        if (Adb.Instance.Remove(SelectPackage))
                        {
                            MessageBox.Show(FindChar("UninstallSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                            await LinkWSA();
                        }
                        else
                            MessageBox.Show(FindChar("UninstallFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                LoadVisable = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
        private Task InstallApkAsync()
        {
            RunOnUIThread(async () =>
            {
                LoadVisable = Visibility.Visible;
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    FileName = string.Empty,
                    Filter = FindChar("ApkFile")
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    if (Adb.Instance.Install(openFileDialog.FileName))
                    {
                        MessageBox.Show(FindChar("InstallSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                        await LinkWSA();
                    }
                    else
                        MessageBox.Show(FindChar("InstallFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                LoadVisable = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
        private Task SearchAsync()
        {
            RunOnUIThread(async () =>
            {
                LoadVisable = Visibility.Visible;
                await LinkWSA(SearchKeywords);
                LoadVisable = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
        private Task UninstallAsync()
        {
            RunOnUIThread(() =>
             {
                 LoadVisable = Visibility.Visible;
                 WSA.Instance.Clear();
                 if (MessageBox.Show(FindChar("RebootTips"), FindChar("Tips"), MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                     Command.Instance.Excute("shutdown -r -t 10", out _);
                 Application.Current.Shutdown();
                 LoadVisable = Visibility.Collapsed;
             });
            return Task.CompletedTask;
        }
        private async Task InstallWSAAsync()
        {
            await InitWSA();
        }
        private Task InstallVmAsync()
        {
            RunOnUIThread(async () =>
             {
                 LoadVisable = Visibility.Visible;
                 if (WSA.Instance.Init())
                 {
                     VMState = FindChar("Installed");
                     VMEnable = false;
                     WSARemoveable = true;
                     LoadVisable = Visibility.Collapsed;
                     await InitWSA();
                 }
                 else
                 {
                     VMState = FindChar("NotInstall");
                     VMEnable = true;
                     WSARemoveable = false;
                     LoadVisable = Visibility.Collapsed;
                     if (MessageBox.Show(FindChar("RebootTips"), FindChar("Tips"), MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                         Command.Instance.Excute("shutdown -r -t 10", out _);
                 }
             });
            return Task.CompletedTask;
        }
        private async Task LinkWSA(string condition = "")
        {
            LoadVisable = Visibility.Visible;
            Dispatcher.Invoke(() =>
            {
                Packages.Clear();
            });
            if (await Adb.Instance.Pepair())
            {
                if (string.IsNullOrEmpty(Adb.Instance.DeviceCode))
                {
                    WSARun = false;
                    MessageBox.Show(FindChar("DevlopTips"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var list = Adb.Instance.GetAll(condition);
                    foreach (var name in list)
                    {
                        var item = new ListItem(name);
                        Dispatcher.Invoke(() =>
                        {
                            Packages.Add(item);
                        });
                    }
                }
            }
            else
            {
                MessageBox.Show(FindChar("AdbFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadVisable = Visibility.Collapsed;
        }
        private async Task InitWSA()
        {
            if (!WSA.Instance.Pepair())
            {
                WSAState = FindChar("NotInstall");
                WSAEnable = true;
                WSAList list = new WSAList();
                Enable?.Invoke(this, false);
                if (list.ShowDialog() != null)
                {
                    Enable?.Invoke(this, true);
                    if (WSA.Instance.Pepair())
                    {
                        WSAState = FindChar("Installed");
                        WSAEnable = false;
                        Adb.Instance.Reload();
                        await LinkWSA();
                        MessageBox.Show(FindChar("WsaSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        WSAState = FindChar("NotInstall");
                        WSAEnable = true;
                        MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    Enable?.Invoke(this, true);
                    MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close?.Invoke(this, null);
                }
            }
            else
            {
                WSAState = FindChar("Installed");
                WSAEnable = false;
                Adb.Instance.Reload();
                await LinkWSA();
                MessageBox.Show(FindChar("WsaSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void Downloader_ProcessChange(long receiveSize, long totalSize)
        {
            ProcessVal = receiveSize / totalSize * 100;
        }
        public override void Dispose()
        {
            DownloadManager.Instance.ProcessChange -= Downloader_ProcessChange;
        }
    }
}