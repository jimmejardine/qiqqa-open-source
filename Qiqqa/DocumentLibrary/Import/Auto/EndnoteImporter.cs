using Ionic.Zip;
using Microsoft.Win32;
using Newtonsoft.Json;
using Qiqqa.DocumentLibrary.Import.Auto.Endnote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Strings;

namespace Qiqqa.DocumentLibrary.Import.Auto
{
    public class EndnoteImporter
    {
        public class EndnoteDatabaseDetails
        {
            public int databases_found = 0;
            public int documents_found = 0;
            public int pdfs_found = 0;

            public List<FilenameWithMetadataImport> metadata_imports = new List<FilenameWithMetadataImport>();

            public string PotentialImportMessage
            {
                get
                {
                    return
                        String.Format(
                        "Qiqqa has detected {2} EndNote™ database(s) on your computer.  Qiqqa can automatically import {0} references, {1} of which have associated PDFs."
                        , this.documents_found
                        , this.pdfs_found
                        , this.databases_found
                    );
                }
            }
        }


        private static List<string> GetRecentEndnoteDatabases()
        {
            List<string> databases = new List<string>();
            
            // Go through all the recent Endnote databases
            try
            {
                using (RegistryKey key_software = Registry.CurrentUser.OpenSubKey("Software", false))
                {
                    if (null != key_software)
                        using (RegistryKey key_isi = key_software.OpenSubKey("ISI ResearchSoft", false))
                        {
                            if (null != key_isi)
                                using (RegistryKey key_endnote = key_isi.OpenSubKey("EndNote", false))
                                {
                                    if (null != key_endnote)
                                        using (RegistryKey key_recent_libraries = key_endnote.OpenSubKey("Recent Libraries", false))
                                        {
                                            if (null != key_recent_libraries)
                                                foreach (string value_name in key_recent_libraries.GetValueNames())
                                                {
                                                    string filename = key_recent_libraries.GetValue(value_name) as string;
                                                    if (!String.IsNullOrEmpty(filename))
                                                    {
                                                        if (File.Exists(filename))
                                                        {
                                                            databases.Add(filename);
                                                        }
                                                    }
                                                }
                                        }
                                }
                        }
                }
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem getting the recent Endnote databases.");
            }

            return databases;
        }

        
        internal static EndnoteDatabaseDetails DetectEndnoteDatabaseDetails()
        {
            EndnoteDatabaseDetails edd = new EndnoteDatabaseDetails();

            foreach (string endnote_database_filename in GetRecentEndnoteDatabases())
            {
                try
                {
                    Logging.Info("Reading Endnote database '{0}'", endnote_database_filename);
                    using (MemoryStream ms = MYDDatabase.OpenMYDDatabase(endnote_database_filename))
                    {
                        ++edd.databases_found;

                        MYDBinaryReader br = new MYDBinaryReader(ms);
                        foreach (MYDRecord record in MYDRecordReader.Records(br))
                        {
                            try
                            {
                                FilenameWithMetadataImport fwmi = ConvertEndnoteToFilenameWithMetadataImport(endnote_database_filename, record);
                                edd.metadata_imports.Add(fwmi);
                                
                                // Update statistics
                                ++edd.documents_found;
                                if (null != fwmi.filename)
                                {
                                    ++edd.pdfs_found;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logging.Warn(ex, "Problem processing Endnote record.");
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    Logging.Warn(ex, "Exception while reading Endnote database '{0}'", endnote_database_filename);
                }
            }

            return edd;
        }

        private static FilenameWithMetadataImport ConvertEndnoteToFilenameWithMetadataImport(string endnote_database_filename, MYDRecord record)
        {
            BibTexItem bibtex_item = new BibTexItem();

            string type = "article";
            TransformType(record.reference_type, ref type);
            bibtex_item.Type = type;
            bibtex_item.Key = BibTexTools.GenerateRandomBibTeXKey();

            foreach (var pair in record.fields)
            {
                string key = pair.Key;
                string value = pair.Value;

                TransformKeyValue(record.reference_type, ref key, ref value);

                if ("notes" == key) continue;
                if ("keywords" == key) continue;
                if ("link_to_pdf" == key) continue;                

                bibtex_item[key] = value;
            }

            FilenameWithMetadataImport fwmi = new FilenameWithMetadataImport();
            fwmi.tags.Add("import_endnote");
            fwmi.tags.Add("import_endnote_" + Path.GetFileNameWithoutExtension(endnote_database_filename));
            fwmi.bibtex = bibtex_item.ToBibTex();
            
            if (record.fields.ContainsKey("notes"))
            {
                fwmi.notes = record.fields["notes"];
            }

            if (record.fields.ContainsKey("keywords"))
            {
                string keywords = record.fields["keywords"];
                string[] tags = keywords.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                fwmi.tags.AddRange(tags);
            }
            
            // Handle the attachments
            if (record.fields.ContainsKey("link_to_pdf"))
            {
                string links_string = record.fields["link_to_pdf"];

                fwmi.suggested_download_source_uri = links_string;

                string[] links = links_string.Split(new string[] { ",", "internal-pdf://", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                // Build up the list of candidates
                string base_directory = endnote_database_filename.Substring(0, endnote_database_filename.Length - 4) + ".Data\\PDF\\";
                List<string> pdf_links = new List<string>();

                    // First candidates are those in the subdirectory corresponding to the .ENL file
                    foreach (string link in links)
                    {
                        pdf_links.Add(base_directory + link);
                    }

                    // Second candidates are raw pathnames
                    foreach (string link in links)
                    {
                        pdf_links.Add(link);
                    }

                // Use the first PDF file that exists in the file system
                foreach (string pdf_link in pdf_links)
                {
                    if (pdf_link.ToLower().EndsWith(".pdf") && File.Exists(pdf_link))
                    {
                        fwmi.filename = pdf_link;
                        break;
                    }
                }
            }

            return fwmi;
        }


        private static void TransformType(int reference_type, ref string type)
        {
            switch (reference_type)
            {
                default:

                    type = "article";
                    break;
            }
        }

        private static void TransformKeyValue(int reference_type, ref string key, ref string value)
        {
            if ("secondary_title" == key) key = "booktitle";
            if ("tertiary_title" == key) key = "institution";
            if ("secondary_author" == key) key = "editor";
            if ("tertiary_author" == key) key = "institution";
            if ("place_published" == key) key = "address";

            // Convert authors from line-separated to 'and'-separated
            if ("author" == key)
            {
                string[] authors = value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                value = StringTools.ConcatenateStrings(authors, " and ");
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            //Test(@"D:\Quantisle\trunk\QiqqaClient\Scratchpad\EndnoteImportTests\refs2.enl");
            //Test(@"D:\Quantisle\trunk\QiqqaClient\Scratchpad\EndnoteImportTests\eigen.enl");

            //Test(@"C:\temp\attachment.enl");
            //Test(@"C:\temp\fieldnames.enl");

            EndnoteDatabaseDetails edd = DetectEndnoteDatabaseDetails();
        }
        
        public static void Test(string enl_filename)
        {
            Logging.Info("Importing from EndNote");

            Dictionary<int, List<string>> record_types = new Dictionary<int, List<string>>();
            Dictionary<string, HashSet<string>> field_utilisations = new Dictionary<string, HashSet<string>>();
            {
                for (int i = 0; i < EndnoteConstants.FIELD_NAMES.Length; ++i)
                {
                    field_utilisations[EndnoteConstants.FIELD_NAMES[i]] = new HashSet<string>();                    
                }
            }

            // Then read each row
            MemoryStream ms = MYDDatabase.OpenMYDDatabase(enl_filename);
            MYDBinaryReader br = new MYDBinaryReader(ms);

            foreach (MYDRecord record in MYDRecordReader.Records(br))
            {
                if (!record_types.ContainsKey(record.reference_type)) record_types[record.reference_type] = new List<string>();

                if (record.fields.ContainsKey("title"))
                { 
                record_types[record.reference_type].Add(record.fields["title"]);
                }

                foreach (var pair in record.fields)
                {
                    field_utilisations[pair.Key].Add(pair.Value);
                }
            }

            foreach (var pair in field_utilisations)
            {
                string sb = "";
                sb += Convert.ToString(pair.Key);
                sb += "\t --> ";
                foreach (var name in pair.Value)
                {
                    sb += name;
                    sb += " ";
                }

                Logging.Info("{0}", sb);
            }

            foreach (var pair in record_types)
            {
                string sb = "";
                sb += Convert.ToString(pair.Key);
                sb += "\t --> ";
                foreach (var name in pair.Value)
                {
                    sb += name;
                    sb += " ";
                }

                Logging.Info("{0}", sb);
            }
        }
#endif

        #endregion
    }
}



