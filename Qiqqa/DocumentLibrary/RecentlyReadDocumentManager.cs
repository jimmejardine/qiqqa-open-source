using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common;
using Qiqqa.Documents.PDF;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary
{
    public static class RecentlyReadDocumentManager
    {
        public static AugmentedPopup GetRecentlyReadPopupMenu(Library library)
        {
            List<PDFDocument> pdf_documents_all = library.PDFDocuments;

            var most_recently_read_pdf_documents_INTERIM =
                from pdf_document in pdf_documents_all
                orderby pdf_document.DateLastRead descending
                where pdf_document.Deleted == false
                && pdf_document.DateLastRead.HasValue
                select pdf_document;

            List<PDFDocument> most_recently_read_pdf_documents = new List<PDFDocument>(most_recently_read_pdf_documents_INTERIM);

            StackPanel popup_panel = new StackPanel();
            for (int i = 0; i < 10 && i < most_recently_read_pdf_documents.Count; ++i)
            {
                PDFDocument pdf_document = most_recently_read_pdf_documents[i];

                MenuItem menu_item = new MenuItem();
                menu_item.Header = String.Format("{0} {1} by {2}", pdf_document.YearCombined, pdf_document.TitleCombined, pdf_document.AuthorsCombined);
                menu_item.Tag = pdf_document;
                menu_item.Click += menu_item_Click;
                popup_panel.Children.Add(menu_item);
            }

            AugmentedPopup popup = new AugmentedPopup(popup_panel);
            return popup;
        }

        private static void menu_item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu_item = (MenuItem)sender;
            PDFDocument pdf_document = (PDFDocument)menu_item.Tag;
            MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
        }
    }
}
