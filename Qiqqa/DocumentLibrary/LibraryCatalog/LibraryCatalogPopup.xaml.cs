using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.AnnotationsReportBuilding.LegacyAnnotationConvertorStuff;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.MetadataSuggestions;
using Qiqqa.Documents.PDF.PDFControls.MetadataControls;
using Qiqqa.InCite;
using Qiqqa.Synchronisation.BusinessLogic;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.BibTex.Parsing;

namespace Qiqqa.DocumentLibrary.LibraryCatalog
{
    /// <summary>
    /// Interaction logic for LibraryCatalogPopup.xaml
    /// </summary>
    public partial class LibraryCatalogPopup : UserControl
    {
        private List<PDFDocument> pdf_documents;
        private AugmentedPopup popup;

        public LibraryCatalogPopup(List<PDFDocument> pdf_documents)
        {
            this.pdf_documents = pdf_documents;

            Theme.Initialize();

            InitializeComponent();

            MenuOpen.Click += MenuOpen_Click;
            MenuOpenAgain.Click += MenuOpenAgain_Click;
            MenuOpenOutside.Click += MenuOpenOutside_Click;
            MenuOpenInWindowsExplorer.Click += MenuOpenInWindowsExplorer_Click;
            MenuCloudUpload.Click += MenuCloudUpload_Click;
            MenuCloudDownload.Click += MenuCloudDownload_Click;
            MenuViewAllAnnotations.Click += MenuViewAllAnnotations_Click;
            MenuDelete.Click += MenuDelete_Click;
            MenuUseFilenameAsTitle.Click += MenuUseFilenameAsTitle_Click;
            MenuUseDirectoriesAsTags.Click += MenuUseDirectoriesAsTags_Click;
            MenuUseKeywordsAsTags.Click += MenuUseKeywordsAsTags_Click;
            MenuCopyBibTeXKey.Click += MenuCopyBibTeXKey_Click;
            MenuCopyQiqqaURI.Click += MenuCopyQiqqaURI_Click;
            MenuInCite_Word.Click += MenuInCite_Word_Click;
            MenuInCite_WordSeparated.Click += MenuInCite_WordSeparated_Click;
            MenuInCite_Snippet.Click += MenuInCite_Snippet_Click;

            MenuCopyToAnotherLibrary.Click += MenuCopyToAnotherLibrary_Click;
            MenuMoveToAnotherLibrary.Click += MenuMoveToAnotherLibrary_Click;
            MenuConvertLegacyAnnotations.Click += MenuConvertLegacyAnnotations_Click;
            MenuForgetLegacyAnnotations.Click += MenuForgetLegacyAnnotations_Click;
            MenuExploreInBrainstorm.Click += MenuExploreInBrainstorm_Click;
            MenuExploreInExpedition.Click += MenuExploreInExpedition_Click;
            MenuExploreInPivot.Click += MenuExploreInPivot_Click;
            MenuAddMultipleTags.Click += MenuAddMultipleTags_Click;
            MenuRemoveAllTags.Click += MenuRemoveAllTags_Click;
            MenuRemoveAllBibTeX.Click += MenuRemoveAllBibTeX_Click;
            MenuForceOCR_eng.Click += MenuForceOCR_Click;
            MenuForceOCR_fra.Click += MenuForceOCR_Click;
            MenuForceOCR_rus.Click += MenuForceOCR_Click;
            MenuForceOCR_deu.Click += MenuForceOCR_Click;
            MenuForceOCR_spa.Click += MenuForceOCR_Click;
            MenuForceOCR_por.Click += MenuForceOCR_Click;
            MenuClearOCR.Click += MenuClearOCR_Click;
            MenuReIndex.Click += MenuReIndex_Click;

            MenuDelete.Background = ThemeColours.Background_Brush_Warning_Transparent;

            popup = new AugmentedPopup(this);
        }

