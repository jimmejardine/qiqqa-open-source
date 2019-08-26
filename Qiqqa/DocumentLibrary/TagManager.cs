using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Qiqqa.Common.TagManagement;
using Qiqqa.Documents.PDF;
using Utilities;

namespace Qiqqa.DocumentLibrary
{
    public class TagManager
    {
        #region --- Singleton ------------------------------------------------------------------------------------

        public static readonly TagManager Instance = new TagManager();

        private TagManager()
        {
        }

        #endregion

        HashSet<string> tags = new HashSet<string>();
        List<string> tags_sorted = new List<string>();
        private object tags_lock = new object();
        bool requires_sort = false;

        internal void ProcessDocument(PDFDocument pdf_document)
        {
            if (pdf_document.Deleted) return;

            ProcessTags(TagTools.ConvertTagBundleToTags(pdf_document.Tags));

            foreach (var pdf_annotation in pdf_document.Annotations)
            {
                ProcessAnnotation(pdf_annotation);
            }
        }

        internal void ProcessAnnotation(PDFAnnotation pdf_annotation)
        {
            if (pdf_annotation.Deleted) return;

            ProcessTags(TagTools.ConvertTagBundleToTags(pdf_annotation.Tags));
        }

        internal void ProcessTags(IEnumerable<string> tags_list)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (tags_lock)
            {
                l1_clk.LockPerfTimerStop();
                foreach (var tag in tags_list)
                {
                    AddTag_LOCK(tag);
                }
            }
        }

        private void AddTag_LOCK(String tag)
        {
            bool is_new_tag = tags.Add(tag);
            if (is_new_tag)
            {
                tags_sorted.Add(tag);
                requires_sort = true;
            }
        }

        public ReadOnlyCollection<string> SortedTags
        {
            get
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (tags_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    if (requires_sort)
                    {
                        tags_sorted.Sort();
                    }
                    requires_sort = false;
                    return tags_sorted.AsReadOnly();
                }
            }
        }

        private HashSet<string> Tags
        {
            get
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (tags_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    return new HashSet<string>(tags);
                }
            }
        }
    }
}
