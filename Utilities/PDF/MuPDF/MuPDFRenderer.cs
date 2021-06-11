using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Utilities.Files;
using Utilities.GUI;
using Utilities.ProcessTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Text;
using Utilities.Shutdownable;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.PDF.MuPDF
{
    public class PDFErrors
    {
        public const int PAGECOUNT_PENDING = -10;

        public const int PAGECOUNT_TOOL_MISSING = -6;                       // mutool missing?!?!
        public const int PAGECOUNT_DOCUMENT_IS_BAD_HTML_DOWNLOAD = -5;      // PDF file is a HTML file instead, probably due to a b0rked download
        public const int PAGECOUNT_DOCUMENT_TIMEOUT = -4;                   // PDF failed to deliver during the time alloted. Smells like Very Bad Juju in your PDF!
        public const int DOCUMENT_IS_CORRUPTED = -3;
        public const int DOCUMENT_DOES_NOT_EXIST = -2;
        public const int PAGECOUNT_GENERAL_FAILURE = -1;

        public static string ToString(int errcode)
        {
            switch (errcode)
            {
                case PAGECOUNT_PENDING:
                    return "<pending>";

                case PAGECOUNT_TOOL_MISSING:
                    return "<Tool MISSING?!>";

                case PAGECOUNT_DOCUMENT_IS_BAD_HTML_DOWNLOAD:
                    return "<bad HTML download?>";

                case PAGECOUNT_DOCUMENT_TIMEOUT:
                    return "<tool TIMEOUT>";

                case DOCUMENT_IS_CORRUPTED:
                    return "<corrupt doc>";

                case DOCUMENT_DOES_NOT_EXIST:
                    return "<file missing>";

                case PAGECOUNT_GENERAL_FAILURE:
                    return "<general fail>";

                default:
                    return null;
            }
        }
    }

    public class PDFDocumentMuPDFMetaInfo
    {
        public int PageCount = PDFErrors.PAGECOUNT_PENDING;
        public string PDFVersion;
        public int Chapters = -1;
        public List<int> ChapterPages;
        public string Title;
        public string Author;
        public string Format;
        public string Encryption;
        public string PDFCreator;
        public string PDFProducer;
        public string Subject;
        public string Keywords;
        public DateTime? Creation_Date = null;
        public DateTime? Modification_Date = null;
        public string Permissions;
        public string PDFStatus;
        public int DocumentUpdateCount = -1;
        public string ChangeHistoryValidation;
        public int FormFieldSignaturesCount = 0;
        public string UpdatesStatus;
        public bool WasRepaired;
        public bool NeedsPassword;
        public bool WasCryptedWithEmptyPassword;
        public bool HasDocumentOutlines;
        public int AttachedFilesCount = 0;
        public int EmbeddedJavaScriptFilesCount = 0;

        public bool DocumentIsCorrupted = false;
        public int DocumentErrorCode = 0;

        // ---- RAW content + error feedback items: --------------------------------------------------

        /// <summary>
        /// JSON decode errors + ...
        /// </summary>
        public List<string> errors = new List<string>();
        /// <summary>
        /// the data as produced by `mutool metadump`
        /// </summary>
        public string raw_metadump_text = null;
        /// <summary>
        /// the `mutool metadump` data, as decoded by the JSON parser a.k.a. deserializer
        /// </summary>
        public List<MultiPurpDocumentInfoObject> raw_decoded_json = new List<MultiPurpDocumentInfoObject>();

        /// <summary>
        /// Erase the raw content to help save heap space.
        ///
        /// This will nuke the `raw_metadump_text` and `raw_decoded_json` members.
        /// This will also *clear* the `errors` list.
        /// </summary>
        public void ClearRawContent()
        {
            errors.Clear();
            raw_metadump_text = null;
            raw_decoded_json = null;
        }
    }

    // MuPDF mutool metadump JSON output classes ------------------------------------------------------------------------------------------------------------------------------------------------

    public class MultiPurpGatheredErrors
    {
        public string Log;
        public bool? LogOverflow;
    }

    public class MultiPurpTargetCoordinates
    {
        public float X;
        public float Y;
    }

    public class MultiPurpDocumentOutline
    {
        public string InternalLink;
        public int? TargetPageNumber;
        public MultiPurpTargetCoordinates TargetCoordinates;
        public string ExternalLink;
        public string Title;
        public bool? IsOpen;
        public List<MultiPurpDocumentOutline> Children;
    }

    public class MultiPurpAttachedJavascriptFile
    {
        public string S; /* 'JavaScript' */
        public string JS; /* the JS source code */
    }

    public class MultiPurpAttachedJavascriptFiles
    {
        public List<MultiPurpAttachedJavascriptFile> Names;
    }

    public class MultiPurpAttachedEmbeddedFile
    {
        public bool? IsEmbeddedFile;
        public string EmbeddedFileType;
        public string EmbeddedFileName;
        public string UF;
        public int? EmbeddedFileIndex;
        public object CI;
        public string F;
        public object EF;
        public string FileSubtype;
        public int? EmbedLength;
        public string EmbedFilter;
        public int? FileDataLength;
        public string FileCreationDate;
        public string FileModDate;
        public int? FileSize;
        public string FileCheckSum;
        public string FileDesc;
        public string FileRecType;
    }

    public class MultiPurpAttachedEmbeddedFiles
    {
        public List<MultiPurpAttachedEmbeddedFile> Names;
    }

    public class MultiPurpAttachedFiles
    {
        public object AP;
        public MultiPurpAttachedJavascriptFiles JavaScript;
        public MultiPurpAttachedEmbeddedFiles EmbeddedFiles;
    }

    public class MultiPurpDocumentGlobalInfo
    {
        public string Version;
        public Dictionary<string, string> Info;
        public object Encryption;
        public object Metadata;
        public int? Pages;
        public int? Chapters;
        public List<int> ChapterPages;
        public List<MultiPurpDocumentOutline> DocumentOutlines;
        public MultiPurpAttachedFiles AttachedFiles;
        public MultiPurpGatheredErrors GatheredErrors;
    }

    public class MultiPurpPageMediaBox
    {
        public int? Page;
        public List<float> Bounds;
    }

    public class MultiPurpPageFont
    {
        public int? Page;
        public string FontType;
        public string FontName;
        public string FontEncoding;
    }

    public class MultiPurpImageFilter
    {
        public string Filter;
    }

    public class MultiPurpImage
    {
        public string type; // "stream"
        public object data;
    }

    public class MultiPurpImageDimensions
    {
        public float W;
        public float H;
    }

    public class MultiPurpPageImage
    {
        public int? Page;
        public MultiPurpImageFilter ImageFilter;
        public int? ImageWidth;
        public int? ImageHeight;
        public MultiPurpImageDimensions ImageDimensions;
        public int? ImageBPC;
        public string ImageCS;
        public MultiPurpImage Image;
    }

    public class MultiPurpPageAnnotation
    {
        public int? AnnotNumber;
        public string AnnotType;
        public List<float> BoundsInDocument;
        public List<float> Bounds;
        public bool? NeedsNewAP;
        public string Author;
        public string CreationDate;
        public string ModificationDate;
        public string Flags;
        public List<float> PopupBounds;
        public bool? HasInkList;
        public bool? HasQuadPoints;
        public bool? HasVertexData;
        public bool? HasLineData;
        public bool? HasInteriorColor;
        public bool? HasLineEndingStyles;
        public bool? HasIconName;
        public bool? HasOpenAction;
        public bool? HasAuthorData;
        public bool? IsActive;
        public bool? IsHot;
        public string Language;
        public string Icon;
        public string FieldFlags;
        public string FieldKey;
        public string FieldValue;
        public bool? IsEmbeddedFile;
        public string EmbeddedFileName;
        public string EmbeddedFileType;
        public object Popup;
        public string Contents;
    }

    public class MultiPurpLinkInPage
    {
        public string LinkType;
        public string Url;
        public int? TargetPageNumber;
        public int? TargetChapter;
        public int? TargetChapterPage;
        public string TargetError;
        public MultiPurpTargetCoordinates TargetCoordinates;
        public List<float> LinkBounds;
    }

    public class MultiPurpLayerConfig
    {
        public string Name;
        public string Creator;
    }

    public class MultiPurpUILayerConfig
    {
        public int? Depth;
        public string Type;
        public bool? Selected;
        public bool? Locked;
        public string Text;
    }

    public class MultiPurpSinglePageInfo
    {
        public int? PageNumber;
        public List<MultiPurpPageMediaBox> Mediaboxes;
        public List<MultiPurpPageFont> Fonts;
        public List<MultiPurpPageImage> Images;
        public List<float> PageBounds;
        public List<MultiPurpPageAnnotation> Annotations;
        public List<MultiPurpLinkInPage> LinksInPage;
        public List<MultiPurpLayerConfig> PDFLayerConfigs;
        public List<MultiPurpUILayerConfig> PDFUILayerConfigs;
        public string PageError;
        public MultiPurpGatheredErrors GatheredErrors;
    }

    public class MultiPurpFormFieldSignature
    {
        public string Type;
        public string CERT;
        public string DIGEST;
        public string SignatureError;
    }

    public class MultiPurpDocumentGeneralInfo
    {
        public string Title;
        public string Author;
        public string Format;
        public string Encryption;
        public string PDF_Creator;
        public string PDF_Producer;
        public string Subject;
        public string Keywords;
        public string Creation_Date;
        public string Modification_Date;
        public Dictionary<string, object> MetaInfoDictionary;
        public string Permissions;
        public string Status;
        public bool? DocWasLinearized;
        public int? DocumentUpdateCount;
        public string ChangeHistoryValidation;
        public List<MultiPurpFormFieldSignature> FormFieldSignatures;
        public string UpdatesStatus;
        public bool? WasRepaired;
        public bool? NeedsPassword;
        public bool? WasCryptedWithEmptyPassword;
    }

    public class MultiPurpDocumentInfoObject
    {
        public string DocumentFilePath;
        public MultiPurpDocumentGlobalInfo GlobalInfo;
        public int? FirstPage;
        public int? LastPage;
        public List<MultiPurpSinglePageInfo> PageInfo;
        public MultiPurpDocumentGeneralInfo DocumentGeneralInfo;
        public MultiPurpGatheredErrors GatheredErrors;

        // and when we have a severe failure in metadump, a second 'crash record' may be output carrying these fields:
        public string Type;
        public string FailureMessage;
    }

    // ------------------------------------------------------------------------------------------------------------------------------------------------

    public class ExecResultAggregate
    {
        public Exception error;
        public string executable;
        public string process_parameters;

        public int exitCode = -1;
        public bool stdoutIsBinary;
        public byte[] stdoutBinaryData;
        public ProcessOutputDump errOutputDump;
    }

    public static class MuPDFRenderer
    {
        public static byte[] GetPageByHeightAsImage(string filename, string pdf_user_password, int page, int height, int width)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                // sample command (PNG written to stdout for page #2, width and height are limiting/reducing, dpi-resolution is driving):
                //
                //      mudraw -q -o - -F png -r 600 -w 1920 -h 1280 G:\Qiqqa\evil\Guest\documents\1\1A9760F3917A107AC46E6E292B9C839364F09E73.pdf  2
                var img = RenderPDFPage(filename, page, height, width, pdf_user_password, ProcessPriorityClass.BelowNormal);

                return img;
            }
            catch (Exception ex)
            {
                throw new GenericException(ex, $"PDF Render: Error while rasterising page {page} at {height}x{width} pixels of '{filename}'");
            }
        }

        private static volatile int render_count = 0;

        private static byte[] RenderPDFPage(string pdf_filename, int page_number, int height, int width, string password, ProcessPriorityClass priority_class)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            render_count++;

            string process_parameters = String.Format(
                $"draw -T0 -stmf -w {width} -h {height} -F png -o -"
                + " " + (String.IsNullOrEmpty(password) ? "" : "-p " + password)
                + " " + '"' + pdf_filename + '"'
                + " " + page_number
                );

            string exe = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"MuPDF/mutool.exe"));
            if (!File.Exists(exe))
            {
                throw new Exception($"PDF Page Rendering: missing modern MuPDF 'mutool.exe': it does not exist in the expected path: '{exe}'");
            }
            if (!File.Exists(pdf_filename))
            {
                throw new Exception($"PDF Page Rendering: INTERNAL ERROR: missing PDF: it does not exist in the expected path: '{pdf_filename}'");
            }

            ExecResultAggregate rv = ReadEntireStandardOutput(exe, process_parameters, binary_output: true, priority_class);

            if (rv.error != null)
            {
                throw rv.error;
            }
            return rv.stdoutBinaryData;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------

        public static DateTime? ParsePDFTimestamp(string date_str)
        {
            if (String.IsNullOrWhiteSpace(date_str))
                return null;

            date_str = date_str.Trim();
            if (date_str.StartsWith("D:"))
                date_str = date_str.Replace("D:", "");

            // now there's a couple formats we've seen out there, next to our own 
            // metadump "D:%Y%m%d%H%M%SZ" strftime() and "D:%Y-%m-%d %H:%M:%S UTC" fz_printf("%T") outputs:
            // "D:%Y%m%d%H%M%S[-+]TZ'TZ'" for timezoned timestamps, e.g. "D:20120208094057-08'00'" or "D:20090612163852+02'00'"
            DateTime t;

            string[] dt_formats =
            {
                "yyyyMMddHHmmssK",
                "yyyyMMddHHmmsszzz",
                "yyyyMMddHHmmss",
                "yyyy/MM/dd HH:mm:ss UTC",
                "yyyy-MM-dd HH:mm:ss UTC",
            };
            if (DateTime.TryParseExact(date_str, dt_formats, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out t))
                return t;
            // copy with "HH'MM'" timezone formats:
            date_str = date_str.Replace("'", ":").TrimEnd(":".ToCharArray());
            if (DateTime.TryParseExact(date_str, dt_formats, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out t))
                return t;

            // last resort: try to parse any generic timestamp
            if (DateTime.TryParse(date_str, out t))
                return t;
            return null;
        }

        public static PDFDocumentMuPDFMetaInfo ParseDocumentMetaInfo(string json)
        {
            PDFDocumentMuPDFMetaInfo rv = new PDFDocumentMuPDFMetaInfo();

            rv.raw_metadump_text = json;

            if (!String.IsNullOrWhiteSpace(json))
            {
                rv.raw_decoded_json = JsonConvert.DeserializeObject<List<MultiPurpDocumentInfoObject>>(json,
                    new JsonSerializerSettings
                    {
                        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                        {
                            rv.errors.Add(args.ErrorContext.Error.Message);
                            args.ErrorContext.Handled = true;
                        },
                        //Converters = { new IsoDateTimeConverter() }
                    });
            }
            else
            {
                rv.raw_decoded_json = null;
            }

            if (rv.raw_decoded_json != null)
            {
                foreach (MultiPurpDocumentInfoObject raw_infos in rv.raw_decoded_json)
                {
                    var info = raw_infos.GlobalInfo;
                    if (info?.Pages != null)
                    {
                        rv.PageCount = info.Pages.Value;
                    }

                    // when the document had to be reported or caused trouble, we flag it as "corrupted":
                    if ((raw_infos.PageInfo?.Count ?? 0) > 0 && (raw_infos.DocumentGeneralInfo?.WasRepaired ?? false))
                    {
                        rv.DocumentIsCorrupted = true;
                        rv.DocumentErrorCode = PDFErrors.DOCUMENT_IS_CORRUPTED;
                    }

                    // when we have ZERO pages, then the errors must have been severe and we surely have a corrupted (or at least a very untrustworthy!) PDF analysis:
                    if ((raw_infos.PageInfo?.Count ?? 0) == 0)
                    {
                        rv.DocumentIsCorrupted = true;
                        rv.DocumentErrorCode = PDFErrors.DOCUMENT_IS_CORRUPTED;
                    }

                    // when we have an outer error block, then the errors were severe and we surely have a corrupted (or at least a very untrustworthy!) PDF:
                    if (raw_infos.GatheredErrors != null)
                    {
                        string errmsg = raw_infos.GatheredErrors?.Log;
                        if (!String.IsNullOrEmpty(errmsg))
                        {
                            rv.errors.Add($"GatheredError: {errmsg}");
                            rv.DocumentIsCorrupted = true;
                            rv.DocumentErrorCode = PDFErrors.DOCUMENT_IS_CORRUPTED;
                        }
                    }

                    if (!String.IsNullOrEmpty(raw_infos.FailureMessage))
                    {
                        rv.errors.Add($"MuPDF FailureMessage: { raw_infos.FailureMessage }");
                        rv.DocumentIsCorrupted = true;
                        rv.DocumentErrorCode = PDFErrors.DOCUMENT_IS_CORRUPTED;
                    }

                    // when we have JSON parse errors, then the errors must have been severe and we surely have a corrupted (or at least a very untrustworthy!) PDF analysis:
                    if (rv.errors.Count > 0)
                    {
                        rv.DocumentIsCorrupted = true;
                        rv.DocumentErrorCode = PDFErrors.DOCUMENT_IS_CORRUPTED;
                    }

                    // now collect the basic info summary datums:
                    if (info != null)
                    {
                        rv.PDFVersion = info.Version;
                        rv.Chapters = info.Chapters ?? 0;
                        // 1 chapter is essentially the same as NO CHAPTER
                        if (rv.Chapters == 1)
                            rv.Chapters = 0;
                        if ((info.ChapterPages?.Count ?? 0) > 1)
                        {
                            rv.ChapterPages = info.ChapterPages;
                        }
                        rv.HasDocumentOutlines = ((info.DocumentOutlines?.Count ?? 0) >= 1);
                        rv.AttachedFilesCount = (info.AttachedFiles?.EmbeddedFiles?.Names?.Count ?? 0);
                        rv.EmbeddedJavaScriptFilesCount = (info.AttachedFiles?.JavaScript?.Names?.Count ?? 0);
                    }

                    var summary = raw_infos.DocumentGeneralInfo;
                    if (summary != null)
                    {
                        rv.Title = summary.Title;
                        rv.Author = summary.Author;
                        rv.Format = summary.Format;
                        rv.Encryption = summary.Encryption;
                        if (String.IsNullOrWhiteSpace(rv.Encryption) || rv.Encryption == "None")
                        {
                            rv.Encryption = null;
                        }
                        rv.PDFCreator = summary.PDF_Creator;
                        rv.PDFProducer = summary.PDF_Producer;
                        rv.Subject = summary.Subject;
                        if (!String.IsNullOrWhiteSpace(summary.Keywords))
                        {
                            rv.Keywords = summary.Keywords;
                        }
                        rv.Creation_Date = ParsePDFTimestamp(summary.Creation_Date);
                        rv.Modification_Date = ParsePDFTimestamp(summary.Modification_Date);
                        rv.Permissions = summary.Permissions;
                        rv.PDFStatus = summary.Status;
                        rv.DocumentUpdateCount = summary.DocumentUpdateCount ?? 0;
                        rv.ChangeHistoryValidation = summary.ChangeHistoryValidation;
                        rv.FormFieldSignaturesCount = summary.FormFieldSignatures?.Count ?? 0;
                        rv.UpdatesStatus = summary.UpdatesStatus;
                        rv.WasRepaired = summary.WasRepaired ?? false;
                        rv.NeedsPassword = summary.NeedsPassword ?? false;
                        rv.WasCryptedWithEmptyPassword = summary.WasCryptedWithEmptyPassword ?? false;
                    }
                }
            }
            else
            {
                // when we have JSON parse errors, then the errors must have been severe and we surely have a corrupted (or at least a very untrustworthy!) PDF analysis:
                rv.PageCount = 0;
                rv.DocumentIsCorrupted = true;
                rv.DocumentErrorCode = PDFErrors.PAGECOUNT_GENERAL_FAILURE;
            }

            return rv;
        }


        public static PDFDocumentMuPDFMetaInfo GetDocumentMetaInfo(string pdf_filename, string password, ProcessPriorityClass priority_class)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            string process_parameters = String.Format(
                $"metadump -m 1 -o -"
                + " " + (String.IsNullOrEmpty(password) ? "" : "-p " + password)
                + " " + '"' + pdf_filename + '"'
                );

            string exe = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"MuPDF/mutool.exe"));
            if (!File.Exists(exe))
            {
                PDFDocumentMuPDFMetaInfo rv = ParseDocumentMetaInfo(null);
                rv.errors = new List<string>() { $"PDF metadata gathering: missing modern MuPDF 'mutool': it does not exist in the expected path: '{exe}'" };
                rv.DocumentIsCorrupted = true;
                rv.DocumentErrorCode = PDFErrors.PAGECOUNT_TOOL_MISSING;
                return rv;
            }
            if (!File.Exists(pdf_filename))
            {
                PDFDocumentMuPDFMetaInfo rv = ParseDocumentMetaInfo(null);
                rv.errors = new List<string>() { $"PDF metadata gathering: INTERNAL ERROR: missing PDF: it does not exist in the expected path: '{pdf_filename}'" };
                rv.DocumentIsCorrupted = true;
                rv.DocumentErrorCode = PDFErrors.DOCUMENT_DOES_NOT_EXIST;
                return rv;
            }

            ExecResultAggregate execResult = null;
            try
            {
                execResult = ReadEntireStandardOutput(exe, process_parameters, binary_output: false, priority_class);
                if (execResult.stdoutBinaryData == null && execResult.error != null)
                {
                    throw execResult.error;
                }
                using (MemoryStream ms = new MemoryStream(execResult.stdoutBinaryData))
                {
                    using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        string json = sr.ReadToEnd();
                        //string json = Encoding.UTF8.GetString(txt);

                        return ParseDocumentMetaInfo(json);
                    }
                }
            }
            catch (Exception ex)
            {
                if (execResult == null)
                {
                    execResult = new ExecResultAggregate
                    {
                        executable = exe,
                        process_parameters = process_parameters,
                        stdoutIsBinary = false,
                        error = ex,
                        exitCode = -1,
                    };
                }

                Logging.Error(ex, $"Failed to process the output from the command:\n     { execResult.executable } { execResult.process_parameters }\n --->\n    exitCode: { execResult.exitCode }\n    stderr: { execResult.errOutputDump.stderr }\n    runtime error: { execResult.error }");

                string errstr = execResult.errOutputDump.stderr ?? "";
                PDFDocumentMuPDFMetaInfo rv = ParseDocumentMetaInfo(null);
                rv.DocumentIsCorrupted = true;
                rv.errors = new List<string>() { errstr };
                rv.DocumentErrorCode = PDFErrors.PAGECOUNT_GENERAL_FAILURE;

                // apply some damaged PDF heuristics:
                if (execResult.exitCode == 1 && (errstr.IndexOf("<!DOCTYPE html") >= 0 || errstr.IndexOf("<html") >= 0))
                {
                    rv.DocumentErrorCode = PDFErrors.PAGECOUNT_DOCUMENT_IS_BAD_HTML_DOWNLOAD;
                }

                return rv;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------

        public class TextChunk
        {
            public string text;
            public string font_name;
            public double font_size;
            public int page;
            public double x0, y0, x1, y1;

            public override string ToString()
            {
                return String.Format("{0} p{5} {1:.000},{2:.000} {3:.000},{4:.000} ", text, x0, y0, x1, y1, page);
            }
        }

        public static List<TextChunk> GetEmbeddedText(string pdf_filename, string page_numbers, string password, ProcessPriorityClass priority_class)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            string process_parameters = String.Format(
                ""
                + " " + "-tt "
                + " " + (String.IsNullOrEmpty(password) ? "" : "-p " + password)
                + " " + '"' + pdf_filename + '"'
                + " " + page_numbers
                );

            string exe = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"pdfdraw.exe"));
            if (!File.Exists(exe))
            {
                throw new Exception($"PDF Text Extraction: missing 'pdfdraw.exe': it does not exist in the expected path: '{exe}'");
            }
            if (!File.Exists(pdf_filename))
            {
                throw new Exception($"PDF Text Extraction: INTERNAL ERROR: missing PDF: it does not exist in the expected path: '{pdf_filename}'");
            }

            var execResult = ReadEntireStandardOutput("pdfdraw.exe", process_parameters, binary_output: false, priority_class);
            if (execResult.stdoutBinaryData == null && execResult.error != null)
            {
                throw execResult.error;
            }

            using (MemoryStream ms = new MemoryStream(execResult.stdoutBinaryData))
            {
                using (StreamReader sr_lines = new StreamReader(ms, Encoding.UTF8))
                {
                    List<TextChunk> text_chunks = new List<TextChunk>();

                    int page = 0;
                    double page_x0 = 0;
                    double page_y0 = 0;
                    double page_x1 = 0;
                    double page_y1 = 0;
                    double page_rotation = 0;

                    string current_font_name = "";
                    double current_font_size = 0;

                    string line;
                    while (null != (line = sr_lines.ReadLine()))
                    {
                        // Look for a character element (note that even a " can be the character in the then malformed XML)
                        {
                            Match match = Regex.Match(line, "char ucs=\"(.*)\" bbox=\"\\[(\\S*) (\\S*) (\\S*) (\\S*)\\]");
                            if (Match.Empty != match)
                            {
                                string text = match.Groups[1].Value;
                                double word_x0 = Convert.ToDouble(match.Groups[2].Value, Internationalization.DEFAULT_CULTURE);
                                double word_y0 = Convert.ToDouble(match.Groups[3].Value, Internationalization.DEFAULT_CULTURE);
                                double word_x1 = Convert.ToDouble(match.Groups[4].Value, Internationalization.DEFAULT_CULTURE);
                                double word_y1 = Convert.ToDouble(match.Groups[5].Value, Internationalization.DEFAULT_CULTURE);

                                ResolveRotation(page_rotation, ref word_x0, ref word_y0, ref word_x1, ref word_y1);

                                // safety measure: discard zero-width and zero-height "words" as those only cause trouble down the line:
                                if (word_x0 == word_x1 || word_y0 == word_y1)
                                {
                                    Logging.Warn("Zero-width/height bounding box for text chunk: ignoring this 'word' @ {0}.", line);
                                    continue;
                                }

                                // Position this little grubber
                                TextChunk text_chunk = new TextChunk();
                                text_chunk.text = text;
                                text_chunk.font_name = current_font_name;
                                text_chunk.font_size = current_font_size;
                                text_chunk.page = page;
                                text_chunk.x0 = (word_x0 - page_x0) / (page_x1 - page_x0);
                                text_chunk.y0 = 1 - (word_y0 - page_y0) / (page_y1 - page_y0);
                                text_chunk.x1 = (word_x1 - page_x0) / (page_x1 - page_x0);
                                text_chunk.y1 = 1 - (word_y1 - page_y0) / (page_y1 - page_y0);

                                // Cater for the rotation
                                if (0 != page_rotation)
                                {
                                    text_chunk.y0 = 1 - text_chunk.y0;
                                    text_chunk.y1 = 1 - text_chunk.y1;
                                }

                                // Make sure the bounding box is TL-BR
                                if (text_chunk.x1 < text_chunk.x0)
                                {
                                    Swap.swap(ref text_chunk.x0, ref text_chunk.x1);
                                }
                                if (text_chunk.y1 < text_chunk.y0)
                                {
                                    Swap.swap(ref text_chunk.y0, ref text_chunk.y1);
                                }

                                if (text_chunk.x1 <= text_chunk.x0 || text_chunk.y1 <= text_chunk.y0)
                                {
                                    Logging.Warn("Bad bounding box for text chunk ({0})", process_parameters);
                                }

                                // And add him to the result list
                                text_chunks.Add(text_chunk);

                                continue;
                            }
                        }

                        // Look for a change in font name
                        {
                            Match match = Regex.Match(line, " font=\"(\\S*)\" size=\"(\\S*)\" ");
                            if (Match.Empty != match)
                            {
                                current_font_name = match.Groups[1].Value;
                                current_font_size = Convert.ToDouble(match.Groups[2].Value, Internationalization.DEFAULT_CULTURE);

                                continue;
                            }
                        }

                        // Look for the page header with dimensions
                        {
                            Match match = Regex.Match(line, @"\[Page (.+) X0 (\S+) Y0 (\S+) X1 (\S+) Y1 (\S+) R (\S+)\]");
                            if (Match.Empty != match)
                            {
                                page = Convert.ToInt32(match.Groups[1].Value, Internationalization.DEFAULT_CULTURE);
                                page_x0 = Convert.ToDouble(match.Groups[2].Value, Internationalization.DEFAULT_CULTURE);
                                page_y0 = Convert.ToDouble(match.Groups[3].Value, Internationalization.DEFAULT_CULTURE);
                                page_x1 = Convert.ToDouble(match.Groups[4].Value, Internationalization.DEFAULT_CULTURE);
                                page_y1 = Convert.ToDouble(match.Groups[5].Value, Internationalization.DEFAULT_CULTURE);
                                page_rotation = Convert.ToDouble(match.Groups[6].Value, Internationalization.DEFAULT_CULTURE);

                                ResolveRotation(page_rotation, ref page_x0, ref page_y0, ref page_x1, ref page_y1);

                                continue;
                            }
                        }
                    }

                    text_chunks = AggregateOverlappingTextChunks(text_chunks, process_parameters);
                    return text_chunks;
                }
            }
        }

        private static void ResolveRotation(double page_rotation, ref double x0, ref double y0, ref double x1, ref double y1)
        {
            // If this page is rotated -- i guess we should be looking for 90 or 270, etc, but lets assume non-zero will work
            if (0 != page_rotation)
            {
                Swap<double>.swap(ref x0, ref y0);
                Swap<double>.swap(ref x1, ref y1);
            }
        }

        private static List<TextChunk> AggregateOverlappingTextChunks(List<TextChunk> text_chunks_original, string debug_cli_info)
        {
            List<TextChunk> text_chunks = new List<TextChunk>();

            TextChunk current_text_chunk = null;
            foreach (TextChunk text_chunk in text_chunks_original)
            {
                if (text_chunk.x1 <= text_chunk.x0 || text_chunk.y1 <= text_chunk.y0)
                {
                    Logging.Warn("Bad bounding box for raw text chunk ({0})", debug_cli_info);
                }

                // If we flushed the last word
                if (null == current_text_chunk)
                {
                    current_text_chunk = text_chunk;
                    text_chunks.Add(text_chunk);
                    continue;
                }

                // If it's a space
                if (0 == text_chunk.text.CompareTo(" "))
                {
                    current_text_chunk = null;
                    continue;
                }

                // If it's on a different page...
                if (text_chunk.page != current_text_chunk.page)
                {
                    current_text_chunk = text_chunk;
                    text_chunks.Add(text_chunk);
                    continue;
                }

                // If its substantially below the current chunk
                if (text_chunk.y0 > current_text_chunk.y1)
                {
                    current_text_chunk = text_chunk;
                    text_chunks.Add(text_chunk);
                    continue;
                }

                // If its substantially above the current chunk
                if (text_chunk.y1 < current_text_chunk.y0)
                {
                    current_text_chunk = text_chunk;
                    text_chunks.Add(text_chunk);
                    continue;
                }

                // If it is substantially to the left of the current chunk
                if (text_chunk.x1 < current_text_chunk.x0)
                {
                    current_text_chunk = text_chunk;
                    text_chunks.Add(text_chunk);
                    continue;
                }

                // If its more than a letter's distance across from the current word
                double average_letter_width = (current_text_chunk.x1 - current_text_chunk.x0) / current_text_chunk.text.Length;
                double current_letter_gap = (text_chunk.x0 - current_text_chunk.x1);
                if (current_letter_gap > average_letter_width)
                {
                    current_text_chunk = text_chunk;
                    text_chunks.Add(text_chunk);
                    continue;
                }

                // If we get here we aggregate
                {
                    current_text_chunk.text = current_text_chunk.text + text_chunk.text;
                    current_text_chunk.x0 = Math.Min(current_text_chunk.x0, Math.Min(text_chunk.x0, text_chunk.x1));
                    current_text_chunk.y0 = Math.Min(current_text_chunk.y0, Math.Min(text_chunk.y0, text_chunk.y1));
                    current_text_chunk.x1 = Math.Max(current_text_chunk.x1, Math.Max(text_chunk.x0, text_chunk.x1));
                    current_text_chunk.y1 = Math.Max(current_text_chunk.y1, Math.Max(text_chunk.y0, text_chunk.y1));
                }

                if (current_text_chunk.x1 <= current_text_chunk.x0 || current_text_chunk.y1 <= current_text_chunk.y0)
                {
                    Logging.Warn("Bad bounding box for aggregated text chunk ({0})", debug_cli_info);
                }
            }

            return text_chunks;
        }

        private static ExecResultAggregate ReadEntireStandardOutput(string pdfDrawExe, string process_parameters, bool binary_output, ProcessPriorityClass priority_class)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            ExecResultAggregate rv = new ExecResultAggregate
            {
                executable = pdfDrawExe,
                process_parameters = process_parameters,
                stdoutIsBinary = binary_output
            };

            Stopwatch clk = Stopwatch.StartNew();

            // STDOUT/STDERR
            Logging.Debug("PDFDRAW :: ReadEntireStandardOutput command: pdfdraw.exe {0}", process_parameters);
            using (Process process = ProcessSpawning.SpawnChildProcess(pdfDrawExe, process_parameters, priority_class, stdout_is_binary: true))
            {
                using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process, stdout_is_binary: true))
                {
                    // Read image from stdout
                    long elapsed = clk.ElapsedMilliseconds;
                    Logging.Debug("PDFDRAW :: ReadEntireStandardOutput setup time: {0} ms for parameters:\n    {1}", elapsed, process_parameters);

                    long duration = 0;

                    // Wait a few minutes for the Text Extract process to exit
                    while (true)
                    {
                        duration = clk.ElapsedMilliseconds;

                        if (ShutdownableManager.Instance.IsShuttingDown)
                        {
                            break;
                        }
                        if (process.WaitForExit(1000))
                        {
                            break;
                        }
                        if (duration >= Constants.MAX_WAIT_TIME_MS_FOR_QIQQA_OCR_TASK_TO_TERMINATE + Constants.EXTRA_TIME_MS_FOR_WAITING_ON_QIQQA_OCR_TASK_TERMINATION)
                        {
                            break;
                        }
                    }

                    // Check that the process has exited properly
                    long elapsed2 = clk.ElapsedMilliseconds;

                    if (!process.HasExited)
                    {
                        Logging.Debug("PDFRenderer process did not terminate, so killing it.\n{0}", process_output_reader.GetOutputsDumpStrings().stderr);

                        try
                        {
                            process.Kill();

                            // wait for the completion signal; this also helps to collect all STDERR output of the application (even while it was KILLED)
                            process.WaitForExit();
                        }
                        catch (Exception ex)
                        {
                            long elapsed3 = clk.ElapsedMilliseconds;
                            Logging.Error(ex, "There was a problem killing the PDFRenderer process after timeout ({0} ms)", elapsed3);
                        }

                        // grab stderr output for successful runs and log it anyway: MuPDF diagnostics, etc. come this way:
                        var outs = process_output_reader.GetOutputsDumpStrings();
                        rv.errOutputDump = outs;

                        Logging.Error($"PDFRenderer process did not terminate, so killed it. Commandline:\n    {pdfDrawExe} {process_parameters}\n{outs.stderr}");

                        rv.error = new ApplicationException($"PDFRenderer process did not terminate, so killed it.\n    Commandline: {pdfDrawExe} {process_parameters}");
                        rv.exitCode = 0;
                        if (process.HasExited)
                        {
                            rv.exitCode = Math.Abs(process.ExitCode);
                        }
                        if (rv.exitCode == 0)
                        {
                            rv.exitCode = PDFErrors.PAGECOUNT_DOCUMENT_TIMEOUT;  // timeout
                        }
                    }
                    else if (process.ExitCode != 0)
                    {
                        // grab stderr output for successful runs and log it anyway: MuPDF diagnostics, etc. come this way:
                        var outs = process_output_reader.GetOutputsDumpStrings();
                        rv.errOutputDump = outs;

                        Logging.Error($"MuPDF did fail with exit code {process.ExitCode} for commandline:\n    {pdfDrawExe} {process_parameters}\n{outs.stderr}");

                        rv.error = new ApplicationException($"PDFRenderer::PDFDRAW did fail with exit code {process.ExitCode}.\n    Commandline: {pdfDrawExe} {process_parameters}");
                        rv.exitCode = Math.Abs(process.ExitCode);
                    }
                    else
                    {
                        // grab stderr output for successful runs and log it anyway: MuPDF diagnostics, etc. come this way:
                        var outs = process_output_reader.GetOutputsDumpStrings();
                        rv.errOutputDump = outs;

                        Logging.Error($"MuPDF did SUCCEED with exit code {process.ExitCode} for commandline:\n    {pdfDrawExe} {process_parameters}\n{outs.stderr}");

                        rv.error = null;
                        rv.exitCode = Math.Abs(process.ExitCode);

                        rv.stdoutBinaryData = process_output_reader.BinaryOutput;
                        int total_size = rv.stdoutBinaryData?.Length ?? 0;

                        Logging.Debug("PDFDRAW image output {0} bytes in {1} ms (output copy took {2} ms) for command:\n    {4} {3}", total_size, elapsed2, elapsed2 - elapsed, process_parameters, pdfDrawExe);
                    }

                    return rv;
                }
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness_TEXT_RENDER()
        {
            // SUCCESSES
            //TestHarness_TEXT_RENDER_ONE(@"C:\temp\2.pdf", 2);
            //TestHarness_TEXT_RENDER_ONE(@"C:\temp\3.pdf", 3);
            //TestHarness_TEXT_RENDER_ONE(@"C:\temp\6.pdf", 3);
            //TestHarness_TEXT_RENDER_ONE(@"C:\temp\japanese.pdf", 3);
            //TestHarness_TEXT_RENDER_ONE(@"C:\temp\p5.pdf", 3);
            //TestHarness_TEXT_RENDER_ONE(@"C:\temp\7.pdf", 3);
            //TestHarness_TEXT_RENDER_ONE(@"C:\temp\7a.pdf", 3);

            // FAILURES
            //TestHarness_TEXT_RENDER_ONE(@"C:\temp\kevin.pdf", 3);
            //TestHarness_TEXT_RENDER_ONE(@"C:\Users\Jimme\AppData\Roaming\Quantisle\Qiqqa\\7CDA3872-F99B-49B5-A0EB-E58C08719C1C\documents\1\1E18A4945DA8F9CDB6621F12FECE3CFFC3CB7CF.pdf", 2);
            //TestHarness_TEXT_RENDER_ONE(@"C:\Users\Jimme\AppData\Roaming\Quantisle\Qiqqa\\7CDA3872-F99B-49B5-A0EB-E58C08719C1C\documents\3\3760DF1C9E1D7944F15FAC5E077E9CE4520F33E.pdf", 8);

            for (int i = 0; i < 2; ++i)
            {
                TestHarness_TEXT_RENDER_ONE(@"C:\Users\Jimme\AppData\Roaming\Quantisle\Qiqqa\\7CDA3872-F99B-49B5-A0EB-E58C08719C1C\documents\1\1E18A4945DA8F9CDB6621F12FECE3CFFC3CB7CF.pdf", 8);
            }
        }

        public static void TestHarness_TEXT_RENDER_ONE(string PDF_FILENAME, int PAGE)
        {
            SolidColorBrush BRUSH_EDGE = new SolidColorBrush(Colors.Red);
            SolidColorBrush BRUSH_BACKGROUND = new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.GreenYellow, 64));

            // Load the image graphic
            MemoryStream ms = RenderPDFPage(PDF_FILENAME, PAGE, 150, null, ProcessPriorityClass.Normal);
            BitmapSource bitmap_image = BitmapImageTools.LoadFromBytes(ms.ToArray());
            Image image = new Image();
            image.Source = bitmap_image;
            image.Stretch = Stretch.Fill;

            // Load the image text
            Canvas canvas = new Canvas();
            List<TextChunk> text_chunks = GetEmbeddedText(PDF_FILENAME, "" + PAGE, null, ProcessPriorityClass.Normal);
            List<Rectangle> rectangles = new List<Rectangle>();
            foreach (TextChunk text_chunk in text_chunks)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Stroke = BRUSH_EDGE;
                rectangle.Fill = BRUSH_BACKGROUND;
                rectangle.Tag = text_chunk;
                rectangles.Add(rectangle);
                canvas.Children.Add(rectangle);
            }

            RectanglesManager rm = new RectanglesManager(rectangles, canvas);

            Grid grid = new Grid();
            grid.Children.Add(image);
            grid.Children.Add(canvas);

            ControlHostingWindow window = new ControlHostingWindow("PDF", grid);
            window.Show();
        }

        class RectanglesManager
        {
            List<Rectangle> rectangles;
            Canvas canvas;
            public RectanglesManager(List<Rectangle> rectangles, Canvas canvas)
            {
                this.rectangles = rectangles;
                this.canvas = canvas;

                canvas.SizeChanged += canvas_SizeChanged;
            }

            void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
            {
                if (Double.IsNaN(canvas.ActualWidth) || Double.IsNaN(canvas.ActualHeight))
                {
                    return;
                }

                foreach (Rectangle rectangle in rectangles)
                {
                    TextChunk text_chunk = (TextChunk)rectangle.Tag;

                    rectangle.Width = canvas.ActualWidth * (text_chunk.x1 - text_chunk.x0);
                    rectangle.Height = canvas.ActualHeight * (text_chunk.y1 - text_chunk.y0);

                    Canvas.SetLeft(rectangle, canvas.ActualWidth * text_chunk.x0);
                    Canvas.SetTop(rectangle, canvas.ActualHeight * text_chunk.y0);
                }
            }
        }

        public static void TestHarness_TEXT()
        {
            Logging.Info("Start!");

            List<TextChunk> chunks = GetEmbeddedText(@"C:\temp\poo.pdf", "1", null, ProcessPriorityClass.Normal);

            //for (int i = 1; i <= 32; ++i)
            //{
            //    Logging.Info("Page {0}", i);
            //    List<TextChunk> chunks = GetEmbeddedText(@"C:\temp\2.pdf", i, null, ProcessPriorityClass.Normal);
            //}
            //for (int i = 1; i <= 32; ++i)
            //{
            //    Logging.Info("Page {0}", i);
            //    List<TextChunk> chunks = GetEmbeddedText(@"C:\temp\3.pdf", i, null, ProcessPriorityClass.Normal);
            //}
            //for (int i = 1; i <= 32; ++i)
            //{
            //    Logging.Info("Page {0}", i);
            //    List<TextChunk> chunks = GetEmbeddedText(@"C:\temp\kevin.pdf", i, null, ProcessPriorityClass.Normal);
            //}
            //for (int i = 1; i <= 132; ++i)
            //{
            //    Logging.Info("Page {0}", i);
            //    List<TextChunk> chunks = GetEmbeddedText(@"C:\temp\6.pdf", i, null, ProcessPriorityClass.Normal);
            //}

            Logging.Info("Done!");
        }


        public static void TestHarness_IMAGE()
        {
            Logging.Info("Start!");
            for (int i = 0; i < 10; ++i)
            {
                MemoryStream ms = RenderPDFPage(@"C:\temp\3.pdf", 1, 200, null, ProcessPriorityClass.Normal);
                Bitmap bitmap = new Bitmap(ms);
            }
            Logging.Info("Done!");
        }
#endif

        #endregion ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
