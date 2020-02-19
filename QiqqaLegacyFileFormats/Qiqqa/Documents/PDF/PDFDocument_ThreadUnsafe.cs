using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Newtonsoft.Json;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Documents.PDF.ThreadUnsafe
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
#if SAMPLE_LOAD_CODE
        [NonSerialized]
        private TypedWeakReference<Library> library;
        public Library Library => null; // => library?.TypedTarget;
#endif

        private DictionaryBasedObject dictionary = new DictionaryBasedObject();

        public string GetAttributesAsJSON()
        {
            string json = JsonConvert.SerializeObject(dictionary.Attributes, Formatting.Indented);
            return json;
        }

        internal PDFDocument_ThreadUnsafe()
        {
        }

        public string Fingerprint
        {
            get => dictionary["Fingerprint"] as string;
            /* protected */
            set => dictionary["Fingerprint"] = value;
        }

#if SAMPLE_LOAD_CODE
        /// <summary>
        /// Unique id for both this document and the library that it exists in.
        /// </summary>
        public string UniqueId => string.Format("{0}_{1}", Fingerprint, Library.WebLibraryDetail.Id);
#endif

        public string FileType
        {
            get => dictionary["FileType"] as string;
            set => dictionary["FileType"] = value.ToLower();
        }

        [NonSerialized]
        private BibTexItem bibtex_item = null;
        [NonSerialized]
        private bool bibtex_item_parsed = false;
        public BibTexItem BibTexItem
        {
            get
            {
                if (!bibtex_item_parsed)
                {
                    //...
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

                //...
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
                return null;
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

#if SAMPLE_LOAD_CODE

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

#endif

#if SAMPLE_LOAD_CODE
        public string DocumentBasePath => PDFDocumentFileLocations.DocumentBasePath(Library, Fingerprint);

        /// <summary>
        /// The location of the PDF on disk.
        /// </summary>
        public string DocumentPath => PDFDocumentFileLocations.DocumentPath(Library, Fingerprint, FileType);
#endif

        public bool IsVanillaReference => String.Compare(FileType, Constants.VanillaReferenceFileType, StringComparison.OrdinalIgnoreCase) == 0;

#if SAMPLE_LOAD_CODE

        [NonSerialized]
        private PDFAnnotationList annotations = null;

        public PDFAnnotationList GetAnnotations(Dictionary<string, byte[]> library_items_annotations_cache = null)
        {
            if (null == annotations)
            {
                annotations = new PDFAnnotationList();
                PDFAnnotationSerializer.ReadFromDisk(this, ref annotations, library_items_annotations_cache);
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

        [NonSerialized]
        private PDFHightlightList highlights = null;
        public PDFHightlightList Highlights => GetHighlights(null);

        internal PDFHightlightList GetHighlights(Dictionary<string, byte[]> library_items_highlights_cache)
        {
            if (null == highlights)
            {
                highlights = new PDFHightlightList();
                PDFHighlightSerializer.ReadFromStream(this, highlights, library_items_highlights_cache);
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

        [NonSerialized]
        private PDFInkList inks = null;
        public PDFInkList Inks => GetInks(null);

        internal PDFInkList GetInks(Dictionary<string, byte[]> library_items_inks_cache)
        {
            if (null == inks)
            {
                inks = new PDFInkList();
                PDFInkSerializer.ReadFromDisk(this, inks, library_items_inks_cache);
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

        public void CopyMetaData(PDFDocument_ThreadUnsafe pdf_document_template, bool copy_fingerprint = true)
        {
            // TODO: do a proper merge, based on flags from the caller about to do and what to pass:
            HashSet<string> keys = new HashSet<string>(dictionary.Keys);
            foreach (var k2 in pdf_document_template.dictionary.Keys)
            {
                keys.Add(k2);
            }
            // now go through the list and see where the clashes are:
            foreach (var k in keys)
            {
                if (null == dictionary[k])
                {
                    // no collision possible: overwriting NULL or empty/non-existing slot, so we're good
                }
                else
                {
                    object o1 = dictionary[k];
                    object o2 = pdf_document_template.dictionary[k];
                    string s1 = o1?.ToString();
                    string s2 = o2?.ToString();
                    string t1 = o1?.GetType().ToString();
                    string t2 = o2?.GetType().ToString();
                    if (s1 == s2 && t1 == t2)
                    {
                        // values match, so no change. We're golden.
                    }
                    else
                    {
                        Logging.Warn("Copying/Moving metadata into {0}: collision on key {1}: old value = ({4})'{2}', new value = ({5})'{3}'", this.Fingerprint, k, s1, s2, t1, t2);

                        // TODO: when this is used for merging metadata anyway...
                        switch (k)
                        {
                            case "DateAddedToDatabase":
                                // take oldest date:
                                break;

                            case "DateLastModified":
                                // take latest, unless the last mod dates match the DateAddedToDatabase records: in that case, use the picked DateAddedToDatabase
                                break;
                        }
                    }
                }
            }

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

#endif

        public override string ToString()
        {
            return Fingerprint;
        }
    }
}
