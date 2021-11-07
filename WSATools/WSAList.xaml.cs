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