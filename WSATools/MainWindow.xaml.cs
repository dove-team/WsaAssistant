using System;
using System.ComponentModel;
using System.Windows;
using WSATools.Libs;
using WSATools.ViewModels;

namespace WSATools
{
    public partial class MainWindow : Window
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
                ViewModel.Loading += ViewModel_Loading;
            }
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
        private void ViewModel_Close(object sender, EventArgs e)
        {
            Close();
            Application.Current.Shutdown();
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