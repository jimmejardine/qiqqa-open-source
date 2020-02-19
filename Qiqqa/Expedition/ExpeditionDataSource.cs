using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Utilities.Mathematics.Topics.LDAStuff;

namespace Qiqqa.Expedition
{
    [Serializable]
    public class ExpeditionDataSource
    {
        public DateTime date_created;

        public List<string> words;
        public Dictionary<string, int> words_index;

        public List<string> docs;
        public Dictionary<string, int> docs_index;

        public int[][] words_in_docs; // jagged array [doc][word]

        public LDASampler lda_sampler;

        [NonSerialized]
        private LDAAnalysis lda_analysis = null;

        public LDAAnalysis LDAAnalysis
        {
            get
            {
                if (null == lda_analysis)
                {
                    // TODO: Analyse code flow and find out if we can DELAY-LOAD expedition and brainstorm
                    // data as those CAN be wild & huge and cause OutOfMemory issues, which we cannot fix in-app
                    // as these buggers load as part of the init phase. :-(
                    lda_analysis = new LDAAnalysis(lda_sampler);
                }

                return lda_analysis;
            }
        }

        [NonSerialized]
        private Color[] colours;
        public Color[] Colours
        {
            get
            {
                if (null == colours)
                {
                    int num_topics = lda_sampler.NumTopics;

                    colours = new Color[num_topics];
                    for (int i = 0; i < num_topics; ++i)
                    {
                        colours[i] = LDAAnalysisTools.GetTopicColour(i, num_topics);
                    }
                }

                return colours;
            }
        }

        internal void RebuildIndices()
        {
            words_index = new Dictionary<string, int>();
            for (int i = 0; i < words.Count; ++i)
            {
                words_index[words[i]] = i;
            }

            docs_index = new Dictionary<string, int>();
            for (int i = 0; i < docs.Count; ++i)
            {
                docs_index[docs[i]] = i;
            }
        }

        public void PrintStats()
        {
            PrintStats_DOCS();
            PrintStats_TOPICS();
        }

        public void PrintStats_TOPICS()
        {
            for (int topic = 0; topic < LDAAnalysis.NUM_TOPICS; ++topic)
            {
                Console.WriteLine("Topic: {0}", GetDescriptionForTopic(topic));
                for (int word = 0; word < 10; ++word)
                {
                    Console.WriteLine("{0} & {1} & {2}", word + 1, words[LDAAnalysis.DensityOfWordsInTopicsSorted[topic][word].word], LDAAnalysis.DensityOfWordsInTopicsSorted[topic][word].prob);
                }
                Console.WriteLine();
            }
        }

        public void PrintStats_DOCS()
        {
            for (int doc = 0; doc < LDAAnalysis.NUM_DOCS; ++doc)
            {
                Console.Write("Doc {0}:", doc);
                for (int topic = 0; topic < LDAAnalysis.NUM_TOPICS; ++topic)
                {
                    Console.Write("\t{0:0}", 100 * LDAAnalysis.DensityOfTopicsInDocuments[doc, topic]);
                }
                Console.WriteLine();
            }
        }

        public string DumpTopicsPopularity()
        {
            StringBuilder sb = new StringBuilder();

            // Count how many docs count each topic in their top-5
            int TOP_N = Math.Min(5, LDAAnalysis.NUM_TOPICS); // Must be less than or equal to 5
            int[,] topics_popularity = new int[LDAAnalysis.NUM_TOPICS, TOP_N];

            {
                TopicProbability[][] density_of_top5_topics_in_docs_sorted = LDAAnalysis.DensityOfTop5TopicsInDocsSorted; // [doc][n<5]                
                for (int doc = 0; doc < LDAAnalysis.NUM_DOCS; ++doc)
                {
                    for (int n = 0; n < TOP_N; ++n)
                    {
                        int topic = density_of_top5_topics_in_docs_sorted[doc][n].topic;
                        ++topics_popularity[topic, n];
                    }
                }
            }

            // Show the descriptive keywords for each topic
            {
                for (int topic = 0; topic < LDAAnalysis.NUM_TOPICS; ++topic)
                {
                    string description = GetDescriptionForTopic(topic, true, ";", false);
                    sb.AppendFormat("{0}", description);
                    sb.AppendLine();
                    for (int n = 0; n < TOP_N; ++n)
                    {
                        sb.AppendFormat("{0}de:{1}\t", n + 1, topics_popularity[topic, n]);
                    }
                    sb.AppendLine();
                }
            }

            sb.AppendLine();

            {
                for (int topic = 0; topic < LDAAnalysis.NUM_TOPICS; ++topic)
                {
                    sb.AppendFormat("{0}\t", topic);

                    for (int n = 0; n < TOP_N; ++n)
                    {
                        sb.AppendFormat("{0}\t", topics_popularity[topic, n]);
                    }
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public string GetDescriptionForTopic(int topic, bool include_topic_number = true, string separator = "; ", bool stop_at_word_probability_jump = true)
        {
            StringBuilder sb = new StringBuilder();

            if (include_topic_number)
            {
                sb.Append(String.Format("{0}. ", topic + 1));
            }

            double last_term_prob = 0;
            for (int t = 0; t < 5 && t < LDAAnalysis.NUM_WORDS; ++t)
            {
                if (last_term_prob / LDAAnalysis.DensityOfWordsInTopicsSorted[topic][t].prob > 10)
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
                last_term_prob = LDAAnalysis.DensityOfWordsInTopicsSorted[topic][t].prob;

                sb.Append(String.Format("{0}", words[LDAAnalysis.DensityOfWordsInTopicsSorted[topic][t].word]));
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
