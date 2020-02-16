using System;
using System.Windows.Threading;
using Syncfusion.Pdf.Parsing;
using Utilities.GUI;

namespace Utilities.PDF
{
    /// <summary>
    /// Wraps PdfLoadedDocument - predominantly to close the underlying document when Disposed of.
    /// Always use in a using(){} block to ensure the PDF is closed and not left locked.
    /// </summary>   

    public class AugmentedPdfLoadedDocument : PdfLoadedDocument, IDisposable
    {
        public AugmentedPdfLoadedDocument(string filename)
            : base(filename)
        {
            Logging.Debug("+AugmentedPdfLoadedDocument::constructor: {0}", filename);
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~AugmentedPdfLoadedDocument()
        {
            Logging.Debug("~AugmentedPdfLoadedDocument()");
            Dispose(false);
        }

        public new void Dispose()
        {
            Logging.Debug("Disposing AugmentedPdfLoadedDocument");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("AugmentedPdfLoadedDocument::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    Close(true);
                }
            });

            WPFDoEvents.SafeExec(() =>
            {
                base.Dispose();
            });
            
            ++dispose_count;
        }

        #endregion

    }
}
