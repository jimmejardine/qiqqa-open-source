using System.Collections.Generic;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.AITagsStuff;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.Strings;

namespace Qiqqa.Documents.PDF.InfoBarStuff.PDFDocumentTagCloudStuff
{
    public class PDFDocumentTagCloudBuilder
    {
        public static List<TagCloudEntry> BuildTagCloud(Library library, PDFDocument pdf_document)
        {
            int MAX_PAGE_LIMIT = 20;

            AITags ai_tags = pdf_document.Library.AITagManager.AITags;

            HashSet<string> autotags = ai_tags.GetTagsWithDocument(pdf_document.Fingerprint);
            foreach (var tag in TagTools.ConvertTagBundleToTags(pdf_document.Tags))
            {
                autotags.Add(tag);
            }

            
            CountingDictionary<string> word_counts = new CountingDictionary<string>();
            {
                Logging.Info("+Counting the autotags");
                int total_tags = 0;

                for (int page = 1; page <= pdf_document.PDFRenderer.PageCount && page < MAX_PAGE_LIMIT; ++page)
                {
                    string page_text = pdf_document.PDFRenderer.GetFullOCRText(page);
                    foreach (string autotag in autotags)
                    {
                        int word_count = StringTools.CountStringOccurence(page_text, autotag);
                        if (0 < word_count)
                        {
                            ++total_tags;
                            word_counts.TallyOne(autotag);
                        }
                    }
                }
                Logging.Info("-Counting the autotags: total_occurences={0} unique_tags={1}", total_tags, word_counts.Count);
            }

            Logging.Info("+Building the ratios");
            List<TagCloudEntry> entries = new List<TagCloudEntry>();
            foreach (var pair in word_counts)
            {
                int document_count = ai_tags.GetTagCount(pair.Key) + 1;

                // Limit the wordcount to cull the hyperfrequent words
                int word_count = pair.Value;

                TagCloudEntry entry = new TagCloudEntry();
                entry.word = pair.Key;
                entry.word_count = word_count;
                entry.document_count = document_count;
                entry.importance = word_count / (double)document_count;

                entries.Add(entry);
            }
            Logging.Info("-Building the ratios");

            entries.Sort(delegate(TagCloudEntry a, TagCloudEntry b) { return -Sorting.Compare(a.importance, b.importance); });
            return entries;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            Library library = Library.GuestInstance;
            PDFDocument pdf_document = library.PDFDocuments[1];
            List<TagCloudEntry> entries = BuildTagCloud(library, pdf_document);

            TagCloudRendererControl tcrc = new TagCloudRendererControl();
            tcrc.Entries = entries;

            ControlHostingWindow window = new ControlHostingWindow("Tag cloud", tcrc);
            window.Show();
        }
#endif

        #endregion
    }    
}
