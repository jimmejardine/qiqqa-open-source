using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.Brainstorm.SceneManager;
using Qiqqa.Common;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.Expedition;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Language;
using Utilities.Mathematics.Topics.LDAStuff;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for DocumentNodeContentControl.xaml
    /// </summary>
    public partial class PDFDocumentNodeContentControl : UserControl, IKeyPressableNodeContentControl
    {
        NodeControl node_control;
        PDFDocumentNodeContent pdf_document_node_content;

        // TODO:
        //
        // Warning CA1001  Implement IDisposable on 'PDFAnnotationNodeContentControl' because it creates 
        // members of the following IDisposable types: 'LibraryIndexHoverPopup'. 
        // If 'PDFAnnotationNodeContentControl' has previously shipped, adding new members that implement 
        // IDisposable to this type is considered a breaking change to existing consumers.
        //
        // Note from GHO: that object is already managed through the sequence of tooltip_open and tooltip_close 
        // handlers below and is currently not considered a memory leak risk for https://github.com/jimmejardine/qiqqa-open-source/issues/19
        // and there-abouts.

        LibraryIndexHoverPopup library_index_hover_popup = null;

        public PDFDocumentNodeContentControl(NodeControl node_control, PDFDocumentNodeContent pdf_document_node_content)
        {
            InitializeComponent();

            this.node_control = node_control;
            this.pdf_document_node_content = pdf_document_node_content;

            this.DataContextChanged += PDFDocumentNodeContentControl_DataContextChanged;
            this.DataContext = pdf_document_node_content.PDFDocument.Bindable;

            this.Focusable = true;

            this.ImageIcon.Source = Icons.GetAppIcon(Icons.BrainstormDocument);

            ImageIcon.Opacity = 0.5;
            ImageIcon.Width = NodeThemes.image_width;
            TextBorder.CornerRadius = NodeThemes.corner_radius;
            TextBorder.Background = NodeThemes.background_brush;
            
            TextTitle.FontWeight = FontWeights.Bold;
            TextPublication.FontStyle = FontStyles.Italic;
            
            this.MouseDoubleClick += DocumentNodeContentControl_MouseDoubleClick;
            this.ToolTip = "";
            this.ToolTipClosing += PDFDocumentNodeContentControl_ToolTipClosing;
            this.ToolTipOpening += PDFDocumentNodeContentControl_ToolTipOpening;
        }

        void PDFDocumentNodeContentControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ObjThemeSwatch.Background = ThemeBrushes.GetBrushForDocument(pdf_document_node_content.PDFDocument);
            if (ThemeBrushes.UNKNOWN_BRUSH == TextBorder.Background)
            {
                ObjThemeSwatch.Background = Brushes.Transparent;
            }
        }

        private void ExpandRelevants()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Similars);

            List<ExpeditionPaperSuggestions.Result> results = ExpeditionPaperSuggestions.GetRelevantOthers(pdf_document_node_content.PDFDocument, 10);
            foreach (ExpeditionPaperSuggestions.Result result in results)
            {
                PDFDocumentNodeContent content = new PDFDocumentNodeContent(result.pdf_document.Fingerprint, result.pdf_document.Library.WebLibraryDetail.Id);
                NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
            }
        }
        
        private void ExpandSimilars()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Similars);

            if (null != pdf_document_node_content.PDFDocument.Library.ExpeditionManager)
            {
                ExpeditionDataSource eds = pdf_document_node_content.PDFDocument.Library.ExpeditionManager.ExpeditionDataSource;
                if (null != eds)
                {
                    if (eds.docs_index.ContainsKey(pdf_document_node_content.PDFDocument.Fingerprint))
                    {
                        int doc_id = eds.docs_index[pdf_document_node_content.PDFDocument.Fingerprint];
                        float[,] density_of_topics_in_docs = eds.LDAAnalysis.DensityOfTopicsInDocuments;

                        float[] distribution = new float[eds.LDAAnalysis.NUM_TOPICS];                
                        for (int topic_i = 0; topic_i < eds.LDAAnalysis.NUM_TOPICS; ++topic_i)
                        {
                            distribution[topic_i] = density_of_topics_in_docs[doc_id, topic_i];
                        }

                        ThemeNodeContentControl.AddDocumentsSimilarToDistribution(node_control, pdf_document_node_content.PDFDocument.Library, eds, distribution);
                    }
                }
            }
        }


        private void ExpandThemes()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Themes);

            bool added_at_least_one_theme = false;

            if (null != pdf_document_node_content.PDFDocument.Library.ExpeditionManager)
            {
                ExpeditionDataSource eds = pdf_document_node_content.PDFDocument.Library.ExpeditionManager.ExpeditionDataSource;
                if (null != eds)
                {
                    if (eds.docs_index.ContainsKey(pdf_document_node_content.PDFDocument.Fingerprint))
                    {
                        int doc_id = eds.docs_index[pdf_document_node_content.PDFDocument.Fingerprint];
                        TopicProbability[] topics = eds.LDAAnalysis.DensityOfTopicsInDocsSorted[doc_id];

                        for (int t = 0; t < topics.Length && t < 5; ++t)
                        {
                            string topic_name = eds.GetDescriptionForTopic(topics[t].topic, false, "\n");
                            ThemeNodeContent tnc = new ThemeNodeContent(topic_name, pdf_document_node_content.PDFDocument.Library.WebLibraryDetail.Id);
                            NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, tnc, false);

                            added_at_least_one_theme = true;
                        }
                    }
                }
            }


            if (!added_at_least_one_theme)
            {
                MessageBoxes.Warn("There were no themes available for this document.  Please run Expedition against your library.");
            }
        }

        private void ExpandCitationsInbound()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_CitationsInbound);

            List<Citation> citations = pdf_document_node_content.PDFDocument.PDFDocumentCitationManager.GetInboundCitations();

            foreach (var citation in citations)
            {
                PDFDocumentNodeContent content = new PDFDocumentNodeContent(citation.fingerprint_outbound, pdf_document_node_content.PDFDocument.Library.WebLibraryDetail.Id);
                if (!content.PDFDocument.Deleted)
                {
                    NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
                }
            }
        }


        private void ExpandCitationsOutbound()        
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_CitationsOutbound);

            List<Citation> citations = pdf_document_node_content.PDFDocument.PDFDocumentCitationManager.GetOutboundCitations();

            foreach (var citation in citations)
            {
                // NB: We assube the citations are from the same library!!
                PDFDocumentNodeContent content = new PDFDocumentNodeContent(citation.fingerprint_inbound, pdf_document_node_content.PDFDocument.Library.WebLibraryDetail.Id);
                if (!content.PDFDocument.Deleted)
                {
                    NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
                }
            }
        }
        
        private void ExpandAnnotations()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Annotations);

            foreach (var annotation in pdf_document_node_content.PDFDocument.Annotations)
            {
                if (!annotation.Deleted)
                {
                    PDFAnnotationNodeContent content = new PDFAnnotationNodeContent(pdf_document_node_content.PDFDocument.Library.WebLibraryDetail.Id, pdf_document_node_content.PDFDocument.Fingerprint, annotation.Guid.Value);
                    NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
                }
            }
        }

        private void ExpandAutoTags()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_AutoTags);

            PDFDocument pdf_document = pdf_document_node_content.PDFDocument;

            HashSet<string> tags = pdf_document.Library.AITagManager.AITags.GetTagsWithDocument(pdf_document.Fingerprint);
            foreach (string tag in tags)
            {
                PDFAutoTagNodeContent pdf_auto_tag = new PDFAutoTagNodeContent(pdf_document.Library.WebLibraryDetail.Id, tag);
                NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, pdf_auto_tag, false);
            }
        }

        private void ExpandTags()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Tags);

            PDFDocument pdf_document = pdf_document_node_content.PDFDocument;

            foreach (string tag in TagTools.ConvertTagBundleToTags(pdf_document.Tags))
            {
                PDFTagNodeContent pdf_tag = new PDFTagNodeContent(pdf_document.Library.WebLibraryDetail.Id, tag);
                NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, pdf_tag, false);
            }
        }

        private void ExpandAuthors()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Authors);

            PDFDocument pdf_document = pdf_document_node_content.PDFDocument;

            string authors = pdf_document.AuthorsCombined;
            if (String.IsNullOrEmpty(authors) || Constants.UNKNOWN_AUTHORS == authors)
            {
                return;
            }

            List<NameTools.Name> names = new List<NameTools.Name>();
            string[] authors_split = NameTools.SplitAuthors_LEGACY(authors);
            foreach (string author_split in authors_split)
            {
                string first_names, last_name;
                NameTools.SplitName_LEGACY(author_split, out first_names, out last_name);
                string initial = String.IsNullOrEmpty(first_names) ? null : first_names.Substring(0, 1);
                PDFAuthorNodeContent pdf_author = new PDFAuthorNodeContent(pdf_document.Library.WebLibraryDetail.Id, last_name, initial);
                NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, pdf_author, false);
            }
        }

        void PDFDocumentNodeContentControl_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            try
            {
                if (null == library_index_hover_popup)
                {
                    library_index_hover_popup = new LibraryIndexHoverPopup();
                    library_index_hover_popup.SetPopupContent(pdf_document_node_content.PDFDocument, 1);
                    this.ToolTip = library_index_hover_popup;
                }
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "Exception while displaying document preview popup");
            }
        }

        void PDFDocumentNodeContentControl_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            this.ToolTip = "";
            library_index_hover_popup?.Dispose();
            library_index_hover_popup = null;
        }

        void  DocumentNodeContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document_node_content.PDFDocument);
        }

        public void ProcessKeyPress(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.I:
                    ExpandCitationsInbound();
                    e.Handled = true;
                    break;

                case Key.O:
                    ExpandCitationsOutbound();
                    e.Handled = true;
                    break;

                case Key.A:
                    ExpandAuthors();
                    e.Handled = true;
                    break;

                case Key.T:
                    ExpandTags();
                    e.Handled = true;
                    break;

                case Key.G:
                    ExpandAutoTags();
                    e.Handled = true;
                    break;

                case Key.N:
                    ExpandAnnotations();
                    e.Handled = true;
                    break;

                case Key.M:
                    ExpandThemes();
                    e.Handled = true;
                    break;

                case Key.S:
                    ExpandSimilars();
                    e.Handled = true;
                    break;

                case Key.R:
                    ExpandRelevants();
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }
    }
}
