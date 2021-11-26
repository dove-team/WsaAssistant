using HandyControl.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using WSATools.Libs;
using WSATools.ViewModels;

namespace WSATools
{
    public partial class WSAList : BlurWindow
    {
        private WSAListViewModel ViewModel;
        public WSAList()
        {
            InitializeComponent();
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (DataContext is WSAListViewModel viewModel)
            {
                ViewModel = viewModel;
                ViewModel.Close += ViewModel_Close;
                ViewModel.Loading += ViewModel_Loading;
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
        private void ViewModel_Loading(object sender, bool result)
        {

        }
        private void ViewModel_Close(object sender, bool? result)
        {
            try
            {
                DialogResult = result;
                ViewModel.Dispose();
                Close();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("WSAList Close", ex);
            }
        }
        private void BlurWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MouseDown += WSAList_MouseDown;
        }
        private void WSAList_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}