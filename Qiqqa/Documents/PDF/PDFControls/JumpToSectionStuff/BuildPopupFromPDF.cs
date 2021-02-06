using System;
using Syncfusion.Pdf.Interactive;
using Utilities;
using Utilities.PDF;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    internal class BuildPopupFromPDF
    {
        private JumpToSectionPopup popup;
        private PDFDocument pdf_document;

        internal BuildPopupFromPDF(JumpToSectionPopup popup, PDFDocument pdf_document)
        {
            this.popup = popup;
            this.pdf_document = pdf_document;
        }

        internal void BuildMenu()
        {
            try
            {
                using (AugmentedPdfLoadedDocument doc = new AugmentedPdfLoadedDocument(pdf_document.DocumentPath))
                {
                    if (null != doc.Bookmarks)
                    {
                        GenerateBookmarks(doc, doc.Bookmarks, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem while generating bookmarks from the PDF file");
            }
        }

        private void GenerateBookmarks(AugmentedPdfLoadedDocument doc, PdfBookmarkBase bookmark_base, int depth)
        {
            // Don't go too deep in the bookmark hierarchy
            if (depth > 0)
            {
                return;
            }

            int last_start_page = 0;

            for (int i = 0; i < bookmark_base.Count; ++i)
            {
                PdfBookmark bookmark = bookmark_base[i];
                int page_number = -1;
                if (null != bookmark.Destination)
                {
                    if (-1 == page_number)
                    {
                        for (int j = last_start_page; j < doc.Pages.Count; ++j)
                        {
                            if (doc.Pages[j] == bookmark.Destination.Page)
                            {
                                page_number = j;
                                last_start_page = j;
                                break;
                            }
                        }
                    }

                    if (-1 == page_number)
                    {
                        for (int j = 0; j < last_start_page; ++j)
                        {
                            if (doc.Pages[j] == bookmark.Destination.Page)
                            {
                                page_number = j;
                                last_start_page = j;
                                break;
                            }
                        }
                    }
                }

                if (-1 != page_number)
                {
                    popup.Children.Add(new JumpToSectionItem(popup, popup.pdf_reading_control, bookmark.Title, page_number + 1));
                }

                GenerateBookmarks(doc, bookmark, depth + 1);
            }
        }
    }
}

