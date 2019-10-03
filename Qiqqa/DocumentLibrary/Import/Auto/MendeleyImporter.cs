using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;

namespace Qiqqa.DocumentLibrary.Import.Auto
{
    public class MendeleyImporter
    {
        public class MendeleyDatabaseDetails
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
                        "Qiqqa has detected Mendeley™ on your computer.  Qiqqa can automatically import {0} references, {1} of which have associated PDFs."
                        , this.documents_found
                        , this.pdfs_found
                    );
                }
            }
        }

        internal static MendeleyDatabaseDetails DetectMendeleyDatabaseDetails()
        {
            MendeleyDatabaseDetails mdd = new MendeleyDatabaseDetails();

            string BASE_DIR_FOR_MENDELEY_DATABASE = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Mendeley Ltd\Mendeley Desktop\";

            if (!Directory.Exists(BASE_DIR_FOR_MENDELEY_DATABASE))
            {
                Logging.Info("Mendeley not found.");
                mdd.databases_found = 0;
                mdd.documents_found = 0;
                mdd.pdfs_found = 0;
                return mdd;
            }

            try
            {
                string[] sqlite_filenames = Directory.GetFiles(BASE_DIR_FOR_MENDELEY_DATABASE, "*.sqlite", SearchOption.TopDirectoryOnly);
                foreach (string sqlite_filename in sqlite_filenames)
                {
                    // Skip the monitor database
                    if (sqlite_filename.EndsWith("monitor.sqlite")) continue;

                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + sqlite_filename))
                        {
                            connection.Open();

                            // Build the authors lookup
                            Dictionary<long, string> authors_lookup = new Dictionary<long, string>();
                            {
                                string command_string = "SELECT * FROM DocumentContributors";
                                using (var command = new SQLiteCommand(command_string, connection))
                                {
                                    SQLiteDataReader reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        long document_id = (long)reader["documentId"];
                                        string surname = reader["lastName"] as string;
                                        string firstnames = reader["firstNames"] as string;
                                        string compound_name = (String.IsNullOrEmpty(surname)) ? firstnames : (String.IsNullOrEmpty(firstnames) ? surname : (surname + ", " + firstnames));
                                        if (!String.IsNullOrEmpty(compound_name))
                                        {
                                            if (!authors_lookup.ContainsKey(document_id))
                                            {
                                                authors_lookup[document_id] = compound_name;
                                            }
                                            else
                                            {
                                                authors_lookup[document_id] = authors_lookup[document_id] + " AND " + compound_name;
                                            }
                                        }
                                    }
                                }
                            }

                            Dictionary<long, List<string>> tags_lookup = new Dictionary<long, List<string>>();
                            {
                                string command_string = "SELECT * FROM DocumentKeywords";
                                using (var command = new SQLiteCommand(command_string, connection))
                                {
                                    SQLiteDataReader reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        long document_id = (long)reader["documentId"];
                                        string keyword = reader["keyword"] as string;
                                        if (!String.IsNullOrEmpty(keyword))
                                        {
                                            if (!tags_lookup.ContainsKey(document_id))
                                            {
                                                tags_lookup[document_id] = new List<string>();
                                            }

                                            tags_lookup[document_id].Add(keyword);
                                        }
                                    }
                                }
                            }


                            // Get the bibtexes
                            {
                                //string command_string = "SELECT * FROM Documents WHERE 1=1 ";
                                string command_string =
                                    ""
                                    + "SELECT * "
                                    + "FROM Documents "
                                    + "LEFT OUTER JOIN DocumentFiles ON Documents.id == DocumentFiles.documentId "
                                    + "LEFT OUTER JOIN Files ON DocumentFiles.Hash = Files.Hash "
                                    ;

                                using (var command = new SQLiteCommand(command_string, connection))
                                {
                                    SQLiteDataReader reader = command.ExecuteReader();

                                    ++mdd.databases_found;

                                    while (reader.Read())
                                    {
                                        try
                                        {
                                            BibTexItem bibtex_item = new BibTexItem();

                                            bibtex_item.Type = reader["type"] as string;
                                            bibtex_item.Key = reader["citationKey"] as string;
                                            if (String.IsNullOrEmpty(bibtex_item.Key))
                                            {
                                                bibtex_item.Key = BibTexTools.GenerateRandomBibTeXKey();
                                            }

                                            PopulateTentativeField(bibtex_item, reader, "title");
                                            PopulateTentativeField(bibtex_item, reader, "abstract");
                                            PopulateTentativeField(bibtex_item, reader, "advisor");
                                            PopulateTentativeField(bibtex_item, reader, "city");
                                            PopulateTentativeField(bibtex_item, reader, "country");
                                            PopulateTentativeField(bibtex_item, reader, "day");
                                            PopulateTentativeField(bibtex_item, reader, "month");
                                            PopulateTentativeField(bibtex_item, reader, "dateAccessed", "accessed");
                                            PopulateTentativeField(bibtex_item, reader, "department");
                                            PopulateTentativeField(bibtex_item, reader, "doi");
                                            PopulateTentativeField(bibtex_item, reader, "edition");
                                            PopulateTentativeField(bibtex_item, reader, "institution");
                                            PopulateTentativeField(bibtex_item, reader, "isbn");
                                            PopulateTentativeField(bibtex_item, reader, "issn");
                                            PopulateTentativeField(bibtex_item, reader, "issue");
                                            PopulateTentativeField(bibtex_item, reader, "medium");
                                            PopulateTentativeField(bibtex_item, reader, "pages");
                                            PopulateTentativeField(bibtex_item, reader, "pmid");
                                            PopulateTentativeField(bibtex_item, reader, "publication");
                                            PopulateTentativeField(bibtex_item, reader, "publisher");
                                            PopulateTentativeField(bibtex_item, reader, "sections", "section");
                                            PopulateTentativeField(bibtex_item, reader, "series");
                                            PopulateTentativeField(bibtex_item, reader, "session");
                                            PopulateTentativeField(bibtex_item, reader, "volume");
                                            PopulateTentativeField(bibtex_item, reader, "year");

                                            long document_id = (long)reader["id"];
                                            if (authors_lookup.ContainsKey(document_id))
                                            {
                                                bibtex_item["author"] = authors_lookup[document_id];
                                            }

                                            FilenameWithMetadataImport fwmi = new FilenameWithMetadataImport();
                                            fwmi.tags.Add("import_mendeley");
                                            fwmi.bibtex = bibtex_item.ToBibTex();

                                            string filename = reader["localUrl"] as string;
                                            if (!String.IsNullOrEmpty(filename))
                                            {
                                                const string FILE_PREFIX = "file:///";
                                                if (filename.StartsWith(FILE_PREFIX))
                                                {
                                                    filename = filename.Substring(FILE_PREFIX.Length);
                                                }

                                                filename = Uri.UnescapeDataString(filename);
                                                filename = filename.Replace('/', '\\');

                                                fwmi.filename = filename;

                                                ++mdd.pdfs_found;
                                            }

                                            if (tags_lookup.ContainsKey(document_id))
                                            {
                                                fwmi.tags.AddRange(tags_lookup[document_id]);
                                            }

                                            string note = reader["note"] as string;
                                            if (!String.IsNullOrEmpty(note))
                                            {
                                                note = note.Replace("<m:italic>", "");
                                                note = note.Replace("</m:italic>", "");
                                                note = note.Replace("<m:bold>", "");
                                                note = note.Replace("</m:bold>", "");
                                                note = note.Replace("<m:note>", "");
                                                note = note.Replace("</m:note>", "");
                                                note = note.Replace("<m:underline>", "");
                                                note = note.Replace("</m:underline>", "");
                                                note = note.Replace("<m:right>", "");
                                                note = note.Replace("</m:right>", "");
                                                note = note.Replace("<m:center>", "");
                                                note = note.Replace("</m:center>", "");
                                                note = note.Replace("<m:linebreak/>", "\n");

                                                fwmi.notes = note;
                                            }

                                            mdd.metadata_imports.Add(fwmi);

                                            ++mdd.documents_found;
                                        }

                                        catch (Exception ex)
                                        {
                                            Logging.Error(ex, "Exception while extracting a Mendeley document.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "Exception while exploring for Mendeley instance in file '{0}'.", sqlite_filename);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Exception while exploring for Mendeley instances.");
            }

            Logging.Info("Got {0} libraries with {1} documents and {2} PDFs.", mdd.databases_found, mdd.documents_found, mdd.pdfs_found);

            return mdd;
        }

        private static void PopulateTentativeField(BibTexItem bibtex_item, SQLiteDataReader reader, string shared_key)
        {
            PopulateTentativeField(bibtex_item, reader, shared_key, shared_key);
        }

        private static void PopulateTentativeField(BibTexItem bibtex_item, SQLiteDataReader reader, string mendeley_key, string bibtex_key)
        {
            object value = reader[mendeley_key];
            if (DBNull.Value != value)
            {
                bibtex_item[bibtex_key] = Convert.ToString(value);
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            MendeleyDatabaseDetails mdd = DetectMendeleyDatabaseDetails();
        }
#endif

        #endregion
    }
}
