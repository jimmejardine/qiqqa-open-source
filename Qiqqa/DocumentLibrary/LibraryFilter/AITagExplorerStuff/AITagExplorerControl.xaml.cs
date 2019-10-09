using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using icons;
using Qiqqa.DocumentLibrary.AITagsStuff;
using Qiqqa.DocumentLibrary.TagExplorerStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.DocumentLibrary.LibraryFilter.AITagExplorerStuff
{
    /// <summary>
    /// Interaction logic for TagExplorerControl.xaml
    /// </summary>
    public partial class AITagExplorerControl : UserControl
    {
        private Library library;

        public delegate void OnTagSelectionChangedDelegate(HashSet<string> fingerprints, Span descriptive_span);
        public event OnTagSelectionChangedDelegate OnTagSelectionChanged;

        public AITagExplorerControl()
        {
            InitializeComponent();

            this.ToolTip = "Here are the AutoTags that were generated for your documents.  " + GenericLibraryExplorerControl.YOU_CAN_FILTER_TOOLTIP;

            ButtonRefreshTags.Caption = "Refresh";
            ButtonRefreshTags.ToolTip = "Refresh AutoTags:\nPress this to automatically generate AutoTags for your documents.  This may take some time but you can keep working in the meanwhile...";
            ButtonRefreshTags.Icon = Icons.GetAppIcon(Icons.Refresh);
            ButtonRefreshTags.CaptionDock = Dock.Right;
            ButtonRefreshTags.Click += ButtonRefreshTags_Click;

            ButtonManageLists.Caption = "Manage";
            ButtonManageLists.ToolTip = "Manage your AutoTag black/whitelists:\nIf there are AutoTags you absolutely do or do not want Qiqqa to use, then you can add them here.";
            ButtonManageLists.Icon = Icons.GetAppIcon(Icons.LibraryAutoTagsBlackWhiteLists);
            ButtonManageLists.CaptionDock = Dock.Right;
            ButtonManageLists.Click += ButtonManageLists_Click;

            TagExplorerTree.DescriptionTitle = "AutoTags";
            
            TagExplorerTree.GetNodeItems = GetNodeItems;
            TagExplorerTree.OnItemPopup = OnItemPopup;
            TagExplorerTree.OnItemDragOver = OnItemDragOver;
            TagExplorerTree.OnItemDrop = OnItemDrop;

            GridVote.Visibility = Visibility.Collapsed;
            VoteUp.Icon = Icons.GetAppIcon(Icons.ThumbsUp);
            VoteUp.Click += VoteUp_Click;
            VoteDown.Icon = Icons.GetAppIcon(Icons.ThumbsDown);
            VoteDown.Click += VoteDown_Click;

            TagExplorerTree.OnTagSelectionChanged += TagExplorerTree_OnTagSelectionChanged;
        }

        void ButtonManageLists_Click(object sender, RoutedEventArgs e)
        {
            BlackWhiteListEditorWindow w = new BlackWhiteListEditorWindow();
            w.SetLibrary(library);
            w.Show();
        }

        void VoteDown_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(
                Features.Vote_AutoTag,
                "Direction", "-1",
                "PDFCount", library.PDFDocuments.Count
                );

            MessageBoxes.Info("Thanks for your feedback.  We're sorry that the AutoTags are not up to scratch!\n\nAutoTags are sensitive to the titles of your documents, so if you are missing a lot of titles, that might just be the cause.");
            
            GridVote.Visibility = Visibility.Collapsed;
        }

        void VoteUp_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(
                Features.Vote_AutoTag,
                "Direction", "+1",
                "PDFCount", library.PDFDocuments.Count
                );

            GridVote.Visibility = Visibility.Collapsed;
        }

        void ButtonRefreshTags_Click(object sender, RoutedEventArgs e)
        {
            SafeThreadPool.QueueUserWorkItem(o => library.AITagManager.Regenerate(AITagsRegenerated_NON_GUI_THREAD));
        }

        void AITagsRegenerated_NON_GUI_THREAD(IAsyncResult ar)
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    AITagsRegenerated_GUI_THREAD();
                }
                ));
        }

        void AITagsRegenerated_GUI_THREAD()
        {
            Reset();
            GridVote.Visibility = Visibility.Visible;
        }

        // -----------------------------

        public Library Library
        {
            set
            {
                this.library = value;
                TagExplorerTree.Library = value;
            }
        }

        public void Reset()
        {
            if (null == library || null == library.AITagManager.AITags)
            {
                this.TxtWarningNeverGenerated.Visibility = Visibility.Visible;
                this.TxtWarningGettingOld.Visibility = Visibility.Collapsed;
                this.TxtWarningNoAutoTags.Visibility = Visibility.Collapsed;
                this.TagExplorerTree.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.TxtWarningNeverGenerated.Visibility = Visibility.Collapsed;
                this.TxtWarningGettingOld.Visibility = library.AITagManager.AITags.IsGettingOld ? Visibility.Visible : Visibility.Collapsed;
                this.TxtWarningNoAutoTags.Visibility = library.AITagManager.AITags.HaveNoTags ? Visibility.Visible : Visibility.Collapsed;
                this.TagExplorerTree.Visibility = Visibility.Visible;
            }

            TagExplorerTree.Reset();
        }

        // -----------------------------

        internal static MultiMapSet<string, string> GetNodeItems(Library library, HashSet<string> parent_fingerprints)
        {
            Logging.Info("+Getting node items for AutoTags");

            if (null == library.AITagManager.AITags)
            {
                return new MultiMapSet<string,string>();
            }

            MultiMapSet<string, string> rv;
            if (null == parent_fingerprints)
            {
                rv = library.AITagManager.AITags.GetTagsWithDocuments();
            }
            else
            {
                rv = library.AITagManager.AITags.GetTagsWithDocuments(parent_fingerprints);
            }
            Logging.Debug特("AITagExplorerControl: processing {0} documents from library {1}", rv.Count, library.WebLibraryDetail.Title);
            return rv;
        }

        void OnItemPopup(Library library, string item_tag)
        {
            AITagExplorerItemPopup popup = new AITagExplorerItemPopup(library, item_tag);
            popup.Open();
        }

        void OnItemDragOver(Library library, string item_tag, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PDFDocument)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(List<PDFDocument>)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        void OnItemDrop(Library library, string item_tag, DragEventArgs e)
        {
            if (false) { }
            else if (e.Data.GetDataPresent(typeof(PDFDocument)))
            {
                PDFDocument pdf_document = (PDFDocument)e.Data.GetData(typeof(PDFDocument));
                Logging.Info("The PDF dropped onto tag {1} is {0}", pdf_document, item_tag);

                pdf_document.AddTag(item_tag);
            }
            else if (e.Data.GetDataPresent(typeof(List<PDFDocument>)))
            {
                List<PDFDocument> pdf_documents = (List<PDFDocument>)e.Data.GetData(typeof(List<PDFDocument>));
                Logging.Info("The PDF list dropped onto tag {1} has {0} items", pdf_documents.Count, item_tag);

                foreach (PDFDocument pdf_document in pdf_documents)
                {
                    pdf_document.AddTag(item_tag);
                }
            }

            e.Handled = true;
        }

        void TagExplorerTree_OnTagSelectionChanged(HashSet<string> fingerprints, Span descriptive_span)
        {
            OnTagSelectionChanged?.Invoke(fingerprints, descriptive_span);
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            Library library = Library.GuestInstance;
            TagExplorerControl tec = new TagExplorerControl();
            tec.Library = library;

            ControlHostingWindow w = new ControlHostingWindow("Tags", tec);
            w.Show();
        }
#endif

        #endregion
    }
}
