using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Utilities.GUI.Wizard;

namespace Qiqqa.AnnotationsReportBuilding
{
    /// <summary>
    /// Interaction logic for AnnotationReportOptionsWindow.xaml
    /// </summary>
    public partial class AnnotationReportOptionsWindow : StandardWindow
    {
        public AnnotationReportOptionsWindow()
        {
            InitializeComponent();

            WizardDPs.SetPointOfInterest(this, "LibraryAnnotationReportOptionsWindow");
            WizardDPs.SetPointOfInterest(CmdGenerate, "LibraryAnnotationReportOptionsWindowGenerateButton");

            Title = "Qiqqa Annotation Report Options";
            Width = 800;
            Height = 600;

            CmdSelectNone.Click += CmdSelectNone_Click;
            CmdSelectAll.Click += CmdSelectAll_Click;

            CmdGenerate.Caption = "Generate";
            CmdGenerate.Icon = Icons.GetAppIcon(Icons.LibraryAnnotationsReport);
            CmdCancel.Caption = "Cancel";
            CmdCancel.Icon = Icons.GetAppIcon(Icons.Cancel);

            CmdGenerate.Click += CmdGenerate_Click;
            CmdCancel.Click += CmdCancel_Click;

            ObjFilterByCreatorMeButton.Click += ObjFilterByCreatorMeButton_Click;
        }

        private void ObjFilterByCreatorMeButton_Click(object sender, RoutedEventArgs e)
        {
            ObjFilterByCreatorCombo.Text = ConfigurationManager.Instance.ConfigurationRecord.Account_Nickname;
            e.Handled = true;
        }

        private void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            OnShowTagOptionsComplete(library, pdf_documents, GetAnnotationReportOptions());
            Close();
        }

        private void CmdSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ListTags.SelectAll();
            ListTags.Focus();
        }

        private void CmdSelectNone_Click(object sender, RoutedEventArgs e)
        {
            ListTags.SelectedItem = null;
            ListTags.Focus();
        }

        public delegate void OnShowTagOptionsCompleteDelegate(Library library, List<PDFDocument> pdf_documents, AnnotationReportOptions annotation_report_options);

        private Library library;
        private List<PDFDocument> pdf_documents;
        private OnShowTagOptionsCompleteDelegate OnShowTagOptionsComplete;

        internal void ShowTagOptions(Library library_, List<PDFDocument> pdf_documents_, OnShowTagOptionsCompleteDelegate OnShowTagOptionsComplete_)
        {
            library = library_;
            pdf_documents = pdf_documents_;
            OnShowTagOptionsComplete = OnShowTagOptionsComplete_;

            // Collate all the available tags
            HashSet<string> all_tags = new HashSet<string>();
            HashSet<string> all_creators = new HashSet<string>();
            foreach (var pdf_document in pdf_documents)
            {
                if (pdf_document.Deleted)
                {
                    continue;
                }

                foreach (var pdf_annotation in pdf_document.GetAnnotations())
                {
                    if (pdf_annotation.Deleted)
                    {
                        continue;
                    }

                    if (null != pdf_annotation.Creator)
                    {
                        all_creators.Add(pdf_annotation.Creator);
                    }

                    string tags_bundle = pdf_annotation.Tags;
                    HashSet<string> local_tags = TagTools.ConvertTagBundleToTags(tags_bundle);
                    foreach (var tag in local_tags)
                    {
                        all_tags.Add(tag);
                    }
                }
            }

            List<string> all_creators_sorted = new List<string>(all_creators);
            all_creators_sorted.Sort();
            ObservableCollection<string> all_creators_source = new ObservableCollection<string>(all_creators_sorted);
            ObjFilterByCreatorCombo.ItemsSource = all_creators_source;

            all_tags.Add(HighlightToAnnotationGenerator.HIGHLIGHTS_TAG);
            all_tags.Add(InkToAnnotationGenerator.INKS_TAG);
            List<string> all_tags_sorted = new List<string>(all_tags);
            all_tags_sorted.Sort();
            ObservableCollection<string> bindable_tags = new ObservableCollection<string>(all_tags_sorted);
            ListTags.Items.Clear();
            ListTags.ItemsSource = bindable_tags;

            Show();
        }

        private AnnotationReportOptions GetAnnotationReportOptions()
        {
            // TODO: Replace this awful mess with BINDINGS
            AnnotationReportOptions annotation_report_options = new AnnotationReportOptions();
            annotation_report_options.filter_tags = GetDesiredFilterTags();
            annotation_report_options.IncludeComments = ObjIncludeComments.IsChecked ?? false;
            annotation_report_options.IncludeAbstract = ObjIncludeAbstract.IsChecked ?? false;
            annotation_report_options.IncludeAllPapers = ObjIncludeAllPapers.IsChecked ?? false;

            annotation_report_options.ObeySuppressedImages = !(ObjIMAGEShow.IsChecked ?? false);
            annotation_report_options.ObeySuppressedText = !(ObjTEXTShow.IsChecked ?? false);
            annotation_report_options.SuppressAllImages = ObjIMAGEHide.IsChecked ?? false;
            annotation_report_options.SuppressAllText = ObjTEXTHide.IsChecked ?? false;

            annotation_report_options.FilterByCreationDate = ObjFilterByCreationDate.IsChecked ?? false;
            annotation_report_options.FilterByCreationDate_From = ObjFilterByCreationDate_From.SelectedDate ?? DateTime.MinValue;
            annotation_report_options.FilterByCreationDate_To = ObjFilterByCreationDate_To.SelectedDate ?? DateTime.MaxValue;
            annotation_report_options.FilterByFollowUpDate = ObjFilterByFollowUpDate.IsChecked ?? false;
            annotation_report_options.FilterByFollowUpDate_From = ObjFilterByFollowUpDate_From.SelectedDate ?? DateTime.MinValue;
            annotation_report_options.FilterByFollowUpDate_To = ObjFilterByFollowUpDate_To.SelectedDate ?? DateTime.MaxValue;

            annotation_report_options.FilterByCreator = ObjFilterByCreator.IsChecked ?? false;
            annotation_report_options.FilterByCreator_Name = ObjFilterByCreatorCombo.Text;

            return annotation_report_options;
        }

        private HashSet<string> GetDesiredFilterTags()
        {
            HashSet<string> final_tags = new HashSet<string>();

            // If they have selected ALL tags, then it is the same as selecting NONE of them.
            if (ListTags.Items.Count != ListTags.SelectedItems.Count)
            {
                foreach (var item in ListTags.SelectedItems)
                {
                    final_tags.Add((string)item);
                }
            }

            return final_tags;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            WizardDPs.ClearPointOfInterest(this);
            WizardDPs.ClearPointOfInterest(CmdGenerate);

            library = null;
            pdf_documents.Clear();
            pdf_documents = null;

            ListTags.ItemsSource = null;
            ListTags.Items.Clear();
        }
    }
}
