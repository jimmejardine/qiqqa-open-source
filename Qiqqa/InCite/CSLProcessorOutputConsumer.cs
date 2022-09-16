using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Gecko;
using Newtonsoft.Json.Linq;
using Utilities;
using Utilities.GUI;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.InCite
{
    public class CSLProcessorOutputConsumer : IDisposable
    {
        public static readonly string RTF_START = @"{\rtf1" + "\n";
        public static readonly string RTF_END = @"}";
        public static readonly string RTF_NEWLINE = @"\line" + "\n";

        public string citations_javascript;
        public BibliographyReadyDelegate brd;
        public object user_argument;

        // http://gsl-nagoya-u.net/http/pub/citeproc-doc.html#generating-bibliographies
        public int max_offset;
        public int entry_spacing;
        public int line_spacing;
        public int hanging_indent;
        public string second_field_align;
        public string bib_start;
        public string bib_end;

        private Dictionary<int, string> position_to_inline = new Dictionary<int, string>();
        private Dictionary<string, List<int>> inline_to_positions = new Dictionary<string, List<int>>();
        public Dictionary<int, string> position_to_text = new Dictionary<int, string>();
        public List<string> bibliography = new List<string>();
        public List<string> logs = new List<string>();
        public string error_message = null;

        public bool success;
        private GeckoWebBrowser web_browser;

        public delegate void BibliographyReadyDelegate(CSLProcessorOutputConsumer ip);

        public CSLProcessorOutputConsumer(string script_directory, string citations_javascript, BibliographyReadyDelegate brd, object user_argument)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            this.citations_javascript = citations_javascript;
            this.brd = brd;
            this.user_argument = user_argument;

            // Create the browser
            Logging.Info("Creating web browser for InCite CSL processing");
            web_browser = new GeckoWebBrowser();
            web_browser.CreateControl();

            // Add the name of the script to run
            script_directory = Path.GetFullPath(Path.Combine(script_directory, @"runengine.html"));
            script_directory = script_directory.Replace(@"\\", @"\");
            script_directory = script_directory.Replace(@"//", @"/");

            Uri uri = new Uri(script_directory);
            Logging.Info("CSLProcessorOutputConsumer is about to browse to {0}", uri);

            // This is the only way we can communicate from JavaScript to .NET!!
            web_browser.EnableConsoleMessageNotfication();
            web_browser.ConsoleMessage += web_browser_ConsoleMessage;

            // Kick off citeproc computation
            web_browser.Navigate(uri.ToString());
        }

        private void web_browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                Logging.Debug特("JAVASCRIPT CONSOLE MESSAGE: {0}", e.Message);

                try
                {
                    if (finished_processing)
                    {
                        Logging.Info("Finished processing, so ignoring InCite JavaScript error: {0}", e.Message);
                        return;
                    }

                    if (e.Message.Contains("Permission denied"))
                    {
                        Logging.Info("Skipping known exception: {0}", e.Message);
                        return;
                    }

                    if (e.Message.Contains("SyntaxError"))
                    {
                        finished_processing = true;

                        Logging.Info("Acting on InCite syntax error: {0}", e.Message);

                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"line:\s+(\d+)\s+column:\s+(\d+)\s+");
                        System.Text.RegularExpressions.MatchCollection matches = regex.Matches(e.Message);
                        if (0 < matches.Count)
                        {
                            int line = Int32.Parse(matches[0].Groups[1].Value);
                            int pos = Int32.Parse(matches[0].Groups[2].Value);

                            string[] citations_javascript_lines = citations_javascript.Split(new char[] { '\n' });

                            int min = (int)Math.Max(0, line - 1 - 15);
                            int max = (int)Math.Min(citations_javascript_lines.Length, line - 1 + 5);
                            StringBuilder sb_error_region = new StringBuilder();
                            sb_error_region.AppendFormat("Syntax error ({1},{2}): {0}\n", e.Message, line, pos);
                            for (int i = min; i <= max; ++i)
                            {
                                string indicator = (i == line - 1) ? "?????? " : "       ";
                                sb_error_region.AppendFormat("{0}{1}\n", indicator, citations_javascript_lines[i]);
                            }

                            error_message = sb_error_region.ToString();
                        }
                        else
                        {
                            error_message = e.Message;
                        }

                        Logging.Info("Calling the BibliographyReadyDelegate");
                        success = false;
                        brd(this);
                        Logging.Info("Called the BibliographyReadyDelegate");
                    }

                    if (e.Message.Contains("INCITE_FINISHED"))
                    {
                        finished_processing = true;

                        Logging.Info("Acting on signalling InCite JavaScript error: {0}", e.Message);

                        Logging.Info("Received citeproc results");
                        {
                            string json_output_encoded = web_browser.DomDocument.GetHtmlElementById("ObjOutput").InnerHtml;
                            string json_output = WebUtility.HtmlDecode(json_output_encoded);
                            JObject json = JObject.Parse(json_output);

                            // Pull out the inlines
                            {
                                Logging.Info("Pulling out the inlines");
                                foreach (var inline in json["inlines"])
                                {
                                    string key = (string)inline["citation"]["citationID"];
                                    int total_items_affected_by_key = inline["citation_output"].Count();
                                    foreach (var citation_output in inline["citation_output"])
                                    {
                                        int position = (int)citation_output[0];
                                        string text = (string)citation_output[1];

                                        SetInline(key, position, text, total_items_affected_by_key);
                                    }
                                }
                            }

                            // Pull out the bibliography
                            {
                                Logging.Info("Pulling out the bibliography settings");

                                // Do these results lack a bibliography?
                                JToken has_bibliography = json["bibliography"] as JToken;
                                if (null != has_bibliography && JTokenType.Boolean == has_bibliography.Type)
                                {
                                    Logging.Info("This style has no bibliography: json bibliography node is {0}", has_bibliography);
                                }
                                else
                                {
                                    var settings = json["bibliography"][0];
                                    SetBibliographySettings(
                                        (int)(settings["maxoffset"] ?? 0),
                                        (int)(settings["entryspacing"] ?? 0),
                                        (int)(settings["linespacing"] ?? 0),
                                        (int)(settings["hangingindent"] ?? 0),
                                        Convert.ToString(settings["second-field-align"] ?? ""),
                                        (string)(settings["bibstart"] ?? ""),
                                        (string)(settings["bibend"] ?? "")
                                    );

                                    Logging.Info("Pulling out the bibliography");
                                    var bibliography = json["bibliography"][1];
                                    foreach (var bib in bibliography)
                                    {
                                        SetBibliography((string)bib);
                                    }
                                }
                            }

                            Logging.Debug特("Calling the BibliographyReadyDelegate");
                            success = true;
                            brd(this);
                            Logging.Debug特("Called the BibliographyReadyDelegate");
                        }

                        if (finished_processing)
                        {
                            // Clean up
                            if (null != web_browser)
                            {
                                Logging.Debug特("Disposing of web browser for InCite CSL processing");
                                web_browser.Dispose();
                                web_browser = null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem in the callback from citeproc");
                }
            });
        }

        private bool finished_processing = false;

        private void web_browser_JavascriptError(object sender, JavascriptErrorEventArgs e)
        {
        }

        private void SetInline(string key, int position, string text, int total_items_affected_by_key)
        {
            // Trim off all whitespace
            text = text.Trim();

            // Sometimes citeproc-js returns some dodgy stuff...
            string[] DOUBLE_PAGE_SUFFICES = new string[]
            {
                ", p.", ", pp.", ", chap.", ", sec." ,
                ": p.", ": pp.", ": chap.", ": sec." ,
            };
            foreach (string DOUBLE_PAGE_SUFFIX in DOUBLE_PAGE_SUFFICES)
            {
                if (text.EndsWith(DOUBLE_PAGE_SUFFIX))
                {
                    Logging.Info("Stripping off " + DOUBLE_PAGE_SUFFIX);
                    text = text.Substring(0, text.Length - DOUBLE_PAGE_SUFFIX.Length);
                }
            }

            // Store the text at this position
            position_to_text[position] = text;

            // If we have never seen this position before, associate it with the key
            if (!position_to_inline.ContainsKey(position))
            {
                // Remember the key with which this position is associated
                position_to_inline[position] = key;

                // Remember the positions with which this key is associated
                if (!inline_to_positions.ContainsKey(key))
                {
                    inline_to_positions[key] = new List<int>();
                }
                inline_to_positions[key].Add(position);
            }
        }

        private void SetBibliographySettings(
            int max_offset,
            int entry_spacing,
            int line_spacing,
            int hanging_indent,
            string second_field_align,
            string bib_start,
            string bib_end
        )
        {
            this.max_offset = max_offset;
            this.entry_spacing = entry_spacing;
            this.line_spacing = line_spacing;
            this.hanging_indent = hanging_indent;
            this.second_field_align = second_field_align;
            this.bib_start = bib_start;
            this.bib_end = bib_end;
        }

        private void SetBibliography(string bibliography)
        {
            // Some polish
            string line_polished = bibliography;
            {
                line_polished = line_polished.Replace("  ", " ");
                line_polished = line_polished.Replace(". .", ".");
                line_polished = line_polished.Replace(". ,", ".,");
                line_polished = line_polished.Replace("., &", ". &");
            }

            this.bibliography.Add(line_polished);
        }

        internal IEnumerable<string> GetCitationClusterKeys()
        {
            return inline_to_positions.Keys;
        }

        internal string GetTextForCluster(string key)
        {
            if (inline_to_positions.ContainsKey(key))
            {
                StringBuilder sb = new StringBuilder();
                bool already_have_one = false;
                foreach (int position in inline_to_positions[key])
                {
                    if (already_have_one)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(position_to_text[position]);

                    already_have_one = true;
                }
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        internal string GetFormattedBibliographySection()
        {
            StringBuilder sb = new StringBuilder();

            if (0 < bibliography.Count)
            {
                if (0 < hanging_indent || 0 < max_offset)
                {
                    sb.Append(@"\li" + (hanging_indent + max_offset) * 130);
                    sb.Append(@"\fi-" + (hanging_indent + max_offset) * 130);
                    sb.Append("\n");
                }

                foreach (string line in bibliography)
                {
                    sb.Append(line);

                    for (int i = 0; i < line_spacing; ++i)
                    {
                        sb.Append(RTF_NEWLINE);
                    }

                    sb.Append(@"\par");
                    sb.Append("\n");
                }
            }

            return sb.ToString();
        }

        internal string GetRtf()
        {
            StringBuilder sb = new StringBuilder();

            if (0 < bibliography.Count)
            {
                sb.Append(CSLProcessorOutputConsumer.RTF_START);
                foreach (string line in bibliography)
                {
                    sb.Append(line);
                    sb.Append(@"\par");
                    sb.Append(@"\line");
                    sb.Append("\n");
                }
                sb.Append(CSLProcessorOutputConsumer.RTF_END);
            }

            else
            {
                sb.Append(CSLProcessorOutputConsumer.RTF_START);
                foreach (string line in position_to_text.Values)
                {
                    sb.Append(line);
                    sb.Append(@"\par");
                    sb.Append(@"\line");
                    sb.Append("\n");
                }
                sb.Append(CSLProcessorOutputConsumer.RTF_END);
            }

            return sb.ToString();
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~CSLProcessorOutputConsumer()
        {
            Logging.Debug("~CSLProcessorOutputConsumer()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing CSLProcessorOutputConsumer");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("CSLProcessorOutputConsumer::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        // Get rid of managed resources
                        web_browser?.Dispose();
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    web_browser = null;

                    brd = null;
                    user_argument = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    position_to_inline.Clear();
                    inline_to_positions.Clear();
                    position_to_text.Clear();
                    bibliography.Clear();
                    logs.Clear();
                });

                ++dispose_count;
            });
        }

        #endregion
    }
}
