using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.BibTex
{
    public class EntryTypes
    {
        // singleton; follow the idiom from https://stackoverflow.com/questions/5897681/unit-testing-singletons
        public static EntryTypes _Instance = null;
        public static EntryTypes Instance
        {
            get
            {
                if (null == _Instance)
                {
                    _Instance = new EntryTypes();
                }
                return _Instance;
            }
        }
        public static void ResetForTesting()
        {
            _Instance = null;
        }

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
            string filename = EntryTypes.EntryTypesDefinitionFilename;

            // when the setup file does not exist, yak!
            if (!File.Exists(filename))
            {
                Logging.Error("BibTeX entry types definition JSON file '{0}' does not exist! Your install may be boogered!", filename);

                // do it old skool style. Only a few so it's easily noticed by the user!
                AddEntryType(
                new EntryType(
                "book",
                new string[] { "author", "editor", "title", "publisher", "year" },
                new string[] { "volume", "series", "address", "edition", "month", "note" }
                ));

                AddEntryType(
                new EntryType(
                "misc",
                null,
                new string[] { "title", "author", "year", "DOI", "PMID" }
                ));

#if false && TEST
                string json = JsonConvert.SerializeObject(entry_types, Formatting.Indented);
                File.WriteAllText(filename, json);
#endif
            }
            else
            {
                // file exists; load it to get all the goodness.
                try
                {
                    string json = File.ReadAllText(filename, System.Text.Encoding.UTF8);
                    entry_types = JsonConvert.DeserializeObject<Dictionary<string, EntryType>>(json);
                    Logging.Info("Loaded {0} BibTeX entry types from configuration file '{1}':\n    [{2}]", entry_types.Count, filename, String.Join(", ", entry_types.Keys));
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Failed to load/parse the BibTeX entry types definition file '{0}'.", filename);
                }
            }

            // see if the 'common denominator' entry has been defined in there. 
            // IFF it's not, we add it on the spot and barge on.
            if (!entry_types.ContainsKey("misc"))
            {
                AddEntryType(
                new EntryType(
                "misc",
                null,
                new string[] { "title", "author", "year", "DOI", "PMID" }
                ));
            }
        }

        // Dont forget to add these CSL types:
        //
        // "article", "article-magazine", "article-newspaper", "article-journal", "bill", 
        // "book", "broadcast", "chapter", "entry", "entry-dictionary", "entry-encyclopedia", 
        // "figure", "graphic", "interview", "legislation", "legal_case", "manuscript", "map", 
        // "motion_picture", "musical_score", "pamphlet", "paper-conference", "patent", "post", 
        // "post-weblog", "personal_communication", "report", "review", "review-book", "song", 
        // "speech", "thesis", "treaty", "webpage"
        // + 
        // "datasheet", "application-note"


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

        private static string EntryTypesDefinitionFilename
        {
            get
            {
                return Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"BibTeX/qiqqa-entry-type-definitions.json"));
            }
        }
    }
}
