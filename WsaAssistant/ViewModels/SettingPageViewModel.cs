using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;
using WsaAssistant.Libs;
using WsaAssistant.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WsaAssistant.ViewModels
{
    public sealed class SettingPageViewModel : ViewModelBase
    {
        private bool hasWsaUpdate;
        public bool HasWsaUpdate
        {
            get => hasWsaUpdate;
            set => SetProperty(ref hasWsaUpdate, value);
        }
        private bool hasClientUpdate;
        public bool HasClientUpdate
        {
            get => hasClientUpdate;
            set => SetProperty(ref hasClientUpdate, value);
        }
        public IAsyncRelayCommand WsaFixCommand { get; }
        public IAsyncRelayCommand UpdateWsaCommand { get; }
        public IAsyncRelayCommand UpdateClientCommand { get; }
        public SettingPageViewModel()
        {
            WsaFixCommand = new AsyncRelayCommand(WsaFixAsync);
            UpdateWsaCommand = new AsyncRelayCommand(UpdateWsaAsync);
            UpdateClientCommand = new AsyncRelayCommand(UpdateClientAsync);
            Client.Instance.DownloadComplete += Instance_DownloadComplete;
        }
        private Task WsaFixAsync()
        {
            RunOnUIThread(async () =>
            {
                if (Adb.Instance.TryConnect())
                {
                    await Adb.Instance.Excute("settings put global captive_portal_detection_enabled 1", 80);
                    await Adb.Instance.Excute("settings put global captive_portal_mode 1", 80);
                    await Adb.Instance.Excute("settings put global captive_portal_use_https 0", 80);
                    await Adb.Instance.Excute("settings put global captive_portal_server connect.rom.miui.com", 80);
                    await Adb.Instance.Excute("settings put global captive_portal_http_url http://connect.rom.miui.com/generate_204", 80);
                    await Adb.Instance.Excute("settings put global captive_portal_https_url https://connect.rom.miui.com/generate_204", 200);
                    await WSA.Instance.ReStart();
                }
                else
                {
                    MessageBox.Show(FindChar("ClientSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });
            return Task.CompletedTask;
        }
        private async void Instance_DownloadComplete(object sender, bool state)
        {
            if (!state)
            {
                if (MessageBoxResult.Yes == MessageBox.Show(FindChar("ClientFailed"), FindChar("Tips"),
                    MessageBoxButton.YesNo, MessageBoxImage.Error))
                {
                    LogManager.Instance.LogInfo("下载客户端异常，重试中！");
                    await Client.Instance.Start();
                }
            }
            else
            {
                LogManager.Instance.LogInfo("下载客户端完成，开始安装！");
                MessageBox.Show(FindChar("ClientSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                Client.Instance.Install();
                Application.Current.Shutdown();
            }
            HideLoading();
        }
        private  Task UpdateClientAsync()
        {
            RunOnUIThread(async () =>
            {
                ShowLoading();
                await Client.Instance.Start();
            });
            return Task.CompletedTask;
        }
        private Task UpdateWsaAsync()
        {
            NavigateTo("InstallWsaPage");
            return Task.CompletedTask;
        }
        public void LoadAsync(object sender, EventArgs e)
        {
            Dispatcher = (sender as SettingPage).Dispatcher;
            RunOnUIThread(async () =>
            {
                ShowLoading();
                HasWsaUpdate = await WSA.Instance.HasUpdate();
                HasClientUpdate = Client.Instance.HasUpdate;
                HideLoading();
            });
        }
        public override void Dispose()
        {
            Client.Instance.DownloadComplete -= Instance_DownloadComplete;
        }
    }
}