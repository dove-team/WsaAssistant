using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WsaAssistant.Libs;

namespace WsaAssistant.ViewModels
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        private MainFrame MainWindow { get; }
        private MainFrameViewModel MainView { get; }
        public Dispatcher Dispatcher { get; protected set; }
        public ViewModelBase()
        {
            if (Application.Current.MainWindow is MainFrame mainFrame)
            {
                MainWindow = mainFrame;
                if (MainWindow.DataContext is MainFrameViewModel viewModel)
                    MainView = viewModel;
            }
        }
        protected void NavigateTo(string pageName)
        {
            Dispatcher.Invoke(() =>
            {
                var uri = new Uri($"pack://application:,,,/Views/{pageName}.xaml");
                MainWindow.frame.Navigate(uri);
            });
        }
        protected void ShowLoading()
        {
            if (MainView != null)
            {
                MainView.MenuEnable = false;
                MainView.LoadVisable = Visibility.Visible;
            }
        }
        protected void HideLoading()
        {
            if (MainView != null)
            {
                MainView.LoadVisable = Visibility.Collapsed;
                MainView.MenuEnable = true;
            }
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
                    catch (Exception ex)
                    {
                        LogManager.Instance.LogError("RunOnUIThread", ex);
                    }
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
                    catch (Exception ex)
                    {
                        LogManager.Instance.LogError("RunOnUIThread", ex);
                    }
                });
            });
        }
        public string FindChar(string key)
        {
            var obj = LangManager.Instance.Resource[key];
            return obj == null ? string.Empty : obj.ToString();
        }
        public abstract void Dispose();
    }
}