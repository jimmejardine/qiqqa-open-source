using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Utilities;
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
        public delegate bool ExpeditionBuilderProgressUpdateDelegate(string message, double percentage_complete);

        private static bool DefaultExpeditionBuilderProgressUpdate(string message, double percentage_complete)
        {
            Logging.Info("ExpeditionBuilder progress {0}:, {1}", percentage_complete, message);
            return true;
        }

        public static ExpeditionDataSource BuildExpeditionDataSource(Library library, int num_topics, bool add_autotags, bool add_tags, ExpeditionBuilderProgressUpdateDelegate progress_update_delegate)
        {
            bool not_aborted_by_user = true;

            // Check that we have a progres update delegate
            if (null == progress_update_delegate)
            {
                progress_update_delegate = DefaultExpeditionBuilderProgressUpdate;
            }

            // What are the sources of data?
            progress_update_delegate("Assembling tags", 0);
            HashSet<string> tags = BuildLibraryTagList(library, add_autotags, add_tags);
            List<PDFDocument> pdf_documents = library.PDFDocumentsWithLocalFilePresent;

            // Initialise the datasource
            progress_update_delegate("Initialising datasource", 0);
            ExpeditionDataSource data_source = new ExpeditionDataSource();

            data_source.date_created = DateTime.UtcNow;

            progress_update_delegate("Adding tags", 0);
            data_source.words = new List<string>();            
            foreach (string tag in tags)
            {
                data_source.words.Add(tag);
            }

            progress_update_delegate("Adding docs", 0);
            data_source.docs = new List<string>();            
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                data_source.docs.Add(pdf_document.Fingerprint);
            }

            progress_update_delegate("Rebuilding indices", 0);
            data_source.RebuildIndices();

            // Now go through each doc and find the tags that match
            data_source.words_in_docs = new int[data_source.docs.Count][];

            int total_processed = 0;

            Parallel.For(0, data_source.docs.Count, d =>
            //for (int d = 0; d < data_source.docs.Count; ++d)
            {                
                int total_processed_local = Interlocked.Increment(ref total_processed);
                if (0 == total_processed_local % 10)
                {
                    not_aborted_by_user = not_aborted_by_user && progress_update_delegate("Scanning documents", total_processed_local / (double)data_source.docs.Count);
                }

                List<int> tags_in_document = new List<int>();

                if (not_aborted_by_user)
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
            not_aborted_by_user = not_aborted_by_user && progress_update_delegate("Building themes sampler", 0);
            int num_threads = Environment.ProcessorCount;            
            double alpha = 2.0 / num_topics;
            double beta = 0.01;
            data_source.lda_sampler = new LDASampler(alpha, beta, num_topics, data_source.words.Count, data_source.docs.Count, data_source.words_in_docs);
            
            LDASamplerMCSerial lda_sampler_mc = new LDASamplerMCSerial(data_source.lda_sampler, num_threads);
            for (int i = 0; i < MAX_TOPIC_ITERATIONS; ++i)
            {
                if (!not_aborted_by_user) break;
                not_aborted_by_user = not_aborted_by_user && progress_update_delegate("Building themes", i / (double)MAX_TOPIC_ITERATIONS);
                lda_sampler_mc.MC(10);
            }

            // Results
            if (not_aborted_by_user)
            {
                progress_update_delegate("Built Expedition", 1);
            }
            else
            {
                progress_update_delegate("Cancelled Expedition", 1);
            }

            return data_source;
        }

        private static HashSet<string> BuildLibraryTagList(Library library, bool add_autotags, bool add_tags)
        {
            HashSet<string> tags = new HashSet<string>();

            if (add_autotags)
            {
                // Check that the AutoTags are not getting too old
                if (null == library.AITagManager.AITags || library.AITagManager.AITags.IsGettingOld || library.AITagManager.AITags.HaveNoTags)
                {
                    library.AITagManager.Regenerate();
                }

                // Add in the auto tags
                if (null != library.AITagManager.AITags)
                {
                    tags.UnionWith(library.AITagManager.AITags.GetAllTags());
                }
            }

            // Add in the manually generated tags
            if (add_tags)
            {
                foreach (PDFDocument pdf_document in library.PDFDocuments)
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
