using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.Common;
using Qiqqa.Documents.Common;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.InfoBarStuff.CitationsStuff;
using Qiqqa.Documents.PDF.Search;
using Qiqqa.Expedition;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.GUI.Wizard;
using Utilities.Reflection;

namespace Qiqqa.DocumentLibrary.LibraryCatalog
{
    /// <summary>
    /// Interaction logic for LibraryCatalogOverviewControl.xaml
    /// </summary>
    public partial class LibraryCatalogOverviewControl : Grid, IDisposable
    {
        private LibraryIndexHoverPopup library_index_hover_popup = null;
        private DragDropHelper drag_drop_helper = null;

        public LibraryCatalogOverviewControl()
        {
            Theme.Initialize();

            InitializeComponent();

            WizardDPs.SetPointOfInterest(PanelSearchScore, "LibrarySearchScoreButton");
            WizardDPs.SetPointOfInterest(ObjLookInsidePanel, "LibrarySearchDetails");

            ObjFavouriteImage.Source = Icons.GetAppIcon(Icons.Favourite);
            ObjFavouriteImage.ToolTip = "You love it!  This is one of your favourite references.";
            RenderOptions.SetBitmapScalingMode(ObjFavouriteImage, BitmapScalingMode.HighQuality);

            TextTitle.Cursor = Cursors.Hand;
            TextTitle.MouseLeftButtonUp += TextTitle_MouseLeftButtonUp;

            ButtonOpen.ToolTipOpening += HyperlinkPreview_ToolTipOpening;
            ButtonOpen.ToolTipClosing += HyperlinkPreview_ToolTipClosing;
            ButtonOpen.ToolTip = "";
            ButtonOpen.Cursor = Cursors.Hand;

            ButtonSearchInside.IconVisibility = Visibility.Collapsed;
            ButtonSearchInside.Click += ButtonSearchInside_Click;
            ObjLookInsidePanel.Visibility = Visibility.Collapsed;

            ButtonThemeSwatch.ToolTip = "This swatch shows the themes in this document.  If the swatch is grey, there is no Expedition information for this document - please run Expedition again!\n\nClick here to open the document in Expedition.";
            ButtonThemeSwatch.Click += ButtonThemeSwatch_Click;

            ButtonOpen.Click += ButtonOpen_Click;

            ListSearchDetails.SearchClicked += ListSearchDetails_SearchSelectionChanged;

            drag_drop_helper = new DragDropHelper(ButtonOpen, GetDocumentDragData);

            MouseRightButtonUp += LibraryCatalogOverviewControl_MouseRightButtonUp;
            MouseEnter += LibraryCatalogOverviewControl_MouseEnter;
            MouseLeave += LibraryCatalogOverviewControl_MouseLeave;

            DataContextChanged += LibraryCatalogOverviewControl_DataContextChanged;
        }

        private void ButtonThemeSwatch_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = PDFDocumentBindable;
            if (null != pdf_document_bindable)
            {
                MainWindowServiceDispatcher.Instance.OpenExpedition(pdf_document_bindable.Underlying.Library, pdf_document_bindable.Underlying);
            }
        }

