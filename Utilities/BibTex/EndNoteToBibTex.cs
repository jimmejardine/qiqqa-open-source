using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utilities.BibTex.Parsing;
using Utilities.Strings;

#if TEST
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using static QiqqaTestHelpers.MiscTestHelpers;
using Newtonsoft.Json;
using Utilities;

using Utilities.BibTex;
#endif


#if !TEST

namespace Utilities.BibTex
{
    /// <summary>
    /// Convert EndNote records to BibTeX.
    /// See the bottom of this file for the field mappings...
    /// </summary>
    public class EndNoteToBibTex
    {
        public class EndNoteRecord
        {
            public Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();
            public List<String> errors = new List<string>();

            public BibTexItem ToBibTeX()
            {
                BibTexItem bibtex_item = new BibTexItem();

                bibtex_item.Key = GenerateReasonableKey();

                // Process the type
                {
                    string endnote_type = "";
                    if (attributes.ContainsKey("%0"))
                    {
                        endnote_type = attributes["%0"][0];
                    }
                    bibtex_item.Type = MapEndNoteToBibTeXType(endnote_type);
                }

                // Process the generic fields -* note that we will silently throw away all "multiple" items with this field name
                foreach (var attribute_pair in attributes)
                {
                    string bibtex_field_type = MapEndNoteToBibTeXFieldType(attribute_pair.Key);
                    if (!String.IsNullOrEmpty(bibtex_field_type))
                    {
                        bibtex_item[bibtex_field_type] = attribute_pair.Value[0];
                    }
                }

                // Create the authors
                if (attributes.ContainsKey("%A"))
                {
                    StringBuilder sb = new StringBuilder();

                    List<string> author_list = attributes["%A"];
                    foreach (string author in author_list)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(" and ");
                        }

                        sb.Append(author);
                    }

                    string authors = sb.ToString();
                    if (!String.IsNullOrEmpty(authors))
                    {
                        bibtex_item["author"] = authors;
                    }
                }


                // Create the keywords
                if (attributes.ContainsKey("%K"))
                {
                    string keywords = (attributes["%K"][0]).Replace("\n", ",");
                    bibtex_item["keywords"] = keywords;
                }

                return bibtex_item;
            }

            private string GenerateReasonableKey()
            {
                StringBuilder sb = new StringBuilder();

                // Author
                if (attributes.ContainsKey("%A"))
                {
                    sb.Append(attributes["%A"][0].Split(' ')[0]);
                }

                // Year
                if (attributes.ContainsKey("%D"))
                {
                    sb.Append(attributes["%D"][0].Split(' ')[0]);
                }

                // Title - 3 words
                if (attributes.ContainsKey("%T"))
                {
                    string[] titles = attributes["%T"][0].Split(' ');
                    for (int i = 0; i < 3 && i < titles.Length; ++i)
                    {
                        sb.Append(titles[i]);
                    }
                }

                // Polish
                string key = sb.ToString();
                key = key.ToLower();
                key = StringTools.StripToASCII(key);

                return key;
            }

            private static string MapEndNoteToBibTeXType(string type)
            {
                type = type.Trim().ToLower();

                if (type == "journal article") return "article";
                if (type == "book") return "book";
                if (type == "book section") return "inbook";
                if (type == "conference paper") return "inproceedings";
                if (type == "generic") return "misc";
                if (type == "thesis") return "phdthesis";
                if (type == "conference proceedings") return "proceedings";
                if (type == "report") return "techreport";
                if (type == "unpublished work") return "unpublished";

                // Default to this
                return "article";
            }

