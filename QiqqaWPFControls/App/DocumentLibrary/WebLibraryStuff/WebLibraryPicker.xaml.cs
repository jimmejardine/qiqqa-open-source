using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using icons;
using Qiqqa.Common.GUI;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    /// <summary>
    /// Interaction logic for WebLibraryPicker.xaml
    /// </summary>
    public partial class WebLibraryPicker : StandardWindow
    {
        public static readonly string TITLE = "Please choose a library";

        private WebLibraryDetail last_picked_web_library_detail = null;

        private WebLibraryPicker()
        {
            InitializeComponent();

            Title = TITLE;

            ButtonCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            ButtonCancel.Caption = "Cancel";
            ButtonCancel.Click += ButtonCancel_Click;

            ObjWebLibraryListControl.OnWebLibrarySelected += ObjWebLibraryListControl_OnWebLibrarySelected;
            PreviewKeyDown += WebLibraryPicker_PreviewKeyDown;

            ObjWebLibraryListControl.Refresh();
        }

        private void Cancel()
        {
            last_picked_web_library_detail = null;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void WebLibraryPicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Cancel();
                e.Handled = true;
            }
        }

        private void ObjWebLibraryListControl_OnWebLibrarySelected(WebLibraryDetail web_library_detail)
        {
            last_picked_web_library_detail = web_library_detail;
            Close();
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static WebLibraryDetail PickWebLibrary()
        {
            return PickWebLibrary(null);
        }

        public static WebLibraryDetail PickWebLibrary(string message)
        {
            List<WebLibraryDetail> wlds = WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries_All;

            switch (wlds.Count)
            {
                case 0:
                    // If we have no libraries, use the guest account...
                    return WebLibraryManager.Instance.WebLibraryDetails_Guest;

                case 1:
                    // If we have only one library, use that...
                    return wlds[0];

                default:
                    // Otherwise fall back on the GUI
                    return PickWebLibrary_GUI(message);
            }
        }

        private static WebLibraryDetail PickWebLibrary_GUI(string message)
        {
            WebLibraryPicker web_library_picker = new WebLibraryPicker();
            web_library_picker.last_picked_web_library_detail = null;

            if (null != message)
            {
                web_library_picker.TextMessage.Inlines.Clear();
                web_library_picker.TextMessage.Inlines.Add(message);
            }
            web_library_picker.ShowDialog();
            return web_library_picker.last_picked_web_library_detail;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            WebLibraryDetail web_library_detail = PickWebLibrary();
            Logging.Info("You picked {0}", web_library_detail);
        }
#endif

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            last_picked_web_library_detail = null;
        }
    }
}
