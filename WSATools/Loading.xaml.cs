using System.Windows;
using System.Windows.Controls;

namespace WSATools
{
    public partial class Loading : UserControl
    {
        public Loading()
        {
            InitializeComponent();
        }
        public static new readonly DependencyProperty ContentProperty =
           DependencyProperty.Register("Content", typeof(string), typeof(Loading),
               new PropertyMetadata("TextBlock", new PropertyChangedCallback(OnContentChanged)));
        private static void OnContentChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            Loading source = (Loading)sender;
            source.txtValue.Text = (string)args.NewValue;
        }
        public new string Content
        {
            get => (string)GetValue(ContentProperty);
            set
            {
                txtValue.Text = value;
                SetValue(ContentProperty, value);
            }
        }
    }
}