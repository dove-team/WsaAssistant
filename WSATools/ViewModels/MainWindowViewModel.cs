using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using WSATools.Libs;

namespace WSATools.ViewModels
{
    public sealed class MainWindowViewModel : ObservableObject, IDisposable
    {
        public event EventHandler Close;
        public IAsyncRelayCommand CloseCommand { get; }
        public IAsyncRelayCommand SearchCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand UninstallCommand { get; }
        public IAsyncRelayCommand InstallVmCommand { get; }
        public IAsyncRelayCommand InstallApkCommand { get; }
        public IAsyncRelayCommand InstallToolCommand { get; }
        public IAsyncRelayCommand InstallWSACommand { get; }
        public IAsyncRelayCommand DowngradeCommand { get; }
        public IAsyncRelayCommand UninstallApkCommand { get; }
        public MainWindowViewModel()
        {
            CloseCommand = new AsyncRelayCommand(CloseAsync);
            SearchCommand = new AsyncRelayCommand(SearchAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            UninstallCommand = new AsyncRelayCommand(UninstallAsync);
            InstallVmCommand = new AsyncRelayCommand(InstallVmAsync);
            InstallToolCommand = new AsyncRelayCommand(InstallToolAsync);
            InstallApkCommand = new AsyncRelayCommand(InstallApkAsync);
            InstallWSACommand = new AsyncRelayCommand(InstallWSAAsync);
            DowngradeCommand = new AsyncRelayCommand(DowngradeAsync);
            UninstallApkCommand = new AsyncRelayCommand(UninstallApkAsync);
        }
        private Task CloseAsync()
        {
            Application.Current.Shutdown();
            return Task.CompletedTask;
        }
        public void LoadAsync(object sender, EventArgs e)
        {
            Downloader.ProcessChange += Downloader_ProcessChange;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                LoadVisable = Visibility.Visible;
                var result = WSA.State();
                if (result.VM)
                {
                    VMState = "已安装";
                    VMEnable = false;
                }
                else
                {
                    VMState = "未安装";
                    VMEnable = true;
                }
                if (result.WSA)
                {
                    WSAState = "已安装";
                    WSAEnable = false;
                }
                else
                {
                    WSAState = "未安装";
                    WSAEnable = true;
                }
                if (result.VM && result.WSA)
                {
                    Adb.Instance.Reload();
                    await LinkWSA();
                    WSARemoveable = true;
                }
                else
                {
                    WSARemoveable = false;
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
        private string vmState = "检测中...";
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
        private Visibility loadVisable = Visibility.Collapsed;
        public Visibility LoadVisable
        {
            get => loadVisable;
            set => SetProperty(ref loadVisable, value);
        }
        private bool toolEnable = true;
        public bool ToolEnable
        {
            get => toolEnable;
            set => SetProperty(ref toolEnable, value);
        }
        private string wsaState = "检测中...";
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
        private async Task RefreshAsync()
        {
            LoadVisable = Visibility.Visible;
            await LinkWSA();
            LoadVisable = Visibility.Collapsed;
        }
        private Task DowngradeAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                LoadVisable = Visibility.Visible;
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    FileName = string.Empty,
                    Filter = "APK文件|*.apk"
                };
                if (!string.IsNullOrEmpty(SelectPackage) && openFileDialog.ShowDialog() == true)
                {
                    if (Adb.Instance.Downgrade(openFileDialog.FileName))
                        MessageBox.Show("降级安装成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("降级安装失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                LoadVisable = Visibility.Collapsed;
            });
        }
        private async Task UninstallApkAsync()
        {
            LoadVisable = Visibility.Visible;
            if (!string.IsNullOrEmpty(SelectPackage))
            {
                if (MessageBox.Show($"确定卸载{SelectPackage}？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question)
                      == MessageBoxResult.Yes)
                {
                    if (Adb.Instance.Remove(SelectPackage))
                    {
                        MessageBox.Show("卸载成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LinkWSA();
                    }
                    else
                        MessageBox.Show("卸载失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            LoadVisable = Visibility.Collapsed;
        }
        private async Task InstallApkAsync()
        {
            LoadVisable = Visibility.Visible;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                FileName = string.Empty,
                Filter = "APK文件|*.apk"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                if (Adb.Instance.Install(openFileDialog.FileName))
                {
                    MessageBox.Show("安装成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LinkWSA();
                }
                else
                    MessageBox.Show("安装失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadVisable = Visibility.Collapsed;
        }
        private async Task SearchAsync()
        {
            LoadVisable = Visibility.Visible;
            await LinkWSA(SearchKeywords);
            LoadVisable = Visibility.Collapsed;
        }
        private Task InstallToolAsync()
        {
            return Task.Factory.StartNew(async () =>
             {
                 LoadVisable = Visibility.Visible;
                 ToolEnable = false;
                 string path = Path.Combine(Environment.CurrentDirectory, "APKInstaller.zip"),
                 targetDirectory = Path.Combine(Environment.CurrentDirectory, "APKInstaller");
                 if (await Downloader.Create("https://github.com/michael-eddy/WSATools/releases/download/v1.0.3/APKInstaller.zip", path, 60)
                 && Zipper.UnZip(path, targetDirectory))
                 {
                     Command.Instance.Shell(Path.Combine(targetDirectory, "Install.ps1"), out _);
                     Command.Instance.Shell("Get-AppxPackage|findstr AndroidAppInstaller", out string message);
                     var msg = !string.IsNullOrEmpty(message) ? "安装成功！" : "安装失败，请稍后重试！";
                     Directory.Delete(targetDirectory, true);
                     MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                 }
                 else
                 {
                     MessageBox.Show("初始化APKInstaller安装包失败，请稍后重试！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                 }
                 ToolEnable = true;
                 LoadVisable = Visibility.Collapsed;
             });
        }
        private Task UninstallAsync()
        {
            return Task.Factory.StartNew(() =>
             {
                 LoadVisable = Visibility.Visible;
                 WSA.Clear();
                 if (MessageBox.Show("需要重启系统以完成操作！(确定后10s内重启系统，请保存好你的数据后进行重启！！！)", "提示",
                     MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                     Command.Instance.Excute("shutdown -r -t 10", out _);
                 LoadVisable = Visibility.Collapsed;
             });
        }
        private async Task InstallWSAAsync()
        {
            LoadVisable = Visibility.Visible;
            await InitWSA();
            LoadVisable = Visibility.Collapsed;
        }
        private Task InstallVmAsync()
        {
            return Application.Current.Dispatcher.Invoke(async () =>
             {
                 LoadVisable = Visibility.Visible;
                 var idx = WSA.Init();
                 if (idx == 1)
                 {
                     VMState = "已安装";
                     VMEnable = false;
                     WSARemoveable = true;
                     await InitWSA();
                 }
                 else
                 {
                     VMState = "未安装";
                     VMEnable = true;
                     WSARemoveable = false;
                 }
                 LoadVisable = Visibility.Collapsed;
             });
        }
        private async Task LinkWSA(string condition = "")
        {
            LoadVisable = Visibility.Visible;
            Packages.Clear();
            if (await Adb.Instance.Pepair())
            {
                var list = Adb.Instance.GetAll(condition);
                foreach (var name in list)
                {
                    var item = new ListItem(name);
                    Packages.Add(item);
                }
            }
            else
            {
                MessageBox.Show("初始化ADB环境失败，请稍后重试！或者直接使用APKInstall进行管理！", "提示",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadVisable = Visibility.Collapsed;
        }
        private async Task InitWSA()
        {
            LoadVisable = Visibility.Visible;
            if (!WSA.Pepair())
            {
                WSAState = "未安装";
                WSAEnable = true;
                WSAList list = new WSAList();
                if (list.ShowDialog() != null)
                {
                    var result = WSA.Pepair();
                    if (result)
                    {
                        WSAState = "已安装";
                        WSAEnable = false;
                        Adb.Instance.Reload();
                        await LinkWSA();
                        MessageBox.Show("恭喜你，看起来WSA环境已经准备好了！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        WSAState = "未安装";
                        WSAEnable = true;
                        MessageBox.Show("很无语，看起来WSA环境安装失败了，请稍后试试！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("未安装WSA无法进行操作，程序即将退出！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close?.Invoke(this, null);
                }
            }
            else
            {
                WSAState = "已安装";
                WSAEnable = false;
                Adb.Instance.Reload();
                await LinkWSA();
                MessageBox.Show("恭喜你，看起来现在的WSA环境很好！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadVisable = Visibility.Collapsed;
        }
        private void Downloader_ProcessChange(int receiveSize, long totalSize)
        {
            ProcessVal = receiveSize / totalSize * 100;
        }
        public void Dispose()
        {
            Downloader.ProcessChange -= Downloader_ProcessChange;
        }
    }
}