            private static string MapEndNoteToBibTeXFieldType(string field_type)
            {
                //if (field_type == "%B") return "booktitle";
                if (field_type == "%B") return "journal"; // 20120103 - it seems that the journal name is put into a %B field.  Lets see how many people complain about this change...                

                if (field_type == "%C") return "address";
                if (field_type == "%D") return "year";
                if (field_type == "%E") return "editor";
                if (field_type == "%I") return "publisher";
                if (field_type == "%J") return "journal";
                if (field_type == "%N") return "number";
                if (field_type == "%P") return "pages";
                if (field_type == "%T") return "title";
                if (field_type == "%U") return "url";
                if (field_type == "%V") return "volume";
                if (field_type == "%X") return "abstract";
                if (field_type == "%Z") return "note";
                if (field_type == "7%") return "edition";
                if (field_type == "%&") return "chapter";

                return "misc";
            }
        }

        public static List<EndNoteRecord> Parse(string endnote_text)
        {
            List<EndNoteRecord> endnote_records = new List<EndNoteRecord>();

            List<List<string>> endnote_record_texts = SplitMultipleEndNoteLines(endnote_text);
            foreach (List<string> endnote_record_text in endnote_record_texts)
            {
                EndNoteRecord endnote_record = MapEndNoteLinesToDictionary(endnote_record_text);
                endnote_records.Add(endnote_record);
            }

            return endnote_records;
        }

        protected static EndNoteRecord MapEndNoteLinesToDictionary(List<string> lines)
        {
            EndNoteRecord endnote_record = new EndNoteRecord();

            string last_attribute = null;

            for (int i = 0; i < lines.Count; ++i)
            {
                string line = lines[i];

                try
                {
                    // If it's a new attribute
                    if (line.StartsWith("%"))
                    {
                        string attribute = line.Substring(0, 2).ToUpper();
                        string remainder = line.Substring(3);

                        if (!endnote_record.attributes.ContainsKey(attribute))
                        {
                            endnote_record.attributes[attribute] = new List<string>();
                        }
                        endnote_record.attributes[attribute].Add(remainder);

                        last_attribute = attribute;
                        continue;
                    }

                    // Check that the rest are not just blanks
                    if (String.IsNullOrEmpty(line))
                    {
                        bool have_some_non_blanks = false;
                        for (int j = i + 1; j < lines.Count; ++j)
                        {
                            if (!String.IsNullOrEmpty(lines[j]))
                            {
                                have_some_non_blanks = true;
                                break;
                            }
                        }

                        if (!have_some_non_blanks)
                        {
                            break;
                        }
                    }

                    // If we get here, it must be continuation code...
                    {
                        if (null != last_attribute)
                        {
                            // Append this txt to the end of the last text of the last attribute
                            int max_attribute_index = endnote_record.attributes[last_attribute].Count - 1;
                            endnote_record.attributes[last_attribute][max_attribute_index] =
                                endnote_record.attributes[last_attribute][max_attribute_index]
                                + "\n" +
                                line;
                        }
                        else
                        {
                            throw new Exception(String.Format("Parsed line with no attribution: {0}", line));
                        }
                    }
                }
                catch (Exception ex)
                {
                    endnote_record.errors.Add(String.Format("Error parsing line '{0}': {1}", line, ex.Message));
                }
            }

            return endnote_record;
        }

        protected static List<List<string>> SplitMultipleEndNoteLines(string endnote_text)
        {
            string[] endnote_text_lines = endnote_text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            List<List<string>> all_records = new List<List<string>>();
            List<string> current_record = null;

            for (int i = 0; i < endnote_text_lines.Length; ++i)
            {
                // IF this is a new record, start a new record
                if (endnote_text_lines[i].ToUpper().StartsWith("%0 "))
                {
                    current_record = new List<string>();
                    all_records.Add(current_record);
                }

                if (null != current_record)
                {
                    current_record.Add(endnote_text_lines[i]);
                }
            }

            return all_records;
        }
    }
}

#else

#region --- Test ------------------------------------------------------------------------

namespace QiqqaSystemTester
{
    // Note https://stackoverflow.com/questions/10375090/accessing-protected-members-of-another-class

    [TestClass]
    public class EndNoteToBibTexTester : EndNoteToBibTex   // HACK: inherit so we can access protected members
    {
        private class Result
        {
            internal List<List<string>> lines_set;
            internal List<EndNoteRecord> records = new List<EndNoteRecord>();
            internal List<BibTexItem> bibtex_items = new List<BibTexItem>();
        }

