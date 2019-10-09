using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Strings;

namespace Qiqqa.DocumentLibrary
{
    public class FilenameWithMetadataImport
    {
        public string filename = null;
        public string original_filename = null;
        public string suggested_download_source_uri = null;
        public string bibtex = null;
        public string notes = null;
        public HashSet<string> tags = new HashSet<string>();

        public override string ToString()
        {
            return String.Format(
                "---\r\n{0}\r\n{1}\r\n{2}\r\n{3}\r\n---"
                , filename
                , bibtex
                , notes
                , StringTools.ConcatenateStrings(tags, separator: ";")
            );
        }
    }
}
