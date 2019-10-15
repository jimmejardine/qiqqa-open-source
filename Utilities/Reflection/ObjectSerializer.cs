using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.Reflection
{
	[Obsolete("This class will be phased out. .NET binary serialization causes too much trouble, e.g. https://stackoverflow.com/questions/6825819/how-can-i-tell-when-what-is-loading-certain-assemblies and https://social.msdn.microsoft.com/forums/vstudio/en-US/7192f23e-7d43-47b5-b401-5fcd19671cf6/invalidcastexception-thrown-when-casting-to-the-same-type. Use Json.NET instead. And then there's https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/how-to-enable-and-disable-automatic-binding-redirection", false)]
    public class ObjectSerializer
    {
        public static object LoadObject(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return LoadObject(fs);
            }
        }

        private static object LoadObject(Stream stream)
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

        private static void SaveObject(Stream stream, object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, obj);
        }
    }
}