        [DataRow("Utilities/BibTex/TestFiles/SampleEndnote.txt")]
        [DataTestMethod]
        public void Basic_Import_Test(string endnote_filepath)
        {
            string path = GetNormalizedPathToAnyFile(endnote_filepath);
            ASSERT.FileExists(path);

            string endnote_text = GetTestFileContent(path);

            Result rv = new Result();
            rv.lines_set = SplitMultipleEndNoteLines(endnote_text);
            foreach (List<string> lines in rv.lines_set)
            {
                EndNoteRecord record = MapEndNoteLinesToDictionary(lines);
                rv.records.Add(record);
                rv.bibtex_items.Add(record.ToBibTeX());
            }

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Newtonsoft.Json.Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new DataTestApprovalTextWriter(json_out, endnote_filepath),
                new DataTestLocationNamer(endnote_filepath) /* GetDefaultNamer() */,
                ApprovalTests.Approvals.GetReporter()
            );
        }
    }
}

#endregion

#endif



#region --- File Format Documentation ------------------------------------------------------------------------

/*
 * 
 * From http://blog.bibsonomy.org/2006/03/endnote-export.html


EndNote Tag	EndNote Field Name	=	BibTeX Field Name
%A	Author	=	author
%B	Secondary Title	=	booktitle
%C	Place Published	=	address
%D	Year	=	year
%E	Editor	=	editor
%F	Label	=	
%G	Language	=	
%H	Translated Author	=	
%I	Publisher	=	publisher
%J	Journal	=	journal
%K	Keywords	=	keywords
%L	Call Number	=	
%M	Accession Number	=	
%N	Number	=	number
%P	Pages	=	pages
%Q	Translated Title	=	
%R	Electronic Resource Number	=	
%S	Tertiary Title	=	
%T	Title	=	title
%U	URL	=	url
%V	Volume	=	volume
%W	Database Provider	=	
%X	Abstract	=	abstract
%Y	Tertiary Author	=	
%Z	Notes	=	annote
0%	Reference Type	=	BibTeX Entry Type
1%	Custom 1	=	
2%	Custom 2	=	
3%	Custom 3	=	
4%	Custom 4	=	
6%	Number of Volumes	=	
7%	Edition	=	edition
8%	Date	=	
9%	Type of Work	=	
%?	Subsidiary Author	=	
%@	ISBN/ISSN	=	
%!	Short Title	=	
%#	Custom 5	=	
%$	Custom 6	=	
%]	Custom 7	=	
%&	Section	=	chapter
%(	Original Publication	=	
%)	Reprint Edition	=	
%*	Reviewed Item	=	
%+	Author Address	=	
%^	Caption	=	
%>	Link to PDF	=	
%<	Research Notes	=	
%[	Access Date	=	
%=	Last Modified Date	=	
%~	Name of Database	=	
		=	crossref
		=	howpublished
		=	institution
		=	key
		=	month
		=	note
		=	organization
		=	school
		=	series
		=	type



We will handle these thus:

--- Special procesing:
0%	Reference Type	=	BibTeX Entry Type

--- Special processing
%K	keywords - create a smart getKeyWords method that returns a list of strings 
%A	author - separate separate authors with ' and '

--- One-for-one
%B	booktitle
%C	address
%D	year
%E	editor
%I	publisher
%J	journal
%N	number
%P	pages
%T	title
%U	url
%V	volume
%X	abstract
%Z	note
7%	edition
%&	chapter

Otherwise
*   misc
 
 
 */




/*

Some sample ENDNOTE

%0 Journal Article
%T Inhibition of Cyclin‐Dependent Kinases by Purine Analogues
%A Veselý, J.
%A Havliček, L.
%A Strnad, M.
%A Blow, J.J.
%A Donella‐Deana, A.
%A Pinna, L.
%A Letham, D.S.
%A Kato, J.
%A Detivaud, L.
%A Leclerc, S.
%J European Journal of Biochemistry
%V 224
%N 2
%P 771-786
%@ 1432-1033
%D 1994
%I Wiley Online Library


*/

#endregion

