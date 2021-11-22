using HandyControl.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
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
        private void ViewModel_Loading(object sender, bool result)
        {
            switch (result)
            {
                case false:
                    loading.IsOpen = false;
                    break;
                case true:
                    loading.IsOpen = true;
                    break;
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
        private void BlurWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MouseDown += MainWindow_MouseDown;
            UpdateBackgroundThread.Instance.CheckUpdate();
        }
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!UpdateBackgroundThread.Instance.ShowUpdate(e))
            {
                if (DownloadManager.Instance.HasClear && MessageBox.Show(ViewModel.FindChar("RemvoeDownload"),
                    ViewModel.FindChar("Tips"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    DownloadManager.Instance.Clear();
                e.Cancel = false;
            }
            base.OnClosing(e);
        }
    }
}