using System;
using System.Windows;
using WSATools.Libs;
using WSATools.ViewModels;

namespace WSATools
{
    public partial class WSAList : Window
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
                ViewModel.Loading+=ViewModel_Loading;
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
    }
}