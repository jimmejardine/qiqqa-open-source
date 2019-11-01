using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Newtonsoft.Json;
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
using Utilities.Strings;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Documents.PDF.ThreadUnsafe
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

    public class PDFDocument_ThreadUnsafe
    {
        private Library library;
        public Library Library => library;

        private DictionaryBasedObject dictionary = new DictionaryBasedObject();

        public string GetAttributesAsJSON()
        {
            string json = JsonConvert.SerializeObject(dictionary.Attributes, Formatting.Indented);
            return json;
        }

        internal static readonly PropertyDependencies property_dependencies = new PropertyDependencies();

        static PDFDocument_ThreadUnsafe()
        {
            PDFDocument_ThreadUnsafe p = null;

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

        internal PDFDocument_ThreadUnsafe(Library library)
        {
            this.library = library;
            dictionary = new DictionaryBasedObject();
        }

        internal PDFDocument_ThreadUnsafe(Library library, DictionaryBasedObject dictionary)
        {
            this.library = library;
            this.dictionary = dictionary;
        }

        [NonSerialized]
        private PDFRenderer pdf_renderer;
        public PDFRenderer PDFRenderer
        {
            get
            {
                if (null == pdf_renderer)
                {
                    pdf_renderer = new PDFRenderer(Fingerprint, DocumentPath, Library.PasswordManager.GetPassword(this), Library.PasswordManager.GetPassword(this));
                }

                return pdf_renderer;
            }
        }

        [NonSerialized]
        private PDFRendererFileLayer pdf_renderer_file_layer;
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
        public bool HasOCRdata =>
                // do not check if DocumentExists: our pagecount cache check is sufficient and one I/O per check.
                PDFRendererFileLayer.HasOCRdata(Fingerprint);

        public string Fingerprint
        {
            get => dictionary["Fingerprint"] as string;
            /* protected */
            set => dictionary["Fingerprint"] = value;
        }

        /// <summary>
        /// Unique id for both this document and the library that it exists in.
        /// </summary>
        public string UniqueId => string.Format("{0}_{1}", Fingerprint, library.WebLibraryDetail.Id);

        public string FileType
        {
            get => dictionary["FileType"] as string;
            set => dictionary["FileType"] = value.ToLower();
        }

        private BibTexItem bibtex_item = null;
        private bool bibtex_item_parsed = false;
        public BibTexItem BibTexItem
        {
            get
            {
                if (!bibtex_item_parsed)
                {
                    bibtex_item = BibTexParser.ParseOne(BibTex, true);
                    bibtex_item_parsed = true;

                    if (null != bibtex_item)
                    {
                        // if the bibtex is ill formatted, make sure some basic sanity is provided:
                        if (String.IsNullOrWhiteSpace(BibTexItem.Type))
                        {
                            BibTexItem.Type = "empty_and_erroneous";
                        }
                        if (String.IsNullOrWhiteSpace(BibTexItem.Key))
                        {
                            BibTexItem.Key = BibTexTools.GenerateRandomBibTeXKey();
                        }
                    }
                }

                return bibtex_item;
            }
        }

        public string BibTex
        {
            get => dictionary["BibTex"] as string;
            set
            {
                // Clear the cached item
                bibtex_item = null;
                bibtex_item_parsed = false;

                // Store the new value
                dictionary["BibTex"] = value;

                // If the bibtex contains title, author or year, use those by clearing out any overriding values
                if (!String.IsNullOrEmpty(BibTexTools.GetTitle(BibTexItem)))
                {
                    Title = null;
                }
                if (!String.IsNullOrEmpty(BibTexTools.GetAuthor(BibTexItem)))
                {
                    Authors = null;
                }
                if (!String.IsNullOrEmpty(BibTexTools.GetYear(BibTexItem)))
                {
                    Year = null;
                }
            }
        }

        public string BibTexKey
        {
            get
            {
                try
                {
                    BibTexItem item = BibTexItem;
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
        {
            get => dictionary["Title"] as string;
            set => dictionary["Title"] = value;
        }
        public string TitleSuggested
        {
            get => dictionary["TitleSuggested"] as string;
            set => dictionary["TitleSuggested"] = value;
        }
        public string TitleCombinedReason => String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}\nSource: {4}",
                    TitleCombined,
                    Title,
                    BibTexTools.GetTitle(BibTexItem),
                    TitleSuggested,
                    DownloadLocation
                    );

        public string TitleCombined
        {
            get
            {
                if (!String.IsNullOrEmpty(Title)) return Title;

                string bibtex = BibTexTools.GetTitle(BibTexItem);
                if (!String.IsNullOrEmpty(bibtex)) return bibtex;

                if (!String.IsNullOrEmpty(TitleSuggested)) return TitleSuggested;

                if (!String.IsNullOrEmpty(DownloadLocation)) return DownloadLocation;

                return Constants.TITLE_UNKNOWN;
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
                if (String.IsNullOrEmpty(Title) && !String.IsNullOrEmpty(BibTexTools.GetTitle(BibTexItem)))
                {
                    BibTex = BibTexTools.SetTitle(BibTex, value);
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
                string bibtex = BibTexTools.GetTitle(BibTexItem);
                if (!String.IsNullOrEmpty(bibtex)) return true;

                return false;
            }
        }

        public string Authors
        {
            get => dictionary["Authors"] as string;
            set => dictionary["Authors"] = value;
        }
        public string AuthorsSuggested
        {
            get => dictionary["AuthorsSuggested"] as string;
            set => dictionary["AuthorsSuggested"] = value;
        }
        public string AuthorsCombinedReason => String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}",
                    AuthorsCombined,
                    Authors,
                    BibTexTools.GetAuthor(BibTexItem),
                    AuthorsSuggested
                    );

        public string AuthorsCombined
        {
            get
            {
                if (!String.IsNullOrEmpty(Authors)) return Authors;

                string bibtex = BibTexTools.GetAuthor(BibTexItem);
                if (!String.IsNullOrEmpty(bibtex)) return bibtex;

                if (!String.IsNullOrEmpty(AuthorsSuggested)) return AuthorsSuggested;

                return Constants.UNKNOWN_AUTHORS;
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
                if (String.IsNullOrEmpty(Authors) && !String.IsNullOrEmpty(BibTexTools.GetAuthor(BibTexItem)))
                {
                    BibTex = BibTexTools.SetAuthor(BibTex, value);
                    return;
                }

                // If we get here, they are changing the Authors cos there is no bibtex to update...
                Authors = value;
            }
        }

        public string Year
        {
            get => dictionary["Year"] as string;
            set => dictionary["Year"] = value;
        }
        public string YearSuggested
        {
            get => dictionary["YearSuggested"] as string;
            set => dictionary["YearSuggested"] = value;
        }
        public string YearCombinedReason => String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}",
                    YearCombined,
                    Year,
                    BibTexTools.GetYear(BibTexItem),
                    YearSuggested
                    );

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
        /// When setting this value, the first action in this prioirty list is executed, where the conditions pass:
        /// 
        /// - check if there's a non-empty (partial) BibTeX record: when there is, add/update the `year` field
        /// - update the manual-entry Year field (@xref Year)
        /// </summary>
        public string YearCombined
        {
            get
            {
                if (!String.IsNullOrEmpty(Year)) return Year;

                string bibtex_year = BibTexTools.GetYear(BibTexItem);
                if (!String.IsNullOrEmpty(bibtex_year)) return bibtex_year;

                if (!String.IsNullOrEmpty(YearSuggested)) return YearSuggested;

                return Constants.UNKNOWN_YEAR;
            }
            set
            {
                string old_combined = YearCombined;
                if (null != old_combined && 0 == old_combined.CompareTo(value))
                {
                    return;
                }

                // If they are clearing out a value, clear the title
                if (String.IsNullOrEmpty(value))
                {
                    Year = null;
                    return;
                }

                // Then see if they are updating bibtex
                if (String.IsNullOrEmpty(Year) && !String.IsNullOrEmpty(BibTexTools.GetYear(BibTexItem)))
                {
                    BibTex = BibTexTools.SetYear(BibTex, value);
                    return;
                }

                // If we get here, they are changing the Year cos there is no bibtex to update...
                Year = value;
            }
        }

        public string Publication
        {
            get => BibTexTools.GetGenericPublication(BibTexItem);

            set
            {
                BibTexItem bibtex_item = BibTexItem;
                if (null != bibtex_item)
                {
                    BibTexTools.SetGenericPublication(bibtex_item, value);
                    BibTex = bibtex_item.ToBibTex();
                }
            }
        }

        public string Id
        {
            get
            {
                BibTexItem bibtex_item = BibTexItem;
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
        {
            get => dictionary["DownloadLocation"] as string;
            set => dictionary["DownloadLocation"] = value;
        }

        [NonSerialized]
        private DateTime? date_added_to_db = null;
        public DateTime? DateAddedToDatabase
        {
            get
            {
                if (date_added_to_db.HasValue) return date_added_to_db.Value;

                date_added_to_db = dictionary.GetDateTime("DateAddedToDatabase");
                return date_added_to_db;
            }
            set
            {
                date_added_to_db = null;
                dictionary.SetDateTime("DateAddedToDatabase", value);
            }
        }

        [NonSerialized]
        private DateTime? date_last_modified = null;
        public DateTime? DateLastModified
        {
            get
            {
                if (date_last_modified.HasValue) return date_last_modified.Value;

                date_last_modified = dictionary.GetDateTime("DateLastModified");
                return date_last_modified;
            }
            set
            {
                date_last_modified = null;
                dictionary.SetDateTime("DateLastModified", value);
            }
        }

        [NonSerialized]
        private DateTime? date_last_read = null;
        public DateTime? DateLastRead
        {
            get
            {
                if (date_last_read.HasValue) return date_last_read.Value;

                date_last_read = dictionary.GetDateTime("DateLastRead");
                return date_last_read;
            }
            set
            {
                date_last_read = null;
                dictionary.SetDateTime("DateLastRead", value);
            }
        }

        public DateTime? DateLastCited
        {
            get => dictionary.GetDateTime("DateLastCited");
            set => dictionary.SetDateTime("DateLastCited", value);
        }

        public void MarkAsModified()
        {
            DateLastModified = DateTime.UtcNow;
        }

        public string ReadingStage
        {
            get => dictionary["ReadingStage"] as string;
            set => dictionary["ReadingStage"] = value as string;
        }

        public bool? HaveHardcopy
        {
            get => dictionary["HaveHardcopy"] as bool?;
            set => dictionary["HaveHardcopy"] = value as bool?;
        }

        public bool? IsFavourite
        {
            get => dictionary["IsFavourite"] as bool?;
            set => dictionary["IsFavourite"] = value as bool?;
        }

        public string Rating
        {
            get => dictionary["Rating"] as string;
            set => dictionary["Rating"] = value as string;
        }

        public string Comments
        {
            get => dictionary["Comments"] as string;
            set => dictionary["Comments"] = value as string;
        }

        public string Abstract
        {
            get
            {
                // First check if there is an abstract override
                {
                    string abstract_override = dictionary["AbstractOverride"] as string;
                    if (!String.IsNullOrEmpty(abstract_override))
                    {
                        return abstract_override;
                    }
                }

                // Then check if there is an abstract in the bibtex
                {
                    BibTexItem item = BibTexItem;
                    if (null != item)
                    {
                        string abstract_bibtex = item["abstract"];
                        if (!String.IsNullOrEmpty(abstract_bibtex))
                        {
                            return abstract_bibtex;
                        }
                    }
                }

                // Otherwise try get the abstract from the doc itself
                return PDFAbstractExtraction.GetAbstractForDocument(this);
            }
            set => dictionary["AbstractOverride"] = value as string;
        }

        public string Bookmarks
        {
            get => dictionary["Bookmarks"] as string;
            set => dictionary["Bookmarks"] = value as string;
        }

        public Color Color
        {
            get => dictionary.GetColor("ColorWrapper");
            set => dictionary.SetColor("ColorWrapper", value);
        }

        public int PageLastRead
        {
            get => Convert.ToInt32(dictionary["PageLastRead"] ?? 0);
            set => dictionary["PageLastRead"] = value;
        }

        public bool Deleted
        {
            get => (bool)(dictionary["Deleted"] ?? false);
            set => dictionary["Deleted"] = value;
        }

        #region --- AutoSuggested ------------------------------------------------------------------------------

        public bool AutoSuggested_PDFMetadata
        {
            get => (dictionary["AutoSuggested_PDFMetadata"] as bool?) ?? false;
            set => dictionary["AutoSuggested_PDFMetadata"] = value;
        }

        public bool AutoSuggested_OCRFrontPage
        {
            get => (dictionary["AutoSuggested_OCRFrontPage"] as bool?) ?? false;
            set => dictionary["AutoSuggested_OCRFrontPage"] = value;
        }

        public bool AutoSuggested_BibTeXSearch
        {
            get => (dictionary["AutoSuggested_BibTeXSearch"] as bool?) ?? false;
            set => dictionary["AutoSuggested_BibTeXSearch"] = value;
        }

        #endregion

        #region --- Tags ------------------------------------------------------------------------------

        public bool AddTag(string new_tag_bundle)
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
                return true;
            }
            return false;
        }

        public bool RemoveTag(string dead_tag_bundle)
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
                return true;
            }
            return false;
        }

        public string Tags
        {
            get
            {
                object obj = dictionary["Tags"];

                // Backwards compatibility
                {
                    List<string> tags_list = obj as List<string>;
                    if (null != tags_list)
                    {
                        FeatureTrackingManager.Instance.UseFeature(Features.Legacy_DocumentTagsList);
                        return TagTools.ConvertTagListToTagBundle(tags_list);
                    }
                }

                // Also the bundle version
                {
                    string tags_string = obj as string;
                    if (null != tags_string)
                    {
                        return tags_string;
                    }
                }

                // If we get this far, then there are no tags!  So create some blanks...
                {
                    return "";
                }
            }

            set => dictionary["Tags"] = value;
        }

        #endregion ----------------------------------------------------------------------------------------------------

        public string DocumentBasePath => PDFDocumentFileLocations.DocumentBasePath(library, Fingerprint);

        /// <summary>
        /// The location of the PDF on disk.
        /// </summary>
        public string DocumentPath => PDFDocumentFileLocations.DocumentPath(library, Fingerprint, FileType);

        [NonSerialized]
        private bool? document_exists = null;
        public bool DocumentExists
        {
            get
            {
                if (document_exists.HasValue) return document_exists.Value;

                document_exists = File.Exists(DocumentPath);
                return document_exists.Value;
            }
        }

        [NonSerialized]
        private long document_size = 0;
        public long DocumentSizeInBytes
        {
            get
            {
                // When the document does not exist, the size is reported as ZERO.
                // When we do not know yet whether the document exists, we'll have to go and check and find its size anyhow.
                if (!DocumentExists) return 0;
                if (document_size > 0) return document_size;

                document_size = File.GetSize(DocumentPath);
                return document_size;
            }
        }

        public bool IsVanillaReference => String.Compare(FileType, Constants.VanillaReferenceFileType, StringComparison.OrdinalIgnoreCase) == 0;

        #region --- Annotations / highlights / ink ----------------------------------------------------------------------

        [NonSerialized]
        private PDFAnnotationList annotations = null;

        public PDFAnnotationList GetAnnotations(Dictionary<string, byte[]> library_items_annotations_cache = null)
        {
            if (null == annotations)
            {
                annotations = new PDFAnnotationList();
                PDFAnnotationSerializer.ReadFromDisk(this, ref annotations, library_items_annotations_cache);
#if false
                annotations.OnPDFAnnotationListChanged += annotations_OnPDFAnnotationListChanged;
#endif
            }

            return annotations;
        }

        public string GetAnnotationsAsJSON()
        {
            string json = String.Empty;

            if (null != annotations && annotations.Count > 0)
            {
                // A little hack to make sure the legacies are updated...
                foreach (PDFAnnotation annotation in annotations)
                {
                    annotation.Color = annotation.Color;
                    annotation.DateCreated = annotation.DateCreated;
                    annotation.FollowUpDate = annotation.FollowUpDate;
                }

                List<Dictionary<string, object>> attributes_list = new List<Dictionary<string, object>>();
                foreach (PDFAnnotation annotation in annotations)
                {
                    attributes_list.Add(annotation.Dictionary.Attributes);
                }
                json = JsonConvert.SerializeObject(attributes_list, Formatting.Indented);
            }
            return json;
        }

        //public void QueueToStorage()
        //{
        //    DocumentQueuedStorer.Instance.Queue(this);
        //}

        //void annotations_OnPDFAnnotationListChanged()
        //{
        //    QueueToStorage();
        //    this.library.LibraryIndex.ReIndexDocument(this);
        //}

        [NonSerialized]
        private PDFHightlightList highlights = null;
        public PDFHightlightList Highlights => GetHighlights(null);

        internal PDFHightlightList GetHighlights(Dictionary<string, byte[]> library_items_highlights_cache)
        {
            if (null == highlights)
            {
                highlights = new PDFHightlightList();
                PDFHighlightSerializer.ReadFromStream(this, highlights, library_items_highlights_cache);
#if false
                highlights.OnPDFHighlightListChanged += highlights_OnPDFHighlightListChanged;
#endif
                return highlights;
            }

            return highlights;
        }

        public string GetHighlightsAsJSON()
        {
            string json = String.Empty;

            if (null != highlights && highlights.Count > 0)
            {
                List<PDFHighlight> highlights_list = new List<PDFHighlight>();
                foreach (PDFHighlight highlight in highlights.GetAllHighlights())
                {
                    highlights_list.Add(highlight);
                }

                json = JsonConvert.SerializeObject(highlights_list, Formatting.Indented);

                Logging.Info("Wrote {0} highlights to JSON", highlights_list.Count);
            }
            return json;
        }

        //void highlights_OnPDFHighlightListChanged()
        //{
        //    QueueToStorage();
        //    this.library.LibraryIndex.ReIndexDocument(this);
        //}

        [NonSerialized]
        private PDFInkList inks = null;
        public PDFInkList Inks => GetInks(null);

        internal PDFInkList GetInks(Dictionary<string, byte[]> library_items_inks_cache)
        {
            if (null == inks)
            {
                inks = new PDFInkList();
                PDFInkSerializer.ReadFromDisk(this, inks, library_items_inks_cache);
#if false
                inks.OnPDFInkListChanged += inks_OnPDFInkListChanged;
#endif
            }

            return inks;
        }

        public byte[] GetInksAsJSON()
        {
            byte[] data = null;

            if (null != inks)
            {
                Dictionary<int, byte[]> page_ink_blobs = new Dictionary<int, byte[]>();
                foreach (var pair in inks.PageInkBlobs)
                {
                    page_ink_blobs.Add(pair.Key, pair.Value);
                }

                // We only write to disk if we have at least one page of blobbies to write...
                if (page_ink_blobs.Count > 0)
                {
                    data = SerializeFile.ProtoSaveToByteArray<Dictionary<int, byte[]>>(page_ink_blobs);
                }
            }
            return data;
        }

        #endregion -------------------------------------------------------------------------------------------------

        public void SaveToMetaData()
        {
            // Save the metadata            
            PDFMetadataSerializer.WriteToDisk(this);

            // Save the annotations
            PDFAnnotationSerializer.WriteToDisk(this);

            // Save the highlights
            PDFHighlightSerializer.WriteToDisk(this);

            // Save the inks
            PDFInkSerializer.WriteToDisk(this);
        }

        //void bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    QueueToStorage();
        //    this.library.LibraryIndex.ReIndexDocument(this);
        //}

