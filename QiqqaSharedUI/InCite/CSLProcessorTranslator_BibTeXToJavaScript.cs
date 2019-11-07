using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.BibTex.Parsing;
using Utilities.Language;

namespace Qiqqa.InCite
{
    internal class CSLProcessorTranslator_BibTeXToJavaScript
    {
        internal static string Translate_INIT(Dictionary<string, CSLProcessorBibTeXFinder.MatchingBibTeXRecord> bibtex_items)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("var CITATION_INIT = [");
            bool is_additional_bibtex_item = false;
            foreach (var pair in bibtex_items)
            {
                if (is_additional_bibtex_item)
                {
                    sb.AppendLine(",");
                }
                else
                {
                    is_additional_bibtex_item = true;
                }

                string id = pair.Key;
                sb.Append("\"" + id + "\"");
            }

            sb.AppendLine();
            sb.AppendLine("];");

            return sb.ToString();
        }

        internal static string Translate_DATABASE(Dictionary<string, CSLProcessorBibTeXFinder.MatchingBibTeXRecord> bibtex_items, Dictionary<string, string> abbreviations)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("var CITATION_DATABASE = {");
            bool is_additional_bibtex_item = false;
            foreach (var pair in bibtex_items)
            {
                if (is_additional_bibtex_item)
                {
                    sb.AppendLine(",");
                }
                else
                {
                    is_additional_bibtex_item = true;
                }

                string id = pair.Key;
                BibTexItem bibtex_item = (null != pair.Value) ? pair.Value.bibtex_item : null;


                // Open the current bibtex item
                sb.AppendLine();
                sb.AppendLine("\"" + id + "\": {");
                sb.AppendLine("  " + MakeQuotedPair("id", id));

                // IF there is no bibtex for this item, make up an error message
                if (null == bibtex_item)
                {
                    string error_message = "MISSING:" + id;
                    sb.AppendLine(", " + MakeQuotedPair("type", "article-journal"));
                    sb.AppendLine(", " + MakeQuotedPair("title", error_message));
                    sb.AppendLine(", " + MakeQuotedPair("author", error_message));
                    sb.AppendLine(", \"issued\": { \"date-parts\": [[\"" + DateTime.UtcNow.Year + "\"]] }");
                }
                else
                {
                    ProcessType(sb, bibtex_item.Type);

                    // Now translate each field in the bibtex
                    foreach (var field_pair in bibtex_item.Fields)
                    {
                        // Don't let these clash with the actual citeproc fields
                        string[] CITEPROC_RESERVED_FIELDS = new string[] { "id", "type" };
                        if (CITEPROC_RESERVED_FIELDS.Contains(field_pair.Key))
                        {
                            Logging.Warn("Skipping the {0} field for the BibTeX record with key {1}", field_pair.Key, bibtex_item.Key);
                            continue;
                        }

                        sb.Append(",");

                        switch (field_pair.Key)
                        {
                            case "author":
                            case "authors":
                                ProcessAuthors(sb, field_pair);
                                break;

                            case "year":
                                ProcessYear(sb, field_pair);
                                break;

                            case "accessed":
                            case "container":
                            case "event-date":
                            case "issued":
                            case "original-date":
                                ProcessGenericDate(sb, field_pair);
                                break;

                            default:
                                ProcessGeneric(sb, field_pair, abbreviations);
                                break;
                        }
                    }
                }

                // Close the current bibtex item
                sb.AppendLine("}");
            }

            sb.AppendLine();
            sb.AppendLine("};");

