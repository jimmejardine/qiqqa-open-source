using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Exporting
{
    internal class CitationMatrixExport
    {
        private class FingerprintToBibTeXMap
        {
            private Library library;
            private Dictionary<string, string> map;

            public FingerprintToBibTeXMap(Library library)
            {
                this.library = library;
                map = new Dictionary<string, string>();
            }

            public string Map(string fingerprint, bool use_bibtex_key_where_possible)
            {
                if (!use_bibtex_key_where_possible)
                {
                    return fingerprint;
                }

                if (map.ContainsKey(fingerprint))
                {
                    return map[fingerprint];
                }

                PDFDocument pdf_document = library.GetDocumentByFingerprint(fingerprint);
                if (null != pdf_document)
                {
                    string bibtex_key = pdf_document.BibTexKey;
                    if (!String.IsNullOrEmpty(bibtex_key))
                    {
                        map[fingerprint] = bibtex_key;
                        return bibtex_key;
                    }
                }

                // If we get here we are either lacking the PDF or the BibTeX...
                map[fingerprint] = fingerprint;
                return fingerprint;
            }
        }


        internal static void Export(Library library, List<PDFDocument> pdf_documents)
        {
            StatusManager.Instance.UpdateStatus("CitationMatrix", "Exporting Citation Maxtrix");

            // Ask the user what they want
            bool export_inbound_citations = MessageBoxes.AskQuestion("Do you want to follow one layer of INBOUND citations?");
            bool export_outbound_citations = MessageBoxes.AskQuestion("Do you want to follow one layer of OUTBOUND citations?");
            bool export_bibtex_keys = MessageBoxes.AskQuestion("Do you want to use BibTeX keys instead of Qiqqa fingerprints where possible?");

            SaveFileDialog save_file_dialog = new SaveFileDialog();
            {
                save_file_dialog.AddExtension = true;
                save_file_dialog.CheckPathExists = true;
                save_file_dialog.DereferenceLinks = true;
                save_file_dialog.OverwritePrompt = true;
                save_file_dialog.ValidateNames = true;
                save_file_dialog.DefaultExt = "txt";
                save_file_dialog.Filter = "Text files (*.txt)|*.zip|All files (*.*)|*.*";
                save_file_dialog.FileName = "Qiqqa Citation Matrix.txt";

                if (true != save_file_dialog.ShowDialog())
                {
                    Logging.Info("User cancelled export of citation matrix.");
                    StatusManager.Instance.UpdateStatus("CitationMatrix", "Cancelled export of Citation Maxtrix");
                    return;
                }
            }

            // Build the list of papers we want to list
            StatusManager.Instance.UpdateStatus("CitationMatrix", "Widening export set.");
            HashSet<string> working_set_fingerprint = new HashSet<string>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                working_set_fingerprint.Add(pdf_document.Fingerprint);

                if (export_inbound_citations)
                {
                    foreach (var citation in pdf_document.PDFDocumentCitationManager.GetInboundCitations())
                    {
                        working_set_fingerprint.Add(citation.fingerprint_outbound);
                    }
                }
                if (export_outbound_citations)
                {
                    foreach (var citation in pdf_document.PDFDocumentCitationManager.GetOutboundCitations())
                    {
                        working_set_fingerprint.Add(citation.fingerprint_inbound);
                    }
                }
            }

            // The bibtex mapper
            FingerprintToBibTeXMap map = new FingerprintToBibTeXMap(library);

            // Build the result
            StatusManager.Instance.UpdateStatus("CitationMatrix", "Building Citation Matrix.");
            DateTime start_time = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine("Cross citation report");
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine(String.Format("Generated by Qiqqa ({0})", Common.Configuration.WebsiteAccess.Url_Documentation4Qiqqa));
            sb.AppendLine(String.Format("On {0} {1}", start_time.ToLongDateString(), start_time.ToLongTimeString()));
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine("Format is:");
            sb.AppendLine("  source1,target1,target2,target3,");
            sb.AppendLine("  source2,target1,target4,");
            sb.AppendLine("  source3,target1,target3,target5,target6,");
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine();

            foreach (var fingerprint in working_set_fingerprint)
            {
                PDFDocument pdf_document = library.GetDocumentByFingerprint(fingerprint);
                if (null != pdf_document)
                {
                    List<Citation> citations = pdf_document.PDFDocumentCitationManager.GetOutboundCitations();
                    sb.AppendFormat("{0}", map.Map(pdf_document.Fingerprint, export_bibtex_keys));
                    sb.AppendFormat(",");
                    foreach (Citation citation in citations)
                    {
                        sb.AppendFormat("{0},", map.Map(citation.fingerprint_inbound, export_bibtex_keys));
                    }
                    sb.AppendFormat("\r\n");
                }
                else
                {
                    Logging.Warn("While exporting cocitations, couldn't locate document {0} in library {1}.", fingerprint, library.WebLibraryDetail.Title);
                }
            }

            StatusManager.Instance.UpdateStatus("CitationMatrix", "Writing Citation Matrix.");
            string target_filename = save_file_dialog.FileName;
            File.WriteAllText(target_filename, sb.ToString());

            StatusManager.Instance.UpdateStatus("CitationMatrix", "Exported Citation Maxtrix");
        }
    }
}
