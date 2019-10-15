using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.Reflection
{
    public class ObjectSerializer
    {
        public static object LoadObject(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return LoadObject(fs);
            }
        }

        public static object LoadObject(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(stream);
        }

        public static void SaveObject(string filename, object obj)
        {
            // Make sure the path exists
            string pathname = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(pathname);

            using (FileStream fs = File.OpenWrite(filename))
            {
                SaveObject(fs, obj);
            }
        }

        public static void SaveObject(Stream stream, object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, obj);
        }
    }
}
