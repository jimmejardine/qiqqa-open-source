using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.DocumentConversionStuff
{
    /// <summary>
    /// Interaction logic for DocumentConversionControl.xaml
    /// </summary>
    public partial class DocumentConversionControl : UserControl
    {
        public DocumentConversionControl()
        {
            InitializeComponent();

            ObjInstructions.AllowDrop = true;
            ObjInstructions.DragOver += OnDragOver;
            ObjInstructions.Drop += OnDrop;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Link;
            }

            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
                DoConvert(filenames);
            }

            e.Handled = true;
        }

        private void DoConvert(string[] filenames)
        {
            foreach (string filename in filenames)
            {
                Logging.Info("Processing {0}", filename);
                if (DocumentConversion.CanConvert(filename))
                {
                    string pdf_filename = TempFile.GenerateTempFilename("pdf");
                    DocumentConversion.Convert(filename, pdf_filename);

                    if (ObjAddToGuest.IsChecked ?? false)
                    {
                        WPFDoEvents.InvokeInUIThread(() =>
                            {
                                PDFDocument pdf_document = WebLibraryManager.Instance.Library_Guest.AddNewDocumentToLibrary_SYNCHRONOUS(new FilenameWithMetadataImport
                                {
                                    Filename = pdf_filename,
                                    OriginalFilename = filename,
                                    SuggestedDownloadSourceURI = filename
                                }, true);
                                PDFReadingControl pdf_reading_control = MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
                                pdf_reading_control.EnableGuestMoveNotification();
                            },
                            priority: DispatcherPriority.Background
                        );
                    }

                    if (ObjPromptToSave.IsChecked ?? false)
                    {
                        string recommended_filename = TempFile.GenerateTempFilename("pdf");

                        SaveFileDialog save_file_dialog = new SaveFileDialog();
                        save_file_dialog.AddExtension = true;
                        save_file_dialog.CheckPathExists = true;
                        save_file_dialog.DereferenceLinks = true;
                        save_file_dialog.OverwritePrompt = true;
                        save_file_dialog.ValidateNames = true;
                        save_file_dialog.DefaultExt = "pdf";
                        save_file_dialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                        save_file_dialog.FileName = recommended_filename;

                        if (true == save_file_dialog.ShowDialog())
                        {
                            string target_filename = save_file_dialog.FileName;
                            File.Copy(pdf_filename, target_filename);
                        }
                    }

                    File.Delete(pdf_filename);
                }
                else
                {
                    StatusManager.Instance.UpdateStatus("DOC_CONV", String.Format("Unable to convert a file of type {0}", Path.GetExtension(filename)));
                }
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            DocumentConversionControl dcc = new DocumentConversionControl();
            ControlHostingWindow w = new ControlHostingWindow("PDF Convert", dcc);
            w.Show();
        }
#endif
        #endregion
    }
}
