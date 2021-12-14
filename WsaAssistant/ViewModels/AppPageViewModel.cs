﻿using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WsaAssistant.Libs;
using WsaAssistant.Libs.Model;
using WsaAssistant.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WsaAssistant.ViewModels
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
        private Package selectPackage;
        public Package SelectPackage
        {
            get => selectPackage;
            set => SetProperty(ref selectPackage, value);
        }
        private List<Package> AllPackages;
        private ObservableCollection<Package> packages = new ObservableCollection<Package>();
        public ObservableCollection<Package> Packages
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
                if (WSA.Instance.HasWsa)
                {
                    AdbEnable = Adb.Instance.TryConnect();
                    SearchApps();
                }
                else
                {
                    MessageBox.Show(FindChar("NeedWsa"), FindChar("Tips"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
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
                if (SelectPackage == null)
                {

                    return;
                }
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    FileName = string.Empty,
                    Filter = FindChar("ApkFile")
                };
                if (!string.IsNullOrEmpty(SelectPackage.PackageName) && openFileDialog.ShowDialog() == true)
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
               var packageName = SelectPackage?.PackageName;
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
                AllPackages = Adb.Instance.GetAll(condition);
                foreach (var item in AllPackages)
                    Dispatcher.Invoke(() => { Packages.Add(item); });
            }
            HideLoading();
        }
        public void FilterPackages(bool userOnly)
        {
            try
            {
                Dispatcher.Invoke(() => { Packages.Clear(); });
                if (userOnly)
                {
                    var array = AllPackages.Where(x => x.IsSystem == false);
                    foreach (var obj in array)
                        Dispatcher.Invoke(() => { Packages.Add(obj); });
                }
                else
                {
                    foreach (var item in AllPackages)
                        Dispatcher.Invoke(() => { Packages.Add(item); });
                }
            }
            catch { }
        }
        public override void Dispose()
        {

        }
    }
}