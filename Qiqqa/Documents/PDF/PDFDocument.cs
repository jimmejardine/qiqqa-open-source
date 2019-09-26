using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.Common;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.Documents.PDF.DiskSerialisation;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.Documents.PDF.PDFRendering;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF
{
    /// <summary>
    /// ******************* NB NB NB NB NB NB NB NB NB NB NB ********************************
    /// 
    /// ALL PROPERTIES STORED IN THE DICTIONARY MUST BE SIMPLE TYPES - string, int or double.  
    /// NO DATES, NO COLORS, NO STRUCTs.  
    /// If you want to store Color and DateTime, then there are helper methods on the DictionaryBasedObject to convert TO/FROM.  Use those!
    /// Otherwise platform independent serialisation will break!
    /// 
    /// ******************* NB NB NB NB NB NB NB NB NB NB NB ********************************
    /// </summary>

    public class PDFDocument
    {
        private const string VANILLA_REFERENCE_FILETYPE = "VANILLA_REFERENCE";

        public Library Library
        { get; }

        //[NonSerialized]
        //private DictionaryBasedObject dictionary = new DictionaryBasedObject();

        //internal DictionaryBasedObject Dictionary
        //{
        //    get { return dictionary; }
        //}

        static readonly PropertyDependencies property_dependencies = new PropertyDependencies();

        static PDFDocument()
        {
            PDFDocument p = null;

            property_dependencies.Add(() => p.TitleCombined, () => p.Title);
            property_dependencies.Add(() => p.TitleCombined, () => p.BibTex);
            property_dependencies.Add(() => p.AuthorsCombined, () => p.Authors);
            property_dependencies.Add(() => p.AuthorsCombined, () => p.BibTex);
            property_dependencies.Add(() => p.YearCombined, () => p.Year);
            property_dependencies.Add(() => p.YearCombined, () => p.BibTex);

            property_dependencies.Add(() => p.Publication, () => p.BibTex);
            property_dependencies.Add(() => p.BibTex, () => p.Publication);

            property_dependencies.Add(() => p.Title, () => p.TitleCombined);
            property_dependencies.Add(() => p.Title, () => p.TitleCombinedReason);
            property_dependencies.Add(() => p.BibTex, () => p.TitleCombined);
            property_dependencies.Add(() => p.BibTex, () => p.TitleCombinedReason);
            property_dependencies.Add(() => p.TitleSuggested, () => p.TitleCombined);
            property_dependencies.Add(() => p.TitleSuggested, () => p.TitleCombinedReason);
            property_dependencies.Add(() => p.DownloadLocation, () => p.TitleCombined);
            property_dependencies.Add(() => p.DownloadLocation, () => p.TitleCombinedReason);
            property_dependencies.Add(() => p.TitleCombined, () => p.TitleCombinedReason);

            property_dependencies.Add(() => p.Authors, () => p.AuthorsCombined);
            property_dependencies.Add(() => p.Authors, () => p.AuthorsCombinedReason);
            property_dependencies.Add(() => p.BibTex, () => p.AuthorsCombined);
            property_dependencies.Add(() => p.BibTex, () => p.AuthorsCombinedReason);
            property_dependencies.Add(() => p.AuthorsSuggested, () => p.AuthorsCombined);
            property_dependencies.Add(() => p.AuthorsSuggested, () => p.AuthorsCombinedReason);
            property_dependencies.Add(() => p.AuthorsCombined, () => p.AuthorsCombinedReason);

            property_dependencies.Add(() => p.Year, () => p.YearCombined);
            property_dependencies.Add(() => p.Year, () => p.YearCombinedReason);
            property_dependencies.Add(() => p.BibTex, () => p.YearCombined);
            property_dependencies.Add(() => p.BibTex, () => p.YearCombinedReason);
            property_dependencies.Add(() => p.YearSuggested, () => p.YearCombined);
            property_dependencies.Add(() => p.YearSuggested, () => p.YearCombinedReason);
            property_dependencies.Add(() => p.YearCombined, () => p.YearCombinedReason);

            property_dependencies.Add(() => p.BibTex, () => p.Publication);
            property_dependencies.Add(() => p.BibTex, () => p.Id);
            property_dependencies.Add(() => p.BibTex, () => p.Abstract);
        }

        private PDFDocument(Library library)
        {
            this.Library = library;
        }

        [NonSerialized]
        PDFRenderer pdf_renderer;
        public PDFRenderer PDFRenderer
        {
            get
            {
                if (null == pdf_renderer)
                {
                    pdf_renderer = new PDFRenderer(Fingerprint, DocumentPath, this.Library.PasswordManager.GetPassword(this), this.Library.PasswordManager.GetPassword(this));
                }

                return pdf_renderer;
            }
        }

        [NonSerialized]
        PDFRendererFileLayer pdf_renderer_file_layer;
        public PDFRendererFileLayer PDFRendererFileLayer
        {
            get
            {
                if (null == pdf_renderer_file_layer)
                {
                    pdf_renderer_file_layer = new PDFRendererFileLayer(Fingerprint, DocumentPath);
                }

                return pdf_renderer_file_layer;
            }
        }

        public int SafePageCount
        {
            get
            {
                if (DocumentExists)
                {
                    return PDFRenderer.PageCount;
                }
                else
                {
                    return 0;
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
                // do not check if DocumentExists: our pagecount cache check is sufficient and one I/O per check.
                return PDFRendererFileLayer.HasOCRdata(Fingerprint);
            }
        }

        [NonSerialized]
        AugmentedBindable<PDFDocument> bindable = null;
        public AugmentedBindable<PDFDocument> Bindable
        {
            get
            {
                if (null == bindable)
                {
                    bindable = new AugmentedBindable<PDFDocument>(this, property_dependencies);
                    bindable.PropertyChanged += bindable_PropertyChanged;
                }

                return bindable;
            }
        }

        public string Fingerprint
        { get; set; }

        /// <summary>
        /// Unique id for both this document and the library that it exists in.
        /// </summary>
        public string UniqueId
        {
            get
            {
                return string.Format("{0}_{1}", Fingerprint, this.Library.WebLibraryDetail.Id);
            }
        }

        private string _FileType;
        public string FileType
        {
            get
            {
                return _FileType;
            }
            set
            {
                _FileType = value.TrimStart('.').ToLower();
            }
        }

        [NonSerialized]
        private BibTexItem bibtex_item = null;
        [NonSerialized]
        private bool bibtex_item_parsed = false;
        [NonSerialized]
        private bool bibtex_item_edited = false;
        public BibTexItem BibTexItem
        {
            get
            {
                if (!bibtex_item_parsed)
                {
                    bibtex_item = BibTexParser.ParseOne(BibTex, true);
                    bibtex_item_parsed = true;
                }

                return bibtex_item;
            }
        }

        private string _BibTex;
        public string BibTex
        {
            get
            {
                return _BibTex;
            }
            set
            {
                // Clear the cached item
                bibtex_item = null;
                bibtex_item_parsed = false;
                bibtex_item_edited = false;

                // Store the new value
                _BibTex = value;

                // If the bibtex contains title, author or year, use those by clearing out any overriding values
                BibTexItem item = BibTexItem;
                if (item.HasTitle())
                {
                    Title = null;
                }
                if (item.HasAuthor())
                {
                    Authors = null;
                }
                if (item.HasYear())
                {
                    Year = null;
                }
            }
        }

        public bool UpdateBibTex(BibTexItem fresh)
        {
            if (fresh == null || fresh.IsEmpty() || fresh.IsContentIdenticalTo(bibtex_item))
            {
                return false;
            }

            // TODO: check and heuristic for merge/update

            bibtex_item = fresh;
            bibtex_item_parsed = true;
            bibtex_item_edited = false;

            // Store the new value
            _BibTex = bibtex_item.ToBibTex();

            // If the bibtex contains title, author or year, use those by clearing out any overriding values
            if (bibtex_item.HasTitle())
            {
                Title = null;
            }
            if (bibtex_item.HasAuthor())
            {
                Authors = null;
            }
            if (bibtex_item.HasYear())
            {
                Year = null;
            }

            return true;
        }

        public string BibTexKey
        {
            get
            {
                try
                {
                    BibTexItem item = this.BibTexItem;
                    if (null != item)
                    {
                        return item.Key;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "exception in BibTexKey");
                }

                return null;
            }
        }

        public string Title
        { get; private set; }

        public string TitleSuggested
        { get; set; }

        public string TitleCombinedReason
        {
            get
            {
                return String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}\nSource: {4}",
                    TitleCombined,
                    Title,
                    BibTexItem.GetTitle(),
                    TitleSuggested,
                    DownloadLocation
                    );
            }
        }

        public static readonly string TITLE_UNKNOWN = "(unknown title)";
        public string TitleCombined
        {
            get
            {
                if (!String.IsNullOrEmpty(Title)) return Title;

                if (BibTexItem.HasTitle()) return BibTexItem.GetTitle();

                if (!String.IsNullOrEmpty(TitleSuggested)) return TitleSuggested;

                if (!String.IsNullOrEmpty(DownloadLocation)) return DownloadLocation;

                return TITLE_UNKNOWN;
            }
            set
            {
                string old_combined = TitleCombined;
                if (null != old_combined && 0 == old_combined.CompareTo(value))
                {
                    return;
                }

                // If they are clearing out a value, clear the title
                if (String.IsNullOrEmpty(value))
                {
                    Title = null;
                    return;
                }

                // Then see if they are updating bibtex
                if (String.IsNullOrEmpty(Title) && BibTexItem.HasTitle())
                {
                    BibTexItem.SetTitle(value);
                    return;
                }

                // If we get here, they are changing the Title cos there is no bibtex to update...
                Title = value;
            }
        }

        /// <summary>
        /// Is true if the user made this title by hand (e.g. typed it in or got some BibTeX)
        /// </summary>
        public bool IsTitleGeneratedByUser
        {
            get
            {
                if (!String.IsNullOrEmpty(Title)) return true;
                return BibTexItem.HasTitle();
            }
        }

        public string Authors
        { get; private set; }

        public string AuthorsSuggested
        { get; set; }

        public string AuthorsCombinedReason
        {
            get
            {
                return String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}",
                    AuthorsCombined,
                    Authors,
                    BibTexItem.GetAuthor(),
                    AuthorsSuggested
                    );
            }
        }

        public string AuthorsCombined
        {
            get
            {
                if (!String.IsNullOrEmpty(Authors)) return Authors;

                if (BibTexItem.HasAuthor()) return BibTexItem.GetAuthor();

                if (!String.IsNullOrEmpty(AuthorsSuggested)) return AuthorsSuggested;

                return Utilities.Language.NameTools.UNKNOWN_AUTHORS;
            }
            set
            {
                string old_combined = AuthorsCombined;
                if (null != old_combined && 0 == old_combined.CompareTo(value))
                {
                    return;
                }

                // If they are clearing out a value, clear the authors override
                if (String.IsNullOrEmpty(value))
                {
                    Authors = null;
                    return;
                }

                // Then see if they are updating bibtex
                if (String.IsNullOrEmpty(Authors) && BibTexItem.HasAuthor())
                {
                    BibTexItem.SetAuthor(value);
                    return;
                }

                // If we get here, they are changing the Authors cos there is no bibtex to update...
                Authors = value;
            }
        }

        public string Year
        { get; private set; }

        public string YearSuggested
        { get; set; }

        public string YearCombinedReason
        {
            get
            {
                return String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}",
                    YearCombined,
                    Year,
                    BibTexItem.GetYear(),
                    YearSuggested
                    );
            }
        }

        public static readonly string UNKNOWN_YEAR = "(unknown year)";
        public string YearCombined
        {
            get
            {
                if (!String.IsNullOrEmpty(Year)) return Year;

                if (BibTexItem.HasYear()) return BibTexItem.GetYear();

                if (!String.IsNullOrEmpty(YearSuggested)) return YearSuggested;

                return UNKNOWN_YEAR;
            }
            set
            {
                string old_combined = YearCombined;
                if (null != old_combined && 0 == old_combined.CompareTo(value))
                {
                    return;
                }

                // If they are clearing out a value, clear the year override
                if (String.IsNullOrEmpty(value))
                {
                    Year = null;
                    return;
                }

                // Then see if they are updating bibtex
                if (String.IsNullOrEmpty(Year) && BibTexItem.HasYear())
                {
                    BibTexItem.SetYear(value);
                    return;
                }

                // If we get here, they are changing the Year cos there is no bibtex to update...
                Year = value;
            }
        }

        public string Publication
        {
            get
            {
                return BibTexItem.GetGenericPublication();
            }

            set
            {
                BibTexItem bibtex_item = this.BibTexItem;
                if (null != bibtex_item)
                {
                    BibTexItem.SetGenericPublication(value);
                    this.BibTex = bibtex_item.ToBibTex();
                }
            }
        }

        public string Id
        {
            get
            {
                BibTexItem bibtex_item = this.BibTexItem;
                if (null != bibtex_item)
                {
                    return bibtex_item.Key;
                }
                else
                {
                    return "";
                }
            }
        }

        public string DownloadLocation
        { get; set; }

        public DateTime DateAddedToDatabase
        { get; set; }

        public DateTime DateLastModified
        { get; set; }

        public DateTime DateLastRead
        { get; set; }

        public DateTime DateLastCited
        { get; set; }

        public void MarkAsModified()
        {
            DateLastModified = DateTime.UtcNow;
        }

        public string ReadingStage
        { get; set; }

        public bool? HaveHardcopy
        { get; set; }

        public bool? IsFavourite
        { get; set; }

        public string Rating
        { get; set; }

        public string Comments
        { get; set; }

        private string _AbstractOverride;
        public string Abstract
        {
            get
            {
                // First check if there is an abstract override
                {
                    string abstract_override = _AbstractOverride;
                    if (!String.IsNullOrEmpty(abstract_override))
                    {
                        return abstract_override;
                    }
                }

                // Then check if there is an abstract in the bibtex
                {
                    string abstract_bibtex = this.BibTexItem["abstract"];
                    if (!String.IsNullOrEmpty(abstract_bibtex))
                    {
                        return abstract_bibtex;
                    }
                }

                // Otherwise try get the abstract from the doc itself
                return PDFAbstractExtraction.GetAbstractForDocument(this);
            }
            set
            {
                _AbstractOverride = value;
            }
        }

        public string Bookmarks
        { get; set; }

        public Color Color
        { get; set; }

        public int PageLastRead
        { get; set; }

        public bool Deleted
        { get; set; }

        #region --- AutoSuggested ------------------------------------------------------------------------------

        public bool AutoSuggested_PDFMetadata
        { get; set; }

        public bool AutoSuggested_OCRFrontPage
        { get; set; }

        public bool AutoSuggested_BibTeXSearch
        { get; set; }

        #endregion

        #region --- Tags ------------------------------------------------------------------------------

        public void AddTag(string new_tag_bundle)
        {
            HashSet<string> new_tags = TagTools.ConvertTagBundleToTags(new_tag_bundle);

            HashSet<string> tags = TagTools.ConvertTagBundleToTags(Tags);
            int tag_count_old = tags.Count;
            tags.UnionWith(new_tags);
            int tag_count_new = tags.Count;

            // Update listeners if we changed anything
            if (tag_count_old != tag_count_new)
            {
                Tags = TagTools.ConvertTagListToTagBundle(tags);
                Bindable.NotifyPropertyChanged(() => Tags);
                TagManager.Instance.ProcessDocument(this);
            }
        }

        public void RemoveTag(string dead_tag_bundle)
        {
            HashSet<string> dead_tags = TagTools.ConvertTagBundleToTags(dead_tag_bundle);

            HashSet<string> tags = TagTools.ConvertTagBundleToTags(Tags);
            int tag_count_old = tags.Count;
            foreach (string dead_tag in dead_tags)
            {
                tags.Remove(dead_tag);
            }
            int tag_count_new = tags.Count;

            if (tag_count_old != tag_count_new)
            {
                Tags = TagTools.ConvertTagListToTagBundle(tags);
                Bindable.NotifyPropertyChanged(() => Tags);
                TagManager.Instance.ProcessDocument(this);
            }
        }

        private HashSet<string> tags_list = new HashSet<string>();
        public string Tags
        {
            get
            {
                if (tags_list.Count > 0)
                {
                    FeatureTrackingManager.Instance.UseFeature(Features.Legacy_DocumentTagsList);
                    return TagTools.ConvertTagListToTagBundle(tags_list);
                }

                // If we get this far, then there are no tags!  So create some blanks...
                return "";
            }

            set
            {
                tags_list = TagTools.ConvertTagBundleToTags(value);
            }
        }

        #endregion ----------------------------------------------------------------------------------------------------

        public string DocumentBasePath
        {
            get
            {
                return PDFDocumentFileLocations.DocumentBasePath(this.Library, Fingerprint);
            }
        }

        /// <summary>
        /// The location of the PDF on disk.
        /// </summary>
        public string DocumentPath
        {
            get
            {
                return PDFDocumentFileLocations.DocumentPath(this.Library, Fingerprint, FileType);
            }
        }

        [NonSerialized]
        bool document_exists = false;
        public bool DocumentExists
        {
            get
            {
                if (document_exists) return true;

                document_exists = File.Exists(DocumentPath);
                return document_exists;
            }
        }

        public bool IsVanillaReference
        {
            get
            {
                return String.Compare(this.FileType, VANILLA_REFERENCE_FILETYPE, StringComparison.OrdinalIgnoreCase) == 0;
            }
        }

        #region --- Annotations / highlights / ink ----------------------------------------------------------------------

        PDFAnnotationList annotations = null;
        public PDFAnnotationList Annotations
        {
            get
            {
                return GetAnnotations(null);
            }
        }

        public PDFAnnotationList GetAnnotations(Dictionary<string, byte[]> library_items_annotations_cache)
        {
            if (null == annotations)
            {
                annotations = new PDFAnnotationList();
                PDFAnnotationSerializer.ReadFromDisk(this, annotations, library_items_annotations_cache);
                annotations.OnPDFAnnotationListChanged += annotations_OnPDFAnnotationListChanged;
            }

            return annotations;
        }

        public void QueueToStorage()
        {
            DocumentQueuedStorer.Instance.Queue(this);
        }

        void annotations_OnPDFAnnotationListChanged()
        {
            QueueToStorage();
            this.Library.LibraryIndex.ReIndexDocument(this);
        }

        [NonSerialized]
        PDFHightlightList highlights = null;
        public PDFHightlightList Highlights
        {
            get
            {
                return GetHighlights(null);
            }
        }

        internal PDFHightlightList GetHighlights(Dictionary<string, byte[]> library_items_highlights_cache)
        {
            if (null == highlights)
            {
                highlights = new PDFHightlightList();
                PDFHighlightSerializer.ReadFromStream(this, highlights, library_items_highlights_cache);
                highlights.OnPDFHighlightListChanged += highlights_OnPDFHighlightListChanged;
            }

            return highlights;
        }


        void highlights_OnPDFHighlightListChanged()
        {
            QueueToStorage();
            this.Library.LibraryIndex.ReIndexDocument(this);
        }

        [NonSerialized]
        PDFInkList inks = null;
        public PDFInkList Inks
        {
            get
            {
                return GetInks(null);
            }
        }

        internal PDFInkList GetInks(Dictionary<string, byte[]> library_items_inks_cache)
        {
            if (null == inks)
            {
                inks = new PDFInkList();
                PDFInkSerializer.ReadFromDisk(this, inks, library_items_inks_cache);
                inks.OnPDFInkListChanged += inks_OnPDFInkListChanged;
            }

            return inks;
        }

        void inks_OnPDFInkListChanged()
        {
            Logging.Info("Document has changed inks");
            QueueToStorage();
            this.Library.LibraryIndex.ReIndexDocument(this);
        }


        #endregion -------------------------------------------------------------------------------------------------

        #region --- Managers ----------------------------------------------------------------------

        [NonSerialized]
        PDFDocumentCitationManager _pdf_document_citation_manager = null;
        public PDFDocumentCitationManager PDFDocumentCitationManager
        {
            get
            {
                if (null == _pdf_document_citation_manager)
                {
                    _pdf_document_citation_manager = new PDFDocumentCitationManager(this);
                }
                return _pdf_document_citation_manager;
            }
        }

        #endregion -------------------------------------------------------------------------------------------------

        public void SaveToMetaData()
        {
            // Save the metadata            
            PDFMetadataSerializer.WriteToDisk(this);

            // Save the annotations
            if (null != annotations && annotations.Count > 0)
            {
                PDFAnnotationSerializer.WriteToDisk(this);
            }

            // Save the highlights
            if (null != highlights && highlights.Count > 0)
            {
                PDFHighlightSerializer.WriteToDisk(this);
            }

            // Save the inks
            if (null != inks)
            {
                PDFInkSerializer.WriteToDisk(this);
            }
        }

        void bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            QueueToStorage();
            this.Library.LibraryIndex.ReIndexDocument(this);
        }

        /// <summary>
        /// Throws exception when metadata could not be converted to a valid PDFDocument instance.
        /// </summary>
        /// <param name="library"></param>
        /// <param name="data"></param>
        /// <param name="library_items_annotations_cache"></param>
        /// <returns></returns>
        public static PDFDocument LoadFromMetaData(Library library, byte[] data, Dictionary<string, byte[]> /* can be null */ library_items_annotations_cache)
        {
            PDFDocument pdf_document = PDFMetadataSerializer.ReadFromStream(library, data);
            pdf_document.GetAnnotations(library_items_annotations_cache);
            return pdf_document;
        }

        public static PDFDocument CreateFromPDF(Library library, string filename, string precalculated_fingerprint__can_be_null)
        {
            string fingerprint = precalculated_fingerprint__can_be_null;
            if (String.IsNullOrEmpty(fingerprint))
            {
                fingerprint = StreamFingerprint.FromFile(filename);
            }

            PDFDocument pdf_document = new PDFDocument(library);

            // Store the most important information
            pdf_document.FileType = Path.GetExtension(filename);
            pdf_document.Fingerprint = fingerprint;
            pdf_document.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.DocumentBasePath);

            pdf_document.StoreAssociatedPDFInRepository(filename);

            List<LibraryDB.LibraryItem> library_items = library.LibraryDB.GetLibraryItems(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA);
            if (0 == library_items.Count)
            {
                DocumentQueuedStorer.Instance.Queue(pdf_document);
            }
            else
            {
                try
                {
                    LibraryDB.LibraryItem library_item = library_items[0];
                    pdf_document = LoadFromMetaData(library, library_item.data, null);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem reloading an existing PDF from existing metadata, so overwriting it! (Fingerprint: {0})", pdf_document.Fingerprint);
                    DocumentQueuedStorer.Instance.Queue(pdf_document);
                    //pdf_document.SaveToMetaData();
                }
            }

            return pdf_document;
        }

        public static PDFDocument CreateFromVanillaReference(Library library)
        {
            PDFDocument pdf_document = new PDFDocument(library);

            // Store the most important information
            pdf_document.FileType = VANILLA_REFERENCE_FILETYPE;
            pdf_document.Fingerprint = VanillaReferenceCreating.CreateVanillaReferenceFingerprint();
            pdf_document.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.DocumentBasePath);

            List<LibraryDB.LibraryItem> library_items = library.LibraryDB.GetLibraryItems(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA);
            if (0 == library_items.Count)
            {
                DocumentQueuedStorer.Instance.Queue(pdf_document);
            }
            else
            {
                try
                {
                    LibraryDB.LibraryItem library_item = library_items[0];
                    pdf_document = LoadFromMetaData(library, library_item.data, null);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem reloading an existing PDF from existing metadata, so overwriting it! (Fingerprint: {0})", pdf_document.Fingerprint);
                    DocumentQueuedStorer.Instance.Queue(pdf_document);
                }
            }

            return pdf_document;
        }


        /// <summary>
        /// NB: only call this as part of document creation.
        /// </summary>
        public void CloneMetaData(PDFDocument existing_pdf_document)
        {
            bindable = null;

            Logging.Info("Cloning metadata from {0}: {1}", existing_pdf_document.Fingerprint, existing_pdf_document.TitleCombined);
            //dictionary = (DictionaryBasedObject)existing_pdf_document.dictionary.Clone();
            annotations = (PDFAnnotationList)existing_pdf_document.Annotations.Clone();
            highlights = (PDFHightlightList)existing_pdf_document.Highlights.Clone();
            inks = (PDFInkList)existing_pdf_document.Inks.Clone();
            SaveToMetaData();

            // Copy the citations
            PDFDocumentCitationManager.CloneFrom(existing_pdf_document.PDFDocumentCitationManager);

            //  Now clear out the references for the annotations and highlights, so that when they are reloaded the events are resubscribed
            annotations = null;
            highlights = null;
            inks = null;
        }

        public void StoreAssociatedPDFInRepository(string filename)
        {
            if (File.Exists(filename) && !File.Exists(DocumentPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DocumentPath));
                Alphaleonis.Win32.Filesystem.File.Copy(filename, DocumentPath);
            }
        }

        public override string ToString()
        {
            return Fingerprint;
        }

        internal PDFAnnotation GetAnnotationByGuid(Guid guid)
        {
            foreach (PDFAnnotation pdf_annotation in Annotations)
            {
                if (pdf_annotation.Guid == guid)
                {
                    return pdf_annotation;
                }
            }

            return null;
        }

        internal PDFDocument AssociatePDFWithVanillaReference(string pdf_filename)
        {
            // Only works with vanilla references
            if (!IsVanillaReference)
            {
                throw new Exception("You can only associate a PDF with a vanilla reference.");
            }

            // Create the new PDF document
            PDFDocument new_pdf_document = this.Library.AddNewDocumentToLibrary_SYNCHRONOUS(pdf_filename, pdf_filename, pdf_filename, null, null, null, false, true);

            // Overwrite the new document's metadata with that of the vanilla reference...
            if (null != new_pdf_document)
            {
                string fingerprint = new_pdf_document.Fingerprint;
                //new_pdf_document.dictionary = (DictionaryBasedObject)this.dictionary.Clone();
                new_pdf_document.Fingerprint = fingerprint;
                new_pdf_document.FileType = Path.GetExtension(pdf_filename);

                DocumentQueuedStorer.Instance.Queue(new_pdf_document);

                // Delete this one
                this.Deleted = true;
                QueueToStorage();

                // Tell library to refresh
                this.Library.SignalThatDocumentsHaveChanged(new_pdf_document);
            }
            else
            {
                MessageBoxes.Warn("The reference has not been associated with {0}", pdf_filename);
            }

            return new_pdf_document;
        }
    }
}
