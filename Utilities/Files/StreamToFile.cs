using System;
using System.IO;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.Files
{
    public class StreamToFile
    {
        public static void CopyIntToStream(int input, Stream output)
        {
            uint input_clean = (uint)input;
            output.WriteByte((byte)((input_clean >> 24) & 0xff));
            output.WriteByte((byte)((input_clean >> 16) & 0xff));
            output.WriteByte((byte)((input_clean >> 8) & 0xff));
            output.WriteByte((byte)((input_clean >> 0) & 0xff));
        }

        public static int ReadIntFromStream(Stream input)
        {
            uint result = 0;

            for (int i = 0; i < 4; ++i)
            {
                int next = input.ReadByte();
                if (-1 == next)
                {
                    throw new Exception("End of stream occurred when trying to read an int");
                }

                byte next_byte = (byte)next;

                result = (result << 8) | next_byte;
            }

            return (int)result;
        }

        public static void CopyStreamToBuffer(Stream input, byte[] buffer, int bytes_to_read)
        {
            int bytes_read = 0;

            while (bytes_to_read > 0)
            {
                int bytes_read_this_time = input.Read(buffer, bytes_read, bytes_to_read);
                if (0 == bytes_read_this_time)
                {
                    throw new Exception(String.Format("End of stream encountered when still expecting {0} bytes", bytes_to_read));
                }

                bytes_read += bytes_read_this_time;
                bytes_to_read -= bytes_read_this_time;
            }
        }

        public static int CopyBufferToStream(Stream output, byte[] buffer, int bytes_to_write)
        {
            using (MemoryStream ms = new MemoryStream(buffer, 0, bytes_to_write, false))
            {
                return CopyStreamToStream(ms, output);
            }
        }

        public static int CopyStreamToStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[256 * 1024];
            int read;
            int total_read = 0;

            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                total_read += read;
                output.Write(buffer, 0, read);
            }

            return total_read;
        }

        public static string CopyStreamToString(Stream input)
        {
            using (StreamReader sr = new StreamReader(input))
            {
                return sr.ReadToEnd();
            }
        }

        public static int CopyStreamToFile(Stream input, string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                return CopyStreamToStream(input, fs);
            }
        }

        public static byte[] CopyStreamToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                CopyStreamToStream(input, ms);
                return ms.ToArray();
            }
        }
    }
}
