using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Microsoft.Win32;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.ReadOutLoud;
using Qiqqa.Common.SpeedRead;
using Qiqqa.Common.WebcastStuff;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.LibraryCatalog;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff;
using Qiqqa.Documents.PDF.PDFControls.PDFExporting;
using Qiqqa.Documents.PDF.PDFControls.Printing;
using Qiqqa.Documents.PDF.Search;
using Qiqqa.Exporting;
using Qiqqa.InCite;
using Qiqqa.Localisation;
using Qiqqa.UtilisationTracking;
using Qiqqa.WebBrowsing.GeckoStuff;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.GUI.Wizard;
using Utilities.Misc;
using Utilities.OCR;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Documents.PDF.PDFControls
{
    /// <summary>
    /// Interaction logic for PDFRendererControl.xaml
    /// </summary>
    public partial class PDFReadingControl : UserControl, IDisposable
    {
        private PDFRendererControl pdf_renderer_control = null;
        private PDFRendererControlStats pdf_renderer_control_stats = null;

        public PDFReadingControl(PDFDocument pdf_document)
        {
            InitializeComponent();

            GoogleScholarSideBar.Visibility = Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.GoogleScholar_DoExtraBackgroundQueries ? Visibility.Visible : Visibility.Collapsed;

            pdf_renderer_control = new PDFRendererControl(pdf_document, true);
            pdf_renderer_control.OperationModeChanged += pdf_renderer_control_OperationModeChanged;
            pdf_renderer_control.ZoomTypeChanged += pdf_renderer_control_ZoomTypeChanged;
            pdf_renderer_control.SelectedPageChanged += pdf_renderer_control_SelectedPageChanged;

            pdf_renderer_control_stats = new PDFRendererControlStats(pdf_renderer_control, pdf_document);

            Utilities.GUI.Animation.Animations.EnableHoverFade(ObjToolbarGrid);

            // Add the renderer control to our grid
            PDFRendererControlArea.Children.Add(pdf_renderer_control);

            HighlightCanvasToolbar.PDFRendererControl = pdf_renderer_control;
            InkCanvasToolbar.PDFRendererControl = pdf_renderer_control;
            TextCanvasToolbar.PDFRendererControl = pdf_renderer_control;

            KeyUp += PDFReadingControl_KeyUp;

            ButtonHand.Icon = Icons.GetAppIcon(Icons.Hand);
            ButtonHand.ToolTip = LocalisationManager.Get("PDF/TIP/MOVE_PAGE");
            ButtonHand.Click += ButtonHand_Click;

            ButtonTextSentenceSelect.Icon = Icons.GetAppIcon(Icons.TextSentenceSelect);
            ButtonTextSentenceSelect.ToolTip = LocalisationManager.Get("PDF/TIP/SELECT_TEXT");
            ButtonTextSentenceSelect.Click += ButtonTextSentenceSelect_Click;

            ButtonAnnotation.Icon = Icons.GetAppIcon(Icons.Annotation);
            ButtonAnnotation.ToolTip = LocalisationManager.Get("PDF/TIP/ADD_ANNOTATION");
            ButtonAnnotation.Click += ButtonAnnotation_Click;

            ButtonHighlighter.Icon = Icons.GetAppIcon(Icons.Highlighter);
            ButtonHighlighter.ToolTip = LocalisationManager.Get("PDF/TIP/ADD_HIGHLIGHT");
            ButtonHighlighter.Click += ButtonHighlighter_Click;

            ButtonCamera.Icon = Icons.GetAppIcon(Icons.Camera);
            ButtonCamera.ToolTip = LocalisationManager.Get("PDF/TIP/SNAPSHOT");
            ButtonCamera.Click += ButtonCamera_Click;

            ButtonInk.Icon = Icons.GetAppIcon(Icons.Ink);
            ButtonInk.ToolTip = LocalisationManager.Get("PDF/TIP/ADD_INK");
            ButtonInk.Click += ButtonInk_Click;

            ButtonExplore.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonExplore.AttachPopup(ButtonExplorePopup);
            ButtonExplore.Icon = Icons.GetAppIcon(Icons.Explore);
            ButtonExplore.ToolTip = LocalisationManager.Get("PDF/TIP/EXPLORE");

            ButtonExpedition.Icon = Icons.GetAppIcon(Icons.ModuleExpedition);
            ButtonExpedition.Caption = LocalisationManager.Get("PDF/TIP/EXPEDITION");
            ButtonExpedition.Click += ButtonExpedition_Click;

            ButtonExploreInBrainstorm.Icon = Icons.GetAppIcon(Icons.ModuleBrainstorm);
            ButtonExploreInBrainstorm.Caption = LocalisationManager.Get("PDF/TIP/BRAINSTORM");
            ButtonExploreInBrainstorm.Click += ButtonExploreInBrainstorm_Click;

            ButtonInCite.AttachPopup(ButtonInCitePopup);
            ButtonInCite.Icon = Icons.GetAppIcon(Icons.ModuleInCite);
            ButtonInCite.ToolTip = LocalisationManager.Get("PDF/TIP/INCITE");

            ButtonInCite_Word.Icon = Icons.GetAppIcon(Icons.InCiteNewCitation);
            ButtonInCite_Word.Caption = LocalisationManager.Get("PDF/CAP/CITE_WORD");
            ButtonInCite_Word.Click += ButtonInCite_Word_Click;

            ButtonInCite_WordSeparate.Icon = Icons.GetAppIcon(Icons.InCiteNewCitation);
            ButtonInCite_WordSeparate.Caption = LocalisationManager.Get("PDF/CAP/CITE_WORD_SEPARATE");
            ButtonInCite_WordSeparate.Click += ButtonInCite_WordSeparate_Click;

            ButtonInCite_Snippet.Icon = Icons.GetAppIcon(Icons.InCiteCitationSnippet);
            ButtonInCite_Snippet.Caption = LocalisationManager.Get("PDF/CAP/CITE_SNIPPET");
            ButtonInCite_Snippet.Click += ButtonInCite_Snippet_Click;

            ButtonInCite_BibTeXKey.Icon = Icons.GetAppIcon(Icons.ExportBibTex);
            ButtonInCite_BibTeXKey.Caption = LocalisationManager.Get("PDF/CAP/CITE_BIBTEX");
            ButtonInCite_BibTeXKey.Click += ButtonInCite_BibTeXKey_Click;

            ButtonFullScreen.Icon = Icons.GetAppIcon(Icons.DocumentFullScreen);
            ButtonFullScreen.ToolTip = LocalisationManager.Get("PDF/TIP/FULL_SCREEN");
            ButtonFullScreen.Click += ButtonFullScreen_Click;

            ButtonZoom.AttachPopup(ButtonZoomPopup);
            ButtonZoom.Icon = Icons.GetAppIcon(Icons.ZoomIn);
            ButtonZoom.ToolTip = LocalisationManager.Get("PDF/TIP/ZOOM");

            Button1Up.Icon = Icons.GetAppIcon(Icons.Page1Up);
            Button1Up.Caption = LocalisationManager.Get("PDF/TIP/ZOOM_1");
            Button1Up.Click += Button1Up_Click;

            Button2Up.Icon = Icons.GetAppIcon(Icons.Page2Up);
            Button2Up.Caption = LocalisationManager.Get("PDF/TIP/ZOOM_2");
            Button2Up.Click += Button2Up_Click;

            ButtonNUp.Icon = Icons.GetAppIcon(Icons.PageNUp);
            ButtonNUp.Caption = LocalisationManager.Get("PDF/TIP/ZOOM_N");
            ButtonNUp.Click += ButtonNUp_Click;

            ButtonWholeUp.Icon = Icons.GetAppIcon(Icons.PageWholeUp);
            ButtonWholeUp.Caption = LocalisationManager.Get("PDF/TIP/ZOOM_WHOLE");
            ButtonWholeUp.Click += ButtonWholeUp_Click;

            ButtonRotate.Icon = Icons.GetAppIcon(Icons.PageRotate);
            ButtonRotate.Caption = LocalisationManager.Get("PDF/TIP/ROTATE");
            ButtonRotate.Click += ButtonRotate_Click;

            ButtonRotateAll.Icon = Icons.GetAppIcon(Icons.PageRotate);
            ButtonRotateAll.Caption = LocalisationManager.Get("PDF/TIP/ROTATE_ALL");
            ButtonRotateAll.Click += ButtonRotateAll_Click;

            ButtonZoomIn.Icon = Icons.GetAppIcon(Icons.ZoomIn);
            ButtonZoomIn.Caption = LocalisationManager.Get("PDF/TIP/ZOOM_IN");
            ButtonZoomIn.Click += ButtonZoomIn_Click;

            ButtonZoomOut.Icon = Icons.GetAppIcon(Icons.ZoomOut);
            ButtonZoomOut.Caption = LocalisationManager.Get("PDF/TIP/ZOOM_OUT");
            ButtonZoomOut.Click += ButtonZoomOut_Click;

            ButtonMisc.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonMisc.AttachPopup(ButtonMiscPopup);
            ButtonMisc.Icon = Icons.GetAppIcon(Icons.DocumentMisc);
            ButtonMisc.ToolTip = LocalisationManager.Get("PDF/TIP/MISC");

            ButtonDocumentSave.Icon = Icons.GetAppIcon(Icons.DocumentSave);
            ButtonDocumentSave.Caption = LocalisationManager.Get("PDF/TIP/SAVE_COPY");
            ButtonDocumentSave.Click += ButtonDocumentSave_Click;

            ButtonPrint.Icon = Icons.GetAppIcon(Icons.Printer);
            ButtonPrint.Caption = LocalisationManager.Get("PDF/TIP/PRINT");
            ButtonPrint.Click += ButtonPrint_Click;

            ButtonOpenLibrary.Icon = Icons.GetAppIcon(Icons.ModuleDocumentLibrary);
            ButtonOpenLibrary.Caption = LocalisationManager.Get("PDF/TIP/OPEN_PARENT_LIBRARY");
            ButtonOpenLibrary.Click += ButtonOpenLibrary_Click;

            ButtonExportToText.Icon = Icons.GetAppIcon(Icons.ExportToText);
            ButtonExportToText.Caption = LocalisationManager.Get("PDF/TIP/CONVERT_TO_TEXT");
            ButtonExportToText.Click += ButtonExportToText_Click;

            ButtonReadOutLoud.Icon = Icons.GetAppIcon(Icons.ReadOutLoud);
            ButtonReadOutLoud.Caption = LocalisationManager.Get("PDF/TIP/READ_ALOUD");
            ButtonReadOutLoud.Click += ButtonReadOutLoud_Click;

            ButtonSpeedRead.Icon = Icons.GetAppIcon(Icons.SpeedRead);
            ButtonSpeedRead.Caption = LocalisationManager.Get("PDF/TIP/SPEED_READ");
            ButtonSpeedRead.Click += ButtonSpeedRead_Click;

            ButtonInvertColours.Icon = Icons.GetAppIcon(Icons.DocumentsInvertColours);
            ButtonInvertColours.Caption = LocalisationManager.Get("PDF/TIP/NEGATIVE");
            ButtonInvertColours.IsChecked = false;
            ButtonInvertColours.Click += ButtonInvertColours_Click;

            ButtonMoreMenus.Icon = Icons.GetAppIcon(Icons.DocumentMisc);
            ButtonMoreMenus.Caption = LocalisationManager.Get("PDF/TIP/MORE_MENUS");
            ButtonMoreMenus.Click += ButtonMoreMenus_Click;

            ButtonJumpToSection.Icon = Icons.GetAppIcon(Icons.JumpToSection);
            ButtonJumpToSection.ToolTip = LocalisationManager.Get("PDF/TIP/BOOKMARKS");
            ButtonJumpToSection.Click += ButtonJumpToSection_Click;

            ButtonPreviousPage.Icon = Icons.GetAppIcon(Icons.Previous);
            ButtonPreviousPage.ToolTip = LocalisationManager.Get("PDF/TIP/PAGE_PREV");
            ButtonPreviousPage.Click += ButtonPreviousPage_Click;

            ButtonNextPage.Icon = Icons.GetAppIcon(Icons.Next);
            ButtonNextPage.ToolTip = LocalisationManager.Get("PDF/TIP/PAGE_NEXT");
            ButtonNextPage.Click += ButtonNextPage_Click;

            TextBoxFind.ToolTip = LocalisationManager.Get("PDF/TIP/SEARCH");
            TextBoxFind.OnHardSearch += TextBoxFind_OnHardSearch;

            Webcasts.FormatWebcastButton(ButtonWebcast, Webcasts.PDF_VIEWER);

            // Make some space
            ToolBar.SetOverflowMode(ButtonPrint, OverflowMode.Always);
            ToolBar.SetOverflowMode(ButtonInvertColours, OverflowMode.Always);
            ToolBar.SetOverflowMode(ButtonCamera, OverflowMode.Always);
            ToolBar.SetOverflowMode(ButtonDocumentSave, OverflowMode.Always);
            ToolBar.SetOverflowMode(ButtonExportToText, OverflowMode.Always);
            ToolBar.SetOverflowMode(ButtonReadOutLoud, OverflowMode.Always);
            ToolBar.SetOverflowMode(ButtonMoreMenus, OverflowMode.Always);

            // Wizard
            WizardDPs.SetPointOfInterest(ButtonAnnotation, "PDFReadingAnnotationButton");

            ListSearchDetails.SearchSelectionChanged += ListSearchDetails_SearchSelectionChanged;
            ListSearchDetails.SearchClicked += ListSearchDetails_SearchSelectionChanged;

            TagCloud.TagClick += TagCloud_TagClick;

            JumpToPageNumber.Text = "" + 1;
            JumpToPageNumberMax.Text = " of " + pdf_renderer_control_stats.pdf_document.PDFRenderer.PageCount;
            JumpToPageNumber.KeyDown += JumpToPageNumber_KeyDown;
            JumpToPageNumber.KeyUp += JumpToPageNumber_KeyUp;
            JumpToPageNumber.GotFocus += JumpToPageNumber_GotFocus;
            string tooltip = LocalisationManager.Get("PDF/TIP/PAGE_JUMP");
            JumpToPageNumberLabel.ToolTip = tooltip;
            JumpToPageNumber.ToolTip = tooltip;

            // The search results are initially hidden
            GridBOTTOM.Visibility = Visibility.Collapsed;

            // Start in hand mode
            pdf_renderer_control.ReconsiderOperationMode(PDFRendererControl.OperationMode.Hand);

            ObjHyperlinkGuestPreviewMoveOther.Click += ObjHyperlinkGuestPreviewMoveOther_Click;
            ObjHyperlinkGuestPreviewMoveDefault.Click += ObjHyperlinkGuestPreviewMoveDefault_Click;
            ObjHyperlinkGuestPreviewVanillaAttach.Click += ObjHyperlinkGuestPreviewVanillaAttach_Click;

            ObjReadOnlyInfoBar.Visibility = pdf_document.Library.WebLibraryDetail.IsReadOnly ? Visibility.Visible : Visibility.Collapsed;

            DataContext = pdf_document.Bindable;

            ObjDocumentMetadataControlsPanel.SelectedPageChanged += ObjDocumentMetadataControlsPanel_ObjDocumentMetadataControlsPanel_SelectedPageChanged;

            // Kick off a thread that populates the interesting analysis
            SafeThreadPool.QueueUserWorkItem(o => PDFRendererControlInterestingAnalysis.DoInterestingAnalysis(this, pdf_renderer_control, pdf_renderer_control_stats));

            Loaded += PDFReadingControl_Loaded;
        }

        private void PDFReadingControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfigurationManager.Instance.ConfigurationRecord.GUI_IsNovice)
            {
                GridRIGHT.Collapse();
            }
            else
            {
                GridRIGHT.Restore();
            }
        }

        private void ObjDocumentMetadataControlsPanel_ObjDocumentMetadataControlsPanel_SelectedPageChanged(int page)
        {
            pdf_renderer_control.SelectPage(page);
        }

        public PDFRendererControlStats PDFRendererControlStats => pdf_renderer_control_stats;

        private void ButtonInCite_Word_Click(object sender, RoutedEventArgs e)
        {
            ButtonInCitePopup.Close();
            {
                FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitation_FromDocument);
                PDFDocumentCitingTools.CitePDFDocument(pdf_renderer_control_stats.pdf_document, false);
                e.Handled = true;
            }
        }

        private void ButtonInCite_WordSeparate_Click(object sender, RoutedEventArgs e)
        {
            ButtonInCitePopup.Close();
            {
                FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitation_FromDocument);
                PDFDocumentCitingTools.CitePDFDocument(pdf_renderer_control_stats.pdf_document, true);
                e.Handled = true;
            }
        }

        private void ButtonInCite_Snippet_Click(object sender, RoutedEventArgs e)
        {
            ButtonInCitePopup.Close();
            {
                FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitationSnippet_FromDocument);
                PDFDocumentCitingTools.CiteSnippetPDFDocument(false, pdf_renderer_control_stats.pdf_document);
                e.Handled = true;
            }
        }

        private void ButtonInCite_BibTeXKey_Click(object sender, RoutedEventArgs e)
        {
            ButtonInCitePopup.Close();
            {
                if (!String.IsNullOrEmpty(pdf_renderer_control_stats.pdf_document.BibTexKey))
                {
                    string result = @"\cite{" + pdf_renderer_control_stats.pdf_document.BibTexKey + @"}";
                    ClipboardTools.SetText(result);
                    StatusManager.Instance.UpdateStatus("CopyBibTeXKey", String.Format("Copied '{0}' to clipboard.", result));

                }

                e.Handled = true;
            }
        }

        private void ButtonExpedition_Click(object sender, RoutedEventArgs e)
        {
            ButtonExplorePopup.Close();
            {

                FeatureTrackingManager.Instance.UseFeature(Features.Expedition_Open_Document);
                MainWindowServiceDispatcher.Instance.OpenExpedition(pdf_renderer_control_stats.pdf_document.Library, pdf_renderer_control_stats.pdf_document);
            }
        }

        private void ButtonOpenLibrary_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenLibrary(pdf_renderer_control_stats.pdf_document.Library);
        }

        #region IDisposable

        ~PDFReadingControl()
        {
            Logging.Debug("~PDFReadingControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing PDFReadingControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFReadingControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources
                    PDFRendererControlArea.Children.Clear();

                    pdf_renderer_control?.Dispose();

                    pdf_renderer_control_stats?.pdf_document.PDFRenderer.FlushCachedPageRenderings();
                }
            }, must_exec_in_UI_thread: true);

            WPFDoEvents.SafeExec(() =>
            {
                pdf_renderer_control = null;
                pdf_renderer_control_stats = null;
            });

            ++dispose_count;
        }

        #endregion

        private void pdf_renderer_control_OperationModeChanged(PDFRendererControl.OperationMode operation_mode)
        {
            // Reset the toggle buttons
            ButtonHand.IsChecked = false;
            ButtonTextSentenceSelect.IsChecked = false;
            ButtonAnnotation.IsChecked = false;
            ButtonHighlighter.IsChecked = false;
            ButtonCamera.IsChecked = false;
            ButtonInk.IsChecked = false;

            // Hide the various toolboxes
            InkCanvasToolbarBorder.Visibility = Visibility.Collapsed;
            HighlightCanvasToolbarBorder.Visibility = Visibility.Collapsed;
            TextCanvasToolbarBorder.Visibility = Visibility.Collapsed;

            // Set the selected toggle button
            switch (operation_mode)
            {
                case PDFRendererControl.OperationMode.Hand:
                    ButtonHand.IsChecked = true;
                    break;
                case PDFRendererControl.OperationMode.Annotation:
                    ButtonAnnotation.IsChecked = true;
                    break;
                case PDFRendererControl.OperationMode.Highlighter:
                    ButtonHighlighter.IsChecked = true;
                    HighlightCanvasToolbarBorder.Visibility = Visibility.Visible;
                    break;
                case PDFRendererControl.OperationMode.Camera:
                    ButtonCamera.IsChecked = true;
                    break;
                case PDFRendererControl.OperationMode.Ink:
                    ButtonInk.IsChecked = true;
                    InkCanvasToolbarBorder.Visibility = Visibility.Visible;
                    break;
                case PDFRendererControl.OperationMode.TextSentenceSelect:
                    ButtonTextSentenceSelect.IsChecked = true;
                    TextCanvasToolbarBorder.Visibility = Visibility.Visible;
                    break;
                default:
                    Logging.Warn("Unknown operation mode {0}", operation_mode);
                    break;
            }
        }

        private void pdf_renderer_control_ZoomTypeChanged(PDFRendererControl.ZoomType zoom_type)
        {
            Button1Up.IsChecked = false;
            Button2Up.IsChecked = false;
            ButtonNUp.IsChecked = false;
            ButtonWholeUp.IsChecked = false;

            switch (zoom_type)
            {
                case PDFRendererControl.ZoomType.Zoom1Up:
                    Button1Up.IsChecked = true;
                    break;
                case PDFRendererControl.ZoomType.Zoom2Up:
                    Button2Up.IsChecked = true;
                    break;
                case PDFRendererControl.ZoomType.ZoomNUp:
                    ButtonNUp.IsChecked = true;
                    break;
                case PDFRendererControl.ZoomType.ZoomWholeUp:
                    ButtonWholeUp.IsChecked = true;
                    break;
                case PDFRendererControl.ZoomType.Other:
                    break;
                default:
                    Logging.Warn("Unknown zoom type {0}", zoom_type);
                    break;
            }
        }

        private void pdf_renderer_control_SelectedPageChanged(int page)
        {
            JumpToPageNumber.Text = "" + page;
        }

        private void JumpToPageNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            JumpToPageNumber.SelectAll();
        }

        private void JumpToPageNumber_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Add:
                    DoJumpToPageNumber(+1);
                    e.Handled = true;
                    break;

                case Key.Subtract:
                    DoJumpToPageNumber(-1);
                    e.Handled = true;
                    break;
            }
        }

        private void JumpToPageNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoJumpToPageNumber(0);
            }
        }

        private void DoJumpToPageNumber(int offset)
        {
            try
            {
                int page_number = Int32.Parse(JumpToPageNumber.Text) + offset;
                if (page_number < 1) page_number = pdf_renderer_control_stats.pdf_document.PDFRenderer.PageCount;
                if (page_number > pdf_renderer_control_stats.pdf_document.PDFRenderer.PageCount) page_number = 1;

                pdf_renderer_control.MoveSelectedPageAbsolute(page_number);

                JumpToPageNumber.Text = "" + page_number;
                JumpToPageNumber.SelectAll();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error jumping to manual page number '{0}'.", JumpToPageNumber.Text);
            }
        }

        private void PDFReadingControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (KeyboardTools.IsCTRLDown() && e.Key == Key.G)
            {
                JumpToPageNumber.Focus();
                e.Handled = true;
            }
            else if (KeyboardTools.IsCTRLDown() && e.Key == Key.F)
            {
                SetSearchKeywords();    // TODO: ***
                e.Handled = true;
            }
            else if (e.Key == Key.F11)
            {
                ToggleFullScreen();
                e.Handled = true;
            }
            // else forward it to the render control...?
            else
            {
                pdf_renderer_control.PDFRendererControl_KeyUp(sender, e);
            }
        }

        #region --- Mouse operation mode --------------------------------------------------------------------------------------------------------

        private void ButtonTextSentenceSelect_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.ReconsiderOperationMode(PDFRendererControl.OperationMode.TextSentenceSelect);
        }

        private JumpToSectionPopup jtsp = null;

        private void ButtonJumpToSection_Click(object sender, RoutedEventArgs e)
        {
            if (null == jtsp)
            {
                Logging.Info("Building popup for first time");
                jtsp = new JumpToSectionPopup(this, pdf_renderer_control, pdf_renderer_control_stats);
            }

            jtsp.Open();
        }

        private void ButtonAnnotation_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.ReconsiderOperationMode(PDFRendererControl.OperationMode.Annotation);
        }

        private void ButtonHighlighter_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.ReconsiderOperationMode(PDFRendererControl.OperationMode.Highlighter);
        }

        private void ButtonInk_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.ReconsiderOperationMode(PDFRendererControl.OperationMode.Ink);
        }

        private void ButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.ReconsiderOperationMode(PDFRendererControl.OperationMode.Camera);
        }

        private void ButtonHand_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.ReconsiderOperationMode(PDFRendererControl.OperationMode.Hand);
        }

        private void ButtonExploreInBrainstorm_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonExplorePopup))
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Document_ExploreDocumentInBrainstorm);
                MainWindowServiceDispatcher.Instance.ExploreDocumentInBrainstorm(pdf_renderer_control_stats.pdf_document);
            }
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            if (null != pdf_renderer_control_stats && null != pdf_renderer_control_stats.pdf_document)
            {
                PDFPrinter.Print(pdf_renderer_control_stats.pdf_document, pdf_renderer_control_stats.pdf_document.PDFRenderer, "PDFRenderer");
            }
        }

        private void ButtonDocumentSave_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_Save);

            string filename = ExportingTools.MakeExportFilename(pdf_renderer_control_stats.pdf_document);

            //string desktop_directory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //filename = Path.GetFullPath(Path.Combine(desktop_directory, filename));

            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.AddExtension = true;
            save_file_dialog.CheckPathExists = true;
            save_file_dialog.DereferenceLinks = true;
            save_file_dialog.OverwritePrompt = true;
            save_file_dialog.ValidateNames = true;
            save_file_dialog.DefaultExt = "pdf";
            save_file_dialog.Filter = "PDF files|*.pdf" + "|" + "All files|*.*";
            save_file_dialog.FileName = filename;

            if (true == save_file_dialog.ShowDialog())
            {
                Logging.Info("Saving PDF with fingerprint {1} file to {0}", save_file_dialog.FileName, pdf_renderer_control_stats.pdf_document.Fingerprint);
                File.Copy(pdf_renderer_control_stats.pdf_document.DocumentPath, save_file_dialog.FileName);
            }
        }

        private void TagCloud_TagClick(List<string> tags)
        {
            string search_terms = ArrayFormatter.ListElements(tags, " ", "\"", "\"");
            SetSearchKeywords(search_terms);
        }

        private void TextBoxFind_OnHardSearch()
        {
            SetSearchKeywords();
        }

        private void ListSearchDetails_SearchSelectionChanged(PDFSearchResult search_result)
        {
            pdf_renderer_control.FlashSelectedSearchItem(search_result);
        }

        public void SetSearchKeywords(string keywords)
        {
            TextBoxFind.Text = keywords;
            SetSearchKeywords();
        }

        public void SetSearchKeywords()
        {
            TextBoxFind.FocusSearchArea();
            SetSearchKeywords_EXECUTE(TextBoxFind.Text);
        }

        private string previous_search_string = null;
        private PDFSearchResultSet previous_search_result_set = null;

        private void SetSearchKeywords_EXECUTE(string search_string)
        {
            // Check if we are repeating the search or not...
            if (previous_search_string != search_string)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Document_Search);
                previous_search_string = search_string;
                previous_search_result_set = PDFSearcher.Search(pdf_renderer_control_stats.pdf_document, search_string);
            }
            else
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Document_SearchAgain);
            }

            PDFSearchResultSet search_result_set = previous_search_result_set;

            // Set the PDF viewer search results
            pdf_renderer_control.SetSearchKeywords(search_result_set);

            // Set the bottom list box search results
            if (null != search_result_set && search_result_set.Count > 0)
            {
                GridBOTTOM.Visibility = Visibility.Visible;
            }
            else
            {
                GridBOTTOM.Visibility = Visibility.Collapsed;
            }

            ListSearchDetails.DataContext = search_result_set.AsList();
        }

        public void SelectPage(int page)
        {
            pdf_renderer_control.SelectPage(page);
        }

        #endregion

        #region --- Page navigation --------------------------------------------------------------------------------------------------------

        private void ButtonPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.MoveSelectedPageDelta(-1);
        }

        private void ButtonNextPage_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.MoveSelectedPageDelta(+1);
        }

        #endregion

        private void Button1Up_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.PageZoom(PDFRendererControl.ZoomType.Zoom1Up);
        }

        private void Button2Up_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.PageZoom(PDFRendererControl.ZoomType.Zoom2Up);
        }

        private void ButtonNUp_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.PageZoom(PDFRendererControl.ZoomType.ZoomNUp);
        }

        private void ButtonWholeUp_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.PageZoom(PDFRendererControl.ZoomType.ZoomWholeUp);
        }

        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.IncrementalZoom(-1);
        }

        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.IncrementalZoom(+1);
        }

        private void ButtonInvertColours_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.InvertColours(ButtonInvertColours.IsChecked.Value);
        }

        private void ButtonRotate_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.RotatePage();
        }

        private void ButtonRotateAll_Click(object sender, RoutedEventArgs e)
        {
            pdf_renderer_control.RotateAllPages();
        }

        private void ButtonMoreMenus_Click(object sender, RoutedEventArgs e)
        {
            LibraryCatalogPopup popup = new LibraryCatalogPopup(new List<PDFDocument> { pdf_renderer_control_stats.pdf_document });
            popup.Open();
            e.Handled = true;
        }

        #region --- Export-to-text ------------------------------------------------------------------------------------------------------------------------------------------

        private void ButtonExportToText_Click(object sender, RoutedEventArgs e)
        {
            //ExportToText.ExportToTextAndLaunch(this.pdf_renderer_control_stats.pdf_document);
            ExportToWord.ExportToTextAndLaunch(pdf_renderer_control_stats.pdf_document);
            e.Handled = true;
        }

        #endregion

        #region --- Speed read and text-to-speech ------------------------------------------------------------------------------------------------------------------------------------------

        private void GetCombinedWordsList(List<string> words, List<int> page_word_offsets, int single_page_only = -1)
        {
            if (null != pdf_renderer_control_stats)
            {
                int start_page = 0;
                int end_page = pdf_renderer_control_stats.pdf_document.SafePageCount - 1;

                if (-1 != single_page_only)
                {
                    start_page = single_page_only;
                    end_page = single_page_only;
                }

                for (int page = start_page; page <= end_page; ++page)
                {
                    WordList words_on_page = pdf_renderer_control_stats.pdf_document.PDFRenderer.GetOCRText(page + 1);
                    page_word_offsets.Add(words.Count);
                    if (null != words_on_page)
                    {
                        foreach (Word word in words_on_page)
                        {
                            words.Add(word.Text);
                        }
                    }
                }
            }
        }

        private void ButtonSpeedRead_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_SpeedRead);

            List<string> words = new List<string>();
            List<int> page_word_offsets = new List<int>();
            GetCombinedWordsList(words, page_word_offsets);

            SpeedReadControl src = MainWindowServiceDispatcher.Instance.OpenSpeedRead();
            src.UseText(words);
        }

        private void ButtonReadOutLoud_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_ReadOutLoud);

            AugmentedToggleButton button = (AugmentedToggleButton)sender;

            if (null != button.IsChecked && button.IsChecked.Value)
            {
                if (null != pdf_renderer_control_stats && null != pdf_renderer_control.SelectedPage)
                {
                    int page = pdf_renderer_control.SelectedPage.PageNumber;
                    WordList words = pdf_renderer_control_stats.pdf_document.PDFRenderer.GetOCRText(page);
                    if (null != words)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var word in words)
                        {
                            sb.Append(word.Text);
                            sb.Append(' ');
                        }

                        ReadOutLoudManager.Instance.Read(sb.ToString());
                    }
                    else
                    {
                        Logging.Info("No OCR to read");
                        ReadOutLoudManager.Instance.Read("Please run OCR on this page before it can be read.");
                    }
                }
                else
                {
                    Logging.Info("No page selected to read");
                    ReadOutLoudManager.Instance.Read("Please select the page that you wish to have read to you.");
                }
            }
            else
            {
                ReadOutLoudManager.Instance.Pause();
            }
        }

        #endregion

        #region --- Full screen mode -------------------------------------------------------------------------------------------------------------

        private void ToggleFullScreen()
        {
            ButtonFullScreen.IsChecked = !(ButtonFullScreen.IsChecked ?? false);
            ReevaluateFullScreen();
        }

        private void ReevaluateFullScreen()
        {
            if (ButtonFullScreen.IsChecked ?? false)
            {
                // Go full screen
                GridLEFT.Collapse();
                GridRIGHT.Collapse();
                pdf_renderer_control.PageZoom(PDFRendererControl.ZoomType.Zoom1Up);
            }
            else
            {
                // Revert
                GridLEFT.Restore();
                GridRIGHT.Restore();
            }
        }

        private void ButtonFullScreen_Click(object sender, RoutedEventArgs e)
        {
            ReevaluateFullScreen();
        }

        #endregion

        internal void EnableGuestMoveNotification(PDFDocument potential_attachment_pdf_document = null)
        {
            ObjGuestPreviewMove.Visibility = Visibility.Visible;

            // Get the MRU web library details
            ObjHyperlinkGuestPreviewMoveDefault.Visibility = Visibility.Collapsed;
            List<WebLibraryDetail> web_library_details = new List<WebLibraryDetail>();
            web_library_details.AddRange(WebLibraryManager.Instance.WebLibraryDetails_All_IncludingDeleted);
            WebLibraryManager.Instance.SortWebLibraryDetailsByLastAccessed(web_library_details);
            if (0 < web_library_details.Count)
            {
                WebLibraryDetail web_library_detail = web_library_details[0];
                if (WebLibraryManager.Instance.WebLibraryDetails_Guest != web_library_detail)
                {
                    ObjHyperlinkGuestPreviewMoveDefault.Content = "Move to " + web_library_detail.Title;
                    ObjHyperlinkGuestPreviewMoveDefault.Tag = web_library_detail;
                    ObjHyperlinkGuestPreviewMoveDefault.Visibility = Visibility.Visible;
                }
            }

            ObjHyperlinkGuestPreviewVanillaAttach.Visibility = System.Windows.Visibility.Collapsed;
            if (null != potential_attachment_pdf_document)
            {
                ObjHyperlinkGuestPreviewVanillaAttach.Content = String.Format("Attach to Vanilla Reference '{0}'", potential_attachment_pdf_document.TitleCombined);
                ObjHyperlinkGuestPreviewVanillaAttach.Tag = potential_attachment_pdf_document;
                ObjHyperlinkGuestPreviewVanillaAttach.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void ObjHyperlinkGuestPreviewMoveDefault_Click(object sender, RoutedEventArgs e)
        {
            WebLibraryDetail web_library_detail = ObjHyperlinkGuestPreviewMoveDefault.Tag as WebLibraryDetail;
            if (null != web_library_detail)
            {
                MoveGuestPreviewPDFDocument(web_library_detail);
            }
        }

        private void ObjHyperlinkGuestPreviewVanillaAttach_Click(object sender, RoutedEventArgs e)
        {
            PDFDocument potential_attachment_pdf_document = ObjHyperlinkGuestPreviewVanillaAttach.Tag as PDFDocument;
            if (null != potential_attachment_pdf_document)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Library_AttachToVanilla_Web);

                PDFDocument source_pdf_document = pdf_renderer_control_stats.pdf_document;
                PDFDocument cloned_pdf_document = potential_attachment_pdf_document.AssociatePDFWithVanillaReference(pdf_renderer_control_stats.pdf_document.DocumentPath);

                // Close the old
                MainWindowServiceDispatcher.Instance.ClosePDFReadingControl(this);

                // Delete the old
                if (cloned_pdf_document != source_pdf_document)
                {
                    source_pdf_document.Deleted = true;
                    source_pdf_document.Bindable.NotifyPropertyChanged(nameof(source_pdf_document.Deleted));
                }

                // Forget the target attachment
                PDFInterceptor.Instance.PotentialAttachmentPDFDocument = null;
            }
        }

        private void ObjHyperlinkGuestPreviewMoveOther_Click(object sender, RoutedEventArgs e)
        {
            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != web_library_detail)
            {
                MoveGuestPreviewPDFDocument(web_library_detail);
            }
        }

        private void MoveGuestPreviewPDFDocument(WebLibraryDetail web_library_detail)
        {
            PDFDocument source_pdf_document = pdf_renderer_control_stats.pdf_document;

            SafeThreadPool.QueueUserWorkItem(o =>
            {
                PDFDocument cloned_pdf_document = ImportingIntoLibrary.ClonePDFDocumentsFromOtherLibrary_SYNCHRONOUS(source_pdf_document, web_library_detail.library, false);

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    // Open the new
                    if (null != cloned_pdf_document)
                    {
                        MainWindowServiceDispatcher.Instance.OpenDocument(cloned_pdf_document);
                    }
                    else
                    {
                        MessageBoxes.Warn("There was a problem moving this document to another library.");
                    }

                    // Close the old
                    MainWindowServiceDispatcher.Instance.ClosePDFReadingControl(this);

                    // Delete the old
                    if (cloned_pdf_document != source_pdf_document)
                    {
                        source_pdf_document.Deleted = true;
                        source_pdf_document.Bindable.NotifyPropertyChanged(nameof(source_pdf_document.Deleted));
                    }
                });
            });
        }
    }
}
