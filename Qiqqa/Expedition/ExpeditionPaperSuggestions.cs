using System;
using System.Collections.Generic;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Mathematics.Topics.LDAStuff;

namespace Qiqqa.Expedition
{
    internal class ExpeditionPaperSuggestions
    {
        public class Result
        {
            public PDFDocument pdf_document;
            public double relevance;
        }

        public static List<Result> GetRelevantOthers(PDFDocument pdf_document, int NUM_OTHERS)
        {
            List<Result> results = new List<Result>();

            try
            {
                ExpeditionDataSource eds = pdf_document.LibraryRef?.Xlibrary?.ExpeditionManager?.ExpeditionDataSource;

                if (null != eds)
                {
                    LDAAnalysis lda_analysis = eds.LDAAnalysis;

                    if (eds.docs_index.ContainsKey(pdf_document.Fingerprint))
                    {
                        // Fill the similar papers

                        int doc_id = eds.docs_index[pdf_document.Fingerprint];
                        TopicProbability[] topics = lda_analysis.DensityOfTopicsInDocsSorted[doc_id];

                        List<DocProbability> similar_docs = new List<DocProbability>();

                        // Only look at the first 5 topics
                        for (int t = 0; t < topics.Length && t < 3; ++t)
                        {
                            int topic = topics[t].topic;
                            double topic_prob = topics[t].prob;

                            // Look at the first 50 docs in each topic (if there are that many)
                            DocProbability[] docs = lda_analysis.DensityOfDocsInTopicsSorted[topic];
                            for (int d = 0; d < docs.Length && d < 50; ++d)
                            {
                                int doc = docs[d].doc;
                                double doc_prob = docs[d].prob;

                                DocProbability dp = new DocProbability(Math.Sqrt(topic_prob * doc_prob), doc);
                                similar_docs.Add(dp);
                            }
                        }

                        // Now take the top N docs
                        similar_docs.Sort();
                        for (int i = 0; i < similar_docs.Count && i < NUM_OTHERS; ++i)
                        {
                            string fingerprint_to_look_for = eds.docs[similar_docs[i].doc];
                            PDFDocument pdf_document_similar = pdf_document.LibraryRef.Xlibrary.GetDocumentByFingerprint(fingerprint_to_look_for);
                            if (null == pdf_document_similar)
                            {
                                Logging.Warn("ExpeditionPaperSuggestions: Cannot find similar document anymore for fingerprint {0}", fingerprint_to_look_for);
                            }
                            else
                            {
                                results.Add(new Result { pdf_document = pdf_document_similar, relevance = similar_docs[i].prob });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem getting the relevant others for document {0}", pdf_document.Fingerprint);
            }

            return results;
        }
    }
}