#if false
        /// <summary>
        /// Throws exception when metadata could not be converted to a valid PDFDocument instance.
        /// </summary>
        /// <param name="library"></param>
        /// <param name="data"></param>
        /// <param name="library_items_annotations_cache"></param>
        /// <returns></returns>
        public static PDFDocument LoadFromMetaData(Library library, byte[] data, Dictionary<string, byte[]> /* can be null */ library_items_annotations_cache)
        {
            DictionaryBasedObject dictionary = PDFMetadataSerializer.ReadFromStream(data);
            PDFDocument pdf_document = new PDFDocument(library, dictionary);
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
            pdf_document.FileType = Path.GetExtension(filename).TrimStart('.');
            pdf_document.Fingerprint = fingerprint;
            pdf_document.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.DocumentBasePath);

            pdf_document.StoreAssociatedPDFInRepository(filename);

            List<LibraryDB.LibraryItem> library_items = library.LibraryDB.GetLibraryItems(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA);
            if (0 == library_items.Count)
            {
                pdf_document.QueueToStorage();
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
                    pdf_document.QueueToStorage();
                    //pdf_document.SaveToMetaData();
                }
            }

            return pdf_document;
        }

        public static PDFDocument CreateFromVanillaReference(Library library)
        {
            PDFDocument pdf_document = new PDFDocument(library);

            // Store the most important information
            pdf_document.FileType = Constants.VanillaReferenceFileType;
            pdf_document.Fingerprint = VanillaReferenceCreating.CreateVanillaReferenceFingerprint();
            pdf_document.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.DocumentBasePath);

            List<LibraryDB.LibraryItem> library_items = library.LibraryDB.GetLibraryItems(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA);
            if (0 == library_items.Count)
            {
                pdf_document.QueueToStorage();
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
                    pdf_document.QueueToStorage();
                }
            }

            return pdf_document;
        }