        private void TextTitle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenPDFDocument();
            e.Handled = true;
        }

        private void LibraryCatalogOverviewControl_MouseLeave(object sender, MouseEventArgs e)
        {
            LibraryCatalogOverviewControl c = sender as LibraryCatalogOverviewControl;
            c.Background = Brushes.Transparent;
        }

        private void LibraryCatalogOverviewControl_MouseEnter(object sender, MouseEventArgs e)
        {
            LibraryCatalogOverviewControl c = sender as LibraryCatalogOverviewControl;
            c.Background = ThemeColours.Background_Brush_Blue_LightToDark;
        }

        private LibraryCatalogControl _library_catalog_control;
        private LibraryCatalogControl LibraryCatalogControl
        {
            get
            {
                try
                {
                    if (null == _library_catalog_control)
                    {
                        _library_catalog_control = GUITools.GetParentControl<LibraryCatalogControl>(this);
                    }
                }

                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem while trying to detemine the parent of the library catalog overview control");
                    _library_catalog_control = null;
                }

                return _library_catalog_control;
            }
        }

        private void OpenPDFDocument()
        {
            // Sometimes one of these is NULL and I have no idea why and can't replicate it...
            if (null == PDFDocumentBindable)
            {
                Logging.Warn("PDFDocumentBindable is null");
                return;
            }
            if (null == LibraryCatalogControl)
            {
                Logging.Warn("LibraryCatalogControl is null");
                return;
            }

            MainWindowServiceDispatcher.Instance.OpenDocument(PDFDocumentBindable.Underlying, search_terms: LibraryCatalogControl.FilterTerms);
        }

        private void LibraryCatalogOverviewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Make the button semi-transparent if our new document is not actually on the harddrive            
            PanelSearchScore.Visibility = Visibility.Collapsed;
            ListSearchDetails.DataContext = null;
            ObjLookInsidePanel.Visibility = Visibility.Collapsed;
            ButtonThemeSwatch.Visibility = Visibility.Collapsed;
            ObjFavouriteImage.Visibility = Visibility.Collapsed;
            ButtonOpen.Background = Brushes.Transparent;
            TextTitle.Foreground = Brushes.Black;
            TextTitle.FontSize = TextAuthors.FontSize;

            // No more work if our context is null
            if (null == PDFDocumentBindable)
            {
                return;
            }

            // The wizard
            if ("The Qiqqa Manual" == PDFDocumentBindable.Underlying.TitleCombined)
            {
                WizardDPs.SetPointOfInterest(TextTitle, "GuestLibraryQiqqaManualTitle");
                WizardDPs.SetPointOfInterest(ButtonOpen, "GuestLibraryQiqqaManualOpenButton");
            }
            else
            {
                WizardDPs.ClearPointOfInterest(TextTitle);
                WizardDPs.ClearPointOfInterest(ButtonOpen);
            }

            // Choose the icon depending on the reference type
            if (PDFDocumentBindable.Underlying.IsVanillaReference)
            {
                ButtonOpen.Icon = Icons.GetAppIcon(Icons.LibraryCatalogOpenVanillaReference);
                ButtonOpen.Opacity = 1.0;
            }
            else
            {
                ButtonOpen.Icon = Icons.GetAppIcon(Icons.LibraryCatalogOpen);
                ButtonOpen.Opacity = PDFDocumentBindable.Underlying.DocumentExists ? 1.0 : 0.5;
            }

            // Favourite?
            if (PDFDocumentBindable.Underlying.IsFavourite ?? false)
            {
                ObjFavouriteImage.Visibility = Visibility.Visible;
            }

            // Colour
            if (Colors.Transparent != PDFDocumentBindable.Underlying.Color)
            {
                ButtonOpen.Background = new SolidColorBrush(ColorTools.MakeTransparentColor(PDFDocumentBindable.Underlying.Color, 64));
            }

            // Rating
            if (!String.IsNullOrEmpty(PDFDocumentBindable.Underlying.Rating))
            {
                double rating;
                if (Double.TryParse(PDFDocumentBindable.Underlying.Rating, out rating))
                {
                    TextTitle.FontSize = TextAuthors.FontSize + rating;
                }
            }

            // Reading stage
            switch (PDFDocumentBindable.Underlying.ReadingStage)
            {
                case Choices.ReadingStages_TOP_PRIORITY:
                    TextTitle.Foreground = Brushes.DarkRed;
                    break;

                case Choices.ReadingStages_READ_AGAIN:
                case Choices.ReadingStages_INTERRUPTED:
                case Choices.ReadingStages_STARTED_READING:
                    TextTitle.Foreground = Brushes.DarkGreen;
                    break;

                case Choices.ReadingStages_SKIM_READ:
                case Choices.ReadingStages_BROWSED:
                case Choices.ReadingStages_INTEREST_ONLY:
                    TextTitle.Foreground = Brushes.DarkBlue;
                    break;

                case Choices.ReadingStages_FINISHED_READING:
                case Choices.ReadingStages_DUPLICATE:
                    TextTitle.Foreground = Brushes.DarkGray;
                    break;

                case Choices.ReadingStages_UNREAD:
                    TextTitle.Foreground = Brushes.Black;
                    break;

                default:
                    TextTitle.Foreground = Brushes.Black;
                    break;
            }

            // Populate the score if we have them
            if (null != LibraryCatalogControl)
            {
                Dictionary<string, double> search_scores = LibraryCatalogControl.SearchScores;
                if (null != search_scores)
                {
                    PanelSearchScore.Visibility = Visibility.Visible;

                    double score;
                    search_scores.TryGetValue(PDFDocumentBindable.Underlying.Fingerprint, out score);

                    string score_string = String.Format("{0:0}%", score * 100);
                    ButtonSearchInside.Caption = "" + score_string + "";
                    ButtonSearchInside.ToolTip = String.Format("Search score is {0}. Click here to see why...", score_string);
                    ButtonSearchInside.Cursor = Cursors.Hand;
                    ButtonSearchInside.MinWidth = 0;
                    Color color = Color.FromRgb(255, (byte)(255 - 150 * score), 100);
                    ButtonSearchInside.Background = new SolidColorBrush(color);
                }
            }

            // Populate the theme swatch
            ButtonThemeSwatch.Visibility = Visibility.Visible;
            ButtonThemeSwatch.Background = ThemeBrushes.GetBrushForDocument(PDFDocumentBindable.Underlying);

            // Populate the linked documents
            CitationsUserControl.PopulatePanelWithCitations(DocsPanel_Linked, PDFDocumentBindable.Underlying.Library, PDFDocumentBindable.Underlying, PDFDocumentBindable.Underlying.PDFDocumentCitationManager.GetLinkedDocuments(), Features.LinkedDocument_Library_OpenDoc, " » ", false);
        }

        private void ButtonSearchInside_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_SearchInsideOpen);

            PDFSearchResultSet search_result_set = PDFSearcher.Search(PDFDocumentBindable.Underlying, LibraryCatalogControl.FilterTerms);
            ListSearchDetails.DataContext = search_result_set.AsList();
            ObjLookInsidePanel.Visibility = Visibility.Visible;
        }

        private void HyperlinkPreview_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_PreviewPDF);

            try
            {
                if (null == library_index_hover_popup)
                {
                    library_index_hover_popup = new LibraryIndexHoverPopup();
                    library_index_hover_popup.SetPopupContent(PDFDocumentBindable.Underlying, 1);
                    ButtonOpen.ToolTip = library_index_hover_popup;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Exception while displaying document preview popup");
            }
        }

        private void HyperlinkPreview_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            library_index_hover_popup?.Dispose();
            library_index_hover_popup = null;

            ButtonOpen.ToolTip = "";
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenPDFDocument();
            e.Handled = true;
        }

        private void ListSearchDetails_SearchSelectionChanged(PDFSearchResult search_result)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_SearchInsideJumpToLocation);

            if (null != search_result)
            {
                MainWindowServiceDispatcher.Instance.OpenDocument(PDFDocumentBindable.Underlying, search_result.page, LibraryCatalogControl.FilterTerms, false);
            }
        }

        private object GetDocumentDragData()
        {
            LibraryCatalogControl lcc = GUITools.GetParentControl<LibraryCatalogControl>(this);

            if (null != lcc)
            {
                List<PDFDocument> selected_pdf_documents = lcc.SelectedPDFDocuments;
                if (!selected_pdf_documents.Contains(PDFDocumentBindable.Underlying))
                {
                    selected_pdf_documents.Add(PDFDocumentBindable.Underlying);
                }

                return selected_pdf_documents;
            }
            else
            {
                return new List<PDFDocument>();
            }
        }

        private void LibraryCatalogOverviewControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            LibraryCatalogControl lcc = GUITools.GetParentControl<LibraryCatalogControl>(this);

            if (null != lcc)
            {
                List<PDFDocument> selected_pdf_documents = lcc.SelectedPDFDocuments;

                LibraryCatalogPopup popup = new LibraryCatalogPopup(selected_pdf_documents);
                popup.Open();
            }

            e.Handled = true;
        }

        public AugmentedBindable<PDFDocument> PDFDocumentBindable => DataContext as AugmentedBindable<PDFDocument>;

        #region --- IDisposable ------------------------------------------------------------------------

        ~LibraryCatalogOverviewControl()
        {
            Logging.Debug("~LibraryCatalogOverviewControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing LibraryCatalogOverviewControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("LibraryCatalogOverviewControl::Dispose({0}) @{1}", disposing, dispose_count);

            try
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources / get rid of cyclic references:
                    library_index_hover_popup?.Dispose();

                    WPFDoEvents.InvokeInUIThread(() =>
                    {
                        try
                        {
                            WizardDPs.ClearPointOfInterest(PanelSearchScore);
                            WizardDPs.ClearPointOfInterest(ObjLookInsidePanel);
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }

                        TextTitle.MouseLeftButtonUp -= TextTitle_MouseLeftButtonUp;

                        ButtonOpen.ToolTipOpening -= HyperlinkPreview_ToolTipOpening;
                        ButtonOpen.ToolTipClosing -= HyperlinkPreview_ToolTipClosing;

                        ListSearchDetails.SearchClicked -= ListSearchDetails_SearchSelectionChanged;

                        DataContextChanged -= LibraryCatalogOverviewControl_DataContextChanged;

                        try
                        {
                            DataContext = null;
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                    });
                }

                // Clear the references for sanity's sake
                library_index_hover_popup = null;
                drag_drop_helper = null;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

            ++dispose_count;
        }

        #endregion
    }
}
