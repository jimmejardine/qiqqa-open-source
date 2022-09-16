using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.IntranetLibraryStuff
{
    //
    // See ../LibraryDB.cs for the db_access_lock class and comments re the SQLite lockup issues.
    //
    // Also note that this source code file is a near *copy* of that source file: ../LibraryDB.cs !
    //

    public class IntranetLibraryDB
    {
        private string base_path;
        private string library_path;

        public IntranetLibraryDB(string base_path)
        {
            this.base_path = base_path;
            library_path = IntranetLibraryTools.GetLibraryMetadataPath(base_path);

            // Copy a library into place...
            if (!File.Exists(library_path))
            {
                Logging.Warn("Intranet Library metadata db does not exist so copying the template to {0}", library_path);
                string library_metadata_template_path = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"DocumentLibrary/IntranetLibraryStuff/IntranetLibrary.Metadata.Template.s3db"));
                if (!File.Exists(library_metadata_template_path))
                {
                    throw new Exception($"Sync template file '{library_metadata_template_path}' does not exist!");
                }
                string basedir = Path.GetDirectoryName(library_path);
                if (!Directory.Exists(basedir))
                {
                    throw new Exception($"Sync target directory '{basedir}' for Qiqqa database '{library_path}' does not exist!");
                }
                try
                {
                    File.Copy(library_metadata_template_path, library_path);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error 0x{2:08X}: Failed to write the sync template '{0}' to sync target directory '{1}'", library_metadata_template_path, basedir, ex.HResult);
                    throw;
                }
            }
        }

        private SQLiteConnection GetConnection()
        {
            SQLiteConnection connection = new SQLiteConnection("Pooling=True;Max Pool Size=3;Data Source=" + library_path);

            // Turn on extended result codes
            connection.SetExtendedResultCodes(true);

            return connection;
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

            try
            {
                lock (DBAccessLock.db_access_lock)
                {
                    using (var connection = GetConnection())
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            bool managed_update = false;

                            using (var command = new SQLiteCommand("UPDATE LibraryItem SET MD5=@md5, DATA=@data, LAST_UPDATED_BY=@last_updated_by WHERE filename=@filename", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@filename", filename);
                                command.Parameters.AddWithValue("@last_updated_by", Environment.UserName);
                                command.Parameters.AddWithValue("@md5", md5);
                                command.Parameters.AddWithValue("@data", data);
                                int num_rows_updated = command.ExecuteNonQuery();
                                if (1 == num_rows_updated)
                                {
                                    managed_update = true;
                                }
                            }

                            if (!managed_update)
                            {
                                using (var command = new SQLiteCommand("INSERT INTO LibraryItem(filename, last_updated_by, md5, data) VALUES(@filename, @last_updated_by, @md5, @data)", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@filename", filename);
                                    command.Parameters.AddWithValue("@last_updated_by", Environment.UserName);
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
                    // see SO link in ../LibraryDB.cs at the `DBAccessLock.db_access_lock` declaration.
                    //
                    // We keep this *inside* the critical section so that we know we'll be the only active SQLite
                    // action which just transpired.
                    // *This* is also the reason why I went with a *global* lock (singleton) for *all* databases,
                    // even while *theoretically* this is *wrong* or rather: *unnecessary* as the databases
                    // i.e. Qiqqa Libraries shouldn't bite one another. I, however, need to ensure that the
                    // added `System.Data.SQLite.SQLiteConnection.ClearAllPools();` statements don't foul up
                    // matters in library B while lib A I/O is getting cleaned up.
                    //
                    // In short: Yuck. + Cave canem.
                    //
                    SQLiteConnection.ClearAllPools();
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "IntranetLibraryDB::PutBLOB: Database I/O failure for DB '{0}'.", library_path);
                LibraryDB.FurtherDiagnoseDBProblem(ex, null, library_path);
                throw;
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

        public List<IntranetLibraryItem> GetIntranetLibraryItems(string filename, int MaxRecordCount = 0)
        {
            List<IntranetLibraryItem> results = new List<IntranetLibraryItem>();
            List<Exception> database_corruption = new List<Exception>();

            try
            {
                lock (DBAccessLock.db_access_lock)
                {
                    using (var connection = GetConnection())
                    {
                        connection.Open();

                        string command_string = "SELECT filename, last_updated_by, md5, data FROM LibraryItem WHERE 1=1 ";
                        command_string += turnArgumentIntoQueryPart("filename", filename);
                        if (MaxRecordCount > 0)
                        {
                            // http://www.sqlitetutorial.net/sqlite-limit/
                            command_string += " LIMIT @maxnum";
                        }

                        using (var command = new SQLiteCommand(command_string, connection))
                        {
                            turnArgumentIntoQueryParameter(command, "filename", filename);
                            if (MaxRecordCount > 0)
                            {
                                command.Parameters.AddWithValue("@maxnum", MaxRecordCount);
                            }

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    IntranetLibraryItem result = new IntranetLibraryItem();
                                    results.Add(result);

                                    long total_bytes = 0;

                                    try
                                    {
                                    result.filename = reader.GetString(0);
                                    result.last_updated_by = reader.GetString(1);
                                    result.md5 = reader.GetString(2);

                                        total_bytes = reader.GetBytes(3, 0, null, 0, 0);
                                        result.data = new byte[total_bytes];
                                        long total_bytes2 = reader.GetBytes(3, 0, result.data, 0, (int)total_bytes);
                                        if (total_bytes != total_bytes2)
                                        {
                                            throw new Exception("Error reading blob - blob size different on each occasion.");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string msg = String.Format("IntranetLibraryDB::GetLibraryItems: Database record #{4} decode failure for DB '{0}': filename_id={1}, last_updated_by={2}, md5={3}, length={5}.",
                                            library_path,
                                            String.IsNullOrEmpty(result.filename) ? "???" : result.filename,
                                            String.IsNullOrEmpty(result.last_updated_by) ? "???" : result.last_updated_by,
                                            String.IsNullOrEmpty(result.md5) ? "???" : result.md5,
                                            reader.StepCount, // ~= results.Count + database_corruption.Count
                                            total_bytes
                                            );
                                        Logging.Error(ex, "{0}", msg);

                                        Exception ex2 = new Exception(msg, ex);

                                        database_corruption.Add(ex2);
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
                    // *This* is also the reason why I went with a *global* lock (singleton) for *all* databases,
                    // even while *theoretically* this is *wrong* or rather: *unnecessary* as the databases
                    // i.e. Qiqqa Libraries shouldn't bite one another. I, however, need to ensure that the
                    // added `System.Data.SQLite.SQLiteConnection.ClearAllPools();` statements don't foul up
                    // matters in library B while lib A I/O is getting cleaned up.
                    //
                    // In short: Yuck. + Cave canem.
                    //
                    SQLiteConnection.ClearAllPools();
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "IntranetLibraryDB::GetLibraryItems: Database I/O failure for DB '{0}'.", library_path);
                LibraryDB.FurtherDiagnoseDBProblem(ex, database_corruption, library_path);
                throw;
            }

            if (database_corruption.Count > 0)
            {
                // report database corruption: the user may want to recover from this ASAP!
                if (MessageBoxes.AskErrorQuestion(true, "INTRANET Library (Sync Point) '{0}' has some data corruption. Do you want to abort the application to attempt recovery using external tools, e.g. a data restore from backup?\n\nWhen you answer NO, we will continue with what we could recover so far instead.\n\n\nConsult the Qiqqa logfiles to see the individual corruptions reported.",
                    library_path))
                {
                    Logging.Warn("User chose to abort the application on database corruption report");
                    Environment.Exit(3);
                }
            }

            return results;
        }

        public List<IntranetLibraryItem> GetIntranetLibraryItemsSummary()
        {
            List<IntranetLibraryItem> results = new List<IntranetLibraryItem>();
            List<Exception> database_corruption = new List<Exception>();

            try
            {
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

                                    try
                                    {
                                    result.filename = reader.GetString(0);
                                    result.md5 = reader.GetString(1);
                                    }
                                    catch (Exception ex)
                                    {
                                        string msg = String.Format("IntranetLibraryDB::GetIntranetLibraryItemsSummary: Database record #{3} decode failure for DB '{0}': filename_id={1}, md5={2}.",
                                            library_path,
                                            String.IsNullOrEmpty(result.filename) ? "???" : result.filename,
                                            String.IsNullOrEmpty(result.md5) ? "???" : result.md5,
                                            reader.StepCount // ~= results.Count + database_corruption.Count
                                            );
                                        Logging.Error(ex, "{0}", msg);

                                        Exception ex2 = new Exception(msg, ex);

                                        database_corruption.Add(ex2);
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
                    // *This* is also the reason why I went with a *global* lock (singleton) for *all* databases,
                    // even while *theoretically* this is *wrong* or rather: *unnecessary* as the databases
                    // i.e. Qiqqa Libraries shouldn't bite one another. I, however, need to ensure that the
                    // added `System.Data.SQLite.SQLiteConnection.ClearAllPools();` statements don't foul up
                    // matters in library B while lib A I/O is getting cleaned up.
                    //
                    // In short: Yuck. + Cave canem.
                    //
                    SQLiteConnection.ClearAllPools();
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "IntranetLibraryDB::GetLibraryItemsSummary: Database I/O failure for DB '{0}'.", library_path);
                LibraryDB.FurtherDiagnoseDBProblem(ex, database_corruption, library_path);
                throw;
            }

            if (database_corruption.Count > 0)
            {
                // report database corruption: the user may want to recover from this ASAP!
                if (MessageBoxes.AskErrorQuestion(true, "INTRANET Library (Sync Point) '{0}' has some data corruption. Do you want to abort the application to attempt recovery using external tools, e.g. a data restore from backup?\n\nWhen you answer NO, we will continue with what we could recover so far instead.\n\n\nConsult the Qiqqa logfiles to see the individual corruptions reported.",
                    library_path))
                {
                    Logging.Warn("User chose to abort the application on database corruption report");
                    Environment.Exit(3);
                }
            }

            return results;
        }
    }
}
