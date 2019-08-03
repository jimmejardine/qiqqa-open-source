using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.TagExplorerStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Expedition;
using Utilities;
using Utilities.Collections;

namespace Qiqqa.DocumentLibrary.LibraryFilter.GeneralExplorers
{
    /// <summary>
    /// Interaction logic for ThemeExplorerControl.xaml
    /// </summary>
    public partial class ThemeExplorerControl : UserControl
    {
        private Library library;

        public delegate void OnTagSelectionChangedDelegate(HashSet<string> fingerprints, Span descriptive_span);
        public event OnTagSelectionChangedDelegate OnTagSelectionChanged;

        public ThemeExplorerControl()
        {
            InitializeComponent();

            this.ToolTip = "Here are the Themes in your documents.  If you see no themes here, please run Expedition.  " + GenericLibraryExplorerControl.YOU_CAN_FILTER_TOOLTIP;

            ThemeExplorerTree.DescriptionTitle = "Theme";

            ThemeExplorerTree.GetNodeItems = GetNodeItems;

            ThemeExplorerTree.OnTagSelectionChanged += TagExplorerTree_OnTagSelectionChanged;

            HypRunExpedition.Click += HypRunExpedition_Click;
        }

        void HypRunExpedition_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenExpedition(library);
        }

        // -----------------------------

        public Library Library
        {
            set
            {
                this.library = value;
                ThemeExplorerTree.Library = value;
            }
        }

        public void Reset()
        {
            ThemeExplorerTree.Reset();
        }

        // -----------------------------

        MultiMapSet<string, string> GetNodeItems(Library library, HashSet<string> parent_fingerprints)
        {
            MultiMapSet<string, string> results = GetNodeItems_STATIC(library, parent_fingerprints);

            // Show the no themes message
            {
                bool have_topics =
                    true
                    && null != library.ExpeditionManager
                    && null != library.ExpeditionManager.ExpeditionDataSource
                    && 0 < library.ExpeditionManager.ExpeditionDataSource.LDAAnalysis.NUM_TOPICS
                    ;

                TxtNoThemesMessage.Visibility = 0 == results.Count ? Visibility.Visible : Visibility.Collapsed;
            }

            return results;
        }
        
        public static MultiMapSet<string, string> GetNodeItems_STATIC(Library library, HashSet<string> parent_fingerprints)
        {
            MultiMapSet<string, string> results = new MultiMapSet<string, string>();

            try
            {
                // Check that expedition has been run...
                if (null == library.ExpeditionManager || null == library.ExpeditionManager.ExpeditionDataSource)
                {
                    return results;
                }

                ExpeditionDataSource eds = library.ExpeditionManager.ExpeditionDataSource;
                for (int t = 0; t < eds.LDAAnalysis.NUM_TOPICS; ++t)
                {
                    string topic_name = eds.GetDescriptionForTopic(t, false, "; ");

                    // Show the top % of docs
                    int num_docs = eds.LDAAnalysis.NUM_DOCS / 10;
                    num_docs = Math.Max(num_docs, 3);

                    for (int d = 0; d < eds.LDAAnalysis.NUM_DOCS && d < num_docs; ++d)
                    {
                        PDFDocument pdf_document = library.GetDocumentByFingerprint(eds.docs[eds.LDAAnalysis.DensityOfDocsInTopicsSorted[t][d].doc]);
                        if (null == parent_fingerprints || parent_fingerprints.Contains(pdf_document.Fingerprint))
                        {
                            results.Add(topic_name, pdf_document.Fingerprint);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem while loading the themes for the library explorer.");
            }

            return results;
        }

        void TagExplorerTree_OnTagSelectionChanged(HashSet<string> fingerprints, Span descriptive_span)
        {
            if (null != OnTagSelectionChanged)
            {
                OnTagSelectionChanged(fingerprints, descriptive_span);
            }
        }
    }
}
