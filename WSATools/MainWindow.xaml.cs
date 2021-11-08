using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using WSATools.Libs;
using WSATools.ViewModels;

namespace WSATools
{
    public partial class MainWindow : Window
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

        [Flags]
        public enum DwmWindowAttribute : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029
        }

        // Enable Mica on the given HWND.
        public static void EnableMica(HwndSource source, bool darkThemeEnabled)
        {
            int trueValue = 0x01;
            int falseValue = 0x00;

            // Set dark mode before applying the material, otherwise you'll get an ugly flash when displaying the window.
            if (darkThemeEnabled)
                DwmSetWindowAttribute(source.Handle, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref trueValue, Marshal.SizeOf(typeof(int)));
            else
                DwmSetWindowAttribute(source.Handle, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref falseValue, Marshal.SizeOf(typeof(int)));

            DwmSetWindowAttribute(source.Handle, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
        }

        public static void UpdateStyleAttributes(HwndSource hwnd)
        {
            // You can avoid using ModernWpf here and just rely on Win32 APIs or registry parsing if you want to.
            var darkThemeEnabled = ModernWpf.ThemeManager.Current.ActualApplicationTheme == ModernWpf.ApplicationTheme.Dark;

            EnableMica(hwnd, darkThemeEnabled);
        }

        private MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (DataContext is MainWindowViewModel viewModel)
            {
                ViewModel = viewModel;
                ViewModel.Close += ViewModel_Close;
                ViewModel.Loading += ViewModel_Loading;
            }
        }

        private void ViewModel_Loading(object sender, Visibility result)
        {
            switch (result)
            {
                case Visibility.Collapsed:
                    //loading.IsOpen = false;
                    break;
                case Visibility.Visible:
                    //loading.IsOpen = true;
                    break;
            }
        }

        private void ViewModel_Close(object sender, EventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ViewModel.Dispose();
            if (MessageBox.Show("是否清除下载的文件？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Downloader.Clear();
            base.OnClosing(e);
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            // Apply Mica brush and ImmersiveDarkMode if needed
            UpdateStyleAttributes((HwndSource)sender);

            // Hook to Windows theme change to reapply the brushes when needed
            ModernWpf.ThemeManager.Current.ActualApplicationThemeChanged += (s, ev) => UpdateStyleAttributes((HwndSource)sender);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get PresentationSource
            PresentationSource presentationSource = PresentationSource.FromVisual((Visual)sender);

            // Subscribe to PresentationSource's ContentRendered event
            presentationSource.ContentRendered += Window_ContentRendered;
        }
    }
}