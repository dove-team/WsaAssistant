using HandyControl.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WSATools
{
    public partial class MainFrame : BlurWindow
    {
        public MainFrame()
        {
            InitializeComponent();
        }
        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content.ToString())
            {
                case "环境":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/WsaPage.xaml"));
                        break;
                    }
                case "应用":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/AppPage.xaml"));
                        break;
                    }
                case "关于":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/AboutPage.xaml"));
                        break;
                    }
                case "退出":
                    {
                        Application.Current.Shutdown();
                        break;
                    }
            }
        }
        private void BlurWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MouseDown += WSAList_MouseDown;
        }
        private void WSAList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }
        private void BlurWindow_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}