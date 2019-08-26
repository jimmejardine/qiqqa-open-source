using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Maintainable;
using Utilities.Misc;
using Application = Microsoft.Office.Interop.Word.Application;
using Shape = Microsoft.Office.Interop.Word.Shape;

namespace Qiqqa.InCite
{
    public class WordConnector
    {
        public static WordConnector Instance = new WordConnector();

        static readonly string RTF_START = @"{\rtf1" + "\n";
        static readonly string RTF_END = @"}";
        
        bool paused;
        bool have_iterated_at_least_once = false;
        bool repopulating_clusters = false;

        Application word_application = null;

        string current_context_word = null;
        string current_context_backward = null;
        string current_context_surround = null;
        CitationCluster current_context_citation_cluster = null;

        public delegate void ContextChangedDelegate(string context_word, string context_backward, string context_surround);
        public event ContextChangedDelegate ContextChanged;

        public delegate void CitationClusterChangedDelegate(CitationCluster context_citation_cluster);
        public event CitationClusterChangedDelegate CitationClusterChanged;
        
        private WordConnector()
        {
            this.paused = false;
            MaintainableManager.Instance.RegisterHeldOffTask(DoMaintenance, 0, ThreadPriority.BelowNormal);
        }

        public void SetPaused(bool paused)
        {
            this.paused = paused;
        }