        private void MenuInCite_Snippet_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            if (0 < pdf_documents.Count)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitationSnippet_FromPopup);
                PDFDocumentCitingTools.CiteSnippetPDFDocuments(false, pdf_documents);
            }
        }

        private void MenuInCite_Word_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            if (0 < pdf_documents.Count)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitation_FromPopup);
                PDFDocumentCitingTools.CitePDFDocuments(pdf_documents, false);
            }
        }

        private void MenuInCite_WordSeparated_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            if (0 < pdf_documents.Count)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitation_FromPopup);
                PDFDocumentCitingTools.CitePDFDocuments(pdf_documents, true);
            }
        }

        private void MenuExploreInExpedition_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_ExploreDocumentInExpedition);

            if (0 < pdf_documents.Count)
            {
                PDFDocument pdf_document = pdf_documents[0];
                MainWindowServiceDispatcher.Instance.OpenExpedition(pdf_document.Library, pdf_document);
            }
        }

        private void MenuExploreInPivot_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_ExploreDocumentInPivot);

            if (0 < pdf_documents.Count)
            {
                PDFDocument pdf_document = pdf_documents[0];
                MainWindowServiceDispatcher.Instance.OpenPivot(pdf_document.Library, pdf_documents);
            }
        }

        private void MenuReIndex_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_ReIndex);

            foreach (var pdf_document in pdf_documents)
            {
                pdf_document.Library.LibraryIndex.ReIndexDocument(pdf_document);
            }
        }

        private void MenuCopyQiqqaURI_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_CopyQiqqaURI);

            StringBuilder sb = new StringBuilder();
            sb.Append("qiqqa://open/");
            sb.Append(pdf_documents[0].Library.WebLibraryDetail.Id);
            sb.Append("/");
            sb.Append(pdf_documents[0].Fingerprint);

            // To clipboard
            string html = @"Version:0.9
