using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qiqqa.DocumentLibrary.Import.Manual;
using Utilities.Strings;

namespace Qiqqa.DocumentLibrary
{
    public class FilenameWithMetadataImport : BibTeXEntry
    {
        public override string ToString()
        {
            return String.Format(
                "---\r\n{0}\r\n{1}\r\n{2}\r\n{3}\r\n---"
                , Filename
                , BibTex.toBibTex()
                , Notes
                , StringTools.ConcatenateStrings(Tags, ';')
            );
        }
    }
}
