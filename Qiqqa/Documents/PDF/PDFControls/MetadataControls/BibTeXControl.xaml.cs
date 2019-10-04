using System;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Localisation;
using Utilities.BibTex.Parsing;
using Utilities.GUI;
using Utilities.Reflection;
using Utilities.Strings;
using Utilities;
using Qiqqa.Documents.PDF.MetadataSuggestions;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for BibTeXControl.xaml
    /// </summary>
    public partial class BibTeXControl : UserControl
    {
        public BibTeXControl()
        {
            InitializeComponent();

            ButtonBibTexEditor.Caption = "Popup";
            ButtonBibTexEditor.ToolTip = "Edit this BibTeX in a larger popup window.";
            ButtonBibTexEditor.Click += ButtonBibTexEditor_Click;

            ButtonBibTexClear.Caption = "Clear";
            ButtonBibTexClear.ToolTip = "Clear this BibTeX.";
            ButtonBibTexClear.Click += ButtonBibTexClear_Click;

            ButtonBibTexAutoFind.Caption = "Find";
            ButtonBibTexAutoFind.ToolTip = LocalisationManager.Get("LIBRARY/TIP/BIBTEX_AUTOFIND");
            ButtonBibTexAutoFind.Click += ButtonBibTexAutoFind_Click;
            ButtonBibTexAutoFind.MinWidth = 0;

            ButtonBibTexSniffer.Caption = "Sniffer";
            ButtonBibTexSniffer.ToolTip = LocalisationManager.Get("LIBRARY/TIP/BIBTEX_SNIFFER");
            ButtonBibTexSniffer.Click += ButtonBibTexSniffer_Click;
            ButtonBibTexSniffer.MinWidth = 0;

            ButtonUseSummary.Caption = "Summary";
            ButtonUseSummary.ToolTip = "Use your Reference Summary information to create a BibTeX record.";
            ButtonUseSummary.Click += ButtonUseSummary_Click;
            ButtonUseSummary.MinWidth = 0;
        }

        void ButtonBibTexClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxes.AskQuestion("Are you sure you wish to clear this BibTeX?"))
            {
                AugmentedBindable<PDFDocument> pdf_document_bindable = this.DataContext as AugmentedBindable<PDFDocument>;
                if (null == pdf_document_bindable)
                {
                    return;
                }

                pdf_document_bindable.Underlying.BibTex = "";
                pdf_document_bindable.NotifyPropertyChanged(() => pdf_document_bindable.Underlying.BibTex);
            }
        }


        void ButtonUseSummary_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = this.DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable)
            {
                return;
            }

            if (!pdf_document_bindable.Underlying.BibTex.IsEmpty())
            {
                if (!MessageBoxes.AskQuestion("You already have BibTeX associated with this record.  Are you sure you want to overwrite it?"))
                {
                    return;
                }
            }

            BibTexItem bibtem_item = new BibTexItem();
            bibtem_item.Type = "article";

            bibtem_item.SetTitle(pdf_document_bindable.Underlying.TitleCombined);
            bibtem_item.SetAuthor(pdf_document_bindable.Underlying.AuthorsCombined);
            bibtem_item.SetYear(pdf_document_bindable.Underlying.YearCombined);

            pdf_document_bindable.Underlying.BibTex = bibtem_item;
            pdf_document_bindable.NotifyPropertyChanged(() => pdf_document_bindable.Underlying.BibTex);
        }

        void ButtonBibTexEditor_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = this.DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable)
            {
                return;
            }

            MetadataBibTeXEditorControl editor = new MetadataBibTeXEditorControl();
            editor.Show(pdf_document_bindable);
        }

        void ButtonBibTexAutoFind_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = this.DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable)
            {
                return;
            }

            bool found_bibtex = PDFMetadataInferenceFromBibTeXSearch.InferBibTeX(pdf_document_bindable.Underlying, true);
            if (!found_bibtex)
            {
                if (MessageBoxes.AskQuestion("Qiqqa was unable to automatically find BibTeX for this document.  Do you want to try using the BibTeX Sniffer?"))
                {
                    GoogleBibTexSnifferControl sniffer = new GoogleBibTexSnifferControl();
                    sniffer.Show(pdf_document_bindable.Underlying);
                }
            }
        }

        void ButtonBibTexSniffer_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = this.DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable)
            {
                return;
            }

            GoogleBibTexSnifferControl sniffer = new GoogleBibTexSnifferControl();
            sniffer.Show(pdf_document_bindable.Underlying);
        }
    }
}
