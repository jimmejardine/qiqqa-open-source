using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.WebcastStuff;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.AITagsStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Mathematics.Topics.LDAStuff;
using Utilities.Misc;

namespace Qiqqa.Expedition
{
    /// <summary>
    /// Interaction logic for InCiteControl.xaml
    /// </summary>
    public partial class ExpeditionControl : UserControl
    {
        private Library library = null;

        public ExpeditionControl()
        {
            InitializeComponent();

            bool ADVANCED_MENUS = ConfigurationManager.Instance.ConfigurationRecord.GUI_AdvancedMenus;

            if (!ADVANCED_MENUS) ButtonRefresh.Caption = "Refresh\nExpedition";
            ButtonRefresh.Icon = Icons.GetAppIcon(Icons.Refresh);
            ButtonRefresh.ToolTip = "Rebuild and refresh the Expedition database for this library.\nThis may take some time, but you can get on with other work in the meanwhile...";
            ButtonRefresh.Click += ButtonRefresh_Click;

            if (!ADVANCED_MENUS) ButtonRefreshTags.Caption = "Refresh\nAutoTags";
            ButtonRefreshTags.ToolTip = "Refresh AutoTags.\nPress this to automatically generate AutoTags for your documents.  This may take some time but you can keep working in the meanwhile...";
            ButtonRefreshTags.Icon = Icons.GetAppIcon(Icons.Refresh);
            ButtonRefreshTags.Click += ButtonRefreshTags_Click;

            if (!ADVANCED_MENUS) ButtonManageLists.Caption = "Manage\nAutoTags";
            ButtonManageLists.ToolTip = "Manage your AutoTag black/whitelists.\nIf there are AutoTags you absolutely do or do not want Qiqqa to use, then you can add them here.";
            ButtonManageLists.Icon = Icons.GetAppIcon(Icons.LibraryAutoTagsBlackWhiteLists);
            ButtonManageLists.Click += ButtonManageLists_Click;

            if (!ADVANCED_MENUS) ButtonExportTopics.Caption = "Export\nThemes";
            ButtonExportTopics.ToolTip = "Export a list of themes for your library.";
            ButtonExportTopics.Icon = Icons.GetAppIcon(Icons.ExportToText);
            ButtonExportTopics.Click += ButtonExportTopics_Click;


            ButtonCollapseAll.Icon = Icons.GetAppIcon(Icons.Minus);
            ButtonCollapseAll.IconHeight = 12;
            ButtonCollapseAll.IconWidth = 12;
            ButtonCollapseAll.ToolTip = "Collapse all topics";
            ButtonCollapseAll.Click += ButtonCollapseAll_Click;

            ButtonExpandAll.Icon = Icons.GetAppIcon(Icons.Plus);
            ButtonExpandAll.IconHeight = 12;
            ButtonExpandAll.IconWidth = 12;
            ButtonExpandAll.ToolTip = "Expand all topics";
            ButtonExpandAll.Click += ButtonExpandAll_Click;

            GridVote.Visibility = Visibility.Collapsed;
            VoteUp.Icon = Icons.GetAppIcon(Icons.ThumbsUp);
            VoteUp.Click += VoteUp_Click;
            VoteDown.Icon = Icons.GetAppIcon(Icons.ThumbsDown);
            VoteDown.Click += VoteDown_Click;


            TextLibraryForExpedition.IsReadOnly = true;
            TextLibraryForExpedition.PreviewMouseDown += TextLibraryForExpedition_PreviewMouseDown;
            TextLibraryForExpedition.ToolTip = "Click to choose a Library to use for your Expedition.";
            TextLibraryForExpedition.Cursor = Cursors.Hand;

            ObjDocumentOverviewControl.ObjExpeditionPaperSimilarsControl.PDFDocumentSelected += ObjDocumentOverviewControl_PDFDocumentSelected;

            Webcasts.FormatWebcastButton(ButtonWebcast, Webcasts.EXPEDITION);
            if (!ADVANCED_MENUS) ButtonWebcast.Caption = "Tutorial\n";

            ChooseNewLibrary(null);
        }

        private void ButtonExportTopics_Click(object sender, RoutedEventArgs e)
        {
            if (null != library.ExpeditionManager.ExpeditionDataSource)
            {
                ExpeditionDataSource eds = library.ExpeditionManager.ExpeditionDataSource;
                LDAAnalysis lda_analysis = library.ExpeditionManager.ExpeditionDataSource.LDAAnalysis;

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < library.ExpeditionManager.ExpeditionDataSource.LDAAnalysis.NUM_TOPICS; ++i)
                {
                    string topic_description = eds.GetDescriptionForTopic(i);
                    sb.AppendFormat("{1}\r\n", i, topic_description);
                }

                string filename = TempFile.GenerateTempFilename("txt");
                File.WriteAllText(filename, sb.ToString());
                Process.Start(filename);
            }
            else
            {
                MessageBoxes.Error("You need to first run Expedition for this library.");
            }
        }

        private void ButtonRefreshTags_Click(object sender, RoutedEventArgs e)
        {
            if (null != library)
            {
                SafeThreadPool.QueueUserWorkItem(o => library.AITagManager.Regenerate());
            }
            else
            {
                MessageBoxes.Warn("Please select a Library before trying to refresh its AutoTags.");
            }
        }

        private void ButtonManageLists_Click(object sender, RoutedEventArgs e)
        {
            if (null != library)
            {
                BlackWhiteListEditorWindow w = new BlackWhiteListEditorWindow();
                w.SetLibrary(library);
                w.Show();
            }
            else
            {
                MessageBoxes.Warn("Please select a Library before trying to manage its AutoTags.");
            }
        }

