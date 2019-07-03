using System;
using System.Windows;
using icons;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Common.GUI
{
    public class StandardWindow : Window
    {
        public StandardWindow()
        {
            this.Background = ThemeColours.Background_Brush_Blue_LightToDark;
            this.FontFamily = ThemeTextStyles.FontFamily_Standard;
            this.Icon = Icons.GetAppIconICO(Icons.Qiqqa);
            this.Width = 800;
            this.Height = 600;

            this.UseLayoutRounding = true;

            if (RegistrySettings.Instance.IsSet(RegistrySettings.SnapToPixels))
            {
                Logging.Info("Snapping to device pixels.");
                this.SnapsToDevicePixels = true;
            }

            this.Closed += StandardWindow_Closed;
        }

        void StandardWindow_Closed(object sender, EventArgs e)
        {
            IDisposable disposable = this.Content as IDisposable;
            this.Content = null;

            if (null != disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
