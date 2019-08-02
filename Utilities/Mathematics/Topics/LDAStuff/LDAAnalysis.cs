using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Mathematics.Topics.LDAStuff
{
    public class WordProbability : IComparable
    {
        public double prob;
        public int word;

        public WordProbability(double prob, int word)
        {
            this.prob = prob;
            this.word = word;
        }

        public int CompareTo(object other_obj)
        {
            WordProbability other = other_obj as WordProbability;
            return -prob.CompareTo(other.prob);
        }
    }

    public class DocProbability : IComparable
    {
        public double prob;
        public int doc;

        public DocProbability(double prob, int doc)
        {
            this.prob = prob;
            this.doc = doc;
        }

        public int CompareTo(object other_obj)
        {
            DocProbability other = other_obj as DocProbability;
            return -prob.CompareTo(other.prob);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", doc, prob);
        }
    }

    public class TopicProbability : IComparable
    {
        public double prob;
        public int topic;

        public TopicProbability(double prob, int topic)
        {
            this.prob = prob;
            this.topic = topic;
        }

        public int CompareTo(object other_obj)
        {
            TopicProbability other = other_obj as TopicProbability;
            return -prob.CompareTo(other.prob);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", topic, prob);
        }
    }

    public class LDAAnalysis
    {
        LDASampler lda;

        public LDAAnalysis(LDASampler lda)
        {
            this.lda = lda;
        }

        public int NUM_DOCS
        {
            get
            {
                return lda.NUM_DOCS;
            }
        }

        public int NUM_TOPICS
        {
            get
            {
                return lda.NUM_TOPICS;
            }
        }

        public int NUM_WORDS
        {
            get
            {
                return lda.NUM_WORDS;
            }
        }

        private float[,] _density_of_words_in_topics; // [topic,word]
        public float[,] DensityOfWordsInTopics // [topic,word]
        {
            get
            {
                try
                { 
                    if (null == _density_of_words_in_topics)
                    {
                        Logging.Info("+Generating density_of_words_in_topics");
                        _density_of_words_in_topics = new float[lda.NUM_TOPICS, lda.NUM_WORDS];

                        Parallel.For(0, lda.NUM_TOPICS, (topic) =>
                        //for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                        {
                            for (int word = 0; word < lda.NUM_WORDS; ++word)
                            {
                                _density_of_words_in_topics[topic, word] =
                                    lda.number_of_times_a_topic_has_a_specific_word[topic, word] / lda.number_of_times_a_topic_has_any_word[topic];

                                //density_of_words_in_topics[topic, word] =
                                //    (lda.number_of_times_a_topic_has_a_specific_word[topic, word] + lda.BETA) /
                                //    (lda.number_of_times_a_topic_has_any_word[topic] + lda.NUM_WORDS * lda.BETA);
                            }
                        });

                        Logging.Info("-Generating density_of_words_in_topics");
                    }
                }
                catch (System.OutOfMemoryException ex)
                {
                    // terminate app
                    throw ex;
                }

                return _density_of_words_in_topics;
            }
        }

        private float[,] _density_of_topics_in_documents; // [doc,topic]
        public float[,] DensityOfTopicsInDocuments // [doc,topic]
        {
            get
            {
                try
                { 
                    if (null == _density_of_topics_in_documents)
                    {
                        Logging.Info("+Generating density_of_topics_in_documents");
                        _density_of_topics_in_documents = new float[lda.NUM_DOCS, lda.NUM_TOPICS];

                        Parallel.For(0, lda.NUM_DOCS, (doc) =>
                        //for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
                        {
                            for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                            {
                                _density_of_topics_in_documents[doc, topic] =
                                    lda.number_of_times_doc_has_a_specific_topic[doc, topic] / lda.number_of_times_a_doc_has_any_topic[doc];

                                //density_of_topics_in_documents[doc, topic] =
                                //    (lda.number_of_times_doc_has_a_specific_topic[doc, topic] + lda.ALPHA) /
                                //    (lda.number_of_times_a_doc_has_any_topic[doc] + lda.NUM_TOPICS * lda.ALPHA);
                            }
                        });
                        Logging.Info("-Generating density_of_topics_in_documents");
                    }
                }
                catch (System.OutOfMemoryException ex)
                {
                    // terminate app
                    throw ex;
                }

                return _density_of_topics_in_documents; 
            }
        }

        private float[,] _pseudo_density_of_topics_in_words; // [word,topic]   // pseudo because I haven't yet derived a statistical basis for this measure
        public float[,] PseudoDensityOfTopicsInWords // [word,topic]
        {
            get
            {
                try
                {
                    if (null == _pseudo_density_of_topics_in_words)
                    {
                        Logging.Info("+Generating pseudo_density_of_topics_in_words");
                        _pseudo_density_of_topics_in_words = new float[lda.NUM_WORDS, lda.NUM_TOPICS];
                        for (int word = 0; word < lda.NUM_WORDS; ++word)
                        {
                            float denominator = 0;
                            for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                            {
                                denominator += lda.number_of_times_a_topic_has_a_specific_word[topic, word];

                                //denominator += lda.number_of_times_a_topic_has_a_specific_word[topic, word] + lda.BETA;
                            }

                            for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                            {
                                _pseudo_density_of_topics_in_words[word, topic] =
                                    lda.number_of_times_a_topic_has_a_specific_word[topic, word] / denominator;

                                //pseudo_density_of_topics_in_words[word, topic] =
                                //    (lda.number_of_times_a_topic_has_a_specific_word[topic, word] + lda.BETA) / denominator;
                            }
                        }

                        Logging.Info("+Generating pseudo_density_of_topics_in_words");
                    }
                }
                catch (System.OutOfMemoryException ex)
                {
                    // terminate app
                    throw ex;
                }

                return _pseudo_density_of_topics_in_words;
            }
        }

        [NonSerialized]
        WordProbability[][] density_of_words_in_topics_sorted; // [topic][word]
        public WordProbability[][] DensityOfWordsInTopicsSorted // [topic][word]
        {
            get
            {
                try
                { 
                    // Build this if we need to
                    if (null == density_of_words_in_topics_sorted)
                    {
                        // Work out the sorted ranks
                        density_of_words_in_topics_sorted = new WordProbability[lda.NUM_TOPICS][];
                        for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                        {
                            density_of_words_in_topics_sorted[topic] = new WordProbability[lda.NUM_WORDS];
                            for (int word = 0; word < lda.NUM_WORDS; ++word)
                            {
                                density_of_words_in_topics_sorted[topic][word] = new WordProbability(DensityOfWordsInTopics[topic, word], word);
                            }
                            Array.Sort(density_of_words_in_topics_sorted[topic]);
                        }
                    }
                }
                catch (System.OutOfMemoryException ex)
                {
                    // terminate app
                    throw ex;
                }

                return density_of_words_in_topics_sorted;
            }
        }

        DocProbability[][] density_of_docs_in_topics_sorted; // [topic][doc]
        /// <summary>
        /// [topic][doc]
        /// </summary>
        public DocProbability[][] DensityOfDocsInTopicsSorted // [topic][doc]
        {
            get
            {
                try
                {
                    // Build this if we need to
                    if (null == density_of_docs_in_topics_sorted)
                    {
                        // Work out the sorted ranks
                        density_of_docs_in_topics_sorted = new DocProbability[lda.NUM_TOPICS][];
                        for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                        {
                            density_of_docs_in_topics_sorted[topic] = new DocProbability[lda.NUM_DOCS];
                            for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
                            {
                                density_of_docs_in_topics_sorted[topic][doc] = new DocProbability(DensityOfTopicsInDocuments[doc, topic], doc);
                            }
                            Array.Sort(density_of_docs_in_topics_sorted[topic]);
                        }
                    }
                }
                catch (System.OutOfMemoryException ex)
                {
                    // terminate app
                    throw ex;
                }

                return density_of_docs_in_topics_sorted;
            }
        }

        TopicProbability[][] density_of_topics_in_docs_sorted; // [doc][n]
        /// <summary>
        /// [doc][n]
        /// </summary>
        public TopicProbability[][] DensityOfTopicsInDocsSorted // [doc][n]
        {
            get
            {
                // Build this if we need to
                if (null == density_of_topics_in_docs_sorted)
                {
                    // Work out the sorted ranks
                    density_of_topics_in_docs_sorted = CalculateDensityOfTopicsInDocsSorted(0);
                }

                return density_of_topics_in_docs_sorted;
            }
        }

        TopicProbability[][] density_of_top5_topics_in_docs_sorted; // [doc][n<5]
        /// <summary>
        /// [doc][n<5]
        /// </summary>
        public TopicProbability[][] DensityOfTop5TopicsInDocsSorted // [doc][n<5]
        {
            get
            {
                // Build this if we need to
                if (null == density_of_top5_topics_in_docs_sorted)
                {
                    // Work out the sorted ranks
                    density_of_top5_topics_in_docs_sorted = CalculateDensityOfTopicsInDocsSorted(5);
                }

                return density_of_top5_topics_in_docs_sorted;
            }
        }

        private TopicProbability[][] CalculateDensityOfTopicsInDocsSorted(int max_topics_to_retain)
        {
            try
            {
                TopicProbability[][] local_density_of_topics_in_docs_sorted = new TopicProbability[lda.NUM_DOCS][];

                // How many topics will we remember for each doc?
                int topics_to_retain = max_topics_to_retain;
                if (topics_to_retain <= 0) topics_to_retain = lda.NUM_TOPICS;
                if (topics_to_retain > lda.NUM_TOPICS) topics_to_retain = lda.NUM_TOPICS;

                // Calculate the density
                float[,] densityoftopicsindocuments = DensityOfTopicsInDocuments;
                Parallel.For(0, lda.NUM_DOCS, (doc) =>
                {
                    TopicProbability[] density_of_topics_in_doc = new TopicProbability[lda.NUM_TOPICS];

                    for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                    {
                        density_of_topics_in_doc[topic] = new TopicProbability(densityoftopicsindocuments[doc, topic], topic);
                    }
                    Array.Sort(density_of_topics_in_doc);

                    // Copy the correct number of items to retain
                    if (topics_to_retain == lda.NUM_TOPICS)
                    {
                        local_density_of_topics_in_docs_sorted[doc] = density_of_topics_in_doc;
                    }
                    else
                    {
                        local_density_of_topics_in_docs_sorted[doc] = new TopicProbability[topics_to_retain];
                        Array.Copy(density_of_topics_in_doc, local_density_of_topics_in_docs_sorted[doc], topics_to_retain);
                    }
                });

                return local_density_of_topics_in_docs_sorted;
            }
            catch (System.OutOfMemoryException ex)
            {
                // terminate app
                throw ex;
            }
        }

        TopicProbability[][] density_of_topics_in_docs_scaled_sorted; // [doc][topic]
        /// <summary>
        /// [doc][topic]
        /// </summary>
        public TopicProbability[][] DensityOfTopicsInDocsScaledSorted // [doc][topic]
        {
            get
            {
                try
                { 
                    // Build this if we need to
                    if (null == density_of_topics_in_docs_scaled_sorted)
                    {
                        // This hold how much each topic is used in all the documents
                        double[] total_density_of_topics_in_docs = new double[lda.NUM_TOPICS];
                        for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                        {
                            for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
                            {
                                total_density_of_topics_in_docs[topic] += DensityOfTopicsInDocuments[doc, topic];
                            }
                        }
                    
                        // Work out the sorted ranks
                        density_of_topics_in_docs_scaled_sorted = new TopicProbability[lda.NUM_DOCS][];

                        // For each doc
                        for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
                        {
                            density_of_topics_in_docs_scaled_sorted[doc] = new TopicProbability[lda.NUM_TOPICS];

                            // Get the initial density (sums to unity)
                            for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                            {
                                density_of_topics_in_docs_scaled_sorted[doc][topic] = new TopicProbability(DensityOfTopicsInDocuments[doc, topic], topic);
                            }

                            // Scale each topic density down by the number of docs that use the topic (the more docs that use the topic, the less the topic is weighted)
                            for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                            {
                                density_of_topics_in_docs_scaled_sorted[doc][topic].prob /= total_density_of_topics_in_docs[topic];
                            }

                            // Normalise the column again
                            double total = 0;
                            for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                            {
                                total += density_of_topics_in_docs_scaled_sorted[doc][topic].prob;
                            }
                            for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                            {
                                density_of_topics_in_docs_scaled_sorted[doc][topic].prob /= total;
                            }

                            Array.Sort(density_of_topics_in_docs_scaled_sorted[doc]);
                        }
                    }

                    return density_of_topics_in_docs_scaled_sorted;
                }
                catch (System.OutOfMemoryException ex)
                {
                    // terminate app
                    throw ex;
                }
            }
        }


        public void PrintStats(List<string> words)
        {
            PrintStats_DOCS();
            PrintStats_TOPICS(words);
        }

        public void PrintStats_TOPICS(List<string> words)
        {
            for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
            {
                Console.WriteLine("Topic: {0}", GetDescriptionForTopic(words, topic));
                for (int word = 0; word < 10; ++word)
                {
                    Console.WriteLine("{0} & {1} & {2}", word + 1, words[DensityOfWordsInTopicsSorted[topic][word].word], DensityOfWordsInTopicsSorted[topic][word].prob);
                }
                Console.WriteLine();
            }
        }

        public void PrintStats_DOCS()
        {
            for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
            {
                Console.Write("Doc {0}:", doc);
                for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                {
                    Console.Write("\t{0:0}", 100 * DensityOfTopicsInDocuments[doc, topic]);
                }
                Console.WriteLine();
            }
        }

        public string GetDescriptionForTopic(IList<string> words, int topic)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("{0}. ", topic + 1));

            double last_term_prob = 0;
            for (int t = 0; t < 5 && t < NUM_WORDS; ++t)
            {
                if (last_term_prob / DensityOfWordsInTopicsSorted[topic][t].prob > 10)
                {
                    break;
                }
                last_term_prob = DensityOfWordsInTopicsSorted[topic][t].prob;

                sb.Append(String.Format("{0}; ", words[DensityOfWordsInTopicsSorted[topic][t].word]));
            }

            string description = sb.ToString();
            description = description.TrimEnd(' ', ';');

            return description;
        }

        public string GetDescriptionForTopic(IList<string> words, int topic, bool include_topic_number, string separator, bool stop_at_word_probability_jump = true)
        {
            StringBuilder sb = new StringBuilder();

            if (include_topic_number)
            {
                sb.Append(String.Format("{0}. ", topic + 1));
            }

            double last_term_prob = 0;
            for (int t = 0; t < 5 && t < NUM_WORDS; ++t)
            {
                if (last_term_prob / DensityOfWordsInTopicsSorted[topic][t].prob > 10)
                {
                    if (stop_at_word_probability_jump)
                    {
                        break;
                    }
                    else
                    {
                        sb.Append(" // ");
                    }
                }
                last_term_prob = DensityOfWordsInTopicsSorted[topic][t].prob;

                sb.Append(String.Format("{0}", words[DensityOfWordsInTopicsSorted[topic][t].word]));
                sb.Append(separator);
            }

            string description = sb.ToString();
            if (description.EndsWith(separator))
            {
                description = description.Substring(0, description.Length - separator.Length);
            }

            return description;
        }
    }
}