#endif

        public void CopyMetaData(PDFDocument_ThreadUnsafe pdf_document_template, bool copy_fingerprint = true)
        {
            dictionary["ColorWrapper"] = pdf_document_template.dictionary["ColorWrapper"];
            dictionary["DateAddedToDatabase"] = pdf_document_template.dictionary["DateAddedToDatabase"];
            dictionary["DateLastCited"] = pdf_document_template.dictionary["DateLastCited"];
            dictionary["DateLastModified"] = pdf_document_template.dictionary["DateLastModified"];
            dictionary["DateLastRead"] = pdf_document_template.dictionary["DateLastRead"];
            dictionary["AbstractOverride"] = pdf_document_template.dictionary["AbstractOverride"];
            dictionary["Authors"] = pdf_document_template.dictionary["Authors"];
            dictionary["AuthorsSuggested"] = pdf_document_template.dictionary["AuthorsSuggested"];
            dictionary["AutoSuggested_BibTeXSearch"] = pdf_document_template.dictionary["AutoSuggested_BibTeXSearch"];
            dictionary["AutoSuggested_OCRFrontPage"] = pdf_document_template.dictionary["AutoSuggested_OCRFrontPage"];
            dictionary["AutoSuggested_PDFMetadata"] = pdf_document_template.dictionary["AutoSuggested_PDFMetadata"];
            dictionary["BibTex"] = pdf_document_template.dictionary["BibTex"];
            dictionary["Bookmarks"] = pdf_document_template.dictionary["Bookmarks"];
            dictionary["Comments"] = pdf_document_template.dictionary["Comments"];
            dictionary["Deleted"] = pdf_document_template.dictionary["Deleted"];
            dictionary["DownloadLocation"] = pdf_document_template.dictionary["DownloadLocation"];
            dictionary["FileType"] = pdf_document_template.dictionary["FileType"];
            if (copy_fingerprint)
            {
                dictionary["Fingerprint"] = pdf_document_template.dictionary["Fingerprint"];
            }
            dictionary["HaveHardcopy"] = pdf_document_template.dictionary["HaveHardcopy"];
            dictionary["IsFavourite"] = pdf_document_template.dictionary["IsFavourite"];
            dictionary["PageLastRead"] = pdf_document_template.dictionary["PageLastRead"];
            dictionary["Rating"] = pdf_document_template.dictionary["Rating"];
            dictionary["ReadingStage"] = pdf_document_template.dictionary["ReadingStage"];
            dictionary["Tags"] = pdf_document_template.dictionary["Tags"];
            dictionary["Tags"] = pdf_document_template.dictionary["Tags"];
            dictionary["Title"] = pdf_document_template.dictionary["Title"];
            dictionary["TitleSuggested"] = pdf_document_template.dictionary["TitleSuggested"];
            dictionary["Year"] = pdf_document_template.dictionary["Year"];
            dictionary["YearSuggested"] = pdf_document_template.dictionary["YearSuggested"];

            annotations = (PDFAnnotationList)pdf_document_template.GetAnnotations(null).Clone();
            highlights = (PDFHightlightList)pdf_document_template.Highlights.Clone();
            inks = (PDFInkList)pdf_document_template.Inks.Clone();
        }

        /// <summary>
        /// NB: only call this as part of document creation.
        /// </summary>
        public void CloneMetaData(PDFDocument_ThreadUnsafe existing_pdf_document)
        {
            //bindable = null;

            Logging.Info("Cloning metadata from {0}: {1}", existing_pdf_document.Fingerprint, existing_pdf_document.TitleCombined);

            //dictionary = (DictionaryBasedObject)existing_pdf_document.dictionary.Clone();
            CopyMetaData(existing_pdf_document);

#if false
            // Copy the citations
            PDFDocumentCitationManager.CloneFrom(existing_pdf_document.PDFDocumentCitationManager);

            QueueToStorage();
#endif

#if false
            SaveToMetaData();

            //  Now clear out the references for the annotations and highlights, so that when they are reloaded the events are resubscribed
            annotations = null;
            highlights = null;
            inks = null;
#else
#if false
            annotations.OnPDFAnnotationListChanged += annotations_OnPDFAnnotationListChanged;
            highlights.OnPDFHighlightListChanged += highlights_OnPDFHighlightListChanged;
            inks.OnPDFInkListChanged += inks_OnPDFInkListChanged;
#endif
#endif
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
            foreach (PDFAnnotation pdf_annotation in GetAnnotations(null))
            {
                if (pdf_annotation.Guid == guid)
                {
                    return pdf_annotation;
                }
            }

            return null;
        }

        internal PDFDocument AssociatePDFWithVanillaReference_Part1(string pdf_filename)
        {
            // Only works with vanilla references
            if (!IsVanillaReference)
            {
                throw new Exception("You can only associate a PDF with a vanilla reference.");
            }

            // Create the new PDF document
            PDFDocument new_pdf_document = library.AddNewDocumentToLibrary_SYNCHRONOUS(pdf_filename, pdf_filename, pdf_filename, null, null, null, false, true);

            return new_pdf_document;
        }
    }
}



