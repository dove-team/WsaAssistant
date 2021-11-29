using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using WSATools.Libs;
using WSATools.Libs.Model;
using WSATools.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WSATools.ViewModels
{
    public sealed class AppPageViewModel : ViewModelBase
    {
        private bool adbEnable = false;
        public bool AdbEnable
        {
            get => adbEnable;
            set => SetProperty(ref adbEnable, value);
        }
        private string searchKeywords;
        public string SearchKeywords
        {
            get => searchKeywords;
            set => SetProperty(ref searchKeywords, value);
        }
        private ListItem selectPackage;
        public ListItem SelectPackage
        {
            get => selectPackage;
            set => SetProperty(ref selectPackage, value);
        }
        private ObservableCollection<ListItem> packages = new ObservableCollection<ListItem>();
        public ObservableCollection<ListItem> Packages
        {
            get => packages;
            set => SetProperty(ref packages, value);
        }
        public IAsyncRelayCommand SearchCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand UninstallCommand { get; }
        public IAsyncRelayCommand InstallApkCommand { get; }
        public IAsyncRelayCommand DowngradeCommand { get; }
        public IAsyncRelayCommand UninstallApkCommand { get; }
        public AppPageViewModel()
        {
            SearchCommand = new AsyncRelayCommand(SearchAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            UninstallCommand = new AsyncRelayCommand(UninstallAsync);
            InstallApkCommand = new AsyncRelayCommand(InstallApkAsync);
            DowngradeCommand = new AsyncRelayCommand(DowngradeAsync);
            UninstallApkCommand = new AsyncRelayCommand(UninstallApkAsync);
        }
        public void LoadAsync(object sender, EventArgs e)
        {
            Dispatcher = (sender as AppPage).Dispatcher;
            RunOnUIThread(() =>
           {
               ShowLoading();
               AdbEnable = Adb.Instance.TryConnect();
               SearchApps();
               HideLoading();
           });
        }
        private Task RefreshAsync()
        {
            RunOnUIThread(() =>
           {
               ShowLoading();
               SearchApps();
               HideLoading();
           });
            return Task.CompletedTask;
        }
        private Task DowngradeAsync()
        {
            RunOnUIThread(() =>
            {
                ShowLoading();
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    FileName = string.Empty,
                    Filter = FindChar("ApkFile")
                };
                if (!string.IsNullOrEmpty(SelectPackage.Content) && openFileDialog.ShowDialog() == true)
                {
                    if (Adb.Instance.Downgrade(openFileDialog.FileName))
                        MessageBox.Show(FindChar("DowngradeSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show(FindChar("DowngradeFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                HideLoading();
            });
            return Task.CompletedTask;
        }
        private Task UninstallApkAsync()
        {
            RunOnUIThread(() =>
           {
               ShowLoading();
               var packageName = SelectPackage?.Content;
               if (!string.IsNullOrEmpty(packageName))
               {
                   if (MessageBox.Show($"{FindChar("UninstallTips")}{packageName}？", FindChar("Tips"), MessageBoxButton.YesNo, MessageBoxImage.Question)
                         == MessageBoxResult.Yes)
                   {
                       if (Adb.Instance.Uninstall(packageName))
                       {
                           MessageBox.Show(FindChar("UninstallSuccess"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Information);
                           SearchApps();
                       }
                       else
                           MessageBox.Show(FindChar("UninstallFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                   }
               }
               HideLoading();
           });
            return Task.CompletedTask;
        }
        private Task InstallApkAsync()
        {
            RunOnUIThread(() =>
           {
               ShowLoading();
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
                       SearchApps();
                   }
                   else
                       MessageBox.Show(FindChar("InstallFailed"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
               }
               HideLoading();
           });
            return Task.CompletedTask;
        }
        private Task SearchAsync()
        {
            RunOnUIThread(() =>
           {
               ShowLoading();
               SearchApps(SearchKeywords);
               HideLoading();
           });
            return Task.CompletedTask;
        }
        private Task UninstallAsync()
        {
            RunOnUIThread(() =>
            {
                ShowLoading();
                WSA.Instance.Clear();
                if (MessageBox.Show(FindChar("RebootTips"), FindChar("Tips"), MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    Command.Instance.Excute("shutdown -r -t 10", out _);
                Application.Current.Shutdown();
                HideLoading();
            });
            return Task.CompletedTask;
        }
        private void SearchApps(string condition = "")
        {
            ShowLoading();
            Dispatcher.Invoke(() => { Packages.Clear(); });
            if (!Adb.Instance.Connect())
            {
                AdbEnable = false;
                MessageBox.Show(FindChar("DevlopTips"), FindChar("Tips"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                AdbEnable = true;
                var list = Adb.Instance.GetAll(condition);
                foreach (var name in list)
                {
                    var item = new ListItem(name);
                    Dispatcher.Invoke(() => { Packages.Add(item); });
                }
            }
            HideLoading();
        }
        public override void Dispose()
        {

        }
    }
}