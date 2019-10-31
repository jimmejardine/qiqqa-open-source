using System;
using System.Collections.Generic;
using System.Text;
using Utilities.OCR;
using Utilities.Strings;

namespace Qiqqa.Documents.PDF.Search
{
    public static class PDFSearcher
    {
        public static PDFSearchResultSet Search(PDFDocument pdf_document, string terms)
        {
            PDFSearchResultSet search_result_set = new PDFSearchResultSet();

            if (pdf_document.DocumentExists)
            {
                for (int page = 1; page <= pdf_document.PDFRenderer.PageCount; ++page)
                {
                    search_result_set[page] = SearchPage(pdf_document, page, terms, PDFSearcher.MATCH_CONTAINS);
                }
            }

            return search_result_set;
        }

        public static PDFSearchResultSet Search(PDFDocument pdf_document, int page, string terms)
        {
            return Search(pdf_document, page, terms, PDFSearcher.MATCH_CONTAINS);
        }
        
        public static PDFSearchResultSet Search(PDFDocument pdf_document, int page, string terms, MatchDelegate match)
        {
            PDFSearchResultSet search_result_set = new PDFSearchResultSet();

            if (pdf_document.DocumentExists)
            {
                search_result_set[page] = SearchPage(pdf_document, page, terms, match);
            }   

            return search_result_set;
        }

        public delegate bool MatchDelegate(string target, string keyword);
        public static bool MATCH_EXACT(string target, string keyword)
        {            
            return (0 == target.CompareTo(keyword));
        }
        public static bool MATCH_CONTAINS(string target, string keyword)
        {            
            return target.Contains(keyword);
        }