//=======================================================================================================
//=======================================================================================================
//=======================================================================================================
//=======================================================================================================
//=======================================================================================================



namespace Qiqqa.Documents.PDF
{
    internal class LockObject : object
    {
    }

    public class PDFDocument
    {
        private LockObject access_lock;

        private ThreadUnsafe.PDFDocument_ThreadUnsafe doc;

        public Library Library => doc.Library;

        public string GetAttributesAsJSON()
        {
            lock (access_lock)
            {
                return doc.GetAttributesAsJSON();
            }
        }

        private PDFDocument(LockObject _lock, Library library)
        {
            access_lock = _lock;
            doc = new ThreadUnsafe.PDFDocument_ThreadUnsafe(library);
        }

        private PDFDocument(LockObject _lock, Library library, DictionaryBasedObject dictionary)
        {
            access_lock = _lock;
            doc = new ThreadUnsafe.PDFDocument_ThreadUnsafe(library, dictionary);
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
                        bindable = new AugmentedBindable<PDFDocument>(this, ThreadUnsafe.PDFDocument_ThreadUnsafe.property_dependencies);
                        bindable.PropertyChanged += bindable_PropertyChanged;
                    }

                    return bindable;
                }
            }
        }

        private void bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            QueueToStorage();
            Library.LibraryIndex.ReIndexDocument(this);
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
        /// When setting this value, the first action in this prioirty list is executed, where the conditions pass:
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
                Bindable.NotifyPropertyChanged(() => Tags);
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
                Bindable.NotifyPropertyChanged(() => Tags);
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

        public long DocumentSizeInBytes
        {
            get
            {
                lock (access_lock)
                {
                    return doc.DocumentSizeInBytes;
                }
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

        public PDFAnnotationList GetAnnotations(Dictionary<string, byte[]> library_items_annotations_cache = null)
        {
            PDFAnnotationList annotations;

            lock (access_lock)
            {
                annotations = doc.GetAnnotations(library_items_annotations_cache);
            }

            annotations.OnPDFAnnotationListChanged += annotations_OnPDFAnnotationListChanged;

            return annotations;
        }

        private void annotations_OnPDFAnnotationListChanged()
        {
            QueueToStorage();
            Library.LibraryIndex.ReIndexDocument(this);
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

                highlights.OnPDFHighlightListChanged += highlights_OnPDFHighlightListChanged;
            }

            return highlights;
        }

        private void highlights_OnPDFHighlightListChanged()
        {
            QueueToStorage();
            Library.LibraryIndex.ReIndexDocument(this);
        }

        public string GetHighlightsAsJSON()
        {
            lock (access_lock)
            {
                return doc.GetHighlightsAsJSON();
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

        internal PDFInkList GetInks(Dictionary<string, byte[]> library_items_inks_cache)
        {
            PDFInkList inks;

            lock (access_lock)
            {
                inks = doc.GetInks(library_items_inks_cache);

                inks.OnPDFInkListChanged += inks_OnPDFInkListChanged;
            }

            return inks;
        }

        private void inks_OnPDFInkListChanged()
        {
            Logging.Info("Document has changed inks");
            QueueToStorage();
            Library.LibraryIndex.ReIndexDocument(this);
        }

        public byte[] GetInksAsJSON()
        {
            lock (access_lock)
            {
                return doc.GetInksAsJSON();
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

        public void SaveToMetaData()
        {
            lock (access_lock)
            {
                doc.SaveToMetaData();
            }
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
            DictionaryBasedObject dictionary = PDFMetadataSerializer.ReadFromStream(data);
            LockObject _lock = new LockObject();
            PDFDocument pdf_document = new PDFDocument(_lock, library, dictionary);
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.doc.GetAnnotations(library_items_annotations_cache);
            return pdf_document;
        }

        public static PDFDocument CreateFromPDF(Library library, string filename, string precalculated_fingerprint__can_be_null)
        {
            string fingerprint = precalculated_fingerprint__can_be_null;
            if (String.IsNullOrEmpty(fingerprint))
            {
                fingerprint = StreamFingerprint.FromFile(filename);
            }

            LockObject _lock = new LockObject();
            PDFDocument pdf_document = new PDFDocument(_lock, library);

            // Store the most important information
            //
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.doc.FileType = Path.GetExtension(filename).TrimStart('.');
            pdf_document.doc.Fingerprint = fingerprint;
            pdf_document.doc.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.doc.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.DocumentBasePath);

            pdf_document.doc.StoreAssociatedPDFInRepository(filename);

            List<LibraryDB.LibraryItem> library_items = library.LibraryDB.GetLibraryItems(pdf_document.doc.Fingerprint, PDFDocumentFileLocations.METADATA);
            if (0 == library_items.Count)
            {
                pdf_document.QueueToStorage();
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
                    Logging.Error(ex, "There was a problem reloading an existing PDF from existing metadata, so overwriting it!");

                    // TODO: WARNING: overwriting old (possibly corrupted) records like this can loose you old/corrupted/unsupported metadata content!
                    pdf_document.QueueToStorage();
                    //pdf_document.SaveToMetaData();
                }
            }

            return pdf_document;
        }

        public static PDFDocument CreateFromVanillaReference(Library library)
        {
            LockObject _lock = new LockObject();
            PDFDocument pdf_document = new PDFDocument(_lock, library);

            // Store the most important information
            //
            // thread-UNSAFE access is permitted as the PDF has just been created so there's no thread-safety risk yet.
            pdf_document.FileType = Constants.VanillaReferenceFileType;
            pdf_document.Fingerprint = VanillaReferenceCreating.CreateVanillaReferenceFingerprint();
            pdf_document.DateAddedToDatabase = DateTime.UtcNow;
            pdf_document.DateLastModified = DateTime.UtcNow;

            Directory.CreateDirectory(pdf_document.DocumentBasePath);

            List<LibraryDB.LibraryItem> library_items = library.LibraryDB.GetLibraryItems(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA);
            if (0 == library_items.Count)
            {
                pdf_document.QueueToStorage();
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
                    Logging.Error(ex, "There was a problem reloading an existing PDF from existing metadata, so overwriting it!");

                    // TODO: WARNING: overwriting old (possibly corrupted) records like this can loose you old/corrupted/unsupported metadata content!
                    pdf_document.QueueToStorage();
                }
            }

            return pdf_document;
        }

        public void CopyMetaData(PDFDocument pdf_document_template, bool copy_fingerprint = true)
        {
            // prevent deadlock due to possible incorrect use of this API:
            if (pdf_document_template != this)
            {
                lock (access_lock)
                {
                    doc.CopyMetaData(pdf_document_template.doc, copy_fingerprint);
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
                lock (existing_pdf_document.access_lock)
                {
                    lock (access_lock)
                    {
                        bindable = null;

                        doc.CloneMetaData(existing_pdf_document.doc);

                        doc.GetAnnotations().OnPDFAnnotationListChanged += annotations_OnPDFAnnotationListChanged;
                        doc.Highlights.OnPDFHighlightListChanged += highlights_OnPDFHighlightListChanged;
                        doc.Inks.OnPDFInkListChanged += inks_OnPDFInkListChanged;

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
            PDFDocument new_pdf_document;

            lock (access_lock)
            {
                new_pdf_document = doc.AssociatePDFWithVanillaReference_Part1(pdf_filename);
            }

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
                    new_pdf_document.CopyMetaData(this, copy_fingerprint: false);
#endif
                    new_pdf_document.QueueToStorage();

                    // Delete this one
                    Deleted = true;
                    QueueToStorage();

                    // Tell library to refresh
                    Library.SignalThatDocumentsHaveChanged(this);
                    new_pdf_document.Library.SignalThatDocumentsHaveChanged(new_pdf_document);
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
                doc.Library.PasswordManager.AddPassword(doc, password);
            }
        }

        public void RemovePassword()
        {
            lock (access_lock)
            {
                doc.Library.PasswordManager.RemovePassword(doc);
            }
        }

        public string GetPassword()
        {
            lock (access_lock)
            {
                return doc.Library.PasswordManager.GetPassword(doc);
            }
        }
    }
}
