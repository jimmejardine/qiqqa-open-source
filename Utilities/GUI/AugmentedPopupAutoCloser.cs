using System;
using System.Windows.Controls.Primitives;

namespace Utilities.GUI
{
    public class AugmentedPopupAutoCloser : IDisposable
    {
        Popup popup;

        public AugmentedPopupAutoCloser(Popup popup)
        {
            this.popup = popup;
        }

        ~AugmentedPopupAutoCloser()
        {
            Logging.Debug("~AugmentedPopupAutoCloser()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Debug("Disposing AugmentedPopupAutoCloser");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        private void Dispose(bool disposing)
        {
            Logging.Debug("AugmentedPopupAutoCloser::Dispose({0}) @{1}", disposing ? "true" : "false", ++dispose_count);
            if (disposing)
            {
                // Get rid of managed resources
                if (null != popup)
                {
                    popup.IsOpen = false;
                }
            }

            popup = null;

            // Get rid of unmanaged resources 
        }
    }
}
