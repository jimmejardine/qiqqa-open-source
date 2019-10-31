using System;
using System.Windows.Controls;
using Utilities;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    public class PageLayer : Canvas, IDisposable
    {
        internal virtual void SelectPage() { }
        internal virtual void DeselectPage() { }
        internal virtual void SelectLayer() { }
        internal virtual void DeselectLayer() { }
        internal virtual void PageTextAvailable() { }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PageLayer()
        {
            Logging.Debug("~PageLayer()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing PageLayer");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PageLayer::Dispose({0}) @{1}", disposing, dispose_count);

            ++dispose_count;

            throw new Exception("PageLayer: unexpected invocation of abstract parent class Dispose method");
        }

        #endregion

    }
}
