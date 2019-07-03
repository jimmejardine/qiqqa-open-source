using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.Files;

namespace Qiqqa.DocumentLibrary.IntranetLibraryStuff
{
    public class IntranetLibraryDB
    {
        string base_path;
        string library_path;

        public IntranetLibraryDB(string base_path)
        {
            this.base_path = base_path;
            this.library_path = IntranetLibraryTools.GetLibraryMetadataPath(base_path);

            // Copy a library into place...
            if (!File.Exists(library_path))
            {
                Logging.Warn("Intranet Library metadata db does not exist so copying the template to {0}", library_path);
                string library_metadata_template_path = ConfigurationManager.Instance.StartupDirectoryForQiqqa + @"DocumentLibrary\IntranetLibraryStuff\IntranetLibrary.Metadata.Template.s3db";
                File.Copy(library_metadata_template_path, library_path);
            }
        }

        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Pooling=True;Max Pool Size=3;Data Source=" + library_path);
        }

        public void PutString(string filename, string data)
        {
            byte[] data_bytes = Encoding.UTF8.GetBytes(data);
            PutBlob(filename, data_bytes);
        }

        public void PutBlob(string filename, byte[] data)
        {
            // Guard
            if (String.IsNullOrEmpty(filename))
            {
                throw new Exception("Can't store in LibraryDB with null filename.");
            }

            // Calculate the MD5 of this blobbiiiieeeeee
            string md5 = StreamMD5.FromBytes(data);

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new SQLiteCommand("DELETE FROM LibraryItem WHERE filename=@filename", connection, transaction))
                    {
                        command.Parameters.AddWithValue("@filename", filename);
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SQLiteCommand("INSERT INTO LibraryItem(filename, last_updated_by, md5, data) VALUES(@filename, @last_updated_by, @md5, @data)", connection, transaction))
                    {
                        command.Parameters.AddWithValue("@filename", filename);
                        command.Parameters.AddWithValue("@last_updated_by", Environment.UserName);
                        command.Parameters.AddWithValue("@md5", md5);
                        command.Parameters.AddWithValue("@data", data);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public class IntranetLibraryItem
        {
            public string filename;
            public string last_updated_by;
            public byte[] data;
            public string md5;

            public override string ToString()
            {
                return string.Format("{0}", filename);
            }

            internal string ToFileNameFormat()
            {
                return string.Format("{0}", filename);
            }
        }

        public IntranetLibraryItem GetIntranetLibraryItem(string filename)
        {
            List<IntranetLibraryItem> items = GetIntranetLibraryItems(filename);

            if (0 == items.Count)
            {
                throw new Exception(String.Format("We were expecting one item matching {0} but found none.", filename));
            }            
            if (1 != items.Count)
            {
                throw new Exception(String.Format("We were expecting only one item matching {0}", filename));
            }

            return items[0];
        }

        public List<IntranetLibraryItem> GetIntranetLibraryItems(string filename)
        {
            List<IntranetLibraryItem> results = new List<IntranetLibraryItem>();

            using (var connection = GetConnection())
            {
                connection.Open();

                string command_string = "SELECT filename, last_updated_by, md5, data FROM LibraryItem WHERE 1=1 ";
                if (null != filename) command_string = command_string + " AND filename=@filename";

                using (var command = new SQLiteCommand(command_string, connection))
                {
                    if (null != filename) command.Parameters.AddWithValue("@filename", filename);

                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        IntranetLibraryItem result = new IntranetLibraryItem();
                        results.Add(result);

                        result.filename = reader.GetString(0);
                        result.last_updated_by = reader.GetString(1);
                        result.md5 = reader.GetString(2);

                        long total_bytes = reader.GetBytes(3, 0, null, 0, 0);
                        result.data = new byte[total_bytes];
                        long total_bytes2 = reader.GetBytes(3, 0, result.data, 0, (int)total_bytes);
                        if (total_bytes != total_bytes2)
                        {
                            throw new Exception("Error reading blob - blob size different on each occasion.");
                        }
                    }
                }
            }

            return results;
        }

        public List<IntranetLibraryItem> GetIntranetLibraryItemsSummary()
        {
            List<IntranetLibraryItem> results = new List<IntranetLibraryItem>();

            using (var connection = GetConnection())
            {
                connection.Open();

                string command_string = "SELECT filename, md5 FROM LibraryItem WHERE 1=1 ";

                using (var command = new SQLiteCommand(command_string, connection))
                {
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        IntranetLibraryItem result = new IntranetLibraryItem();
                        results.Add(result);

                        result.filename = reader.GetString(0);
                        result.md5 = reader.GetString(1);
                    }
                }
            }

            return results;
        }
    }
}
