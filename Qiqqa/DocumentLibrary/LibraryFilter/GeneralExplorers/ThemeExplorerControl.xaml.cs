using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.TagExplorerStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Expedition;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.Mathematics.Topics.LDAStuff;
using Utilities.Misc;

namespace Qiqqa.DocumentLibrary.LibraryFilter.GeneralExplorers
{
    /// <summary>
    /// Interaction logic for ThemeExplorerControl.xaml
    /// </summary>
    public partial class ThemeExplorerControl : UserControl
    {
        private WebLibraryDetail web_library_detail;

        public delegate void OnTagSelectionChangedDelegate(HashSet<string> fingerprints, Span descriptive_span);
        public event OnTagSelectionChangedDelegate OnTagSelectionChanged;

        public ThemeExplorerControl()
        {
            InitializeComponent();

            ToolTip = "Here are the Themes in your documents.  If you see no themes here, please run Expedition.  " + GenericLibraryExplorerControl.YOU_CAN_FILTER_TOOLTIP;

            ThemeExplorerTree.DescriptionTitle = "Theme";

            ThemeExplorerTree.GetNodeItems = GetNodeItems;

            ThemeExplorerTree.OnTagSelectionChanged += TagExplorerTree_OnTagSelectionChanged;

            HypRunExpedition.Click += HypRunExpedition_Click;
        }

        private void HypRunExpedition_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenExpedition(web_library_detail);
        }

        // -----------------------------

        public WebLibraryDetail LibraryRef
        {
            get => web_library_detail;
            set
            {
                web_library_detail = value;
                ThemeExplorerTree.LibraryRef = value;
            }
        }

        public void Reset()
        {
            ThemeExplorerTree.Reset();
        }

        // -----------------------------

        private void UpdateVisibility(bool has_themes)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            TxtNoThemesMessage.Visibility = !has_themes ? Visibility.Visible : Visibility.Collapsed;
        }

        public static MultiMapSet<string, string> GetNodeItems(WebLibraryDetail web_library_detail, HashSet<string> parent_fingerprints)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            MultiMapSet<string, string> results = new MultiMapSet<string, string>();

            try
            {
                // Check that expedition has been run...
                    ExpeditionDataSource eds = web_library_detail.Xlibrary?.ExpeditionManager?.ExpeditionDataSource;
                if (eds != null)
                {
                    HashSet<string> pdf_doc_fingerprints = web_library_detail.Xlibrary.GetAllDocumentFingerprints();

                    LDAAnalysis lda = eds.LDAAnalysis;

                    for (int t = 0; t < lda.NUM_TOPICS; ++t)
                    {
                        string topic_name = eds.GetDescriptionForTopic(t, include_topic_number: false, "; ");

                        // Show the top % of docs
                        int num_docs = lda.NUM_DOCS / 10;
                        num_docs = Math.Max(num_docs, 3);

                        for (int d = 0; d < lda.NUM_DOCS && d < num_docs; ++d)
                        {
                            DocProbability[] docs = lda.DensityOfDocsInTopicsSorted[t];
                            ASSERT.Test(docs != null);
                            ASSERT.Test(docs.Length == lda.NUM_DOCS);
                            DocProbability prob = docs[d];
                            ASSERT.Test(prob != null);
                            ASSERT.Test(prob.doc >= 0);
                            ASSERT.Test(prob.doc < lda.NUM_DOCS);
                            ASSERT.Test(prob.doc < eds.docs.Count);

                            string fingerprint_to_look_for = eds.docs[prob.doc];
                            if (fingerprint_to_look_for == null || !pdf_doc_fingerprints.Contains(fingerprint_to_look_for))
                            {
                                Logging.Warn("ThemeExplorer: Cannot find document anymore for fingerprint {0}", fingerprint_to_look_for);
                            }
                            else if (null == parent_fingerprints || parent_fingerprints.Contains(fingerprint_to_look_for))
                            {
                                results.Add(topic_name, fingerprint_to_look_for);
                            }
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

        private void TagExplorerTree_OnTagSelectionChanged(HashSet<string> fingerprints, Span descriptive_span)
        {
            OnTagSelectionChanged?.Invoke(fingerprints, descriptive_span);
        }
    }
}
