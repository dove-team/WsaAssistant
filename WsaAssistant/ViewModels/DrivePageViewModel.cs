using Downloader;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.IO;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WsaAssistant.Libs;
using WsaAssistant.Libs.Model;
using WsaAssistant.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WsaAssistant.ViewModels
{
    public sealed class DrivePageViewModel : ViewModelBase
    {
        private Visibility processVisable = Visibility.Collapsed;
        public Visibility ProcessVisable
        {
            get => processVisable;
            set => SetProperty(ref processVisable, value);
        }
        private string processVal = "已下载：0%";
        public string ProcessVal
        {
            get => processVal;
            set => SetProperty(ref processVal, value);
        }
        private bool openGLEnable = false;
        public bool OpenGLEnable
        {
            get => openGLEnable;
            set => SetProperty(ref openGLEnable, value);
        }
        private bool amdEnable = false;
        public bool AmdEnable
        {
            get => amdEnable;
            set => SetProperty(ref amdEnable, value);
        }
        private bool nvidiaEnable = false;
        public bool NvidiaEnable
        {
            get => nvidiaEnable;
            set => SetProperty(ref nvidiaEnable, value);
        }
        private bool intelEnable = false;
        public bool IntelEnable
        {
            get => intelEnable;
            set => SetProperty(ref intelEnable, value);
        }
        private GPUType GPU { get; set; }
        public IAsyncRelayCommand InstallIntelCommand { get; }
        public IAsyncRelayCommand InstallAmdCommand { get; }
        public IAsyncRelayCommand InstallNvidiaCommand { get; }
        public IAsyncRelayCommand InstallOpenGLCommand { get; }
        public DrivePageViewModel()
        {
            InstallIntelCommand = new AsyncRelayCommand(InstallIntelAsync);
            InstallAmdCommand = new AsyncRelayCommand(InstallAmdAsync);
            InstallNvidiaCommand = new AsyncRelayCommand(InstallNvidiaAsync);
            InstallOpenGLCommand = new AsyncRelayCommand(InstallOpenGLAsync);
            Drives.Instance.DownloadComplete += Instance_DownloadComplete;
            DownloadManager.Instance.ProcessChange += Downloader_ProcessChange;
        }
        private void Downloader_ProcessChange(string progressPercentage)
        {
            ProcessVal = $"已下载：{progressPercentage} %" ;
        }
        private async void Instance_DownloadComplete(object sender, bool state)
        {
            if (sender is string)
            {
                switch (GPU)
                {
                    case GPUType.AMD:
                        AmdEnable = true;
                        break;
                    case GPUType.Intel:
                        IntelEnable = true;
                        break;
                    case GPUType.Nvidia:
                        NvidiaEnable = true;
                        break;
                }
                if (!state)
                {
                    ProcessVisable = Visibility.Collapsed;
                    LogManager.Instance.LogInfo("下载OpenGL异常，退出！");
                    string message = string.Format(FindChar("DownloadDriveFailed"), Drives.Instance.GetLink(GPU));
                    MessageBox.Show(message, FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (!state)
                {
                    if (MessageBoxResult.Yes == MessageBox.Show(FindChar("OpenGLDFailed"), FindChar("Tips"),
                        MessageBoxButton.YesNo, MessageBoxImage.Error))
                    {
                        LogManager.Instance.LogInfo("下载OpenGL异常，重试中！");
                        await Drives.Instance.Retry(true);
                    }
                    else
                    {
                        OpenGLEnable = true;
                        ProcessVisable = Visibility.Collapsed;
                        LogManager.Instance.LogInfo("下载OpenGL异常，退出！");
                    }
                }
                else
                {
                    LogManager.Instance.LogInfo("下载OpenGL完成，开始安装！");
                    ExcuteCommand();
                    ProcessVisable = Visibility.Collapsed;
                }
            }
            HideLoading();
        }
        private void ExcuteCommand()
        {
            try
            {
                StringBuilder shellBuilder = new StringBuilder();
                foreach (Tuple<string, Uri, bool?, DownloadPackage> package in Drives.Instance.PackageList)
                    shellBuilder.AppendLine($"Add-AppxPackage {package.Item1} -ForceApplicationShutdown");
                Command.Instance.Shell("Set-ExecutionPolicy RemoteSigned", out _);
                Command.Instance.Shell("Set-ExecutionPolicy -ExecutionPolicy Unrestricted", out _);
                var file = "install_gd.ps1";
                if (File.Exists(file))
                    File.Delete(file);
                File.WriteAllText(file, shellBuilder.ToString());
                var shellFile = Path.Combine(this.ProcessPath(), file);
                Command.Instance.Shell(@".\" + file, out string message);
                LogManager.Instance.LogInfo("Install OpenGL Script Result:" + message);
                LogManager.Instance.LogInfo("Install OpenGL Script Content:" + shellBuilder.ToString());
                File.Delete(file);
                if (Drives.Instance.HasOpenGL)
                {
                    OpenGLEnable = false;
                    MessageBox.Show(FindChar("OpenGLSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    OpenGLEnable = true;
                    MessageBox.Show(FindChar("OpenGLFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                HideLoading();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("ExcuteCommand", ex);
            }
        }
        private Task InstallIntelAsync()
        {
            RunOnUIThread(async () =>
            {
                ShowLoading();
                IntelEnable = false;
                GPU = GPUType.Intel;
                ProcessVisable = Visibility.Visible;
                await Drives.Instance.WSLDrive(GPUType.Intel);
            });
            return Task.CompletedTask;
        }
        private Task InstallNvidiaAsync()
        {
            RunOnUIThread(async () =>
            {
                ShowLoading();
                NvidiaEnable = false;
                GPU = GPUType.Nvidia;
                ProcessVisable = Visibility.Visible;
                await Drives.Instance.WSLDrive(GPUType.Nvidia);
                HideLoading();
            });
            return Task.CompletedTask;
        }
        private Task InstallAmdAsync()
        {
            RunOnUIThread(async () =>
            {
                ShowLoading();
                AmdEnable = false;
                GPU = GPUType.AMD;
                ProcessVisable = Visibility.Visible;
                await Drives.Instance.WSLDrive(GPUType.AMD);
                HideLoading();
            });
            return Task.CompletedTask;
        }
        private Task InstallOpenGLAsync()
        {
            RunOnUIThread(async () =>
            {
                ShowLoading();
                OpenGLEnable = false;
                ProcessVisable = Visibility.Visible;
                if (!await Drives.Instance.InstallOpenGL())
                {
                    OpenGLEnable = true;
                    ProcessVisable = Visibility.Collapsed;
                    MessageBox.Show(FindChar("CreateDownloadFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (Drives.Instance.PackageList.Count == Drives.Instance.PackageList.GetCount(x => x.Item3 == true))
                {
                    OpenGLEnable = true;
                    ExcuteCommand();
                    ProcessVisable = Visibility.Collapsed;
                    HideLoading();
                }
            });
            return Task.CompletedTask;
        }
        public void LoadAsync(object sender, EventArgs e)
        {
            Dispatcher = (sender as DrivePage).Dispatcher;
            RunOnUIThread(() =>
            {
                ShowLoading();
                OpenGLEnable = !Drives.Instance.HasOpenGL;
                foreach (ManagementObject mo in new ManagementObjectSearcher("Select * from Win32_VideoController").Get())
                {
                    var name = mo["Name"].ToString();
                    if (name.Contains("amd", StringComparison.CurrentCultureIgnoreCase))
                        AmdEnable = true;
                    if (name.Contains("nvidia", StringComparison.CurrentCultureIgnoreCase))
                        NvidiaEnable = true;
                    if (name.Contains("intel", StringComparison.CurrentCultureIgnoreCase))
                        IntelEnable = true;
                }
                HideLoading();
            });
        }
        public override void Dispose()
        {
            Drives.Instance.DownloadComplete -= Instance_DownloadComplete;
            DownloadManager.Instance.ProcessChange -= Downloader_ProcessChange;
        }
    }
}