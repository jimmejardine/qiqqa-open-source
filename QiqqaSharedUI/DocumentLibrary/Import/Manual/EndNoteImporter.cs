using System;
using System.Collections.Generic;
using Qiqqa.UtilisationTracking;
using Utilities.BibTex;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.Import.Manual
{
    public class EndNoteImporter : FileImporter
    {
        private string _pdfRootDir;

        public EndNoteImporter(Library library, string exportFilename, string libraryDataDir)
            : base(library, exportFilename)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_ImportFromEndNote);

            _pdfRootDir = GetDocumentRootFolder(libraryDataDir);
        }

        public override ParseFileResult GetResult()
        {

            // Parse the endnote clump
            string endnote_text = File.ReadAllText(ExportFileName);
            List<EndNoteToBibTex.EndNoteRecord> endnote_records = EndNoteToBibTex.Parse(endnote_text);

            // Then process each one
            foreach (EndNoteToBibTex.EndNoteRecord endnote_record in endnote_records)
            {
                bool associated_filename_is_rooted = false; // By default they're relative to the import file directory

                // Check if there is an associated filename with this guy, use it.  Note that we stop at the FIRST associated PDF file...all the rest are ignored!
                string associated_filename = null;
                {
                    if (endnote_record.attributes.ContainsKey("%>"))
                    {
                        string filenames_combined = endnote_record.attributes["%>"][0];
                        string[] filenames = filenames_combined.Split('\n');
                        foreach (string filename in filenames)
                        {
                            string test_filename = filename;
                            test_filename = test_filename.ToLower();

                            // Looks like this: 
                            // internal-pdf://2020827050-1893725446/2020827050.pdf
                            if (test_filename.StartsWith("internal-pdf://") && test_filename.EndsWith(".pdf"))
                            {
                                associated_filename = test_filename.Substring("internal-pdf://".Length);
                                break;
                            }

                            //Or sometimes like this:
                            // file://F:\Manzotti\Stuff on Library\EL\MEL.Data\PDF\Bain - 2007 - The Southern Journal of Philosophy - Color Externalism and Switch Cases.pdf
                            if (test_filename.StartsWith("file://"))
                            {
                                associated_filename = test_filename.Substring("file://".Length);
                                associated_filename_is_rooted = true;
                            }
                        }
                    }
                }

                string notes = null;
                {
                    if (endnote_record.attributes.ContainsKey("%Z"))
                    {
                        notes = endnote_record.attributes["%Z"][0];
                    }
                }

                // Create our import entry
                BibTeXEntry bibtex_entry = new BibTeXEntry();
                bibtex_entry.Item = endnote_record.ToBibTeX();
                bibtex_entry.EntryType = bibtex_entry.Item.Type;
                bibtex_entry.BibTeX = bibtex_entry.Item.ToBibTex();
                bibtex_entry.Notes = notes;
                if (null != _pdfRootDir /* a valid root dir has been chosen */ && null != associated_filename)
                {
                    if (associated_filename_is_rooted)
                    {
                        bibtex_entry.Filename = associated_filename;
                    }
                    else
                    {
                        bibtex_entry.Filename = Path.Combine(_pdfRootDir, associated_filename);
                    }
                    bibtex_entry.FileType = "pdf";
                }

                Entries.Add(bibtex_entry);
            }


            var res = CreateFinalResult();

            if (endnote_text.Length > 0 && Entries.Count == 0)
            {
                //Perhaps they tried to import the endnote library file, as opposed to export...
                InputFileAppearsToBeWrongFormat = true;
            }

            return res;
        }

        public static bool ValidateDocumentRootFolder(string libraryDataDir)
        {
            return !String.IsNullOrEmpty(GetDocumentRootFolder(libraryDataDir));
        }

        private static string GetDocumentRootFolder(string libraryDataDir)
        {
            try
            {
                string pdfDir = Path.Combine(libraryDataDir, "PDF");
                if (!Directory.Exists(pdfDir)) return null;

                return pdfDir;
            }
            catch
            {
                return null;
            }
        }
    }
}
