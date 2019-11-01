using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utilities.Files
{
    public class StreamMD5
    {
        public static string FromStream(Stream stream)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                md5.ComputeHash(stream);

                StringBuilder buff = new StringBuilder();
                foreach (byte hash_byte in md5.Hash)
                {
                    buff.Append(String.Format("{0:X2}", hash_byte));
                }

                return buff.ToString();
            }
        }

        public static string FromFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 32 * 1024))
            {
                return FromStream(fs);
            }
        }

        public static string FromText(string text)
        {
            byte[] unicode_bytes = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream(unicode_bytes);
            return FromStream(ms);
        }

        public static string FromBytes(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            return FromStream(ms);
        }
    }
}