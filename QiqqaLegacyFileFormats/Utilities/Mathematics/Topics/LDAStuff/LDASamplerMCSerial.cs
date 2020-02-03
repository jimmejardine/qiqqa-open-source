using System;
using System.Collections.Generic;
using System.Threading;

namespace QiqqaLegacyFileFormats          // namespace Utilities.Mathematics.Topics.LDAStuff
{
    [Serializable]
    public class LDASamplerMCSerial
    {
        private LDASampler lda_sampler;
        private int NUM_THREADS;
        private int total_words_in_corpus;
        private List<int> random_mc_orderings;
        private RandomAugmented[] random_mt;
        private double[][] probability_working_buffer;

        public LDASamplerMCSerial(LDASampler lda_sampler, int NUM_THREADS)
        {
            this.lda_sampler = lda_sampler;
            this.NUM_THREADS = NUM_THREADS;

            // Work out the ratio of each doc for our sampling
            {
                int MAX_REPRESENTATION = 100;

                int max_doc_length = 0;
                for (int doc = 0; doc < lda_sampler.NUM_DOCS; ++doc)
                {
                    max_doc_length = Math.Max(max_doc_length, lda_sampler.WORDS_IN_DOCS[doc].Length);
                }

                total_words_in_corpus = 0;
                for (int doc = 0; doc < lda_sampler.NUM_DOCS; ++doc)
                {
                    total_words_in_corpus += lda_sampler.WORDS_IN_DOCS[doc].Length;
                }

                random_mc_orderings = new List<int>();
                for (int doc = 0; doc < lda_sampler.NUM_DOCS; ++doc)
                {
                    int doc_representation = (0 < max_doc_length) ? MAX_REPRESENTATION * lda_sampler.WORDS_IN_DOCS[doc].Length / max_doc_length : 1;
                    if (0 == doc_representation && lda_sampler.WORDS_IN_DOCS[doc].Length > 0)
                    {
                        //Logging.Info("We have had to bump up the representation for doc {0} because it is too small", doc);
                        doc_representation = 1;
                    }
                    for (int i = 0; i < doc_representation; ++i)
                    {
                        random_mc_orderings.Add(doc);
                    }
                }
            }

            random_mt = new RandomAugmented[NUM_THREADS];
            probability_working_buffer = new double[NUM_THREADS][];
            for (int thread = 0; thread < NUM_THREADS; ++thread)
            {
                random_mt[thread] = new RandomAugmented((DateTime.UtcNow.Millisecond * (1 + thread)));
                probability_working_buffer[thread] = new double[lda_sampler.NUM_TOPICS];
            }
        }
    }
}
