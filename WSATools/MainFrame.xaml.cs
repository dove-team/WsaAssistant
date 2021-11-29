using HandyControl.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WSATools.Libs;
using MessageBox = HandyControl.Controls.MessageBox;

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
            switch (((Button)sender).CommandParameter.ToString())
            {
                case "wsa":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/WsaPage.xaml"));
                        break;
                    }
                case "app":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/AppPage.xaml"));
                        break;
                    }
                case "about":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/AboutPage.xaml"));
                        break;
                    }
                case "exit":
                    {
                        if (MessageBox.Show(this.FindChar("ExitApp"), this.FindChar("Tips"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
            Adb.Instance.Close();
        }
    }
}