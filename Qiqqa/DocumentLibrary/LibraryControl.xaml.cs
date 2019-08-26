using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using icons;
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
using Qiqqa.AnnotationsReportBuilding;
using Qiqqa.DocumentLibrary.VisualGalleryStuff;

namespace Qiqqa.DocumentLibrary
{
    /// <summary>
    /// Interaction logic for LibraryControl.xaml
    /// </summary>
    public partial class LibraryControl : UserControl, IDualTabbedLayoutDragDrop
    {
        private Library library;
        private DragToLibraryManager dual_tab_drag_to_library_manager;

        private List<PDFDocument> pdf_documents = null;

        public LibraryControl(Library library)
        {
            Theme.Initialize();

            Logging.Info("+LibraryControl()");

            this.library = library;
            this.dual_tab_drag_to_library_manager = new DragToLibraryManager(library);

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

            ButtonViewOnline.Icon = Icons.GetAppIcon(Icons.ViewOnline);
            ButtonViewOnline.Caption = LocalisationManager.Get("LIBRARY/CAP/VIEW_ONLINE");
            ButtonViewOnline.ToolTip = LocalisationManager.Get("LIBRARY/TIP/VIEW_ONLINE");
            ButtonViewOnline.Click += ButtonViewOnline_Click;

            ButtonTopUp.Icon = Icons.GetAppIcon(Icons.TopUp);
            ButtonTopUp.Caption = LocalisationManager.Get("LIBRARY/CAP/TOP_UP");
            ButtonTopUp.ToolTip = LocalisationManager.Get("LIBRARY/TIP/TOP_UP");
            ButtonTopUp.Click += ButtonTopUp_Click;

            ButtonInvite.Icon = Icons.GetAppIcon(Icons.Share);
            ButtonInvite.Caption = LocalisationManager.Get("LIBRARY/CAP/INVITE");
            ButtonInvite.ToolTip = LocalisationManager.Get("LIBRARY/TIP/INVITE");
            ButtonInvite.Click += ButtonInvite_Click;

            ButtonEditDelete.Icon = Icons.GetAppIcon(Icons.Open);
            ButtonEditDelete.Caption = LocalisationManager.Get("LIBRARY/CAP/EDIT_DELETE");
            ButtonEditDelete.ToolTip = LocalisationManager.Get("LIBRARY/TIP/EDIT_DELETE");
            ButtonEditDelete.Click += ButtonEditDelete_Click;

            ButtonPublicStatus.Icon = Icons.GetAppIcon(Icons.Share);
            ButtonPublicStatus.Caption = LocalisationManager.Get("LIBRARY/CAP/PUBLIC_STATUS");
            ButtonPublicStatus.ToolTip = LocalisationManager.Get("LIBRARY/TIP/PUBLIC_STATUS");
            ButtonPublicStatus.Click += ButtonPublicStatus_Click;

            // Guest library doesnt have some settings
            if (false && !library.WebLibraryDetail.IsWebLibrary)
            {
                ButtonViewOnline.Visibility = Visibility.Collapsed;
                ButtonTopUp.Visibility = Visibility.Collapsed;
                ButtonInvite.Visibility = Visibility.Collapsed;
                ButtonEditDelete.Visibility = Visibility.Collapsed;
                ButtonPublicStatus.Visibility = Visibility.Collapsed;
            }

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

            ObjLibraryEmptyDescriptionText.Background = ThemeColours.Background_Brush_Blue_LightToDark;

            // Tie all our GUI objects together
            ObjLibraryFilterControl.library_filter_control_search = ObjLibraryFilterControl_Search;
            ObjLibraryFilterControl_Search.library_filter_control = ObjLibraryFilterControl;

            ObjLibraryFilterControl.OnFilterChanged += ObjLibraryFilterControl_OnFilterChanged;
            ObjLibraryFilterControl.OnFilterChanged += ObjLibraryFilterOverviewControl.OnFilterChanged;
            ObjLibraryFilterControl.OnFilterChanged += ObjLibraryCatalogControl.OnFilterChanged;

            ObjLibraryFilterControl.Library = library;
            ObjLibraryFilterControl.ResetFilters(true);

            ObjLibraryCatalogControl.Library = library;

            // Catch some keyboard commands
            this.KeyDown += LibraryControl_KeyDown;

            if (!ADVANCED_MENUS) ButtonWebcast.Caption = "Tutorial\n";
            Webcasts.FormatWebcastButton(ButtonWebcast, Webcasts.LIBRARY);

            // IF the library readonly?
            ReflectReadOnlyStatus();

            Logging.Info("-LibraryControl()");
        }


