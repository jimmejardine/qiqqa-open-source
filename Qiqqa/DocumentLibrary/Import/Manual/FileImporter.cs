using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities;
using Utilities.Files;

namespace Qiqqa.DocumentLibrary.Import.Manual
{

    /// <summary>
    /// Contains core logic to handle file importing and return BibTeX entries 
    /// </summary>
    public abstract class FileImporter
    {
        protected List<BibTeXEntry> Entries;
        protected readonly Library ImportLibrary;
        public bool InputFileAppearsToBeWrongFormat { get; protected set; }
        protected string ExportFileName { get; private set; }
        private readonly string _exportDirectory;

        /// <summary>
        /// ctor is responsible for parsing the file
        /// </summary>
        protected FileImporter(Library library, string fileName)
        {
            Entries = new List<BibTeXEntry>();
            ImportLibrary = library;
            ExportFileName = fileName;
            _exportDirectory = Path.GetDirectoryName(fileName);
        }

        /// <summary>
        /// This method is responsible for processing the contents. 
        /// </summary>
        /// <returns></returns>
        public abstract ParseFileResult GetResult();

        /// <summary>
        /// Removes invalid files, calculates fingerprints, determines if vanilla, and checks if fingerprint already in library. 
        /// </summary>
        protected ParseFileResult CreateFinalResult()
        {
            List<BibTeXEntry> finalEntries = new List<BibTeXEntry>();
            foreach (var entry in Entries)
            {
                try
                {
                    // Assume vanilla to start
                    entry.IsVanilla = true;
                    entry.Fingerprint = null;

                    if (entry.FileType != null && entry.FileType.Equals("pdf") && !String.IsNullOrEmpty(entry.Filename))
                    {
                        if (!File.Exists(entry.Filename))
                        {
                            // Perhaps it's a relative reference instead (like Qiqqa's export)
                            try
                            {
                                string speculativeRelativeFn = Path.Combine(_exportDirectory, entry.Filename);
                                if (File.Exists(speculativeRelativeFn)) entry.Filename = speculativeRelativeFn;
                            }
                            catch
                            {
                                
                            }
                        }

                        if (File.Exists(entry.Filename))
                        {
                            entry.Fingerprint = StreamFingerprint.FromFile(entry.Filename);

                            if (!String.IsNullOrEmpty(entry.Fingerprint))
                            {
                                // Definitely has a valid file...
                                entry.IsVanilla = false;
                            }
                        }
                    }else
                    {
                        // If file could not be found, ensure it's blanked out so we don't try to import it. This is particularly import w.r.t filenames with funny characters, where the import will choke. 
                        entry.Filename = entry.FileType = null;
                    }

                    finalEntries.Add(entry);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex);

                    // TODO log /status manager.  
                    // Ignore problems with individual files. 
                }
            }

            Entries = finalEntries;

            foreach (var entry in Entries)
            {
                if (entry.IsVanilla)
                {
                    entry.ExistsInLibrary = ImportLibrary.DocumentExistsInLibraryWithBibTeX(entry.BibTexRecord.Key);
                }
                else
                {
                    entry.ExistsInLibrary = ImportLibrary.DocumentExistsInLibraryWithFingerprint(entry.Fingerprint);
                }
            }

            int vanillaEntriesCount = Entries.Where(x => x.IsVanilla).Count(); 
            
            return new ParseFileResult(Entries, vanillaEntriesCount);
        }
    }
}


