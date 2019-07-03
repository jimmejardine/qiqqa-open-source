using System;
using System.Collections.Generic;
using System.Windows;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.Main.LoginStuff;
using Utilities.GUI.Wizard;

namespace Qiqqa.AnnotationsReportBuilding
{
    /// <summary>
    /// Interaction logic for AnnotationReportOptionsWindow.xaml
    /// </summary>
    public partial class AnnotationReportOptionsWindow : StandardWindow
    {
        public class AnnotationReportOptions
        {
            public HashSet<string> filter_tags = null;
            public bool ObeySuppressedImages;
            public bool ObeySuppressedText;
            public bool SuppressAllImages;
            public bool SuppressAllText;

            public bool IncludeComments = false;
            public bool IncludeAbstract = false;
            public bool IncludeAllPapers = false;            

            public bool SuppressPDFDocumentHeader = false;
            public bool SuppressPDFAnnotationTags = false;

            public bool FilterByCreationDate = false;
            public DateTime FilterByCreationDate_From = DateTime.MinValue;
            public DateTime FilterByCreationDate_To = DateTime.MaxValue;
            public bool FilterByFollowUpDate = false;
            public DateTime FilterByFollowUpDate_From = DateTime.MinValue;
            public DateTime FilterByFollowUpDate_To = DateTime.MaxValue;
            public bool FilterByCreator = false;
            public string FilterByCreator_Name = null;

            public int InitialRenderDelayMilliseconds = 0;
        }

        public AnnotationReportOptionsWindow()
        {
            InitializeComponent();

            WizardDPs.SetPointOfInterest(this, "LibraryAnnotationReportOptionsWindow");
            WizardDPs.SetPointOfInterest(CmdGenerate, "LibraryAnnotationReportOptionsWindowGenerateButton");            

            this.Title = "Qiqqa Annotation Report Options";
            this.Width = 800;
            this.Height = 600;

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

        void ObjFilterByCreatorMeButton_Click(object sender, RoutedEventArgs e)
        {
            ObjFilterByCreatorCombo.Text = ConfigurationManager.Instance.ConfigurationRecord.Account_Nickname;
            e.Handled = true;
        }

        void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void CmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            this.OnShowTagOptionsComplete(this.library, this.pdf_documents, GetAnnotationReportOptions());
            this.Close();
        }

        void CmdSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ListTags.SelectAll();
            ListTags.Focus();
        }

        void CmdSelectNone_Click(object sender, RoutedEventArgs e)
        {
            ListTags.SelectedItem = null;
            ListTags.Focus();
        }


        public delegate void OnShowTagOptionsCompleteDelegate(Library library, List<PDFDocument> pdf_documents, AnnotationReportOptions annotation_report_options);
        Library library;
        List<PDFDocument> pdf_documents;
        OnShowTagOptionsCompleteDelegate OnShowTagOptionsComplete;

        internal void ShowTagOptions(Library library, List<PDFDocument> pdf_documents, OnShowTagOptionsCompleteDelegate OnShowTagOptionsComplete)
        {
            this.library = library;
            this.pdf_documents = pdf_documents;
            this.OnShowTagOptionsComplete = OnShowTagOptionsComplete;

            // Collate all the availalbe tags
            HashSet<string> all_tags = new HashSet<string>();
            HashSet<string> all_creators = new HashSet<string>();
            foreach (var pdf_document in pdf_documents)
            {
                if (pdf_document.Deleted)
                {
                    continue;
                }

                foreach (var pdf_annotation in pdf_document.Annotations)
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
            ObjFilterByCreatorCombo.ItemsSource = all_creators_sorted;
            
            List<string> bindable_tags = new List<string>(all_tags);
            bindable_tags.Add(HighlightToAnnotationGenerator.HIGHLIGHTS_TAG);
            bindable_tags.Add(InkToAnnotationGenerator.INKS_TAG);
            bindable_tags.Sort();
            ListTags.Items.Clear();
            ListTags.ItemsSource = bindable_tags;

            this.Show();
        }

        private AnnotationReportOptions GetAnnotationReportOptions()
        {
            // TODO: Replace this awful mess with BINDINGS
            AnnotationReportOptions annotation_report_options = new AnnotationReportOptions();
            annotation_report_options.filter_tags = GetDesiredFilterTags();
            annotation_report_options.IncludeComments = this.ObjIncludeComments.IsChecked ?? false;
            annotation_report_options.IncludeAbstract = this.ObjIncludeAbstract.IsChecked ?? false;
            annotation_report_options.IncludeAllPapers = this.ObjIncludeAllPapers.IsChecked ?? false;

            annotation_report_options.ObeySuppressedImages = !(this.ObjIMAGEShow.IsChecked ?? false);
            annotation_report_options.ObeySuppressedText = !(this.ObjTEXTShow.IsChecked ?? false);
            annotation_report_options.SuppressAllImages = this.ObjIMAGEHide.IsChecked ?? false;
            annotation_report_options.SuppressAllText = this.ObjTEXTHide.IsChecked ?? false;

            annotation_report_options.FilterByCreationDate = this.ObjFilterByCreationDate.IsChecked ?? false;
            annotation_report_options.FilterByCreationDate_From = this.ObjFilterByCreationDate_From.SelectedDate ?? DateTime.MinValue;
            annotation_report_options.FilterByCreationDate_To = this.ObjFilterByCreationDate_To.SelectedDate ?? DateTime.MaxValue;
            annotation_report_options.FilterByFollowUpDate = this.ObjFilterByFollowUpDate.IsChecked ?? false;
            annotation_report_options.FilterByFollowUpDate_From = this.ObjFilterByFollowUpDate_From.SelectedDate ?? DateTime.MinValue;
            annotation_report_options.FilterByFollowUpDate_To = this.ObjFilterByFollowUpDate_To.SelectedDate ?? DateTime.MaxValue;

            annotation_report_options.FilterByCreator = this.ObjFilterByCreator.IsChecked ?? false;
            annotation_report_options.FilterByCreator_Name = this.ObjFilterByCreatorCombo.Text;

            return annotation_report_options;
        }

        HashSet<string> GetDesiredFilterTags()
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
    }
}
