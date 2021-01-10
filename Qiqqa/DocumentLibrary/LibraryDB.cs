using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


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
        private string base_path;
        private string library_path;

        public LibraryDB(WebLibraryDetail web_library_detail)
        {
            base_path = web_library_detail.LIBRARY_BASE_PATH;
            library_path = LibraryDB.GetLibraryDBPath(base_path);
            string db_syncref_path = IntranetLibraryTools.GetLibraryMetadataPath(base_path);

            // Copy a library into place...
            // but only if this is not a Internet sync directory/DB!
            if (File.Exists(db_syncref_path))
            {
                throw new Exception(String.Format("MUST NOT attempt to create a regular Qiqqa library in the Qiqqa Internet/Intranet Sync directory: '{0}'", base_path));
            }
            if (!File.Exists(library_path))
            {
                Logging.Warn($"Library db for '{web_library_detail.Id}' does not exist so copying the template to '{library_path}'");
                string library_template_path = LibraryDB.GetLibraryDBTemplatePath();
                File.Copy(library_template_path, library_path);
            }
        }

        internal static string GetLibraryDBPath(string base_path)
        {
            return Path.GetFullPath(Path.Combine(base_path, @"Qiqqa.library"));
        }

        internal static string GetLibraryDBTemplatePath()
        {
            return Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"DocumentLibrary/Library.Template.s3db"));
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

                            using (var command = new SQLiteCommand("UPDATE LibraryItem SET MD5=@md5, DATA=@data WHERE fingerprint=@fingerprint AND extension=@extension", connection, transaction))
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
                                using (var command = new SQLiteCommand("INSERT INTO LibraryItem(fingerprint, extension, md5, data) VALUES(@fingerprint, @extension, @md5, @data)", connection, transaction))
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
                Logging.Error(ex, "LibraryDB::PutBLOB: Database I/O failure for DB '{0}'.", library_path);
                LibraryDB.FurtherDiagnoseDBProblem(ex, null, library_path);
                throw;
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
            List<Exception> database_corruption = new List<Exception>();

            try
            {
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

                                    int field_count = 0;

                                    // Read the record in 2-3 gangs, as there's some DBs out there which have the BLOB as NOT A BLOB but as a STRING type instead:
                                    // this is probably caused by manual editing (using SQLite CLI or other means) of the BLOB record.

                                    // gang 1: load the field count and header fields: these almost never fail.
                                    try
                                    {
                                        field_count = reader.FieldCount;

                                        result.fingerprint = reader.GetString(0);
                                        result.extension = reader.GetString(1);
                                        result.md5 = reader.GetString(2);
                                    }
                                    catch (Exception ex)
                                    {
                                        string msg = String.Format("LibraryDB::GetLibraryItems: Database record #{4} gang 1 decode failure for DB '{0}': fingerprint={1}, ext={2}, md5={3}, field_count={5}.",
                                            library_path,
                                            String.IsNullOrEmpty(result.fingerprint) ? "???" : result.fingerprint,
                                            String.IsNullOrEmpty(result.extension) ? "???" : result.extension,
                                            String.IsNullOrEmpty(result.md5) ? "???" : result.md5,
                                            reader.StepCount, // ~= results.Count + database_corruption.Count
                                            field_count
                                            );
                                        Logging.Error(ex, "{0}", msg);

                                        Exception ex2 = new Exception(msg, ex);

                                        database_corruption.Add(ex2);

                                        // it's no use to try to decode the rest of the DB record: it is lost to us
                                        continue;
                                    }

                                    {
                                        Exception ex2 = null;

                                        long total_bytes = 0;

                                        // gang 2: get the BLOB
                                        try
                                        {
                                            total_bytes = reader.GetBytes(3, 0, null, 0, 0);
                                            result.data = new byte[total_bytes];
                                            long total_bytes2 = reader.GetBytes(3, 0, result.data, 0, (int)total_bytes);
                                            if (total_bytes != total_bytes2)
                                            {
                                                throw new Exception("Error reading blob - blob size different on each occasion.");
                                            }

                                            results.Add(result);
                                            continue;
                                        }
                                        catch (Exception ex)
                                        {
                                            string msg = String.Format("LibraryDB::GetLibraryItems: Database record #{4} BLOB decode failure for DB '{0}': fingerprint={1}, ext={2}, md5={3}, BLOB length={5}.",
                                                library_path,
                                                String.IsNullOrEmpty(result.fingerprint) ? "???" : result.fingerprint,
                                                String.IsNullOrEmpty(result.extension) ? "???" : result.extension,
                                                String.IsNullOrEmpty(result.md5) ? "???" : result.md5,
                                                reader.StepCount, // ~= results.Count + database_corruption.Count
                                                total_bytes
                                                );

                                            ex2 = new Exception(msg, ex);

                                            // gang 3: get at the BLOB-née-STRING in an indirect way
                                            object[] fields = new object[5];

                                            try
                                            {
                                                reader.GetValues(fields);
                                                byte[] arr = fields[3] as byte[];
                                                if (arr != null)
                                                {
                                                    string blob = Encoding.UTF8.GetString(arr, 0, arr.Length);

                                                    result.data = new byte[arr.Length];
                                                    Array.Copy(arr, result.data, arr.Length);

                                                    results.Add(result);

                                                    Logging.Warn("LibraryDB::GetLibraryItems: Database record #{0} BLOB field is instead decoded as UTF8 STRING, following this RESOLVED ERROR: {1}\n  Decoded STRING content:\n{2}",
                                                reader.StepCount, // ~= results.Count + database_corruption.Count
                                                ex2.ToStringAllExceptionDetails(),
                                                blob);

                                                    continue;
                                                }
                                                else
                                                {
                                                    throw new Exception("Cannot extract BLOB field.");
                                                }
                                            }
                                            catch (Exception ex3)
                                            {
                                                Logging.Error(ex2);
                                                Logging.Error(ex3);

                                                database_corruption.Add(ex2);
                                            }
                                        }
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
                Logging.Error(ex, "LibraryDB::GetLibraryItems: Database I/O failure for DB '{0}'.", library_path);
                LibraryDB.FurtherDiagnoseDBProblem(ex, database_corruption, library_path);
                throw;
            }

            if (database_corruption.Count > 0)
            {
                // report database corruption: the user may want to recover from this ASAP!
                if (MessageBoxes.AskErrorQuestion(true, "Library '{0}' has some data corruption. Do you want to abort the application to attempt recovery using external tools, e.g. a data restore from backup?\n\nWhen you answer NO, we will continue with what we could recover so far instead.\n\n\nConsult the Qiqqa logfiles to see the individual corruptions reported.",
                    library_path))
                {
                    Logging.Warn("User chose to abort the application on database corruption report");
                    Environment.Exit(3);
                }
            }

            return results;
        }

        public static void FurtherDiagnoseDBProblem(Exception ex, List<Exception> corruption_list, string library_path)
        {
            SQLiteException sql_ex = ex as SQLiteException;

            // so we had a failure (or several)... now check the state of the database file and report on our findings:
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.AppendLine("--- Diagnosis for reported problem ---");
                sb.AppendLine("======================================");
                sb.AppendLine("");

                if (!File.Exists(library_path))
                {
                    sb.AppendLine($"--> The database file '{library_path}' does not exist.");
                    break;
                }

                bool looks_sane = true;
                bool is_readonly = false;

                // what are the access rights and size?
                try
                {
                    var info = File.GetFileSystemEntryInfo(library_path);

                    sb.AppendLine($"--> File Attributes:                                {info.Attributes}");
                    sb.AppendLine($"--> File Creation Date (UTC):                       {info.CreationTimeUtc}");
                    sb.AppendLine($"--> File Last Changed Date (UTC):                   {info.LastWriteTimeUtc}");
                    sb.AppendLine($"--> File Last Access Date (UTC):                    {info.LastAccessTimeUtc}");
                    sb.AppendLine($"--> Is marked as READ ONLY:                         {info.IsReadOnly}");
                    sb.AppendLine($"--> Is marked as OFFLINE:                           {info.IsOffline}");
                    sb.AppendLine($"--> Is marked as archived:                          {info.IsArchive}");
                    sb.AppendLine($"--> Is marked as HIDDEN:                            {info.IsHidden}");
                    sb.AppendLine($"--> Is a SYSTEM FILE:                               {info.IsSystem}");
                    sb.AppendLine($"--> Is encrypted by the operating system:           {info.IsEncrypted}");
                    sb.AppendLine($"--> Is compressed by the operating system:          {info.IsCompressed}");
                    sb.AppendLine($"--> Is a mount point:                               {info.IsMountPoint}");
                    sb.AppendLine($"--> Is temporary:                                   {info.IsTemporary}");
                    sb.AppendLine($"--> Is a symbolic link:                             {info.IsSymbolicLink}");
                    sb.AppendLine($"--> Is a sparse file:                               {info.IsSparseFile}");
                    sb.AppendLine($"--> Is a reparse point:                             {info.IsReparsePoint}");
                    sb.AppendLine($"--> Is not content indexed by the operating system: {info.IsNotContentIndexed}");
                    sb.AppendLine($"--> Is a directory:                                 {info.IsDirectory}");
                    sb.AppendLine($"--> Is a device:                                    {info.IsDevice}");
                    sb.AppendLine($"--> Is a normal file:                               {info.IsNormal}");
                    sb.AppendLine($"--> File size:                                      {info.FileSize} bytes");

                    is_readonly = info.IsReadOnly;

                    if (info.IsOffline || info.IsHidden || info.IsSystem || info.IsEncrypted || info.IsMountPoint || info.IsTemporary || info.IsSymbolicLink || info.IsSparseFile || info.IsReparsePoint || info.IsDirectory || info.IsDevice)
                    {
                        sb.AppendLine("");
                        sb.AppendLine("--> WARNING: this doesn't look like a very normal file.");
                        sb.AppendLine("    Check the attributes above to determine if they are as you expect.");
                        sb.AppendLine("");
                        looks_sane = false;
                    }
                }
                catch (Exception ex2)
                {
                    sb.AppendLine($"--> FAILED to collect the file attributes: {ex2.ToStringAllExceptionDetails()}");
                    looks_sane = false;
                }

                // Check if we can open the file for basic I/O:
                try
                {
                    // read the entire file into a 1M buffer. Watch for errors.
                    byte[] buf = new byte[1024 * 1024];
                    const int count = 1024 * 1024;
                    long length_read = 0;

                    using (var stream = File.OpenRead(library_path))
                    {
                        int rc;

                        while (true)
                        {
                            rc = stream.Read(buf, 0, count);
                            if (rc > 0)
                            {
                                length_read += rc;
                            }
                            else if (rc == 0)
                            {
                                // EOF
                                break;
                            }
                            else
                            {
                                throw new Exception($"stream.Read produced a negative result: {rc}");
                            }
                        }
                    }

                    long file_size = File.GetSize(library_path);

                    if (file_size != length_read)
                    {
                        throw new Exception($"stream.Read was unable to read/scan the entire file:\n      file size reported by the file system = {file_size} bytes, length scanned = {length_read} bytes");
                    }

                    sb.AppendLine($"--> Successfully scanned the entire file: length scanned = {length_read} bytes");
                }
                catch (Exception ex2)
                {
                    sb.AppendLine($"--> FAILED to read/scan the file: {ex2.ToStringAllExceptionDetails()}");
                    looks_sane = false;
                }

                if (!is_readonly)
                {
                    // check if we can open the file for WRITE access
                    try
                    {
                        using (var stream = File.OpenWrite(library_path))
                        {
                            sb.AppendLine($"--> Successfully opened the file for WRITE ACCESS");
                        }
                    }
                    catch (Exception ex2)
                    {
                        sb.AppendLine($"--> FAILED to open the file for WRITE ACCESS:);");
                        sb.AppendLine(ex2.ToStringAllExceptionDetails());
                        looks_sane = false;
                    }
                }

                if (corruption_list != null && corruption_list.Count > 0)
                {
                    if (looks_sane)
                    {
                        sb.AppendLine("--> WARNING: while the RAW file scan and access checks may have PASSED,");
                        sb.AppendLine("    the Qiqqa inner systems certainly found stuff in the file to complain about:");
                        sb.AppendLine("    these data corruptions (a.k.a. DECODE FAILURES) have already been reported,");
                        sb.AppendLine("    but here they are once more in summarized form:");
                    }
                    else
                    {
                        sb.AppendLine("--> ERROR: while the RAW file scan and access checks may have FAILED,");
                        sb.AppendLine("    the Qiqqa inner systems also found stuff in the file to complain about");
                        sb.AppendLine("    -- which is VERY PROBABLY related to or caused by the findings reported above.");
                        sb.AppendLine("");
                        sb.AppendLine("    The data corruptions (a.k.a. DECODE FAILURES) have already been reported,");
                        sb.AppendLine("    but here they are once more in summarized form:");
                    }
                    sb.AppendLine("");

                    int index = 1;
                    foreach (var corruption in corruption_list)
                    {
                        sb.AppendLine($"      #{index.ToString("03")}: {corruption.Message.Split('\n')[0]}");
                        index++;
                    }
                    sb.AppendLine($"      --- {corruption_list.Count} reported data corruptions ---------------------------------------");

                    looks_sane = false;
                }

                if (sql_ex != null)
                {
                    sb.AppendLine("");
                    sb.AppendLine("    As this report is about a SQLite database error, it MAY be useful to search");
                    sb.AppendLine("    the Internet for generic help and/or discussion of the reported SQLite error:");
                    sb.AppendLine("");
                    int errorcode = sql_ex.ErrorCode;
                    int basenum = errorcode & 0xFF;
                    int extended_shift = errorcode >> 8;
                    int herr = sql_ex.HResult;

                    sb.AppendLine("    ( ref: https://sqlite.org/rescode.html )");
                    sb.AppendLine("    ( ref: https://sqlite.org/c3ref/c_abort.html )");
                    sb.AppendLine($"    SQLite Error Code: {basenum}");
                    if (extended_shift != 0)
                    {
                        sb.AppendLine("    ( ref: https://sqlite.org/c3ref/c_abort_rollback.html )");
                        sb.AppendLine($"    SQLite Extended Error Code: ({basenum} | ({extended_shift} << 8))   = {errorcode}");
                    }
                    else
                    {
                        sb.AppendLine("    Reported error code is NOT a SQLite Extended Error Code.");
                    }
                    sb.AppendLine($"    SQLite HResult: {herr.ToString("08:X")}");
                    sb.AppendLine("");
                    sb.AppendLine($"      SQLite: the define constants (i.e. compile-time options): {SQLiteConnection.DefineConstants}");
                    sb.AppendLine($"      SQLite: the underlying SQLite core library: {SQLiteConnection.SQLiteVersion}");
                    sb.AppendLine($"      SQLite: SQLITE_SOURCE_ID: {SQLiteConnection.SQLiteSourceId}");
                    sb.AppendLine($"      SQLite: the compile-time options: {SQLiteConnection.SQLiteCompileOptions}");
                    sb.AppendLine($"      SQLite: the version of the interop SQLite assembly: {SQLiteConnection.InteropVersion}");
                    sb.AppendLine($"      SQLite: the unique identifier for the source checkout used to build the interop assembly: {SQLiteConnection.InteropSourceId}");
                    sb.AppendLine($"      SQLite: the compile-time options used to compile the SQLite interop assembly: {SQLiteConnection.InteropCompileOptions}");
                    sb.AppendLine($"      SQLite: the version of the managed components: {SQLiteConnection.ProviderVersion}");
                    sb.AppendLine($"      SQLite: the unique identifier for the source checkout used to build the managed components: {SQLiteConnection.ProviderSourceId}");
                    sb.AppendLine($"      SQLite: the extra connection flags: {SQLiteConnection.SharedFlags}");
                    sb.AppendLine($"      SQLite: the default connection flags: {SQLiteConnection.DefaultFlags}");
                    sb.AppendLine("");
                    sb.AppendLine("      (ref: https://sqlite.org/c3ref/extended_result_codes.html )");
                    sb.AppendLine("      SQLite Extended Error Reporting has been ENABLED: SetExtendedResultCodes(true)");
                }

                sb.AppendLine("---------");

                if (looks_sane)
                {
                    sb.AppendLine("");
                    sb.AppendLine("--> VERDICT OK(?): this DOES look like a very normal file.");
                    sb.AppendLine("");
                    sb.AppendLine("    HOWEVER, it may have incorrect a.k.a. 'corrupted' content, which made Qiqqa barf,");
                    sb.AppendLine("    or there's something else going on which this simply diagnosis routine");
                    sb.AppendLine("    is unable to unearth.");
                    sb.AppendLine("");
                    sb.AppendLine("    Please file a report at the Qiqqa issue tracker and include this logging");
                    sb.AppendLine("    for further inspection.");
                }
                else
                {
                    sb.AppendLine("");
                    sb.AppendLine("--> VERDICT BAD(?): as far as this simple diagnostic routine can tell,");
                    sb.AppendLine("    this is NOT an 'okay' file.");
                    sb.AppendLine("");
                    if (is_readonly)
                    {
                        sb.AppendLine("    It MAY be marked READ-ONLY, which MAY be okay in your book, but is certainly");
                        sb.AppendLine("    unexpected here.");
                        sb.AppendLine("");
                    }
                    sb.AppendLine("    There's something going on which this simply diagnosis routine CANNOT diagnose further.");
                    sb.AppendLine("");
                    sb.AppendLine("    Please file a report at the Qiqqa issue tracker and include this logging");
                    sb.AppendLine("    for further inspection.");
                }

                sb.AppendLine("");
                sb.AppendLine("==================== End of diagnostics report ============================================");
            } while (false);

            Logging.Error(sb.ToString());
        }
    }
}

