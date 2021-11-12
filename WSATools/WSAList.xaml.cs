using HandyControl.Controls;
using System;
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