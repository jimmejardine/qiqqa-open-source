using System;
using System.ComponentModel;
using System.Windows;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;

namespace Qiqqa.Main.LogoutStuff
{
    /// <summary>
    /// Interaction logic for LogoutWindow.xaml
    /// </summary>
    public partial class LogoutWindow : StandardWindow
    {
        public LogoutWindow()
        {
            InitializeComponent();

            this.DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            ObjCheckOptions.IsEnabled = true;

            this.ButtonOk.Icon = Icons.GetAppIcon(Icons.Logout);
            this.ButtonOk.Caption = "Exit";
            this.ButtonOk.Click += ButtonOk_Click;
            this.ButtonCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            this.ButtonCancel.Caption = "Cancel";
            this.ButtonCancel.Click += ButtonCancel_Click;
        }

        void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this calss Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            this.DataContext = null;
        }
    }
}
