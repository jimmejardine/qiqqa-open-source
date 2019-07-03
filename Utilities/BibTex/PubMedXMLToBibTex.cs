using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Utilities.BibTex
{
    public class PubMedXMLToBibTex
    {
        public static bool TryConvert(string pubmed_xml, out string bibtex, out List<string> messages, bool include_abstract = true, bool include_original_pubmed_record = true)
        {
            // Initialise the outputs
            messages = new List<string>();
            bibtex = "";

            string pubmed_xml_wrapped =
                ""
                + "<PubmedArticles>"
                + pubmed_xml
                + "</PubmedArticles>"
                ;

            try
            {
                // Attempt to parse the XML
                XmlDocument xml_doc_wrapped = new XmlDocument();
                xml_doc_wrapped.LoadXml(pubmed_xml_wrapped);

                XmlNode xml_doc = xml_doc_wrapped.SelectSingleNode("/PubmedArticles/PubmedArticle");

                // Check that it is PubMed XML
                string pmid = GetElementText(xml_doc, "MedlineCitation/PMID");
                if (String.IsNullOrEmpty(pmid))
                {
                    throw new Exception("No PubMed ID was found in the XML");
                }

                // Get the fields
                string journal_title = GetElementText(xml_doc, "MedlineCitation/Article/Journal/Title");
                string journal_title_iso = GetElementText(xml_doc, "MedlineCitation/Article/Journal/ISOAbbreviation");
                string journal_issn = GetElementText(xml_doc, "MedlineCitation/Article/Journal/JournalIssue/ISSN");
                string journal_volume = GetElementText(xml_doc, "MedlineCitation/Article/Journal/JournalIssue/Volume");
                string journal_issue = GetElementText(xml_doc, "MedlineCitation/Article/Journal/JournalIssue/Issue");
                string journal_year = GetElementText(xml_doc, "MedlineCitation/Article/Journal/JournalIssue/PubDate/Year");
                string journal_month = GetElementText(xml_doc, "MedlineCitation/Article/Journal/JournalIssue/PubDate/Month");
                string journal_day = GetElementText(xml_doc, "MedlineCitation/Article/Journal/JournalIssue/PubDate/Day");
                string article_title = GetElementText(xml_doc, "MedlineCitation/Article/ArticleTitle");
                string article_pages = GetElementText(xml_doc, "MedlineCitation/Article/Pagination/MedlinePgn");
                string doi = GetElementText(xml_doc, "MedlineCitation/Article/ELocationID[@EIdType='doi']");
                string abstract_str = GetElementText(xml_doc, "MedlineCitation/Article/Abstract");


                string article_authors = GetAuthors(xml_doc);

                // Do some post processing
                article_pages = article_pages.Replace("-", "--");

                // Spit out the BibTeX
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("@article{{pmid:{0},\n", pmid);
                {
                    AppendIfPresent(sb, "title", article_title);
                    AppendIfPresent(sb, "author", article_authors);
                    AppendIfPresent(sb, "journal", journal_title);
                    AppendIfPresent(sb, "journal-iso", journal_title_iso);
                    AppendIfPresent(sb, "volume", journal_volume);
                    AppendIfPresent(sb, "number", journal_issue);
                    AppendIfPresent(sb, "year", journal_year);
                    AppendIfPresent(sb, "month", journal_month);
                    AppendIfPresent(sb, "pages", article_pages);
                    AppendIfPresent(sb, "issn", journal_issn);
                    AppendIfPresent(sb, "doi", doi);
                    AppendIfPresent(sb, "pmid", pmid);
                    if (include_abstract)
                    {
                        AppendIfPresent(sb, "abstract", abstract_str);
                    }
                }
                RemoveLastComma(sb);
                sb.AppendFormat("}}\n");

                // Append the original xml
                if (include_original_pubmed_record)
                {
                    sb.AppendFormat("\n\n");
                    sb.AppendFormat("% BEGIN:Original PubMed XML\n");

                    string article_xml = XMLNodeToPrettyString(xml_doc);
                    string[] article_xml_lines = article_xml.Split('\n');
                    foreach (string article_xml_line in article_xml_lines)
                    {
                        sb.AppendFormat("%  {0}\n", article_xml_line);
                    }
                    sb.AppendFormat("% END:Original PubMed XML\n");
                }

                bibtex = sb.ToString();
                return true;
            }

            catch (Exception ex)
            {
                messages.Add(ex.Message);
                return false;
            }
        }

        private static string XMLNodeToPrettyString(XmlNode xml_node)
        {
            StringBuilder sb = new StringBuilder();
            {
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (XmlTextWriter tw = new XmlTextWriter(sw))
                    {
                        tw.Formatting = Formatting.Indented;
                        xml_node.WriteTo(tw);
                        return sb.ToString();
                    }
                }
            }
        }

        private static void AppendIfPresent(StringBuilder sb, string field, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = value.Replace("{", "\\{");
                value = value.Replace("}", "\\}");

                sb.AppendFormat("  {0}={{{1}}},\n", field, value);
            }
        }

        private static void RemoveLastComma(StringBuilder sb)
        {
            if (sb.Length > 0 && ',' == sb[sb.Length - 2])
            {
                sb.Remove(sb.Length-2, 1);
            }
        }


        private static string GetAuthors(XmlNode xml_node)
        {
            StringBuilder sb = new StringBuilder();

            XmlNodeList xml_authors = xml_node.SelectNodes("MedlineCitation/Article/AuthorList/Author");
            foreach (XmlNode xml_author in xml_authors)
            {
                string lastname = GetElementText(xml_author, "LastName");
                string forename = GetElementText(xml_author, "ForeName");
                string initials = GetElementText(xml_author, "Initials");

                // We have to have a surname to proceed with this author
                if (String.IsNullOrEmpty(lastname))
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append(" and ");
                }

                // Pick a reasonable firstname
                string firstname = forename;
                if (String.IsNullOrEmpty(firstname))
                {
                    firstname = initials;
                }

                sb.Append(lastname);
                if (!String.IsNullOrEmpty(firstname))
                {
                    sb.Append(", " + firstname);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Attempts to get the InnerText of the specified XML element.
        /// Returns "" if the element does not exist.
        /// </summary>
        /// <param name="xml_node"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetElementText(XmlNode xml_node, string path)
        {
            XmlNode inner_xml_node = xml_node.SelectSingleNode(path);
            if (null != inner_xml_node)
            {
                return inner_xml_node.InnerText;
            }
            else
            {
                return "";
            }
        }

        private static void Test_SINGLE(string filename)
        {
            // See http://www.nlm.nih.gov/bsd/licensee/elements_descriptions.html for the low-down

            string pubmed_xml = File.ReadAllText(filename);

            string bibtex;
            List<string> messages;
            bool success = TryConvert(pubmed_xml, out bibtex, out messages);
            if (success)
            {
                string filename_out = filename + ".txt";
                File.WriteAllText(filename_out, bibtex);
            }
        }

        public static void Test()
        {
            Test_SINGLE(@"C:\temp\sample_pubmed.xml");            
        }
    }
}
