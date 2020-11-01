using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.AnnotationsReportBuilding
{
    internal class JSONAnnotationReportBuilder
    {
        private class AnnotationJSON
        {
            public string fingerprint { get; set; }
            public string title { get; set; }
            public double page { get; set; }
            public double left { get; set; }
            public double top { get; set; }
            public double width { get; set; }
            public double height { get; set; }
            public string tags { get; set; }
            public string text { get; set; }
        }

#if false
        internal static void BuildReport(WebLibraryDetail web_library_detail, List<PDFDocument> pdf_documents)
        {
            AnnotationReportOptions annotation_report_options = new AnnotationReportOptions();

            List<AnnotationWorkGenerator.AnnotationWork> annotation_works = AnnotationWorkGenerator.GenerateAnnotationWorks(library, pdf_documents, annotation_report_options);

            IEnumerable<AnnotationJSON> annotation_jsons = annotation_works.Select(annotation_work =>
                new AnnotationJSON
                {
                    fingerprint = annotation_work.pdf_document.Fingerprint,
                    title = annotation_work.pdf_document.TitleCombined,
                    page = annotation_work.pdf_annotation.Page,
                    left = annotation_work.pdf_annotation.Left,
                    top = annotation_work.pdf_annotation.Top,
                    width = annotation_work.pdf_annotation.Width,
                    height = annotation_work.pdf_annotation.Height,
                    tags = annotation_work.pdf_annotation.Tags,
                    text = annotation_work.pdf_annotation.Text,
                }
            );

            string json = JsonConvert.SerializeObject(annotation_jsons, Formatting.Indented);
            string filename = Path.GetTempFileName() + ".json.txt";
            File.WriteAllText(filename, json);
            Process.Start(filename);
        }
#endif
    }
}
