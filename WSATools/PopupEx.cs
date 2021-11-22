using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace WSATools
{
    public sealed class PopupEx : Popup
    {
        public bool IsPositionUpdate
        {
            get { return (bool)GetValue(IsPositionUpdateProperty); }
            set { SetValue(IsPositionUpdateProperty, value); }
        }
        public static readonly DependencyProperty IsPositionUpdateProperty =
            DependencyProperty.Register("IsPositionUpdate", typeof(bool), typeof(PopupEx), new PropertyMetadata(true, new PropertyChangedCallback(IsPositionUpdateChanged)));
        private static void IsPositionUpdateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PopupEx).OnLoaded(d as PopupEx, null);
        }
        public PopupEx()
        {
            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Popup pup = sender as Popup;
            var win = VisualTreeHelper.GetParent(pup);
            while (win != null && (win as Window) == null)
                win = VisualTreeHelper.GetParent(win);
            if (win is Window window)
            {
                window.LocationChanged -= PositionChanged;
                window.SizeChanged -= PositionChanged;
                if (IsPositionUpdate)
                {
                    window.LocationChanged += PositionChanged;
                    window.SizeChanged += PositionChanged;
                }
            }
        }
        private void PositionChanged(object sender, EventArgs e)
        {
            try
            {
                var method = typeof(Popup).GetMethod("UpdatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
                if (IsOpen) method.Invoke(this, null);
            }
            catch
            {
                return;
            }
        }
        public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(typeof(Popup), new FrameworkPropertyMetadata(false, OnTopmostChanged));
        public bool Topmost
        {
            get => (bool)GetValue(TopmostProperty);
            set => SetValue(TopmostProperty, value);
        }
        private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as PopupEx).UpdateWindow();
        }
        protected override void OnOpened(EventArgs e)
        {
            UpdateWindow();
        }
        private void UpdateWindow()
        {
            var hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;
            if (NativeMethods.GetWindowRect(hwnd, out RECT rect))
                NativeMethods.SetWindowPos(hwnd, Topmost ? -1 : -2, rect.Left, rect.Top, (int)Width, (int)Height, 0);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public static class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
            [DllImport("user32", EntryPoint = "SetWindowPos")]
            internal static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        }
    }
}