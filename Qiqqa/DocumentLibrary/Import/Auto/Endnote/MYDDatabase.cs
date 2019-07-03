using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qiqqa.DocumentLibrary.Import.Auto.Endnote
{
    class MYDDatabase
    {
        public static MemoryStream OpenMYDDatabase(string enl_filename)
        {
            MemoryStream ms = new MemoryStream();
            {
                using (ZipFile zip_file = ZipFile.Read(enl_filename))
                {
                    foreach (ZipEntry zip_entry in zip_file.Entries)
                    {
                        if (zip_entry.FileName.ToLower().EndsWith(".myd"))
                        {
                            using (var reader = zip_entry.OpenReader())
                            {
                                reader.CopyTo(ms);
                            }
                        }
                    }
                }

                ms.Seek(0, SeekOrigin.Begin);
            }

            return ms;
        }
    }
}
