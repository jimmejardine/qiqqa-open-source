using System.IO;
using System.Xml;

namespace Utilities
{
    public class XMLTools
    {
        public static string ToString(XmlDocument doc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xtw = new XmlTextWriter(sw);
            xtw.Formatting = Formatting.Indented;
            doc.WriteTo(xtw);
            return sw.ToString();
        }
    }
}
