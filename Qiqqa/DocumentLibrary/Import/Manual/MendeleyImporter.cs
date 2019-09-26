using System;
using System.Text.RegularExpressions;
using Qiqqa.UtilisationTracking;
using Utilities;

namespace Qiqqa.DocumentLibrary.Import.Manual
{
    /// <summary>
    /// Also GenericBibTex importer.
    /// </summary>
    public class MendeleyImporter : BibTeXImporter
    {
        public MendeleyImporter(Library library, string filename, bool recordUsageAsGenericBibTeX)
            : base(library, filename)
        {
            if (recordUsageAsGenericBibTeX)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Library_ImportFromBibTeXGeneric);
            }
            else
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Library_ImportFromMendeley);
            }
        }

        public override ParseFileResult GetResult()
        {
            string matchFilenameRegex = "(.*?):(.*):(.*)";
            Regex rx = new Regex(matchFilenameRegex);

            foreach (var entry in Entries)
            {
                try
                {
                    Utilities.BibTex.Parsing.BibTexItem item = entry.Parsed;

                    #region Deal with file
                    if (item.ContainsField("file"))
                    {
                        string fn = item["file"];

                        Match match = rx.Match(fn);
                        if (match.Success)
                        {
                            // description:Applications/bla bla/bla/faq.pdf:file_type
                            entry.FileType = match.Groups[3].Value.ToLower();

                            // Qiqqa outputs this when exporting...
                            if (entry.FileType == "application/pdf")
                            {
                                entry.FileType = "pdf";
                            }

                            //For Mendeley, description and file_type in the above can be absent.
                            fn = match.Groups[2].Value;
                            fn = TranslateFilePath(fn);

                            entry.Filename = fn;
                        }
                        else
                        {
                            Logging.Warn("Non-conformant file key in Mendeley import:" + fn);
                        }
                    }
                    #endregion

                    // Handle notes

                    // Parse notes. 
                    string notes = item["annote"];
                    if (!String.IsNullOrEmpty(notes))
                    {
                        entry.Notes = notes;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "Exception while parsing Mendeley import");
                }
            }

            return CreateFinalResult();
        }
    }
}
