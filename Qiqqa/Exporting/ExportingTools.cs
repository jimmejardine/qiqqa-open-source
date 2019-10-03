using System;
using System.Collections.Generic;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Files;
using Utilities.Language;

namespace Qiqqa.Exporting
{
    public class ExportingTools
    {
        public static string MakeExportFilename(PDFDocument pdf_document)
        {
            string year = pdf_document.YearCombined;
            if (year == Constants.UNKNOWN_YEAR)
            {
                year = "";
            }

            string authors = "";
            List<NameTools.Name> author_names = NameTools.SplitAuthors(pdf_document.AuthorsCombined);
            if (0 < author_names.Count)
            {
                authors = author_names[0].last_name;
            }

            int MAX_TITLE_LENGTH = 100;
            string title = pdf_document.TitleCombined;
            if (title.Length > MAX_TITLE_LENGTH)
            {
                title = title.Substring(0, MAX_TITLE_LENGTH);
            }

            string filename =
                String.Format(
                "{0}{1} - {2}",
                authors,
                year,
                title,
                null);

            filename = filename.Trim();
            filename = FileTools.MakeSafeFilename(filename);
            filename = filename + ".pdf";

            return filename;
        }
    }
}
