using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Utilities.BibTex.Parsing;
using Utilities.Language;
using Utilities.Strings;

namespace Utilities.BibTex
{
    /// <summary>
    /// Converts a BibTex file to a Word2007 file
    /// </summary>
    public class BibTexToWordConvertor
    {
        private static string NS = "http://schemas.openxmlformats.org/officeDocument/2006/bibliography";
        private static string NS_TAG = "b";

        #region --- Lookups ------------------------------------------------------------------------------------------------------------------------------

        private static Dictionary<string, string> ENTRY_TYPE_MAP = new Dictionary<string, string>()
        {
            { "@article", "JournalArticle" },
            { "@book", "Book" },
            { "@booklet", "BookSection" },
            { "@collection", "ConferenceProceedings" },
            { "@conference", "ConferenceProceedings" },
            { "@inbook", "BookSection" },
            { "@incollection", "BookSection" },
            { "@inproceedings", "ConferenceProceedings" },
            { "@manual", "Report" },
            { "@mastersthesis", "Report" },
            { "@misc", "Misc" },
            { "@patent", "Patent" },
            { "@phdthesis", "Report" },
            { "@proceedings", "ConferenceProceedings" },
            { "@techreport", "Report" },
            { "@zzz", "zzz" },
        };

        private static string TranslateEntryType(string from)
        {
            string to;
            if (!ENTRY_TYPE_MAP.TryGetValue(from.ToLower(), out to))
            {                
                to = "Misc";
                Logging.Warn("Unable to translate unknown entry type {0}, so defaulting to {1}", from, to);
            }
            return to;
       }

        private static Dictionary<string, string> FIELD_TYPE_MAP = new Dictionary<string, string>()
        {
            { "address", "City" },
            { "author", "Author" },
            { "booktitle", "ConferenceName" },
            { "city", "City" },
            { "comment", "Comments" },
            { "doi", "StandardNumber" },
            { "edition", "Edition" },
            { "editor", "Editor" },
            { "ee", "StandardNumber" },
            { "file", "File" },
            { "isbn", "StandardNumber" },
            { "issn", "StandardNumber" },
            { "journal", "JournalName" },
            { "lccn", "StandardNumber" },
            { "misc", "Misc" },
            { "month", "Month" },
            { "note", "Comments" },
            { "number", "Issue" },
            { "organization", "ConferenceName" },
            { "pages", "Pages" },
            { "publisher", "Publisher" },
            { "school", "Institution" },
            { "title", "Title" },
            { "type", "ThesisType" },
            { "volume", "Volume" },
            { "url", "Url" },
            { "year", "Year" },
            { "zzz", "Zzz" }
        };

        private static string TranslateFieldType(string from)
        {
            string to;
            if (!FIELD_TYPE_MAP.TryGetValue(from.ToLower(), out to))
            {
                to = "Misc";
                Logging.Warn("Unable to translate unknown field type {0}, so defaulting to {1}", from, to);
            }
            return to;
        }

        #endregion

        #region --- Public interface ---------------------------------------------------------------------------

        /// <summary>
        /// Converts a batch of bibtex entries into a well-formed XML string
        /// </summary>
        /// <param name="bibtexes"></param>
        /// <returns></returns>
        public static string ConvertBibTexesToXML(string bibtexes)
        {
            XmlDocument doc;
            XmlElement elem_sources;
            ConvertWrapperBibTexToXML(out doc, out elem_sources);

            List<string> bibtex_list = SplitBibTexes(bibtexes);
            foreach (string bibtex in bibtex_list)
            {
                XmlNode node_source = ConvertBibTexToXML(doc, bibtex);
                elem_sources.AppendChild(node_source);
            }

            return XMLTools.ToString(doc);
        }


        /// <summary>
        /// Creates the well-formed <Sources></Sources> wrapper element for a series of bibtex-to-word translations.
        /// Add each of the bibtex-to-word translations to the elem_sources using something like:
        ///     XmlNode node_source = ConvertBibTexToXML(doc, bibtex);
        ///     elem_sources.AppendChild(node_source);
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elem_sources"></param>
        public static void ConvertWrapperBibTexToXML(out XmlDocument doc, out XmlElement elem_sources)
        {
            doc = new XmlDocument();

            // Create the Sources wrapper element
            elem_sources = doc.CreateElement("b:Sources");
            elem_sources.SetAttribute("xmlns", NS);
            elem_sources.SetAttribute("xmlns:" + NS_TAG, NS);
            doc.AppendChild(elem_sources);
        }        
        
        /// <summary>
        /// Converts a single bibtex entry into an XML snippet.
        /// The snippets needs to be wrapped in a well-formed <Sources></Sources> wrapper element.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bibtex"></param>
        /// <returns></returns>
        public static XmlNode ConvertBibTexToXML(XmlDocument doc, BibTexItem bibtex)
        {
            int brace_start = bibtex.IndexOf('{');

            string type_bibtex = bibtex.Substring(0, brace_start);
            string type_word2007 = TranslateEntryType(type_bibtex);
            XmlNode node_source_type = doc.CreateElement(NS_TAG, "SourceType", NS);
            node_source_type.AppendChild(doc.CreateTextNode(type_word2007));

            string tag = bibtex.Substring(brace_start + 1, bibtex.IndexOf(',') - 1 - brace_start);
            XmlNode node_tag = doc.CreateElement(NS_TAG, "Tag", NS);
            node_tag.AppendChild(doc.CreateTextNode(tag));

            XmlNode node_guid = doc.CreateElement(NS_TAG, "Guid", NS);
            node_guid.AppendChild(doc.CreateTextNode(Guid.NewGuid().ToString()));

            XmlNode node_source = doc.CreateElement(NS_TAG, "Source", NS);
            node_source.AppendChild(node_tag);
            node_source.AppendChild(node_source_type);
            node_source.AppendChild(node_guid);

            int fields_start = bibtex.IndexOf(',') + 1;
            int fields_end = bibtex.LastIndexOf('}') - 1;
            string fields = bibtex.Substring(fields_start, fields_end - fields_start + 1);
            ConvertEachBibTexPairIntoWord2007Pair(doc, node_source, fields);

            return node_source;
        }

