using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace QiqqaLegacyFileFormats          // namespace Utilities.Files
{
    public class XmlSerializeFile
    {
        /// <summary>
        /// Deserialize the file if possible, otherwise log the problem and return null.
        /// </summary>
        public static T Deserialize<T>(string filename) where T : class
        {
            try
            {
                using (Stream file_stream = File.OpenRead(filename))
                {
                    XmlReader xml_reader = new XmlTextReader(file_stream);
                    XmlSerializer xml_serializer = new XmlSerializer(typeof(T));
                    return xml_serializer.Deserialize(xml_reader) as T;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Unable to deserialize type {0} from file: {1}", typeof(T).FullName, filename);
                try
                {
                    Logging.Info("XML from file {0}:\n{1}", filename, File.ReadAllText(filename));
                }
                catch (Exception ex2)
                {
                    Logging.Error(ex2);

                    //  oh well, was just for info anyway
                }
                return null;
            }
        }

        public static void Serialize<T>(T obj, string filename)
        {
            using (FileStream text_writer = File.OpenWrite(filename))
            {
                new XmlSerializer(typeof(T)).Serialize(text_writer, filename);
            }
        }
    }
}
