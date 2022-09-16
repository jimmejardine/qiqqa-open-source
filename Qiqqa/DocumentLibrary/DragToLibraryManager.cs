using System.Collections.Generic;
using System.Text;
using System.Windows;
using Qiqqa.DocumentLibrary.Import.Manual;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary
{
    internal class DragToLibraryManager
    {
        private WebLibraryDetail default_library;
        private FrameworkElement previously_registered_control = null;

        public DragToLibraryManager(WebLibraryDetail web_library_detail)
        {
            this.default_library = web_library_detail;
        }

        public WebLibraryDetail DefaultLibrary
        {
            get => default_library;
            set => default_library = value;
        }

        public void RegisterControl(FrameworkElement element)
        {
            if (null != previously_registered_control)
            {
                element.AllowDrop = false;
                element.DragEnter -= OnDragEnter;
                element.DragOver -= OnDragOver;
                element.Drop -= OnDrop;
            }

            element.AllowDrop = true;
            element.DragEnter += OnDragEnter;
            element.DragOver += OnDragOver;
            element.Drop += OnDrop;

            previously_registered_control = element;
        }

        public void OnDragEnter(object sender, DragEventArgs e)
        {
#if false
            Logging.Debug("Allowed effects are {0}", e.AllowedEffects);

            foreach (string format in e.Data.GetFormats(true))
            {
                Logging.Debug("Format is {0}", format);
            }
#endif
        }

        public void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Link;
            }
            else if (e.Data.GetDataPresent("UniformResourceLocator"))
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Link;
            }
            else if (e.Data.GetDataPresent("FileGroupDescriptor"))
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Link;
            }
            else if (e.Data.GetDataPresent(typeof(PDFDocument)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(List<PDFDocument>)))
            {
                e.Effects = DragDropEffects.Copy;
            }

            e.Handled = true;
        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            // Pick the library
            WebLibraryDetail web_library_detail = default_library;
            if (null == web_library_detail)
            {
                web_library_detail = WebLibraryPicker.PickWebLibrary();
            }

            // If there still is no library (the user canceled perhaps)
            if (null == web_library_detail)
            {
                Logging.Info("No library was selected for the DragToLibraryManager.");
                return;
            }

#if DEBUG
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("The available formats are:");
                foreach (string format in e.Data.GetFormats(true))
                {
                    sb.AppendFormat(" - {0}\n", format);
                }
                Logging.Debug(sb.ToString());
            }
#endif

#if DEBUG
            if (Runtime.IsRunningInVisualStudioDesigner) return;
#endif

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);

                // IF they have dragged and dropped a single directory
                if (0 < filenames.Length && Directory.Exists(filenames[0]))
                {
                    if (1 == filenames.Length)
                    {
                        // Invoke the directory handler
                        new ImportFromFolder(web_library_detail, filenames[0]).ShowDialog();
                    }
                    else
                    {
                        MessageBoxes.Warn("You can drag only one directory at a time to Qiqqa.");
                    }
                }
                else
                {
                    ImportingIntoLibrary.AddNewPDFDocumentsToLibrary_ASYNCHRONOUS(web_library_detail, false, false, filenames);
                }
            }
            else if (e.Data.GetDataPresent("UniformResourceLocator"))
            {
                string download_url = DragDropTools.GetDataString("UniformResourceLocator", e);
                Logging.Info("The dropped item is {0}", download_url);
                ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_ASYNCHRONOUS(web_library_detail, download_url);
            }
            else if (e.Data.GetDataPresent(typeof(PDFDocument)))
            {
                PDFDocument pdf_document = (PDFDocument)e.Data.GetData(typeof(PDFDocument));
                Logging.Info("The dropped item is {0}", pdf_document);
                ImportingIntoLibrary.ClonePDFDocumentsFromOtherLibrary_ASYNCHRONOUS(pdf_document, web_library_detail, suppress_signal_that_docs_have_changed: false);
            }
            else if (e.Data.GetDataPresent(typeof(List<PDFDocument>)))
            {
                List<PDFDocument> pdf_documents = (List<PDFDocument>)e.Data.GetData(typeof(List<PDFDocument>));
                ImportingIntoLibrary.ClonePDFDocumentsFromOtherLibrary_ASYNCHRONOUS(pdf_documents, web_library_detail);
            }
            else
            {
                Logging.Info("Not using any of:");
                foreach (string s in e.Data.GetFormats())
                {
                    Logging.Info(s);
                }
            }

            e.Handled = true;
        }
    }
}
