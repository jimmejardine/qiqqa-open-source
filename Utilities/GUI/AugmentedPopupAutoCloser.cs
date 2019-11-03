using System;
using System.Windows.Controls.Primitives;

namespace Utilities.GUI
{
    public class AugmentedPopupAutoCloser : IDisposable
    {
        private Popup popup;

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
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("AugmentedPopupAutoCloser::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                // Get rid of managed resources
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }

            popup = null;

            ++dispose_count;
        }
    }
}
