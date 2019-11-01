using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;

namespace Qiqqa.Documents.PDF
{
    internal class PDFDocumentBibTeXProcessor
    {
        internal static string Process(string source)
        {
            // If it is empty
            if (String.IsNullOrEmpty(source))
            {
                return source;
            }

            // See if it is valid BibTeX - if so, use it...
            try
            {
                List<BibTexItem> bibtexes = BibTexParser.Parse(source).Items;
                if (0 == bibtexes[0].Exceptions.Count)
                {
                    return source;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex, "BibTeX");
            }

            // See if it is valid PubMed XML
            try
            {
                string bibtex;
                List<string> messages;
                if (PubMedXMLToBibTex.TryConvert(source, out bibtex, out messages))
                {
                    return bibtex;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex, "PubMed");
            }

            // See if it is valid EndNote
            try
            {
                List<EndNoteToBibTex.EndNoteRecord> endnotes = EndNoteToBibTex.Parse(source);
                if (endnotes.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var endnote in endnotes)
                    {
                        sb.Append(endnote.ToBibTeX().ToBibTex());
                        sb.Append("\n\n");
                    }
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex, "EndNote");
            }

            // If we get here, we don't have a clue what is going on...
            return source;
        }

        private static void ExceptionLog(Exception ex, string msg)
        {
            Logging.Warn(ex, msg);
        }
    }
}
