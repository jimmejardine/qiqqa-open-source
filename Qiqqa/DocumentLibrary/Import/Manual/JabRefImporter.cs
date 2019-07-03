using System;
using Utilities;

namespace Qiqqa.DocumentLibrary.Import.Manual
{
    public class JabRefImporter : MendeleyImporter
    {
        public JabRefImporter(Library library, string filename)
            : base(library, filename, true)
        {
            //JabRef files can have a comment like this:
            //@comment{jabref-meta: fileDirectory:D:\\Biblioteca;} 
            //which means the base dir is rooted off that. 

            try
            {
                foreach (var comment in BibTexParseResult.Comments)
                {
                    if (comment.Trim().StartsWith("jabref-meta: fileDirectory:"))
                    {
                        FileEntryBasePath = comment.Trim().Replace("jabref-meta: fileDirectory:", "").Trim();
                        if (FileEntryBasePath.EndsWith(";") && FileEntryBasePath.Length > 1)
                            FileEntryBasePath = FileEntryBasePath.Substring(0, FileEntryBasePath.Length - 1).Trim();
                    }
                }
            }catch(Exception ex)
            {
                Logging.Warn(ex, "Could not extract jabref file base metadata");
            }

        }
    }

}
