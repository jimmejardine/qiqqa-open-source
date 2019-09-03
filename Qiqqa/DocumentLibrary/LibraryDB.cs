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
    public static class DBAccessLock
    {
        //
        // This lock doesn't solve the lockup-on-connect() issues I'm having, but we keep it for 
        // safety's sake - better safe than sorry. I suspect we're suffering from
        // https://stackoverflow.com/questions/12532729/sqlite-keeps-the-database-locked-even-after-the-connection-is-closed
        // (Note that the *explicit* Close() call inside those `using (...)` blocks seems to alleviate
        // matters quite a bit, but isn't the end-all as lockup is still happening, though rarely.
        // 
        public static object db_access_lock = new object();
    }

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

        private static readonly char[] queryWildcards = { '*', '?', '%', '_' };

        private string turnArgumentIntoQueryPart(string key, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                if (value.IndexOfAny(queryWildcards) >= 0)
                {
                    value = value
                        .Replace('*', '%')
                        .Replace('?', '_')
                        // and for query safety:
                        .Replace('\'', '_');
                    return String.Format(" AND {0}='{1}'", key, value);
                }
                else
                {
                    return String.Format(" AND {0}=@{0}", key);
                }
            }
            return "";
        }

        private void turnArgumentIntoQueryParameter(SQLiteCommand command, string key, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                if (value.IndexOfAny(queryWildcards) < 0)
                {
                    command.Parameters.AddWithValue("@" + key, value);
                }
            }
        }

        public void PutString(string fingerprint, string extension, string data)
        {
            byte[] data_bytes = Encoding.UTF8.GetBytes(data);
            PutBlob(fingerprint, extension, data_bytes);
        }

        public void PutBlob(string fingerprint, string extension, byte[] data)
        {
            // Guard
            if (String.IsNullOrWhiteSpace(fingerprint))
            {
                throw new Exception("Can't store in LibraryDB with null fingerprint.");
            }
            if (String.IsNullOrWhiteSpace(extension))
            {
                throw new Exception("Can't store in LibraryDB with null extension.");
            }

            // Calculate the MD5 of this blobbiiiieeeeee
            string md5 = StreamMD5.FromBytes(data);

            lock (DBAccessLock.db_access_lock)
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
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

                        transaction.Commit();
                    }
                    connection.Close();
                }

                //
                // see SO link above at the `DBAccessLock.db_access_lock` declaration.
                //
                // We keep this *inside* the critical section so that we know we'll be the only active SQLite
                // action which just transpired.
                // *This* is also the reason why I went with a *global* lock (singeton) for *all* databases,
                // even while *theoretically* this is *wrong* or rather: *unneccessary* as the databases
                // i.e. Qiqqa Libraries shouldn't bite one another. I, however, need to ensure that the
                // added `System.Data.SQLite.SQLiteConnection.ClearAllPools();` statements don't foul up
                // matters in another library while lib A I/O is getting cleaned up.
                //
                // In short: Yuck. + Cave canem.
                //
                SQLiteConnection.ClearAllPools();
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


        public List<LibraryItem> GetLibraryItems(string fingerprint, string extension, int MaxRecordCount = 0)
        {
            List<LibraryItem> results = new List<LibraryItem>();

            lock (DBAccessLock.db_access_lock)
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    string command_string = "SELECT fingerprint, extension, md5, data FROM LibraryItem WHERE 1=1 ";
                    command_string += turnArgumentIntoQueryPart("fingerprint", fingerprint);
                    command_string += turnArgumentIntoQueryPart("extension", extension);
                    if (MaxRecordCount > 0)
                    {
                        // http://www.sqlitetutorial.net/sqlite-limit/
                        command_string += " LIMIT @maxnum";
                    }

                    using (var command = new SQLiteCommand(command_string, connection))
                    {
                        turnArgumentIntoQueryParameter(command, "fingerprint", fingerprint);
                        turnArgumentIntoQueryParameter(command, "extension", extension);
                        if (MaxRecordCount > 0)
                        {
                            command.Parameters.AddWithValue("@maxnum", MaxRecordCount);
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
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

                            reader.Close();
                        }
                    }

                    connection.Close();
                }

                //
                // see SO link above at the `DBAccessLock.db_access_lock` declaration.
                //
                // We keep this *inside* the critical section so that we know we'll be the only active SQLite
                // action which just transpired.
                // *This* is also the reason why I went with a *global* lock (singeton) for *all* databases,
                // even while *theoretically* this is *wrong* or rather: *unneccessary* as the databases
                // i.e. Qiqqa Libraries shouldn't bite one another. I, however, need to ensure that the
                // added `System.Data.SQLite.SQLiteConnection.ClearAllPools();` statements don't foul up
                // matters in another library while lib A I/O is getting cleaned up.
                //
                // In short: Yuck. + Cave canem.
                //
                SQLiteConnection.ClearAllPools();
            }

            return results;
        }
    }
}
