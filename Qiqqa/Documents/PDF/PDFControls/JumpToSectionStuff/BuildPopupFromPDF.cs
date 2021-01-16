using System;
#if SYNCFUSION_ANTIQUE
using Syncfusion.Pdf.Interactive;
#endif
using Utilities;
using Utilities.PDF;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    internal class BuildPopupFromPDF
    {
        private JumpToSectionPopup popup;

        internal BuildPopupFromPDF(JumpToSectionPopup popup)
        {
            this.popup = popup;
        }

#if SYNCFUSION_ANTIQUE
        internal void BuildMenu()
        {
            try
            {
                using (AugmentedPdfLoadedDocument doc = new AugmentedPdfLoadedDocument(popup.pdf_renderer_control_stats.pdf_document.DocumentPath))
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
                    popup.Children.Add(new JumpToSectionItem(popup, popup.pdf_reading_control, popup.pdf_render_control, popup.pdf_renderer_control_stats, bookmark.Title, page_number + 1));
                }

                GenerateBookmarks(doc, bookmark, depth + 1);
            }
        }
#endif
    }
}


/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.PDF;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    internal class BuildPopupFromPDF
    {
        JumpToSectionPopup popup;

        internal BuildPopupFromPDF(JumpToSectionPopup popup)
        {
            this.popup = popup;
        }

        internal void BuildMenu()
        {
            try
            {
                using (PdfDocument doc = PdfReader.Open(popup.pdf_renderer_control_stats.pdf_document.DocumentPath, PdfDocumentOpenMode.ReadOnly))
                {
                    if (null != doc.Outlines)
                    {
                        GenerateBookmarks(doc, doc.Outlines, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem while generating bookmarks from the PDF file");
            }
        }

        private void GenerateBookmarks(PdfDocument doc, PdfOutline.PdfOutlineCollection pdf_outlines, int depth)
        {
            if (null == pdf_outlines)
            {
                return;
            }

            // Don't go too deep in the bookmark hierarchy
            if (depth > 0)
            {
                return;
            }

            for (int i = 0; i < pdf_outlines.Count; ++i)
            {
                PdfOutline bookmark = pdf_outlines[i];
                int page_number = -1;
                if (null != bookmark.DestinationPage)
                {
                    if (-1 == page_number)
                    {
                        for (int j = 0; j < doc.Pages.Count; ++j)
                        {
                            if (doc.Pages[j] == bookmark.DestinationPage)
                            {
                                page_number = j;
                                break;
                            }
                        }
                    }
                }

                if (-1 != page_number)
                {
                    popup.Children.Add(new JumpToSectionItem(popup, popup.pdf_reading_control, popup.pdf_render_control, popup.pdf_renderer_control_stats, bookmark.Title, page_number + 1));
                }

                GenerateBookmarks(doc, bookmark.Outlines, depth + 1);
            }
        }
    }
}


*/