        void DoMaintenance(Daemon daemon)
        {
#if false
            if (Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
            {
                daemon.Sleep(60 * 1000);
                return;
            }
#endif

            if (paused || repopulating_clusters)
            {
                Logging.Info("WordConnector paused");
                daemon.Sleep(1500);
                return;
            }

            try
            {
                EnsureWordIsConnected();
                CheckTheCurrentTextContext();
                daemon.Sleep(1500);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem attaching to Word.");
                DisconnectFromWord();
                daemon.Sleep(1500);
            }

            have_iterated_at_least_once = true;
        }

        public void WaitForAtLeastOneIteration()
        {
            if (!have_iterated_at_least_once)
            {
                for (int i = 0; i < 20 && !have_iterated_at_least_once; ++i)
                {
                    Logging.Info("Waiting for the InCite thread to iterate once.");
                    Thread.Sleep(250);
                }
            }
        }

        private void CheckTheCurrentTextContext()
        {
            string context_word = null;
            string context_backward = null;
            string context_surround = null;

            CitationCluster context_citation_cluster = null;

            // The textual context
            {
                Selection selection = word_application.Selection;
                if (null != selection)
                {
                    Range range = selection.Range;

                    range.MoveStart(WdUnits.wdWord, -1);
                    range.MoveEnd(WdUnits.wdWord, -1);
                    range.MoveEnd(WdUnits.wdWord, +1);
                    context_word = range.Text;

                    range.MoveStart(WdUnits.wdWord, -9);
                    context_backward = range.Text;

                    range.MoveEnd(WdUnits.wdWord, 9);
                    context_surround = range.Text;

                    //Logging.Info("context_word is    : {0}", context_word);
                    //Logging.Info("context_backward is: {0}", context_backward);
                    //Logging.Info("context_surround is: {0}", context_surround);
                }
            }

            // The citation context
            {
                // First check the selected region
                CitationCluster citation_cluster;
                Field field;
                GetCurrentlySelectedCitationCluster(out citation_cluster, out field);
                context_citation_cluster = citation_cluster;
            }

            CheckForChangedContexts(context_word, context_backward, context_surround, context_citation_cluster);
        }

        private void GetCurrentlySelectedCitationCluster(out CitationCluster return_citation_cluster, out Field return_field)
        {
            CitationCluster context_citation_cluster = null;
            Field context_field = null;
            int context_citation_cluster_count = 0;

            // The current cursor position / selected
            Selection selection = word_application.Selection;
            if (null != selection)
            {
                Range range = selection.Range;

                // Widen around the current selection to sniff for fields that overlap our range
                Range range_to_scan = word_application.Selection.Range;
                range_to_scan.MoveStart(WdUnits.wdParagraph, -1);
                range_to_scan.MoveEnd(WdUnits.wdParagraph, +1);

                // Check each field
                foreach (Field field in range_to_scan.Fields)
                {
                    try
                    {
                        int field_start = Math.Min(field.Result.Start, field.Code.Start);
                        int field_end = Math.Max(field.Result.End, field.Code.End);
                        // Skip all ranges that do not overlap
                        if (field_start > range.End+1 || field_end < range.Start)
                        {
                            continue;
                        }

                        // We use the first one that does overlap
                        CitationCluster citation_cluster = GenerateCitationClusterFromField(field);
                        if (null != citation_cluster)
                        {
                            context_citation_cluster = citation_cluster;
                            context_field = field;
                            ++context_citation_cluster_count;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "Skipping field that can't be an InCite field.");
                        continue;
                    }
                }
            }

            // We only return a citation cluster if EXACTLY ONE has been selected
            if (1 == context_citation_cluster_count)
            {
                return_citation_cluster = context_citation_cluster;
                return_field = context_field;
            }
            else
            {
                return_citation_cluster = null;
                return_field = null;
            }
        }

        public void ReissueContextChanged()
        {
            ContextChanged?.Invoke(current_context_word, current_context_backward, current_context_surround);

            CitationClusterChanged?.Invoke(current_context_citation_cluster);
        }
        
        private void CheckForChangedContexts(string context_word, string context_backward, string context_surround, CitationCluster context_citation_cluster)
        {
            // Has text context changed?
            if (context_word != current_context_word || context_backward != current_context_backward || context_surround != current_context_surround)
            {
                current_context_word = context_word;
                current_context_backward = context_backward;
                current_context_surround = context_surround;

                ContextChanged?.Invoke(current_context_word, current_context_backward, current_context_surround);
            }

            // Has citation context changed?
            if ((context_citation_cluster == null ? null : context_citation_cluster.cluster_id) != (current_context_citation_cluster == null ? null : current_context_citation_cluster.cluster_id))
            {
                current_context_citation_cluster = context_citation_cluster;
                CitationClusterChanged?.Invoke(current_context_citation_cluster);
            }
        }

        private bool IsFieldInCiteField(Field field, string field_type)
        {
            try
            {
                if (WdFieldType.wdFieldMergeField == field.Type)
                {
                    string code = field.Code.Text.Trim();
                    string[] code_splits = code.Split(new char[] { ' ' }, 2);

                    if (code_splits[1].StartsWith(field_type))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem while inspecting field for citation cluster.");
            }

            return false;
        }

        private bool IsFieldBibliography(Field field)
        {
            return IsFieldInCiteField(field, InCiteFields.BIBLIOGRAPHY);
        }

        private bool IsFieldCSLStats(Field field)
        {
            return IsFieldInCiteField(field, InCiteFields.CSL_STATS);
        }

        
        private CitationCluster GenerateCitationClusterFromField(Field field)
        {
            try
            {
                if (WdFieldType.wdFieldMergeField == field.Type)
                {
                    string code = field.Code.Text.Trim();
                    string[] code_splits = code.Split(new char[] { ' ' }, 2);

                    if (code_splits[1].Contains(CitationCluster.QIQQA_CLUSTER))
                    {
                        return new CitationCluster(code_splits[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem while inspecting field for citation cluster.");
            }

            return null;
        }

        private static List<Field> GetDocumentFields(Document document)
        {
            List<Field> fields = new List<Field>();
            
            foreach (Field field in document.Fields)
            {
                fields.Add(field);
            }
            foreach (Endnote endnote in document.Endnotes)
            {
                foreach (Field field in endnote.Range.Fields)
                {
                    fields.Add(field);
                }
            }
            foreach (Footnote footnote in document.Footnotes)
            {
                foreach (Field field in footnote.Range.Fields)
                {
                    fields.Add(field);
                }
            }
            foreach (Shape shape in document.Shapes)
            {
                if (shape.Type == MsoShapeType.msoTextBox && 0 != shape.TextFrame.HasText)
                {
                    foreach (Field field in shape.TextFrame.TextRange.Fields)
                    {
                        fields.Add(field);
                    }
                }
            }

            return fields;
        }
        
        public List<CitationCluster> GetAllCitationClustersFromCurrentDocument()
        {
            HashSet<string> used_cluster_ids = new HashSet<string>();

            List<CitationCluster> citation_clusters = new List<CitationCluster>();
            
            Document document = word_application.Selection.Document;

            List<Field> fields = GetDocumentFields(document);
            foreach (Field field in fields)
            {
                CitationCluster citation_cluster = GenerateCitationClusterFromField(field);
                if (null != citation_cluster)
                {
                    // Check that this cluster id has not already been used - that happens if they have copied and pasted a field.
                    if (used_cluster_ids.Contains(citation_cluster.cluster_id))
                    {
                        Logging.Info("A cluster has been copied, so we are regenerating a new cluster id for {0}", citation_cluster);
                        citation_cluster.cluster_id = CitationCluster.GetRandomClusterId();
                        PopulateFieldWithRawCitationCluster(field, citation_cluster);
                    }

                    used_cluster_ids.Add(citation_cluster.cluster_id);
                    citation_clusters.Add(citation_cluster);
                }
            }
            
            return citation_clusters;
        }

        public void FindCitationCluster(CitationCluster target_citation_cluster)
        {
            Document document = word_application.Selection.Document;
            List<Field> fields = GetDocumentFields(document);
            foreach (Field field in fields)
            {
                CitationCluster citation_cluster = GenerateCitationClusterFromField(field);
                if (null != citation_cluster)
                {
                    if (citation_cluster.cluster_id == target_citation_cluster.cluster_id)
                    {
                        field.Select();
                        break;
                    }
                }
            }
        }

        void DisconnectFromWord()
        {
            CheckForChangedContexts(null, null, null, null);

            if (null != word_application)
            {
                try
                {
                    Marshal.FinalReleaseComObject(word_application);
                }
                catch (Exception ex2)
                {
                    Logging.Warn(ex2, "It wasn't pretty while detatching from Word.");
                }

                word_application = null;
            }
        }

        void EnsureWordIsConnected()
        {
            if (null == word_application)
            {
                Logging.Info("We do not have a connection with Word, so trying to get one");
                word_application = (Application)Marshal.GetActiveObject("Word.Application");

                Logging.Info("Word Version     : {0}", word_application.Version);
                Logging.Info("Word StartupPath : {0}", word_application.StartupPath);                
            }
        }

        private static void PopulateFieldWithRawCitationCluster(Field field, CitationCluster citation_cluster)
        {
            citation_cluster.rtf_hash = null;
            field.Code.Text = citation_cluster.ToCodedString();
            field.Result.Text = citation_cluster.GetBibTeXKeySummary();
        }

        internal void AppendCitation(CitationCluster citation_cluster)
        {
            if (0 == citation_cluster.citation_items.Count)
            {
                Logging.Warn("Not appending zero citations");
                return;
            }

            CitationCluster existing_citation_cluster;
            Field existing_field;
            GetCurrentlySelectedCitationCluster(out existing_citation_cluster, out existing_field);
            if (null != existing_citation_cluster)
            {
                existing_citation_cluster.citation_items.AddRange(citation_cluster.citation_items);
                PopulateFieldWithRawCitationCluster(existing_field, existing_citation_cluster);
                word_application.Activate();
            }
            else
            {
                // Shrink the selection to the end of it, so that we can add a citation after it...
                Range range = GetInsertionPointRange();
                Field field = range.Fields.Add(range, WdFieldType.wdFieldMergeField, citation_cluster.ToString(), true);
                field.Locked = true;
                PopulateFieldWithRawCitationCluster(field, citation_cluster);
                word_application.Activate();
            }
        }

        internal void ModifyCitation(CitationCluster citation_cluster)
        {
            Field field;
            CitationCluster currently_selected_citation_cluster;

            GetCurrentlySelectedCitationCluster(out currently_selected_citation_cluster, out field);
            if (null != currently_selected_citation_cluster && currently_selected_citation_cluster.cluster_id == citation_cluster.cluster_id)
            {
                PopulateFieldWithRawCitationCluster(field, citation_cluster);
                word_application.Activate();
            }
            else
            {
                Logging.Warn("Throwing away modified citation cluster because it is no longer selected: " + citation_cluster);
            }
        }


        internal void AddBibliography()
        {
            // Shrink the selection to the end of it, so that we can add a citation after it...
            Range range = GetInsertionPointRange();
            Field field = range.Fields.Add(range, WdFieldType.wdFieldMergeField, InCiteFields.BIBLIOGRAPHY, true);
            field.Locked = true;
            word_application.Activate();
        }

        internal void AddCSLStats()
        {
            // Shrink the selection to the end of it, so that we can add a citation after it...
            Range range = GetInsertionPointRange();
            Field field = range.Fields.Add(range, WdFieldType.wdFieldMergeField, InCiteFields.CSL_STATS, true);
            field.Locked = true;
            word_application.Activate();
        }

        private Range GetInsertionPointRange()
        {
            Range range_selected = word_application.Selection.Range;
            range_selected.SetRange(range_selected.End, range_selected.End);
            return range_selected;
        }

        internal void RepopulateFromCSLProcessor(object ip_object)
        {
            CSLProcessorOutputConsumer ip = (CSLProcessorOutputConsumer)ip_object;
            RepopulateFromCSLProcessor(ip, (CSLProcessor.BrowserThreadPassThru)ip.user_argument);
        }

        internal void RepopulateFromCSLProcessor(CSLProcessorOutputConsumer ip, CSLProcessor.BrowserThreadPassThru passthru)
        {
            if (null != ip.error_message)
            {
                RepopulateFromCSLProcessor_FAIL(ip, passthru);
            }
            else
            {
                RepopulateFromCSLProcessor_SUCCESS(ip, passthru);
            }

            StatusManager.Instance.UpdateStatus("InCite", "Updated InCite fields in Word");
        }

        internal void RepopulateFromCSLProcessor_FAIL(CSLProcessorOutputConsumer ip, CSLProcessor.BrowserThreadPassThru passthru)
        {
            // Dump the logs
            StringBuilder sb = new StringBuilder();
            foreach (string msg in ip.logs)
            {
                sb.AppendLine(msg);
            }
            sb.AppendLine();
            sb.AppendLine(ip.error_message);

            // Display the error
            string dialog_msg =
                "There was a problem while processing one of your references.  Please check this output to see if perhaps there is something strange in one of your BibTeX fields.  The error is sometimes preceded by ??????\n\n"
                + ip.error_message;

            MessageBoxes.Error("{0}", dialog_msg);
        }

        internal void RepopulateFromCSLProcessor_SUCCESS(CSLProcessorOutputConsumer ip, CSLProcessor.BrowserThreadPassThru passthru)
        {
            int MAX_DODGY_PASTE_RETRY_COUNT = 5;
            int DODGY_PASTE_RETRY_SLEEP_TIME_MS = 200;
            int dodgy_paste_retry_count = 0;

            try
            {
                // Suppress screen updates (for performance reasons)
                repopulating_clusters = true;
                word_application.ScreenUpdating = false;

                Document document = word_application.Selection.Document;

                // Get all the fields out there
                Logging.Info("+Enumerating all fields");
                List<Field> fields = GetDocumentFields(document);
                Logging.Info("-Enumerating all fields");

                StatusManager.Instance.ClearCancelled("InCite");
                for (int f = 0; f < fields.Count; ++f)
                {
                    if (StatusManager.Instance.IsCancelled("InCite"))
                    {
                        Logging.Info("Updating of citations in Word has been cancelled by the user.");
                        break;
                    }

                    Field field = fields[f];

                    StatusManager.Instance.UpdateStatus("InCite", "Updating InCite fields in Word", f, fields.Count, true);
                    word_application.StatusBar = String.Format("Updating InCite field {0} of {1}...", f, fields.Count);

                    try
                    {
                        // Do we have a citation that we can fill?
                        CitationCluster citation_cluster = GenerateCitationClusterFromField(field);
                        if (null != citation_cluster)
                        {
                            string text_for_cluster = ip.GetTextForCluster(citation_cluster.cluster_id);
                            string rtf_hash = GenerateRTFHash(passthru.is_note_format, text_for_cluster);

                            // Update this citation cluster only if it needs updating (if it has changed from what is currently stored in it)
                            if (!String.IsNullOrEmpty(text_for_cluster))
                            {
                                bool needs_update = false;

                                if (!needs_update)
                                {
                                    if (rtf_hash != citation_cluster.rtf_hash)
                                    {
                                        needs_update = true;
                                    }
                                }

                                if (!needs_update)
                                {
                                    string current_field_contents = field.Result.Text;
                                    if (current_field_contents.Contains("QIQQA"))
                                    {
                                        needs_update = true;
                                    }
                                }

                                if (needs_update)
                                {
                                    // Update the field with the new hash
                                    citation_cluster.rtf_hash = rtf_hash;
                                    PopulateFieldWithRawCitationCluster(field, citation_cluster);

                                    // Remember the font
                                    string font_name = field.Result.Font.Name;
                                    float font_size = field.Result.Font.Size;

                                    string rtf = RTF_START + text_for_cluster + RTF_END;
                                    ClipboardTools.SetText(rtf, TextDataFormat.Rtf);

                                    if (passthru.is_note_format)
                                    {
                                        field.Result.Text = "";
                                        Footnote footnote = field.Result.Footnotes.Add(field.Result);
                                        footnote.Range.Text = " ";
                                        Range range = footnote.Range;
                                        range.Collapse(WdCollapseDirection.wdCollapseStart);
                                        range.PasteSpecial(DataType: WdPasteDataType.wdPasteRTF);
                                        footnote.Range.Font.Name = font_name;
                                        footnote.Range.Font.Size = font_size;
                                    }
                                    else
                                    {
                                        field.Result.Text = " ";
                                        Range range = field.Result;
                                        range.Collapse(WdCollapseDirection.wdCollapseStart);
                                        range.PasteSpecial(DataType:WdPasteDataType.wdPasteRTF);
                                        field.Result.Text = field.Result.Text.Trim();
                                        field.Result.Font.Name = font_name;
                                        field.Result.Font.Size = font_size;
                                    }
                                }
                            }
                            else
                            {
                                citation_cluster.rtf_hash = rtf_hash;
                                PopulateFieldWithRawCitationCluster(field, citation_cluster);
                                field.Result.Text = String.Format("ERROR: Unable to find key {0} in the CSL output.", citation_cluster.cluster_id);
                            }

                            // If we get here, it must have worked!
                            dodgy_paste_retry_count = 0;

                            continue;
                        }

                        // Do we have a bibliography that we can fill?
                        if (IsFieldBibliography(field))
                        {
                            // Remember the font
                            string font_name = field.Result.Font.Name;
                            float font_size = field.Result.Font.Size;

                            string formatted_bibliography_section = ip.GetFormattedBibliographySection();
                            if (String.IsNullOrEmpty(formatted_bibliography_section))
                            {
                                formatted_bibliography_section = "Either this CSL citation style does not produce a bibliography section or you have not yet added any citations to this document, so this bibliography is empty.";
                            }

                            string formatted_bibliography_section_wrapped = CSLProcessorOutputConsumer.RTF_START + formatted_bibliography_section + CSLProcessorOutputConsumer.RTF_END;
                            
                            ClipboardTools.SetText(formatted_bibliography_section_wrapped, TextDataFormat.Rtf);

                            field.Result.Text = " ";
                            Range range = field.Result;
                            range.Collapse(WdCollapseDirection.wdCollapseStart);
                            range.PasteSpecial(DataType: WdPasteDataType.wdPasteRTF);
                            field.Result.Font.Name = font_name;
                            field.Result.Font.Size = font_size;

                            // If we get here, it must have worked!
                            dodgy_paste_retry_count = 0;

                            continue;
                        }

                        // Do we have a CSL stats region?
                        if (IsFieldCSLStats(field))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(RTF_START);
                            {
                                sb.AppendFormat("maxoffset={0}\\line\n", ip.max_offset);
                                sb.AppendFormat("entryspacing={0}\\line\n", ip.entry_spacing);
                                sb.AppendFormat("linespacing={0}\\line\n", ip.line_spacing);
                                sb.AppendFormat("hangingindent={0}\\line\n", ip.hanging_indent);
                                sb.AppendFormat("second_field_align={0}\\line\n", ip.second_field_align);

                            }
                            sb.Append(RTF_END);

                            Clipboard.SetText(sb.ToString(), TextDataFormat.Rtf);

                            field.Result.Text = " ";
                            Range range = field.Result;
                            range.Collapse(WdCollapseDirection.wdCollapseStart);
                            range.PasteSpecial(DataType: WdPasteDataType.wdPasteRTF);

                            // If we get here, it must have worked!
                            dodgy_paste_retry_count = 0;

                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        ++dodgy_paste_retry_count;
                        if (dodgy_paste_retry_count < MAX_DODGY_PASTE_RETRY_COUNT)
                        {
                            Logging.Warn(ex, "Will try again (try {0}) because there was a problem updating a citation field: {1}", dodgy_paste_retry_count, field.Result.Text);

                            // Back up one field so we can try again
                            Thread.Sleep(DODGY_PASTE_RETRY_SLEEP_TIME_MS);
                            --f;
                            continue;
                        }
                        else
                        {
                            Logging.Error(ex, "Giving up because there was a problem updating a citation field: {0}", field.Result.Text);
                            dodgy_paste_retry_count = 0;
                            continue;
                        }
                    }
                }
            }
            finally
            {
                // Restore the screen updates
                repopulating_clusters = false;
                word_application.ScreenUpdating = true;
                word_application.StatusBar = "Updated InCite fields.";
            }
        }

        private static string GenerateRTFHash(bool is_note_format, string text_for_cluster)
        {
            if (!String.IsNullOrEmpty(text_for_cluster))
            {
                return 
                    ""
                    + (is_note_format ? "0" : "1")
                    + StreamFingerprint.FromText(text_for_cluster).Substring(0,8)
                    ;
            }
            else
            {
                return null;
            }
        }
    }
}
