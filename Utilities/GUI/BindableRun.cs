using System.Windows;
using System.Windows.Documents;

namespace Utilities.GUI
{
    public class BindableRun : Run
    {
        public static DependencyProperty BindableTextProperty = DependencyProperty.Register("BindableText", typeof(string), typeof(BindableRun), new PropertyMetadata(null, OnBindableTextChanged));
        public string BindableText
        {
            get { return (string)GetValue(BindableTextProperty); }
            set { SetValue(BindableTextProperty, value); }
        }

        static void OnBindableTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BindableRun br = (BindableRun)sender;
            br.Text = (string)e.NewValue;
        }
    }
}
