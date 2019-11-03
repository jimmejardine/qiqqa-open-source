using System;
using System.IO;
using Utilities.Random;

namespace Utilities.Mathematics.Topics.LDAStuff
{
    [Serializable]
    public class LDASampler
    {
        #region --- LDA parameters ----------------------------------------------------------------------------------------------------------

        internal readonly int NUM_TOPICS;
        internal readonly int NUM_WORDS;
        internal readonly int NUM_DOCS;

        internal readonly double ALPHA;
        internal readonly double BETA;

        internal readonly int[][] WORDS_IN_DOCS;    // [doc][word]

        #endregion ---------------------------------------------------------------------------------------------------------------------------

        #region --- The distributions calculated so far ---------------------------------------------------------------------------------------

        internal int[][] topic_of_word_in_doc; // [doc][i]
        internal float[,] number_of_times_doc_has_a_specific_topic; // [doc,topic]
        internal float[] number_of_times_a_doc_has_any_topic; // [doc]
        internal float[,] number_of_times_a_topic_has_a_specific_word; // [topic,word]
        internal float[] number_of_times_a_topic_has_any_word; // [topic]

        internal int total_iterations;
        private double[] probability_working_buffer; // [topic]

        #endregion -------------------------------------------------------------------------------------------------------------------------------------

        public int NumTopics => NUM_TOPICS;

        public int NumDocs => NUM_DOCS;

        public LDASampler(double alpha, double beta, int num_topics, int num_words, int num_docs, int[][] WORDS_IN_DOCS /* jagged array [doc][word] */) :
            this(alpha, beta, num_topics, num_words, num_docs, WORDS_IN_DOCS, true)
        {
        }

        public LDASampler(double alpha, double beta, int num_topics, int num_words, int num_docs, int[][] WORDS_IN_DOCS /* jagged array [doc][word] */, bool initialise_initial_distribution)
        {
            ALPHA = alpha;
            BETA = beta;

            NUM_TOPICS = num_topics;
            NUM_WORDS = num_words;
            NUM_DOCS = num_docs;

            this.WORDS_IN_DOCS = WORDS_IN_DOCS;

            AllocateMemoryStructures();

            if (initialise_initial_distribution)
            {
                GenerateRandomInitialDistributions();
            }
        }

        private void AllocateMemoryStructures()
        {
            // Make space for the distributions
            number_of_times_doc_has_a_specific_topic = new float[NUM_DOCS, NUM_TOPICS];
            number_of_times_a_doc_has_any_topic = new float[NUM_DOCS];
            number_of_times_a_topic_has_a_specific_word = new float[NUM_TOPICS, NUM_WORDS];
            number_of_times_a_topic_has_any_word = new float[NUM_TOPICS];

            topic_of_word_in_doc = new int[NUM_DOCS][];
            for (int doc = 0; doc < NUM_DOCS; ++doc)
            {
                int doc_length = WORDS_IN_DOCS[doc].Length;
                topic_of_word_in_doc[doc] = new int[doc_length];
            }

            probability_working_buffer = new double[NUM_TOPICS];
        }

        private void GenerateRandomInitialDistributions()
        {
            Logging.Info("+Assigning every word in every document to a random topic");

            // Prime each distribution with the correct "smoothing" factor
            for (int doc = 0; doc < NUM_DOCS; ++doc)
            {
                number_of_times_a_doc_has_any_topic[doc] = (float)(NUM_TOPICS * ALPHA);
                for (int topic = 0; topic < NUM_TOPICS; ++topic)
                {
                    number_of_times_doc_has_a_specific_topic[doc, topic] = (float)(ALPHA);
                }
            }
            for (int topic = 0; topic < NUM_TOPICS; ++topic)
            {
                number_of_times_a_topic_has_any_word[topic] = (float)(NUM_WORDS * BETA);
                for (int word = 0; word < NUM_WORDS; ++word)
                {
                    number_of_times_a_topic_has_a_specific_word[topic, word] = (float)(BETA);
                }
            }

            // Assign every word in each document to a random topic (same word can be in a different topic in different documents)                        
            for (int doc = 0; doc < NUM_DOCS; ++doc)
            {
                int doc_length = WORDS_IN_DOCS[doc].Length;
                for (int doc_word_index = 0; doc_word_index < doc_length; ++doc_word_index)
                {
                    int word = WORDS_IN_DOCS[doc][doc_word_index];
                    int topic = (int)RandomAugmented.Instance.NextIntExclusive(NUM_TOPICS);

                    ++number_of_times_doc_has_a_specific_topic[doc, topic];
                    ++number_of_times_a_doc_has_any_topic[doc];
                    ++number_of_times_a_topic_has_a_specific_word[topic, word];
                    ++number_of_times_a_topic_has_any_word[topic];

                    topic_of_word_in_doc[doc][doc_word_index] = topic;
                }
            }

            total_iterations = 0;

            Logging.Info("-Assigning every word in every document to a random topic");
        }

