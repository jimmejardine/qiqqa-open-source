using System;
using System.Collections.Generic;
using System.Threading;
using Utilities.Random;

namespace Utilities.Mathematics.Topics.LDAStuff
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

        public void MC(int num_iterations)
        {
            // Run gibbs sampling
            while (num_iterations > 0)
            {
                if (0 == lda_sampler.total_iterations % 10)
                {
                    Logging.Info("{0} iterations remain. {1} iterations have run in total", num_iterations, lda_sampler.total_iterations);
                }

                Thread[] threads = new Thread[NUM_THREADS];
                for (int thread = 0; thread < NUM_THREADS; ++thread)
                {
                    threads[thread] = new Thread(MC_THREAD);
                    threads[thread].Priority = ThreadPriority.BelowNormal;
                    threads[thread].Name = "LDA Worker " + thread;
                    threads[thread].Start(thread);
                }

                // Wait for the threads
                for (int thread = 0; thread < NUM_THREADS; ++thread)
                {
                    threads[thread].Join();
                }

                --num_iterations;
                ++lda_sampler.total_iterations;
            }
        }

        private void MC_THREAD(object thread_object)
        {
            int thread = (int)thread_object;

            int total_topic_shifts = 0;

            for (int i = 0; i < total_words_in_corpus; i += NUM_THREADS)
            {
                {
                    // Get the next doc to process
                    int doc = random_mc_orderings[random_mt[thread].NextIntExclusive(random_mc_orderings.Count)];
                    int doc_word_index = random_mt[thread].NextIntExclusive(lda_sampler.WORDS_IN_DOCS[doc].Length);

                    // Get the associated word/topic with this position in the doc
                    int word = lda_sampler.WORDS_IN_DOCS[doc][doc_word_index];

                    // What is the most likely topic for this document/word?                        
                    int new_topic = SampleTopicForWordInDoc(doc, word, thread);

                    // Has the word moved topic?
                    int old_topic = lda_sampler.topic_of_word_in_doc[doc][doc_word_index];
                    if (new_topic != old_topic)
                    {
                        Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                        lock (lda_sampler)
                        {
                            l1_clk.LockPerfTimerStop();
                            // Resample the old topic, because it might have been moved already...
                            old_topic = lda_sampler.topic_of_word_in_doc[doc][doc_word_index];
                            if (new_topic != old_topic)
                            {
                                ++total_topic_shifts;

                                if (old_topic == lda_sampler.topic_of_word_in_doc[doc][doc_word_index])
                                {
                                    // Remove the word from its existing topic
                                    --lda_sampler.number_of_times_doc_has_a_specific_topic[doc, old_topic];
                                    --lda_sampler.number_of_times_a_topic_has_any_word[old_topic];
                                    --lda_sampler.number_of_times_a_topic_has_a_specific_word[old_topic, word];

                                    // Give the word a new topic
                                    ++lda_sampler.number_of_times_doc_has_a_specific_topic[doc, new_topic];
                                    ++lda_sampler.number_of_times_a_topic_has_any_word[new_topic];
                                    ++lda_sampler.number_of_times_a_topic_has_a_specific_word[new_topic, word];
                                }
                                else
                                {
                                    Logging.Info("It must have been moved on already!");
                                }

                                lda_sampler.topic_of_word_in_doc[doc][doc_word_index] = new_topic;
                            }
                        }
                    }
                }
            }

            //Logging.Info("{0} words changed topic", total_topic_shifts);
        }

        private int SampleTopicForWordInDoc(int doc, int word, int thread)
        {
            //lock (lda_sampler)
            {
                for (int topic = 0; topic < lda_sampler.NUM_TOPICS; ++topic)
                {
                    probability_working_buffer[thread][topic] =
                        lda_sampler.number_of_times_doc_has_a_specific_topic[doc, topic] *
                        lda_sampler.number_of_times_a_topic_has_a_specific_word[topic, word] /
                        lda_sampler.number_of_times_a_topic_has_any_word[topic]
                        ;

                    /*
                     * It used to look like this, but if we initialise the arrays correctly, then we can take out ALPHA and BETA.
                    probability_working_buffer[topic] =
                        (lda_sampler.number_of_times_doc_has_a_specific_topic[doc, topic] + lda_sampler.ALPHA) *
                        (lda_sampler.number_of_times_a_topic_has_a_specific_word[topic, word] + lda_sampler.BETA) /
                        (lda_sampler.number_of_times_a_topic_has_any_word[topic] + lda_sampler.NUM_WORDS * lda_sampler.BETA)
                        ;
                     */

                    /*
                     * It used to look like this, but we can take out the ALPHA denominator because it doesn't change, and so is a universal scaling constant that comes out in the Giibs selection anyway
                    probability_working_buffer[topic] =
                        (number_of_times_a_topic_has_a_specific_word[topic, word] + BETA) /
                        (number_of_times_a_topic_has_any_word[topic] + NUM_WORDS * BETA)
                        *
                        (number_of_times_doc_has_a_specific_topic[doc, topic] + ALPHA) /
                        (number_of_times_a_doc_has_any_topic[doc] + NUM_TOPICS * ALPHA);
                    */
                }
            }

            // Now pick the topic number according to their relative probabilities
            for (int topic = 1; topic < lda_sampler.NUM_TOPICS; ++topic)
            {
                probability_working_buffer[thread][topic] += probability_working_buffer[thread][topic - 1];
            }
            double u = random_mt[thread].NextDouble() * probability_working_buffer[thread][lda_sampler.NUM_TOPICS - 1];
            for (int topic = 0; topic < lda_sampler.NUM_TOPICS; ++topic)
            {
                if (probability_working_buffer[thread][topic] > u)
                {
                    return topic;
                }
            }
            throw new Exception("A topic should have been picked before now");
        }
    }
}
