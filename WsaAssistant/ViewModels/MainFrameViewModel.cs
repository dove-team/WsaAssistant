using System.Windows;
using System.Windows.Media;

namespace WsaAssistant.ViewModels
{
    public sealed class MainFrameViewModel : ViewModelBase
    {
        private SolidColorBrush sysColor;
        public SolidColorBrush SysColor
        {
            get => sysColor;
            set => SetProperty(ref sysColor, value);
        }
        public MainFrameViewModel()
        {
            try
            {
                SysColor = new SolidColorBrush(SystemParameters.WindowGlassColor);
            }
            catch
            {
                SysColor = new SolidColorBrush(Colors.Cyan);
            }
        }
        private Visibility loadVisable = Visibility.Collapsed;
        public Visibility LoadVisable
        {
            get => loadVisable;
            set => SetProperty(ref loadVisable, value);
        }
        private bool menuEnable = true;
        public bool MenuEnable
        {
            get => menuEnable;
            set => SetProperty(ref menuEnable, value);
        }
        public override void Dispose()
        {

        }
    }
}