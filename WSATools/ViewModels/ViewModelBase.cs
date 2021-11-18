﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WSATools.Libs;

namespace WSATools.ViewModels
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        public event BooleanHandler Loading;
        public Dispatcher Dispatcher { get; protected set; }
        private Visibility loadVisable = Visibility.Collapsed;
        public Visibility LoadVisable
        {
            get => loadVisable;
            set
            {
                SetProperty(ref loadVisable, value);
                Dispatcher.Invoke(() =>
                {
                    var boolean = value == Visibility.Visible;
                    Loading?.Invoke(this, boolean);
                });
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
    }
}