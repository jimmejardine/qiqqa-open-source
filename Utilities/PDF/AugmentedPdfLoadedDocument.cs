using System;
using Syncfusion.Pdf.Parsing;

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

        ~AugmentedPdfLoadedDocument()
        {
            Logging.Info("~AugmentedPdfLoadedDocument()");
            Dispose(false);
        }

        public new void Dispose()
        {
            Logging.Info("Disposing AugmentedPdfLoadedDocument");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        private void Dispose(bool disposing)
        {
            Logging.Debug("AugmentedPdfLoadedDocument::Dispose({0}) @{1}", disposing ? "true" : "false", ++dispose_count);
            if (disposing)
            {
                // Get rid of managed resources
                this.Close(true);
            }

            // Get rid of unmanaged resources 

            base.Dispose();
        }
    }
}
