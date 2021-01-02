using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.Files
{
    /*
     * WARNING - THIS DOES NOT RETURN A TRUE HEX VERSION OF THE SHA.  IT LEAVES OUT THE 0 ON BYTES THAT ARE SMALL...THEREFORE ALL RESULTING STRINGS ARE NOT THE SAME LENGTH...
     */
    public class StreamFingerprint
    {
        // Hash v1: WARNING - THIS DOES NOT RETURN A TRUE HEX VERSION OF THE SHA.  IT LEAVES OUT THE 0 ON BYTES THAT ARE SMALL...THEREFORE ALL RESULTING STRINGS ARE NOT THE SAME LENGTH...
        public static string FromStream(Stream stream)
        {
            return FromStream_DOTNET(stream);
        }

        private static string FromStream_DOTNET(Stream stream)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                sha1.ComputeHash(stream);

                StringBuilder buff = new StringBuilder();
                foreach (byte hash_byte in sha1.Hash)
                {
                    buff.Append(String.Format("{0:X1}", hash_byte));    // <--- NB NB NB NEVER CHANGE THIS LINE TO X2 - the while of Qiqqa's infrastructure requires this shitty X1...
                }

                return buff.ToString();
            }
        }

        // Hash v1: WARNING - THIS DOES NOT RETURN A TRUE HEX VERSION OF THE SHA.  IT LEAVES OUT THE 0 ON BYTES THAT ARE SMALL...THEREFORE ALL RESULTING STRINGS ARE NOT THE SAME LENGTH...
        public static string FromFile(string filename)
        {
            //using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
            using (FileStream fs = File.OpenRead(filename))
            {
                return FromStream(fs);
            }
        }

        // Hash v1: WARNING - THIS DOES NOT RETURN A TRUE HEX VERSION OF THE SHA.  IT LEAVES OUT THE 0 ON BYTES THAT ARE SMALL...THEREFORE ALL RESULTING STRINGS ARE NOT THE SAME LENGTH...
        public static string FromText(string text)
        {
            byte[] unicode_bytes = Encoding.UTF8.GetBytes(text);
            using (MemoryStream ms = new MemoryStream(unicode_bytes))
            {
                return FromStream(ms);
            }
        }

        // The new Qiqqa hash: SHA256 so we can store PDFs which have SHA1 / Qiqqa hash v1 collisions (edge cases).
        //
        // Hash v2: returns the SHA256 hash of the stream content as HEX string
        public static string FromStream_SHA256(Stream stream)
        {
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                sha256.ComputeHash(stream);

                StringBuilder buff = new StringBuilder();
                foreach (byte hash_byte in sha256.Hash)
                {
                    buff.Append(String.Format("{0:X2}", hash_byte));
                }

                return buff.ToString();
            }
        }

        // Hash v2: returns the SHA256 hash of the file content as HEX string
        public static string FromFile_SHA256(string filename)
        {
            //using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
            using (FileStream fs = File.OpenRead(filename))
            {
                return FromStream_SHA256(fs);
            }
        }

        // Hash v2: returns the SHA256 hash of the string content as HEX string
        public static string FromText_SHA256(string text)
        {
            byte[] unicode_bytes = Encoding.UTF8.GetBytes(text);
            using (MemoryStream ms = new MemoryStream(unicode_bytes))
            {
                return FromStream_SHA256(ms);
            }
        }
    }
}
