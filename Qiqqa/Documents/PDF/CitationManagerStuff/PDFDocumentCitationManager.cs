using System;
using System.Collections.Generic;
using System.Text;
using Qiqqa.DocumentLibrary;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Strings;

namespace Qiqqa.Documents.PDF.CitationManagerStuff
{
    public class PDFDocumentCitationManager
    {
        // TODO: make this a WeakReference, or better yet: get rid of it entirely!
        private PDFDocument pdf_document;

        private object citations_lock = new object();
        private List<Citation> _citations = null;

        private List<Citation> Citations_RAW
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

        /// <summary>
        /// Deliver a safe-to-use copy of the citations list: use this API
        /// to access the citations list from another instance of PDFDocumentCitationManager.
        /// </summary>
        private List<Citation> Citations_LOCKED
        {
            get
            {
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (citations_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    return new List<Citation>(Citations_RAW);
                }
            }
        }

        public PDFDocumentCitationManager(PDFDocument pdf_document)
        {
            this.pdf_document = pdf_document;
        }

        public bool ContainsInboundCitation(string fingerprint_outbound)
        {
            string fingerprint_inbound = pdf_document.Fingerprint;
            return ContainsCitation(fingerprint_outbound, fingerprint_inbound);
        }

        public bool ContainsOutboundCitation(string fingerprint_inbound)
        {
            string fingerprint_outbound = pdf_document.Fingerprint;
            return ContainsCitation(fingerprint_outbound, fingerprint_inbound);
        }

        private bool ContainsCitation(string fingerprint_outbound, string fingerprint_inbound)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (citations_lock)
            {
                // l1_clk.LockPerfTimerStop();
                List<Citation> citations = Citations_RAW;
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
            string fingerprint_inbound = pdf_document.Fingerprint;
            AddCitation(fingerprint_outbound, fingerprint_inbound, Citation.Type.AUTO_CITATION);
        }

        public void AddOutboundCitation(string fingerprint_inbound)
        {
            string fingerprint_outbound = pdf_document.Fingerprint;
            AddCitation(fingerprint_outbound, fingerprint_inbound, Citation.Type.AUTO_CITATION);
        }

        public void AddLinkedDocument(string fingerprint_linked)
        {
            string fingerprint_this = pdf_document.Fingerprint;
            AddCitation(fingerprint_this, fingerprint_linked, Citation.Type.MANUAL_LINK);
        }

        private void AddCitation(string fingerprint_outbound, string fingerprint_inbound, Citation.Type type)
        {
            AddCitation(new Citation { fingerprint_outbound = fingerprint_outbound, fingerprint_inbound = fingerprint_inbound, type = type });
        }

        private void AddCitation(Citation new_citation)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (citations_lock)
            {
                // l1_clk.LockPerfTimerStop();
                // We can't cite ourself!
                if (0 == new_citation.fingerprint_outbound.CompareTo(new_citation.fingerprint_inbound))
                {
                    return;
                }

                // Try to update an existing record
                bool needs_write = false;
                bool citation_exists = false;
                List<Citation> citations = Citations_RAW;
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
            string fingerprint = pdf_document.Fingerprint;
            List<Citation> result_citations = new List<Citation>();

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (citations_lock)
            {
                // l1_clk.LockPerfTimerStop();
                List<Citation> citations = Citations_RAW;
                foreach (Citation citation in citations)
                {
                    if (Citation.Type.AUTO_CITATION == citation.type && 0 == fingerprint.CompareTo(citation.fingerprint_inbound)) result_citations.Add(citation);
                }
            }

            return result_citations;
        }

        public List<Citation> GetOutboundCitations()
        {
            string fingerprint = pdf_document.Fingerprint;
            List<Citation> result_citations = new List<Citation>();

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (citations_lock)
            {
                // l1_clk.LockPerfTimerStop();
                List<Citation> citations = Citations_RAW;
                foreach (Citation citation in citations)
                {
                    if (Citation.Type.AUTO_CITATION == citation.type && 0 == fingerprint.CompareTo(citation.fingerprint_outbound)) result_citations.Add(citation);
                }
            }

            return result_citations;
        }

        public List<Citation> GetLinkedDocuments()
        {
            string fingerprint = pdf_document.Fingerprint;

            List<Citation> result_citations = new List<Citation>();

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (citations_lock)
            {
                // l1_clk.LockPerfTimerStop();
                List<Citation> citations = Citations_RAW;
                foreach (Citation citation in citations)
                {
                    if (Citation.Type.MANUAL_LINK == citation.type && 0 == fingerprint.CompareTo(citation.fingerprint_outbound)) result_citations.Add(citation);
                }
            }

            return result_citations;
        }

        private static void WriteToDisk(PDFDocument pdf_document, List<Citation> citations)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

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
            pdf_document.LibraryRef.Xlibrary.LibraryDB.PutString(pdf_document.Fingerprint, PDFDocumentFileLocations.CITATIONS, text);
        }

        private static List<Citation> ReadFromDisk(PDFDocument pdf_document)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            List<Citation> citations = new List<Citation>();

            List<LibraryDB.LibraryItem> library_items = pdf_document.LibraryRef.Xlibrary.LibraryDB.GetLibraryItems(PDFDocumentFileLocations.CITATIONS, new List<string>() { pdf_document.Fingerprint });
            ASSERT.Test(library_items.Count < 2);
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
                    citation.type = (Citation.Type)Convert.ToInt32(chunks[2]);

                    citations.Add(citation);
                }
            }

            return citations;
        }

        internal void CloneFrom(PDFDocumentCitationManager other)
        {
            // prevent deadlock due to possible incorrect use of this API:
            if (other != this)
            {
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (citations_lock)
                {
                    // l1_clk.LockPerfTimerStop();

                    // the other PDFCitMgr instance has its own lock, so without
                    // a copy through `other.Citations_LOCKED` this code would
                    // be very much thread-UNSAFE!
                    Citations_RAW.AddRange(other.Citations_LOCKED);
                    WriteToDisk(pdf_document, Citations_RAW);
                }
            }
        }
    }
}
