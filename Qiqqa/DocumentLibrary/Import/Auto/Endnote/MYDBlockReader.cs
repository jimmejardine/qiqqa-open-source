using System;
using System.Collections.Generic;
using Utilities;

namespace Qiqqa.DocumentLibrary.Import.Auto.Endnote
{
    public static class MYDBlockReader
    {
        public static IEnumerable<byte[]> Blocks(MYDBinaryReader br)
        {
            while (br.Position < br.Length)
            {
                MYDBlock root_block = MYDBlock.Read(br);
                //root_block.DumpStats();

                long post_block_processing_return_offset = br.Position;

                // Only follow blocks that are a 'starting block'
                if (!root_block.is_block_first) continue;

                // Concatenate all blocks
                byte[] record_data = new byte[root_block.rec_len];
                {
                    MYDBlock block = root_block;
                    int record_data_i = 0;
                    while (true)
                    {
                        Array.Copy(block.data, 0, record_data, record_data_i, block.data_len);
                        record_data_i += block.data_len;
                        if (block.is_block_last) break;

                        if (0 == block.next_filepos) throw new GenericException("Expecting a file offset if we have more blocks!");
                        br.Seek(block.next_filepos);

                        //Logging.Info("Appending additional data from block at {0}", block.next_filepos);
                        block = MYDBlock.Read(br);
                        //block.DumpStats("\t");
                    }

                    if (record_data_i != root_block.rec_len)
                    {
                        throw new GenericException("Block building length {0} is different to expected length of {1}!", record_data_i, root_block.rec_len);
                    }
                }

                br.Seek(post_block_processing_return_offset);

                yield return record_data;
            }
        }
    }
}

/*

  MI_MIN_BLOCK_LENGTH   20           
 MI_MAX_BLOCK_LENGTH   16777212      
 MI_DYN_ALIGN_SIZE     4             

 Part header[0] (decimal/hexadecimal, one byte): 

0/00: Deleted block
         block_len 3 bytes [1-3]
         next_filepos 8 bytes [4-11]
         prev_filepos 8 bytes [12-19]
         => header length 20
1/01: Full small record, full block
         rec_len,data_len,block_len 2 bytes [1-2]
         => header length 3
2/02: Full big record, full block
         rec_len,data_len,block_len 3 bytes [1-3]
         => header length 4
3/03: Full small record, unused space
         rec_len,data_len 2 bytes [1-2]
         unused_len 1 byte [3]
         => header length 4
4/04: Full big record, unused space
         rec_len,data_len 3 bytes [1-3]
         unused_len 1 byte [4]
         => header length 5
5/05: Start small record
         rec_len 2 bytes [1-2]
         data_len,block_len 2 bytes [3-4]
         next_filepos 8 bytes [5-12]
         => header length 13
6/06: Start big record
         rec_len 3 bytes [1-3]
         data_len,block_len 3 bytes [4-6]
         next_filepos 8 bytes [7-14]
         => header length 15
7/07: End small record, full block
         data_len,block_len 2 bytes [1-2]
         => header length 3
8/08: End big record, full block
         data_len,block_len 3 bytes [1-3]
         => header length 4
9/09: End small record, unused space
         data_len 2 bytes [1-2]
         unused_len 1 byte [3]
         => header length 4
10/0A: End big record, unused space
         data_len 3 bytes [1-3]
         unused_len 1 byte [4]
         => header length 5
11/0B: Continue small record
         data_len,block_len 2 bytes [1-2]
         next_filepos 8 bytes [3-10]
         => header length 11
12/0C: Continue big record
         data_len,block_len 3 bytes [1-3]
         next_filepos 8 bytes [4-11]
         => header length 12
13/0D: Start giant record
         rec_len 4 bytes [1-4]
         data_len,block_len 3 bytes [5-7]
         next_filepos 8 bytes [8-15]
         => header length 16

*/
