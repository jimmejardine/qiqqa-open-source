using System;
using Syncfusion.Pdf.Interactive;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.PDF;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    internal class BuildPopupFromPDF
    {
        static internal void BuildMenu(JumpToSectionPopup popup, PDFReadingControl pdf_reading_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            PDFDocument pdf_document = pdf_reading_control.GetPDFDocument();
            ASSERT.Test(pdf_document != null);

            try
            {
                using (AugmentedPdfLoadedDocument doc = new AugmentedPdfLoadedDocument(pdf_document.DocumentPath))
                {
                    if (null != doc.Bookmarks)
                    {
                        GenerateBookmarks(popup, pdf_reading_control, doc, doc.Bookmarks, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem while generating bookmarks from the PDF file");
            }
        }

        static private void GenerateBookmarks(JumpToSectionPopup popup, PDFReadingControl pdf_reading_control, AugmentedPdfLoadedDocument doc, PdfBookmarkBase bookmark_base, int depth)
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
                    WPFDoEvents.InvokeInUIThread(() =>
                    {
                        popup.Children.Add(new JumpToSectionItem(popup, pdf_reading_control, bookmark.Title, page_number + 1));
                    });
                }

                GenerateBookmarks(popup, pdf_reading_control, doc, bookmark, depth + 1);
            }
        }
    }
}