            return sb.ToString();
        }

        private static void ProcessGeneric(StringBuilder sb, KeyValuePair<string, string> field_pair, Dictionary<string, string> abbreviations)
        {
            // Appends this (for example):
            //     "volume": "18"

            // Map the BibTeX to CSL key
            string key = field_pair.Key;
            key = key.ToLower();

            string translated_key = key;

            switch (key)
            {
                case "journal":
                    translated_key = "container-title";
                    break;

                case "publication":
                    translated_key = "container-title";
                    break;

                case "booktitle":
                    translated_key = "container-title";
                    break;

                case "series":
                    translated_key = "collection-title";
                    break;

                case "series number":
                case "series-number":
                    translated_key = "collection-number";
                    break;

                case "chapter":
                    translated_key = "chapter-number";
                    break;

                case "volume":
                    translated_key = "volume";
                    break;

                case "number":
                    translated_key = "issue";
                    break;

                case "page":
                    translated_key = "page";
                    break;

                case "pages":
                    translated_key = "page";
                    break;

                case "doi":
                    translated_key = "DOI";
                    break;

                case "url":
                    translated_key = "URL";
                    break;

                case "pmid":
                    translated_key = "PMID";
                    break;

                case "isbn":
                    translated_key = "ISBN";
                    break;

                case "issn":
                    translated_key = "ISSN";
                    break;

                case "address":
                    translated_key = "publisher-place";
                    break;

                case "location":
                    translated_key = "publisher-place";
                    break;

                case "journal-iso":
                    translated_key = "journalAbbreviation";
                    break;

                case "editor":
                    translated_key = "editor";
                    break;

                case "keywords":
                    translated_key = "keyword";
                    break;
            }

            // The value field
            string value = field_pair.Value;

            // Replace the -- with - (sometimes for number ranges)
            value = value.Replace("--", "-");

            // Replace the {}s
            value = value.Replace("{", "");
            value = value.Replace("}", "");

            // Abbreviations
            if (0 < abbreviations.Count)
            {
                string value_lowercase = value.ToLower();
                string value_abbreviation = null;
                if (abbreviations.TryGetValue(value_lowercase, out value_abbreviation))
                {
                    Logging.Info("Abbreviating '{0}' to '{1}'", value, value_abbreviation);
                    value = value_abbreviation;
                }
            }

            sb.Append(" ");
            sb.AppendLine(MakeQuotedPair(translated_key, value));
        }

        private static void ProcessYear(StringBuilder sb, KeyValuePair<string, string> field_pair)
        {
            // Appends this:
            //     "issued": { "date-parts": [[<year>]] }

            sb.AppendLine(" \"issued\" : { \"date-parts\" : [[\"" + field_pair.Value + "\"]] }");
        }

        private static void ProcessGenericDate(StringBuilder sb, KeyValuePair<string, string> field_pair)
        {
            sb.AppendLine(" \"" + field_pair.Key + "\" : { \"raw\" : \"" + field_pair.Value + "\" }");
        }
               
        private static void ProcessType(StringBuilder sb, string type)
        {
            // The following are available in CSL
            List<string> CSL_TYPES = new List<string>(new string[] { "article-magazine", "article-newspaper", "article-journal", "bill", "book", "broadcast", "chapter", "entry", "entry-dictionary", "entry-encyclopedia", "figure", "graphic", "interview", "legislation", "legal_case", "manuscript", "map", "motion_picture", "musical_score", "pamphlet", "paper-conference", "patent", "post", "post-weblog", "personal_communication", "report", "review", "review-book", "song", "speech", "thesis", "treaty", "webpage" });

            // Map the type from BibTeX type to CSL type
            type = type.ToLower();

            string translated_type = type;
            
            if (CSL_TYPES.Contains(type)) translated_type = type;

            // BibTeX to CSL - http://www.docear.org/2012/08/08/docear4word-mapping-bibtex-fields-and-types-with-the-citation-style-language/
            else if ("article" == type) translated_type = "article-journal";
            else if ("proceedings" == type) translated_type = "book";
            else if ("manual" == type) translated_type = "book";
            else if ("book" == type) translated_type = "book";
            else if ("periodical" == type) translated_type = "book";
            else if ("booklet" == type) translated_type = "pamphlet";
            else if ("inbook" == type) translated_type = "chapter";
            else if ("incollection" == type) translated_type = "chapter";
            else if ("inproceedings" == type) translated_type = "paper-conference";
            else if ("conference" == type) translated_type = "paper-conference";
            else if ("phdthesis" == type) translated_type = "thesis";
            else if ("mastersthesis" == type) translated_type = "thesis";
            else if ("techreport" == type) translated_type = "report";
            else if ("patent" == type) translated_type = "patent";
            else if ("electronic" == type) translated_type = "webpage";
            else if ("standard" == type) translated_type = "legislation";
            else if ("unpublished" == type) translated_type = "manuscript";

            // Mendeley types - from http://support.mendeley.com/customer/portal/articles/364144-csl-type-mapping
            else if ("journalarticle" == type) translated_type = "article-journal";
            else if ("booksection" == type) translated_type = "chapter";
            else if ("case" == type) translated_type = "legal_case";
            else if ("computerprogram" == type) translated_type = "article";
            else if ("conferenceproceedings" == type) translated_type = "paper-conference";
            else if ("generic" == type) translated_type = "article";
            else if ("encyclopediaarticle" == type) translated_type = "entry-encyclopedia";
            else if ("film" == type) translated_type = "motion_picture";
            else if ("hearing" == type) translated_type = "speech";
            else if ("film" == type) translated_type = "motion_picture";
            else if ("magazinearticle" == type) translated_type = "article-magazine";
            else if ("newspaperarticle" == type) translated_type = "article-newspaper";
            else if ("statute" == type) translated_type = "legislation";
            else if ("televisionbroadcast" == type) translated_type = "broadcast";
            else if ("workingpaper" == type) translated_type = "report";

            else if ("article-csl" == type) translated_type = "article";
            else translated_type = "article-journal";

            sb.AppendLine(", " + MakeQuotedPair("type", translated_type));
        }

        private static void ProcessAuthors(StringBuilder sb, KeyValuePair<string, string> field_pair)
        {
            // Appends this
            /*
                    "author": [
			            {
				            "family": ""Zelle"",
				            "given": "Rintze M."
			            },
			            {
				            "family": ""Hulster"",
				            "given": "Erik",
				            "non-dropping-particle":"de"        <--- we dont do this - we double quote the "family" instead
			            },
			            {
				            "family": ""Kloezen"",
				            "given": "Wendy"
			            },
			            {
				            "family":""Pronk"",
				            "given":"Jack T."
			            },
			            {
				            "family": ""Maris"",
				            "given":"Antonius J. A.",            <---- note that initials must be separated with a space...
				            "non-dropping-particle":"van"       <--- we dont do this - we double quote the "family" instead
			            }
		            ]
             */

            sb.AppendLine(" \"author\": [");

            string value = field_pair.Value;

            // Replace the {}s
            value = value.Replace("{", "");
            value = value.Replace("}", "");

            string[] authors_split = NameTools.SplitAuthors_LEGACY(value);
            bool is_additional_author_item = false;
            foreach (string author_split in authors_split)
            {
                if (is_additional_author_item)
                {
                    sb.Append(",");
                }
                else
                {
                    sb.Append(" ");
                    is_additional_author_item = true;
                }

                sb.Append("  { ");
                NameTools.Name name = NameTools.SplitName(author_split);
                sb.Append(MakeQuotedPair("family", name.last_name));

                // Make sure the initials have a space between them
                string first_names_with_initials_separated = name.first_names;
                if (!String.IsNullOrEmpty(first_names_with_initials_separated))
                {
                    first_names_with_initials_separated = first_names_with_initials_separated.Replace(".", ". ");
                    first_names_with_initials_separated = first_names_with_initials_separated.Trim();
                }

                sb.Append(", " + MakeQuotedPair("given", first_names_with_initials_separated));
                sb.Append("} ");
                sb.AppendLine();
            }

            sb.AppendLine("   ]");
        }

        private static string MakeQuotedPair(string key, string value)
        {
            value = value.Replace("\\", "\\\\");
            value = value.Replace("\"", "\\\"");
            value = value.Replace("\n", " ");
            value = value.Replace("\r", " ");
            return "\"" + key + "\"" + " : " + "\"" + value + "\"";
        }

        /*
         * Creates this:
         * 
            var CITATION_DATABASE = {
	            "ITEM-1": {
		            "id": "ITEM-1",
		            "title":"Boundaries of Dissent: Protest and State Power in the Media Age",
		            "author": [
			            {
				            "family": "De_Arcus",
				            "given": "Bruce",
				            "static-ordering": false
			            }
		            ],
                    "note":"The apostrophe in Bruce's name appears in proper typeset form.",
		            "publisher": "Routledge",
                    "publisher-place": "New York",
		            "issued": {
			            "date-parts":[
				            [2006]
			            ]
		            },
		            "type": "book"
	            },
	            "ITEM-2": {
		            "id": "ITEM-2",
		            "author": [
			            {
			                "family": "Bennett",
				            "given": "Frank G.",
				            "non-dropping-particle": "von",
				            "suffix": "Jr.",
				            "comma-suffix": true,
				            "static-ordering": false
			            }
		            ],
		            "title":"Getting Property Right: \"Informal\" Mortgages in the Japanese Courts",
		            "container-title":"Pacific Rim Law & Policy Journal",
		            "volume": "18",
		            "page": "463-509",
		            "issued": {
			            "date-parts":[
				            [2009, 8]
			            ]
		            },
		            "type": "article-journal",
                    "note": "Note the flip-flop behavior of the quotations marks around \"informal\" in the title of this citation.  This works for quotation marks in any style locale.  Oh, and, uh, these notes illustrate the formatting of annotated bibliographies (!)."
	            },
	            "ITEM-3": {
		            "id": "ITEM-3",
		            "title":"Key Process Conditions for Production of C<sub>4</sub> Dicarboxylic Acids in Bioreactor Batch Cultures of an Engineered <i>Saccharomyces cerevisiae</i> Strain",
                    "note":"This cite illustrates the rich text formatting capabilities in the new processor, as well as page range collapsing (in this case, applying the collapsing method required by the Chicago Manual of Style).  Also, as the IEEE example above partially illustrates, we also offer robust handling of particles such as \"van\" and \"de\" in author names.",
		            "author": [
			            {
				            "family": "Zelle",
				            "given": "Rintze M."
			            },
			            {
				            "family": "Hulster",
				            "given": "Erik",
				            "non-dropping-particle":"de"
			            },
			            {
				            "family": "Kloezen",
				            "given": "Wendy"
			            },
			            {
				            "family":"Pronk",
				            "given":"Jack T."
			            },
			            {
				            "family": "Maris",
				            "given":"Antonius J.A.",
				            "non-dropping-particle":"van"
			            }
		            ],
		            "container-title": "Applied and Environmental Microbiology",
		            "issued":{
			            "date-parts":[
				            [2010, 2]
			            ]
		            },
		            "page": "744-750",
		            "volume":"76",
		            "issue": "3",
		            "DOI":"10.1128/AEM.02396-09",
		            "type": "article-journal"
	            },
	            "ITEM-4": {
		            "id": "ITEM-4",
		            "author": [
			            {
				            "family": "Razlogova",
				            "given": "Elena"
			            }
		            ],
		            "title": "Radio and Astonishment: The Emergence of Radio Sound, 1920-1926",
		            "type": "speech",
		            "event": "Society for Cinema Studies Annual Meeting",
		            "event-place": "Denver, CO",
                    "note":"All styles in the CSL repository are supported by the new processor, including the popular Chicago styles by Elena.",
		            "issued": {
			            "date-parts": [
				            [
					            2002,
					            5
				            ]
			            ]
		            }
	            },
	            "ITEM-5": {
		            "id": "ITEM-5",
		            "author": [
			            {
				            "family": "\u68b6\u7530",
				            "given": "\u5c06\u53f8",
				            "multi":{
					            "_key":{
						            "ja-alalc97":{
								            "family": "Kajita",
								            "given": "Shoji"
						            }
					            }
				            }				
			            },
			            {
				            "family": "\u89d2\u6240",
				            "given": "\u8003",
				            "multi":{
					            "_key":{
						            "ja-alalc97":{
							            "family": "Kakusho",
							            "given": "Takashi"
						            }
					            }
				            }				
			            },
			            {
				            "family": "\u4e2d\u6fa4",
				            "given": "\u7be4\u5fd7",
				            "multi":{
					            "_key":{
						            "ja-alalc97":{
							            "family": "Nakazawa",
							            "given": "Atsushi"
						            }
					            }
				            }				
			            },
			            {
				            "family": "\u7af9\u6751",
				            "given": "\u6cbb\u96c4",
				            "multi":{
					            "_key":{
						            "ja-alalc97":{
							            "family": "Takemura",
							            "given": "Haruo"
						            }
					            }
				            }				
			            },
			            {
				            "family": "\u7f8e\u6fc3",
				            "given": "\u5c0e\u5f66",
				            "multi":{
					            "_key":{
						            "ja-alalc97":{
							            "family": "Mino",
							            "given": "Michihiko"
						            }
					            }
				            }				
			            },
			            {
				            "family": "\u9593\u702c",
				            "given": "\u5065\u4e8c",
				            "multi":{
					            "_key":{
						            "ja-alalc97":{
							            "family": "Mase",
							            "given": "Kenji"
						            }
					            }
				            }				
			            }
		            ],
		            "title": "\u9ad8\u7b49\u6559\u80b2\u6a5f\u95a2\u306b\u304a\u3051\u308b\u6b21\u4e16\u4ee3\u6559\u80b2\u5b66\u7fd2\u652f\u63f4\u30d7\u30e9\u30c3\u30c8\u30d5\u30a9\u30fc\u30e0\u306e\u69cb\u7bc9\u306b\u5411\u3051\u3066",
		            "multi":{
			            "_keys":{
				            "title":{
					            "ja-alalc97": "K\u014dt\u014d ky\u014diku ni okeru jisedai ky\u014diku gakush\u016b shien puratto f\u014dmu no k\u014dchiku ni mukete",
					            "en": "Toward the Development of Next-Generation Platforms for Teaching and Learning in Higher Education"
				            },
				            "container-title":{
					            "ja-alalc97": "Nihon ky\u014diku k\u014dgaku ronbunshi",
					            "en": "Journal of the Japan Educational Engineering Society"
				            }
			            }
		            },
		            "container-title": "\u65e5\u672c\u6559\u80b2\u5de5\u5b66\u4f1a\u8ad6\u6587\u8a8c",
		            "volume": "31",
		            "issue": "3",
		            "page": "297-305",
		            "issued": {
			            "date-parts": [
				            [
					            2007,
					            12
				            ]
			            ]
		            },
                    "note": "Note the transformations to which this cite is subjected in the samples above, and the fact that it appears in the correct sort position in all rendered forms.  Selection of multi-lingual content can be configured in the style, permitting one database to serve a multi-lingual author in all languages in which she might publish.",
		            "type": "article-journal"

	            },
	            "ITEM-6": {
		            "id": "ITEM-6",
		            "title":"Evaluating Components of International Migration: Consistency of 2000 Nativity Data",
		            "note": "This cite illustrates the formatting of institutional authors.  Note that there is no \"and\" between the individual author and the institution with which he is affiliated.",
		            "author": [
			            {
				            "family": "Malone",
				            "given": "Nolan J.",
				            "static-ordering": false
			            },
			            {
				            "literal": "U.S. Bureau of the Census"
			            }
		            ],
		            "publisher": "Routledge",
                    "publisher-place": "New York",
		            "issued": {
			            "date-parts":[
				            [2001, 12, 5]
			            ]
		            },
		            "type": "book"
	            },
	            "ITEM-7": {
		            "id": "ITEM-7",
		            "title": "True Crime Radio and Listener Disenchantment with Network Broadcasting, 1935-1946",
		            "author":[
			            {
				            "family": "Razlogova",
				            "given": "Elena"
			            }
		            ],
		            "container-title": "American Quarterly",
		            "volume": "58",
		            "page": "137-158",
		            "issued": {
			            "date-parts": [
				            [2006, 3]
			            ]
		            },
		            "type": "article-journal"
	            },
	            "ITEM-8": {
		            "id": "ITEM-8",
		            "title": "The Guantanamobile Project",
		            "container-title": "Vectors",
		            "volume": "1",
		            "author":[
			            {
				            "family": "Razlogova",
				            "given": "Elena"
			            },
			            {
				            "family": "Lynch",
				            "given": "Lisa"
			            }
		            ],
		            "issued": {
			            "season": 3,
			            "date-parts": [
				            [2005]
			            ]
		            },
		            "type": "article-journal"

	            },
	            "ITEM-9": {
		            "id": "ITEM-9",
		            "container-title": "FEMS Yeast Research",
		            "volume": "9",
		            "issue": "8",
		            "page": "1123-1136",
		            "title": "Metabolic engineering of <i>Saccharomyces cerevisiae</i> for production of carboxylic acids: current status and challenges",
		            "contributor":[
			            {
				            "family": "Zelle",
				            "given": "Rintze M."
			            }
		            ],
		            "author": [
			            {
				            "family": "Abbott",
				            "given": "Derek A."
			            },
			            {
				            "family": "Zelle",
				            "given": "Rintze M."
			            },
			            {
				            "family":"Pronk",
				            "given":"Jack T."
			            },
			            {
				            "family": "Maris",
				            "given":"Antonius J.A.",
				            "non-dropping-particle":"van"
			            }
		            ],
		            "issued": {
			            "season": "2",
			            "date-parts": [
				            [
					            2009,
					            6,
					            6
				            ]
			            ]
		            },
		            "type": "article-journal"
	            },
                "ITEM-10": {
                    "container-title": "N.Y.2d", 
                    "id": "ITEM-10", 
                    "issued": {
                        "date-parts": [
                            [
                                "1989"
                            ]
                        ]
                    }, 
                    "page": "683", 
                    "title": "People v. Taylor", 
                    "type": "legal_case", 
                    "volume": 73
                }, 
                "ITEM-11": {
                    "container-title": "N.E.2d", 
                    "id": "ITEM-11", 
                    "issued": {
                        "date-parts": [
                            [
                                "1989"
                            ]
                        ]
                    }, 
                    "page": "386", 
                    "title": "People v. Taylor", 
                    "type": "legal_case", 
                    "volume": 541
                }, 
                "ITEM-12": {
                    "container-title": "N.Y.S.2d", 
                    "id": "ITEM-12", 
                    "issued": {
                        "date-parts": [
                            [
                                "1989"
                            ]
                        ]
                    }, 
                    "page": "357", 
                    "title": "People v. Taylor", 
                    "type": "legal_case", 
                    "volume": 543
                },
                "ITEM-13": {
                    "id": "ITEM-13", 
                    "title": "\u6c11\u6cd5",
		            "multi":{
			            "_keys":{
				            "title": {
					            "ja-alalc97": "Minp\u014d",
					            "en": "Japanese Civil Code"
				            }
			            }
		            },
                    "type": "legislation"
                },
                "ITEM-14": {
                    "id": "ITEM-14", 
                    "title": "Clayton Act",
                    "container-title": "ch.",
                    "number": 323,
		            "issued": {
                       "date-parts": [
                         [
                            1914
                         ]
                       ]
		            },
                    "type": "legislation"
                },
                "ITEM-15": {
                    "id": "ITEM-15", 
                    "title": "Clayton Act",
		            "volume":38,
                    "container-title": "Stat.",
                    "page": 730,
		            "issued": {
                       "date-parts": [
                         [
                            1914
                         ]
                       ]
		            },
                    "type": "legislation"
                },
                "ITEM-16": {
                    "id": "ITEM-16", 
                    "title": "FTC Credit Practices Rule",
		            "volume":16,
                    "container-title": "C.F.R.",
                    "section": 444,
		            "issued": {
                       "date-parts": [
                         [
                            1999
                         ]
                       ]
		            },
                    "type": "legislation"
                },
                "ITEM-17": {
                    "id": "ITEM-17", 
                    "title": "Beck v. Beck",
		            "volume":1999,
                    "container-title": "ME",
                    "page": 110,
		            "issued": {
                       "date-parts": [
                         [
                            1999
                         ]
                       ]
		            },
                    "type": "legal_case"
                },
                "ITEM-18": {
                    "id": "ITEM-18", 
                    "title": "Beck v. Beck",
		            "volume":733,
                    "container-title": "A.2d",
                    "page": 981,
		            "issued": {
                       "date-parts": [
                         [
                            1999
                         ]
                       ]
		            },
                    "type": "legal_case"
                },
                "ITEM-19": {
                    "id": "ITEM-19", 
                    "title": "Donoghue v. Stevenson",
		            "volume":1932,
                    "container-title": "App. Cas.",
                    "page": 562,
		            "issued": {
                       "date-parts": [
                         [
                            1932
                         ]
                       ]
		            },
                    "type": "legal_case"
                },
                "ITEM-20": {
                    "id": "ITEM-20", 
                    "title": "British Columbia Elec. Ry. v. Loach",
		            "volume":1916,
		            "issue":1,
                    "container-title": "App. Cas.",
                    "page": 719,
		            "authority":"P.C.",
		            "issued": {
                       "date-parts": [
                         [
                            1915
                         ]
                       ]
		            },
                    "type": "legal_case"
                },
                "ITEM-21": {
                    "id": "ITEM-21", 
                    "title": "Chapters on Chaucer",
		            "author":[
			            {
				            "family": "Malone",
				            "given": "Kemp"
			            }
		            ],
                    "publisher":"Johns Hopkins Press",
                    "publisher-place": "Baltimore",
		            "issued": {
                       "date-parts": [
                         [
                            1951
                         ]
                       ]
		            },
                    "type": "book"
                }
            };

         */
    }
}
