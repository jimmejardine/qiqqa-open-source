using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;
using Utilities.Mathematics;
using Utilities.Mathematics.Topics.LDAStuff;
using Utilities.Strings;

namespace Qiqqa.Expedition
{
    public static class ExpeditionBuilder
    {
        private const int MAX_TOPIC_ITERATIONS = 30;

        /// <summary>
        /// Return TRUE to keep going.  FALSE to abort...
        /// </summary>
        /// <param name="message"></param>
        /// <param name="percentage_complete"></param>
        /// <returns></returns>
        public delegate bool ExpeditionBuilderProgressUpdateDelegate(string message, long current_update_number = 0, long total_update_count = 0);

        private static bool DefaultExpeditionBuilderProgressUpdate(string message, long current_update_number, long total_update_count)
        {
            Logging.Info("ExpeditionBuilder progress {0}: {1}/{2} = {3:P}", message, current_update_number, total_update_count, Perunage.Calc(current_update_number, total_update_count));
            return true;
        }

        public static ExpeditionDataSource BuildExpeditionDataSource(WebLibraryDetail web_library_detail, int num_topics, bool add_autotags, bool add_tags, ExpeditionBuilderProgressUpdateDelegate progress_update_delegate)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            // Initialise the datasource
            ExpeditionDataSource data_source = new ExpeditionDataSource();

            data_source.date_created = DateTime.UtcNow;

            try
            {
                // Check that we have a progress update delegate
                if (null == progress_update_delegate)
                {
                    progress_update_delegate = DefaultExpeditionBuilderProgressUpdate;
                }

                // What are the sources of data?
                progress_update_delegate("Assembling tags");
                HashSet<string> tags = BuildLibraryTagList(web_library_detail, add_autotags, add_tags);
                List<PDFDocument> pdf_documents = web_library_detail.Xlibrary.PDFDocumentsWithLocalFilePresent;

                progress_update_delegate("Adding tags");
                data_source.words = new List<string>();
                foreach (string tag in tags)
                {
                    data_source.words.Add(tag);
                }

                progress_update_delegate("Adding docs");
                data_source.docs = new List<string>();
                foreach (PDFDocument pdf_document in pdf_documents)
                {
                    data_source.docs.Add(pdf_document.Fingerprint);
                }

                progress_update_delegate("Rebuilding indices");
                data_source.RebuildIndices();

                // Now go through each doc and find the tags that match
                int DATA_SOURCE_DOCS_COUNT = data_source.docs.Count;
                data_source.words_in_docs = new int[DATA_SOURCE_DOCS_COUNT][];

                //int total_processed = 0;

                Parallel.For(0, DATA_SOURCE_DOCS_COUNT, d =>
                //for (int d = 0; d < DATA_SOURCE_DOCS_COUNT; ++d)
                {
                    //int total_processed_local = Interlocked.Increment(ref total_processed);
                    //if (0 == total_processed_local % 50)
                    if (0 == d % 50)
                    {
                        if (!progress_update_delegate("Scanning documents", d, DATA_SOURCE_DOCS_COUNT))
                        {
                            // Parallel.For() doc at https://docs.microsoft.com/en-us/archive/msdn-magazine/2007/october/parallel-performance-optimize-managed-code-for-multi-core-machines
                            // says:
                            //
                            // Finally, if any exception is thrown in any of the iterations, all iterations are canceled
                            // and the first thrown exception is rethrown in the calling thread, ensuring that exceptions
                            // are properly propagated and never lost.
                            //
                            // --> We can thus easily use an exception to terminate/cancel all iterations of Parallel.For()!
                            throw new TaskCanceledException("Operation canceled by user");
                        }
                    }

                    List<int> tags_in_document = new List<int>();

                    {
                        PDFDocument pdf_document = pdf_documents[d];
                        string full_text = " " + pdf_document.PDFRenderer.GetFullOCRText() + " ";
                        string full_text_lower = full_text.ToLower();

                        for (int t = 0; t < data_source.words.Count; ++t)
                        {
                            string tag = ' ' + data_source.words[t] + ' ';

                            string full_text_to_search = full_text;
                            if (StringTools.HasSomeLowerCase(tag))
                            {
                                full_text_to_search = full_text_lower;
                                tag = tag.ToLower();
                            }

                            int num_appearances = StringTools.CountStringOccurence(full_text_to_search, tag);
                            for (int i = 0; i < num_appearances; ++i)
                            {
                                tags_in_document.Add(t);
                            }
                        }
                    }

                    data_source.words_in_docs[d] = tags_in_document.ToArray();
                }
                );

                // Initialise the LDA
                if (!progress_update_delegate("Building themes sampler"))
                {
                    // Parallel.For() doc at https://docs.microsoft.com/en-us/archive/msdn-magazine/2007/october/parallel-performance-optimize-managed-code-for-multi-core-machines
                    // says:
                    //
                    // Finally, if any exception is thrown in any of the iterations, all iterations are canceled
                    // and the first thrown exception is rethrown in the calling thread, ensuring that exceptions
                    // are properly propagated and never lost.
                    //
                    // --> We can thus easily use an exception to terminate/cancel all iterations of Parallel.For()!
                    throw new TaskCanceledException("Operation canceled by user");
                }

                int num_threads = Math.Min(1, (Environment.ProcessorCount - 1) / 2);
                double alpha = 2.0 / num_topics;
                double beta = 0.01;
                data_source.lda_sampler = new LDASampler(alpha, beta, num_topics, data_source.words.Count, data_source.docs.Count, data_source.words_in_docs);

                LDASamplerMCSerial lda_sampler_mc = new LDASamplerMCSerial(data_source.lda_sampler, num_threads);
                lda_sampler_mc.MC(MAX_TOPIC_ITERATIONS, (iteration, num_iterations) =>
                {
                    if (!progress_update_delegate("Building themes", iteration, num_iterations))
                    {
                        // Parallel.For() doc at https://docs.microsoft.com/en-us/archive/msdn-magazine/2007/october/parallel-performance-optimize-managed-code-for-multi-core-machines
                        // says:
                        //
                        // Finally, if any exception is thrown in any of the iterations, all iterations are canceled
                        // and the first thrown exception is rethrown in the calling thread, ensuring that exceptions
                        // are properly propagated and never lost.
                        //
                        // --> We can thus easily use an exception to terminate/cancel all iterations of Parallel.For()!
                        throw new TaskCanceledException("Operation canceled by user");
                    }
                });
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (TaskCanceledException ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                // This exception should only occur when the user *canceled* the process and should therefor
                // *not* be propagated. Instead, we have to report an aborted result:
                progress_update_delegate("Canceled Expedition", 1, 1);
                return null;
            }

            progress_update_delegate("Built Expedition", 1, 1);

            return data_source;
        }

