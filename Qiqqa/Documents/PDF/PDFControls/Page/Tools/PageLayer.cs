using System.Windows.Controls;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    public class PageLayer : Canvas
    {
        internal virtual void Dispose() { }
        internal virtual void SelectPage() { }
        internal virtual void DeselectPage() { }
        internal virtual void SelectLayer() { }
        internal virtual void DeselectLayer() { }
        internal virtual void PageTextAvailable() { }
    }
}
