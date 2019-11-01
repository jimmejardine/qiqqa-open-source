using Utilities;

namespace Qiqqa.DocumentLibrary.Import.Auto.Endnote
{
    public class MYDBlock
    {
        public long start_position;
        public byte header;
        public int rec_len;
        public int data_len;
        public int block_len;
        public int next_filepos;
        public bool is_block_first;
        public bool is_block_last;
        public bool is_block_deleted;

        public byte[] data;

        public void DumpStats(string indent = "")
        {
            Logging.Info("{5}pos={4}\t header={0}\t rec_len={1}\t data_len={2}\t block_len={3}", header, rec_len, data_len, block_len, start_position, indent);
        }

        internal static MYDBlock Read(MYDBinaryReader br)
        {
            MYDBlock block = new MYDBlock();

            // Debugging purposes
            block.start_position = br.Position;

            // Start of header
            block.header = br.ReadByte();

            block.rec_len = 0;
            block.data_len = 0;
            block.block_len = 0;
            block.next_filepos = 0;

            // See http://dev.mysql.com/doc/internals/en/layout-record-storage-frame.html
            // Or bottom of this code
            switch (block.header)
            {
                case 0:
                    block.block_len = br.Read3();
                    block.block_len -= 4; // Deleted blocks include the header in their total length...sigh.
                    block.is_block_deleted = true;
                    break;

                case 1:
                    block.rec_len = block.data_len = block.block_len = br.Read2();
                    block.is_block_first = block.is_block_last = true;
                    break;

                case 2:
                    block.rec_len = block.data_len = block.block_len = br.Read3();
                    block.is_block_first = block.is_block_last = true;
                    break;

                case 13:
                    block.rec_len = br.Read4();
                    block.data_len = block.block_len = br.Read3();
                    block.next_filepos = br.Read8();
                    block.is_block_first = true;
                    break;

                case 3:
                    block.rec_len = block.data_len = br.Read2();
                    block.block_len = block.rec_len + br.Read1();
                    block.is_block_first = block.is_block_last = true;
                    break;

                case 4:
                    block.rec_len = block.data_len = br.Read3();
                    block.block_len = block.rec_len + br.Read1();
                    block.is_block_first = block.is_block_last = true;
                    break;

                case 5:
                    block.rec_len = br.Read2();
                    block.data_len = block.block_len = br.Read2();
                    block.next_filepos = br.Read8();
                    block.is_block_first = true;
                    break;

                case 6:
                    block.rec_len = br.Read3();
                    block.data_len = block.block_len = br.Read3();
                    block.next_filepos = br.Read8();
                    block.is_block_first = true;
                    break;

                case 7:
                    block.data_len = block.block_len = br.Read2();
                    block.is_block_last = true;
                    break;

                case 8:
                    block.data_len = block.block_len = br.Read3();
                    block.is_block_last = true;
                    break;

                case 9:
                    block.data_len = br.Read2();
                    block.block_len = block.data_len + br.Read1();
                    block.is_block_last = true;
                    break;

                case 10:
                    block.data_len = br.Read3();
                    block.block_len = block.rec_len + br.Read1();
                    block.is_block_last = true;
                    break;

                case 11:
                    block.data_len = block.block_len = br.Read2();
                    block.next_filepos = br.Read8();
                    break;

                case 12:
                    block.data_len = block.block_len = br.Read3();
                    block.next_filepos = br.Read8();
                    break;

                default:
                    break;
            }


            block.data = br.ReadBytes(block.block_len);

            return block;
        }
    }
}
