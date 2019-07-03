using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Qiqqa.Common.TagManagement;
using Qiqqa.Documents.PDF;

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
            lock (tags)
            {
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
                lock (tags)
                {
                    if (requires_sort) tags_sorted.Sort();
                    requires_sort = false;
                    return tags_sorted.AsReadOnly();
                }
            }
        }

        private HashSet<string> Tags
        {
            get
            {
                lock (tags)
                {                    
                    return new HashSet<string>(tags);
                }
            }
        }
    }
}
