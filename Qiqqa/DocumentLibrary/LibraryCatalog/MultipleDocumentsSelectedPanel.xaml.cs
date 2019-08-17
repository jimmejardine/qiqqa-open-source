using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Qiqqa.Common.TagManagement;
using Qiqqa.Documents.PDF;
using Utilities.BibTex.Parsing;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryCatalog
{
    /// <summary>
    /// Interaction logic for MultipleDocumentsSelectedPanel.xaml
    /// </summary>
    public partial class MultipleDocumentsSelectedPanel : UserControl
    {
        public MultipleDocumentsSelectedPanel()
        {
            InitializeComponent();

            this.DataContextChanged += MultipleDocumentsSelectedPanel_DataContextChanged;

            ButtonResetTagsAdd.Caption = "Reset";
            ButtonResetTagsAdd.Click += ButtonResetTagsAdd_Click;
            ButtonApplyTagsAdd.Caption = "Apply";
            ButtonApplyTagsAdd.Click += ButtonApplyTagsAdd_Click;
            ResetTagsAddStub();

            ButtonResetTagsRemove.Caption = "Reset";
            ButtonResetTagsRemove.Click += ButtonResetTagsRemove_Click;
            ButtonApplyTagsRemove.Caption = "Apply";
            ButtonApplyTagsRemove.Click += ButtonApplyTagsRemove_Click;
            ResetTagsRemoveStub();

            ButtonResetReview.Caption = "Reset";
            ButtonResetReview.Click += ButtonResetReview_Click;
            ButtonApplyReview.Caption = "Apply";
            ButtonApplyReview.Click += ButtonApplyReview_Click;
            ResetReviewStub();

            ButtonResetBibTeX.Caption = "Reset";
            ButtonResetBibTeX.Click += ButtonResetBibTeX_Click;
            ButtonApplyBibTeX.Caption = "Apply";
            ButtonApplyBibTeX.Click += ButtonApplyBibTeX_Click;
            ResetBibTeXStub();
        }

        // ---------------------------------------------------------------------------------------------------

        List<PDFDocument> SelectedPDFDocuments
        {
            get
            {
                return this.DataContext as List<PDFDocument>;
            }
        }

        void MultipleDocumentsSelectedPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (null == SelectedPDFDocuments) return;

            TxtNumDocuments.Text = Convert.ToString(SelectedPDFDocuments.Count);
        }

        // ---------------------------------------------------------------------------------------------------

        class TagsAddStub
        {
            public string Tags { get; set; }
        }

        TagsAddStub tags_add_stub = null;

        private void ResetTagsAddStub()
        {
            ObjTagsAddEditorControl.DataContext = tags_add_stub = new TagsAddStub();
        }

        void ButtonResetTagsAdd_Click(object sender, RoutedEventArgs e)
        {
            ResetTagsAddStub();
        }

        void ButtonApplyTagsAdd_Click(object sender, RoutedEventArgs e)
        {
            List<PDFDocument> selected_pdf_documents = SelectedPDFDocuments;

            if (null == selected_pdf_documents) return;

            HashSet<string> new_tags = TagTools.ConvertTagBundleToTags(tags_add_stub.Tags);

            if (!MessageBoxes.AskQuestion("Are you sure you want to add {0} tags to {1} documents?"
                , new_tags.Count
                , selected_pdf_documents.Count))
            {
                return;
            }

            foreach (var pdf_document in selected_pdf_documents)
            {
                pdf_document.AddTag(tags_add_stub.Tags);
            }
        }

        // ---------------------------------------------------------------------------------------------------

        class TagsRemoveStub
        {
            public string Tags { get; set; }
        }

        TagsRemoveStub tags_remove_stub = null;

        private void ResetTagsRemoveStub()
        {
            ObjTagsRemoveEditorControl.DataContext = tags_remove_stub = new TagsRemoveStub();
        }

        void ButtonResetTagsRemove_Click(object sender, RoutedEventArgs e)
        {
            ResetTagsRemoveStub();
        }

        void ButtonApplyTagsRemove_Click(object sender, RoutedEventArgs e)
        {
            List<PDFDocument> selected_pdf_documents = SelectedPDFDocuments;

            if (null == selected_pdf_documents) return;

            HashSet<string> new_tags = TagTools.ConvertTagBundleToTags(tags_remove_stub.Tags);

            if (!MessageBoxes.AskQuestion("Are you sure you want to remove {0} tags from {1} documents?"
                , new_tags.Count
                , selected_pdf_documents.Count
                ))
            {
                return;
            }

            foreach (var pdf_document in selected_pdf_documents)
            {
                pdf_document.RemoveTag(tags_remove_stub.Tags);
            }
        }

        // ---------------------------------------------------------------------------------------------------

        class ReviewStub
        {
            public string ReadingStage { get; set; }
            public string Rating { get; set; }
            public bool? IsFavourite { get; set; }
            public Color Color { get; set; }
        }

        ReviewStub review_stub = null;

        private void ResetReviewStub()
        {
            ObjUserReviewControl.DataContext = review_stub = new ReviewStub();
        }

        static readonly Color NULL_COLOR = Color.FromArgb(0,0,0,0);

        void ButtonResetReview_Click(object sender, RoutedEventArgs e)
        {
            ResetReviewStub();
        }

        void ButtonApplyReview_Click(object sender, RoutedEventArgs e)
        {
            List<PDFDocument> selected_pdf_documents = SelectedPDFDocuments;

            if (null == selected_pdf_documents) return;

            if (!MessageBoxes.AskQuestion("Are you sure you want to mass-edit {0} documents?", selected_pdf_documents.Count))
            {
                return;
            }

            foreach (var pdf_document in selected_pdf_documents)
            {
                if (!String.IsNullOrEmpty(review_stub.ReadingStage))
                {
                    pdf_document.ReadingStage = review_stub.ReadingStage;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.ReadingStage);
                }
                if (!String.IsNullOrEmpty(review_stub.Rating))
                {
                    pdf_document.Rating = review_stub.Rating;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.Rating);
                }
                if (review_stub.IsFavourite.HasValue)
                {
                    pdf_document.IsFavourite = review_stub.IsFavourite;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.IsFavourite);
                }
                if (review_stub.Color != NULL_COLOR)
                {
                    pdf_document.Color = review_stub.Color;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.Color);
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------

        class BibTeXStub
        {
            public string BibTex { get; set; }
        }

        BibTeXStub bibtex_stub = null;

        private void ResetBibTeXStub()
        {
            bibtex_stub = new BibTeXStub();
            bibtex_stub.BibTex = "";
            ObjBibTeXEditorControl.DataContext = bibtex_stub;
        }

        void ButtonResetBibTeX_Click(object sender, RoutedEventArgs e)
        {
            ResetBibTeXStub();
        }

        void ButtonApplyBibTeX_Click(object sender, RoutedEventArgs e)
        {            
            List<PDFDocument> selected_pdf_documents = SelectedPDFDocuments;

            if (null == selected_pdf_documents) return;

            if (!MessageBoxes.AskQuestion("Are you sure you want to mass-edit {0} documents?", selected_pdf_documents.Count))
            {
                return;
            }

            BibTexItem bibtex_item_global = BibTexParser.ParseOne(bibtex_stub.BibTex, true);

            int non_updateable_documents = 0;

            foreach (var pdf_document in selected_pdf_documents)
            {
                BibTexItem bibtex_item = pdf_document.BibTexItem;
                if (null != bibtex_item)
                {
                    if (!String.IsNullOrEmpty(bibtex_item_global.Type))
                    {
                        bibtex_item.Type = bibtex_item_global.Type;
                    }

                    foreach (var field_pair in bibtex_item_global.EnumerateFields())
                    {
                        bibtex_item[field_pair.Key] = field_pair.Value;
                    }

                    pdf_document.BibTex = bibtex_item.ToBibTex();
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.BibTex);
                }
                else
                {
                    ++non_updateable_documents;
                }
            }

            if (0 < non_updateable_documents)
            {
                MessageBoxes.Warn("There was a problem updating {0} documents as they do not have an existing BibTeX record associated with them.", non_updateable_documents);
            }
        }

        // ---------------------------------------------------------------------------------------------------



    }
}
