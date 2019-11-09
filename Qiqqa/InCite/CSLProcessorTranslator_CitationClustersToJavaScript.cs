using System;
using System.Collections.Generic;
using System.Text;

namespace Qiqqa.InCite
{
    internal class CSLProcessorTranslator_CitationClustersToJavaScript
    {
        /*
         * Creates this:
         
                var CITATION_USES = [
                {
                    "citationId": "<id>",
                    "citationItems": [
                        {
                            id: "ITEM-1"
                        },
                        {
                            id: "ITEM-2"
                        }
	                ],
                    "properties": {
                        "noteIndex": 1
                    }
                }
                    ,
                {
                    "citationItems": [
                        {
                            id: "ITEM-2"
                        }
	                ],
                    "properties": {
                        "noteIndex": 1
                    }
                }                    
                ];

         */

        internal static string Translate(List<CitationCluster> citation_clusters)
        {
            // Translate each cluster into the corresponding csl javascript record
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var CITATION_USES = [");
            bool is_additional_cluster_item = false;
            foreach (CitationCluster citation_cluster in citation_clusters)
            {
                if (is_additional_cluster_item)
                {
                    sb.AppendLine(",");
                }
                else
                {
                    is_additional_cluster_item = true;
                }

                string param_separate_author_date = citation_cluster.citation_items[0].GetParameter(CitationItem.PARAM_SEPARATE_AUTHOR_DATE);
                if (CitationItem.OPTION_SEPARATE_AUTHOR_DATE_TRUE != param_separate_author_date)
                {
                    Translate_CitationCluster(sb, citation_cluster, false, false);
                }
                else
                {
                    Translate_CitationCluster(sb, citation_cluster, false, true);
                    sb.AppendLine(",");
                    Translate_CitationCluster(sb, citation_cluster, true, false);
                }
            }
            sb.AppendLine("];");

            return sb.ToString();
        }

        private static void Translate_CitationCluster(StringBuilder sb, CitationCluster citation_cluster, bool suppress_author, bool suppress_date)
        {
            sb.AppendLine("{");
            sb.AppendLine(String.Format("  \"citationID\": \"{0}\",", citation_cluster.cluster_id));
            sb.AppendLine("  \"citationItems\": [");
            bool is_additional_citation_item = false;
            foreach (CitationItem citation_item in citation_cluster.citation_items)
            {
                if (is_additional_citation_item)
                {
                    sb.AppendLine("    ,");
                }
                else
                {
                    is_additional_citation_item = true;
                }

                sb.AppendLine("    {");
                sb.AppendLine("      \"id\": \"" + citation_item.reference_key + "\"");

                if (suppress_author)
                {
                    sb.AppendLine("     ,\"suppress-author\": 1");
                }

                if (suppress_date)
                {
                    sb.AppendLine("     ,\"author-only\": 1");
                }

                string param_specifier_type = citation_item.GetParameter(CitationItem.PARAM_SPECIFIER_TYPE);
                if (!String.IsNullOrEmpty(param_specifier_type))
                {
                    string param_specifier_location = citation_item.GetParameter(CitationItem.PARAM_SPECIFIER_LOCATION);
                    sb.AppendFormat("     ,\"label\": \"{0}\"\n", param_specifier_type);
                    sb.AppendFormat("     ,\"locator\": \"{0}\"\n", param_specifier_location);
                }

                // Prefix and suffix
                {
                    string prefix = citation_item.GetParameter(CitationItem.PARAM_PREFIX);
                    if (!String.IsNullOrEmpty(prefix))
                    {
                        sb.AppendFormat("     ,\"prefix\": \"{0}\"\n", prefix);
                    }
                    string suffix = citation_item.GetParameter(CitationItem.PARAM_SUFFIX);
                    if (!String.IsNullOrEmpty(suffix))
                    {
                        sb.AppendFormat("     ,\"suffix\": \"{0}\"\n", suffix);
                    }
                }

                sb.AppendLine("    }");

            }

            sb.AppendLine("  ],");
            sb.AppendLine("  \"properties\": {");
            sb.AppendLine("    \"noteIndex\": 1");
            sb.AppendLine("  }");
            sb.AppendLine("}");
        }
    }
}
