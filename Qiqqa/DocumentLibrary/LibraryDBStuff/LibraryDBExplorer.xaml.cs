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
        private Library library = null;

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
            if (null == library)
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

            var items = library.LibraryDB.GetLibraryItems(TxtFingerprint.Text, TxtExtension.Text, MaxRecordCount);
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

                    try
                    {
                        PDFDocument doc = PDFDocument.LoadFromMetaData(library, item.data, null);
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
                            BibTexItem bibtex = doc.BibTex;
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
                }

                // also dump the output to file (for diagnostics)
                string path = Path.GetFullPath(Path.Combine(library.LIBRARY_BASE_PATH, @"Qiqqa.DBexplorer.QueryDump.txt"));

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
            if (null == library)
            {
                MessageBoxes.Error("You must choose a library...");
                return;
            }

            string json = TxtData.Text;
            if (MessageBoxes.AskQuestion("Are you sure you want to write {0} characters to the database?", json.Length))
            {
                library.LibraryDB.PutString(TxtFingerprint.Text, TxtExtension.Text, json);
            }
        }

        private void TxtLibrary_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            library = null;
            TxtLibrary.Text = "Click to choose a library.";

            // Pick a new library...
            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != web_library_detail)
            {
                library = web_library_detail.library;
                TxtLibrary.Text = web_library_detail.Title;
            }

            e.Handled = true;
        }
    }
}
