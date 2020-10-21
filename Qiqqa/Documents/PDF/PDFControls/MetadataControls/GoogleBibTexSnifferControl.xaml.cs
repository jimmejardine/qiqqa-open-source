using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.Common.WebcastStuff;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.LibraryCatalog;
using Qiqqa.Documents.PDF.MetadataSuggestions;
using Qiqqa.Documents.PDF.Search;
using Qiqqa.Localisation;
using Qiqqa.UtilisationTracking;
using Qiqqa.WebBrowsing;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.GUI;
using Utilities.Internet.GoogleScholar;
using Utilities.Misc;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for GoogleBibTexSnifferControl.xaml
    /// </summary>
    public partial class GoogleBibTexSnifferControl : StandardWindow, IDisposable
    {
        private class SearchOptions
        {
#pragma warning disable 0649
            public bool Missing { get; set; }
            public bool Skipped { get; set; }
            public bool Auto { get; set; }
            public bool Manual { get; set; }
            // checkboxes for subset selection: only when ticked ON are these active:
            public bool HasSourceURL { get; set; }
            public bool HasSourceLocalFileSystem { get; set; }
            public bool Unsourced { get; set; }

            private bool? _HasDocumentPDF;
            public bool? HasDocumentPDF
            {
                get => _HasDocumentPDF;
                set
                {
                    if (value == null)
                    {
                        HasDocumentPDF_TriState = true;
                    }
                    // cycle properly: when the checkbox cycles from ON to OFF, it SHOULD cycle from ON to TRISTATE!
                    else if (value == false && _HasDocumentPDF == true)
                    {
                        HasDocumentPDF_TriState = true;
                    }
                    else
                    {
                        HasDocumentPDF_TriState = false;
                        _HasDocumentPDF = value;
                    }
                }
            }
            public bool HasDocumentPDF_TriState
            {
                get => null == _HasDocumentPDF;
                set
                {
                    if (value)
                    {
                        _HasDocumentPDF = null;
                    }
                    else if (_HasDocumentPDF == null)
                    {
                        _HasDocumentPDF = false;
                    }
                }
            }

            private bool? _DocumentIsOCRed;
            public bool? DocumentIsOCRed
            {
                get => _DocumentIsOCRed;
                set
                {
                    if (value == null)
                    {
                        DocumentIsOCRed_TriState = true;
                    }
                    // cycle properly: when the checkbox cycles from ON to OFF, it SHOULD cycle from ON to TRISTATE!
                    else if (value == false && _DocumentIsOCRed == true)
                    {
                        DocumentIsOCRed_TriState = true;
                    }
                    else
                    {
                        DocumentIsOCRed_TriState = false;
                        _DocumentIsOCRed = value;
                    }
                }
            }
            internal bool DocumentIsOCRed_TriState
            {
                get => null == _DocumentIsOCRed;
                set
                {
                    if (value)
                    {
                        _DocumentIsOCRed = null;
                    }
                    else if (_DocumentIsOCRed == null)
                    {
                        _DocumentIsOCRed = false;
                    }
                }
            }
            // checkbox for NOT query mode
            public bool InvertSelection { get; set; }
#pragma warning restore 0649

            public SearchOptions()
            {
                Missing = true;
                HasDocumentPDF = true;
                DocumentIsOCRed = null;
            }
        }

        private SearchOptions search_options;
        private AugmentedBindable<SearchOptions> search_options_bindable;
        private PDFDocument user_specified_pdf_document = null;
        private List<PDFDocument> pdf_documents_total_pool = new List<PDFDocument>();
        private List<PDFDocument> pdf_documents_search_pool = new List<PDFDocument>();
        private int pdf_documents_search_index = 0;
        private PDFDocument pdf_document;
        private PDFDocument pdf_document_rendered = null;
        private PDFRendererControl pdf_renderer_control;
        private string last_autonavigated_url;

        public GoogleBibTexSnifferControl()
        {
            Theme.Initialize();

            InitializeComponent();

            SetupConfiguredDimensions();

            // Search options
            search_options = new SearchOptions();
            search_options_bindable = new AugmentedBindable<SearchOptions>(search_options);
            ObjSearchOptionsPanel.DataContext = search_options_bindable;
            search_options_bindable.PropertyChanged += search_options_bindable_PropertyChanged;

            // Fades of buttons
            Utilities.GUI.Animation.Animations.EnableHoverFade(PDFRendererControlAreaButtonPanel);
            Utilities.GUI.Animation.Animations.EnableHoverFade(ObjBibTeXEditButtonPanel);

            Title = "Qiqqa BibTeX Sniffer";

            Closing += GoogleBibTexSnifferControl_Closing;
            Closed += GoogleBibTexSnifferControl_Closed;

            KeyUp += GoogleBibTexSnifferControl_KeyUp;

            ButtonPrev.Icon = Icons.GetAppIcon(Icons.Back);
            ButtonPrev.ToolTip = "Move to previous PDF.";
            ButtonPrev.Click += ButtonPrev_Click;

            ButtonNext.Icon = Icons.GetAppIcon(Icons.Forward);
            ButtonNext.ToolTip = "Move to next PDF.  You can press the middle key (the 5 key) as a shortcut.";
            ButtonNext.Click += ButtonNext_Click;

            ButtonClear.Icon = Icons.GetAppIcon(Icons.GoogleBibTexSkipForever);
            ButtonClear.ToolTip = "Clear this BibTeX.";
            ButtonClear.Click += ButtonClear_Click;

            ButtonSkipForever.Icon = Icons.GetAppIcon(Icons.GoogleBibTexSkip);
            ButtonSkipForever.ToolTip = "This document has no BibTeX.  Skip it!";
            ButtonSkipForever.Click += ButtonSkipForever_Click;

            ButtonValidate.Icon = Icons.GetAppIcon(Icons.GoogleBibTexNext);
            ButtonValidate.ToolTip = "The automatic BibTeX for this document is great.  Mark it as valid!";
            ButtonValidate.Click += ButtonValidate_Click;

            ButtonToggleBibTeX.Click += ButtonToggleBibTeX_Click;
            ButtonUndoBibTexEdit.Click += ButtonUndoBibTexEdit_Click;
            ButtonShowBibTeXParseErrors.Click += ButtonShowBibTeXParseErrors_Click;
            ObjBibTeXEditorControl.RegisterOverlayButtons(ButtonShowBibTeXParseErrors, ButtonToggleBibTeX, ButtonUndoBibTexEdit);

            ButtonConfig.Icon = Icons.GetAppIcon(Icons.DocumentMisc);
            ButtonConfig.ToolTip = LocalisationManager.Get("PDF/TIP/MORE_MENUS");
            ButtonConfig.Click += ButtonConfig_Click;

            ButtonRedo.Icon = Icons.GetAppIcon(Icons.DesktopRefresh);
            ButtonRedo.ToolTip = "Retry detection of this PDF.";
            ButtonRedo.Click += ButtonRedo_Click;

            ButtonWizard.Icon = Icons.GetAppIcon(Icons.BibTeXSnifferWizard);
            ButtonWizard.ToolTip = "Toggle the BibTeX Sniffer Wizard.\nWhen this is enabled, the sniffer will automatically browse to the first item it sees in Google Scholar.\nThis saves you time because you just have to scan that the BibTeX is correct before moving onto your next paper!";
            ButtonWizard.DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            ObjWebBrowser.GoBibTeXMode();
            ObjWebBrowser.PageLoaded += ObjWebBrowser_PageLoaded;
            ObjWebBrowser.TabChanged += ObjWebBrowser_TabChanged;

            PDFRendererControlArea.ToolTip = "This is the current PDF that has no BibTeX associated with it.  You can select text from the PDF to automatically search for that text.";
            ObjWebBrowser.ToolTip = "Use this browser to hunt for BibTeX of PubMed XML.  As soon as you find some, it will automatically be associated with your PDF.";
            ObjBibTeXEditorControl.ToolTip = "This is the BibTeX that is currently associated with the displayed PDF.\nFeel free to edit this or replace it with a # if there is no BibTeX for this record and you do not want the Sniffer to keep prompting you for some...";

            HyperlinkBibTeXLinksMissing.Click += HyperlinkBibTeXLinksMissing_Click;

            Webcasts.FormatWebcastButton(ButtonWebcast, Webcasts.BIBTEX_SNIFFER);

            ObjBibTeXEditorControl.ObjBibTeXText.TextChanged += TxtBibTeX_TextChanged;

            // Navigate to GS in a bid to not have the first .bib prompt for download
            ObjWebBrowser.DefaultWebSearcherKey = WebSearchers.SCHOLAR_KEY;
            ObjWebBrowser.ForceAdvancedMenus();
            ObjWebBrowser.SetupSnifferSearchers();
        }

        private void ButtonShowBibTeXParseErrors_Click(object sender, RoutedEventArgs e)
        {
            ObjBibTeXEditorControl.ToggleBibTeXErrorView();
        }

        private void ButtonUndoBibTexEdit_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonToggleBibTeX_Click(object sender, RoutedEventArgs e)
        {
            ObjBibTeXEditorControl.ToggleBibTeXMode(TriState.Arbitrary);
        }

        private void GoogleBibTexSnifferControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (Key.Clear == e.Key)
            {
                MoveDelta(+1);
                e.Handled = true;
            }

            else if (Key.Escape == e.Key)
            {
                Close();
                e.Handled = true;
            }
        }

        private void TxtBibTeX_TextChanged(object sender, TextChangedEventArgs e)
        {
            ObjBibTeXEditorControl.Background = null;

            if (null == pdf_document) return;

            try
            {
                BibTexItem bibtex = pdf_document.BibTexItem;

                PDFSearchResultSet search_result_set;
                if (BibTeXGoodnessOfFitEstimator.DoesBibTeXMatchDocument(bibtex, pdf_document, out search_result_set))
                {
                    ObjBibTeXEditorControl.Background = Brushes.LightGreen;
                    // ReflectPDFDocument(string search_terms)?
                    if (null != pdf_renderer_control)
                    {
                        pdf_renderer_control.SetSearchKeywords(search_result_set);
                    }

                    // If we are feeling really racy, let the wizard button also move onto the next guy cos we are cooking on GAS
                    if (ConfigurationManager.Instance.ConfigurationRecord.Metadata_UseBibTeXSnifferWizard)
                    {
                        if (!pdf_document.BibTex?.Contains(BibTeXActionComments.AUTO_GS) ?? false)
                        {
                            pdf_document.BibTex =
                                BibTeXActionComments.AUTO_GS
                                + "\r\n"
                                + pdf_document.BibTex;
                            pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
                        }

                        // fix: https://github.com/jimmejardine/qiqqa-open-source/issues/60
                        //
                        // check how many PDF files actually match and only move forward when we don't end up
                        // full circle:
                        int count = pdf_documents_search_pool.Count;
                        int step = 1;
                        int my_index = pdf_documents_search_index;
                        PDFDocument next_pdf;
                        if (count <= 1)
                        {
                            next_pdf = null;
                            step--;
                        }
                        else
                        {
                            while (step < count)
                            {
                                int pos = my_index + step;
                                if (pos >= count) pos -= count;
                                next_pdf = pdf_documents_search_pool[pos];

                                if (!next_pdf.BibTex?.Contains(BibTeXActionComments.AUTO_GS) ?? false)
                                {
                                    break;
                                }

                                step++;
                            }
                        }

                        // fix https://github.com/jimmejardine/qiqqa-open-source/issues/60: don't cycle if we didn't change.
                        //
                        // only move forward if there's actually a slot to move to that doesn't automatically
                        // moves forward to the current slot itself. Hence it must be a slot at current-position minus 1
                        // or further back or forward from us, i.e. `step+1 <= count`.
                        if (step != 0 && step < count)
                        {
                            MoveDelta(step);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "TextChanged failure in BibTeXSniffer");
            }
        }

        private void HyperlinkBibTeXLinksMissing_Click(object sender, RoutedEventArgs e)
        {
            string message =
                ""
                + "If you are not seeing an \"Import into BibTeX\" link below each search result, it means that you have not yet enabled BibTeX support in Google Scholar.\n\n"
                + "If you want Qiqqa to attempt to take you to the Google Scholar settings screen, press YES and then press 'Save Preferences' when you have reviewed all the available preferences.\n\n"
                + "If you would prefer to do it yourself, press NO.  Go to the main Google Scholar page (http://scholar.google.com) USING THE QIQQA BUILT-IN BROWSER, and open the Google Scholar Settings.  Make sure you are on the .com version of Scholar, by looking at the web address.  If the address does not end in \".com\", then look for a link in the bottom right called \"Go to Google Scholar\" which should take you there.   Once on the .com Scholar settings page, in the 'Bibliography Manager' section, select 'Show links to import citations into BibTeX' and then press 'Save'.";

            if (MessageBoxes.AskQuestion(message))
            {
                string preferences_url = WebsiteAccess.Url_GoogleScholarBibTeXPreferences;
                ObjWebBrowser.OpenUrl(preferences_url);
            }

            e.Handled = true;
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            MoveDelta(+1);
        }

        private void ButtonPrev_Click(object sender, RoutedEventArgs e)
        {
            MoveDelta(-1);
        }

        private void MoveFirst()
        {
            // Move to position #0:
            MoveDelta(-pdf_documents_search_index);
        }

        private void MoveDelta(int direction)
        {
            if (0 < pdf_documents_search_pool.Count)
            {
                pdf_documents_search_index += direction;
                if (pdf_documents_search_index >= pdf_documents_search_pool.Count) pdf_documents_search_index = 0;
                if (pdf_documents_search_index < 0) pdf_documents_search_index = pdf_documents_search_pool.Count - 1;
                pdf_document = pdf_documents_search_pool[pdf_documents_search_index];
            }
            else
            {
                pdf_documents_search_index = 0;
                pdf_document = null;
            }

            ReflectPDFDocument(null);
        }

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            if (null != pdf_document)
            {
                LibraryCatalogPopup popup = new LibraryCatalogPopup(new List<PDFDocument> { pdf_document });
                popup.Open();
                e.Handled = true;
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            if (null != pdf_document)
            {
                pdf_document.BibTex = "";
                pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
            }
        }

        private void ButtonSkipForever_Click(object sender, RoutedEventArgs e)
        {
            if (null != pdf_document)
            {
                pdf_document.BibTex = BibTeXActionComments.SKIP;
                pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
            }

            MoveDelta(+1);
        }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            if (null != pdf_document && null != pdf_document.BibTex)
            {
                pdf_document.BibTex = pdf_document.BibTex
                    .Replace(BibTeXActionComments.AUTO_GS, "")
                    .Replace(BibTeXActionComments.AUTO_BIBTEXSEARCH, "")
                    .Trim();
                pdf_document.BibTex =
                    BibTeXActionComments.USER_VETTED
                    + "\r\n"
                    + pdf_document.BibTex;

                pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
            }

            MoveDelta(+1);
        }

        private new void Show()
        {
            // We don't want outsiders to be able to use this without supplying a PDFDocument or a Library...
        }


        public void Show(PDFDocument pdf_document)
        {
            Show(pdf_document, null);
        }

        public void Show(PDFDocument pdf_document, string search_terms)
        {
            Show(new List<PDFDocument> { pdf_document }, pdf_document, search_terms);
        }

        public void Show(List<PDFDocument> pdf_documents)
        {
            Show(pdf_documents, null, null);
        }

        public void Show(List<PDFDocument> pdf_documents, PDFDocument user_specified_pdf_document, string search_terms)
        {
            this.user_specified_pdf_document = user_specified_pdf_document;
            pdf_documents_total_pool.Clear();
            pdf_documents_total_pool.AddRange(pdf_documents);
            RecalculateSearchPool();

            ObjWebBrowser.CurrentLibrary = CurrentLibrary;

            base.Show();
        }

        private void search_options_bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RecalculateSearchPool();
        }

        private void RecalculateSearchPool()
        {
            Stopwatch clk = Stopwatch.StartNew();
            const int MAX_MILLISECONDS_ALLOWED_FOR_THE_SLOWER_CHECKS = 5 * 1000;
            int slower_checks_allowed = 1;      // state: 1 = allowed, 0 = disallowed, signalling, -1 = disallowed, already signalled

            HasDocumentPDF_CheckBox.Background = Brushes.White;
            //this.HasDocumentPDF_CheckBox.Background.Opacity = 1.0;
            DocumentIsOCRed_CheckBox.Background = Brushes.White;
            //this.DocumentIsOCRed_CheckBox.Background.Opacity = 1.0;

            List<PDFDocument> pdf_documents_inverted_search_pool = new List<PDFDocument>();
            pdf_documents_search_pool.Clear();
            foreach (PDFDocument pdf_document in pdf_documents_total_pool)
            {
                bool include_in_search_pool = false;

                if (pdf_document.Deleted) continue;

                if (search_options.Missing && String.IsNullOrEmpty(pdf_document.BibTex)) include_in_search_pool = true;

                if (!String.IsNullOrEmpty(pdf_document.BibTex))
                {
                    if (search_options.Auto)
                        include_in_search_pool = (
                            pdf_document.BibTex.Contains(BibTeXActionComments.AUTO_BIBTEXSEARCH)
                            || pdf_document.BibTex.Contains(BibTeXActionComments.AUTO_GS)
                            );
                    else if (search_options.Skipped)
                        include_in_search_pool = pdf_document.BibTex.Contains(BibTeXActionComments.SKIP);
                    else if (search_options.Manual)
                        include_in_search_pool = (
                            !pdf_document.BibTex.Contains(BibTeXActionComments.AUTO_BIBTEXSEARCH)
                            && !pdf_document.BibTex.Contains(BibTeXActionComments.AUTO_GS)
                            && !pdf_document.BibTex.Contains(BibTeXActionComments.SKIP));
                }

                // apply subselections:
                if (include_in_search_pool)
                {
                    // When ANY of these checkboxes is *ON*, then the subselection criteria apply.
                    // Otherwise we simply *pass*.
                    //
                    // Note: the three criteria combine together to ALL, i.e. when all three(3)
                    // tickboxes have been ticked, you get *all* documents from the main filter,
                    // i.e. all ticked means everyone *passes*.
                    if (search_options.Unsourced || search_options.HasSourceURL || search_options.HasSourceLocalFileSystem)
                    {
                        // subselection: only match records which match any or all of the ticked criteria:
                        include_in_search_pool = false;

                        if (search_options.Unsourced)
                        {
                            include_in_search_pool |= String.IsNullOrEmpty(pdf_document.DownloadLocation);
                        }
                        if (search_options.HasSourceURL)
                        {
                            include_in_search_pool |= !String.IsNullOrEmpty(pdf_document.DownloadLocation)
                            && (pdf_document.DownloadLocation.StartsWith("http://")
                            || pdf_document.DownloadLocation.StartsWith("https://")
                            || pdf_document.DownloadLocation.StartsWith("ftp://")
                            || pdf_document.DownloadLocation.StartsWith("ftps://"));
                        }
                        if (search_options.HasSourceLocalFileSystem)
                        {
                            // this is the inverse of HasSourceURL for all documents which *do have* a source:
                            include_in_search_pool |= !String.IsNullOrEmpty(pdf_document.DownloadLocation)
                            && !(pdf_document.DownloadLocation.StartsWith("http://")
                            || pdf_document.DownloadLocation.StartsWith("https://")
                            || pdf_document.DownloadLocation.StartsWith("ftp://")
                            || pdf_document.DownloadLocation.StartsWith("ftps://"));
                        }
                    }
                }

                // Now follow the **slow** checks: these are time-restrained in that we have set an "acceptable upper time limit"
                // for them to take, after which we let *everyone* through: UX-wise performance has to win over accuracy of
                // the filter result.
                //
                // When this timeout happens, we flag the flags RED to signal their *inaccuracy*.
                if (1 == slower_checks_allowed)
                {
                    if (clk.ElapsedMilliseconds > MAX_MILLISECONDS_ALLOWED_FOR_THE_SLOWER_CHECKS)
                    {
                        slower_checks_allowed = 0;
                    }
                }
                if (1 == slower_checks_allowed)
                {
                    // another subselection is ON/OFF: does the library entry have a PDF file available or not?
                    if (include_in_search_pool && null != search_options.HasDocumentPDF)
                    {
                        include_in_search_pool = !((bool)search_options.HasDocumentPDF ^ pdf_document.DocumentExists);
                    }

                    // another subselection is ON/OFF: does the library entry have OCR data available already?
                    if (include_in_search_pool && null != search_options.DocumentIsOCRed)
                    {
                        bool hasOCRdata = !pdf_document.IsVanillaReference && pdf_document.HasOCRdata;
                        // perform a more precise check when there's few documents to process, as this check is pretty costly:
                        //
                        // Note: fetching the `PDFRenderer.PageCount` may produce non-zero results, but it would still
                        // be highly inaccurate as documents can exist with a correct(?) pagecount but not having had their
                        // pages OCR-ed -- and that's what matters in the end.
                        //
                        if (hasOCRdata && pdf_documents_total_pool.Count < 100000)
                        {
                            string w = pdf_document.PDFRenderer.GetFullOCRText();
                            hasOCRdata = !String.IsNullOrWhiteSpace(w);
                        }
#if false
                        if (!hasOCRdata)
                        {
                            Logging.Debug("No OCR data known for {0}", pdf_document.Fingerprint);
                        }
#endif
                        include_in_search_pool = !((bool)search_options.DocumentIsOCRed ^ hasOCRdata);
                    }
                }
                else if (0 == slower_checks_allowed)
                {
                    slower_checks_allowed = -1;

                    // color....
                    HasDocumentPDF_CheckBox.Background = Brushes.Red;
                    //this.HasDocumentPDF_CheckBox.Background.Opacity = 0.75;
                    DocumentIsOCRed_CheckBox.Background = Brushes.Red;
                    //this.DocumentIsOCRed_CheckBox.Background.Opacity = 0.75;
                }

                // the odd one out: the user specified document exists in both regular *and* inverted set:
                if (pdf_document == user_specified_pdf_document)
                {
                    pdf_documents_search_pool.Add(pdf_document);
                    pdf_documents_inverted_search_pool.Add(pdf_document);
                }
                else if (include_in_search_pool)
                {
                    pdf_documents_search_pool.Add(pdf_document);
                }
                else
                {
                    pdf_documents_inverted_search_pool.Add(pdf_document);
                }
            }

            // now decide if this was a positive or negative search query: invert the set or not?
            if (search_options.InvertSelection)
            {
                pdf_documents_search_pool.Clear();
                pdf_documents_search_pool = pdf_documents_inverted_search_pool;
            }
            else
            {
                pdf_documents_inverted_search_pool.Clear();
            }

            MoveFirst();
        }

        private void ButtonRedo_Click(object sender, RoutedEventArgs e)
        {
            ReflectPDFDocument(null);
        }

        private void ReflectPDFDocument(string search_terms)
        {
            if (0 < pdf_documents_search_pool.Count)
            {
                TxtProgress.Text = String.Format("Document {0} of {1}.", pdf_documents_search_index + 1, pdf_documents_search_pool.Count);
                ObjProgress.Value = pdf_documents_search_index + 1;
                ObjProgress.Maximum = pdf_documents_search_pool.Count;
            }
            else
            {
                TxtProgress.Text = "No documents";
                ObjProgress.Value = 1;
                ObjProgress.Maximum = 1;
            }

            // fix: https://github.com/jimmejardine/qiqqa-open-source/issues/60
            // fix: https://github.com/jimmejardine/qiqqa-open-source/issues/39
            // fix https://github.com/jimmejardine/qiqqa-open-source/issues/59: don't reflect if we didn't change.
            //
            // when we re-render the same document, we don't move at all!
            if (pdf_document_rendered == pdf_document)
            {
                return;
            }

            if (null != pdf_document_rendered)
            {
                // Clear down the previous renderer control
                PDFRendererControlArea.Children.Clear();

                if (null != pdf_renderer_control)
                {
                    pdf_renderer_control.Dispose();
                    pdf_renderer_control = null;
                }

                pdf_document_rendered = null;
                DataContext = null;
            }

            if (null != pdf_document)
            {
                // Force inference of the title in case it has not been populated...
                PDFMetadataInferenceFromOCR.InferTitleFromOCR(pdf_document, true);

                pdf_document_rendered = pdf_document;
                DataContext = pdf_document.Bindable;

                if (pdf_document.DocumentExists)
                {
                    ObjNoPDFAvailableMessage.Visibility = Visibility.Collapsed;
                    PDFRendererControlArea.Visibility = Visibility.Visible;

                    // Make sure the first page is OCRed...
                    pdf_document.PDFRenderer.GetOCRText(1);

                    // Set up the new renderer control
                    pdf_renderer_control = new PDFRendererControl(pdf_document, false, PDFRendererControl.ZoomType.Zoom1Up);
                    pdf_renderer_control.ReconsiderOperationMode(PDFRendererControl.OperationMode.TextSentenceSelect);
                    pdf_renderer_control.TextSelected += pdf_renderer_control_TextSelected;
                    PDFRendererControlArea.Children.Add(pdf_renderer_control);
                }
                else
                {
                    ObjNoPDFAvailableMessage.Visibility = Visibility.Visible;
                    PDFRendererControlArea.Visibility = Visibility.Collapsed;
                }

                // Make sure we have something to search for
                if (String.IsNullOrEmpty(search_terms))
                {
                    string title_combined = pdf_document.TitleCombined;
                    if (Constants.TITLE_UNKNOWN != title_combined && pdf_document.DownloadLocation != title_combined)
                    {
                        search_terms = pdf_document.TitleCombined;
                    }
                }

                // Kick off the search
                if (!String.IsNullOrEmpty(search_terms))
                {
                    ObjWebBrowser.DoWebSearch(search_terms);
                }
            }
        }

        private void pdf_renderer_control_TextSelected(string selected_text)
        {
            if (null != selected_text)
            {
                ObjWebBrowser.DoWebSearch(selected_text);
            }
        }

        private void GoogleBibTexSnifferControl_Closing(object sender, CancelEventArgs e)
        {
            ObjWebBrowser.PageLoaded -= ObjWebBrowser_PageLoaded;
            ObjWebBrowser.TabChanged -= ObjWebBrowser_TabChanged;
        }

        private void GoogleBibTexSnifferControl_Closed(object sender, EventArgs e)
        {
            Logging.Info("GoogleBibTexSnifferControl_Closed()");

            ObjWebBrowser.Dispose();

            user_specified_pdf_document = null;
            pdf_documents_total_pool.Clear();
            pdf_documents_search_pool.Clear();
            pdf_document = null;
            pdf_document_rendered = null;

            pdf_renderer_control?.Dispose();
            pdf_renderer_control = null;

            last_autonavigated_url = null;
        }

        private void ObjWebBrowser_PageLoaded()
        {
            Logging.Debug特("BibTexSniffer::Browser::Page Loaded: {0}", ObjWebBrowser.CurrentUri.AbsoluteUri);
            ReflectLatestBrowserContent();
            // When PDFs are viewed in Gecko/Firefox and somehow things went wrong the first time around,
            // but **not enough wrong** so to speak, then the PDF is **cached** by Gecko/FireFox and it WILL NOT
            // show up as one of the URIs being fetched for a page reload! The PDF will only show up **here**,
            // as a completely loaded document.
            //
            // Meanwhile the Acrobat Reader in there will cause the `ObjWebBrowser.CurrentPageHTML` to render
            // something like this:
            //
            // <html><head><meta content="width=device-width; height=device-height;" name="viewport"></head>
            // <body marginheight="0" marginwidth="0"><embed type="application/pdf"
            //    src ="https://escholarship.org/content/qt0cs6v2w7/qt0cs6v2w7.pdf"
            //    name ="plugin" height="100%" width="100%"></body></html>
            //
            // !Yay!          /sarcasm!/
            string uri = null;
            try
            {
                uri = ObjWebBrowser.CurrentUri.AbsoluteUri;
                // we also need to fetch nasty URIs like these ones:
                //
                //    http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.49.6383&rep=rep1&type=pdf
                //    https://digitalcommons.unl.edu/cgi/viewcontent.cgi?article=1130&context=cseconfwork
                //
                // hence we don't care about the exact extension '.pdf' but merely if it MIGHT be a PDF....
                if (uri != "about:blank") 			// if (uri.Contains(".pdf"))
                {
                    // fetch the PDF!
                    ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_ASYNCHRONOUS(CurrentLibrary, uri);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "fetch PDF failed for {0}", uri);
            }
        }

        private void ObjWebBrowser_TabChanged()
        {
            Logging.Debug特("BibTexSniffer::Browser::Tab Changed: {0}", ObjWebBrowser.CurrentUri.AbsoluteUri);
            ReflectLatestBrowserContent();
        }

        private void ReflectLatestBrowserContent()
        {
            try
            {
                // Neaten the text in the browser
                string text = ObjWebBrowser.CurrentPageText;

                if (null == text)
                {
                    text = "";
                }

                // Process
                text = text.Trim();

                // If this is valid BibTeX, offer it
                if (IsValidBibTex(text))
                {
                    FeatureTrackingManager.Instance.UseFeature(Features.MetadataSniffer_ValidBibTeX);
                    UseAsBibTeX(text);

                    return;
                }

                // If this is valid PubMed XML, offer it
                if (!String.IsNullOrEmpty(text))
                {
                    string converted_bibtex;
                    List<string> messages;
                    bool success = PubMedXMLToBibTex.TryConvert(text, out converted_bibtex, out messages);
                    if (success)
                    {
                        FeatureTrackingManager.Instance.UseFeature(Features.MetadataSniffer_ValidPubMed);
                        UseAsBibTeX(converted_bibtex);
                        return;
                    }
                    else
                    {
                        if (0 < messages.Count)
                        {
                            foreach (string message in messages)
                            {
                                Logging.Info("(PubMedXMLToBibTex) " + message);
                            }
                        }
                    }
                }

                // Otherwise let's try parse the page cos it might be a google scholar page
                // and if so we are going to want to try to get the first link to BibTeX
                if (ConfigurationManager.Instance.ConfigurationRecord.Metadata_UseBibTeXSnifferWizard)
                {
                    // Only do this automatically if there is not already bibtex in the record
                    if (null != pdf_document && String.IsNullOrEmpty(pdf_document.BibTex))
                    {
                        string url = ObjWebBrowser.CurrentUri.AbsoluteUri;
                        string html = ObjWebBrowser.CurrentPageHTML;
                        List<GoogleScholarScrapePaper> gssps = GoogleScholarScraper.ScrapeHtml(html, url);

                        try
                        {
                            // Try to process the first bibtex record
                            if (0 < gssps.Count)
                            {
                                GoogleScholarScrapePaper gssp = gssps[0];
                                if (!String.IsNullOrEmpty(gssp.bibtex_url))
                                {
                                    if (last_autonavigated_url != gssp.bibtex_url)
                                    {
                                        last_autonavigated_url = gssp.bibtex_url;

                                        gssp.bibtex_url = gssp.bibtex_url.Replace("&amp;", "&");
                                        ObjWebBrowser.OpenUrl(gssp.bibtex_url);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Warn(ex, "Sniffer was not able to parse the results that came back from GS.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was an exception while trying to parse the html back from Google Scholar");
            }
        }

        private DateTime bibtexsearch_backoff_timestamp = DateTime.MinValue;
        private string last_posted_bibtex = null;

        private void PostBibTeXToAggregator(string bibtex)
        {
            if (last_posted_bibtex == bibtex) return;
            if (bibtexsearch_backoff_timestamp > DateTime.UtcNow) return;

            // Post the bibtex to bibtexsearch.com
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(bibtex);

                Logging.Info("posting BibTeX info to bibtexsearch.com aggregator - URL: {0}, BibTeX = {1}", WebsiteAccess.Url_BibTeXSearch_Submit, bibtex);

                HttpWebRequest web_request = (HttpWebRequest)HttpWebRequest.Create(new Uri(WebsiteAccess.Url_BibTeXSearch_Submit));
                web_request.Proxy = ConfigurationManager.Instance.Proxy;
                web_request.Method = "POST";
                web_request.ContentLength = buffer.Length;
                web_request.ContentType = "text/plain; charset=utf-8";
#if false
                web_request.KeepAlive = false;
#endif
                // Allow ALL protocols
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                // same headers as sent by modern Chrome.
                // Gentlemen, start your prayer wheels!
                web_request.Headers.Add("Cache-Control", "no-cache");
                web_request.Headers.Add("Pragma", "no-cache");
                web_request.UserAgent = ConfigurationManager.Instance.ConfigurationRecord.GetWebUserAgent();

                using (Stream stream = web_request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }

                using (web_request.GetResponse())
                {
                }

                last_posted_bibtex = bibtex;

                //using (Stream stream = web_response.GetResponseStream())
                //{
                //    using (StreamReader stream_reader = new StreamReader(stream, Encoding.UTF8))
                //    {
                //        Logging.Info("bibtexsearch.com says:\n{0}", stream_reader.ReadToEnd());
                //    }
                //}
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem with bibtexsearch.com - URL: {0}", WebsiteAccess.Url_BibTeXSearch_Submit);
                bibtexsearch_backoff_timestamp = DateTime.UtcNow.AddHours(1);
            }
        }

        private void UseAsBibTeX(string text)
        {
            SafeThreadPool.QueueUserWorkItem(o => PostBibTeXToAggregator(text));

            if (null != pdf_document)
            {
                pdf_document.BibTex = text;
                pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
            }
            else
            {
                MessageBoxes.Error("Please first select a PDF in your library before trying to search for BibTeX.");
            }
        }

        private static bool IsValidBibTex(string text)
        {
            return (text.StartsWith("@") && text.EndsWith("}"));
        }

        private Library CurrentLibrary
        {
            get
            {
                if (null != user_specified_pdf_document && null != user_specified_pdf_document.Library)
                {
                    return user_specified_pdf_document.Library;
                }
                if (null != pdf_document_rendered && null != pdf_document_rendered.Library)
                {
                    return pdf_document_rendered.Library;
                }
                if (null != pdf_document && null != pdf_document.Library)
                {
                    return pdf_document.Library;
                }
                foreach (PDFDocument pdf in pdf_documents_total_pool)
                {
                    if (null != pdf && null != pdf.Library)
                    {
                        return pdf.Library;
                    }
                }
                return null;
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            Library library = Library.GuestInstance;
            Thread.Sleep(1000);

            GoogleBibTexSnifferControl c = new GoogleBibTexSnifferControl();
            PDFDocument pdf_document = library.PDFDocuments[0];
            //c.Show(pdf_document.Library, pdf_document, "test search term");
            //c.Show(pdf_document.Library, pdf_document);
            c.Show(pdf_document.Library.PDFDocuments);
        }
#endif

        #endregion

        private void ObjBibTeXEditorControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ObjBibTeXEditorControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

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

            Dispose();
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~GoogleBibTexSnifferControl()
        {
            Logging.Debug("~GoogleBibTexSnifferControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing GoogleBibTexSnifferControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("GoogleBibTexSnifferControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources / get rid of cyclic references:
                    pdf_documents_total_pool?.Clear();
                    pdf_documents_search_pool?.Clear();
                }
            });

            WPFDoEvents.SafeExec(() =>
            {
                search_options_bindable.PropertyChanged -= search_options_bindable_PropertyChanged;

                ObjWebBrowser.PageLoaded -= ObjWebBrowser_PageLoaded;
                ObjWebBrowser.TabChanged -= ObjWebBrowser_TabChanged;

                ObjBibTeXEditorControl.ObjBibTeXText.TextChanged -= TxtBibTeX_TextChanged;
            });

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    pdf_renderer_control?.Dispose();
                    ObjBibTeXEditorControl?.Dispose();
                }
            });

            WPFDoEvents.SafeExec(() =>
            {
                ObjSearchOptionsPanel.DataContext = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                ButtonWizard.DataContext = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                // Clear the references for sanity's sake
                search_options_bindable = null;

                user_specified_pdf_document = null;
                pdf_documents_total_pool = null;

                pdf_documents_search_pool = null;

                pdf_document = null;

                pdf_document_rendered = null;
                pdf_renderer_control = null;

                search_options = null;
                search_options_bindable = null;

                ObjBibTeXEditorControl = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                DataContext = null;
            });

            ++dispose_count;
        }

        #endregion

    }
}
