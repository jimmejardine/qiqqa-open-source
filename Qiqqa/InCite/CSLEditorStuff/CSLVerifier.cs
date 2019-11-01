using System;
using System.Collections.Generic;
using System.Xml;

namespace Qiqqa.InCite.CSLEditorStuff
{
    internal class CSLVerifier
    {
        public static List<string> Verify(string style_file_filename)
        {
            List<string> results = new List<string>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument(); //* create an xml document object.             
                xmlDoc.Load(style_file_filename);
            }
            catch (Exception ex)
            {
                results.Add(ex.Message);
            }

            return results;
        }
    }
}
