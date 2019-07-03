using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using Utilities;
using Utilities.Files;

namespace Qiqqa.UpgradePaths.V037To038
{
    class SQLiteUpgrade_LibraryDB
    {
        string base_path;
        string library_path;

        public SQLiteUpgrade_LibraryDB(string base_path)
        {
            this.base_path = base_path;
            this.library_path = base_path + "Qiqqa.library";

            // Copy a library into place...
            if (!File.Exists(library_path))
            {
                Logging.Warn("Library db does not exist so copying the template to {0}", library_path);
                string library_template_path = StartupDirectoryForQiqqa + @"DocumentLibrary\Library.Template.s3db";
                File.Copy(library_template_path, library_path);
            }
        }

        private string StartupDirectoryForQiqqa
        {
            get
            {
                return Application.StartupPath + @"\";
            }
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Pooling=True;Max Pool Size=10;Data Source=" + library_path);
        }

        internal void PutBlob(SQLiteConnection connection, SQLiteTransaction transaction, string fingerprint, string extension, byte[] data)
        {
            // Calculate the MD5 of this blobbiiiieeeeee
            string md5 = StreamMD5.FromBytes(data);
            using (var command = new SQLiteCommand("INSERT INTO LibraryItem(fingerprint, extension, md5, data) VALUES(@fingerprint, @extension, @md5, @data)", connection, transaction))
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
