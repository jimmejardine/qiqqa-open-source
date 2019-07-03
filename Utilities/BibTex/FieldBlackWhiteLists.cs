using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.BibTex
{
    public class FieldBlackWhiteLists
    {
        public static HashSet<string> KeyWhitelist()
        {
            HashSet<string> list = new HashSet<string>();
            list.Add("title");
            list.Add("booktitle");
            list.Add("shorttitle");
            list.Add("author");
            list.Add("editor");
            list.Add("publisher");
            list.Add("publisher-place");
            list.Add("journal");
            list.Add("journal-iso");
            list.Add("volume");
            list.Add("edition");
            list.Add("number");
            list.Add("series");
            list.Add("issue");
            list.Add("chapter");
            list.Add("pages");
            list.Add("numpages");
            list.Add("address");
            list.Add("location");
            list.Add("year");
            list.Add("month");
            list.Add("day");
            list.Add("doi");
            list.Add("isbn");
            list.Add("issn");
            list.Add("lccn");
            list.Add("acmid");
            list.Add("pmid");
            list.Add("pubmed_id");
            list.Add("pubmedid");
            list.Add("arxivid");
            return list;
        }

        public static HashSet<string> KeyGreylist()
        {
            HashSet<string> list = new HashSet<string>();
            return list;
        }

        public static HashSet<string> KeyBlacklist()
        {
            HashSet<string> list = new HashSet<string>();
            list.Add("institution");
            list.Add("copyright");
            list.Add("correspondence_address");
            list.Add("references");
            list.Add("coden");
            list.Add("art_number");
            list.Add("misc");
            list.Add("tag");
            list.Add("tags");
            list.Add("abstract");
            list.Add("keyword");
            list.Add("keywords");
            list.Add("author_keywords");
            list.Add("comment");
            list.Add("comments");
            list.Add("annote");
            list.Add("note");
            list.Add("notes");
            list.Add("review");
            list.Add("language");
            list.Add("file");
            list.Add("url");
            list.Add("eprint");
            list.Add("citeulike-article-id");
            list.Add("posted-at");
            list.Add("priority");
            list.Add("mendeley-tags");
            list.Add("jstor_articletype");
            list.Add("jstor_formatteddate");
            list.Add("archiveprefix");
            list.Add("primaryclass");
            list.Add("document_type");
            list.Add("adsurl");
            list.Add("pdf");
            list.Add("adsnote");
            list.Add("encoding");
            list.Add("owner");
            list.Add("source");
            list.Add("affiliation");
            return list;
        }
    }
}
