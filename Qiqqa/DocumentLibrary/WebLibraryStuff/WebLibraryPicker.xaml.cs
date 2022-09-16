using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using icons;
using Qiqqa.Common.GUI;
using Utilities;
using Utilities.Misc;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    /// <summary>
    /// Interaction logic for WebLibraryPicker.xaml
    /// </summary>
    public partial class WebLibraryPicker : StandardWindow
    {
        public static readonly string TITLE = "Please choose a library";

        public class PickedEventArgs
        {
            public WebLibraryDetail pickedLibrary;
        }

        //
        // Summary:
        //     Represents the method that will handle the event when the user picked a library.
        //
        // Parameters:
        //   sender:
        //     The source of the event.
        //
        //   e:
        //     An object that contains event data.
        //
        private delegate void PickedEventHandler(object sender, PickedEventArgs e);
        private event PickedEventHandler Picked;

        private WebLibraryPicker()
        {
            //Theme.Initialize(); -- already done in StandardWindow base class

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
            Picked.Invoke(this, new PickedEventArgs()
            {
                pickedLibrary = null
            });

            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Logging.Info("User clicked on CANCEL button in Library picker.");
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
            Logging.Info("User clicked on a library entry in Library picker: {0}", web_library_detail);

            Picked.Invoke(this, new PickedEventArgs()
            {
                pickedLibrary = web_library_detail
            });

            Close();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------

        public static WebLibraryDetail PickWebLibrary(string message = null)
        {
            List<WebLibraryDetail> wlds = WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries;

            ASSERT.Test(wlds.Count > 0);

            switch (wlds.Count)
            {
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
            WebLibraryDetail rv = null;

            web_library_picker.Picked += delegate (object sender, PickedEventArgs e) {
                rv = e.pickedLibrary;
            };

            if (null != message)
            {
                web_library_picker.TextMessage.Inlines.Clear();
                web_library_picker.TextMessage.Inlines.Add(message);
            }
            web_library_picker.ShowDialog();

            return rv;
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

        }
    }
}
