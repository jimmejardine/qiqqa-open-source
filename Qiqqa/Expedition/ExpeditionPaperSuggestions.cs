using System;
using System.Collections.Generic;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Mathematics.Topics.LDAStuff;

namespace Qiqqa.Expedition
{
    class ExpeditionPaperSuggestions
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
                if (null == pdf_document.Library.ExpeditionManager.ExpeditionDataSource)
                {
                    return results;
                }

                ExpeditionDataSource eds = pdf_document.Library.ExpeditionManager.ExpeditionDataSource;
                LDAAnalysis lda_analysis = eds.LDAAnalysis;

                if (!pdf_document.Library.ExpeditionManager.ExpeditionDataSource.docs_index.ContainsKey(pdf_document.Fingerprint))
                {
                    return results;
                }

                // Fill the similar papers
                {
                    int doc_id = pdf_document.Library.ExpeditionManager.ExpeditionDataSource.docs_index[pdf_document.Fingerprint];
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
                        PDFDocument pdf_document_similar = pdf_document.Library.GetDocumentByFingerprint(eds.docs[similar_docs[i].doc]);
                        results.Add(new Result { pdf_document = pdf_document_similar, relevance = similar_docs[i].prob });
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
