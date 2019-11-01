using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Utilities.Misc;

namespace Qiqqa.AnnotationsReportBuilding
{
    internal class AnnotationWorkGenerator
    {
        public class AnnotationWork
        {
            public Library library;
            public PDFDocument pdf_document;
            public PDFAnnotation pdf_annotation;

            // These are the output holders
            public Image report_image = null;
            public Figure report_floater = null;
            public Run processing_error = null;
            public Paragraph annotation_paragraph = null;
        }

        public static List<AnnotationWork> GenerateAnnotationWorks(Library library, List<PDFDocument> pdf_documents, AnnotationReportOptionsWindow.AnnotationReportOptions annotation_report_options)
        {
            List<AnnotationWork> annotation_works = new List<AnnotationWork>();

            // The caches we will need...
            Dictionary<string, byte[]> library_items_highlights_cache = library.LibraryDB.GetLibraryItemsAsCache(PDFDocumentFileLocations.HIGHLIGHTS);
            Dictionary<string, byte[]> library_items_inks_cache = library.LibraryDB.GetLibraryItemsAsCache(PDFDocumentFileLocations.INKS);

            for (int j = 0; j < pdf_documents.Count; ++j)
            {
                if (j % 10 == 0)
                {
                    StatusManager.Instance.UpdateStatus("AnnotationReport", "Initialising annotation report", j, pdf_documents.Count);
                }

                PDFDocument pdf_document = pdf_documents[j];

                // Add the comments token if this document has some comments or abstract (and the user wants them)
                if (annotation_report_options.IncludeAllPapers)
                {
                    if (!String.IsNullOrEmpty(pdf_document.Comments) || !String.IsNullOrEmpty(pdf_document.Abstract))
                    {
                        annotation_works.Add(new AnnotationWork { library = library, pdf_document = pdf_document, pdf_annotation = null });
                    }
                }

                List<PDFAnnotation> pdf_annotations = new List<PDFAnnotation>();
                pdf_annotations.AddRange(pdf_document.GetAnnotations());
                if (library_items_highlights_cache.ContainsKey(pdf_document.Fingerprint))
                {
                    pdf_annotations.AddRange(HighlightToAnnotationGenerator.GenerateAnnotations(pdf_document, library_items_highlights_cache));
                }
                if (library_items_inks_cache.ContainsKey(pdf_document.Fingerprint))
                {
                    pdf_annotations.AddRange(InkToAnnotationGenerator.GenerateAnnotations(pdf_document, library_items_inks_cache));
                }

                pdf_annotations.Sort(AnnotationSorter);

                foreach (PDFAnnotation pdf_annotation in pdf_annotations)
                {
                    if (pdf_annotation.Deleted)
                    {
                        continue;
                    }

                    // Filter by annotations
                    if (null != annotation_report_options.filter_tags && annotation_report_options.filter_tags.Count > 0)
                    {
                        HashSet<string> annotation_tags = TagTools.ConvertTagBundleToTags(pdf_annotation.Tags);
                        annotation_tags.IntersectWith(annotation_report_options.filter_tags);
                        if (0 == annotation_tags.Count)
                        {
                            continue;
                        }
                    }

                    // Filter by date ranges and creators
                    {
                        if (annotation_report_options.FilterByCreationDate && pdf_annotation.DateCreated.HasValue)
                        {
                            DateTime date_to = annotation_report_options.FilterByCreationDate_To;
                            if (date_to != DateTime.MaxValue) date_to = date_to.AddDays(1);

                            if (pdf_annotation.DateCreated.Value < annotation_report_options.FilterByCreationDate_From) continue;
                            if (pdf_annotation.DateCreated.Value >= date_to) continue;
                        }

                        if (annotation_report_options.FilterByFollowUpDate)
                        {
                            DateTime date_to = annotation_report_options.FilterByFollowUpDate_To;
                            if (date_to != DateTime.MaxValue) date_to = date_to.AddDays(1);

                            if (!pdf_annotation.FollowUpDate.HasValue) continue;
                            if (pdf_annotation.FollowUpDate.Value < annotation_report_options.FilterByFollowUpDate_From) continue;
                            if (pdf_annotation.FollowUpDate.Value >= date_to) continue;
                        }

                        if (annotation_report_options.FilterByCreator)
                        {
                            if (pdf_annotation.Creator != annotation_report_options.FilterByCreator_Name) continue;
                        }
                    }

                    annotation_works.Add(new AnnotationWork { library = library, pdf_document = pdf_document, pdf_annotation = pdf_annotation });
                }
            }

            return annotation_works;
        }



        /// <summary>
        /// Sorts by page, then y-offset
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        private static int AnnotationSorter(PDFAnnotation a1, PDFAnnotation a2)
        {
            if (a1.Page < a2.Page) return -1;
            if (a1.Page > a2.Page) return +1;

            if (a1.Top < a2.Top) return -1;
            if (a1.Top > a2.Top) return +1;

            return 0;
        }
    }
}
