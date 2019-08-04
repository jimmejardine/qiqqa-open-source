using System.Collections.Generic;

namespace Utilities.BibTex
{
    public class EntryTypes
    {
        public static EntryTypes Instance = new EntryTypes();

        Dictionary<string, EntryType> entry_types = new Dictionary<string, EntryType>();

        // Populated from http://www.kfunigraz.ac.at/~binder/texhelp/bibtx-7.html (not active anymore in 2019 A.D.)
        // Also at http://nwalsh.com/tex/texhelp/bibtx-7.html
        //
        // Other potential sources:
        // - http://bib-it.sourceforge.net/help/fieldsAndEntryTypes.php
        // - https://www.bibtex.com/e/book-entry/
        // - https://en.wikipedia.org/wiki/BibTeX#Style_files
        //
        private EntryTypes()
        {
            AddEntryType(
            new EntryType(
            "article",
            new string[] { "author", "title", "journal", "year" },
            new string[] { "volume", "number", "pages", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "book",
            new string[] { "author", "editor", "title", "publisher", "year" },
            new string[] { "volume", "series", "address", "edition", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "booklet",
            new string[] { "title" },
            new string[] { "author", "howpublished", "address", "year", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "conference",
            new string[] { "author", "title", "booktitle", "year" },
            new string[] { "editor", "pages", "organization", "publisher", "address", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "inbook",
            new string[] { "author", "title", "booktitle", "chapter", "year" },
            new string[] { "editor", "pages", "organization", "publisher", "address", "month", "note", "volume", "series" }
            ));

            AddEntryType(
            new EntryType(
            "chapter",
            new string[] { "author", "title", "chapter", "year" },
            new string[] { "editor", "pages", "organization", "publisher", "address", "month", "note", "volume", "series" }
            ));

            AddEntryType(
            new EntryType(
            "incollection",
            new string[] { "author", "title", "booktitle", "year" },
            new string[] { "editor", "pages", "organization", "publisher", "address", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "inproceedings",
            new string[] { "author", "title", "booktitle", "year" },
            new string[] { "editor", "pages", "organization", "publisher", "address", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "manual",
            new string[] { "title" },
            new string[] { "author", "organization", "address", "edition", "year", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "mastersthesis",
            new string[] { "author", "title", "school", "year" },
            new string[] { "address", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "misc",
            new string[] { },
            new string[] { "title", "author", "year", "DOI", "PMID" }
            ));

            AddEntryType(
            new EntryType(
            "phdthesis",
            new string[] { "author", "title", "school", "year" },
            new string[] { "address", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "proceedings",
            new string[] { "title", "year" },
            new string[] { "editor", "publisher", "organization", "address", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "techreport",
            new string[] { "author", "title", "institution", "year" },
            new string[] { "type", "number", "address", "month", "note" }
            ));

            AddEntryType(
            new EntryType(
            "unpublished",
            new string[] { "author", "title", "note" },
            new string[] { "year", "month" }
            ));

            AddEntryType(
            new EntryType(
            "webpage",
            new string[] { "title", "URL" },
            new string[] { "accessed" }
            ));

            AddEntryType(
            new EntryType(
            "legal_case",
            new string[] { "title", "volume", "page", "locator", "container-title", "year" },
            new string[] {  }
            ));


        }

        // Dont forget to add these CSL types
        // "article", "article-magazine", "article-newspaper", "article-journal", "bill", "book", "broadcast", "chapter", "entry", "entry-dictionary", "entry-encyclopedia", "figure", "graphic", "interview", "legislation", "legal_case", "manuscript", "map", "motion_picture", "musical_score", "pamphlet", "paper-conference", "patent", "post", "post-weblog", "personal_communication", "report", "review", "review-book", "song", "speech", "thesis", "treaty", "webpage"

        private List<string> entry_type_list = null;
        public List<string> EntryTypeList
        {
            get
            {
                if (null == entry_type_list)
                {
                    entry_type_list = new List<string>();
                    foreach (string key in entry_types.Keys)
                    {
                        entry_type_list.Add(key);
                    }
                    entry_type_list.Sort();
                }

                return entry_type_list;
            }
        }

        private List<string> field_type_list = null;
        public List<string> FieldTypeList
        {
            get
            {
                if (null == field_type_list)
                {
                    // Get the unique field names
                    HashSet<string> fields = new HashSet<string>();
                    foreach (EntryType entry_type in entry_types.Values)
                    {
                        fields.UnionWith(entry_type.requireds);
                        fields.UnionWith(entry_type.optionals);
                    }

                    // Create the sorted list
                    field_type_list = new List<string>(fields);
                    field_type_list.Sort();
                }

                return field_type_list;
            }
        }

        private void AddEntryType(EntryType entryType)
        {
            entry_types[entryType.type] = entryType;
        }

        /// <summary>
        /// Returns the EntryType record for the supplied type.  Returns misc if the supplied type is not recognised...
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public EntryType GetEntryType(string type)
        {
            if (null == type)
            {
                return entry_types["misc"];
            }

            type = type.ToLower();
            if (entry_types.ContainsKey(type))
            {
                return entry_types[type];
            }
            else
            {
                return entry_types["misc"];
            }            
        }
    }
}
