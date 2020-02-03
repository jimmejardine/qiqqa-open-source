using System;
using System.Text;
using System.Windows.Input;
using Utilities;

namespace Qiqqa.Documents.PDF.PDFControls.BookmarkStuff
{
    public static class BookmarkManager
    {
        private static double[] FromStringRepresentation(string src)
        {
            double[] results = new double[10];

            // Check that we have something to work with
            if (!String.IsNullOrEmpty(src))
            {
                // Split into parts
                string[] src_parts = src.Split(',');
                for (int i = 0; i < 10 && i < src_parts.Length; ++i)
                {
                    try
                    {
                        results[i] = Convert.ToDouble(src_parts[i], Internationalization.DEFAULT_CULTURE);
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "Skipping problem while restoring bookmark {0} in {1}", i, src);
                    }
                }
            }

            return results;
        }

        private static string ToStringRepresentation(double[] src)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 10; ++i)
            {
                if (0 != i) sb.Append(',');
                sb.AppendFormat(Internationalization.DEFAULT_CULTURE, "{0:0.00000}", src[i]);
            }

            return sb.ToString();
        }

        public static void SetDocumentBookmark(PDFDocument pdf_document, int bookmark, double position)
        {
            double[] bookmarks = FromStringRepresentation(pdf_document.Bookmarks);
            bookmarks[bookmark] = position;
            pdf_document.Bookmarks = ToStringRepresentation(bookmarks);
            pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Bookmarks));
        }

        public static double[] GetDocumentBookmarks(PDFDocument pdf_document)
        {
            double[] bookmarks = FromStringRepresentation(pdf_document.Bookmarks);
            return bookmarks;
        }

        public static double GetDocumentBookmark(PDFDocument pdf_document, int bookmark_number)
        {
            return GetDocumentBookmarks(pdf_document)[bookmark_number];
        }

        public static int KeyToBookmarkNumber(Key key)
        {
            switch (key)
            {
                case Key.D0: return 0;
                case Key.D1: return 1;
                case Key.D2: return 2;
                case Key.D3: return 3;
                case Key.D4: return 4;
                case Key.D5: return 5;
                case Key.D6: return 6;
                case Key.D7: return 7;
                case Key.D8: return 8;
                case Key.D9: return 9;
                default:
                    Logging.Warn("Have been asked for bookmark for unknown key " + key);
                    return 0;
            }

        }
    }
}
