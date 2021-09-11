using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.Common;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.Documents.PDF.DiskSerialisation;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Reflection;
using Utilities.Strings;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Documents.PDF
{
    internal class LockObject : object
    {
    }

    public partial class PDFDocument
    {
        private LockObject access_lock = new LockObject();


        private AugmentedBindable<PDFDocument> bindable = null;
        public AugmentedBindable<PDFDocument> Bindable
        {
            get
            {
                lock (access_lock)
                {
                    if (null == bindable)
                    {
                        bindable = new AugmentedBindable<PDFDocument>(this, PDFDocument.property_dependencies);
                        bindable.PropertyChanged += bindable_PropertyChanged;
                    }

                    return bindable;
                }
            }
        }

        private void bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                ReprocessDocumentIfDirty();
            });
        }

        public string TitleCombinedTrimmed => StringTools.TrimToLengthWithEllipsis(TitleCombined, 200);

        public string AuthorsCombinedTrimmed => StringTools.TrimToLengthWithEllipsis(AuthorsCombined, 150);

        public string PublicationTrimmed => StringTools.TrimToLengthWithEllipsis(Publication, 100);

        public void ReprocessDocumentIfDirty()
        {
            bool dirty;

            lock (access_lock)
            {
                dirty = dirtyNeedsReindexing;

                // RESET dirty flag until next check: we will reindex then only when it's gotten dirty *again*!
                dirtyNeedsReindexing = false;
            }

            if (dirty)
            {
                QueueToStorage();
                LibraryRef.Xlibrary.LibraryIndex.ReIndexDocument(this);
            }
        }

        public void QueueToStorage()
        {
            DocumentQueuedStorer.Instance.Queue(this);
        }

        private PDFDocumentCitationManager _pdf_document_citation_manager = null;
        public PDFDocumentCitationManager PDFDocumentCitationManager
        {
            get
            {
                lock (access_lock)
                {
                    if (null == _pdf_document_citation_manager)
                    {
                        _pdf_document_citation_manager = new PDFDocumentCitationManager(this);
                    }
                    return _pdf_document_citation_manager;
                }
            }
        }

        /// <summary>
        /// Throws exception when metadata could not be converted to a valid PDFDocument instance.
        /// </summary>
        /// <param name="library"></param>
        /// <param name="data"></param>
        /// <param name="library_items_annotations_cache"></param>
        /// <returns></returns>
        public static PDFDocument LoadFromMetaData(WebLibraryDetail web_library_detail, string fingerprint, byte[] data, PDFAnnotationList prefetched_annotations_for_document = null)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            ASSERT.Test(!String.IsNullOrEmpty(fingerprint));

            DictionaryBasedObject dictionary = PDFMetadataSerializer.ReadFromStream(data);

            // Recover partially damaged record: if there's no fingerprint in the dictionary, add the given one:
            string id = dictionary["Fingerprint"] as string;
            if (String.IsNullOrEmpty(id))
            {
                Logging.Error("Library '{1}' corrupted? Sanity check: the document record for document fingerprint {0} lacked a fingerprint as part of the mandatory metadata. This has been fixed now.", fingerprint, web_library_detail.DescriptiveTitle);

                dictionary["Fingerprint"] = fingerprint;
                id = fingerprint;
            }

            // extra verification / sanity check: this will catch some very obscure DB corruption, or rather **manual editing**! ;-)
            if (id != fingerprint)
            {
                // see which of them makes the most sense... And DO remember that Qiqqa fingerprint hashes are variable-length, due to an old systemic bug!
                Regex re = new Regex(@"^[a-zA-Z0-9]{20,}(?:_REF)?$");
                bool id_is_possibly_legal = re.IsMatch(id);
                bool key_is_possibly_legal = re.IsMatch(fingerprint);

                // if the entry in the record itself is legal, run with that one. Otherwise, fall back to using the DB record KEY.
                if (id_is_possibly_legal)
                {
                    Logging.Error("Library '{2}' corrupted? Sanity check: given fingerprint '{0}' does not match the fingerprint '{1}' obtained from the DB metadata record. Running with the fingerprint specified in the metadata record.", fingerprint, id, web_library_detail.DescriptiveTitle);
                }
                else
                {
                    Logging.Error("Library '{2}' corrupted? Sanity check: given fingerprint '{0}' does not match the fingerprint '{1}' obtained from the DB metadata record. Running with the fingerprint specified as database record KEY, since the other fingerprint does not look like a LEGAL fingerprint.", fingerprint, id, web_library_detail.DescriptiveTitle);

                    dictionary["Fingerprint"] = fingerprint;
                    id = fingerprint;
                }
            }

            PDFDocument pdf_document = new PDFDocument(web_library_detail, dictionary, prefetched_annotations_for_document);

            // thread-UNSAFE access is permitted as the PDF has just been created and 
            // is NOT known to the outside world (i.e. beyond the scope of this function) 
            // so there's no thread-safety risk yet.
            _ = pdf_document.GetAnnotations();

            return pdf_document;
        }

        public static PDFDocument CreateFromPDF(WebLibraryDetail web_library_detail, string filename, string precalculated_fingerprint__can_be_null)
        {
            string fingerprint = precalculated_fingerprint__can_be_null;
            if (String.IsNullOrEmpty(fingerprint))
            {
                fingerprint = StreamFingerprint.FromFile(filename);
            }

            PDFDocument pdf_document = new PDFDocument(web_library_detail);

            // Store the most important information
            //
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.FileType = Path.GetExtension(filename).TrimStart('.');
            pdf_document.Fingerprint = fingerprint;
            pdf_document.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.DocumentBasePath);

            pdf_document.StoreAssociatedPDFInRepository(filename);

            List<LibraryDB.LibraryItem> library_items = web_library_detail.Xlibrary.LibraryDB.GetLibraryItems(PDFDocumentFileLocations.METADATA, new List<string>() { pdf_document.Fingerprint });
            ASSERT.Test(library_items.Count < 2);
            if (0 == library_items.Count)
            {
                pdf_document.QueueToStorage();
            }
            else
            {
                LibraryDB.LibraryItem library_item = null;

                try
                {
                    library_item = library_items[0];
                    pdf_document = LoadFromMetaData(web_library_detail, pdf_document.Fingerprint, library_item.data);
                }
                catch (Exception ex)
                {
                    // keep the unrecognized data around so we may fix it later...
                    Logging.Error(ex, "There was a problem reloading an existing PDF from existing metadata, so overwriting it! (document fingerprint: {0}, data: {1})", pdf_document.Fingerprint, library_item?.MetadataAsString() ?? "???");

                    // TODO: WARNING: overwriting old (possibly corrupted) records like this can loose you old/corrupted/unsupported metadata content!
                    pdf_document.QueueToStorage();
                    //pdf_document.SaveToMetaData();
                }
            }

            return pdf_document;
        }

        public static PDFDocument CreateFromVanillaReference(WebLibraryDetail web_library_detail)
        {
            PDFDocument pdf_document = new PDFDocument(web_library_detail);

            // Store the most important information
            //
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.FileType = Constants.VanillaReferenceFileType;
            pdf_document.Fingerprint = VanillaReferenceCreating.CreateVanillaReferenceFingerprint();
            pdf_document.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.DocumentBasePath);

            List<LibraryDB.LibraryItem> library_items = web_library_detail.Xlibrary.LibraryDB.GetLibraryItems(PDFDocumentFileLocations.METADATA, new List<string>() { pdf_document.Fingerprint });
            ASSERT.Test(library_items.Count < 2);
            if (0 == library_items.Count)
            {
                pdf_document.QueueToStorage();
            }
            else
            {
                LibraryDB.LibraryItem library_item = null;
                try
                {
                    library_item = library_items[0];
                    pdf_document = LoadFromMetaData(web_library_detail, pdf_document.Fingerprint, library_item.data);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem reloading an existing PDF from existing metadata, so overwriting it! (document fingerprint: {0}, data: {1})", pdf_document.Fingerprint, library_item?.MetadataAsString() ?? "???");

                    // TODO: WARNING: overwriting old (possibly corrupted) records like this can loose you old/corrupted/unsupported metadata content!
                    pdf_document.QueueToStorage();
                }
            }

            return pdf_document;
        }

