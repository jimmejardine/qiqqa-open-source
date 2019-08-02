using System;
using System.Collections.Generic;
using System.Text;
using Qiqqa.DocumentLibrary;
using Utilities.Strings;

namespace Qiqqa.Documents.PDF.CitationManagerStuff
{
    public class PDFDocumentCitationManager
    {
        PDFDocument pdf_document;

        object locker = new object();
        List<Citation> _citations = null;
        List<Citation> Citations
        {
            get
            {
                if (null == _citations)
                {
                    _citations = ReadFromDisk(pdf_document);
                }

                return _citations;
            }
        }
        
        public PDFDocumentCitationManager(PDFDocument pdf_document)
        {
            this.pdf_document = pdf_document;
        }

        public bool ContainsInboundCitation(string fingerprint_outbound)
        {
            string fingerprint_inbound = this.pdf_document.Fingerprint;
            return ContainsCitation(fingerprint_outbound, fingerprint_inbound);
        }

        public bool ContainsOutboundCitation(string fingerprint_inbound)
        {
            string fingerprint_outbound = this.pdf_document.Fingerprint;
            return ContainsCitation(fingerprint_outbound, fingerprint_inbound);
        }

        private bool ContainsCitation(string fingerprint_outbound, string fingerprint_inbound)
        {
            lock (locker)
            {
                List<Citation> citations = Citations;
                foreach (Citation citation in citations)
                {
                    if (0 == citation.fingerprint_inbound.CompareTo(fingerprint_inbound) && 0 == citation.fingerprint_outbound.CompareTo(fingerprint_outbound))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void AddInboundCitation(string fingerprint_outbound)
        {
            string fingerprint_inbound = this.pdf_document.Fingerprint;
            AddCitation(fingerprint_outbound, fingerprint_inbound, Citation.Type.AUTO_CITATION);
        }

        public void AddOutboundCitation(string fingerprint_inbound)
        {
            string fingerprint_outbound = this.pdf_document.Fingerprint;
            AddCitation(fingerprint_outbound, fingerprint_inbound, Citation.Type.AUTO_CITATION);
        }

        public void AddLinkedDocument(string fingerprint_linked)
        {
            string fingerprint_this = this.pdf_document.Fingerprint;
            AddCitation(fingerprint_this, fingerprint_linked, Citation.Type.MANUAL_LINK);            
        }

        private void AddCitation(string fingerprint_outbound, string fingerprint_inbound, Citation.Type type)
        {
            this.AddCitation(new Citation { fingerprint_outbound = fingerprint_outbound, fingerprint_inbound = fingerprint_inbound, type = type });
        }
        
        private void AddCitation(Citation new_citation)
        {
            lock (locker)
            {
                // We can't cite ourself!
                if (0 == new_citation.fingerprint_outbound.CompareTo(new_citation.fingerprint_inbound))
                {
                    return;
                }

                // Try to update an existing record
                bool needs_write = false;
                bool citation_exists = false;
                List<Citation> citations = Citations;
                foreach (Citation citation in citations)
                {
                    if (new_citation.Equals(citation))
                    {
                        citation_exists = true;
                        break;
                    }
                }

                // If there was no existing record
                if (!citation_exists)
                {
                    citations.Add(new_citation);
                    needs_write = true;
                }

                // If we need to write, write!
                if (needs_write)
                {
                    WriteToDisk(pdf_document, citations);
                }
            }
        }

        public List<Citation> GetInboundCitations()
        {
            return GetCitations(null, this.pdf_document.Fingerprint);
        }

        public List<Citation> GetOutboundCitations()
        {
            return GetCitations(this.pdf_document.Fingerprint, null);
        }

        private List<Citation> GetCitations(string fingerprint_outbound, string fingerprint_inbound)
        {
            List<Citation> result_citations = new List<Citation>();
            
            lock (locker)
            {
                List<Citation> citations = Citations;
                foreach (Citation citation in citations)
                {
                    if (null != fingerprint_outbound && Citation.Type.AUTO_CITATION == citation.type && 0 == fingerprint_outbound.CompareTo(citation.fingerprint_outbound)) result_citations.Add(citation);
                    if (null != fingerprint_inbound && Citation.Type.AUTO_CITATION == citation.type && 0 == fingerprint_inbound.CompareTo(citation.fingerprint_inbound)) result_citations.Add(citation);
                }
            }

            return result_citations;
        }

        public List<Citation> GetLinkedDocuments()
        {
            string fingerprint = this.pdf_document.Fingerprint;


            List<Citation> result_citations = new List<Citation>();

            lock (locker)
            {
                List<Citation> citations = Citations;
                foreach (Citation citation in citations)
                {
                    if (Citation.Type.MANUAL_LINK == citation.type && 0 == fingerprint.CompareTo(citation.fingerprint_outbound)) result_citations.Add(citation);
                }
            }

            return result_citations;
        }

        private static void WriteToDisk(PDFDocument pdf_document, List<Citation> citations)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var citation in citations)
            {
                sb.AppendLine(String.Format(
                    "{0},{1},{2}",
                    citation.fingerprint_outbound,
                    citation.fingerprint_inbound,
                    (int)citation.type
                    ));
            }

            string text = sb.ToString();
            pdf_document.Library.LibraryDB.PutString(pdf_document.Fingerprint, PDFDocumentFileLocations.CITATIONS, text);
        }

        private static List<Citation> ReadFromDisk(PDFDocument pdf_document)
        {
            List<Citation> citations = new List<Citation>();

            List<LibraryDB.LibraryItem> library_items = pdf_document.Library.LibraryDB.GetLibraryItems(pdf_document.Fingerprint, PDFDocumentFileLocations.CITATIONS);
            if (0 < library_items.Count)
            {
                LibraryDB.LibraryItem library_item = library_items[0];

                string lines_all = Encoding.UTF8.GetString(library_item.data);
                StringArray lines = StringTools.splitAtNewline(lines_all);
                foreach (string line in lines)
                {
                    string[] chunks = line.Split(',');

                    Citation citation = new Citation();
                    citation.fingerprint_outbound = chunks[0];
                    citation.fingerprint_inbound = chunks[1];
                    citation.type = (Citation.Type) Convert.ToInt32(chunks[2]);

                    citations.Add(citation);
                }
            }            

            return citations;
        }

        internal void CloneFrom(PDFDocumentCitationManager other)
        {
            lock (locker)
            {
                this.Citations.AddRange(other.Citations);
                WriteToDisk(pdf_document, Citations);
            }
        }
    }
}