        private static HashSet<string> BuildLibraryTagList(WebLibraryDetail web_library_detail, bool add_autotags, bool add_tags)
        {
            HashSet<string> tags = new HashSet<string>();

            if (add_autotags)
            {
                // Check that the AutoTags are not getting too old
                if (null == web_library_detail.Xlibrary.AITagManager.AITags || web_library_detail.Xlibrary.AITagManager.AITags.IsGettingOld || web_library_detail.Xlibrary.AITagManager.AITags.HaveNoTags)
                {
                    web_library_detail.Xlibrary.AITagManager.Regenerate();
                }

                // Add in the auto tags
                if (null != web_library_detail.Xlibrary.AITagManager.AITags)
                {
                    tags.UnionWith(web_library_detail.Xlibrary.AITagManager.AITags.GetAllTags());
                }
            }

            // Add in the manually generated tags
            if (add_tags)
            {
                foreach (PDFDocument pdf_document in web_library_detail.Xlibrary.PDFDocuments)
                {
                    HashSet<string> document_tags = TagTools.ConvertTagBundleToTags(pdf_document.Tags);
                    tags.UnionWith(document_tags);
                }
            }

            return tags;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            Library library = Library.GuestInstance;
            Thread.Sleep(1000);

            int num_topics = (int)Math.Ceiling(Math.Sqrt(library.PDFDocuments.Count));
            ExpeditionDataSource ebds = BuildExpeditionDataSource(library, num_topics, true, true, null);
        }
#endif

        #endregion
    }
}