#if DEBUG
        public static PDFDocument CreateFakeForDesigner()
        {
            PDFDocument pdf_document = new PDFDocument(null);

            // Store the most important information
            //
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.FileType = Constants.VanillaReferenceFileType;
            pdf_document.Fingerprint = VanillaReferenceCreating.CreateVanillaReferenceFingerprint();
            pdf_document.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.DateLastModified = DateTime.UtcNow;

            return pdf_document;
        }
#endif

        internal PDFDocument AssociatePDFWithVanillaReference(string pdf_filename)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            PDFDocument new_pdf_document = AssociatePDFWithVanillaReference_Part1(pdf_filename, LibraryRef);

            // Prevent nasty things when the API is used in unintended ways, where the current document already happens to have that file
            // associated with it:
            if (this != new_pdf_document)
            {
                // Overwrite the new document's metadata with that of the vanilla reference...
#if false
                string fingerprint = new_pdf_document.Fingerprint;

                new_pdf_document.dictionary = (DictionaryBasedObject)this.dictionary.Clone();

                new_pdf_document.Fingerprint = fingerprint;
                new_pdf_document.FileType = Path.GetExtension(pdf_filename).TrimStart('.');
#else
                new_pdf_document.CopyMetaData(this, copy_fingerprint: false, copy_filetype: false);
#endif
                new_pdf_document.QueueToStorage();

                // Delete this one
                Deleted = true;
                QueueToStorage();

                // Tell library to refresh
                LibraryRef.Xlibrary.SignalThatDocumentsHaveChanged(this);
                new_pdf_document.LibraryRef.Xlibrary.SignalThatDocumentsHaveChanged(new_pdf_document);
            }
            else
            {
                MessageBoxes.Warn("The reference has not been associated with {0}", pdf_filename);
            }

            return new_pdf_document;
        }

        public void AddPassword(string password)
        {
            lock (access_lock)
            {
                LibraryRef.Xlibrary.PasswordManager.AddPassword(this, password);
            }
        }

        public void RemovePassword()
        {
            lock (access_lock)
            {
                LibraryRef.Xlibrary.PasswordManager.RemovePassword(this);
            }
        }

        public string GetPassword()
        {
            lock (access_lock)
            {
                return LibraryRef.Xlibrary.PasswordManager.GetPassword(this);
            }
        }
    }
}