        private void ReflectReadOnlyStatus()
        {
            ObjReadOnlyLibraryDescriptionBorder.Visibility = library.WebLibraryDetail.IsReadOnly ? Visibility.Visible : Visibility.Collapsed;
            ObjReadonlyExplain.Text = "";

            if (library.WebLibraryDetail.IsWebLibrary)
                ObjReadonlyExplain.Text = "If you wish to have write access, ask your library administrator to give you permission.";
            else if (library.WebLibraryDetail.IsIntranetLibrary)
                ObjReadonlyExplain.Text = "You need to upgrade to Premium+ to be able to write to this Intranet Library.";
            else if (library.WebLibraryDetail.IsBundleLibrary)
                ObjReadonlyExplain.Text = "This Bundle Library is automatically overwritten whenever the Bundle creator updates it online.";
        }

        void ButtonBuildBundleLibrary_Click(object sender, RoutedEventArgs e)
        {
            LibraryBundleCreationControl lbcc = new LibraryBundleCreationControl();
            lbcc.ReflectLibrary(this.library);
            MainWindowServiceDispatcher.Instance.OpenControl("LibraryBundleCreationControl" + this.library.WebLibraryDetail.ShortWebId, LibraryBundleCreationControl.TITLE, lbcc, Icons.GetAppIcon(Icons.BuildBundleLibrary));
        }

        void ButtonExportLinkedDocs_Click(object sender, RoutedEventArgs e)
        {
            LinkedDocsAnnotationReportBuilder.BuildReport(this.library, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }        

        void ButtonExportAnnotationsCode_Click(object sender, RoutedEventArgs e)
        {
            JSONAnnotationReportBuilder.BuildReport(this.library, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        void ButtonExportCitationMatrix_Click(object sender, RoutedEventArgs e)
        {
            CitationMatrixExport.Export(this.library, pdf_documents);
        }

        void ButtonInvite_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.InviteFriendsToWebLibrary(this.library.WebLibraryDetail.ShortWebId);            
        }

        void ButtonEditDelete_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.EditOrDeleteLibrary(this.library.WebLibraryDetail.ShortWebId);
        }

        void ButtonPublicStatus_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.ChangeLibraryPublicStatus(this.library.WebLibraryDetail.ShortWebId);
        }

        void ButtonTopUp_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.TopUpWebLibrary(this.library.WebLibraryDetail.ShortWebId);
        }

