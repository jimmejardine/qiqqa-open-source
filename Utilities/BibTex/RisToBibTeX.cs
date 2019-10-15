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
    /// Convert RIS records to BibTeX.
    /// See the bottom of this file for the field mappings...
    /// </summary>
    public class RISToBibTex
    {
        public class RISRecord
        {
            public Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();
            public List<String> errors = new List<string>();

            public BibTexItem ToBibTeX()
            {
                BibTexItem bibtex_item = new BibTexItem();

                bibtex_item.Key = GenerateReasonableKey();

                // Process the type
                {
                    string ris_type = "";
                    if (attributes.ContainsKey("%0"))
                    {
                        ris_type = attributes["%0"][0];
                    }
                    bibtex_item.Type = MapRISToBibTeXType(ris_type);
                }

                // Process the generic fields -* note that we will silently throw away all "multiple" items with this field name
                foreach (var attribute_pair in attributes)
                {
                    string bibtex_field_type = MapRISToBibTeXFieldType(attribute_pair.Key);
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

            private static string MapRISToBibTeXType(string type)
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

            private static string MapRISToBibTeXFieldType(string field_type)
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

        public static List<RISRecord> Parse(string ris_text)
        {
            List<RISRecord> ris_records = new List<RISRecord>();

            List<List<string>> ris_record_texts = SplitMultipleRISLines(ris_text);
            foreach (List<string> ris_record_text in ris_record_texts)
            {
                RISRecord ris_record = MapRISLinesToDictionary(ris_record_text);
                ris_records.Add(ris_record);
            }

            return ris_records;
        }

        protected static RISRecord MapRISLinesToDictionary(List<string> lines)
        {
            RISRecord ris_record = new RISRecord();

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

                        if (!ris_record.attributes.ContainsKey(attribute))
                        {
                            ris_record.attributes[attribute] = new List<string>();
                        }
                        ris_record.attributes[attribute].Add(remainder);

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
                            int max_attribute_index = ris_record.attributes[last_attribute].Count - 1;
                            ris_record.attributes[last_attribute][max_attribute_index] =
                                ris_record.attributes[last_attribute][max_attribute_index]
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
                    ris_record.errors.Add(String.Format("Error parsing line '{0}': {1}", line, ex.Message));
                }
            }

            return ris_record;
        }

        protected static List<List<string>> SplitMultipleRISLines(string ris_text)
        {
            string[] ris_text_lines = ris_text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            List<List<string>> all_records = new List<List<string>>();
            List<string> current_record = null;

            for (int i = 0; i < ris_text_lines.Length; ++i)
            {
                // IF this is a new record, start a new record
                if (ris_text_lines[i].ToUpper().StartsWith("%0 "))
                {
                    current_record = new List<string>();
                    all_records.Add(current_record);
                }

                if (null != current_record)
                {
                    current_record.Add(ris_text_lines[i]);
                }
            }

            return all_records;
        }
    }
}

#else

#region --- Test ------------------------------------------------------------------------

namespace QiqqaUnitTester
{
    // Note https://stackoverflow.com/questions/10375090/accessing-protected-members-of-another-class

    [TestClass]
    public class RISToBibTexTester : RISToBibTex   // HACK: inherit so we can access protected members
    {
        private class Result
        {
            internal List<List<string>> lines_set;
            internal List<RISRecord> records = new List<RISRecord>();
            internal List<BibTexItem> bibtex_items = new List<BibTexItem>();
        }

