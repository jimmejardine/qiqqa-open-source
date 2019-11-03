using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.LibraryCatalog;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Common.GUI
{
    public static class ListFormattingTools
    {
        //
        // Warning CA1001  Implement IDisposable on 'ListFormattingTools::DocumentTextBlockTag' because it creates 
        // members of the following IDisposable types: 'LibraryIndexHoverPopup'. 
        // If 'ListFormattingTools::DocumentTextBlockTag' has previously shipped, adding new members that implement 
        // IDisposable to this type is considered a breaking change to existing consumers.
        //
        // Note from GHO: that object is already managed through the sequence of tooltip_open and tooltip_close 
        // handlers below and is currently not considered a memory leak risk for https://github.com/jimmejardine/qiqqa-open-source/issues/19
        // and there-abouts.

        public class DocumentTextBlockTag
        {
            public PDFDocument pdf_document;
            public Feature feature;
            public object additional_tag;
            public LibraryIndexHoverPopup library_index_hover_popup = null;
        }

        public static TextBlock GetDocumentTextBlock(PDFDocument pdf_document, ref bool alternator, Feature feature, MouseButtonEventHandler mouse_down = null, string prefix = "", object additional_tag = null)
        {
            string header = GetPDFDocumentDescription(pdf_document, null);

            // If they have not given us a mouse down event handler, then just open the PDF
            if (null == mouse_down)
            {
                mouse_down = text_doc_MouseDown;
            }

            TextBlock text_doc = new TextBlock();
            text_doc.Text = prefix + header;
            text_doc.Tag = new DocumentTextBlockTag { pdf_document = pdf_document, feature = feature, additional_tag = additional_tag };
            text_doc.Cursor = Cursors.Hand;
            text_doc.MouseLeftButtonUp += mouse_down;
            text_doc.MouseRightButtonUp += text_doc_MouseRightButtonUp;

            text_doc.ToolTip = "";
            text_doc.ToolTipClosing += PDFDocumentNodeContentControl_ToolTipClosing;
            text_doc.ToolTipOpening += PDFDocumentNodeContentControl_ToolTipOpening;

            alternator = !alternator;

            text_doc.Background = Brushes.Transparent;
            AddGlowingHoverEffect(text_doc);

            return text_doc;
        }

        public static string GetPDFDocumentDescription(PDFDocument pdf_document, string prefix)
        {
            StringBuilder sb = new StringBuilder();
            {
                if (!String.IsNullOrEmpty(prefix))
                {
                    sb.Append(prefix);
                    sb.Append(' ');
                }

                if (null != pdf_document)
                {
                    string year = pdf_document.YearCombined;
                    if (Constants.UNKNOWN_YEAR != year)
                    {
                        sb.Append("(" + year + ") ");
                    }

                    sb.Append(pdf_document.TitleCombined);

                    string authors = pdf_document.AuthorsCombined;
                    if (Constants.UNKNOWN_AUTHORS != authors)
                    {
                        sb.Append(" by " + authors);
                    }
                }

                else
                {
                    Logging.Warn("ListFormattingTools.GetDocumentTextBlock can not process a null PDFDocument.");
                    sb.Append("<null>");
                }
            }

            return sb.ToString();
        }

        private static void text_doc_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock text_block = (TextBlock)sender;
            DocumentTextBlockTag tag = (DocumentTextBlockTag)text_block.Tag;

            LibraryCatalogPopup popup = new LibraryCatalogPopup(new List<PDFDocument> { tag.pdf_document });
            popup.Open();

            e.Handled = true;
        }

        private static void text_doc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock text_block = (TextBlock)sender;
            DocumentTextBlockTag tag = (DocumentTextBlockTag)text_block.Tag;

            if (null != tag.feature)
            {
                FeatureTrackingManager.Instance.UseFeature(tag.feature);
            }

            MainWindowServiceDispatcher.Instance.OpenDocument(tag.pdf_document);

            e.Handled = true;
        }

        private static void PDFDocumentNodeContentControl_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            TextBlock text_block = (TextBlock)sender;
            DocumentTextBlockTag tag = (DocumentTextBlockTag)text_block.Tag;

            try
            {
                if (null == tag.library_index_hover_popup)
                {
                    tag.library_index_hover_popup = new LibraryIndexHoverPopup();
                    tag.library_index_hover_popup.SetPopupContent(tag.pdf_document, 1);
                    text_block.ToolTip = tag.library_index_hover_popup;
                }
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "Exception while displaying document preview popup");
            }
        }

        private static void PDFDocumentNodeContentControl_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            TextBlock text_block = (TextBlock)sender;
            DocumentTextBlockTag tag = (DocumentTextBlockTag)text_block.Tag;

            tag.library_index_hover_popup?.Dispose();
            tag.library_index_hover_popup = null;

            text_block.ToolTip = "";
        }

        public static void AddGlowingHoverEffect(FrameworkElement fe)
        {
            fe.MouseEnter += AddGlowingHoverEffect_MouseEnter;
            fe.MouseLeave += AddGlowingHoverEffect_MouseLeave;
        }

        private static void AddGlowingHoverEffect_MouseLeave(object sender, MouseEventArgs e)
        {
            {
                TextBlock o = sender as TextBlock;
                if (null != o) o.Background = Brushes.Transparent;
            }
            {
                Panel o = sender as Panel;
                if (null != o) o.Background = Brushes.Transparent;
            }
        }

        private static void AddGlowingHoverEffect_MouseEnter(object sender, MouseEventArgs e)
        {
            {
                TextBlock o = sender as TextBlock;
                if (null != o) o.Background = ThemeColours.Background_Brush_Blue_LightToVeryLight;
            }
            {
                Panel o = sender as Panel;
                if (null != o) o.Background = ThemeColours.Background_Brush_Blue_LightToVeryLight;
            }
        }
    }
}