        void ButtonViewOnline_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.OpenWebLibrary(this.library.WebLibraryDetail.ShortWebId);
        }

        void LibraryControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.F == e.Key && KeyboardTools.IsCTRLDown())
            {
                ObjLibraryFilterControl_Search.SearchQuick.FocusSearchArea();
                e.Handled = true;
            }
        }

        public Library Library
        {
            get
            {
                return library;
            }
        }

        void ButtonAddVanillaReference_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonAddPDFPopup))
            {
                PDFDocument pdf_document = library.AddVanillaReferenceDocumentToLibrary(null, null, null, false, false);

                // Let's pop up the bibtex editor window for the new document
                MetadataBibTeXEditorControl editor = new MetadataBibTeXEditorControl();
                editor.Show(pdf_document.Bindable);
            }
        }

        void ButtonImportFromThirdParty_Click(object sender, RoutedEventArgs e)
        {
            new ImportFromThirdParty(library).ShowDialog();
        }

        void ObjLibraryFilterControl_OnFilterChanged(LibraryFilterControl library_filter_control, List<PDFDocument> pdf_documents, Span descriptive_span, string filter_terms, Dictionary<string, double> search_scores, PDFDocument pdf_document_to_focus_on)
        {
            this.pdf_documents = pdf_documents;

            // Check if this library is empty
            int EMPTY_LIBRARY_THRESHOLD = 5;
            if (this.library.PDFDocuments.Count > EMPTY_LIBRARY_THRESHOLD)
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
                int total_without_bibex = 0;
                List<PDFDocument> pdf_document_to_check_for_bibtex = this.library.PDFDocuments;
                for (int i = 0; i < pdf_document_to_check_for_bibtex.Count && i < NUM_TO_CHECK; ++i)
                {
                    if (!pdf_document_to_check_for_bibtex[i].Deleted && String.IsNullOrEmpty(pdf_document_to_check_for_bibtex[i].BibTex))
                    {
                        ++total_without_bibex;
                    }
                }

                // Set the visibility - must have a few documents, and more than 
                ObjNotMuchBibTeXDescriptionBorder.Visibility = Visibility.Collapsed;
                if (this.library.PDFDocuments.Count > EMPTY_LIBRARY_THRESHOLD && pdf_document_to_check_for_bibtex.Count > 0)
                {
                    if (total_without_bibex / (double)pdf_document_to_check_for_bibtex.Count >= 0.5 || total_without_bibex / (double)NUM_TO_CHECK >= 0.5)
                    {
                        ObjNotMuchBibTeXDescriptionBorder.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        void ButtonBibTexSniffer_Click(object sender, RoutedEventArgs e)
        {
            GoogleBibTexSnifferControl sniffer = new GoogleBibTexSnifferControl();
            sniffer.Show(ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything, null, null);
        }

        void ButtonExploreInBrainstorm_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_ExploreInBrainstorm);
            MainWindowServiceDispatcher.Instance.ExploreLibraryInBrainstorm(this.library);
        }

        void ButtonExploreInPivot_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_ExploreInPivot);
            MainWindowServiceDispatcher.Instance.OpenPivot(this.library, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }
        
        void ButtonExpedition_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Expedition_Open_Library);
            MainWindowServiceDispatcher.Instance.OpenExpedition(this.library, null);
        }
        
        void ButtonExportLibrary_Click(object sender, RoutedEventArgs e)
        {
            LibraryExporter.Export(this.library, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }
        
        void ButtonExportBibTex_Click(object sender, RoutedEventArgs e)
        {
            BibTexExport.Export(ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        void ButtonExportWord2007_Click(object sender, RoutedEventArgs e)
        {
            Word2007Export.Export(ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        void ButtonAnnotationsReport_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.GenerateAnnotationReport(library, ObjLibraryCatalogControl.SelectedPDFDocumentsElseEverything);
        }

        void ButtonGenerateReferences_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_GenerateReferences);
            SafeThreadPool.QueueUserWorkItem(o => CitationFinder.FindCitations(library));
        }

        void ButtonFindDuplicates_Click(object sender, RoutedEventArgs e)
        {
            //VisualGalleryControl.Test(library);

            FeatureTrackingManager.Instance.UseFeature(Features.Library_FindDuplicates);
            MassDuplicateCheckingControl.FindDuplicatesForLibrary(library);
        }

        void ButtonSyncMetadaWithCloud_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonSyncPopup))
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Sync_SyncMetadata);
                LibrarySyncManager.Instance.RequestSync(new LibrarySyncManager.SyncRequest(false, library, true, false, false));
            }
        }

        void ButtonSyncPDFsWithCloud_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonSyncPopup))
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Sync_SyncMetadataAndPDFs);
                LibrarySyncManager.Instance.RequestSync(new LibrarySyncManager.SyncRequest(false, library, true, true, false));
            }
        }

        void ButtonSyncDetails_Click(object sender, RoutedEventArgs e)
        {
            using (AugmentedPopupAutoCloser apac = new AugmentedPopupAutoCloser(ButtonSyncPopup))
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Sync_SyncDetails);
                LibrarySyncManager.Instance.RequestSync(new LibrarySyncManager.SyncRequest(true, library, true, true, false));
            }
        }

        void ButtonWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            new FolderWatcherChooser(library).ShowDialog();
        }

        void ButtonAddDocuments_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Filter = "PDF Files|*.pdf",
                    Multiselect = true,
                    Title = "Select the PDF documents you wish to add to your document library"
                })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ImportingIntoLibrary.AddNewPDFDocumentsToLibrary_ASYNCHRONOUS(library, false, false, dlg.FileNames);
                }
            }
        }

        void ButtonAddDocumentsFromLibrary_Click(object sender, RoutedEventArgs e)
        {
            // First choose the source library
            string message = String.Format("You are about to import a lot of PDFs into the library named '{0}'.  Please choose the library FROM WHICH you wish to import the PDFs.", this.library.WebLibraryDetail.Title);
            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary(message);
            if (null != web_library_detail)
            {
                if (web_library_detail == this.library.WebLibraryDetail)
                {
                    MessageBoxes.Error("You can not copy documents into the same library.");
                    return;
                }

                // Then check that they still want to do this
                string message2 = String.Format("You are about to copy ALL of the PDFs from the library named '{0}' into the library named '{1}'.  Are you sure you want to do this?", web_library_detail.Title, this.library.WebLibraryDetail.Title);
                if (!MessageBoxes.AskQuestion(message2))
                {
                    return;                    
                }


                // They are sure!
                ImportingIntoLibrary.ClonePDFDocumentsFromOtherLibrary_ASYNCHRONOUS(web_library_detail.library.PDFDocuments, this.library);
            }
        }
        
        void ButtonAddDocumentsFromFolder_Click(object sender, RoutedEventArgs e)
        {
            new ImportFromFolder(library).ShowDialog();
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