        public void FastSave(string lda_sampler_filename)
        {
            using (FileStream fs = new FileStream(lda_sampler_filename, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(NUM_TOPICS);
                    bw.Write(NUM_WORDS);
                    bw.Write(NUM_DOCS);
                    bw.Write(ALPHA);
                    bw.Write(BETA);
                    bw.Write(total_iterations);

                    //internal int[][] topic_of_word_in_doc; // [doc][i]
                    for (int doc = 0; doc < NUM_DOCS; ++doc)
                        for (int i = 0; i < topic_of_word_in_doc[doc].Length; ++i)
                            bw.Write(topic_of_word_in_doc[doc][i]);

                    //internal float[,] number_of_times_doc_has_a_specific_topic; // [doc,topic]
                    for (int doc = 0; doc < NUM_DOCS; ++doc)
                        for (int topic = 0; topic < NUM_TOPICS; ++topic)
                            bw.Write(number_of_times_doc_has_a_specific_topic[doc, topic]);

                    //internal float[] number_of_times_a_doc_has_any_topic; // [doc]
                    for (int doc = 0; doc < NUM_DOCS; ++doc)
                        bw.Write(number_of_times_a_doc_has_any_topic[doc]);

                    //internal float[,] number_of_times_a_topic_has_a_specific_word; // [topic,word]
                    for (int topic = 0; topic < NUM_TOPICS; ++topic)
                        for (int word = 0; word < NUM_WORDS; ++word)
                            bw.Write(number_of_times_a_topic_has_a_specific_word[topic, word]);

                    //internal float[] number_of_times_a_topic_has_any_word; // [topic]
                    for (int topic = 0; topic < NUM_TOPICS; ++topic)
                        bw.Write(number_of_times_a_topic_has_any_word[topic]);
                }
            }
        }

        public static LDASampler FastLoad(string lda_sampler_filename, int[][] WORDS_IN_DOCS)
        {
            using (FileStream fs = new FileStream(lda_sampler_filename, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int NUM_TOPICS = br.ReadInt32();
                    int NUM_WORDS = br.ReadInt32();
                    int NUM_DOCS = br.ReadInt32();
                    double ALPHA = br.ReadDouble();
                    double BETA = br.ReadDouble();

                    LDASampler lda_sampler = new LDASampler(ALPHA, BETA, NUM_TOPICS, NUM_WORDS, NUM_DOCS, WORDS_IN_DOCS, false);

                    lda_sampler.total_iterations = br.ReadInt32();

                    //internal int[][] topic_of_word_in_doc; // [doc][i]
                    for (int doc = 0; doc < NUM_DOCS; ++doc)
                        for (int i = 0; i < lda_sampler.topic_of_word_in_doc[doc].Length; ++i)
                            lda_sampler.topic_of_word_in_doc[doc][i] = br.ReadInt32();

                    //internal float[,] number_of_times_doc_has_a_specific_topic; // [doc,topic]
                    for (int doc = 0; doc < NUM_DOCS; ++doc)
                        for (int topic = 0; topic < NUM_TOPICS; ++topic)
                            lda_sampler.number_of_times_doc_has_a_specific_topic[doc, topic] = br.ReadSingle();

                    //internal float[] number_of_times_a_doc_has_any_topic; // [doc]
                    for (int doc = 0; doc < NUM_DOCS; ++doc)
                        lda_sampler.number_of_times_a_doc_has_any_topic[doc] = br.ReadSingle();

                    //internal float[,] number_of_times_a_topic_has_a_specific_word; // [topic,word]
                    for (int topic = 0; topic < NUM_TOPICS; ++topic)
                        for (int word = 0; word < NUM_WORDS; ++word)
                            lda_sampler.number_of_times_a_topic_has_a_specific_word[topic, word] = br.ReadSingle();

                    //internal float[] number_of_times_a_topic_has_any_word; // [topic]
                    for (int topic = 0; topic < NUM_TOPICS; ++topic)
                        lda_sampler.number_of_times_a_topic_has_any_word[topic] = br.ReadSingle();

                    return lda_sampler;
                }
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestFastSaveLoad()
        {
            // Random data
            double alpha = 0.1;
            double beta = 0.2;
            int num_topics = 200;
            int num_words = 10000;
            int num_docs = 20000;
            int[][] WORDS_IN_DOCS = new int[num_docs][];
            for (int i = 0; i < num_docs; ++i) WORDS_IN_DOCS[i] = new int[i % 100 + 1];

            LDASampler lda_sampler = new LDASampler(alpha, beta, num_topics, num_words, num_docs, WORDS_IN_DOCS);

            {
                string lda_sampler_filename = @"C:\temp\ldatest_old.dat";
                Logging.Info("+OldSave");
                SerializeFile.Save(lda_sampler_filename, lda_sampler);
                Logging.Info("-OldSave");
                Logging.Info("+OldLoad");
                lda_sampler = (LDASampler)SerializeFile.Load(lda_sampler_filename);
                Logging.Info("-OldLoad");
            }
            {
                string lda_sampler_filename = @"C:\temp\ldatest_new.dat";
                Logging.Info("+NewSave");
                lda_sampler.FastSave(lda_sampler_filename);
                Logging.Info("-NewSave");
                Logging.Info("+NewLoad");
                lda_sampler = FastLoad(lda_sampler_filename, WORDS_IN_DOCS);
                Logging.Info("-NewLoad");
            }
        }
#endif

        #endregion
    }
}
