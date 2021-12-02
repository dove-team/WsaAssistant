using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Management;
using System.Threading.Tasks;
using WsaAssistant.Libs;
using WsaAssistant.Views;

namespace WsaAssistant.ViewModels
{
    public sealed class DrivePageViewModel : ViewModelBase
    {
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
        }
        private Task InstallIntelAsync()
        {
            return Task.CompletedTask;
        }
        private Task InstallNvidiaAsync()
        {
            return Task.CompletedTask;
        }
        private Task InstallAmdAsync()
        {
            return Task.CompletedTask;
        }
        private Task InstallOpenGLAsync()
        {
            return Task.CompletedTask;
        }
        public void LoadAsync(object sender, EventArgs e)
        {
            Dispatcher = (sender as DrivePage).Dispatcher;
            RunOnUIThread(() =>
            {
                ShowLoading();
                OpenGLEnable = Drives.Instance.HasOpenGL;
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

        }
    }
}