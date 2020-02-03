using System;
using System.Collections.Generic;
using System.IO;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.DateTimeTools;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.RecentlyReadStuff
{
    public class RecentlyReadManager
    {
        private TypedWeakReference<Library> library;
        public Library Library => library?.TypedTarget;

        public RecentlyReadManager(Library library)
        {
            this.library = new TypedWeakReference<Library>(library);
        }

        public string Filename_Store => Path.GetFullPath(Path.Combine(Library.LIBRARY_BASE_PATH, @"Qiqqa.recently_read"));

        public void AddRecentlyRead(PDFDocument pdf_document)
        {
            using (StreamWriter sr = new StreamWriter(Filename_Store, append: true))
            {
                sr.WriteLine(
                    "{0},{1},{2}",
                    Guid.NewGuid().ToString(),
                    DateFormatter.asYYYYMMDDHHMMSS(DateTime.UtcNow),
                    pdf_document.Fingerprint
                    );
            }
        }

        public List<DateTime> GetRecentlyReadDates()
        {
            List<DateTime> results = new List<DateTime>();

            try
            {
                if (!File.Exists(Filename_Store)) return results;
                using (StreamReader sr = new StreamReader(Filename_Store))
                {
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line))
                        {
                            break;
                        }

                        string[] line_splits = line.Split(',');
                        DateTime read_date = DateFormatter.FromYYYYMMDDHHMMSS(line_splits[1]);
                        results.Add(read_date);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem with getting the recently read documents.");
            }

            return results;
        }
    }
}
