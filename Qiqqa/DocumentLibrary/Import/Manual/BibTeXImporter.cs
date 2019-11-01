using System;
using System.IO;
using Utilities.BibTex.Parsing;

namespace Qiqqa.DocumentLibrary.Import.Manual
{

    /// <summary>
    /// Importing from BibTeX files
    /// </summary>
    public abstract class BibTeXImporter : FileImporter
    {
        protected const string PDF_MIMETYPE = "application/pdf";

        /// <summary>
        /// The original untransformed bibtex items
        /// </summary>
        protected BibTexParseResult BibTexParseResult { get; private set; }


        /// <summary>
        /// If specified, all the file entries will be prefixed. See JabRef import for example. 
        /// </summary>
        protected string FileEntryBasePath { get; set; }

        protected BibTeXImporter(Library library, string filename)
            : base(library, filename)
        {
            string bibTex = File.ReadAllText(ExportFileName, System.Text.Encoding.UTF8);

            BibTexParseResult = BibTexParser.Parse(bibTex);

            BibTexParseResult.Items.ForEach(x => Entries.Add(new BibTeXEntry { Item = x, BibTeX = x.ToBibTex() }));
        }

        /// <summary>
        /// Does replacements on file paths (for generic bibtex files) to turn the path into something we can use. 
        /// </summary>
        public string TranslateFilePath(string value)
        {
            value = TranslateFilePathWithoutContext(value);

            if (!String.IsNullOrEmpty(FileEntryBasePath))
            {
                //If its rooted, prepending it with anything will always be wrong. 
                //So assume its an error in their export. 
                //Otherwise prepend
                if (!Path.IsPathRooted(value))
                {
                    value = Path.Combine(FileEntryBasePath, value);
                }
            }

            return value;
        }

        /// <summary>
        /// Does replacements on file paths (for generic bibtex files) to turn the path into something we can use. 
        /// Without context meaning static - irresepctive of a particular file
        /// </summary>
        public static string TranslateFilePathWithoutContext(string value)
        {
            value = value.Replace("$\\backslash$:", ":"); //Mendeley
            value = value.Replace("/", "\\");  //Mendeley
            value = value.Replace("\\_", "_"); //Mendeley escapes underscores. I get the feeling from http://www.citeulike.org/groupforum/1245 that others might not.
            value = value.Replace("\\&", "&"); //Mendeley escapes &.(https://quantisle.fogbugz.com/f/cases/15838/) 

            value = value.Replace("\\:", ":");
            value = value.Replace("\\\\", "\\");

            return value;
        }

    }
}


