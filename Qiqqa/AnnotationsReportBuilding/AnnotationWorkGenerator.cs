using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.AnnotationsReportBuilding
{
    internal class AnnotationWorkGenerator
    {
        public class AnnotationWork
        {
            public WebLibraryDetail web_library_detail;
            public PDFDocument pdf_document;
            public PDFAnnotation pdf_annotation;

            // These are the output holders
            public Image report_image = null;
            public Figure report_floater = null;
            public Run processing_error = null;
            public Paragraph annotation_paragraph = null;
        }

        public static List<AnnotationWork> GenerateAnnotationWorks(WebLibraryDetail web_library_detail, List<PDFDocument> pdf_documents, AnnotationReportOptions annotation_report_options)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            Stopwatch clk = Stopwatch.StartNew();
            Logging.Info("+GenerateAnnotationWorks: collecting annotations for documents: {0}", Library.GetDocumentsAsFingerprints(pdf_documents));

            List <AnnotationWork> annotation_works = new List<AnnotationWork>();

            // The caches we will need...
            //
            // ## Performance Note:
            //
            // We need to keep in mind the fact that, for a long list of documents, each of their fingerprints will end up in the
            // SQL query to keep the number of sought results to a minimum. Now that would be very fine indeed if the set of documents 
            // would be *small* for then the bulk of this function has few documents to cycle through in the loop further below.
            // HOWEVER, imagine a library of 10K+ items and a user kicking this function into work with the entire set passed down through
            // `pdf_documents`: if we weren't applying a bit of extra intelligence here, we would end up with an 'optimized' SQL query
            // containing an 'x OR y OR ...' SELECT filter expression containing 10K+ comparisons: the cost of generating such a huge
            // query would be considerable, let alone the *parsing* of it by SQLite and processing in that DB engine.
            //
            // Hence we're applying a bit of heuristic here: when your *set* includes more than, say, 42 documents (which we all know 
            // is the answer to everything and thus also to 'reasonable measure for one page/screen full'  ;-P  ) we simply DO NOT PASS
            // the set on to the query generator-annex-processor call GetLibraryItemsAsCache() but let that one follow its *default* 
            // behaviour *instead*, which is to dump the entire table in our lap and good riddance.
            // for more than 42 documents in the set and medium to huge libraries, this is expected to be 'near enough to optimal'.  :-)
            // When you beg to differ, you may. Benchmarks, please! (I know you'll be able to improve on this, surely, but you get my drift:
            // is it worth it yet? Or are there other areas with bigger fish to fry?  ;-)
            //
            ASSERT.Test(pdf_documents != null);                                                         // check these unexpected input combo's
            ASSERT.Test(pdf_documents.Count > 0);                                                       // check these unexpected input combo's
            bool deliver_reduced_optimal_set = (annotation_report_options.IncludeAllPapers ? false /* we want them *all* anyway! */ : pdf_documents != null ? pdf_documents.Count <= 42 : false /* no fingerprints at all?! */ );
            List<string> reduced_optimal_set = (deliver_reduced_optimal_set ? Library.GetDocumentsAsFingerprints(pdf_documents) : null);
            Dictionary<string, byte[]> library_items_highlights_cache = web_library_detail.Xlibrary.LibraryDB.GetLibraryItemsAsCache(PDFDocumentFileLocations.HIGHLIGHTS, reduced_optimal_set);
            Dictionary<string, byte[]> library_items_inks_cache = web_library_detail.Xlibrary.LibraryDB.GetLibraryItemsAsCache(PDFDocumentFileLocations.INKS, reduced_optimal_set);

            for (int j = 0; j < pdf_documents.Count; ++j)
            {
                if (j % 10 == 0)
                {
                    StatusManager.Instance.UpdateStatus("AnnotationReport", "Initialising annotation report", j, pdf_documents.Count);
                }

                PDFDocument pdf_document = pdf_documents[j];

                // Add the comments token if this document has some comments or abstract (and the user wants them)
                if (annotation_report_options.IncludeAllPapers && (annotation_report_options.IncludeComments || annotation_report_options.IncludeAbstract))
                {
                    if (!String.IsNullOrEmpty(pdf_document.Comments) || !String.IsNullOrEmpty(pdf_document.Abstract))
                    {
                        annotation_works.Add(new AnnotationWork { web_library_detail = web_library_detail, pdf_document = pdf_document, pdf_annotation = null });
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
                    pdf_annotations.AddRange(InkToAnnotationGenerator.GenerateAnnotations(pdf_document));
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

                    annotation_works.Add(new AnnotationWork { web_library_detail = web_library_detail, pdf_document = pdf_document, pdf_annotation = pdf_annotation });
                }
            }

            Logging.Info("-GenerateAnnotationWorks: collecting annotations for documents: {1} (time spent: {0} ms)", clk.ElapsedMilliseconds, Library.GetDocumentsAsFingerprints(pdf_documents));

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
