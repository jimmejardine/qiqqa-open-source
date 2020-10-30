using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using icons;
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

            DataContextChanged += MultipleDocumentsSelectedPanel_DataContextChanged;

            ObjUserReviewControl.SetDatesVisible(false);

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
            ButtonResetBibTeX.Icon = Icons.GetAppIcon(Icons.BibTeXReset);
            ButtonResetBibTeX.Click += ButtonResetBibTeX_Click;
            ButtonResetBibTeX.Icon = Icons.GetAppIcon(Icons.Yes);
            ButtonApplyBibTeX.Caption = "Apply";
            ButtonApplyBibTeX.Click += ButtonApplyBibTeX_Click;

            //ButtonToggleBibTeX.Caption = "Toggle View";
            ButtonToggleBibTeX.Click += ButtonToggleBibTeX_Click;
            //ButtonAckBibTeXParseErrors.Caption = "Parse Errors";
            ButtonAckBibTeXParseErrors.Click += ButtonAckBibTeXParseErrors_Click;
            //ButtonUndoBibTeXEdit.Caption = "Undo";
            ButtonUndoBibTeXEdit.Click += ButtonUndoBibTeXEdit_Click;
            ObjBibTeXEditorControl.RegisterOverlayButtons(ButtonAckBibTeXParseErrors, ButtonToggleBibTeX, ButtonUndoBibTeXEdit);

            ResetBibTeXStub();
        }

        private void ButtonUndoBibTeXEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sorry!\n\nMethod has not been implemented yet!");
        }

        private void ButtonAckBibTeXParseErrors_Click(object sender, RoutedEventArgs e)
        {
            ObjBibTeXEditorControl.ToggleBibTeXErrorView();
        }

        private void ButtonToggleBibTeX_Click(object sender, RoutedEventArgs e)
        {
            ObjBibTeXEditorControl.ToggleBibTeXMode(TriState.Arbitrary);
        }

        // ---------------------------------------------------------------------------------------------------

        private List<PDFDocument> SelectedPDFDocuments => DataContext as List<PDFDocument>;

        private void MultipleDocumentsSelectedPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (null == SelectedPDFDocuments) return;

            TxtNumDocuments.Text = Convert.ToString(SelectedPDFDocuments.Count);
        }

        // ---------------------------------------------------------------------------------------------------

        private class TagsAddStub
        {
            public string Tags { get; set; }
        }

        private TagsAddStub tags_add_stub = null;

        private void ResetTagsAddStub()
        {
            ObjTagsAddEditorControl.DataContext = tags_add_stub = new TagsAddStub();
        }

        private void ButtonResetTagsAdd_Click(object sender, RoutedEventArgs e)
        {
            ResetTagsAddStub();
        }

        private void ButtonApplyTagsAdd_Click(object sender, RoutedEventArgs e)
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

        private class TagsRemoveStub
        {
            public string Tags { get; set; }
        }

        private TagsRemoveStub tags_remove_stub = null;

        private void ResetTagsRemoveStub()
        {
            ObjTagsRemoveEditorControl.DataContext = tags_remove_stub = new TagsRemoveStub();
        }

        private void ButtonResetTagsRemove_Click(object sender, RoutedEventArgs e)
        {
            ResetTagsRemoveStub();
        }

        private void ButtonApplyTagsRemove_Click(object sender, RoutedEventArgs e)
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

        private class ReviewStub
        {
            public string ReadingStage { get; set; }
            public string Rating { get; set; }
            public bool? IsFavourite { get; set; }
            public Color Color { get; set; }
        }

        private ReviewStub review_stub = null;

        private void ResetReviewStub()
        {
            ObjUserReviewControl.DataContext = review_stub = new ReviewStub();
        }

        private static readonly Color NULL_COLOR = Color.FromArgb(0, 0, 0, 0);

        private void ButtonResetReview_Click(object sender, RoutedEventArgs e)
        {
            ResetReviewStub();
        }

        private void ButtonApplyReview_Click(object sender, RoutedEventArgs e)
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
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.ReadingStage));
                }
                if (!String.IsNullOrEmpty(review_stub.Rating))
                {
                    pdf_document.Rating = review_stub.Rating;
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Rating));
                }
                if (review_stub.IsFavourite.HasValue)
                {
                    pdf_document.IsFavourite = review_stub.IsFavourite;
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.IsFavourite));
                }
                if (review_stub.Color != NULL_COLOR)
                {
                    pdf_document.Color = review_stub.Color;
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Color));
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------

        private class BibTeXStub
        {
            public string BibTex { get; set; }
        }

        private BibTeXStub bibtex_stub = null;

        private void ResetBibTeXStub()
        {
            bibtex_stub = new BibTeXStub();
            bibtex_stub.BibTex = "";
            ObjBibTeXEditorControl.DataContext = bibtex_stub;
        }

        private void ButtonResetBibTeX_Click(object sender, RoutedEventArgs e)
        {
            ResetBibTeXStub();
        }

        private void ButtonApplyBibTeX_Click(object sender, RoutedEventArgs e)
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

                    foreach (var field_pair in bibtex_item_global.Fields)
                    {
                        bibtex_item[field_pair.Key] = field_pair.Value;
                    }

                    pdf_document.BibTex = bibtex_item.ToBibTex();
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
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
    }
}
