using Newtonsoft.Json;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Qiqqa.AnnotationsReportBuilding
{
    class LinkedDocsAnnotationReportBuilder
    {
        // Warning CA1812	'LinkedDocsAnnotationReportBuilder' is an internal class that is apparently never instantiated.
        // If this class is intended to contain only static methods, consider adding a private constructor to prevent 
        // the compiler from generating a default constructor.
        private LinkedDocsAnnotationReportBuilder()
        { }

        internal static void BuildReport(Library library, List<PDFDocument> pdf_documents)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_LinkedDocsAnnotationReport);

            StringBuilder sb_o2o = new StringBuilder();
            StringBuilder sb_o2m = new StringBuilder();

            foreach (var pdf_document in pdf_documents)
            {
                var citations = pdf_document.PDFDocumentCitationManager.GetLinkedDocuments();
                if (null == citations || 0 == citations.Count) continue;

                // o2m
                {
                    sb_o2m.AppendFormat("{0}", pdf_document.Fingerprint);
                    sb_o2m.AppendFormat(",");
                    foreach (var citation in citations)
                    {
                        sb_o2m.AppendFormat("{0}", citation.fingerprint_outbound);
                        sb_o2m.AppendFormat(",");
                    }
                    sb_o2m.AppendLine();
                }

                // o2o
                {
                    foreach (var citation in citations)
                    {
                        sb_o2o.AppendFormat("{0}", pdf_document.Fingerprint);
                        sb_o2o.AppendFormat(",");
                        sb_o2o.AppendFormat("{0}", citation.fingerprint_inbound);
                        sb_o2o.AppendLine();
                    }
                }
            }

            string filename_o2o = Path.GetTempFileName() + ".o2o.txt";
            string filename_o2m = Path.GetTempFileName() + ".o2m.txt";
            File.WriteAllText(filename_o2o, sb_o2o.ToString());
            File.WriteAllText(filename_o2m, sb_o2m.ToString());
            Process.Start(filename_o2o);
            Process.Start(filename_o2m);
        }
    }
}