        public static List<PDFSearchResult> SearchPage(PDFDocument pdf_document, int page, string terms, MatchDelegate match)
        {
            // Tidy up the keywords
            if (null == terms)
            {
                terms = "";
            }

            string[] keywords = GenerateIndividualKeywords(terms);

            List<PDFSearchResult> search_results = new List<PDFSearchResult>();
            WordList words = new WordList();
            var SPLITTER_WHITESPACE = new char[] { ' ', '\n', '\r', '\t' };

            // Add the comments
            {
                if (1 == page && !String.IsNullOrEmpty(pdf_document.Comments))
                {
                    var splits = pdf_document.Comments.Split(SPLITTER_WHITESPACE, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var split in splits)
                    {
                        words.Add(new Word { Text = split });
                    }
                }
            }

            // Add the annotations
            {
                foreach (var pdf_annotation in pdf_document.GetAnnotations())
                {
                    if (page == pdf_annotation.Page)
                    {
                        if (!String.IsNullOrEmpty(pdf_annotation.Text))
                        {
                            var splits = pdf_annotation.Text.Split(SPLITTER_WHITESPACE, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var split in splits)
                            {
                                words.Add(new Word { Text = split });
                            }
                        }
                    }
                }
            }

            // Add the PDF running text
            {
                WordList words_pdf = pdf_document.PDFRenderer.GetOCRText(page);
                if (null != words_pdf)
                {
                    words.AddRange(words_pdf);
                }
            }

            // Find the text
            if (null != words && null != keywords)
            {
                // Split keywords
                string[][] split_keywords = new string[keywords.Length][];
                for (int i = 0; i < keywords.Length; ++i)
                {
                    split_keywords[i] = StringTools.Split_NotInDelims(keywords[i].ToLower(), '"', '"', " ").ToArray();
                }

                for (int w = 0; w < words.Count; ++w)
                {
                    Word first_word = words[w];
                    string first_word_lower = first_word.Text.ToLower();

                    for (int i = 0; i < split_keywords.Length; ++i)
                    {
                        // Don't process single keywords that are too short
                        if (2 > split_keywords[i][0].Length)
                        {
                            continue;
                        }

                        // Process the first word - if it doesn't match we are done here
                        if (!match(first_word_lower, split_keywords[i][0]))
                        {
                            continue;
                        }

                        // If there are more words we have to get a little crafty and check the remaining words
                        bool follows_match = true;
                        for (int j = 0; j < split_keywords[i].Length; ++j)
                        {
                            if (w + j < words.Count)
                            {
                                Word follow_word = words[w + j];
                                string follow_word_lower = follow_word.Text.ToLower();
                                if (!match(follow_word_lower, split_keywords[i][j]))
                                {
                                    follows_match = false;
                                    break;
                                }
                            }
                            else
                            {
                                follows_match = false;
                                break;
                            }
                        }

                        // If the remaining words dont match, bail
                        if (!follows_match)
                        {
                            continue;
                        }

                        // If we get here, the word (any any follow words) match
                        {
                            PDFSearchResult search_result = new PDFSearchResult();
                            search_results.Add(search_result);

                            // Store the page
                            search_result.keywords = keywords;
                            search_result.page = page;

                            // Get the words associated with this result
                            {
                                search_result.keyword_index = i;
                                search_result.words = new Word[split_keywords[i].Length];
                                for (int j = 0; j < split_keywords[i].Length; ++j)
                                {
                                    Word follow_word = words[w + j];
                                    search_result.words[j] = follow_word;
                                }
                            }

                            // Create the context sentence
                            {
                                int MIN_CONTEXT_SIZE = 3;
                                int MAX_CONTEXT_SIZE = 10;
                                bool ellipsis_start = false;
                                bool ellipsis_end = false;
                                int w_start = w;
                                while (w_start > 0)
                                {
                                    // Stop at a preceding sentence
                                    if (ContainsASentenceTerminator(words[w_start - 1].Text))
                                    {
                                        if (w - w_start >= MIN_CONTEXT_SIZE)
                                        {
                                            break;
                                        }
                                    }

                                    // Stop if we are going too far
                                    if (w - w_start > MAX_CONTEXT_SIZE)
                                    {
                                        ellipsis_start = true;
                                        break;
                                    }

                                    --w_start;
                                }
                                int w_end = w;
                                while (w_end < words.Count)
                                {
                                    // Stop at the end of a sentence
                                    if (ContainsASentenceTerminator(words[w_end].Text))
                                    {
                                        if (w_end - w >= MIN_CONTEXT_SIZE)
                                        {
                                            break;
                                        }
                                    }

                                    // Stop if we are going too far
                                    if (w_end - w > MAX_CONTEXT_SIZE)
                                    {
                                        ellipsis_end = true;
                                        break;
                                    }


                                    if (w_end + 1 == words.Count)
                                    {
                                        break;
                                    }

                                    ++w_end;
                                }

                                StringBuilder sb = new StringBuilder();
                                sb.AppendFormat("p{0}: ", page);
                                if (ellipsis_start)
                                {
                                    sb.Append("...");
                                }
                                for (int w_current = w_start; w_current <= w_end; ++w_current)
                                {
                                    sb.Append(words[w_current].Text);
                                    sb.Append(" ");
                                }
                                if (ellipsis_end)
                                {
                                    sb.Append("...");
                                }
                                search_result.context_sentence = sb.ToString();
                            }
                        }
                    }
                }
            }

            return search_results;
        }

        public static string[] GenerateIndividualKeywords(string keywords)
        {
            // Strip out the BIG lucene words
            keywords = keywords.Replace("AND", "");
            keywords = keywords.Replace("OR", "");
            keywords = keywords.Replace("NOT", "");

            // Then beat it into shape
            keywords = keywords.ToLower();

            // Break it into individual keywords with quotes preserving spaces
            List<String> keywords_split = StringTools.Split_NotInDelims(keywords, '"', '"', " ", StringSplitOptions.RemoveEmptyEntries);

            return keywords_split.ToArray();
        }

        private static bool ContainsASentenceTerminator(string s)
        {
            return false
                || s.Contains(".")
                || s.Contains(":")
                || s.Contains(";")
                ;
        }
    }
}
