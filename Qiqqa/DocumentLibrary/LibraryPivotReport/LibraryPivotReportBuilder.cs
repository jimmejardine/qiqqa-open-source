using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qiqqa.DocumentLibrary.LibraryFilter.AITagExplorerStuff;
using Qiqqa.DocumentLibrary.LibraryFilter.AuthorExplorerStuff;
using Qiqqa.DocumentLibrary.LibraryFilter.GeneralExplorers;
using Qiqqa.DocumentLibrary.LibraryFilter.PublicationExplorerStuff;
using Qiqqa.DocumentLibrary.TagExplorerStuff;
using Qiqqa.Documents.PDF;
using Syncfusion.Windows.Controls.Grid;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.Mathematics;
using Utilities.Misc;

namespace Qiqqa.DocumentLibrary.LibraryPivotReport
{
    public static class LibraryPivotReportBuilder
    {
        public class PivotResult
        {
            public List<string> y_keys;
            public List<string> x_keys;
            public List<string>[,] common_fingerprints;
        }

        public static readonly string[] AXIS_CHOICES = new string[]
        {
            "None"
            ,"Tag"
            ,"Ratings"
            ,"ReadingStage"
            ,"Author"
            ,"Year"
            ,"AutoTag"
            ,"Publication"
            ,"Theme"
        };

        public static MultiMapSet<string, string> GenerateAxisMap(string axis_name, Library library, HashSet<string> parent_fingerprints)
        {
            switch (axis_name)
            {
                case "Tag": return TagExplorerControl.GetNodeItems(library, parent_fingerprints);
                case "Ratings": return RatingExplorerControl.GetNodeItems(library, parent_fingerprints);
                case "ReadingStage": return ReadingStageExplorerControl.GetNodeItems(library, parent_fingerprints);
                case "Author": return AuthorExplorerControl.GetNodeItems(library, parent_fingerprints);
                case "Year": return YearExplorerControl.GetNodeItems(library, parent_fingerprints);
                case "AutoTag": return AITagExplorerControl.GetNodeItems(library, parent_fingerprints);
                case "Publication": return PublicationExplorerControl.GetNodeItems(library, parent_fingerprints);
                case "Theme": return ThemeExplorerControl.GetNodeItems_STATIC(library, parent_fingerprints);
                case "Type": return TypeExplorerControl.GetNodeItems(library, parent_fingerprints);

                default:
                    Logging.Warn("Unknown pivot axis {0}", axis_name);
                    return GenerateMap_None(library, parent_fingerprints);
            }
        }

        private static MultiMapSet<string, string> GenerateMap_None(Library library, HashSet<string> parent_fingerprints)
        {
            List<PDFDocument> pdf_documents = null;
            if (null == parent_fingerprints)
            {
                pdf_documents = library.PDFDocuments;
            }
            else
            {
                pdf_documents = library.GetDocumentByFingerprints(parent_fingerprints);
            }
            Logging.Debug特("LibraryPivotExplorerControl: processing {0} documents from library {1}", pdf_documents.Count, library.WebLibraryDetail.Title);

            MultiMapSet<string, string> tags_with_fingerprints = new MultiMapSet<string, string>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                tags_with_fingerprints.Add("All", pdf_document.Fingerprint);
            }
            return tags_with_fingerprints;
        }

        public static PivotResult GeneratePivot(MultiMapSet<string, string> map_y_axis, MultiMapSet<string, string> map_x_axis)
        {
            List<string> y_keys = new List<string>(map_y_axis.Keys);
            List<string> x_keys = new List<string>(map_x_axis.Keys);

            y_keys.Sort();
            x_keys.Sort();

            List<string>[,] common_fingerprints = new List<string>[y_keys.Count, x_keys.Count];

            StatusManager.Instance.ClearCancelled("LibraryPivot");
            int y_progress = 0;
            Parallel.For(0, y_keys.Count, (y, loop_state) =>
            //for (int y = 0; y < y_keys.Count; ++y)
            {
                int y_progress_locked = Interlocked.Increment(ref y_progress);

                if (General.HasPercentageJustTicked(y_progress_locked, y_keys.Count))
                {
                    StatusManager.Instance.UpdateStatus("LibraryPivot", "Building library pivot", y_progress_locked, y_keys.Count, true);

                    WPFDoEvents.WaitForUIThreadActivityDone(); // HackityHack

                    if (StatusManager.Instance.IsCancelled("LibraryPivot"))
                    {
                        Logging.Warn("User cancelled library pivot generation");
                        loop_state.Break();
                    }
                }

                string y_key = y_keys[y];
                HashSet<string> y_values = map_y_axis.Get(y_key);

                for (int x = 0; x < x_keys.Count; ++x)
                {
                    string x_key = x_keys[x];
                    HashSet<string> x_values = map_x_axis.Get(x_key);

                    var common_fingerprint = y_values.Intersect(x_values);
                    if (common_fingerprint.Any())
                    {
                        common_fingerprints[y, x] = new List<string>(common_fingerprint);
                    }
                }
            });

            StatusManager.Instance.UpdateStatus("LibraryPivot", "Built library pivot");

            PivotResult pivot_result = new PivotResult();
            pivot_result.y_keys = y_keys;
            pivot_result.x_keys = x_keys;
            pivot_result.common_fingerprints = common_fingerprints;
            return pivot_result;
        }

