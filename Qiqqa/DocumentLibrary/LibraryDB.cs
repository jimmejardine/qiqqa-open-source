using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.Files;

namespace Qiqqa.DocumentLibrary
{
    public class LibraryDB
    {
        string base_path;
        string library_path;

        public LibraryDB(string base_path)
        {
            this.base_path = base_path;
            this.library_path = base_path + "Qiqqa.library";

            // Copy a library into place...
            if (!File.Exists(library_path))
            {
                Logging.Warn("Library db does not exist so copying the template to {0}", library_path);
                string library_template_path = ConfigurationManager.Instance.StartupDirectoryForQiqqa + @"DocumentLibrary\Library.Template.s3db";
                File.Copy(library_template_path, library_path);
            }
        }

        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Pooling=True;Max Pool Size=3;Data Source=" + library_path);
        }

        public void PutString(string fingerprint, string extension, string data)
        {
            byte[] data_bytes = Encoding.UTF8.GetBytes(data);
            PutBlob(fingerprint, extension, data_bytes);
        }

        public void PutBlob(string fingerprint, string extension, byte[] data)
        {
            // Guard
            if (String.IsNullOrEmpty(fingerprint))
            {
                throw new Exception("Can't store in LibraryDB with null fingerprint.");
            }
            if (String.IsNullOrEmpty(extension))
            {
                throw new Exception("Can't store in LibraryDB with null extension.");
            }

            // Calculate the MD5 of this blobbiiiieeeeee
            string md5 = StreamMD5.FromBytes(data);

            using (var connection = GetConnection())
            {
                connection.Open();

                bool managed_update = false;

                using (var command = new SQLiteCommand("UPDATE LibraryItem SET MD5=@md5, DATA=@data WHERE fingerprint=@fingerprint AND extension=@extension", connection))
                {
                    command.Parameters.AddWithValue("@md5", md5);
                    command.Parameters.AddWithValue("@data", data);
                    command.Parameters.AddWithValue("@fingerprint", fingerprint);
                    command.Parameters.AddWithValue("@extension", extension);
                    int num_rows_updated = command.ExecuteNonQuery();
                    if (1 == num_rows_updated)
                    {
                        managed_update = true;
                    }
                }

                if (!managed_update)
                {
                    using (var command = new SQLiteCommand("INSERT INTO LibraryItem(fingerprint, extension, md5, data) VALUES(@fingerprint, @extension, @md5, @data)", connection))
                    {
                        command.Parameters.AddWithValue("@fingerprint", fingerprint);
                        command.Parameters.AddWithValue("@extension", extension);
                        command.Parameters.AddWithValue("@md5", md5);
                        command.Parameters.AddWithValue("@data", data);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public class LibraryItem
        {
            public string fingerprint;
            public string extension;
            public byte[] data;
            public string md5;

            public override string ToString()
            {
                return string.Format("{0}.{1}", fingerprint, extension);
            }

            internal string ToFileNameFormat()
            {
                return string.Format("{0}.{1}", fingerprint, extension);
            }
        }

        public LibraryItem GetLibraryItem(string fingerprint, string extension)
        {
            List<LibraryItem> items = GetLibraryItems(fingerprint, extension);

            if (0 == items.Count)
            {
                throw new Exception(String.Format("We were expecting one item matching {0}.{1} but found none.", fingerprint, extension));
            }            
            if (1 != items.Count)
            {
                throw new Exception(String.Format("We were expecting only one item matching {0}.{1}", fingerprint, extension));
            }

            return items[0];
        }

        public Dictionary<string, byte[]> GetLibraryItemsAsCache(string extension)
        {
            // Make sure we are selecting only one type from the database...
            if (null == extension)
            {
                throw new Exception("Can not build cache off a non-specialised extension");
            }

            List<LibraryItem> library_items_annotations = GetLibraryItems(null, extension);
            Dictionary<string, byte[]> library_items_annotations_cache = new Dictionary<string, byte[]>();
            library_items_annotations.ForEach(o => library_items_annotations_cache.Add(o.fingerprint, o.data));

            return library_items_annotations_cache;
        }
        
        
        public List<LibraryItem> GetLibraryItems(string fingerprint, string extension)
        {
            List<LibraryItem> results = new List<LibraryItem>();

            using (var connection = GetConnection())
            {
                connection.Open();

                string command_string = "SELECT fingerprint, extension, md5, data FROM LibraryItem WHERE 1=1 ";
                if (null != fingerprint) command_string = command_string + " AND fingerprint=@fingerprint";
                if (null != extension) command_string = command_string + " AND extension=@extension";

                using (var command = new SQLiteCommand(command_string, connection))
                {
                    if (null != fingerprint) command.Parameters.AddWithValue("@fingerprint", fingerprint);
                    if (null != extension) command.Parameters.AddWithValue("@extension", extension);

                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        LibraryItem result = new LibraryItem();
                        results.Add(result);

                        result.fingerprint = reader.GetString(0);
                        result.extension = reader.GetString(1);
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
    }
}
