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
            Logging.Info("~AugmentedPopupAutoCloser()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Info("Disposing AugmentedPopupAutoCloser");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Get rid of managed resources
                if (null != popup)
                {
                    popup.IsOpen = false;
                    popup = null;

                }
            }

            // Get rid of unmanaged resources 
        }

    }
}
