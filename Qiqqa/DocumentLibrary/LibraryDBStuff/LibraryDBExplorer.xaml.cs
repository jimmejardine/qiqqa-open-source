using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities.BibTex.Parsing;
using Utilities.GUI;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.LibraryDBStuff
{
    /// <summary>
    /// Interaction logic for LibraryDBExplorer.xaml
    /// </summary>
    public partial class LibraryDBExplorer : UserControl
    {
        private WebLibraryDetail web_library_detail = null;

        public LibraryDBExplorer()
        {
            InitializeComponent();

            TxtLibrary.IsReadOnly = true;
            TxtLibrary.PreviewMouseDown += TxtLibrary_PreviewMouseDown;
            TxtLibrary.ToolTip = "Click to choose a library to use for your Expedition.";
            TxtLibrary.Cursor = Cursors.Hand;

            MaxNumberOfRecords.Text = "100";

            ButtonGet.Caption = "Get";
            ButtonGet.Click += ButtonGet_Click;

            ButtonPut.Caption = "Put";
            ButtonPut.Click += ButtonPut_Click;
        }

        private void ButtonGet_Click(object sender, RoutedEventArgs e)
        {
            if (null == web_library_detail)
            {
                MessageBoxes.Error("You must choose a library...");
                return;
            }

            TxtData.Text = "";

            int MaxRecordCount;
            if (!int.TryParse(MaxNumberOfRecords.Text, out MaxRecordCount))
            {
                MaxRecordCount = 0;
            }

            var items = web_library_detail.Xlibrary.LibraryDB.GetLibraryItems(TxtFingerprint.Text, TxtExtension.Text, MaxRecordCount);
            if (0 == items.Count)
            {
                MessageBoxes.Warn("No entry was found.");
            }
            else if (1 == items.Count)
            {
                byte[] data = items[0].data;
                string json = Encoding.UTF8.GetString(data);
                TxtData.Text = json;
            }
            else
            {
                MessageBoxes.Warn("{0} entries were found; we're showing them all but you'll only be able to PUT/WRITE the first one!", items.Count);

                StringBuilder allstr = new StringBuilder();
                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0)
                    {
                        allstr.Append("\n\n==========================================================\n\n");
                    }

                    LibraryDB.LibraryItem item = items[i];
                    byte[] data = item.data;
                    string json = Encoding.UTF8.GetString(data);
                    allstr.AppendLine(json);
                    allstr.Append("\n--------------(decoded metadata)--------------------------\n");
                    allstr.AppendLine(string.Format("fingerprint: {0}", item.fingerprint));
                    allstr.AppendLine(string.Format("extension: {0}", item.extension));
                    allstr.AppendLine(string.Format("MD5 hash: {0}", item.md5));

                    if (item.data.Length > 0)
                    {
                        switch (item.extension)
                        {
                            case "citations":
                                try
                                {
                                    string citations = Encoding.UTF8.GetString(data);
                                    // format: (hash, hash_REF, unknown_int) on every line. e.g.
                                    //
                                    // 99D81E4872BD14C8766C57B298175B92C9CA749,024F6E48744443D5B7D22DE2007EFA7C_REF,0
                                    // E62116BFF03E2D6AF99D596C8EB5C3D3B6111B5,024F6E48744443D5B7D22DE2007EFA7C_REF,0
                                    // EB9BAE68C451CEEC70E4FE352078AEA4050A427,024F6E48744443D5B7D22DE2007EFA7C_REF,0
                                    // ...
                                    allstr.AppendLine(string.Format("citations = [\n{0}\n]", citations));
                                }
                                catch (Exception ex)
                                {
                                    allstr.AppendLine(string.Format("*** CITATIONS RECORD PARSE ERROR ***:\n    {0}", ex.ToString().Replace("\n", "\n    ")));
                                }
                                break;

                                // highlights format: JSON:
                                //
                                // [
                                //  {
                                //    "P": 3,
                                //    "L": 0.21154,
                                //    "T": 0.1395,
                                //    "W": 0.09829,
                                //    "H": 0.01615,
                                //    "C": 0
                                //  },
                                //  ...
                                //

                            /*
                             * example annotation: JSON format:
                             *
                             *  [
                             *     {
                             *       "Guid": "329abf6b-59b4-450a-b015-65402c25068d",
                             *       "DocumentFingerprint": "0E294EABE45DD6B903A3F6EEF964D80645F272C",
                             *       "Page": 4,
                             *       "DateCreated": "20120427153803358",
                             *       "Deleted": false,
                             *       "ColorWrapper": "#FF87CEEB",
                             *       "Left": 0.11479289940828402,
                             *       "Top": 0.06685699621479417,
                             *       "Width": 0.41301775147928993,
                             *       "Height": 0.25688073394495414,
                             *       "FollowUpDate": "00010101000000000",
                             *       "Text": "rgrgdrgdrgdrgdrgdrgdrdrgdrg"
                             *     }
                             *   ]
                             */

                            case "annotations":
                                try
                                {
                                    string s = Encoding.UTF8.GetString(data);
                                    allstr.AppendLine(string.Format("{1} = [\n{0}\n]", s, item.extension));
                                }
                                catch (Exception ex)
                                {
                                    allstr.AppendLine(string.Format("*** {1} RECORD PARSE ERROR ***:\n    {0}", ex.ToString().Replace("\n", "\n    "), item.extension));
                                }
                                break;

                            case "highlights":
                                try
                                {
                                    string s = Encoding.UTF8.GetString(data);
                                    allstr.AppendLine(string.Format("{1} = [\n{0}\n]", s, item.extension));
                                }
                                catch (Exception ex)
                                {
                                    allstr.AppendLine(string.Format("*** {1} RECORD PARSE ERROR ***:\n    {0}", ex.ToString().Replace("\n", "\n    "), item.extension));
                                }
                                break;

                                // inks format: binary serialized
                                //
                            case "inks":
                                try
                                {
                                    string s = Encoding.UTF8.GetString(data);
                                    allstr.AppendLine(string.Format("{1} = [\n{0}\n]", s, item.extension));
                                }
                                catch (Exception ex)
                                {
                                    allstr.AppendLine(string.Format("*** {1} RECORD PARSE ERROR ***:\n    {0}", ex.ToString().Replace("\n", "\n    "), item.extension));
                                }
                                break;

                            case "metadata":
                                try
                                {
                                    PDFDocument doc = PDFDocument.LoadFromMetaData(web_library_detail, item.data, null);
                                    string bibtexStr = doc.BibTex;
                                    if (null == bibtexStr)
                                    {
                                        bibtexStr = "--(NULL)--";
                                    }
                                    else if (String.IsNullOrWhiteSpace(bibtexStr))
                                    {
                                        bibtexStr = "--(EMPTY)--";
                                    }
                                    else
                                    {
                                        BibTexItem bibtex = doc.BibTexItem;
                                        string bibtexParseErrors;
                                        string formattedBibStr;
                                        string rawStr;

                                        if (bibtex != null)
                                        {
                                            if (bibtex.Exceptions.Count > 0 || bibtex.Warnings.Count > 0)
                                            {
                                                bibtexParseErrors = bibtex.GetExceptionsAndMessagesString();
                                            }
                                            else
                                            {
                                                bibtexParseErrors = String.Empty;
                                            }
                                            formattedBibStr = bibtex.ToBibTex();
                                            if (String.IsNullOrEmpty(formattedBibStr))
                                            {
                                                formattedBibStr = "--(EMPTY)--";
                                            }
                                            rawStr = bibtex.ToString();
                                            if (String.IsNullOrEmpty(rawStr))
                                            {
                                                rawStr = "--(EMPTY)--";
                                            }
                                        }
                                        else
                                        {
                                            bibtexParseErrors = "ERROR: This content is utterly INVALID BibTeX as far as the BibTeX parser is concerned!";
                                            formattedBibStr = String.Empty;
                                            rawStr = String.Empty;
                                        }

                                        if (!String.IsNullOrEmpty(formattedBibStr))
                                        {
                                            allstr.AppendLine(string.Format("\nBibTeX Formatted:\n    {0}", formattedBibStr.Replace("\n", "\n    ")));
                                        }
                                        if (!String.IsNullOrEmpty(rawStr))
                                        {
                                            allstr.AppendLine(string.Format("\nBibTeX RAW FMT:\n    {0}", rawStr.Replace("\n", "\n    ")));
                                        }
                                        if (!String.IsNullOrEmpty(bibtexParseErrors))
                                        {
                                            allstr.AppendLine(string.Format("\nBibTeX Parse Diagnostics:\n    {0}", bibtexParseErrors.Replace("\n", "\n    ")));
                                        }
                                    }
                                    allstr.AppendLine(string.Format("\nBibTeX RAW INPUT:\n    {0}", bibtexStr.Replace("\n", "\n    ")));
                                }
                                catch (Exception ex)
                                {
                                    allstr.AppendLine(string.Format("*** PARSE ERROR ***:\n    {0}", ex.ToString().Replace("\n", "\n    ")));
                                }
                                break;

                            default:
                                try
                                {
                                    string s = Encoding.UTF8.GetString(data);
                                    allstr.AppendLine(string.Format("{1} = [\n{0}\n]", s, item.extension));
                                }
                                catch (Exception ex)
                                {
                                    allstr.AppendLine(string.Format("*** XXX = {1} RECORD PARSE ERROR ***:\n    {0}", ex.ToString().Replace("\n", "\n    "), item.extension));
                                }
                                break;

                        }
                    }
                }

                // also dump the output to file (for diagnostics)
                string path = Path.GetFullPath(Path.Combine(web_library_detail.LIBRARY_BASE_PATH, @"Qiqqa.DBexplorer.QueryDump.txt"));

                // overwrite previous query dump:
                using (StreamWriter sr = new StreamWriter(path, false /* overwrite */))
                {
                    sr.WriteLine(allstr);
                }

                TxtData.Text = allstr.ToString();
            }
        }

        private void ButtonPut_Click(object sender, RoutedEventArgs e)
        {
            if (null == web_library_detail)
            {
                MessageBoxes.Error("You must choose a library...");
                return;
            }

            string json = TxtData.Text;
            if (MessageBoxes.AskQuestion("Are you sure you want to write {0} characters to the database?", json.Length))
            {
                web_library_detail.Xlibrary.LibraryDB.PutString(TxtFingerprint.Text, TxtExtension.Text, json);
            }
        }

        private void TxtLibrary_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            web_library_detail = null;
            TxtLibrary.Text = "Click to choose a library.";

            // Pick a new library...
            WebLibraryDetail picked_web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != picked_web_library_detail)
            {
                web_library_detail = picked_web_library_detail;
                TxtLibrary.Text = picked_web_library_detail.Title;
            }

            e.Handled = true;
        }
    }
}
