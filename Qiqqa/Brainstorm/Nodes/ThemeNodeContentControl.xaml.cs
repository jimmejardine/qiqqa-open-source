using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.Brainstorm.SceneManager;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.Expedition;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Mathematics.Topics.LDAStuff;
using Utilities.Reflection;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for ThemeNodeContentControl.xaml
    /// </summary>
    public partial class ThemeNodeContentControl : UserControl, IKeyPressableNodeContentControl
    {
        private NodeControl node_control;
        private AugmentedBindable<ThemeNodeContent> theme_node_content;

        public ThemeNodeContentControl(NodeControl node_control_, ThemeNodeContent theme_node_content)
        {
            InitializeComponent();

            node_control = node_control_;
            this.theme_node_content = new AugmentedBindable<ThemeNodeContent>(theme_node_content);

            DataContextChanged += ThemeNodeContentControl_DataContextChanged;
            DataContext = this.theme_node_content;

            Focusable = true;

            ImageIcon.Source = Icons.GetAppIcon(Icons.BrainstormAttractorTheme);
            RenderOptions.SetBitmapScalingMode(ImageIcon, BitmapScalingMode.HighQuality);

            TextBorder.CornerRadius = NodeThemes.corner_radius;
        }

        public void ProcessKeyPress(KeyEventArgs e)
        {
            if (Key.S == e.Key)
            {
                ExpandSpecificDocuments();
                e.Handled = true;
            }
            else if (Key.D == e.Key)
            {
                ExpandInfluentialDocuments();
                e.Handled = true;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void ThemeNodeContentControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ApplyTagsDistribution(ColourNodeBackground);
        }

        internal void ColourNodeBackground(NodeControl node_control_, WebLibraryDetail web_library_detail, ExpeditionDataSource eds, float[] tags_distribution)
        {
            TextBorder.Opacity = 0.8;
            TextBorder.Background = ThemeBrushes.GetBrushForDistribution(web_library_detail, tags_distribution.Length, tags_distribution);
            if (ThemeBrushes.UNKNOWN_BRUSH == TextBorder.Background)
            {
                TextBorder.Background = NodeThemes.background_brush;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        internal void ExpandInfluentialDocuments()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Theme_DocumentsInfluential);
            ApplyTagsDistribution(AddDocumentsInfluentialInDistribution);
        }

        internal static void AddDocumentsInfluentialInDistribution(NodeControl node_control_, WebLibraryDetail web_library_detail, ExpeditionDataSource eds, float[] tags_distribution)
        {
            Logging.Info("+Performing ThemedPageRank on {0} documents", eds.LDAAnalysis.NUM_DOCS);

            // We have the distribution of the topic in tags_distribution

            // Create an array for the document biases
            // Fill the array using the dot product of the document distribution dotted with the topic distribution - then normalise
            double[] biases = new double[eds.LDAAnalysis.NUM_DOCS];
            for (int doc = 0; doc < eds.LDAAnalysis.NUM_DOCS; ++doc)
            {
                double bias_num_squared = 0;
                double bias_den_doc = 0;
                double bias_den_tags = 0;

                for (int topic = 0; topic < eds.LDAAnalysis.NUM_TOPICS; ++topic)
                {
                    bias_num_squared += eds.LDAAnalysis.DensityOfTopicsInDocuments[doc, topic] * tags_distribution[topic];
                    bias_den_doc += eds.LDAAnalysis.DensityOfTopicsInDocuments[doc, topic] * eds.LDAAnalysis.DensityOfTopicsInDocuments[doc, topic];
                    bias_den_tags += tags_distribution[topic] * tags_distribution[topic];
                }

                biases[doc] = bias_num_squared / (Math.Sqrt(bias_den_doc) * Math.Sqrt(bias_den_tags));
            }

            // Then build up a matrix FROM each document -
            List<int>[] references_outbound = new List<int>[eds.LDAAnalysis.NUM_DOCS];
            for (int doc = 0; doc < eds.LDAAnalysis.NUM_DOCS; ++doc)
            {
                references_outbound[doc] = new List<int>();

                string fingerprint = eds.docs[doc];
                PDFDocument pdf_document = web_library_detail.Xlibrary.GetDocumentByFingerprint(fingerprint);
                if (null == pdf_document)
                {
                    Logging.Warn("ThemeExplorer::AddInInfluential: Cannot find document anymore for fingerprint {0}", fingerprint);
                }
                else
                {
                    List<Citation> citations_outbound = pdf_document.PDFDocumentCitationManager.GetOutboundCitations();
                    foreach (Citation citation in citations_outbound)
                    {
                        string fingerprint_inbound = citation.fingerprint_inbound;
                        if (eds.docs_index.ContainsKey(fingerprint_inbound))
                        {
                            int doc_inbound = eds.docs_index[fingerprint_inbound];
                            references_outbound[doc].Add(doc_inbound);
                        }
                    }
                }
            }

            // Space for the pageranks
            double[] pageranks_current = new double[eds.LDAAnalysis.NUM_DOCS];
            double[] pageranks_next = new double[eds.LDAAnalysis.NUM_DOCS];

            // Initialise
            for (int doc = 0; doc < eds.LDAAnalysis.NUM_DOCS; ++doc)
            {
                pageranks_current[doc] = biases[doc];
            }

            // Iterate
            int NUM_ITERATIONS = 20;
            for (int iteration = 0; iteration < NUM_ITERATIONS; ++iteration)
            {
                Logging.Info("Performing ThemedPageRank iteration {0}", iteration);

                // Spread out the activation pageranks
                for (int doc = 0; doc < eds.LDAAnalysis.NUM_DOCS; ++doc)
                {
                    foreach (int doc_inbound in references_outbound[doc])
                    {
                        pageranks_next[doc_inbound] += biases[doc] / references_outbound[doc].Count;
                    }
                }

                // Mix the spread out pageranks with the initial bias pageranks
                double ALPHA = 0.5;
                for (int doc = 0; doc < eds.LDAAnalysis.NUM_DOCS; ++doc)
                {
                    pageranks_next[doc] = (1 - ALPHA) * pageranks_next[doc] + ALPHA * biases[doc];
                }

                // Normalise the next pageranks
                double total = 0;
                for (int doc = 0; doc < eds.LDAAnalysis.NUM_DOCS; ++doc)
                {
                    total += pageranks_next[doc];
                }
                if (0 < total)
                {
                    for (int doc = 0; doc < eds.LDAAnalysis.NUM_DOCS; ++doc)
                    {
                        pageranks_next[doc] /= total;
                    }
                }

                // Switch in the next pageranks because we will overwrite them
                double[] pageranks_temp = pageranks_current;
                pageranks_current = pageranks_next;
                pageranks_next = pageranks_temp;
            }

            // Sort the pageranks, descending
            int[] docs = new int[eds.LDAAnalysis.NUM_DOCS];
            for (int doc = 0; doc < eds.LDAAnalysis.NUM_DOCS; ++doc)
            {
                docs[doc] = doc;
            }
            Array.Sort(pageranks_current, docs);
            Array.Reverse(pageranks_current);
            Array.Reverse(docs);

            // Make the nodes
            for (int doc = 0; doc < 10 && doc < docs.Length; ++doc)
            {
                int doc_id = docs[doc];
                string fingerprint = eds.docs[doc_id];

                PDFDocument pdf_document = web_library_detail.Xlibrary.GetDocumentByFingerprint(fingerprint);
                if (null == pdf_document)
                {
                    Logging.Warn("Couldn't find similar document with fingerprint {0}", fingerprint);
                }
                else
                {
                    PDFDocumentNodeContent content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.LibraryRef.Id);
                    NodeControlAddingByKeyboard.AddChildToNodeControl(node_control_, content, false);
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        internal void ExpandSpecificDocuments()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Theme_Documents);
            ApplyTagsDistribution(AddDocumentsSimilarToDistribution);
        }

        internal static void AddDocumentsSimilarToDistribution(NodeControl node_control_, WebLibraryDetail web_library_detail, ExpeditionDataSource eds, float[] tags_distribution)
        {
            // Get the most similar PDFDocuments
            int[] doc_ids = LDAAnalysisTools.GetDocumentsSimilarToDistribution(eds.LDAAnalysis, tags_distribution);

            for (int i = 0; i < 10 && i < doc_ids.Length; ++i)
            {
                int doc_id = doc_ids[i];
                string fingerprint = eds.docs[doc_id];

                PDFDocument pdf_document = web_library_detail.Xlibrary.GetDocumentByFingerprint(fingerprint);
                if (null == pdf_document)
                {
                    Logging.Warn("Couldn't find similar document with fingerprint {0}", fingerprint);
                }
                else
                {
                    PDFDocumentNodeContent content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.LibraryRef.Id);
                    NodeControlAddingByKeyboard.AddChildToNodeControl(node_control_, content, false);
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void ApplyTagsDistribution(DistributionUseDelegate distribution_use)
        {
            // Get the distribution for the themes
            string tags = theme_node_content.Underlying.Tags;
            string[] tags_array = tags.Split('\n');

            string library_fingerprint = theme_node_content.Underlying.library_fingerprint;
            WebLibraryDetail web_library_detail = WebLibraryManager.Instance.GetLibrary(library_fingerprint);
            if (null == web_library_detail)
            {
                Logging.Warn("Unable to locate library " + library_fingerprint);
                return;
            }

            if (null == web_library_detail.Xlibrary.ExpeditionManager || null == web_library_detail.Xlibrary.ExpeditionManager.ExpeditionDataSource)
            {
                Logging.Warn("Expedition has not been run for library '{0}'.", web_library_detail.Title);
                return;
            }

            ExpeditionDataSource eds = web_library_detail.Xlibrary.ExpeditionManager.ExpeditionDataSource;

            float[] tags_distribution = new float[eds.LDAAnalysis.NUM_TOPICS];
            int tags_distribution_denom = 0;
            foreach (string tag in tags_array)
            {
                if (eds.words_index.ContainsKey(tag))
                {
                    ++tags_distribution_denom;

                    int tag_id = eds.words_index[tag];
                    for (int topic_i = 0; topic_i < eds.LDAAnalysis.NUM_TOPICS; ++topic_i)
                    {
                        tags_distribution[topic_i] += eds.LDAAnalysis.PseudoDensityOfTopicsInWords[tag_id, topic_i];
                    }
                }
                else
                {
                    Logging.Warn("Ignoring tag {0} which we don't recognise.", tag);
                }
            }

            if (0 < tags_distribution_denom)
            {
                // Normalise the tags distribution
                for (int topic_i = 0; topic_i < eds.LDAAnalysis.NUM_TOPICS; ++topic_i)
                {
                    tags_distribution[topic_i] /= tags_distribution_denom;
                }
            }

            distribution_use(node_control, web_library_detail, eds, tags_distribution);
        }

        private delegate void DistributionUseDelegate(NodeControl node_control_, WebLibraryDetail web_library_detail, ExpeditionDataSource eds, float[] tags_distribution);

    }
}

