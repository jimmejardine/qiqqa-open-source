using Qiqqa.Documents.PDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qiqqa.DocumentLibrary
{
    public delegate void OnPdfActionType(PDFDocument pdf, string filename);
    public delegate void OnPdfErrorActionType(Exception ex, PDFDocument pdf, string filename);

    public struct LibraryPdfActionCallbacks
    {
        public OnPdfActionType OnAdded;
        public OnPdfActionType OnSkipped;
        public OnPdfActionType OnAddedOrSkipped
        {
            set
            {
                OnAdded = value;
                OnSkipped = value;
            }
        }
        public OnPdfErrorActionType OnError;
    }
}
