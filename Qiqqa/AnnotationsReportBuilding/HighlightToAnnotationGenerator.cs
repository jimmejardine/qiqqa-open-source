using System.Collections.Generic;
using Qiqqa.Documents.PDF;

namespace Qiqqa.AnnotationsReportBuilding
{
    class HighlightToAnnotationGenerator
    {
        public const string HIGHLIGHTS_TAG = "*Highlights*";

        // Warning CA1812	'HighlightToAnnotationGenerator' is an internal class that is apparently never instantiated.
        // If this class is intended to contain only static methods, consider adding a private constructor to prevent 
        // the compiler from generating a default constructor.
        private HighlightToAnnotationGenerator()
        { }

        internal static List<PDFAnnotation> GenerateAnnotations(PDFDocument pdf_document, Dictionary<string, byte[]> library_items_highlights_cache)
        {
            List<PDFAnnotation> annotations = new List<PDFAnnotation>();

            if (pdf_document.DocumentExists)
            {
                PDFHightlightList highlight_list = pdf_document.GetHighlights(library_items_highlights_cache);
                foreach (int page in highlight_list.GetAffectedPages())
                {
                    annotations.AddRange(GenerateAnnotations(pdf_document, page, highlight_list));
                }
            }

            return annotations;
        }

        internal static List<PDFAnnotation> GenerateAnnotations(PDFDocument pdf_document, int page, PDFHightlightList highlight_list)
        {
            List<RegionOfInterest> regions = new List<RegionOfInterest>();

            // Collect all the highlights on this page
            foreach (var highlight in highlight_list.GetHighlightsForPage(page))
            {
                if (page == highlight.Page && 0 != highlight.Width)
                {
                    regions.Add(new RegionOfInterest(highlight.Left, highlight.Top, highlight.Width, highlight.Height));
                }
            }

            RegionOfInterest.AggregateRegions(regions);
           
            // Build a list of annotations
            List<PDFAnnotation> annotations = RegionOfInterest.ConvertRegionsToPDFAnnotations(regions, HIGHLIGHTS_TAG, pdf_document, page);
            return annotations;
        }
    }
}
