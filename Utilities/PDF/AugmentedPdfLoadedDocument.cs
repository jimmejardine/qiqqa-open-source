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
        }

        ~AugmentedPdfLoadedDocument()
        {
            Dispose(false);            
        }

        public new void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);            
        } 

        private void Dispose(bool disposing)
        {
            base.Dispose();

            if (disposing)
            {
                // Get rid of managed resources
                this.Close(true);
            }

            // Get rid of unmanaged resources 
        }
    }
}
