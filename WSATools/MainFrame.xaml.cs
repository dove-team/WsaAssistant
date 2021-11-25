using HandyControl.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

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
                        frame.Navigate(new Uri("Views/WsaPage.xaml"));
                        break;
                    }
                case "应用":
                    {
                        break;
                    }
                case "更多":
                    {
                        break;
                    }
                case "关于":
                    {
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

        }

        private void BlurWindow_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}