using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WSATools.Libs;
using WSATools.Libs.Model;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WSATools.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public event CloseHandler Close;
        public event BooleanHandler Enable;
        public event BooleanHandler WSAStateHandler;
        public IAsyncRelayCommand CloseCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand RegisterCommand { get; }
        public IAsyncRelayCommand InstallVmCommand { get; }
        public IAsyncRelayCommand StartWSACommand { get; }
        public IAsyncRelayCommand InstallWSACommand { get; }
        public MainWindowViewModel()
        {
            WSAStateHandler += MainWindowViewModel_WSAStateHandler;
            CloseCommand = new AsyncRelayCommand(CloseAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            InstallVmCommand = new AsyncRelayCommand(InstallVmAsync);
            StartWSACommand = new AsyncRelayCommand(StartWSAAsync);
            InstallWSACommand = new AsyncRelayCommand(InstallWSAAsync);
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
            this.RemoveMenu();
            var path = Path.Combine(this.ProcessPath(), "WSATools.Background.exe");
            if (path.AddMenu(LangManager.Instance.Current))
                MessageBox.Show(FindChar("OperaSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(FindChar("OperaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
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
            RegistEnable = !string.IsNullOrEmpty(DB.Instance.GetData("menu"));
            DownloadManager.Instance.ProcessChange += Downloader_ProcessChange;
            RunOnUIThread(async () =>
            {
                LoadVisable = Visibility.Visible;
                LoadVisable = Visibility.Collapsed;
            });
        }
        private string processVal = "0.00";
        public string ProcessVal
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
        private bool registEnable;
        public bool RegistEnable
        {
            get => registEnable;
            set => SetProperty(ref registEnable, value);
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
        private Visibility showUpdate = Visibility.Collapsed;
        public Visibility ShowUpdate
        {
            get => showUpdate;
            set => SetProperty(ref showUpdate, value);
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
        private string hasNewVersion;
        public string HasNewVersion
        {
            get => hasNewVersion;
            set => SetProperty(ref hasNewVersion, value);
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
        private void Downloader_ProcessChange(string progressPercentage)
        {
            ProcessVal = progressPercentage;
        }
        public override void Dispose()
        {
            DownloadManager.Instance.ProcessChange -= Downloader_ProcessChange;
        }
    }
}