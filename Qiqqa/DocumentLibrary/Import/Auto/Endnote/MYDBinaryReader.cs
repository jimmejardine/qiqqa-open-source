using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qiqqa.DocumentLibrary.Import.Auto.Endnote
{
    public class MYDBinaryReader
    {
        Stream stream;

        public MYDBinaryReader(Stream stream)
        {
            this.stream = stream;
        }

        public byte[] ReadBytes(int n)
        {
            byte[] buffer = new byte[n];
            stream.Read(buffer, 0, n);
            return buffer;
        }

        public byte ReadByte()
        {
            return ReadBytes(1)[0];
        }

        public int Read8()
        {
            byte[] bytes = ReadBytes(8);
            int total = 0;
            for (int i = 0; i < 8; ++i)
            {
                total = total * 256 + bytes[i];
            }

            return total;
        }

        public int Read4()
        {
            byte[] bytes = ReadBytes(4);
            return bytes[3] + 256 * bytes[2] + 256 * 256 * bytes[1] + 256 * 256 * 256 * bytes[0];
        }

        public int Read3()
        {
            byte[] bytes = ReadBytes(3);
            return bytes[2] + 256 * bytes[1] + 256 * 256 * bytes[0];
        }

        public int Read2()
        {
            byte[] bytes = ReadBytes(2);
            return bytes[1] + 256 * bytes[0];
        }

        public int Read1()
        {
            byte[] bytes = ReadBytes(1);
            return bytes[0];
        }


        public long Position
        {
            get { return stream.Position; }
        }

        public long Length
        {
            get { return stream.Length; }
        }

        internal void Seek(long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
        }
    }
}
