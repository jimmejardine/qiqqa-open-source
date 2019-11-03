using System;
using System.ComponentModel;
using System.Windows;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Utilities;

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

            DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            ObjCheckOptions.IsEnabled = true;

            ButtonOk.Icon = Icons.GetAppIcon(Icons.Logout);
            ButtonOk.Caption = "Exit";
            ButtonOk.Click += ButtonOk_Click;
            ButtonCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            ButtonCancel.Caption = "Cancel";
            ButtonCancel.Click += ButtonCancel_Click;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            try
            {
                DataContext = null;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }
    }
}
