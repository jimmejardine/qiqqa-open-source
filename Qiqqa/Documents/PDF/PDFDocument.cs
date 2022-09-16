using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.Common;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.Documents.PDF.DiskSerialisation;
using Qiqqa.Documents.PDF.PDFRendering;
using Qiqqa.Documents.PDF.ThreadUnsafe;
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

    public class PDFDocument
    {
        private LockObject access_lock;

        private PDFDocument_ThreadUnsafe doc;

        public WebLibraryDetail LibraryRef => doc.LibraryRef;

        public string GetAttributesAsJSON()
        {
            lock (access_lock)
            {
                return doc.GetAttributesAsJSON();
            }
        }

        private PDFDocument(LockObject _lock, WebLibraryDetail web_library_detail)
        {
            access_lock = _lock;
            doc = new PDFDocument_ThreadUnsafe(web_library_detail);
        }

        private PDFDocument(LockObject _lock, WebLibraryDetail web_library_detail, DictionaryBasedObject dictionary)
        {
            access_lock = _lock;
            doc = new PDFDocument_ThreadUnsafe(web_library_detail, dictionary);
        }

        public PDFRenderer PDFRenderer
        {
            get
            {
                lock (access_lock)
                {
                    return doc.PDFRenderer;
                }
            }
        }

        public PDFRendererFileLayer PDFRendererFileLayer
        {
            get
            {
                lock (access_lock)
                {
                    return doc.PDFRendererFileLayer;
                }
            }
        }

        public int SafePageCount
        {
            get
            {
                lock (access_lock)
                {
                    return doc.SafePageCount;
                }
            }
        }

        /// <summary>
        /// This is an approximate response: it takes a *fast* shortcut to check if the given
        /// PDF has been OCR'd in the past.
        ///
        /// The emphasis here is on NOT triggering a new OCR action! Just taking a peek, *quickly*.
        ///
        /// The cost: one(1) I/O per check.
        /// </summary>
        public bool HasOCRdata
        {
            get
            {
                lock (access_lock)
                {
                    return doc.HasOCRdata;
                }
            }
        }

        // TODO: has a cyclic link in the GC to PDFDocument due to PDFDocument being referenced as member of this bindable:
        // PDFDocument -> Bindable -> AugmentedBindable<PDFDocument> -> Underlying -> PDFDocument
        [NonSerialized]
        private AugmentedBindable<PDFDocument> bindable = null;
        public AugmentedBindable<PDFDocument> Bindable
        {
            get
            {
                lock (access_lock)
                {
                    if (null == bindable)
                    {
                        bindable = new AugmentedBindable<PDFDocument>(this, PDFDocument_ThreadUnsafe.property_dependencies);
                        bindable.PropertyChanged += bindable_PropertyChanged;
                    }

                    return bindable;
                }
            }
        }

        private void bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ReprocessDocumentIfDirty();
        }

        public string Fingerprint
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Fingerprint;
                }
            }
            protected set
            {
                lock (access_lock)
                {
                    doc.Fingerprint = value;
                }
            }
        }

        /// <summary>
        /// Unique id for both this document and the library that it exists in.
        /// </summary>
        public string UniqueId
        {
            get
            {
                lock (access_lock)
                {
                    return doc.UniqueId;
                }
            }
        }

        public string FileType
        {
            get
            {
                lock (access_lock)
                {
                    return doc.FileType;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.FileType = value;
                }
            }
        }

        public BibTexItem BibTexItem
        {
            get
            {
                lock (access_lock)
                {
                    return doc.BibTexItem;
                }
            }
        }

        public string BibTex
        {
            get
            {
                lock (access_lock)
                {
                    return doc.BibTex;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.BibTex = value;
                }
            }
        }

        public string BibTexKey
        {
            get
            {
                lock (access_lock)
                {
                    return doc.BibTexKey;
                }
            }
        }

        public string Title
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Title;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Title = value;
                }
            }
        }
        public string TitleSuggested
        {
            get
            {
                lock (access_lock)
                {
                    return doc.TitleSuggested;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.TitleSuggested = value;
                }
            }
        }
        public string TitleCombinedReason
        {
            get
            {
                lock (access_lock)
                {
                    return doc.TitleCombinedReason;
                }
            }
        }

        public string TitleCombined
        {
            get
            {
                lock (access_lock)
                {
                    return doc.TitleCombined;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.TitleCombined = value;
                }
            }
        }

        public string TitleCombinedTrimmed => StringTools.TrimToLengthWithEllipsis(TitleCombined, 200);

        /// <summary>
        /// Is true if the user made this title by hand (e.g. typed it in or got some BibTeX)
        /// </summary>
        public bool IsTitleGeneratedByUser
        {
            get
            {
                lock (access_lock)
                {
                    return doc.IsTitleGeneratedByUser;
                }
            }
        }

        public string Authors
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Authors;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Authors = value;
                }
            }
        }
        public string AuthorsSuggested
        {
            get
            {
                lock (access_lock)
                {
                    return doc.AuthorsSuggested;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.AuthorsSuggested = value;
                }
            }
        }
        public string AuthorsCombinedReason
        {
            get
            {
                lock (access_lock)
                {
                    return doc.AuthorsCombinedReason;
                }
            }
        }

        public string AuthorsCombined
        {
            get
            {
                lock (access_lock)
                {
                    return doc.AuthorsCombined;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.AuthorsCombined = value;
                }
            }
        }

        public string AuthorsCombinedTrimmed => StringTools.TrimToLengthWithEllipsis(AuthorsCombined, 150);

        public string Year
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Year;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Year = value;
                }
            }
        }
        public string YearSuggested
        {
            get
            {
                lock (access_lock)
                {
                    return doc.YearSuggested;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.YearSuggested = value;
                }
            }
        }
        public string YearCombinedReason
        {
            get
            {
                lock (access_lock)
                {
                    return doc.YearCombinedReason;
                }
            }
        }

        /// <summary>
        /// Produce the document's year of publication.
        ///
        /// When producing (getting) this value, the priority is:
        ///
        /// - check the BibTeX `year` field and return that one when it's non-empty
        /// - check the manual-entry `year` field (@xref Year)
        /// - check the suggested year field (@xref YearSuggested)
        /// - if also else fails, return the UNKNOWN_YEAR value.
        ///
        /// When setting this value, the first action in this priority list is executed, where the conditions pass:
        ///
        /// - check if there's a non-empty (partial) BibTeX record: when there is, add/update the `year` field
        /// - update the manual-entry Year field (@xref Year)
        /// </summary>
        public string YearCombined
        {
            get
            {
                lock (access_lock)
                {
                    return doc.YearCombined;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.YearCombined = value;
                }
            }
        }

        public string Publication
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Publication;
                }
            }

            set
            {
                lock (access_lock)
                {
                    doc.Publication = value;
                }
            }
        }

        public string PublicationTrimmed => StringTools.TrimToLengthWithEllipsis(Publication, 100);

        public string Id
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Id;
                }
            }
        }

        public string DownloadLocation
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DownloadLocation;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.DownloadLocation = value;
                }
            }
        }

        public DateTime? DateAddedToDatabase
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DateAddedToDatabase;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.DateAddedToDatabase = value;
                }
            }
        }

        public DateTime? DateLastModified
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DateLastModified;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.DateLastModified = value;
                }
            }
        }

        public DateTime? DateLastRead
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DateLastRead;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.DateLastRead = value;
                }
            }
        }

        public DateTime? DateLastCited
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DateLastCited;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.DateLastCited = value;
                }
            }
        }

        public void MarkAsModified()
        {
            lock (access_lock)
            {
                doc.MarkAsModified();
            }
        }

        public string ReadingStage
        {
            get
            {
                lock (access_lock)
                {
                    return doc.ReadingStage;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.ReadingStage = value;
                }
            }
        }

        public bool? HaveHardcopy
        {
            get
            {
                lock (access_lock)
                {
                    return doc.HaveHardcopy;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.HaveHardcopy = value;
                }
            }
        }

        public bool? IsFavourite
        {
            get
            {
                lock (access_lock)
                {
                    return doc.IsFavourite;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.IsFavourite = value;
                }
            }
        }

        public string Rating
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Rating;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Rating = value;
                }
            }
        }

        public string Comments
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Comments;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Comments = value;
                }
            }
        }

        public string Abstract
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Abstract;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Abstract = value;
                }
            }
        }

        public string Bookmarks
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Bookmarks;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Bookmarks = value;
                }
            }
        }

        public Color Color
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Color;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Color = value;
                }
            }
        }

        public int PageLastRead
        {
            get
            {
                lock (access_lock)
                {
                    return doc.PageLastRead;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.PageLastRead = value;
                }
            }
        }

        public bool Deleted
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Deleted;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.Deleted = value;
                }
            }
        }

        #region --- AutoSuggested ------------------------------------------------------------------------------

        public bool AutoSuggested_PDFMetadata
        {
            get
            {
                lock (access_lock)
                {
                    return doc.AutoSuggested_PDFMetadata;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.AutoSuggested_PDFMetadata = value;
                }
            }
        }

        public bool AutoSuggested_OCRFrontPage
        {
            get
            {
                lock (access_lock)
                {
                    return doc.AutoSuggested_OCRFrontPage;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.AutoSuggested_OCRFrontPage = value;
                }
            }
        }

        public bool AutoSuggested_BibTeXSearch
        {
            get
            {
                lock (access_lock)
                {
                    return doc.AutoSuggested_BibTeXSearch;
                }
            }
            set
            {
                lock (access_lock)
                {
                    doc.AutoSuggested_BibTeXSearch = value;
                }
            }
        }

        #endregion

        #region --- Tags ------------------------------------------------------------------------------

        public void AddTag(string new_tag_bundle)
        {
            bool notify;
            lock (access_lock)
            {
                notify = doc.AddTag(new_tag_bundle);
            }

            if (notify)
            {
                Bindable.NotifyPropertyChanged(nameof(Tags));
                TagManager.Instance.ProcessDocument(this);
            }
        }

        public void RemoveTag(string dead_tag_bundle)
        {
            bool notify;

            lock (access_lock)
            {
                notify = doc.RemoveTag(dead_tag_bundle);
            }

            if (notify)
            {
                Bindable.NotifyPropertyChanged(nameof(Tags));
                TagManager.Instance.ProcessDocument(this);
            }
        }

        public string Tags
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Tags;
                }
            }

            set
            {
                lock (access_lock)
                {
                    doc.Tags = value;
                }
            }
        }

        #endregion ----------------------------------------------------------------------------------------------------

        public string DocumentBasePath
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DocumentBasePath;
                }
            }
        }

        /// <summary>
        /// The location of the PDF on disk.
        /// </summary>
        public string DocumentPath
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DocumentPath;
                }
            }
        }

        public bool DocumentExists
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DocumentExists;
                }
            }
        }

        public long GetDocumentSizeInBytes(long uncached_document_storage_size_override = -1)
        {
            lock (access_lock)
            {
                return doc.GetDocumentSizeInBytes(uncached_document_storage_size_override);
            }
        }

        public bool IsVanillaReference
        {
            get
            {
                lock (access_lock)
                {
                    return doc.IsVanillaReference;
                }
            }
        }

        #region --- Annotations / highlights / ink ----------------------------------------------------------------------

        public PDFAnnotationList GetAnnotations()
        {
            PDFAnnotationList annotations;

            lock (access_lock)
            {
                annotations = doc.GetAnnotations();
            }

            return annotations;
        }

        public void AddUpdatedAnnotation(PDFAnnotation annotation)
        {
            lock (access_lock)
            {
                doc.AddUpdatedAnnotation(annotation);
            }
        }

        public void ReprocessDocumentIfDirty()
        {
            bool dirty;

            lock (access_lock)
            {
                dirty = doc.dirtyNeedsReindexing;

                // RESET dirty flag until next check: we will reindex then only when it's gotten dirty *again*!
                doc.dirtyNeedsReindexing = false;
            }

            if (dirty)
            {
                QueueToStorage();
                LibraryRef.Xlibrary.LibraryIndex.ReIndexDocument(this);
            }
        }

        public string GetAnnotationsAsJSON()
        {
            lock (access_lock)
            {
                return doc.GetAnnotationsAsJSON();
            }
        }

        public void QueueToStorage()
        {
            DocumentQueuedStorer.Instance.Queue(this);
        }

        public PDFHightlightList Highlights
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Highlights;
                }
            }
        }

        internal PDFHightlightList GetHighlights(Dictionary<string, byte[]> library_items_highlights_cache)
        {
            PDFHightlightList highlights;

            lock (access_lock)
            {
                highlights = doc.GetHighlights(library_items_highlights_cache);
            }

            return highlights;
        }

        public string GetHighlightsAsJSON()
        {
            lock (access_lock)
            {
                return doc.GetHighlightsAsJSON();
            }
        }

        public void AddUpdatedHighlight(PDFHighlight highlight)
        {
            lock (access_lock)
            {
                doc.AddUpdatedHighlight(highlight);
            }
        }

        public void RemoveUpdatedHighlight(PDFHighlight highlight)
        {
            lock (access_lock)
            {
                doc.RemoveUpdatedHighlight(highlight);
            }
        }

        public PDFInkList Inks
        {
            get
            {
                lock (access_lock)
                {
                    return doc.Inks;
                }
            }
        }

        internal PDFInkList GetInks()
        {
            PDFInkList inks;

            lock (access_lock)
            {
                inks = doc.GetInks();
            }

            return inks;
        }

        public byte[] GetInksAsJSON()
        {
            lock (access_lock)
            {
                return doc.GetInksAsJSON();
            }
        }

        public void AddPageInkBlob(int page, byte[] page_ink_blob)
        {
            lock (access_lock)
            {
                doc.AddPageInkBlob(page, page_ink_blob);
            }
        }

        #endregion -------------------------------------------------------------------------------------------------

        #region --- Managers ----------------------------------------------------------------------

        [NonSerialized]
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

        #endregion -------------------------------------------------------------------------------------------------

        public void SaveToMetaData(bool force_flush_no_matter_what = false)
        {
            lock (access_lock)
            {
                doc.SaveToMetaData(force_flush_no_matter_what);
            }
        }

        /// <summary>
        /// Throws exception when metadata could not be converted to a valid PDFDocument instance.
        /// </summary>
        /// <param name="library"></param>
        /// <param name="data"></param>
        /// <param name="library_items_annotations_cache"></param>
        /// <returns></returns>
        public static PDFDocument LoadFromMetaData(WebLibraryDetail web_library_detail, string fingerprint, byte[] data)
        {
            ASSERT.Test(!String.IsNullOrEmpty(fingerprint));

            DictionaryBasedObject dictionary = PDFMetadataSerializer.ReadFromStream(data);

            // Recover partially damaged record: if there's no fingerprint in the dictionary, add the given one:
            string id = dictionary["Fingerprint"] as string;
            if (String.IsNullOrEmpty(id))
            {
                dictionary["Fingerprint"] = fingerprint;
            }

            LockObject _lock = new LockObject();
            PDFDocument pdf_document = new PDFDocument(_lock, web_library_detail, dictionary);

            // extra verification / sanity check: this will catch some very obscure DB corruption, or rather **manual editing**! ;-)
            if (pdf_document.Fingerprint != fingerprint)
            {
                Logging.Warn("Sanity check: given fingerprint '{0}' does not match the fingerprint '{1}' obtained from the DB metadata record. Running with the fingerprint specified in the metadata record.", fingerprint, pdf_document.Fingerprint);
            }

            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            _ = pdf_document.doc.GetAnnotations();
            return pdf_document;
        }

        public static PDFDocument CreateFromPDF(WebLibraryDetail web_library_detail, string filename, string precalculated_fingerprint__can_be_null)
        {
            string fingerprint = precalculated_fingerprint__can_be_null;
            if (String.IsNullOrEmpty(fingerprint))
            {
                fingerprint = StreamFingerprint.FromFile(filename);
            }

            LockObject _lock = new LockObject();
            PDFDocument pdf_document = new PDFDocument(_lock, web_library_detail);

            // Store the most important information
            //
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.doc.FileType = Path.GetExtension(filename).TrimStart('.');
            pdf_document.doc.Fingerprint = fingerprint;
            pdf_document.doc.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.doc.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.doc.DocumentBasePath);

            pdf_document.doc.StoreAssociatedPDFInRepository(filename);

            List<LibraryDB.LibraryItem> library_items = web_library_detail.Xlibrary.LibraryDB.GetLibraryItems(PDFDocumentFileLocations.METADATA, new List<string>() { pdf_document.doc.Fingerprint });
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
            LockObject _lock = new LockObject();
            PDFDocument pdf_document = new PDFDocument(_lock, web_library_detail);

            // Store the most important information
            //
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.doc.FileType = Constants.VanillaReferenceFileType;
            pdf_document.doc.Fingerprint = VanillaReferenceCreating.CreateVanillaReferenceFingerprint();
            pdf_document.doc.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.doc.DateLastModified = DateTime.UtcNow;

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
            LockObject _lock = new LockObject();
            PDFDocument pdf_document = new PDFDocument(_lock, null);

            // Store the most important information
            //
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.doc.FileType = Constants.VanillaReferenceFileType;
            pdf_document.doc.Fingerprint = VanillaReferenceCreating.CreateVanillaReferenceFingerprint();
            pdf_document.doc.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.doc.DateLastModified = DateTime.UtcNow;

            return pdf_document;
        }
