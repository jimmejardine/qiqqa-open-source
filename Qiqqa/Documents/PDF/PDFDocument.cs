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

    [Obfuscation(Feature = "properties renaming")]
    public class PDFDocument
    {
        private const string VanillaReferenceFileType = "VANILLA_REFERENCE";

        private Library library;
        public Library Library
        {
            get
            {
                return library;
            }
        }

        private DictionaryBasedObject dictionary = new DictionaryBasedObject();

        internal DictionaryBasedObject Dictionary
        {
            get { return dictionary; }
        }

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
            this.library = library;
            this.dictionary = new DictionaryBasedObject();
        }
 
        private PDFDocument(Library library, DictionaryBasedObject dictionary)
        {
            this.library = library;
            this.dictionary = dictionary;
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
        {
            get { return dictionary["Fingerprint"] as string; }
            protected set { dictionary["Fingerprint"] = value; }
        }

        /// <summary>
        /// Unique id for both this document and the library that it exists in.
        /// </summary>
        public string UniqueId
        {
            get { return string.Format("{0}_{1}", Fingerprint, library.WebLibraryDetail.Id); }
        }

        public string FileType
        {
            get { return dictionary["FileType"] as string; }
            set { dictionary["FileType"] = value.ToLower(); }
        }


        private BibTexItem bibtex_item = null;
        public BibTexItem BibTexItem
        {
            get
            {
                if (null == bibtex_item)
                {
                    bibtex_item = BibTexParser.ParseOne(BibTex, true);
                }

                return bibtex_item;
            }
        }
        
        public string BibTex
        {
            get { return dictionary["BibTex"] as string; }
            set
            {
                // Clear the cached item
                bibtex_item = null;

                // Store the new value
                dictionary["BibTex"] = value;

                // If the bibtex contains title, author or year, use those by clearing out any overriding values
                string bibtex = value;
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
                    BibTexItem item = this.BibTexItem;
                    if (null != item)
                    {
                        return item.Key;
                    }
                }
                catch (Exception) { }

                return null;
            }
        }

        public string Title
        {
            get { return dictionary["Title"] as string; }
            set { dictionary["Title"] = value; }
        }
        public string TitleSuggested
        {
            get { return dictionary["TitleSuggested"] as string; }
            set { dictionary["TitleSuggested"] = value; }
        }
        public string TitleCombinedReason
        {
            get
            {
                return String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}\nSource: {4}",
                    TitleCombined,
                    Title,
                    BibTexTools.GetTitle(BibTexItem),
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

                string bibtex = BibTexTools.GetTitle(BibTexItem);
                if (!String.IsNullOrEmpty(bibtex)) return bibtex;

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
            get { return dictionary["Authors"] as string; }
            set { dictionary["Authors"] = value; }
        }
        public string AuthorsSuggested
        {
            get { return dictionary["AuthorsSuggested"] as string; }
            set { dictionary["AuthorsSuggested"] = value; }
        }
        public string AuthorsCombinedReason
        {
            get
            {
                return String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}",
                    AuthorsCombined,
                    Authors,
                    BibTexTools.GetAuthor(BibTexItem),
                    AuthorsSuggested
                    );
            }
        }
        public static readonly string UNKNOWN_AUTHORS = "(unknown authors)";
        public string AuthorsCombined
        {
            get
            {
                if (!String.IsNullOrEmpty(Authors)) return Authors;

                string bibtex = BibTexTools.GetAuthor(BibTexItem);
                if (!String.IsNullOrEmpty(bibtex)) return bibtex;

                if (!String.IsNullOrEmpty(AuthorsSuggested)) return AuthorsSuggested;

                return UNKNOWN_AUTHORS;
            }
            set
            {
                string old_combined = AuthorsCombined;
                if (null != old_combined && 0 == old_combined.CompareTo(value))
                {
                    return;
                }

                // If they are clearing out a value, clear the title
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
            get { return dictionary["Year"] as string; }
            set { dictionary["Year"] = value; }
        }
        public string YearSuggested
        {
            get { return dictionary["YearSuggested"] as string; }
            set { dictionary["YearSuggested"] = value; }
        }
        public string YearCombinedReason
        {
            get
            {
                return String.Format(
                    "Final decision: {0}\n\nYour override: {1}\nBibTeX: {2}\nSuggested: {3}",
                    YearCombined,
                    Year,
                    BibTexTools.GetYear(BibTexItem),
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

                string bibtex = BibTexTools.GetYear(BibTexItem);
                if (!String.IsNullOrEmpty(bibtex)) return bibtex;

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
            get
            {
                return BibTexTools.GetGenericPublication(BibTexItem);
            }

            set
            {
                BibTexItem bibtex_item = this.BibTexItem;
                if (null != bibtex_item)
                {
                    BibTexTools.SetGenericPublication(bibtex_item, value);
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
        {
            get { return dictionary["DownloadLocation"] as string; }
            set { dictionary["DownloadLocation"] = value; }
        }        

        public DateTime? DateAddedToDatabase
        {
            get { return dictionary.GetDateTime("DateAddedToDatabase"); }
            set { dictionary.SetDateTime("DateAddedToDatabase", value); }
        }

        public DateTime? DateLastModified
        {
            get { return dictionary.GetDateTime("DateLastModified"); }
            set { dictionary.SetDateTime("DateLastModified", value); }
        }

        public DateTime? DateLastRead
        {
            get { return dictionary.GetDateTime("DateLastRead"); }
            set { dictionary.SetDateTime("DateLastRead", value); }
        }

        public DateTime? DateLastCited
        {
            get { return dictionary.GetDateTime("DateLastCited"); }
            set { dictionary.SetDateTime("DateLastCited", value); }
        }

        public void MarkAsModified()
        {
            DateLastModified = DateTime.UtcNow;
        }

        public string ReadingStage
        {
            get { return dictionary["ReadingStage"] as string; }
            set { dictionary["ReadingStage"] = value as string; }
        }

        public bool? HaveHardcopy
        {
            get { return dictionary["HaveHardcopy"] as bool?; }
            set { dictionary["HaveHardcopy"] = value as bool?; }
        }

        public bool? IsFavourite
        {
            get { return dictionary["IsFavourite"] as bool?; }
            set { dictionary["IsFavourite"] = value as bool?; }
        }        

        public string Rating
        {
            get { return dictionary["Rating"] as string; }
            set { dictionary["Rating"] = value as string; }
        }

        public string Comments
        {
            get { return dictionary["Comments"] as string; }
            set { dictionary["Comments"] = value as string; }
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
                    BibTexItem item = this.BibTexItem;
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
            set { dictionary["AbstractOverride"] = value as string; }
        }

        public string Bookmarks
        {
            get { return dictionary["Bookmarks"] as string; }
            set { dictionary["Bookmarks"] = value as string; }
        }

        public Color Color
        {
            get { return dictionary.GetColor("ColorWrapper"); }
            set { dictionary.SetColor("ColorWrapper", value); }
        }

        public int PageLastRead        
        {
            get { return Convert.ToInt32(dictionary["PageLastRead"] ?? 0); }
            set { dictionary["PageLastRead"] = value; }
        }

        public bool Deleted        
        {
            get { return (bool)(dictionary["Deleted"] ?? false); }
            set { dictionary["Deleted"] = value; }
        }

        #region --- AutoSuggested ------------------------------------------------------------------------------

        public bool AutoSuggested_PDFMetadata
        {
            get { return (dictionary["AutoSuggested_PDFMetadata"] as bool?) ?? false; }
            set { dictionary["AutoSuggested_PDFMetadata"] = value; }
        }

        public bool AutoSuggested_OCRFrontPage
        {
            get { return (dictionary["AutoSuggested_OCRFrontPage"] as bool?) ?? false; }
            set { dictionary["AutoSuggested_OCRFrontPage"] = value; }
        }

        public bool AutoSuggested_BibTeXSearch
        {
            get { return (dictionary["AutoSuggested_BibTeXSearch"] as bool?) ?? false; }
            set { dictionary["AutoSuggested_BibTeXSearch"] = value; }
        }
    
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

            set 
            {                 
                dictionary["Tags"] = value;
            }
        }

        #endregion ----------------------------------------------------------------------------------------------------

        public string DocumentBasePath
        {
            get
            {
                return PDFDocumentFileLocations.DocumentBasePath(library, Fingerprint);
            }
        }

        /// <summary>
        /// The location of the PDF on disk.
        /// </summary>
        public string DocumentPath
        {
            get
            {
                return PDFDocumentFileLocations.DocumentPath(library, Fingerprint, FileType);
            }
        }

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
                return String.Compare(this.FileType, VanillaReferenceFileType, StringComparison.OrdinalIgnoreCase) == 0;
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
            this.library.LibraryIndex.ReIndexDocument(this);
        }

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
            this.library.LibraryIndex.ReIndexDocument(this);
        }

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
            this.library.LibraryIndex.ReIndexDocument(this);
        }


        #endregion -------------------------------------------------------------------------------------------------

        #region --- Managers ----------------------------------------------------------------------

        PDFDocumentCitationManager _pdf_document_citation_manager = null;
        public PDFDocumentCitationManager PDFDocumentCitationManager
        {
            get
            {
                if (null == _pdf_document_citation_manager) _pdf_document_citation_manager = new PDFDocumentCitationManager(this);
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
            this.library.LibraryIndex.ReIndexDocument(this);
        }

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
                    Logging.Error(ex, "There was a problem reloading an existing PDF from existing metadata, so overwriting it!");
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
            pdf_document.FileType = VanillaReferenceFileType;
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
                    Logging.Error(ex, "There was a problem reloading an existing PDF from existing metadata, so overwriting it!");
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

            Logging.Info("Cloning metadata from {0}", existing_pdf_document.Title);
            dictionary = (DictionaryBasedObject) existing_pdf_document.dictionary.Clone();
            annotations = (PDFAnnotationList) existing_pdf_document.Annotations.Clone();
            highlights = (PDFHightlightList) existing_pdf_document.Highlights.Clone();
            inks = (PDFInkList) existing_pdf_document.Inks.Clone();
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
            if (!File.Exists(DocumentPath))
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
            PDFDocument new_pdf_document = library.AddNewDocumentToLibrary_SYNCHRONOUS(pdf_filename, pdf_filename, null, null, null, false, true);

            // Overwrite the new document's metadata with that of the vanilla reference...
            if (null != new_pdf_document)
            {
                string fingerprint = new_pdf_document.Fingerprint;
                new_pdf_document.dictionary = (DictionaryBasedObject)this.dictionary.Clone();
                new_pdf_document.Fingerprint = fingerprint;
                new_pdf_document.FileType = Path.GetExtension(pdf_filename).TrimStart('.');

                DocumentQueuedStorer.Instance.Queue(new_pdf_document);

                // Delete this one
                this.Deleted = true;
                QueueToStorage();

                // Tell library to refresh
                library.SignalThatDocumentsHaveChanged(new_pdf_document);
            }
            else
            {
                MessageBoxes.Warn("The reference has not been associated with " + pdf_filename);
            }

            return new_pdf_document;
        }
    }
}
