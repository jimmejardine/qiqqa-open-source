using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Utilities;
using Utilities.Files;

namespace Qiqqa.DocumentLibrary.IntranetLibraryStuff
{
    //
    // See ../LibraryDB.cs for the db_access_lock class and comments re the SQLite lockup issues.
    //
    // Also note that this source code file is a near *copy* of that source file: ../LibraryDB.cs !
    //



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

        public void PutString(string filename, string data)
        {
            byte[] data_bytes = Encoding.UTF8.GetBytes(data);
            PutBlob(filename, data_bytes);
        }

        public void PutBlob(string filename, byte[] data)
        {
            // Guard
            if (String.IsNullOrWhiteSpace(filename))
            {
                throw new Exception("Can't store in LibraryDB with null filename.");
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
                    connection.Close();
                }

                //
                // see SO link in ../LibraryDB.cs at the `DBAccessLock.db_access_lock` declaration.
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

            lock (DBAccessLock.db_access_lock)
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    string command_string = "SELECT filename, last_updated_by, md5, data FROM LibraryItem WHERE 1=1 ";
                    command_string += turnArgumentIntoQueryPart("filename", filename);

                    using (var command = new SQLiteCommand(command_string, connection))
                    {
                        turnArgumentIntoQueryParameter(command, "filename", filename);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
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

        public List<IntranetLibraryItem> GetIntranetLibraryItemsSummary()
        {
            List<IntranetLibraryItem> results = new List<IntranetLibraryItem>();

            lock (DBAccessLock.db_access_lock)
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    string command_string = "SELECT filename, md5 FROM LibraryItem WHERE 1=1 ";

                    using (var command = new SQLiteCommand(command_string, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                IntranetLibraryItem result = new IntranetLibraryItem();
                                results.Add(result);

                                result.filename = reader.GetString(0);
                                result.md5 = reader.GetString(1);
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