#endif

        public void CopyMetaData(PDFDocument pdf_document_template, bool copy_fingerprint = true, bool copy_filetype = true)
        {
            // prevent deadlock due to possible incorrect use of this API:
            if (pdf_document_template != this)
            {
                lock (access_lock)
                {
                    doc.CopyMetaData(pdf_document_template.doc, copy_fingerprint, copy_filetype);
                }
            }
        }

        /// <summary>
        /// NB: only call this as part of document creation.
        /// </summary>
        public void CloneMetaData(PDFDocument existing_pdf_document)
        {
            // prevent deadlock due to possible incorrect use of this API:
            if (existing_pdf_document != this)
            {
                Logging.Warn("TODO: CloneMetaData: MERGE metadata for existing document and document which was copied/moved into this library. Target Document: {0}, Source Document: {1}", this.Fingerprint, existing_pdf_document.LibraryRef);

                lock (existing_pdf_document.access_lock)
                {
                    lock (access_lock)
                    {
                        bindable = null;

                        doc.CloneMetaData(existing_pdf_document.doc);

                        // Copy the citations
                        PDFDocumentCitationManager.CloneFrom(existing_pdf_document.PDFDocumentCitationManager);

                        QueueToStorage();
                    }
                }
            }
        }

        public void StoreAssociatedPDFInRepository(string filename)
        {
            lock (access_lock)
            {
                doc.StoreAssociatedPDFInRepository(filename);
            }
        }

        public override string ToString()
        {
            lock (access_lock)
            {
                return doc.ToString();
            }
        }

        internal PDFAnnotation GetAnnotationByGuid(Guid guid)
        {
            lock (access_lock)
            {
                return doc.GetAnnotationByGuid(guid);
            }
        }

        internal PDFDocument AssociatePDFWithVanillaReference(string pdf_filename)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            PDFDocument new_pdf_document = doc.AssociatePDFWithVanillaReference_Part1(pdf_filename, LibraryRef);

            // Prevent nasty things when the API is used in unintended ways, where the current document already happens to have that file
            // associated with it:
            if (this != new_pdf_document)
            {
                // Overwrite the new document's metadata with that of the vanilla reference...
                if (null != new_pdf_document)
                {
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
            }

            return new_pdf_document;
        }

        public void AddPassword(string password)
        {
            lock (access_lock)
            {
                doc.LibraryRef.Xlibrary.PasswordManager.AddPassword(doc, password);
            }
        }

        public void RemovePassword()
        {
            lock (access_lock)
            {
                doc.LibraryRef.Xlibrary.PasswordManager.RemovePassword(doc);
            }
        }

        public string GetPassword()
        {
            lock (access_lock)
            {
                return doc.LibraryRef.Xlibrary.PasswordManager.GetPassword(doc);
            }
        }
    }
}
