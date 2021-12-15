using System.Windows;
using System.Windows.Controls;

namespace WsaAssistant.Views
{
    public partial class AppPage : Page
    {
        public AppPage()
        {
            InitializeComponent();
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkState = ((CheckBox)sender).IsChecked;
            ViewModel?.FilterPackages(checkState ?? false);
        }
    }
}