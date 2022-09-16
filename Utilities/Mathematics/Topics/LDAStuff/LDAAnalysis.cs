using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities.GUI;

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
        private LDASampler lda;

        public LDAAnalysis(LDASampler lda)
        {
            this.lda = lda;
        }

        public int NUM_DOCS => lda.NUM_DOCS;

        public int NUM_TOPICS => lda.NUM_TOPICS;

        public int NUM_WORDS => lda.NUM_WORDS;

        private object dwt_init_lock = new object();

        private float[,] _density_of_words_in_topics; // [topic,word]
        public float[,] DensityOfWordsInTopics // [topic,word]
        {
            get
            {
                if (null == _density_of_words_in_topics)
                {
                    // WARNING: note that the lock *only* locks the creation/instantiation of the LDA arrays!
                    // As these getters may be invoked from multiple threads it is PARAMOUNT to NOT set the 
                    // cache/recall variables carrying these 2D arrays until we're FINISHED creating them,
                    // or the create/init-only lock system will NOT work.
                    //
                    // Also, we MUST check if the relevant cache/recall variable is set upon entry into the
                    // critical section as the way we coded this may be reducing the lock() calls during regular
                    // operations, but at startup, multiple calls will consequently take this branch and 
                    // WAIT at the lock() to be released. As only the first one through should run the init code,
                    // this "seemingly superfluous check" is MANDATORY.
                    lock (dwt_init_lock)
                    {
                        if (_density_of_words_in_topics == null)
                        {
                            try
                            {
                                Logging.Info("+Generating density_of_words_in_topics");

                                var a = new float[lda.NUM_TOPICS, lda.NUM_WORDS];

                                Parallel.For(0, lda.NUM_TOPICS, (topic) =>
                                //for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                                {
                                    for (int word = 0; word < lda.NUM_WORDS; ++word)
                                    {
                                        a[topic, word] = lda.number_of_times_a_topic_has_a_specific_word[topic, word] / lda.number_of_times_a_topic_has_any_word[topic];

                                        //density_of_words_in_topics[topic, word] =
                                        //    (lda.number_of_times_a_topic_has_a_specific_word[topic, word] + lda.BETA) /
                                        //    (lda.number_of_times_a_topic_has_any_word[topic] + lda.NUM_WORDS * lda.BETA);
                                    }
                                });

                                _density_of_words_in_topics = a;

                                Logging.Info("-Generating density_of_words_in_topics");
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Internal LDAAnalysis error.");

                                // terminate app
                                throw;
                            }
                        }
                    }
                }

                return _density_of_words_in_topics;
            }
        }

        private object dtd_init_lock = new object();

        private float[,] _density_of_topics_in_documents; // [doc,topic]
        public float[,] DensityOfTopicsInDocuments // [doc,topic]
        {
            get
            {
                if (null == _density_of_topics_in_documents)
                {
                    // WARNING: note that the lock *only* locks the creation/instantiation of the LDA arrays!
                    // As these getters may be invoked from multiple threads it is PARAMOUNT to NOT set the 
                    // cache/recall variables carrying these 2D arrays until we're FINISHED creating them,
                    // or the create/init-only lock system will NOT work.
                    //
                    // Also, we MUST check if the relevant cache/recall variable is set upon entry into the
                    // critical section as the way we coded this may be reducing the lock() calls during regular
                    // operations, but at startup, multiple calls will consequently take this branch and 
                    // WAIT at the lock() to be released. As only the first one through should run the init code,
                    // this "seemingly superfluous check" is MANDATORY.
                    lock (dtd_init_lock)
                    {
                        if (null == _density_of_topics_in_documents)
                        {
                            try
                            {
                                Logging.Info("+Generating density_of_topics_in_documents");
                                var a = new float[lda.NUM_DOCS, lda.NUM_TOPICS];

                                Parallel.For(0, lda.NUM_DOCS, (doc) =>
                                //for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
                                {
                                    for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                                    {
                                        a[doc, topic] = lda.number_of_times_doc_has_a_specific_topic[doc, topic] / lda.number_of_times_a_doc_has_any_topic[doc];

                                    //density_of_topics_in_documents[doc, topic] =
                                    //    (lda.number_of_times_doc_has_a_specific_topic[doc, topic] + lda.ALPHA) /
                                    //    (lda.number_of_times_a_doc_has_any_topic[doc] + lda.NUM_TOPICS * lda.ALPHA);
                                }
                                });

                                _density_of_topics_in_documents = a;
                                Logging.Info("-Generating density_of_topics_in_documents");
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Internal LDAAnalysis error.");

                                // terminate app
                                throw;
                            }
                        }
                    }
                }

                return _density_of_topics_in_documents;
            }
        }

        private object pdtw_init_lock = new object();

        private float[,] _pseudo_density_of_topics_in_words; // [word,topic]   // pseudo because I haven't yet derived a statistical basis for this measure
        public float[,] PseudoDensityOfTopicsInWords // [word,topic]
        {
            get
            {
                if (null == _pseudo_density_of_topics_in_words)
                {
                    // WARNING: note that the lock *only* locks the creation/instantiation of the LDA arrays!
                    // As these getters may be invoked from multiple threads it is PARAMOUNT to NOT set the 
                    // cache/recall variables carrying these 2D arrays until we're FINISHED creating them,
                    // or the create/init-only lock system will NOT work.
                    //
                    // Also, we MUST check if the relevant cache/recall variable is set upon entry into the
                    // critical section as the way we coded this may be reducing the lock() calls during regular
                    // operations, but at startup, multiple calls will consequently take this branch and 
                    // WAIT at the lock() to be released. As only the first one through should run the init code,
                    // this "seemingly superfluous check" is MANDATORY.
                    lock (pdtw_init_lock)
                    {
                        if (null == _pseudo_density_of_topics_in_words)
                        {
                            try
                            {
                                Logging.Info("+Generating pseudo_density_of_topics_in_words");
                                var a = new float[lda.NUM_WORDS, lda.NUM_TOPICS];
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
                                        a[word, topic] = lda.number_of_times_a_topic_has_a_specific_word[topic, word] / denominator;

                                        //pseudo_density_of_topics_in_words[word, topic] =
                                        //    (lda.number_of_times_a_topic_has_a_specific_word[topic, word] + lda.BETA) / denominator;
                                    }
                                }

                                _pseudo_density_of_topics_in_words = a;
                                Logging.Info("+Generating pseudo_density_of_topics_in_words");
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Internal LDAAnalysis error.");

                                // terminate app
                                throw;
                            }
                        }
                    }
                }

                return _pseudo_density_of_topics_in_words;
            }
        }

        private object dwts_init_lock = new object();

        [NonSerialized]
        private WordProbability[][] density_of_words_in_topics_sorted; // [topic][word]
        public WordProbability[][] DensityOfWordsInTopicsSorted // [topic][word]
        {
            get
            {
                // Build this if we need to
                if (null == density_of_words_in_topics_sorted)
                {
                    // WARNING: note that the lock *only* locks the creation/instantiation of the LDA arrays!
                    // As these getters may be invoked from multiple threads it is PARAMOUNT to NOT set the 
                    // cache/recall variables carrying these 2D arrays until we're FINISHED creating them,
                    // or the create/init-only lock system will NOT work.
                    //
                    // Also, we MUST check if the relevant cache/recall variable is set upon entry into the
                    // critical section as the way we coded this may be reducing the lock() calls during regular
                    // operations, but at startup, multiple calls will consequently take this branch and 
                    // WAIT at the lock() to be released. As only the first one through should run the init code,
                    // this "seemingly superfluous check" is MANDATORY.
                    lock (dwts_init_lock)
                    {
                        if (null == density_of_words_in_topics_sorted)
                        {
                            try
                            {
                                // Work out the sorted ranks
                                var a = new WordProbability[lda.NUM_TOPICS][];
                                for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                                {
                                    a[topic] = new WordProbability[lda.NUM_WORDS];
                                    for (int word = 0; word < lda.NUM_WORDS; ++word)
                                    {
                                        a[topic][word] = new WordProbability(DensityOfWordsInTopics[topic, word], word);
                                    }
                                    Array.Sort(a[topic]);
                                }

                                density_of_words_in_topics_sorted = a;
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Internal LDAAnalysis error.");

                                // terminate app
                                throw;
                            }
                        }
                    }
                }

                return density_of_words_in_topics_sorted;
            }
        }

        private object ddts_init_lock = new object();

        private DocProbability[][] density_of_docs_in_topics_sorted; // [topic][doc]
        /// <summary>
        /// [topic][doc]
        /// </summary>
        public DocProbability[][] DensityOfDocsInTopicsSorted // [topic][doc]
        {
            get
            {
                // Build this if we need to
                if (null == density_of_docs_in_topics_sorted)
                {
                    // WARNING: note that the lock *only* locks the creation/instantiation of the LDA arrays!
                    // As these getters may be invoked from multiple threads it is PARAMOUNT to NOT set the 
                    // cache/recall variables carrying these 2D arrays until we're FINISHED creating them,
                    // or the create/init-only lock system will NOT work.
                    //
                    // Also, we MUST check if the relevant cache/recall variable is set upon entry into the
                    // critical section as the way we coded this may be reducing the lock() calls during regular
                    // operations, but at startup, multiple calls will consequently take this branch and 
                    // WAIT at the lock() to be released. As only the first one through should run the init code,
                    // this "seemingly superfluous check" is MANDATORY.
                    lock (ddts_init_lock)
                    {
                        if (null == density_of_docs_in_topics_sorted)
                        {
                            try
                            {
                                // Work out the sorted ranks
                                var a = new DocProbability[lda.NUM_TOPICS][];
                                for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                                {
                                    a[topic] = new DocProbability[lda.NUM_DOCS];
                                    for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
                                    {
                                        a[topic][doc] = new DocProbability(DensityOfTopicsInDocuments[doc, topic], doc);
                                    }
                                    Array.Sort(a[topic]);
                                }

                                density_of_docs_in_topics_sorted = a;
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Internal LDAAnalysis error.");

                                // terminate app
                                throw;
                            }
                        }
                    }
                }

                return density_of_docs_in_topics_sorted;
            }
        }

        private object dtds_init_lock = new object();

        private TopicProbability[][] density_of_topics_in_docs_sorted; // [doc][n]
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
                    // WARNING: note that the lock *only* locks the creation/instantiation of the LDA arrays!
                    // As these getters may be invoked from multiple threads it is PARAMOUNT to NOT set the 
                    // cache/recall variables carrying these 2D arrays until we're FINISHED creating them,
                    // or the create/init-only lock system will NOT work.
                    //
                    // Also, we MUST check if the relevant cache/recall variable is set upon entry into the
                    // critical section as the way we coded this may be reducing the lock() calls during regular
                    // operations, but at startup, multiple calls will consequently take this branch and 
                    // WAIT at the lock() to be released. As only the first one through should run the init code,
                    // this "seemingly superfluous check" is MANDATORY.
                    lock (dtds_init_lock)
                    {
                        if (null == density_of_topics_in_docs_sorted)
                        {
                            // Work out the sorted ranks
                            density_of_topics_in_docs_sorted = CalculateDensityOfTopicsInDocsSorted(0);
                        }
                    }
                }

                return density_of_topics_in_docs_sorted;
            }
        }

        private object dt5tds_init_lock = new object();

        private TopicProbability[][] density_of_top5_topics_in_docs_sorted; // [doc][n<5]
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
                    // WARNING: note that the lock *only* locks the creation/instantiation of the LDA arrays!
                    // As these getters may be invoked from multiple threads it is PARAMOUNT to NOT set the 
                    // cache/recall variables carrying these 2D arrays until we're FINISHED creating them,
                    // or the create/init-only lock system will NOT work.
                    //
                    // Also, we MUST check if the relevant cache/recall variable is set upon entry into the
                    // critical section as the way we coded this may be reducing the lock() calls during regular
                    // operations, but at startup, multiple calls will consequently take this branch and 
                    // WAIT at the lock() to be released. As only the first one through should run the init code,
                    // this "seemingly superfluous check" is MANDATORY.
                    lock (dt5tds_init_lock)
                    {
                        if (null == density_of_top5_topics_in_docs_sorted)
                        {
                            // Work out the sorted ranks
                            density_of_top5_topics_in_docs_sorted = CalculateDensityOfTopicsInDocsSorted(5);
                        }
                    }
                }

                return density_of_top5_topics_in_docs_sorted;
            }
        }

        private TopicProbability[][] CalculateDensityOfTopicsInDocsSorted(int max_topics_to_retain)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                TopicProbability[][] local_density_of_topics_in_docs_sorted = new TopicProbability[lda.NUM_DOCS][];

                // How many topics will we remember for each doc?
                int topics_to_retain = max_topics_to_retain;
                if (topics_to_retain <= 0)
                {
                    topics_to_retain = lda.NUM_TOPICS;
                }
                else if (topics_to_retain > lda.NUM_TOPICS)
                {
                    topics_to_retain = lda.NUM_TOPICS;
                }

                // Calculate the density
                float[,] densityoftopicsindocuments = DensityOfTopicsInDocuments;
                Parallel.For(0, lda.NUM_DOCS, (doc) =>
                //for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
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
            catch (Exception ex)
            {
                Logging.Error(ex, "Internal LDAAnalysis error.");

                // terminate app
                throw;
            }
        }

        private object dtdss_init_lock = new object();

        private TopicProbability[][] density_of_topics_in_docs_scaled_sorted; // [doc][topic]
        /// <summary>
        /// [doc][topic]
        /// </summary>
        public TopicProbability[][] DensityOfTopicsInDocsScaledSorted // [doc][topic]
        {
            get
            {
                // Build this if we need to
                if (null == density_of_topics_in_docs_scaled_sorted)
                {
                    // WARNING: note that the lock *only* locks the creation/instantiation of the LDA arrays!
                    // As these getters may be invoked from multiple threads it is PARAMOUNT to NOT set the 
                    // cache/recall variables carrying these 2D arrays until we're FINISHED creating them,
                    // or the create/init-only lock system will NOT work.
                    //
                    // Also, we MUST check if the relevant cache/recall variable is set upon entry into the
                    // critical section as the way we coded this may be reducing the lock() calls during regular
                    // operations, but at startup, multiple calls will consequently take this branch and 
                    // WAIT at the lock() to be released. As only the first one through should run the init code,
                    // this "seemingly superfluous check" is MANDATORY.
                    lock (dtdss_init_lock)
                    {
                        if (null == density_of_topics_in_docs_scaled_sorted)
                        {
                            try
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
                                var a = new TopicProbability[lda.NUM_DOCS][];

                                // For each doc
                                for (int doc = 0; doc < lda.NUM_DOCS; ++doc)
                                {
                                    a[doc] = new TopicProbability[lda.NUM_TOPICS];

                                    // Get the initial density (sums to unity)
                                    for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                                    {
                                        a[doc][topic] = new TopicProbability(DensityOfTopicsInDocuments[doc, topic], topic);
                                    }

                                    // Scale each topic density down by the number of docs that use the topic (the more docs that use the topic, the less the topic is weighted)
                                    for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                                    {
                                        a[doc][topic].prob /= total_density_of_topics_in_docs[topic];
                                    }

                                    // Normalise the column again
                                    double total = 0;
                                    for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                                    {
                                        total += a[doc][topic].prob;
                                    }
                                    for (int topic = 0; topic < lda.NUM_TOPICS; ++topic)
                                    {
                                        a[doc][topic].prob /= total;
                                    }

                                    Array.Sort(a[doc]);
                                }

                                density_of_topics_in_docs_scaled_sorted = a;
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Internal LDAAnalysis error.");

                                // terminate app
                                throw;
                            }
                        }
                    }
                }

                return density_of_topics_in_docs_scaled_sorted;
            }
        }
    }
}
