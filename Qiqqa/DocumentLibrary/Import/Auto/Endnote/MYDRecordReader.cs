using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utilities;
using Utilities.Strings;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.Import.Auto.Endnote
{
    internal class MYDRecordReader
    {
        public static IEnumerable<MYDRecord> Records(MYDBinaryReader br)
        {
            foreach (byte[] record_data in MYDBlockReader.Blocks(br))
            {
                yield return ProcessRecord(record_data);
            }
        }

        private static MYDRecord ProcessRecord(byte[] record_data)
        {
            //DumpRecord(record_data);
            //return;

            using (MemoryStream ms = new MemoryStream(record_data))
            {
                MYDBinaryReader br = new MYDBinaryReader(ms);
                {
                    int NULLIES = 7;

                    // Read the nullyinfo bits
                    byte[] nulls_bytes = br.ReadBytes(NULLIES);
                    string nulls = "";
                    for (int i = 0; i < NULLIES; ++i)
                    {
                        nulls = Convert.ToString(nulls_bytes[i], 2).PadLeft(8, '0') + nulls;
                    }
                    nulls = StringTools.Reverse(nulls);

                    // Read the id
                    int id = -1;
                    if ((nulls_bytes[0] & 0x01) == 0x00)
                    {
                        byte[] id_bytes = br.ReadBytes(4);
                        id = 1 * id_bytes[0] + 256 * id_bytes[1] + 256 * 256 * id_bytes[2] + 256 * 256 * 256 * id_bytes[3];
                    }

                    // Read the reference type
                    int reference_type = -1;
                    if ((nulls_bytes[0] & 0x02) == 0x00)
                    {
                        byte[] reference_type_bytes = br.ReadBytes(2);
                        reference_type = 1 * reference_type_bytes[0] + 256 * reference_type_bytes[1];
                    }

                    // Read the varfields
                    Dictionary<string, string> datas = new Dictionary<string, string>();
                    for (int field = 2; field < 54; ++field)
                    {
                        if (nulls[field] == '0')
                        {
                            byte[] data_length_bytes = br.ReadBytes(3);
                            int data_length = 1 * data_length_bytes[0] + 256 * data_length_bytes[1] + 256 * 256 * data_length_bytes[2];
                            byte[] data_bytes = br.ReadBytes(data_length);
                            string data = Encoding.ASCII.GetString(data_bytes);
                            datas[EndnoteConstants.FIELD_NAMES[field]] = data;
                        }
                    }

                    MYDRecord myd_record = new MYDRecord();
                    myd_record.id = id;
                    myd_record.reference_type = reference_type;
                    myd_record.fields = datas;
                    return myd_record;
                }
            }
        }


        private static void DumpRecord(byte[] record_data)
        {
            using (MemoryStream ms = new MemoryStream(record_data))
            {
                MYDBinaryReader br = new MYDBinaryReader(ms);
                {
                    int NULLIES = 20;

                    // Read the nullyinfo bits
                    byte[] nulls_bytes = br.ReadBytes(NULLIES);
                    string binary = "";
                    string hex = "";
                    string ascii = "";
                    for (int i = 0; i < NULLIES; ++i)
                    {
                        binary = binary + Convert.ToString(nulls_bytes[i], 2).PadLeft(8, '0') + " ";
                        hex = hex + String.Format("{0:X2}", nulls_bytes[i]);
                        ascii = ascii + ((nulls_bytes[i] < 32) ? "." : ASCIIEncoding.ASCII.GetString(nulls_bytes, i, 1));
                    }

                    Logging.Info("{0} {1} {2}", ascii, hex, binary);
                }
            }
        }
    }
}

/*

CREATE TABLE refs
(
    id    INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,

    reference_type    SMALLINT(5) UNSIGNED NOT NULL,
    text_styles    MEDIUMTEXT NOT NULL,
    author    MEDIUMTEXT NOT NULL,
    year    MEDIUMTEXT NOT NULL,
    title    MEDIUMTEXT NOT NULL,
    pages    MEDIUMTEXT NOT NULL,
    secondary_title    MEDIUMTEXT NOT NULL,
    volume    MEDIUMTEXT NOT NULL,
    number    MEDIUMTEXT NOT NULL,
    number_of_volumes    MEDIUMTEXT NOT NULL,

    secondary_author    MEDIUMTEXT NOT NULL,
    place_published    MEDIUMTEXT NOT NULL,
    publisher    MEDIUMTEXT NOT NULL,
    subsidiary_author    MEDIUMTEXT NOT NULL,
    edition    MEDIUMTEXT NOT NULL,
    keywords    MEDIUMTEXT NOT NULL,
    type_of_work    MEDIUMTEXT NOT NULL,
    date    MEDIUMTEXT NOT NULL,
    abstract    MEDIUMTEXT NOT NULL,
    label    MEDIUMTEXT NOT NULL,

    url    MEDIUMTEXT NOT NULL,
    tertiary_title    MEDIUMTEXT NOT NULL,
    tertiary_author    MEDIUMTEXT NOT NULL,
    notes    MEDIUMTEXT NOT NULL,
    isbn    MEDIUMTEXT NOT NULL,
    custom_1    MEDIUMTEXT NOT NULL,
    custom_2    MEDIUMTEXT NOT NULL,
    custom_3    MEDIUMTEXT NOT NULL,
    custom_4    MEDIUMTEXT NOT NULL,
    alternate_title    MEDIUMTEXT NOT NULL,

    accession_number    MEDIUMTEXT NOT NULL,
    call_number    MEDIUMTEXT NOT NULL,
    short_title    MEDIUMTEXT NOT NULL,
    custom_5    MEDIUMTEXT NOT NULL,
    custom_6    MEDIUMTEXT NOT NULL,
    section    MEDIUMTEXT NOT NULL,
    original_publication    MEDIUMTEXT NOT NULL,
    reprint_edition    MEDIUMTEXT NOT NULL,
    reviewed_item    MEDIUMTEXT NOT NULL,
    author_address    MEDIUMTEXT NOT NULL,

    image    MEDIUMTEXT NOT NULL,
    caption    MEDIUMTEXT NOT NULL,
    custom_7    MEDIUMTEXT NOT NULL,
    electronic_resource_number    MEDIUMTEXT NOT NULL,
    link_to_pdf    MEDIUMTEXT NOT NULL,
    translated_author    MEDIUMTEXT NOT NULL,
    translated_title    MEDIUMTEXT NOT NULL,
    name_of_database    MEDIUMTEXT NOT NULL,
    database_provider    MEDIUMTEXT NOT NULL,
    research_notes    MEDIUMTEXT NOT NULL,

    language    MEDIUMTEXT NOT NULL,
    access_date    MEDIUMTEXT NOT NULL,
    last_modified_date    MEDIUMTEXT NOT NULL,

    PRIMARY KEY(id),
    INDEX yearIndex(year)
) ENGINE = MYISAM;

*/


