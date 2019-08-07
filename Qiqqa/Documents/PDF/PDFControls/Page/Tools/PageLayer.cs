using System;
using System.Windows.Controls;
using Utilities;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    public class PageLayer : Canvas
    {
        internal virtual void Dispose() {
            //Logging.Debug("PageLayer::Dispose()");

            throw new Exception("unexpected invocation of abstract parent class Dispose method");
        }
        internal virtual void SelectPage() { }
        internal virtual void DeselectPage() { }
        internal virtual void SelectLayer() { }
        internal virtual void DeselectLayer() { }
        internal virtual void PageTextAvailable() { }
    }
}
