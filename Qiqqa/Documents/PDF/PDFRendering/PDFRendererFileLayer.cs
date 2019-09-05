using System;
using System.IO;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.Files;

namespace Qiqqa.Documents.PDF.PDFRendering
{
    public class PDFRendererFileLayer
    {
        public static readonly string BASE_PATH_DEFAULT = ConfigurationManager.Instance.BaseDirectoryForQiqqa + @"ocr\";

        static PDFRendererFileLayer()
        {
            Directory.CreateDirectory(BASE_PATH_DEFAULT);
        }

        string fingerprint;
        string pdf_filename;

        int num_pages;

        public PDFRendererFileLayer(string fingerprint, string pdf_filename)
        {
            this.fingerprint = fingerprint;
            this.pdf_filename = pdf_filename;
            this.num_pages = CountPDFPages();
        }

        public static bool HasOCRdata(string fingerprint)
        {
            string cached_count_filename = MakeFilename_PageCount(fingerprint);
            
            return File.Exists(cached_count_filename);
        }

        private static string BasePath
        {
            get
            {
                string folder_override = ConfigurationManager.Instance.ConfigurationRecord.System_OverrideDirectoryForOCRs;
                if (!String.IsNullOrEmpty(folder_override))
                {
                    return folder_override;
                }
                else
                {
                    return BASE_PATH_DEFAULT;
                }
            }
        }

        private static string MakeFilename(string fingerprint, string file_type, object token, string extension)
        {
            return String.Format("{0}{1}.{2}.{3}.{4}", BasePath, fingerprint, file_type, token, extension);
        }

        private static string MakeFilenameWith2LevelIndirection(string fingerprint, string file_type, object token, string extension)
        {
            string indirection_characters = fingerprint.Substring(0, 2);
            return String.Format("{0}\\{1}\\{2}.{3}.{4}.{5}", BasePath, indirection_characters, fingerprint, file_type, token, extension);
        }

        internal static string MakeFilename_TextSingle(string fingerprint, int page_number)
        {
            return MakeFilenameWith2LevelIndirection(fingerprint, "text", page_number, "txt");
        }
        internal string MakeFilename_TextSingle(int page_number)
        {
            return MakeFilename_TextSingle(fingerprint, page_number);
        }

        internal static string MakeFilename_TextGroup(string fingerprint, int page, int TEXT_PAGES_PER_GROUP)
        {
            int page_range_start = ((page-1) / TEXT_PAGES_PER_GROUP) * TEXT_PAGES_PER_GROUP + 1;
            int page_range_end = page_range_start + TEXT_PAGES_PER_GROUP - 1;
            string page_range = string.Format("{0:000}_to_{1:000}", page_range_start, page_range_end);
            return MakeFilenameWith2LevelIndirection(fingerprint, "textgroup", page_range, "txt");
        }
        internal string MakeFilename_TextGroup(int page, int TEXT_PAGES_PER_GROUP)
        {
            return MakeFilename_TextGroup(fingerprint, page, TEXT_PAGES_PER_GROUP);
           }

                private static string MakeFilename_PageCount(string fingerprint)
        {
            return MakeFilenameWith2LevelIndirection(fingerprint, "pagecount", "0", "txt");
        }
        
        internal string StorePageTextSingle(int page, string source_filename)
        {
            string filename = MakeFilename_TextSingle(fingerprint, page);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.Copy(source_filename, filename, true);
            File.Delete(source_filename);
            return filename;
        }

        internal string StorePageTextGroup(int page, int TEXT_PAGES_PER_GROUP, string source_filename)
        {
            string filename = MakeFilename_TextGroup(fingerprint, page, TEXT_PAGES_PER_GROUP);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.Copy(source_filename, filename, true);
            File.Delete(source_filename);
            return filename;
        }

        public int PageCount
        {
            get
            {
                return num_pages;
            }
        }

        private int CountPDFPages()
        {
            {
                string cached_count_filename = MakeFilename_PageCount(fingerprint);

                // Try the cached version
                try
                {
                    if (File.Exists(cached_count_filename))
                    {
                        int num_pages = Convert.ToInt32(File.ReadAllText(cached_count_filename));
                        if (0 != num_pages)
                        {
                            return num_pages;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem loading the cached page count.");
                    FileTools.Delete(cached_count_filename);
                }
            }

            // If we get here, either the pagecountfile doesn't exist, or there was an exception
            {
                string cached_count_filename = MakeFilename_PageCount(fingerprint);

                Logging.Debug("Using calculated PDF page count for file {0}", pdf_filename);
                int num_pages = PDFTools.CountPDFPages(pdf_filename);
                if (0 != num_pages)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(cached_count_filename));
                    File.WriteAllText(cached_count_filename, Convert.ToString(num_pages, 10));
                }
                return num_pages;
            }
        }
    }
}