        private void TextLibraryForExpedition_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Expedition_ChooseLibrary);

            // Pick a new library...
            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != web_library_detail)
            {
                ChooseNewLibrary(web_library_detail);
            }

            e.Handled = true;
        }

        private void ButtonExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in ObjTopicListPanel.Children)
            {
                TopicOverviewControl toc = child as TopicOverviewControl;
                toc.ObjHeader.Expand();
            }
        }

        private void ButtonCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in ObjTopicListPanel.Children)
            {
                TopicOverviewControl toc = child as TopicOverviewControl;
                toc.ObjHeader.Collapse();
            }
        }

        public void ChooseNewLibrary(WebLibraryDetail web_library_detail)
        {
            library = null;
            TextLibraryForExpedition.Text = "Click to choose a library.";
            ObjTopicListPanel.Children.Clear();

            ObjExpeditionInstructionsControl.ReflectLibrary(web_library_detail);

            if (null != web_library_detail)
            {
                library = web_library_detail.library;
                TextLibraryForExpedition.Text = web_library_detail.Title;

                int suggested_theme_count = library.ExpeditionManager.RecommendedThemeCount;
                TextExpeditionNumThemes.Text = "" + suggested_theme_count;
                TextExpeditionNumThemes.ToolTip = "How many themes do you want in this Expedition?\n(" + suggested_theme_count + " suggested)";

                if (null != library.ExpeditionManager.ExpeditionDataSource)
                {
                    for (int i = 0; i < library.ExpeditionManager.ExpeditionDataSource.LDAAnalysis.NUM_TOPICS; ++i)
                    {
                        TopicOverviewControl.TopicOverviewData tod = new TopicOverviewControl.TopicOverviewData
                        {
                            library = library,
                            topic = i,
                        };

                        TopicOverviewControl toc = new TopicOverviewControl();
                        toc.PDFDocumentSelected += toc_PDFDocumentSelected;
                        toc.DataContext = tod;

                        ObjTopicListPanel.Children.Add(toc);
                    }
                }
            }
        }

        private void toc_PDFDocumentSelected(PDFDocument pdf_document)
        {
            ChooseNewPDFDocument(pdf_document);
        }

        private void ObjDocumentOverviewControl_PDFDocumentSelected(PDFDocument pdf_document)
        {
            ChooseNewPDFDocument(pdf_document);
        }

        public void ChooseNewPDFDocument(PDFDocument pdf_document)
        {
            ObjDocumentMetadataControlsPanel.DataContext = pdf_document.Bindable;
            ObjDocumentOverviewControl.DataContext = pdf_document.Bindable;
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (null == library)
            {
                MessageBoxes.Warn("You need to first choose a library against which to run Qiqqa Expedition.");
                return;
            }

            int num_topics = Convert.ToInt32(TextExpeditionNumThemes.Text);
            bool add_autotags = ObjAddAutoTags.IsChecked ?? true;
            bool add_tags = ObjAddTags.IsChecked ?? true;
            SafeThreadPool.QueueUserWorkItem(o => library.ExpeditionManager.RebuildExpedition(num_topics, add_autotags, add_tags, OnRebuildExpeditionComplete));
        }

        private void OnRebuildExpeditionComplete()
        {
            WPFDoEvents.InvokeAsyncInUIThread(() => OnRebuildExpeditionComplete_GUITHREAD());
        }

        private void OnRebuildExpeditionComplete_GUITHREAD()
        {
            if (null != library)
            {
                ChooseNewLibrary(library.WebLibraryDetail);
                GridVote.Visibility = Visibility.Visible;
            }
        }

        private void VoteDown_Click(object sender, RoutedEventArgs e)
        {
            if (null != library.ExpeditionManager && null != library.ExpeditionManager.ExpeditionDataSource && null != library.ExpeditionManager.ExpeditionDataSource.docs)
            {
                FeatureTrackingManager.Instance.UseFeature(
                    Features.Vote_Expedition,
                    "Direction", "-1",
                    "DocsCount", library.ExpeditionManager.ExpeditionDataSource.docs.Count,
                    "WordsCount", library.ExpeditionManager.ExpeditionDataSource.words.Count
                );
            }
            else
            {
                FeatureTrackingManager.Instance.UseFeature(
                    Features.Vote_Expedition,
                    "Direction", "-1",
                    "DocsCount", "N/A",
                    "WordsCount", "N/A"
                );
            }

            GridVote.Visibility = Visibility.Collapsed;
        }

        private void VoteUp_Click(object sender, RoutedEventArgs e)
        {
            if (null != library.ExpeditionManager && null != library.ExpeditionManager.ExpeditionDataSource && null != library.ExpeditionManager.ExpeditionDataSource.docs)
            {
                FeatureTrackingManager.Instance.UseFeature(
                    Features.Vote_Expedition,
                    "Direction", "+1",
                    "DocsCount", library.ExpeditionManager.ExpeditionDataSource.docs.Count,
                    "WordsCount", library.ExpeditionManager.ExpeditionDataSource.words.Count
                );
            }
            else
            {
                FeatureTrackingManager.Instance.UseFeature(
                    Features.Vote_Expedition,
                    "Direction", "+1",
                    "DocsCount", "N/A",
                    "WordsCount", "N/A"
                );
            }

            GridVote.Visibility = Visibility.Collapsed;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()        
        {
            Library library = Library.GuestInstance;
            Thread.Sleep(1000);

            ExpeditionControl ec = new ExpeditionControl();
            ec.ChooseNewLibrary(library.WebLibraryDetail);

            ControlHostingWindow w = new ControlHostingWindow("Expedition", ec);            
            w.Width = 800;
            w.Height= 600;
            w.Show();
        }
#endif

        #endregion
    }
}
