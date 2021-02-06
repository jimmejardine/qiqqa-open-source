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
using Utilities.Misc;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for DocumentNodeContentControl.xaml
    /// </summary>
    public partial class PDFDocumentNodeContentControl : UserControl, IKeyPressableNodeContentControl, IDisposable
    {
        private NodeControl node_control;
        private PDFDocumentNodeContent pdf_document_node_content;

        //
        // Warning CA1001  Implement IDisposable on 'PDFAnnotationNodeContentControl' because it creates
        // members of the following IDisposable types: 'LibraryIndexHoverPopup'.
        // If 'PDFAnnotationNodeContentControl' has previously shipped, adding new members that implement
        // IDisposable to this type is considered a breaking change to existing consumers.
        //
        // Note from GHO: that object is already managed through the sequence of tooltip_open and tooltip_close
        // handlers below and is currently not considered a memory leak risk for https://github.com/jimmejardine/qiqqa-open-source/issues/19
        // and there-abouts.

        private LibraryIndexHoverPopup library_index_hover_popup = null;

        public PDFDocumentNodeContentControl(NodeControl node_control, PDFDocumentNodeContent pdf_document_node_content)
        {
            InitializeComponent();

            this.node_control = node_control;
            this.pdf_document_node_content = pdf_document_node_content;

            DataContextChanged += PDFDocumentNodeContentControl_DataContextChanged;
            DataContext = pdf_document_node_content.PDFDocument.Bindable;

            Focusable = true;

            ImageIcon.Source = Icons.GetAppIcon(Icons.BrainstormDocument);

            ImageIcon.Opacity = 0.5;
            ImageIcon.Width = NodeThemes.image_width;
            TextBorder.CornerRadius = NodeThemes.corner_radius;
            TextBorder.Background = NodeThemes.background_brush;

            TextTitle.FontWeight = FontWeights.Bold;
            TextPublication.FontStyle = FontStyles.Italic;

            MouseDoubleClick += DocumentNodeContentControl_MouseDoubleClick;
            ToolTip = "";
            ToolTipClosing += PDFDocumentNodeContentControl_ToolTipClosing;
            ToolTipOpening += PDFDocumentNodeContentControl_ToolTipOpening;
        }

        private void PDFDocumentNodeContentControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ObjThemeSwatch.Background = ThemeBrushes.GetBrushForDocument(pdf_document_node_content.PDFDocument);
            if (ThemeBrushes.UNKNOWN_BRUSH == TextBorder.Background)
            {
                ObjThemeSwatch.Background = Brushes.Transparent;
            }
        }

        private static void ExpandRelevants(PDFDocument doc, NodeControl node_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(doc != null);

            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Similars);

            if (doc != null)
            {
                List<ExpeditionPaperSuggestions.Result> results = ExpeditionPaperSuggestions.GetRelevantOthers(doc, 10);

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    foreach (ExpeditionPaperSuggestions.Result result in results)
                    {
                        PDFDocumentNodeContent content = new PDFDocumentNodeContent(result.pdf_document.Fingerprint, result.pdf_document.LibraryRef.Id);
                        NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
                    }
                });
            }
        }

        private static void ExpandSimilars(PDFDocument doc, NodeControl node_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(doc != null);

            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Similars);

            if (doc != null)
            {
                ASSERT.Test(doc.LibraryRef.Xlibrary != null);

                ExpeditionDataSource eds = doc.LibraryRef.Xlibrary?.ExpeditionManager?.ExpeditionDataSource;
                if (null != eds)
                {
                    if (eds.docs_index.ContainsKey(doc.Fingerprint))
                    {
                        int doc_id = eds.docs_index[doc.Fingerprint];
                        float[,] density_of_topics_in_docs = eds.LDAAnalysis.DensityOfTopicsInDocuments;

                        float[] distribution = new float[eds.LDAAnalysis.NUM_TOPICS];
                        for (int topic_i = 0; topic_i < eds.LDAAnalysis.NUM_TOPICS; ++topic_i)
                        {
                            distribution[topic_i] = density_of_topics_in_docs[doc_id, topic_i];
                        }

                        ThemeNodeContentControl.AddDocumentsSimilarToDistribution(node_control, doc.LibraryRef, eds, distribution);
                    }
                }
                else
                {
                    Logging.Warn("Expedition has not been run for library '{0}'.", doc.LibraryRef.Title);
                }
            }
        }

        private static void ExpandThemes(PDFDocument doc, NodeControl node_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(doc != null);

            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Themes);

            if (doc != null)
            {
                ASSERT.Test(doc.LibraryRef.Xlibrary != null);

                bool added_at_least_one_theme = false;

                    ExpeditionDataSource eds = doc.LibraryRef.Xlibrary?.ExpeditionManager?.ExpeditionDataSource;
                    if (null != eds)
                    {
                        if (eds.docs_index.ContainsKey(doc.Fingerprint))
                        {
                            int doc_id = eds.docs_index[doc.Fingerprint];
                            TopicProbability[] topics = eds.LDAAnalysis.DensityOfTopicsInDocsSorted[doc_id];

                            WPFDoEvents.InvokeInUIThread(() =>
                            {
                                for (int t = 0; t < topics.Length && t < 5; ++t)
                                {
                                    string topic_name = eds.GetDescriptionForTopic(topics[t].topic, include_topic_number: false, "\n");
                                    ThemeNodeContent tnc = new ThemeNodeContent(topic_name, doc.LibraryRef.Id);
                                    NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, tnc, false);

                                    added_at_least_one_theme = true;
                                }
                            });
                        }
                        else
                    {
                        Logging.Warn("Expedition has not been run for library '{0}'.", doc.LibraryRef.Title);
                    }
                }

                if (!added_at_least_one_theme)
                {
                    MessageBoxes.Warn("There were no themes available for this document.  Please run Expedition against your library.");
                }
            }
        }

        private static void ExpandCitationsInbound(PDFDocument doc, NodeControl node_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(doc != null);

            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_CitationsInbound);

            if (doc != null)
            {
                List<Citation> citations = doc.PDFDocumentCitationManager.GetInboundCitations();

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    foreach (var citation in citations)
                    {
                        PDFDocumentNodeContent content = new PDFDocumentNodeContent(citation.fingerprint_outbound, doc.LibraryRef.Id);
                        if (!content.PDFDocument.Deleted)
                        {
                            NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
                        }
                    }
                });
            }
        }

        private static void ExpandCitationsOutbound(PDFDocument doc, NodeControl node_control)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_CitationsOutbound);
            ASSERT.Test(doc != null);

            if (doc != null)
            {
                List<Citation> citations = doc.PDFDocumentCitationManager.GetOutboundCitations();

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    foreach (var citation in citations)
                    {
                    // NB: We assume the citations are from the same library!!
                    PDFDocumentNodeContent content = new PDFDocumentNodeContent(citation.fingerprint_inbound, doc.LibraryRef.Id);
                        if (!content.PDFDocument.Deleted)
                        {
                            NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
                        }
                    }
                });
            }
        }

        private static void ExpandAnnotations(PDFDocument doc, NodeControl node_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(doc != null);

            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Annotations);

            if (doc != null)
            {
                var annotations = doc.GetAnnotations();

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    foreach (var annotation in annotations)
                    {
                        if (!annotation.Deleted)
                        {
                            PDFAnnotationNodeContent content = new PDFAnnotationNodeContent(doc.LibraryRef.Id, doc.Fingerprint, annotation.Guid.Value);
                            NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
                        }
                    }
                });
            }
        }

        private static void ExpandAutoTags(PDFDocument doc, NodeControl node_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(doc != null);

            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_AutoTags);

            if (doc != null)
            {
                HashSet<string> tags = doc.LibraryRef.Xlibrary.AITagManager.AITags.GetTagsWithDocument(doc.Fingerprint);

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    foreach (string tag in tags)
                    {
                        PDFAutoTagNodeContent pdf_auto_tag = new PDFAutoTagNodeContent(doc.LibraryRef.Id, tag);
                        NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, pdf_auto_tag, false);
                    }
                });
            }
        }

        private static void ExpandTags(PDFDocument doc, NodeControl node_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(doc != null);

            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Tags);

            if (doc != null)
            {
                var tags = TagTools.ConvertTagBundleToTags(doc.Tags);

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    foreach (string tag in tags)
                    {
                        PDFTagNodeContent pdf_tag = new PDFTagNodeContent(doc.LibraryRef.Id, tag);
                        NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, pdf_tag, false);
                    }
                });
            }
        }


        private static void ExpandAuthors(PDFDocument doc, NodeControl node_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(doc != null);

            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Document_Authors);

            if (doc != null)
            {
                string authors = doc.AuthorsCombined;
                if (String.IsNullOrEmpty(authors) || Constants.UNKNOWN_AUTHORS == authors)
                {
                    return;
                }

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    List<NameTools.Name> names = new List<NameTools.Name>();
                    string[] authors_split = NameTools.SplitAuthors_LEGACY(authors);
                    foreach (string author_split in authors_split)
                    {
                        string first_names, last_name;
                        NameTools.SplitName_LEGACY(author_split, out first_names, out last_name);
                        string initial = String.IsNullOrEmpty(first_names) ? null : first_names.Substring(0, 1);
                        PDFAuthorNodeContent pdf_author = new PDFAuthorNodeContent(doc.LibraryRef.Id, last_name, initial);
                        NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, pdf_author, false);
                    }
                });
            }
        }

        private void PDFDocumentNodeContentControl_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            try
            {
                if (null == library_index_hover_popup)
                {
                    library_index_hover_popup = new LibraryIndexHoverPopup();
                    library_index_hover_popup.SetPopupContent(pdf_document_node_content.PDFDocument, 1);
                    ToolTip = library_index_hover_popup;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Exception while displaying document preview popup");
            }
        }

        private void PDFDocumentNodeContentControl_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            ToolTip = "";

            library_index_hover_popup?.Dispose();
            library_index_hover_popup = null;
        }

        private void DocumentNodeContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document_node_content.PDFDocument);
        }

        public void ProcessKeyPress(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.I:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandCitationsInbound(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                case Key.O:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandCitationsOutbound(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                case Key.A:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandAuthors(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                case Key.T:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandTags(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                case Key.G:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandAutoTags(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                case Key.N:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandAnnotations(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                case Key.M:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandThemes(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                case Key.S:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandSimilars(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                case Key.R:
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        ExpandRelevants(pdf_document_node_content.PDFDocument, node_control);
                    });
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFDocumentNodeContentControl()
        {
            Logging.Debug("~PDFDocumentNodeContentControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing PDFDocumentNodeContentControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFDocumentNodeContentControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        library_index_hover_popup?.Dispose();
                    }
                    library_index_hover_popup = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    ToolTip = "";
                });

                WPFDoEvents.SafeExec(() =>
                {
                    node_control = null;
                    pdf_document_node_content = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    DataContextChanged -= PDFDocumentNodeContentControl_DataContextChanged;
                    DataContext = null;
                });

                ++dispose_count;
            });
        }

        #endregion
    }
}