        [DataRow("developer.easybib.com__ris-import__sample.txt")]
        [DataTestMethod]
        public void Basic_Import_Test(string ris_filepath)
        {
            string path = GetNormalizedPathToRISTestFile(ris_filepath);
            ASSERT.FileExists(path);

            string ris_text = GetTestFileContent(path);

            Result rv = new Result();
            rv.lines_set = SplitMultipleRISLines(ris_text);
            foreach (List<string> lines in rv.lines_set)
            {
                RISRecord record = MapRISLinesToDictionary(lines);
                rv.records.Add(record);
                rv.bibtex_items.Add(record.ToBibTeX());
            }

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Newtonsoft.Json.Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, ris_filepath),
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
 * From http://en.wikipedia.org/wiki/RIS_(file_format)

FIELDS

TY  - Type of reference (must be the first tag)
A2  - Secondary Author (each author on its own line preceded by the tag)
A3  - Tertiary Author (each author on its own line preceded by the tag)
A4  - Subsidiary Author (each author on its own line preceded by the tag)
AB  - Abstract
AD  - Author Address
AN  - Accession Number
AU  - Author (each author on its own line preceded by the tag)
C1  - Custom 1
C2  - Custom 2
C3  - Custom 3
C4  - Custom 4
C5  - Custom 5
C6  - Custom 6
C7  - Custom 7
C8  - Custom 8
CA  - Caption
CN  - Call Number
CY  - Place Published
DA  - Date
DB  - Name of Database
DO  - DOI
DP  - Database Provider
EP  - End Page
ET  - Edition
IS  - Number 
J2  - Alternate Title (this field is used for the abbreviated title of a book or journal name)
KW  - Keywords (keywords should be entered each on its own line preceded by the tag)
L1  - File Attachments (this is a link to a local file on the users system not a URL link)
L4  - Figure (this is also meant to be a link to a local file on the users's system and not a URL link)
LA  - Language
LB  - Label
M1  - Number
M3  - Type of Work
N1  - Notes
NV  - Number of Volumes
OP  - Original Publication
PB  - Publisher
PY  - Year
RI  - Reviewed Item
RN  - Research Notes
RP  - Reprint Edition
SE  - Section
SN  - ISBN/ISSN
SP  - Start Page
ST  - Short Title
T2  - Secondary Title
T3  - Tertiary Title
TA  - Translated Author
TI  - Title
TT  - Translated Title
UR  - URL
VL  - Volume
Y2  - Access Date
ER  - End of Reference (must be the last tag)
 

TYPES OF REFERENCE

ABST  - Abstract
ADVS  - Audiovisual material
AGGR  - Aggregated Database
ANCIENT - Ancient Text
ART   - Art Work
BILL  - Bill
BLOG  - Blog
BOOK  - Whole book
CASE  - Case
CHAP  - Book chapter
CHART - Chart
CLSWK - Classical Work
COMP  - Computer program
CONF  - Conference proceeding
CPAPER - Conference paper
CTLG  - Catalog
DATA  - Data file
DBASE - Online Database
DICT  - Dictionary
EBOOK - Electronic Book
ECHAP - Electronic Book Section
EDBOOK - Edited Book
EJOUR - Electronic Article
ELEC  - Web Page
ENCYC - Encyclopedia
EQUA  - Equation
FIGURE - Figure
GEN   - Generic
GOVDOC - Government Document
GRANT - Grant
HEAR  - Hearing
ICOMM - Internet Communication
INPR  - In Press
JFULL - Journal (full)
JOUR  - Journal
LEGAL - Legal Rule or Regulation
MANSCPT - Manuscript
MAP   - Map
MGZN  - Magazine article
MPCT  - Motion picture
MULTI - Online Multimedia
MUSIC - Music score
NEWS  - Newspaper
PAMP  - Pamphlet
PAT   - Patent
PCOMM - Personal communication
RPRT  - Report
SER   - Serial publication
SLIDE - Slide
SOUND - Sound recording
STAND - Standard
STAT  - Statute
THES  - Thesis/Dissertation
UNPB  - Unpublished work
VIDEO - Video recording

 */

/*

Some sample RIS

TY  - BOOK
A1  - Stickney, Clyde P.,
AU  - Brown, Paul R.
PB  - Dryden Press/Harcourt Brace College Publishers,
PY  - 1999.
SN  - 0030238110
U3  - 3428
TI  - Financial reporting and statement analysis : a strategic perspective
ER  - 

TY  - BOOK
N1  - The 12th edition of Elliott and Elliott is the recommended text for this module. | Chapter 17 pp453-459
A1  - Elliott, Barry.
AU  - Elliott, Jamie.
PB  - Financial Times / Prentice Hall,
PY  - 2008.
SN  - 0273712314
SN  - 9780273712312
U3  - 501060
TI  - Financial accounting and reporting
ER  - 

TY  - JOUR
N1  - Interesting article about the communication of financial information in Accounting, Auditing and Accountability.
UR  - http://www.emeraldinsight.com/Insight/viewContentItem.do?contentType=Article&contentId=869666
TI  - Improving the communication of accounting information through cartoon graphics Author(s): Malcolm Smith, Richard Taffler
ER  - 

TY  - BOOK
A1  - Black, Geoff.
PB  - Financial Times / Prentice Hall,
PY  - 2003.
SN  - 0273683500
U3  - 45774
TI  - Students' guide to accounting and financial reporting standards
ER  - 

TY  - BOOK
A1  - Lee, T. A.
PB  - Philip Allan,
PY  - June 1981.
SN  - 0860035123
SN  - 086003612x
U3  - 172387
TI  - Developments in financial reporting
ER  - 

TY  - BOOK
A1  - Pijper, Trevor.
PB  - Macmillan P.,
PY  - 1993.
SN  - 0333595920
U3  - 57505
TI  - Creative accounting : the effectiveness of financial reporting in the UK
ER  - 

TY  - ELEC
N1  - A good example of a company which uses the immaterial depreciation argument for not depreciating it's tangible fixed assets.  Look at the PPE note in 'significant accounting policies'.
UR  - http://smartpdf.blacksunplc.com/punchtaverresource005ara/
TI  - Punch Taverns 2005 Annual Report
ER  - 

TY  - BOOK
A1  - Brian, Coyle.
AU  - Institute of Chartered Secretaries and Administrators.
PB  - ICSA Publishing Ltd,
PY  - 2006.
SN  - 1860723500
SN  - 9781860723506
U3  - 493921
TI  - ICSA professional development corporate governance
VL  - ICSA professional development series
ER  - 

TY  - BOOK
A1  - King, Alfred M.
PB  - John Wiley,
PY  - 2006.
SN  - 0471771848
SN  - 9780471771845
U3  - 465814
TI  - Fair value for financial reporting : meeting the new FASB requirements
ER  - 

TY  - ELEC
N1  - IASB summary of intangible assets.
UR  - http://www.iasb.org/NR/rdonlyres/149D67E2-6769-4E8F-976D-6BABEB783D90/0/IAS38.pdf
TI  - IAS 38 Intangible Assets
ER  - 

TY  - ELEC
UR  - https://exchange.plymouth.ac.uk/intranet/edalt/Public/elearndesign2007/Sections/Resources/plagiarism.htm
TI  - Plagiarism Materials
ER  - 

TY  - ELEC
TI  - Plagiarism Materials
ER  - 



*/

#endregion

