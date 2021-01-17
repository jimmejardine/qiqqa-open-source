using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.AnnotationsReportBuilding;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.WebcastStuff;
using Qiqqa.DocumentLibrary.BundleLibrary.LibraryBundleCreation;
using Qiqqa.DocumentLibrary.FolderWatching;
using Qiqqa.DocumentLibrary.Import.Manual;
using Qiqqa.DocumentLibrary.LibraryFilter;
using Qiqqa.DocumentLibrary.MassDuplicateCheckingStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.Documents.PDF.PDFControls.MetadataControls;
using Qiqqa.Exporting;
using Qiqqa.Localisation;
using Qiqqa.Synchronisation.BusinessLogic;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.GUI.DualTabbedLayoutStuff;
using Utilities.GUI.Wizard;
using Utilities.Misc;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace Qiqqa.DocumentLibrary
{
    /// <summary>
    /// Interaction logic for LibraryControl.xaml
    /// </summary>
    public partial class LibraryControl : UserControl, IDualTabbedLayoutDragDrop
    {
        private WebLibraryDetail web_library_detail;
        private DragToLibraryManager dual_tab_drag_to_library_manager;

        private List<PDFDocument> pdf_documents = null;

        public LibraryControl(WebLibraryDetail web_library_detail)
        {
            Theme.Initialize();

            Logging.Debug("+LibraryControl()");

            this.web_library_detail = web_library_detail;
            dual_tab_drag_to_library_manager = new DragToLibraryManager(web_library_detail);

            InitializeComponent();

            ImageLibraryEmptyAddFiles.Source = Icons.GetAppIcon(Icons.LibraryEmptyAddFiles);
            RenderOptions.SetBitmapScalingMode(ImageLibraryEmptyAddFiles, BitmapScalingMode.HighQuality);
            ImageLibraryMissingBibTeX.Source = Icons.GetAppIcon(Icons.BibTexSniffer);
            RenderOptions.SetBitmapScalingMode(ImageLibraryMissingBibTeX, BitmapScalingMode.HighQuality);

            bool ADVANCED_MENUS = ConfigurationManager.Instance.ConfigurationRecord.GUI_AdvancedMenus;

            // Connect the dropdowns
            ButtonAddPDF.AttachPopup(ButtonAddPDFPopup);
            ButtonAddPDF.Icon = Icons.GetAppIcon(Icons.DocumentsAddToLibrary);
            if (!ADVANCED_MENUS) ButtonAddPDF.Caption = LocalisationManager.Get("LIBRARY/CAP/POPUP_ADD_DOCUMENTS");
            ButtonAddPDF.ToolTip = LocalisationManager.Get("LIBRARY/TIP/POPUP_ADD_DOCUMENTS");

            ButtonSync.AttachPopup(ButtonSyncPopup);
            ButtonSync.Icon = Icons.GetAppIcon(Icons.SyncWithCloud);
            if (!ADVANCED_MENUS) ButtonSync.Caption = LocalisationManager.Get("LIBRARY/CAP/POPUP_SYNC");
            ButtonSync.ToolTip = LocalisationManager.Get("LIBRARY/TIP/POPUP_SYNC");

            // Then the menus
            ButtonAddVanillaReference.Icon = Icons.GetAppIcon(Icons.New);
            ButtonAddVanillaReference.Caption = LocalisationManager.Get("LIBRARY/CAP/ADD_REFERENCE");
            ButtonAddVanillaReference.ToolTip = LocalisationManager.Get("LIBRARY/TIP/ADD_REFERENCE");
            ButtonAddVanillaReference.Click += ButtonAddVanillaReference_Click;

            ButtonSyncMetadaWithCloud.Icon = Icons.GetAppIcon(Icons.SyncWithCloud);
            ButtonSyncMetadaWithCloud.Caption = LocalisationManager.Get("LIBRARY/CAP/PARTIAL_SYNC");
            ButtonSyncMetadaWithCloud.ToolTip = LocalisationManager.Get("LIBRARY/TIP/PARTIAL_SYNC");
            ButtonSyncMetadaWithCloud.Click += ButtonSyncMetadaWithCloud_Click;

            ButtonSyncAllPDFsWithCloud.Icon = Icons.GetAppIcon(Icons.SyncPDFsWithCloud);
            ButtonSyncAllPDFsWithCloud.Caption = LocalisationManager.Get("LIBRARY/CAP/FULL_SYNC");
            ButtonSyncAllPDFsWithCloud.ToolTip = LocalisationManager.Get("LIBRARY/TIP/FULL_SYNC");
            ButtonSyncAllPDFsWithCloud.Click += ButtonSyncPDFsWithCloud_Click;

            ButtonSyncDetails.Icon = Icons.GetAppIcon(Icons.SyncDetails);
            ButtonSyncDetails.Caption = LocalisationManager.Get("LIBRARY/CAP/SYNC_DETAILS");
            ButtonSyncDetails.ToolTip = LocalisationManager.Get("LIBRARY/TIP/SYNC_DETAILS");
            ButtonSyncDetails.Click += ButtonSyncDetails_Click;

            ButtonEdit.Icon = Icons.GetAppIcon(Icons.Open);
            ButtonEdit.Caption = LocalisationManager.Get("LIBRARY/CAP/EDIT");
            ButtonEdit.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EDIT");
            ButtonEdit.Click += ButtonEdit_Click;

            ButtonDelete.Icon = Icons.GetAppIcon(Icons.Open);
            ButtonDelete.Caption = LocalisationManager.Get("LIBRARY/CAP/DELETE");
            ButtonDelete.ToolTip = LocalisationManager.Get("LIBRARY/TIP/DELETE");
            ButtonDelete.Click += ButtonDelete_Click;

            ButtonAnnotationsReport.Icon = Icons.GetAppIcon(Icons.LibraryAnnotationsReport);
            if (!ADVANCED_MENUS) ButtonAnnotationsReport.Caption = LocalisationManager.Get("LIBRARY/CAP/ANNOTATION_REPORT");
            ButtonAnnotationsReport.ToolTip = LocalisationManager.Get("LIBRARY/TIP/ANNOTATION_REPORT");
            ButtonAnnotationsReport.Click += ButtonAnnotationsReport_Click;
            WizardDPs.SetPointOfInterest(ButtonAnnotationsReport, "LibraryAnnotationReportButton");

            ButtonGenerateReferences.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonGenerateReferences.Icon = Icons.GetAppIcon(Icons.LibraryGenerateReferences);
            if (!ADVANCED_MENUS) ButtonGenerateReferences.Caption = LocalisationManager.Get("LIBRARY/CAP/FIND_REFERENCES");
            ButtonGenerateReferences.ToolTip = LocalisationManager.Get("LIBRARY/TIP/FIND_REFERENCES");
            ButtonGenerateReferences.Click += ButtonGenerateReferences_Click;

            ButtonFindDuplicates.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonFindDuplicates.Icon = Icons.GetAppIcon(Icons.LibraryFindDuplicates);
            if (!ADVANCED_MENUS) ButtonFindDuplicates.Caption = LocalisationManager.Get("LIBRARY/CAP/FIND_DUPLICATES");
            ButtonFindDuplicates.ToolTip = LocalisationManager.Get("LIBRARY/TIP/FIND_DUPLICATES");
            ButtonFindDuplicates.Click += ButtonFindDuplicates_Click;

            ButtonBibTexSniffer.Icon = Icons.GetAppIcon(Icons.BibTexSniffer);
            if (!ADVANCED_MENUS) ButtonBibTexSniffer.Caption = LocalisationManager.Get("LIBRARY/CAP/BIBTEX_SNIFFER");
            ButtonBibTexSniffer.ToolTip = LocalisationManager.Get("LIBRARY/TIP/BIBTEX_SNIFFER");
            ButtonBibTexSniffer.Click += ButtonBibTexSniffer_Click;

            ButtonExplore.AttachPopup(ButtonExplorePopup);
            ButtonExplore.Icon = Icons.GetAppIcon(Icons.Explore);
            if (!ADVANCED_MENUS) ButtonExplore.Caption = LocalisationManager.Get("LIBRARY/CAP/EXPLORE");
            ButtonExplore.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EXPLORE");
            ButtonExplore.Visibility = ConfigurationManager.Instance.NoviceVisibility;

            ButtonExpedition.Icon = Icons.GetAppIcon(Icons.ModuleExpedition);
            ButtonExpedition.Caption = LocalisationManager.Get("LIBRARY/TIP/EXPEDITION");
            ButtonExpedition.Click += ButtonExpedition_Click;

            ButtonExploreInBrainstorm.Icon = Icons.GetAppIcon(Icons.ModuleBrainstorm);
            ButtonExploreInBrainstorm.Caption = LocalisationManager.Get("LIBRARY/TIP/BRAINSTORM");
            ButtonExploreInBrainstorm.Click += ButtonExploreInBrainstorm_Click;

            ButtonExploreInPivot.Icon = Icons.GetAppIcon(Icons.LibraryPivot);
            ButtonExploreInPivot.Caption = LocalisationManager.Get("LIBRARY/TIP/PIVOT");
            ButtonExploreInPivot.Click += ButtonExploreInPivot_Click;

            ButtonExport.AttachPopup(ButtonExportPopup);
            ButtonExport.Icon = Icons.GetAppIcon(Icons.LibraryExport);
            if (!ADVANCED_MENUS) ButtonExport.Caption = LocalisationManager.Get("LIBRARY/CAP/POPUP_EXPORT");
            ButtonExport.ToolTip = LocalisationManager.Get("LIBRARY/TIP/POPUP_EXPORT");
            ButtonExport.Visibility = ConfigurationManager.Instance.NoviceVisibility;

            ButtonExportLibrary.Icon = Icons.GetAppIcon(Icons.LibraryExport);
            ButtonExportLibrary.Caption = LocalisationManager.Get("LIBRARY/CAP/EXPORT_LIBRARY");
            ButtonExportLibrary.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EXPORT_LIBRARY");
            ButtonExportLibrary.Click += ButtonExportLibrary_Click;

            ButtonExportBibTex.Icon = Icons.GetAppIcon(Icons.ExportBibTex);
            ButtonExportBibTex.Caption = LocalisationManager.Get("LIBRARY/CAP/EXPORT_BIBTEX");
            ButtonExportBibTex.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EXPORT_BIBTEX");
            ButtonExportBibTex.Click += ButtonExportBibTex_Click;

            ButtonExportWord2007.Icon = Icons.GetAppIcon(Icons.ExportWord2007);
            ButtonExportWord2007.Caption = LocalisationManager.Get("LIBRARY/CAP/EXPORT_WORD");
            ButtonExportWord2007.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EXPORT_WORD");
            ButtonExportWord2007.Click += ButtonExportWord2007_Click;

            ButtonExportCitationMatrix.Icon = Icons.GetAppIcon(Icons.ExportCitationMatrix);
            ButtonExportCitationMatrix.Caption = LocalisationManager.Get("LIBRARY/CAP/EXPORT_CITATION_MATRIX");
            ButtonExportCitationMatrix.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EXPORT_CITATION_MATRIX");
            ButtonExportCitationMatrix.Click += ButtonExportCitationMatrix_Click;

            ButtonExportLinkedDocs.Icon = Icons.GetAppIcon(Icons.ExportCitationMatrix);
            ButtonExportLinkedDocs.Caption = LocalisationManager.Get("LIBRARY/CAP/EXPORT_LINKED_DOCS");
            ButtonExportLinkedDocs.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EXPORT_LINKED_DOCS");
            ButtonExportLinkedDocs.Click += ButtonExportLinkedDocs_Click;

            ButtonBuildBundleLibrary.Icon = Icons.GetAppIcon(Icons.BuildBundleLibrary);
            ButtonBuildBundleLibrary.Caption = LocalisationManager.Get("LIBRARY/CAP/BUILD_BUNDLE_LIBRARY");
            ButtonBuildBundleLibrary.ToolTip = LocalisationManager.Get("LIBRARY/TIP/BUILD_BUNDLE_LIBRARY");
            ButtonBuildBundleLibrary.Click += ButtonBuildBundleLibrary_Click;

            ButtonExportAnnotationsCode.Icon = Icons.GetAppIcon(Icons.ExportBibTex);
            ButtonExportAnnotationsCode.Caption = LocalisationManager.Get("LIBRARY/CAP/EXPORT_ANNOTATIONS_CODE");
            ButtonExportAnnotationsCode.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EXPORT_ANNOTATIONS_CODE");
            ButtonExportAnnotationsCode.Click += ButtonExportAnnotationsCode_Click;

            ButtonAddDocuments.Icon = Icons.GetAppIcon(Icons.DocumentsAddToLibrary);
            ButtonAddDocuments.Caption = LocalisationManager.Get("LIBRARY/CAP/ADD_DOCUMENTS");
            ButtonAddDocuments.ToolTip = LocalisationManager.Get("LIBRARY/TIP/ADD_DOCUMENTS");
            ButtonAddDocuments.Click += ButtonAddDocuments_Click;

            ButtonAddDocumentsFromFolder.Icon = Icons.GetAppIcon(Icons.DocumentsAddToLibraryFromFolder);
            ButtonAddDocumentsFromFolder.Caption = LocalisationManager.Get("LIBRARY/CAP/ADD_FOLDER");
            ButtonAddDocumentsFromFolder.ToolTip = LocalisationManager.Get("LIBRARY/TIP/ADD_FOLDER");
            ButtonAddDocumentsFromFolder.Click += ButtonAddDocumentsFromFolder_Click;

            ButtonAddDocumentsFromLibrary.Icon = Icons.GetAppIcon(Icons.DocumentsAddToLibraryFromLibrary);
            ButtonAddDocumentsFromLibrary.Caption = LocalisationManager.Get("LIBRARY/CAP/ADD_LIBRARY");
            ButtonAddDocumentsFromLibrary.ToolTip = LocalisationManager.Get("LIBRARY/TIP/ADD_LIBRARY");
            ButtonAddDocumentsFromLibrary.Click += ButtonAddDocumentsFromLibrary_Click;

            ButtonWatchFolder.Icon = Icons.GetAppIcon(Icons.DocumentsWatchFolder);
            ButtonWatchFolder.Caption = LocalisationManager.Get("LIBRARY/CAP/WATCH_FOLDER");
            ButtonWatchFolder.ToolTip = LocalisationManager.Get("LIBRARY/TIP/WATCH_FOLDER");
            ButtonWatchFolder.Click += ButtonWatchFolder_Click;

            ButtonImportFromThirdParty.Icon = Icons.GetAppIcon(Icons.DocumentsImportFromThirdParty);
            ButtonImportFromThirdParty.Caption = LocalisationManager.Get("LIBRARY/CAP/ADD_IMPORT");
            ButtonImportFromThirdParty.ToolTip = LocalisationManager.Get("LIBRARY/TIP/ADD_IMPORT");
            ButtonImportFromThirdParty.Click += ButtonImportFromThirdParty_Click;

            ButtonAddMissingDocumentsFromSelf.Icon = Icons.GetAppIcon(Icons.DocumentsAddMissingFromSelf);
            ButtonAddMissingDocumentsFromSelf.Caption = "Recover unregistered PDFs in this library";
            ButtonAddMissingDocumentsFromSelf.ToolTip = "This is a live library recovery/restoration operation: inspect the current library's storage and re-register all PDFs in there, which have not been registered in the library already.";
            ButtonAddMissingDocumentsFromSelf.Click += ButtonAddMissingDocumentsFromSelf_Click;

            ObjLibraryEmptyDescriptionText.Background = ThemeColours.Background_Brush_Blue_LightToDark;

            // Tie all our GUI objects together
            ObjLibraryFilterControl.library_filter_control_search = ObjLibraryFilterControl_Search;
            ObjLibraryFilterControl_Search.library_filter_control = ObjLibraryFilterControl;

            ObjLibraryFilterControl.OnFilterChanged += ObjLibraryFilterControl_OnFilterChanged;
            ObjLibraryFilterControl.OnFilterChanged += ObjLibraryFilterOverviewControl.OnFilterChanged;
            ObjLibraryFilterControl.OnFilterChanged += ObjLibraryCatalogControl.OnFilterChanged;

            ObjLibraryFilterControl.LibraryRef = web_library_detail;
            ObjLibraryFilterControl.ResetFilters(true);

            ObjLibraryCatalogControl.LibraryRef = web_library_detail;

            // Catch some keyboard commands
            KeyDown += LibraryControl_KeyDown;

            if (!ADVANCED_MENUS) ButtonWebcast.Caption = "Tutorial\n";
            Webcasts.FormatWebcastButton(ButtonWebcast, Webcasts.LIBRARY);

            // IF the library is read-only?
            ReflectReadOnlyStatus();

            Logging.Debug("-LibraryControl()");
        }

        private void ReflectReadOnlyStatus()
        {
            ObjReadOnlyLibraryDescriptionBorder.Visibility = web_library_detail.IsReadOnlyLibrary ? Visibility.Visible : Visibility.Collapsed;
            ObjReadonlyExplain.Text = "";

            if (web_library_detail.IsBundleLibrary)
            {
                ObjReadonlyExplain.Text = "This Bundle Library is automatically overwritten whenever the Bundle creator updates it online.";
            }
        }

        private void ButtonBuildBundleLibrary_Click(object sender, RoutedEventArgs e)
        {
            LibraryBundleCreationControl lbcc = new LibraryBundleCreationControl();
            lbcc.ReflectLibrary(web_library_detail);
            MainWindowServiceDispatcher.Instance.OpenControl("LibraryBundleCreationControl", LibraryBundleCreationControl.TITLE, lbcc, Icons.GetAppIcon(Icons.BuildBundleLibrary));
        }

        private void ButtonExportLinkedDocs_Click(object sender, RoutedEventArgs e)
        {
            LinkedDocsAnnotationReportBuilder.BuildReport(web_library_detail, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        private void ButtonExportAnnotationsCode_Click(object sender, RoutedEventArgs e)
        {
            JSONAnnotationReportBuilder.BuildReport(web_library_detail, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        private void ButtonExportCitationMatrix_Click(object sender, RoutedEventArgs e)
        {
            CitationMatrixExport.Export(web_library_detail, pdf_documents);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxes.Error("Sorry!\n\nMethod has not been implemented yet!");
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxes.Error("Sorry!\n\nMethod has not been implemented yet!");
        }

        private void LibraryControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.F == e.Key && KeyboardTools.IsCTRLDown())
            {
                ObjLibraryFilterControl_Search.SearchQuick.FocusSearchArea();
                e.Handled = true;
            }
            else if (Key.F5 == e.Key && KeyboardTools.IsNoneOfTheMetaKeysDown())
            {
                ObjLibraryCatalogControl.ListPDFDocuments.UpdateLayout();

                e.Handled = true;
            }
        }

        public WebLibraryDetail LibraryRef => web_library_detail;

        private void ButtonAddVanillaReference_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonAddPDFPopup))
            {
                PDFDocument pdf_document = web_library_detail.Xlibrary.AddVanillaReferenceDocumentToLibrary(null, web_library_detail, null, null, false);

                // Let's pop up the BibTeX editor window for the new document
                MetadataBibTeXEditorControl editor = new MetadataBibTeXEditorControl();
                editor.Show(pdf_document.Bindable);
            }
        }

        private void ButtonImportFromThirdParty_Click(object sender, RoutedEventArgs e)
        {
            new ImportFromThirdParty(web_library_detail).ShowDialog();
        }

        private void ObjLibraryFilterControl_OnFilterChanged(LibraryFilterControl library_filter_control, List<PDFDocument> pdf_documents, Span descriptive_span, string filter_terms, Dictionary<string, double> search_scores, PDFDocument pdf_document_to_focus_on)
        {
            this.pdf_documents = pdf_documents;

            // Check if this library is empty
            int EMPTY_LIBRARY_THRESHOLD = 5;
            if (web_library_detail.Xlibrary.PDFDocuments.Count > EMPTY_LIBRARY_THRESHOLD)
            {
                ObjLibraryEmptyDescriptionBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                ObjLibraryEmptyDescriptionBorder.Visibility = Visibility.Visible;
            }

            // Check if we should hint about BibTex
            {
                int NUM_TO_CHECK = 10;
                int total_without_bibtex = 0;
                List<PDFDocument> pdf_document_to_check_for_bibtex = web_library_detail.Xlibrary.PDFDocuments;
                for (int i = 0; i < pdf_document_to_check_for_bibtex.Count && i < NUM_TO_CHECK; ++i)
                {
                    if (!pdf_document_to_check_for_bibtex[i].Deleted && String.IsNullOrEmpty(pdf_document_to_check_for_bibtex[i].BibTex))
                    {
                        ++total_without_bibtex;
                    }
                }

                // Set the visibility - must have a few documents, and more than
                ObjNotMuchBibTeXDescriptionBorder.Visibility = Visibility.Collapsed;
                if (web_library_detail.Xlibrary.PDFDocuments.Count > EMPTY_LIBRARY_THRESHOLD && pdf_document_to_check_for_bibtex.Count > 0)
                {
                    if (total_without_bibtex / (double)pdf_document_to_check_for_bibtex.Count >= 0.5 || total_without_bibtex / (double)NUM_TO_CHECK >= 0.5)
                    {
                        ObjNotMuchBibTeXDescriptionBorder.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void ButtonBibTexSniffer_Click(object sender, RoutedEventArgs e)
        {
            GoogleBibTexSnifferControl sniffer = new GoogleBibTexSnifferControl();
            sniffer.Show(ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything, null, null);
        }

        private void ButtonExploreInBrainstorm_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_ExploreInBrainstorm);
            MainWindowServiceDispatcher.Instance.ExploreLibraryInBrainstorm(web_library_detail);
        }

        private void ButtonExploreInPivot_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_ExploreInPivot);
            MainWindowServiceDispatcher.Instance.OpenPivot(web_library_detail, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        private void ButtonExpedition_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Expedition_Open_Library);
            MainWindowServiceDispatcher.Instance.OpenExpedition(web_library_detail, null);
        }

        private void ButtonExportLibrary_Click(object sender, RoutedEventArgs e)
        {
            LibraryExporter.Export(web_library_detail, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        private void ButtonExportBibTex_Click(object sender, RoutedEventArgs e)
        {
            BibTexExport.Export(ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        private void ButtonExportWord2007_Click(object sender, RoutedEventArgs e)
        {
            Word2007Export.Export(ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        private void ButtonAnnotationsReport_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.GenerateAnnotationReport(web_library_detail, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        private void ButtonGenerateReferences_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_GenerateReferences);
            SafeThreadPool.QueueUserWorkItem(o => CitationFinder.FindCitations(web_library_detail));
        }

        private void ButtonFindDuplicates_Click(object sender, RoutedEventArgs e)
        {
            //VisualGalleryControl.Test(library);

            FeatureTrackingManager.Instance.UseFeature(Features.Library_FindDuplicates);
            MassDuplicateCheckingControl.FindDuplicatesForLibrary(web_library_detail);
        }

        private void ButtonSyncMetadaWithCloud_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonSyncPopup))
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Sync_SyncMetadata);
                LibrarySyncManager.Instance.RequestSync(new LibrarySyncManager.SyncRequest(wants_user_intervention: false, web_library_detail, suppress_already_in_progress_notification: false));
            }
        }

        private void ButtonSyncPDFsWithCloud_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonSyncPopup))
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Sync_SyncMetadataAndPDFs);
                LibrarySyncManager.Instance.RequestSync(new LibrarySyncManager.SyncRequest(wants_user_intervention: false, web_library_detail, suppress_already_in_progress_notification: false));
            }
        }

        private void ButtonSyncDetails_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonSyncPopup))
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Sync_SyncDetails);
                LibrarySyncManager.Instance.RequestSync(new LibrarySyncManager.SyncRequest(wants_user_intervention: true, web_library_detail, suppress_already_in_progress_notification: false));
            }
        }

        private void ButtonWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonAddPDFPopup))
            {
                new FolderWatcherChooser(web_library_detail).ShowDialog();
            }
        }

        private void ButtonAddDocuments_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonAddPDFPopup))
            {
#if DEBUG
                if (Runtime.IsRunningInVisualStudioDesigner) return;
#endif

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.CheckFileExists = true;
                dlg.CheckPathExists = true;
                dlg.Filter = "PDF Files|*.pdf";
                dlg.Multiselect = true;
                dlg.Title = "Select the PDF documents you wish to add to your document library";
                if (dlg.ShowDialog() == true)
                {
                    ImportingIntoLibrary.AddNewPDFDocumentsToLibrary_ASYNCHRONOUS(web_library_detail, false, dlg.FileNames);
                }
            }
        }

        private void ButtonAddDocumentsFromLibrary_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonAddPDFPopup))
            {
                // First choose the source library
                string message = String.Format("You are about to import a lot of PDFs into the library named '{0}'.  Please choose the library FROM WHICH you wish to import the PDFs.", web_library_detail.Title);
                WebLibraryDetail picked_web_library_detail = WebLibraryPicker.PickWebLibrary(message);
                if (null != picked_web_library_detail)
                {
                    if (picked_web_library_detail == web_library_detail)
                    {
                        MessageBoxes.Error("You can not copy documents into the same library.");
                        return;
                    }

                    // Then check that they still want to do this
                    string message2 = String.Format("You are about to copy ALL of the PDFs from the library named '{0}' into the library named '{1}'.  Are you sure you want to do this?", picked_web_library_detail.Title, web_library_detail.Title);
                    if (!MessageBoxes.AskQuestion(message2))
                    {
                        return;
                    }

                    // They are sure!
                    ImportingIntoLibrary.ClonePDFDocumentsFromOtherLibrary_ASYNCHRONOUS(picked_web_library_detail.Xlibrary.PDFDocuments, web_library_detail);
                }
            }
        }

        private void ButtonAddDocumentsFromFolder_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonAddPDFPopup))
            {
                new ImportFromFolder(web_library_detail).ShowDialog();
            }
        }

        private void ButtonAddMissingDocumentsFromSelf_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonAddPDFPopup))
            {
                var root_folder = web_library_detail.LIBRARY_DOCUMENTS_BASE_PATH;
                if (Directory.Exists(root_folder))
                {
                    // do the import
                    ImportingIntoLibrary.AddNewPDFDocumentsToLibraryFromFolder_ASYNCHRONOUS(web_library_detail, root_folder, true, false, false);
                }
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            Library library = Library.GuestInstance;
            LibraryControl lic = new LibraryControl(library);
            ControlHostingWindow window = new ControlHostingWindow("Library control", lic);
            window.Show();
        }
#endif

        #endregion

        internal void SearchLibrary(string query)
        {
            ObjLibraryFilterControl.SearchLibrary(query);
        }

        public LibraryFilterControl_Search DetachSearchBox()
        {
            LibraryFilterControl_Search child = ObjLibraryFilterControl_Search;
            HolderForObjLibraryFilterControl_Search.Children.Clear();
            return child;
        }

        #region --- Drag and drop onto library tab header --------------------------------------------------------------------------------

        public void DualTabbedLayoutDragEnter(object sender, DragEventArgs e)
        {
            dual_tab_drag_to_library_manager.OnDragEnter(sender, e);
        }

        public void DualTabbedLayoutDragOver(object sender, DragEventArgs e)
        {
            dual_tab_drag_to_library_manager.OnDragOver(sender, e);
        }

        public void DualTabbedLayoutDrop(object sender, DragEventArgs e)
        {
            dual_tab_drag_to_library_manager.OnDrop(sender, e);
        }

        #endregion
    }
}