        #endregion

        #region --- Implementation ------------------------------------------------------------------------


        /// <summary>
        /// Splits the fields at the commas not in nested braces.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static List<string> SplitFields(string fields)
        {
            List<string> field_list = new List<string>();

            // Did we get a useful string?
            if (String.IsNullOrEmpty(fields))
            {
                return field_list;
            }

            // Check if we have only one entry
            int test_index = StringTools.IndexOf_NotInBraces(',', fields);
            if (-1 == test_index)
            {
                field_list.Add(fields);
                return field_list;
            }

            // Keep processing additional entries
            int start_index = 0;
            while (true)
            {
                int end_index = StringTools.IndexOf_NotInBraces(',', fields, start_index + 1);

                // If this is the last one, store it and exit
                if (-1 == end_index)
                {
                    string field = fields.Substring(start_index + 1);
                    field_list.Add(field);
                    break;
                }

                // If there are more, add this one to the list.
                else
                {
                    string field = fields.Substring(start_index + 1, end_index - start_index - 1);
                    field_list.Add(field);
                    start_index = end_index;
                }
            }

            return field_list;
        }

        private static void ConvertEachBibTexPairIntoWord2007Pair(XmlDocument doc, XmlNode node_source, string fields)
        {
            List<string> field_list = SplitFields(fields);

            foreach (string field in field_list)
            {
                string[] field_split = field.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (2 != field_split.Length)
                {
                    Logging.Warn("Unable to process BibTex field '{0}'", field);
                }
                else
                {
                    string key = field_split[0].Trim();
                    string val = field_split[1].Trim();
                    val = val.TrimStart('{').TrimEnd('}');
                    val = BibTexCharacterMap.BibTexToASCII(val);

                    string field_type_word2007 = TranslateFieldType(key);
                    XmlNode node_record = doc.CreateElement(NS_TAG, field_type_word2007, NS);

                    // Process authors specifically
                    if (key.Equals("author"))
                    {
                        XmlNode node_name_list = doc.CreateElement(NS_TAG, "NameList", NS);

                        XmlNode node_author = doc.CreateElement(NS_TAG, "Author", NS);
                        node_author.AppendChild(node_name_list);

                        node_record.AppendChild(node_author);

                        string[] authors = NameTools.SplitAuthors_LEGACY(val);
                        foreach (string author in authors)
                        {
                            string first_name;
                            string last_name;
                            NameTools.SplitName_LEGACY(author, out first_name, out last_name);

                            XmlNode node_last = doc.CreateElement(NS_TAG, "Last", NS);
                            node_last.AppendChild(doc.CreateTextNode(last_name));

                            XmlNode node_first = doc.CreateElement(NS_TAG, "First", NS);
                            node_first.AppendChild(doc.CreateTextNode(first_name));

                            XmlNode node_person = doc.CreateElement(NS_TAG, "Person", NS);
                            node_person.AppendChild(node_last);
                            node_person.AppendChild(node_first);

                            node_name_list.AppendChild(node_person);
                        }
                    }
                                        else
                    {
                        node_record.AppendChild(doc.CreateTextNode(val));
                    }

                    node_source.AppendChild(node_record);
                }
            }
        }

        #endregion


        // ----------------------------------------------------------------------------------------
        // This is obsolete - use the new bibtex parser classes
        // ----------------------------------------------------------------------------------------
        
        /// <summary>
        /// Splits a single string containing a  series of BibTex entries into a series of strings each containing one BibTex entry.
        /// NB: Will not groove if there is an @ anywhere in a BibTex field...
        /// </summary>
        /// <param name="bibtexes"></param>
        /// <returns></returns>
        public static List<string> SplitBibTexes(string bibtexes)
        {
            List<string> bibtex_list = new List<string>();

            // Did we get a useful string?
            if (String.IsNullOrEmpty(bibtexes))
            {
                return bibtex_list;
            }

            // Check that we have at least one entry
            int start_index = bibtexes.IndexOf('@');
            if (-1 == start_index)
            {
                return bibtex_list;
            }

            // Keep processing additional entries
            while (true)
            {
                int end_index = bibtexes.IndexOf('@', start_index + 1);

                // If this is the last one, store it and exit
                if (-1 == end_index)
                {
                    string bibtex = bibtexes.Substring(start_index);
                    bibtex_list.Add(bibtex);
                    break;
                }

                // If there are more, add this one to the list.
                else
                {
                    string bibtex = bibtexes.Substring(start_index, end_index - start_index);
                    bibtex_list.Add(bibtex);
                    start_index = end_index;
                }
            }

            return bibtex_list;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            string bibtexes = File.ReadAllText(@"C:\temp\bibtexsample.bib");
            string word2007 = ConvertBibTexesToXML(bibtexes);
            File.WriteAllText(@"C:\temp\bibtexwordsample.xml", word2007);
        }
#endif

        #endregion
    }
}
