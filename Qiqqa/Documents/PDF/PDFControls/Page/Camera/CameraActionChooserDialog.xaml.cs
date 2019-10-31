using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Camera
{
    /// <summary>
    /// Interaction logic for CameraActionChooserDialog.xaml
    /// </summary>
    public partial class CameraActionChooserDialog : StandardWindow
    {
        private CroppedBitmap image;
        private string raw_text;
        private string tabled_text;

        public CameraActionChooserDialog()
        {
            InitializeComponent();

            this.Title = "What would you like to do with your snapshot?";

            CmdImage.Caption = "Copy the snapshot image to the clipboard.";
            CmdTabulatedText.Caption = "Copy the tabulated text to the clipboard.\n\nThen try pasting it into Excel...";
            CmdTabulatedCharts.Caption = "Generate Datacopia.com charts from the tabulated text.";
            CmdRawText.Caption = "Copy the raw text to the clipboard.\n\nThen try pasting it into Word...";

            CmdImage.Click += Button_Click;
            CmdRawText.Click += Button_Click;
            CmdTabulatedText.Click += Button_Click;
            CmdTabulatedCharts.Click += Button_Click;

            ObjImageDatacopia.Source = Icons.GetAppIcon(Icons.Datacopia);
            ObjImageDatacopia.IsHitTestVisible = false;

            ObjImageSocialMedia.Source = Icons.GetAppIcon(Icons.SocialMedia);
            ObjImageSocialMedia.IsHitTestVisible = false;

            this.KeyDown += CameraActionChooserDialog_KeyDown;
        }

        void CameraActionChooserDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Escape == e.Key)
            {
                e.Handled = true;
                this.Close();
            }
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CmdImage == sender)
            {
                Clipboard.SetImage(this.image);
                StatusManager.Instance.UpdateStatus("RegionSnapshot", "An image snapshot of the PDF region has been copied to the clipboard.");
            }

            else if (CmdRawText == sender)
            {
                ClipboardTools.SetText(raw_text);
                StatusManager.Instance.UpdateStatus("RegionSnapshot", "A text snapshot of the PDF region has been copied to the clipboard.");
            }

            else if (CmdTabulatedText == sender)
            {
                ClipboardTools.SetText(tabled_text);
                StatusManager.Instance.UpdateStatus("RegionSnapshot", "A tabulated snapshot of the PDF region has been copied to the clipboard.");
            }

            else if (CmdTabulatedCharts == sender)
            {
                string data = Uri.EscapeDataString(tabled_text);
                string url = String.Format(WebsiteAccess.Url_DatacopiaSearch, data);
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(url, true);
            }

            this.Close();
        }

        internal void SetLovelyDetails(CroppedBitmap image, string raw_text, string tabled_text)
        {
            this.image = image;
            this.raw_text = raw_text;
            this.tabled_text = tabled_text;

            this.ObjImage.Source = image;
            this.ObjRawText.Text = raw_text;
            this.ObjTabulatedText.Text = tabled_text;
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

            image = null;
            ObjImage.Source = null;
        }
    }
}
