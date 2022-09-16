using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using ICSharpCode.AvalonEdit.Folding;
using Microsoft.Win32;
using Qiqqa.Common.Configuration;
using Qiqqa.UtilisationTracking;
using Utilities.BibTex.Parsing;
using Utilities.Files;
using Utilities.GUI.DualTabbedLayoutStuff;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.InCite.CSLEditorStuff
{
    /// <summary>
    /// Interaction logic for CSLEditorControl.xaml
    /// </summary>
    public partial class CSLEditorControl : UserControl
    {
        private readonly FoldingManager folding_manager;
        private readonly XmlFoldingStrategy folding_strategy;

        private string last_filename = null;

        public CSLEditorControl()
        {
            InitializeComponent();

            // Toolbar
            ButtonNew.Icon = Icons.GetAppIcon(Icons.New);
            ButtonNew.Caption = "New";
            ButtonNew.Click += ButtonNew_Click;
            ButtonOpen.Caption = "Open";
            ButtonOpen.Icon = Icons.GetAppIcon(Icons.Open);
            ButtonOpen.Click += ButtonOpen_Click;
            ButtonSave.Caption = "Save ";
            ButtonSave.Icon = Icons.GetAppIcon(Icons.Save);
            ButtonSave.Click += ButtonSave_Click;
            ButtonSaveAs.Caption = "Save As";
            ButtonSaveAs.Icon = Icons.GetAppIcon(Icons.SaveAs);
            ButtonSaveAs.Click += ButtonSaveAs_Click;
            ButtonRefresh.Caption = "Refresh (F5)";
            ButtonRefresh.Icon = Icons.GetAppIcon(Icons.Refresh);
            ButtonRefresh.Click += ButtonRefresh_Click;
            ButtonHelp.Caption = "Help";
            ButtonHelp.Icon = Icons.GetAppIcon(Icons.Help);
            ButtonHelp.Click += ButtonHelp_Click;



            // Fix the DualTab
            DualTabTags.Children.Clear();
            DualTabTags.AddContent("CSL", "CSL", null, false, false, ObjCSLEditorPanel);
            DualTabTags.AddContent("BibTeX", "BibTeX", null, false, false, ObjBibTexEditorPanel);
            DualTabTags.AddContent("JavaScript", "JavaScript", null, false, false, ObjJavaScriptEditorPanel);
            DualTabTags.MakeActive("CSL");
            DualTabTags.TabPosition = DualTabbedLayout.TabPositions.Sides;

            DragToEditorManager.RegisterControl(ObjCSLEditor);
            DragToEditorManager.RegisterControl(ObjBibTexEditor);
            DragToEditorManager.RegisterControl(ObjJavaScriptEditor);

            // Code folding
            folding_manager = FoldingManager.Install(ObjCSLEditor.TextArea);
            folding_strategy = new XmlFoldingStrategy();
            folding_strategy.ShowAttributesWhenFolded = true;
            ObjCSLEditor.TextArea.Options.ShowSpaces = true;
            ObjCSLEditor.TextArea.Options.ShowTabs = true;
            ObjCSLEditor.TextArea.Options.ShowBoxForControlCharacters = true;
            ObjCSLEditor.TextArea.Options.EnableHyperlinks = true;
            ObjCSLEditor.TextArea.Options.EnableEmailHyperlinks = true;
            ObjCSLEditor.TextArea.Options.RequireControlModifierForHyperlinkClick = true;
            ObjCSLEditor.TextArea.Options.EnableRectangularSelection = true;
            ObjCSLEditor.TextArea.Options.EnableTextDragDrop = true;
            ObjCSLEditor.TextArea.Options.CutCopyWholeLine = true;

            // Events
            ObjCSLEditor.TextChanged += ObjCSLEditor_TextChanged;
            ObjBibTexEditor.TextChanged += ObjBibTexEditor_TextChanged;

            // Load the samples
            string sample_csl_filename = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"InCite/styles/apa.csl"));
            ObjCSLEditor.Text = File.ReadAllText(sample_csl_filename);
            string sample_bibtex_filename = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"InCite/CSLEditorStuff/SampleBibTeX.txt"));
            ObjBibTexEditor.Text = File.ReadAllText(sample_bibtex_filename);

            // Bind the keys
            PreviewKeyDown += CSLEditorControl_PreviewKeyDown;
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.OpenOffsiteUrl(WebsiteAccess.Url_CSLManual);
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            last_filename = null;
            ObjCSLEditor.Text = "";
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_CSLEditorRefresh);

            RunItJoe();
            e.Handled = true;
        }

        private void ButtonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.AddExtension = true;
            save_file_dialog.CheckPathExists = true;
            save_file_dialog.DereferenceLinks = true;
            save_file_dialog.OverwritePrompt = true;
            save_file_dialog.ValidateNames = true;
            save_file_dialog.DefaultExt = "csl";
            save_file_dialog.Filter = "CSL files (*.csl)|*.csl|All files (*.*)|*.*";
            save_file_dialog.FileName = last_filename;

            if (true == save_file_dialog.ShowDialog())
            {
                last_filename = save_file_dialog.FileName;
                DoSave(save_file_dialog.FileName);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // Check that we have something to save, otherwise do SaveAs
            if (String.IsNullOrEmpty(last_filename))
            {
                ButtonSaveAs_Click(sender, e);
                return;
            }

            DoSave(last_filename);
        }

        private void DoSave(string filename)
        {
            File.WriteAllText(filename, ObjCSLEditor.Text);
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (Runtime.IsRunningInVisualStudioDesigner) return;
#endif

            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.AddExtension = true;
            open_file_dialog.CheckPathExists = true;
            open_file_dialog.CheckFileExists = true;
            open_file_dialog.DereferenceLinks = true;
            open_file_dialog.ValidateNames = true;
            open_file_dialog.DefaultExt = "csl";
            open_file_dialog.Filter = "CSL files (*.csl)|*.csl|All files (*.*)|*.*";
            open_file_dialog.FileName = last_filename;

            if (true == open_file_dialog.ShowDialog())
            {
                last_filename = open_file_dialog.FileName;
                ObjCSLEditor.Text = File.ReadAllText(open_file_dialog.FileName);
            }
        }

        private void CSLEditorControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.F5 == e.Key)
            {
                e.Handled = true;
                RunItJoe();
            }
        }

        private void ObjCSLEditor_TextChanged(object sender, EventArgs e)
        {
            folding_strategy.UpdateFoldings(folding_manager, ObjCSLEditor.Document);
        }

        private void ObjBibTexEditor_TextChanged(object sender, EventArgs e)
        {
            RefreshJavaScriptEditor();
        }

        private void RefreshJavaScriptEditor()
        {
            List<BibTexItem> bibtex_items_list = BibTexParser.Parse(ObjBibTexEditor.Text).Items;
            Dictionary<string, CSLProcessorBibTeXFinder.MatchingBibTeXRecord> bitex_items = new Dictionary<string, CSLProcessorBibTeXFinder.MatchingBibTeXRecord>();
            bibtex_items_list.ForEach(o => bitex_items[o.Key] = new CSLProcessorBibTeXFinder.MatchingBibTeXRecord { bibtex_item = o });

            // Sample citation clusters
            List<CitationCluster> citation_clusters = new List<CitationCluster>();
            bibtex_items_list.ForEach(o => citation_clusters.Add(new CitationCluster(new CitationItem(o.Key, ""))));
            CitationCluster mega_citation_cluster = new CitationCluster();
            bibtex_items_list.ForEach(o => mega_citation_cluster.citation_items.Add(new CitationItem(o.Key, "")));
            citation_clusters.Add(mega_citation_cluster);

            Dictionary<string, string> abbreviations = new Dictionary<string, string>();

            string citation_uses = CSLProcessorTranslator_CitationClustersToJavaScript.Translate(citation_clusters);
            string citation_init = CSLProcessorTranslator_BibTeXToJavaScript.Translate_INIT(bitex_items);
            string citation_database = CSLProcessorTranslator_BibTeXToJavaScript.Translate_DATABASE(bitex_items, abbreviations);

            string final_text = ""
                + citation_uses
                + "\n"
                + citation_init
                + "\n"
                + citation_database
                ;

            ObjJavaScriptEditor.Text = final_text;
        }

        private void RunItJoe()
        {
            string csl = ObjCSLEditor.Text;
            string prepared_citation_javascript = ObjJavaScriptEditor.Text;

            // Write to file for processor
            string style_file_filename = Path.GetFullPath(Path.Combine(TempFile.TempDirectoryForQiqqa, @"CSLEditor_CSL.csl"));
            File.WriteAllText(style_file_filename, csl);

            // Validate the CSL
            List<string> csl_parse_results = CSLVerifier.Verify(style_file_filename);
            if (0 < csl_parse_results.Count)
            {
                foreach (string line in csl_parse_results)
                {
                    LogMessage(line);
                }
            }

            CSLProcessor.GenerateCSLEditorCitations(style_file_filename, prepared_citation_javascript, OnBibliographyReady);
        }

        private void OnBibliographyReady(CSLProcessorOutputConsumer ip)
        {
            StringBuilder result_rtf = new StringBuilder();


            result_rtf.Append(CSLProcessorOutputConsumer.RTF_START);

            if (!ip.success)
            {
                result_rtf.Append("---------------------------------------------");
                result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
                result_rtf.Append("There was a problem processing the CSL.");
                result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
                result_rtf.Append("---------------------------------------------");
                result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
                result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            }


            result_rtf.Append("---------------------------------------------");
            result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            result_rtf.Append("Citations:");
            result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            result_rtf.Append("---------------------------------------------");
            result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            foreach (string cluster_id in ip.GetCitationClusterKeys())
            {
                string text_for_cluster = ip.GetTextForCluster(cluster_id);
                result_rtf.Append(text_for_cluster);
                result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            }

            result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            result_rtf.Append("---------------------------------------------");
            result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            result_rtf.Append("Bibliography:");
            result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            result_rtf.Append("---------------------------------------------");
            result_rtf.Append(CSLProcessorOutputConsumer.RTF_NEWLINE);
            result_rtf.Append(ip.GetFormattedBibliographySection());

            result_rtf.Append(CSLProcessorOutputConsumer.RTF_END);

            // Set the box
            //string formatted_bibliography_section = ip.GetFormattedBibliographySection();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(result_rtf.ToString())))
            {
                ObjRichTextEditor.SelectAll();
                ObjRichTextEditor.Selection.Load(ms, DataFormats.Rtf);
            }

            // Display any errors
            LogMessage(ip.error_message);

            LogMessage("Done!");
        }


        private void LogMessage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                ObjErrorEditor.Text += DateTime.Now.ToLongTimeString() + " - " + message + "\n";
                ObjErrorEditorScroller.ScrollToBottom();
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            CSLEditorControl lec = new CSLEditorControl();
            ControlHostingWindow chw = new ControlHostingWindow("CSL Editor", lec);
            chw.Show();
        }
#endif

        #endregion
    }
}
