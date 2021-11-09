using HandyControl.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using WSATools.Libs;
using WSATools.ViewModels;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WSATools
{
    public partial class MainWindow : BlurWindow
    {
        private MainWindowViewModel ViewModel;
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (DataContext is MainWindowViewModel viewModel)
            {
                ViewModel = viewModel;
                ViewModel.Close += ViewModel_Close;
                ViewModel.Enable += ViewModel_Enable;
                ViewModel.Loading += ViewModel_Loading;
            }
        }
        private void ViewModel_Enable(object sender, bool state)
        {
            IsEnabled = state;
        }
        private void ViewModel_Close(object sender, bool? result)
        {
            Close();
            Application.Current.Shutdown();
        }
        private void ViewModel_Loading(object sender, Visibility result)
        {
            switch (result)
            {
                case Visibility.Collapsed:
                    loading.IsOpen = false;
                    break;
                case Visibility.Visible:
                    loading.IsOpen = true;
                    break;
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            ViewModel.Dispose();
            if (MessageBox.Show("是否清除下载的文件？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Downloader.Clear();
            base.OnClosing(e);
        }
    }
}