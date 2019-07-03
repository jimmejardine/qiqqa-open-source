using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common.GUI;
using Qiqqa.Documents.Common;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Misc;
using Utilities.Strings;

namespace Qiqqa.Documents.PDF.InfoBarStuff.DuplicateDetectionStuff
{
    /// <summary>
    /// Interaction logic for DuplicateDetectionControl.xaml
    /// </summary>
    public partial class DuplicateDetectionControl : UserControl
    {
        public DuplicateDetectionControl()
        {
            InitializeComponent();

        }

        public PDFDocument PDFDocument
        {
            set
            {                
                SafeThreadPool.QueueUserWorkItem(o => FindDuplicates(value));                
            }
        }


        // This is used to cache the TitlesCombined
        public class TitleCombinedCacheEntry
        {
            public PDFDocument pdf_document;
            public string title_combined;
        }

        public class TitleCombinedCache
        {
            List<TitleCombinedCacheEntry> entries;

            public TitleCombinedCache(List<PDFDocument> pdf_documents)
            {
                entries = new List<TitleCombinedCacheEntry>();
                foreach (var pdf_document in pdf_documents)
                {
                    entries.Add(new TitleCombinedCacheEntry { pdf_document = pdf_document, title_combined = pdf_document.TitleCombined.ToLower() });
                }
            }

            public List<TitleCombinedCacheEntry> Entries
            {
                get
                {
                    return entries;
                }
            }
        }



        private void FindDuplicates(PDFDocument pdf_document_this)
        {
            // Invoke the GUI
            this.Dispatcher.Invoke(new Action(() =>
            {
                ClearDuplicates();
            }
            ));

            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (null == pdf_document_this)
            {
                Logging.Info("Not doing duplicate detection if we don't have a PDFDocument to work with.");
                return;
            }

            string title_this = pdf_document_this.TitleCombined.ToLower();
            if (PDFDocument.TITLE_UNKNOWN == title_this)
            {
                Logging.Info("Not doing duplicate detection for an unknown title.");
                return;
            }

            // Find all the document in the library that have the same title as this document's
            TitleCombinedCache cache = new TitleCombinedCache(pdf_document_this.Library.PDFDocuments);
            List<PDFDocument> duplicate_pdf_documents = FindDuplicates(pdf_document_this, cache);
            Logging.Info("It took {0}ms to run the duplicate detection.", stopwatch.ElapsedMilliseconds);

            // Invoke the GUI
            this.Dispatcher.Invoke(new Action(() =>
            {
                RenderDuplicates(duplicate_pdf_documents);
            }
            ));
        }

        public static List<PDFDocument> FindDuplicates(PDFDocument pdf_document, TitleCombinedCache cache)
        {
            List<PDFDocument> duplicate_pdf_documents = new List<PDFDocument>();

            // If a document is already marked as duplicate, then it doesn nt have any duplicates
            if (Choices.ReadingStages_DUPLICATE == pdf_document.ReadingStage)
            {
                return duplicate_pdf_documents;
            }

            string title_this = pdf_document.TitleCombined.ToLower();
            if (title_this.Length < 128)
            {
                if (PDFDocument.TITLE_UNKNOWN != title_this)
                {
                    foreach (var entry in cache.Entries)
                    {
                        // Don't match the document to itself
                        if (entry.pdf_document == pdf_document)
                        {
                            continue;
                        }

                        // Don't match the document to duplicates
                        if (Choices.ReadingStages_DUPLICATE == entry.pdf_document.ReadingStage)
                        {
                            continue;
                        }

                        // Check if we are almost similar to the other document
                        string title_other = entry.title_combined;
                        if (PDFDocument.TITLE_UNKNOWN != title_other)
                        {
                            if ((LetterPairSimilarity.CompareStrings(title_this, title_other) > 0.95) && (StringTools.LewensteinSimilarity(title_this, title_other) > 0.9))
                            {
                                duplicate_pdf_documents.Add(entry.pdf_document);
                            }
                        }
                    }
                }
            }
            
            return duplicate_pdf_documents;
        }

        private void ClearDuplicates()
        {
            DocsPanel.Children.Clear();
        }
        
        private void RenderDuplicates(List<PDFDocument> duplicate_pdf_documents)
        {
            bool alternator = false;
            foreach (PDFDocument duplicate_pdf_document in duplicate_pdf_documents)
            {
                TextBlock text_doc = ListFormattingTools.GetDocumentTextBlock(duplicate_pdf_document, ref alternator, Features.DuplicateDetection_OpenDoc);
                DocsPanel.Children.Add(text_doc);
            }

            // If the panel is empty, put NONE
            if (0 == DocsPanel.Children.Count)
            {
                DocsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                DocsPanel.Visibility = Visibility.Visible;
            }
        }
    }
}
