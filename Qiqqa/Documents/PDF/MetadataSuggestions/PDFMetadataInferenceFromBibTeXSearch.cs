using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF.Search;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Files;
using Utilities.Internet;
using Utilities.Strings;

namespace Qiqqa.Documents.PDF.MetadataSuggestions
{
    internal class PDFMetadataInferenceFromBibTeXSearch
    {
        private static BibTeXSearchServerManager bibtex_search_server_manager = new BibTeXSearchServerManager();

        internal static bool NeedsProcessing(PDFDocument pdf_document)
        {
            // Don't allow info to be sent if they have turned off feedback
            if (!ConfigurationManager.Instance.ConfigurationRecord.Feedback_UtilisationInfo)
            {
                return false;
            }

            // Don't allow if there has been a recent problem connecting to bibtexsearch
            if (MustBackoff()) return false;
            if (!pdf_document.DocumentExists) return false;
            if (!String.IsNullOrEmpty(pdf_document.BibTex)) return false;
            if (pdf_document.AutoSuggested_BibTeXSearch) return false;
            if (!ConfigurationManager.Instance.ConfigurationRecord.Metadata_AutomaticallyAssociateBibTeX) return false;

            Logging.Info("{0} requires PDFMetadataInferenceFromBibTeXSearch", pdf_document.Fingerprint);
            return true;
        }

        internal static bool InferBibTeX(PDFDocument pdf_document, bool manual_override)
        {
            if (MustBackoff() && !manual_override) return false;
            if (!pdf_document.DocumentExists) return false;
            if (!String.IsNullOrEmpty(pdf_document.BibTex) && !manual_override) return false;
            if (pdf_document.AutoSuggested_BibTeXSearch && !manual_override) return false;
            if (!ConfigurationManager.Instance.ConfigurationRecord.Metadata_AutomaticallyAssociateBibTeX && !manual_override) return false;

            // Flag on this document that we have tried to do the bibtex
            pdf_document.AutoSuggested_BibTeXSearch = true;
            pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.AutoSuggested_BibTeXSearch);

            string title = pdf_document.TitleCombined;

            title = title.Trim();
            if (String.IsNullOrEmpty(title)) return false;
            if (title.Length < 10) return false;

            // If there is only a single word in the title, it is not useful to us...
            if (-1 == title.IndexOf(' ')) return false;

            // Unwanted automatic titles
            if (Constants.TITLE_UNKNOWN == title || pdf_document.DownloadLocation == title) return false;

            // Get the search results!
            string json = DoSearch(title);
            if (null != json)
            {
                object o = JsonConvert.DeserializeObject(json);
                JArray ja = (JArray)o;

                // Get the bibtexes that suit this document
                List<string> bibtex_choices = new List<string>();
                foreach (var jo in ja)
                {
                    var bibtex = jo["_source"]["bibtex"].ToString();
                    if (String.IsNullOrEmpty(bibtex)) continue;

                    BibTexItem bibtex_item = BibTexParser.ParseOne(bibtex, true);

                    // Does the bibtex match sufficiently? Empty bibtex will be handled accordingly: no fit/match
                    PDFSearchResultSet search_result_set;
                    if (!BibTeXGoodnessOfFitEstimator.DoesBibTeXMatchDocument(bibtex_item, pdf_document, out search_result_set)) continue;

                    // Does the title match sufficiently to the bibtex
                    {
                        string title_string = BibTexTools.GetTitle(bibtex_item);
                        string title_string_tolower = title_string.Trim().ToLower();
                        string title_tolower = title.Trim().ToLower();
                        double similarity = StringTools.LewensteinSimilarity(title_tolower, title_string_tolower);
                        if (0.75 > similarity) continue;
                    }

                    if (!bibtex.Contains(BibTeXActionComments.AUTO_BIBTEXSEARCH))
                    {
                        bibtex =
                            BibTeXActionComments.AUTO_BIBTEXSEARCH
                            + "\r\n"
                            + bibtex;
                    }

                    // If we get this far, we are happy with the bibtex
                    bibtex_choices.Add(bibtex);
                }

                // Pick the longest matching bibtex
                if (0 < bibtex_choices.Count)
                {
                    bibtex_choices.Sort(delegate (string a, string b)
                    {
                        if (a.Length > b.Length) return -1;
                        if (a.Length < b.Length) return +1;
                        return 0;
                    }
                    );

                    pdf_document.BibTex = bibtex_choices[0];
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.BibTex);

                    FeatureTrackingManager.Instance.UseFeature(Features.BibTeX_BibTeXSearchMatch);

                    return true;
                }
            }

            return false;
        }

        private static bool MustBackoff()
        {
            return bibtex_search_server_manager.MustBackoff();
        }

        private static string DoSearch(string title)
        {
            try
            {
                string title_encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(title));

                string auth = title;
                if (0 < auth.Length)
                {
                    // construct key for bibtexsearch.com authentication hash: 
                    auth = auth[0] + auth + auth[0];
                }
                auth = StreamFingerprint.FromText(auth);

                string url_server = bibtex_search_server_manager.GetServerUrl();
                string url = String.Format("{0}/search?auth={1}&qe={2}", url_server, auth, WebUtility.HtmlEncode(title_encoded));
                try
                {
                    WebHeaderCollection header_collection;
                    Stopwatch clk = Stopwatch.StartNew();

                    using (MemoryStream ms = UrlDownloader.DownloadWithBlocking(url, out header_collection))
                    {
                        bibtex_search_server_manager.ReportLatency(url_server, clk.ElapsedMilliseconds);
                        Logging.Debug特("bibtex_search_server_manager: Download {0} took {1} ms", url, clk.ElapsedMilliseconds);

                        string json = Encoding.UTF8.GetString(ms.ToArray());
                        return json;
                    }
                }
                catch (Exception ex)
                {
                    bibtex_search_server_manager.ReportError(url_server);
                    Logging.Warn(ex, "There was a problem searching for BibTeX for title '{0}' at server '{1}'.", title, url_server);
                }
            }

            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem searching for BibTeX for title '{0}'.", title);
            }

            return null;
        }
    }
}
