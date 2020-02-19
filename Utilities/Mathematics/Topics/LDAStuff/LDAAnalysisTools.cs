using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Utilities.Mathematics.Topics.LDAStuff
{
    public class LDAAnalysisTools
    {
        public static Color GetTopicColour(int topic, int MAX_TOPICS)
        {
            int num_topics = MAX_TOPICS;
            int i = topic;

            {
                double hue = 360.0 * i / num_topics;

                //double value = 1 - 0.25 * (i % 3);
                //double saturation = 1 - value / 2;

                //double value = 1 - 0.25 * (i % 3);
                //double saturation = 1 - value / 3;

                double value = 1 - 0.15 * (i % 3);
                double saturation = 0.5 - 0.1 * (2 - i % 3);

                System.Drawing.Color color = Colours.HSVToColor(hue, saturation, value);
                return Color.FromRgb(color.R, color.G, color.B);
            }
        }

        public static int[] GetDocumentsSimilarToDistribution(LDAAnalysis lda_analysis, float[] distribution)
        {
            int[] docs = new int[lda_analysis.NUM_DOCS];
            double[] similarities = new double[lda_analysis.NUM_DOCS];

            // Initial ordering
            for (int doc_i = 0; doc_i < lda_analysis.NUM_DOCS; ++doc_i)
            {
                docs[doc_i] = doc_i;
            }

            // Similarities
            for (int doc_i = 0; doc_i < lda_analysis.NUM_DOCS; ++doc_i)
            {
                similarities[doc_i] = CalculateSimilarity_JS(lda_analysis, distribution, doc_i);
            }

            // Sort
            Array.Sort(similarities, docs);
            Array.Reverse(similarities);
            Array.Reverse(docs);

            return docs;
        }

        private static double CalculateSimilarity_JS(LDAAnalysis lda_analysis, float[] distribution, int doc_i)
        {
            // Calculate the similarity of the "topic distribution" for this pair of words using the Jensen-Shannon divergence (always finite)
            double JS_12 = 0.0;
            double JS_21 = 0.0;
            for (int topic = 0; topic < lda_analysis.NUM_TOPICS; ++topic)
            {
                double P = distribution[topic];
                double Q = lda_analysis.DensityOfTopicsInDocuments[doc_i, topic];
                double M = (P + Q) / 2.0;

                if (0 != M)
                {
                    JS_12 += P * Math.Log(P / M, 2);
                    JS_21 += Q * Math.Log(Q / M, 2);
                }
            }

            return 1.0 - (JS_12 + JS_21) / 2.0;
        }
    }
}
