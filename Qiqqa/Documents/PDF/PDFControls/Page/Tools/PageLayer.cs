using System;
using System.Windows.Controls;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    public class PageLayer : Canvas
    {
        public PageLayer()
        {
            // must run in GUI thread as a lot of UI components require this...
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();
        }

        internal virtual void SelectPage() { }
        internal virtual void DeselectPage() { }
        internal virtual void SelectLayer() { }
        internal virtual void DeselectLayer() { }
        internal virtual void PageTextAvailable() { }

        // --- IDisposable ------------------------------------------------------------------------

        public virtual void Dispose()
        {
            Logging.Debug("PageLayer::Dispose()");

            throw new Exception("PageLayer: unexpected invocation of abstract parent class Dispose method");
        }

    }
}
