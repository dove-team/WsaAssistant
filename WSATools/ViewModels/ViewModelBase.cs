using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WSATools.ViewModels
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        private MainFrameViewModel MainView { get; }
        public Dispatcher Dispatcher { get; protected set; }
        public ViewModelBase()
        {
            if (Application.Current.MainWindow.DataContext is MainFrameViewModel viewModel)
                MainView = viewModel;
        }
        protected void ShowLoading()
        {
            if (MainView != null)
                MainView.LoadVisable = Visibility.Visible;
        }
        protected void HideLoading()
        {
            if (MainView != null)
                MainView.LoadVisable = Visibility.Collapsed;
        }
        protected void RunOnUIThread(Action action)
        {
            Dispatcher.Invoke(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch { }
                });
            });
        }
        protected void RunOnUIThread(Func<Task> func)
        {
            Dispatcher.Invoke(() =>
            {
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await func?.Invoke();
                    }
                    catch { }
                });
            });
        }
        public string FindChar(string key)
        {
            var obj = LangManager.Instance.Resource[key];
            return obj == null ? string.Empty : obj.ToString();
        }
        public abstract void Dispose();
        public void InstallWsa()
        {
            if (Application.Current.MainWindow is MainFrame main)
                main.frame.Navigate(new Uri("pack://application:,,,/Views/InstallWsaPage.xaml"));
        }
    }
}