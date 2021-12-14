using HandyControl.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WsaAssistant.Libs;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WsaAssistant
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
                case "drive":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/DrivePage.xaml"));
                        break;
                    }
                case "app":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/AppPage.xaml"));
                        break;
                    }
                case "setting":
                    {
                        frame.Navigate(new Uri("pack://application:,,,/Views/SettingPage.xaml"));
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
                        {
                            this.Close();
                            Application.Current.Shutdown();
                        }
                        break;
                    }
            }
        }
        private void BlurWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MouseDown += WSAList_MouseDown;
            Client.Instance.CheckUpdate();
        }
        private void WSAList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Adb.Instance.Close();
        }
    }
}