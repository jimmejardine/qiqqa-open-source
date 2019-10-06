using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Controls;
using System.Windows.Media;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.DocumentLibraryIndex;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;
using Utilities.Language.TextIndexing;

namespace Qiqqa.DocumentLibrary.CrossLibrarySearchStuff
{
    /// <summary>
    /// Interaction logic for CrossLibrarySearchControl.xaml
    /// </summary>
    public partial class CrossLibrarySearchControl : UserControl
    {
        public CrossLibrarySearchControl()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(ObjImage, BitmapScalingMode.HighQuality);
            ObjImage.Source = Icons.GetAppIcon(Icons.Search);

            SearchQuick.OnHardSearch += SearchQuick_OnHardSearch;
            SearchQuick.SearchHistoryItemSource = ConfigurationManager.Instance.SearchHistory;
        }

        void SearchQuick_OnHardSearch()
        {
            DoSearch();
        }

        public void DoSearch(string query)
        {
            SearchQuick.Text = query;
            DoSearch();
        }

        class CombinedSearchResultItem
        {
            public string fingerprint;
            public double score;
            public PDFDocument pdf_document;
        }

        public void DoSearch()
        {
            string query = SearchQuick.Text;
            
            // Do we have anything to do?
            if (String.IsNullOrEmpty(query))
            {   
                SystemSounds.Beep.Play();
                return;
            }

            // Search each library
            List<CombinedSearchResultItem> results = new List<CombinedSearchResultItem>();
            foreach (WebLibraryDetail web_library_detail in WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries_All)
            {
                Logging.Info("Searching library {0}", web_library_detail.Title);
                List<IndexResult> index_results = LibrarySearcher.FindAllFingerprintsMatchingQuery(web_library_detail.library, query);

                foreach (IndexResult index_result in index_results)                
                {
                    PDFDocument pdf_document = web_library_detail.library.GetDocumentByFingerprint(index_result.fingerprint);
                    if (null != pdf_document)
                    {
                        CombinedSearchResultItem result = new CombinedSearchResultItem
                        {
                            fingerprint = index_result.fingerprint,
                            score = index_result.score,
                            pdf_document = pdf_document
                        };                        
                        results.Add(result);
                    }
                    else
                    {
                        Logging.Debug特("Received a null document from library search?! (Fingerprint: {0})", index_result.fingerprint);
                    }
                }
            }

            // Sort the results
            results.Sort(delegate(CombinedSearchResultItem p1, CombinedSearchResultItem p2) { return -Sorting.Compare(p1.score, p2.score); });

            // Create the ordered results
            List<PDFDocument> pdf_documents = new List<PDFDocument>();
            Dictionary<string, double> search_scores = new Dictionary<string, double>();
            foreach (CombinedSearchResultItem result in results)
            {
                pdf_documents.Add(result.pdf_document);
                search_scores[result.fingerprint] = result.score;
            }

            ObjLibraryCatalog.SetPDFDocuments(pdf_documents, null, query, search_scores);
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            CrossLibrarySearchControl control = new CrossLibrarySearchControl();
            ControlHostingWindow w = new ControlHostingWindow("Cross library search", control);
            w.Show();
        }
#endif

        #endregion
    }
}