StartHTML:<<<<<<<1
EndHTML:<<<<<<<2
StartFragment:<<<<<<<3
EndFragment:<<<<<<<4
SourceURL: {0}
<html>
<body>
<!--StartFragment-->
<a href='{0}'>{1}</a>
<!--EndFragment-->
</body>
</html>";

            string result = sb.ToString();
            string link = String.Format(html, result, "[Qiqqa]");

            DataObject data_object = new DataObject();
            data_object.SetData(DataFormats.Html, link);
            data_object.SetData(DataFormats.Text, result);
            Clipboard.SetDataObject(data_object, true);

            StatusManager.Instance.UpdateStatus("CopyQiqqaURI", String.Format("Copied '{0}' to clipboard.", result));
        }

        private void MenuCopyBibTeXKey_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_CopyBibTeXKey);

            StringBuilder sb = new StringBuilder();
            foreach (var pdf_document in pdf_documents)
            {
                string key = pdf_document.BibTex.Key;
                if (!String.IsNullOrEmpty(key))
                {
                    sb.Append(key);
                    sb.Append(",");
                }
                else
                {
                    MessageBoxes.Warn("'{0}' does not have a BibTeX key.", pdf_document.TitleCombined);
                }
            }


            // To clipboard
            string result = sb.ToString().Trim(',');
            if (!String.IsNullOrEmpty(result))
            {
                result = @"\cite{" + result + @"}";

                ClipboardTools.SetText(result);
                StatusManager.Instance.UpdateStatus("CopyBibTeXKey", String.Format("Copied '{0}' to clipboard.", result));
            }
        }

        private void MenuConvertLegacyAnnotations_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            int imported_count = 0;

            FeatureTrackingManager.Instance.UseFeature(Features.Library_ImportLegacyAnnotations);
            foreach (var pdf_document in pdf_documents)
            {
                try
                {
                    imported_count += LegacyAnnotationConvertor.ImportLegacyAnnotations(pdf_document);   // TODO: do this sort of heavy task in a background task; now it locks up the UI
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error while importing legacy annotations.");
                }
            }

            MessageBoxes.Info(imported_count + " legacy annotations imported.");
        }

        private void MenuForgetLegacyAnnotations_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_ForgetLegacyAnnotations);
            foreach (var pdf_document in pdf_documents)
            {
                try
                {
                    LegacyAnnotationConvertor.ForgetLegacyAnnotations(pdf_document);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error while forgetting legacy annotations.");
                }
            }

            MessageBoxes.Info("Legacy annotations removed.");
        }

        private void MenuForceOCR_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            string language = "";
            MenuItem menu_item = sender as MenuItem;
            if (null != menu_item)
            {
                language = menu_item.Name.Substring(menu_item.Name.Length - 3, 3);
            }

            FeatureTrackingManager.Instance.UseFeature(
                Features.Library_ForceOCR,
                "language", language
                );

            foreach (var pdf_document in pdf_documents)
            {
                if (pdf_document.DocumentExists)
                {
                    pdf_document.PDFRenderer.ForceOCRText(language);
                }
            }
        }

        private void MenuClearOCR_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_ClearOCR);

            foreach (var pdf_document in pdf_documents)
            {
                pdf_document.PDFRenderer.ClearOCRText();
            }
        }

        private void MenuAddMultipleTags_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_AddMultipleTags);

            QuickAddTagsWindow qatw = new QuickAddTagsWindow(pdf_documents);
            qatw.Show();
        }

        private void MenuRemoveAllTags_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_RemoveAllTags);

            string warning_message = String.Format("You are about to remove EVERY tag from {0} document(s).  This is irreversible and may lead to tears.  Are you sure you want to do this?", pdf_documents.Count);
            if (MessageBoxes.AskQuestion("{0}", warning_message))
            {
                if (MessageBoxes.AskQuestion("Are you REALLY sure?"))
                {
                    foreach (var pdf_document in pdf_documents)
                    {
                        pdf_document.Tags = "";
                        pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Tags));
                    }

                    MessageBoxes.Info("Your tags have been cleared.");
                }
            }
        }

        private void MenuRemoveAllBibTeX_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_RemoveAllBibTeX);

            string warning_message = String.Format("You are about to remove EVERY BibTeX record from {0} document(s).  This is irreversible and may lead to tears.  Are you sure you want to do this?", pdf_documents.Count);
            if (MessageBoxes.AskQuestion("{0}", warning_message))
            {
                if (MessageBoxes.AskQuestion("Are you REALLY sure?"))
                {
                    foreach (var pdf_document in pdf_documents)
                    {
                        pdf_document.BibTex = new BibTexItem();
                        pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
                    }

                    MessageBoxes.Info("Your BibTeX records have been cleared.");
                }
            }
        }

        private void MenuExploreInBrainstorm_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_ExploreDocumentInBrainstorm);

            MainWindowServiceDispatcher.Instance.ExploreDocumentInBrainstorm(pdf_documents);
        }

        private void MenuCopyToAnotherLibrary_Click(object sender, RoutedEventArgs e)
        {
            Logging.Debug特("User picked 'Copy to another library' menu item.");
            popup.Close();
            MoveOrCopyCommon(Features.Library_CopyDocumentToAnotherLibrary, false);
        }

        private void MenuMoveToAnotherLibrary_Click(object sender, RoutedEventArgs e)
        {
            Logging.Debug特("User picked 'Mode to another library' menu item.");
            popup.Close();
            MoveOrCopyCommon(Features.Library_MoveDocumentToAnotherLibrary, true);
        }

        private void MoveOrCopyCommon(Feature feature, bool delete_source_pdf_documents)
        {
            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null == web_library_detail)
            {
                Logging.Warn("User did not pick a library to copy or move to: pick = NULL.");
                return;
            }

            // Check that we are not moving any docs into the same library
            bool same_library = false;
            foreach (var pdf_document in pdf_documents)
            {
                if (pdf_document.Library.WebLibraryDetail == web_library_detail)
                {
                    same_library = true;
                }
            }
            if (same_library)
            {
                MessageBoxes.Error("You can not move/copy a PDF from/to the same library.");
                return;
            }

            // Copying / Moving PDFDocuments takes a while, particularly if it's a large set.
            //
            // Hence this WORK should be executed by a background task.
            SafeThreadPool.QueueUserWorkItem(o =>
            {
                FeatureTrackingManager.Instance.UseFeature(feature);

            ImportingIntoLibrary.ClonePDFDocumentsFromOtherLibrary_ASYNCHRONOUS(pdf_documents, web_library_detail.library, new LibraryPdfActionCallbacks
            {
                //OnAddedOrSkipped   -- too risky for now: we MAY skip when the bibtex differ in both libs and then we shouldn't delete this record!
                OnAdded = (pdf_document, filename) =>
                {
					source = pdf_document;
					// TODO ...
                    if (delete_source_pdf_documents && null != target && null != source && target != source)
                    {
	                    pdf_document.Library.DeleteDocument(pdf_document);
                    }
                }
            });
        }

        private void MenuUseKeywordsAsTags_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Document_ExtractKeywordsAsTags);

            foreach (var pdf_document in pdf_documents)
            {
                try
                {
                    PDFMetadataExtractor.ExtractKeywordsAsTags(pdf_document);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem extracting the document keywords as tags.");
                }
            }
        }

        private void MenuUseDirectoriesAsTags_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_UseDirectoriesAsTags);

            if (MessageBoxes.AskQuestion("Are you sure you wish to use the original directory structure of {0} document(s) as tags?", pdf_documents.Count))
            {
                foreach (var pdf_document in pdf_documents)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(pdf_document.DownloadLocation))
                        {
                            string path = Path.GetDirectoryName(pdf_document.DownloadLocation);
                            string[] directories = path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string directory in directories)
                            {
                                pdf_document.AddTag(directory);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was a problem using the directory structure as tags.");
                    }
                }
            }
        }

        private void MenuUseFilenameAsTitle_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_UseFilenameAsTitle);

            if (MessageBoxes.AskQuestion("Are you sure you wish to replace the titles of {0} document(s) with their original filenames?", pdf_documents.Count))
            {
                foreach (var pdf_document in pdf_documents)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(pdf_document.DownloadLocation))
                        {
                            pdf_document.TitleCombined = Path.GetFileNameWithoutExtension(pdf_document.DownloadLocation);
                            pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Title));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was a problem, using an original filename as a title");
                    }
                }
            }
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            if (MessageBoxes.AskQuestion("Are you sure you wish to delete {0} document(s)?", pdf_documents.Count))
            {
                foreach (var pdf_document in pdf_documents)
                {
                    pdf_document.Library.DeleteDocument(pdf_document);
                }
            }
        }

        private void MenuViewAllAnnotations_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            MainWindowServiceDispatcher.Instance.GenerateAnnotationReport(pdf_documents[0].Library, pdf_documents);
        }

        private void MenuCloudDownload_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();


            List<string> fingerprints = new List<string>();
            foreach (var pdf_document in pdf_documents)
            {
                fingerprints.Add(pdf_document.Fingerprint);
            }

            LibrarySyncManager.Instance.QueueGet(pdf_documents[0].Library, fingerprints.ToArray());
        }

        private void MenuCloudUpload_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();


            List<string> fingerprints = new List<string>();
            foreach (var pdf_document in pdf_documents)
            {
                fingerprints.Add(pdf_document.Fingerprint);
            }

            LibrarySyncManager.Instance.QueuePut(pdf_documents[0].Library, fingerprints.ToArray());
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            if (5 < pdf_documents.Count)
            {
                if (!MessageBoxes.AskQuestion("Are you sure you wish to open {0} document(s)?", pdf_documents.Count))
                {
                    return;
                }
            }

            foreach (var pdf_document in pdf_documents)
            {
                MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
            }
        }

        private void MenuOpenAgain_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            if (5 < pdf_documents.Count)
            {
                if (!MessageBoxes.AskQuestion("Are you sure you wish to open {0} document(s)?", pdf_documents.Count))
                {
                    return;
                }
            }

            foreach (var pdf_document in pdf_documents)
            {
                MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document, open_again: true);
            }
        }

        private void MenuOpenOutside_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_OpenOutsideQiqqa);

            if (5 < pdf_documents.Count)
            {
                if (!MessageBoxes.AskQuestion("Are you sure you wish to open {0} document(s)?", pdf_documents.Count))
                {
                    return;
                }
            }

            foreach (var pdf_document in pdf_documents)
            {
                if (pdf_document.DocumentExists)
                {
                    Process.Start(pdf_document.DocumentPath);
                }
            }
        }

        private void MenuOpenInWindowsExplorer_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();

            FeatureTrackingManager.Instance.UseFeature(Features.Library_OpenInWindowsExplorer);

            if (0 < pdf_documents.Count)
            {
                FileTools.BrowseToFileInExplorer(pdf_documents[0].DocumentPath);
            }
        }

        public void Open()
        {
            popup.IsOpen = true;
        }
    }
}
