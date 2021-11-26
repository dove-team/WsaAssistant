using System.Windows;
using System.Windows.Media;

namespace WSATools.ViewModels
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
        public override void Dispose()
        {

        }
    }
}