using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WSATools.Libs;

namespace WSATools.ViewModels
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        public event LoadingHandler Loading;
        public Dispatcher Dispatcher { get; protected set; }
        private Visibility loadVisable = Visibility.Collapsed;
        public Visibility LoadVisable
        {
            get => loadVisable;
            set
            {
                SetProperty(ref loadVisable, value);
                IsLoading = value == Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Loading?.Invoke(this, value);
                });
            }
        }
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                SetProperty(ref isLoading, value);
            }
        }
        protected void RunOnUIThread(Action action)
        {
            Dispatcher.Invoke(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    action?.Invoke();
                });
            });
        }
        protected void RunOnUIThread(Func<Task> func)
        {
            Dispatcher.Invoke(() =>
            {
                Task.Factory.StartNew(async () =>
                {
                    await func?.Invoke();
                });
            });
        }
        public abstract void Dispose();
    }
}