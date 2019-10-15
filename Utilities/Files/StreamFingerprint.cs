using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;

//using Org.BouncyCastle.Crypto.Digests;

namespace Utilities.Files
{
    /*
     * WARNING - THIS DOES NOT RETURN A TRUE HEX VERSION OF THE SHA.  IT LEAVES OUT THE 0 ON BYTES THAT ARE SMALL...THEREFORE ALL RESULTING STRINGS ARE NOT THE SAME LENGTH...
     */
    public class StreamFingerprint
    {
        public static string FromStream(Stream stream)
        {
            return FromStream_DOTNET(stream);
        }

#if TEST
        private static string FromStream_BOUNCY(Stream stream)
        {
            int BUFFER_SIZE = 5 * 1024 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];

            int total_read = 0;
            int num_read = 0;
            Sha1Digest sha1 = new Sha1Digest();
            while (0 < (num_read = stream.Read(buffer, 0, BUFFER_SIZE)))
            {
                total_read += num_read;
                sha1.BlockUpdate(buffer, 0, num_read);
            }

            byte[] hash = new byte[sha1.GetDigestSize()];
            sha1.DoFinal(hash, 0);

            // Convert to string
            StringBuilder buff = new StringBuilder();
            foreach (byte hash_byte in hash)
            {
                buff.Append(String.Format("{0:X1}", hash_byte));
            }

            return buff.ToString();
        }
#endif
        
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

        public static string FromFile(string filename)
        {
            //using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
            using (FileStream fs = File.OpenRead(filename))
            {
                return FromStream(fs);
            }
        }

        // WARNING - THIS DOES NOT RETURN A TRUE HEX VERSION OF THE SHA.  IT LEAVES OUT THE 0 ON BYTES THAT ARE SMALL...THEREFORE ALL RESULTING STRINGS ARE NOT THE SAME LENGTH...
        public static string FromText(string text)
        {
            byte[] unicode_bytes = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream(unicode_bytes);
            return FromStream(ms);
        }

#region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            string[] filenames = Directory.GetFiles(@"C:\temp");
            foreach (string filename in filenames)
            {
                string hash1, hash2;

                try
                {
                    Logging.Info(".NET");
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
                    {
                        hash2 = FromStream_DOTNET(fs);
                    }
                    Logging.Info("Bouncy");
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
                    {
                        hash1 = FromStream_BOUNCY(fs);
                    }

                    if (hash1 != hash2)
                    {
                        Logging.Error("Non matching hash for " + filename);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Exception for " + filename);
                }
            }
        }
#endif

#endregion
    }
}