        public static readonly string[] IDENTIFIER_CHOICES = new string[]
        {
            "Count"
            ,"Fingerprint"
            ,"BibTeXKey"
        };

        public static class IdentifierImplementations
        {
            public delegate void IdentifierImplementationDelegate(Library library, List<string> fingerprints, GridStyleInfo gsi);

            public static IdentifierImplementationDelegate GetIdentifierImplementation(string identifier_name)
            {
                switch (identifier_name)
                {
                    case "Count": return Count;
                    case "Fingerprint": return Fingerprint;
                    case "BibTeXKey": return BibTeXKey;

                    default:
                        Logging.Warn("Unknown pivot identifier {0}", identifier_name);
                        return Count;
                }
            }

            public static void Count(Library library, List<string> fingerprints, GridStyleInfo gsi)
            {
                if (null != fingerprints)
                {
                    gsi.CellValue = fingerprints.Count;
                    gsi.CellValueType = typeof(int);
                }
            }

            public static void Fingerprint(Library library, List<string> fingerprints, GridStyleInfo gsi)
            {
                if (null != fingerprints)
                {
                    gsi.CellValue = String.Join(";", fingerprints);
                    gsi.CellValueType = typeof(string);
                }
            }

            public static void BibTeXKey(Library library, List<string> fingerprints, GridStyleInfo gsi)
            {
                if (null != fingerprints)
                {
                    List<PDFDocument> pdf_documents = library.GetDocumentByFingerprints(fingerprints);
                    gsi.CellValue = String.Join(";", pdf_documents.Select(pdf_document => pdf_document.BibTex.Key ?? ("?" + pdf_document.Fingerprint)));
                    gsi.CellValueType = typeof(string);
                }
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            // TODO: REMOVE AFTER TESTING
            Library library = WebLibraryManager.Instance.Library_Guest;
            while (!library.LibraryIsLoaded) 
            {
                Thread.Sleep(100);
            }

            LibraryPivotReportControl lprc = new LibraryPivotReportControl();
            lprc.Library = library;

            StandardWindow sw = new StandardWindow();
            sw.Content = lprc;
            sw.Width = 800;
            sw.Height = 800;
            sw.Show();
        }
        
        public static void Test2()
        {
            Library library = WebLibraryManager.Instance.Library_Guest;
            while (!library.LibraryIsLoaded) 
            {
                Thread.Sleep(100);
            }

            List<PDFDocument> pdf_documents = library.PDFDocuments;

            HashSet<string> parent_fingerprints = null;
            MultiMapSet<string, string> map_ratings = RatingExplorerControl.GetNodeItems(library, parent_fingerprints);
            MultiMapSet<string, string> map_reading_stage = ReadingStageExplorerControl.GetNodeItems(library, parent_fingerprints);
            MultiMapSet<string, string> map_author = AuthorExplorerControl.GetNodeItems(library, parent_fingerprints);
            MultiMapSet<string, string> map_year = YearExplorerControl.GetNodeItems(library, parent_fingerprints);
            MultiMapSet<string, string> map_ai_tag = AITagExplorerControl.GetNodeItems(library, parent_fingerprints);
            MultiMapSet<string, string> map_tag = TagExplorerControl.GetNodeItems(library, parent_fingerprints);
            MultiMapSet<string, string> map_publication = PublicationExplorerControl.GetNodeItems(library, parent_fingerprints);

            MultiMapSet<string, string> map_y_axis = map_tag;
            MultiMapSet<string, string> map_x_axis = map_author;

            PivotResult pivot_result = GeneratePivot(map_y_axis, map_x_axis);
        }
#endif

        #endregion
    }
}
