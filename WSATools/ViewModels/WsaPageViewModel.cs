using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;
using WSATools.Libs;
using WSATools.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WSATools.ViewModels
{
    public sealed class WsaPageViewModel : ViewModelBase
    {
        private Visibility wsaRuning = Visibility.Visible;
        public Visibility WsaRuning
        {
            get => wsaRuning;
            set => SetProperty(ref wsaRuning, value);
        }
        private Visibility startWsa = Visibility.Visible;
        public Visibility StartWsa
        {
            get => startWsa;
            set => SetProperty(ref startWsa, value);
        }
        private string registStatus;
        public string RegistStatus
        {
            get => registStatus;
            set => SetProperty(ref registStatus, value);
        }
        private Visibility hasRegist = Visibility.Visible;
        public Visibility HasRegist
        {
            get => hasRegist;
            set => SetProperty(ref hasRegist, value);
        }
        private Visibility installRegist = Visibility.Collapsed;
        public Visibility InstallRegist
        {
            get => installRegist;
            set => SetProperty(ref installRegist, value);
        }
        private string wsaStatus;
        public string WsaStatus
        {
            get => wsaStatus;
            set => SetProperty(ref wsaStatus, value);
        }
        private Visibility hasWsa = Visibility.Visible;
        public Visibility HasWsa
        {
            get => hasWsa;
            set => SetProperty(ref hasWsa, value);
        }
        private Visibility installWsa = Visibility.Collapsed;
        public Visibility InstallWsa
        {
            get => installWsa;
            set => SetProperty(ref installWsa, value);
        }
        private string featureStatus;
        public string FeatureStatus
        {
            get => featureStatus;
            set => SetProperty(ref featureStatus, value);
        }
        private Visibility hasFeature = Visibility.Visible;
        public Visibility HasFeature
        {
            get => hasFeature;
            set => SetProperty(ref hasFeature, value);
        }
        private Visibility installFeature = Visibility.Collapsed;
        public Visibility InstallFeature
        {
            get => installFeature;
            set => SetProperty(ref installFeature, value);
        }
        public IAsyncRelayCommand StartWsaCommand { get; }
        public IAsyncRelayCommand InstallWsaCommand { get; }
        public IAsyncRelayCommand InstallFeatureCommand { get; }
        public WsaPageViewModel()
        {
            StartWsaCommand = new AsyncRelayCommand(StartWsaAsync);
            InstallWsaCommand = new AsyncRelayCommand(InstallWsaAsync);
            InstallFeatureCommand = new AsyncRelayCommand(InstallFeatureAsync);
        }
        private Task StartWsaAsync()
        {
            RunOnUIThread(() =>
            {
                ShowLoading();
                if (Adb.Instance.TryConnect())
                {
                    WsaRuning = Visibility.Visible;
                    WsaStatus = FindChar("Running");
                }
                else
                {
                    WsaRuning = Visibility.Collapsed;
                    WsaStatus = FindChar("NotRunning");
                }
                HideLoading();
            });
            return Task.CompletedTask;
        }
        private Task InstallWsaAsync()
        {
            RunOnUIThread(() =>
            {
                WSAList list = new WSAList();
                if (list.ShowDialog() != null)
                {
                    if (WSA.Instance.HasWsa)
                    {
                        WsaStatus = FindChar("Installed");
                        HasWsa = Visibility.Collapsed;
                        InstallWsa = Visibility.Visible;
                        MessageBox.Show(FindChar("WsaSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        WsaStatus = FindChar("NotInstall");
                        HasWsa = Visibility.Collapsed;
                        InstallWsa = Visibility.Visible;
                        MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                    MessageBox.Show(FindChar("WsaFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Warning);
            });
            return Task.CompletedTask;
        }
        private Task InstallFeatureAsync()
        {
            RunOnUIThread(() =>
            {
                ShowLoading();
                if (WSA.Instance.InstallFeature())
                {
                    FeatureStatus = FindChar("Installed");
                    HasFeature = Visibility.Visible;
                    InstallFeature = Visibility.Collapsed;
                    HideLoading();
                }
                else
                {
                    FeatureStatus = FindChar("NotInstall");
                    HasFeature = Visibility.Collapsed;
                    InstallFeature = Visibility.Visible;
                    HideLoading();
                    if (MessageBox.Show(FindChar("RebootTips"), FindChar("Tips"), MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                        Command.Instance.Excute("shutdown -r -t 10", out _);
                }
            });
            return Task.CompletedTask;
        }
        public void LoadAsync(object sender, EventArgs e)
        {
            Dispatcher = (sender as WsaPage).Dispatcher;
            WsaStatus = RegistStatus = FeatureStatus = FindChar("Checking");
            RunOnUIThread(async () =>
            {
                ShowLoading();
                if (WSA.Instance.HasFeature)
                {
                    HasFeature = Visibility.Visible;
                    InstallFeature = Visibility.Collapsed;
                    FeatureStatus = FindChar("Installed");
                }
                else
                {
                    HasFeature = Visibility.Collapsed;
                    InstallFeature = Visibility.Visible;
                    FeatureStatus = FindChar("NotInstall");
                }
                if (WSA.Instance.HasWsa)
                {
                    HasWsa = Visibility.Visible;
                    InstallWsa = Visibility.Collapsed;
                    WsaStatus = FindChar("Installed");
                }
                else
                {
                    HasWsa = Visibility.Collapsed;
                    InstallWsa = Visibility.Visible;
                    WsaStatus = FindChar("NotInstall");
                }
                if (WSA.Instance.Running)
                {
                    WsaRuning = Visibility.Visible;
                    StartWsa = Visibility.Collapsed;
                    RegistStatus = FindChar("Running");
                }
                else
                {
                    StartWsa = Visibility.Visible;
                    WsaRuning = Visibility.Collapsed;
                    RegistStatus = FindChar("NotRunning");
                }
                if (await WSA.Instance.HasUpdate())
                {

                }
                HideLoading();
            });
        }
        public override void Dispose()
        {

        }
    }
}