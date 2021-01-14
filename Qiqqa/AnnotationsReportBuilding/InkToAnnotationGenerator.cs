using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using Qiqqa.Documents.PDF;

namespace Qiqqa.AnnotationsReportBuilding
{
    internal class InkToAnnotationGenerator
    {
        public const string INKS_TAG = "*Inks*";

        // Warning CA1812	'InkToAnnotationGenerator' is an internal class that is apparently never instantiated.
        // If this class is intended to contain only static methods, consider adding a private constructor to prevent 
        // the compiler from generating a default constructor.
        private InkToAnnotationGenerator()
        { }

        internal static List<PDFAnnotation> GenerateAnnotations(PDFDocument pdf_document)
        {
            List<PDFAnnotation> annotations = new List<PDFAnnotation>();

            if (pdf_document.DocumentExists)
            {
                PDFInkList ink_list = pdf_document.GetInks();
                foreach (int page in ink_list.GetAffectedPages())
                {
                    annotations.AddRange(GenerateAnnotations(pdf_document, page, ink_list));
                }
            }

            return annotations;
        }

        internal static List<PDFAnnotation> GenerateAnnotations(PDFDocument pdf_document, int page, PDFInkList ink_list)
        {
            List<RegionOfInterest> regions = new List<RegionOfInterest>();

            // Collect all the highlights on this page
            StrokeCollection stroke_collection = ink_list.GetInkStrokeCollection(page);
            if (null != stroke_collection)
            {
                foreach (Stroke stroke in stroke_collection)
                {
                    double SCALE = 1000.0;
                    Rect bound = stroke.GetBounds();
                    bound.X /= SCALE;
                    bound.Y /= SCALE;
                    bound.Width /= SCALE;
                    bound.Height /= SCALE;

                    regions.Add(new RegionOfInterest(bound.Left, bound.Top, bound.Width, bound.Height));
                }
            }

            RegionOfInterest.AggregateRegions(regions);

            // Build a list of annotations
            List<PDFAnnotation> annotations = RegionOfInterest.ConvertRegionsToPDFAnnotations(regions, INKS_TAG, pdf_document, page);
            return annotations;
        }
    }
}
