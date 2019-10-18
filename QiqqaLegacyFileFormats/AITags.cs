using System;
using System.Collections.Generic;
using ProtoBuf;

namespace QiqqaLegacyFileFormats
{
    [Serializable]
    [ProtoContract]
    public class AITags
    {
        [ProtoMember(1)]
        private DateTime timestamp_generated;
        [ProtoMember(2)]
        private MultiMapSet<string, string> ai_tags_with_documents; // tag -> documents
        [ProtoMember(3)]
        private MultiMapSet<string, string> ai_documents_with_tags; // document -> tags

        public AITags()
        {
            timestamp_generated = DateTime.UtcNow;
            ai_tags_with_documents = new MultiMapSet<string, string>();
            ai_documents_with_tags = new MultiMapSet<string, string>();
        }

        public List<string> GetAllTags()
        {
            return new List<string>(ai_tags_with_documents.Keys);
        }

        public void Associate(string tag, string document_fingerprint)
        {
            ai_tags_with_documents.Add(tag, document_fingerprint);
            ai_documents_with_tags.Add(document_fingerprint, tag);
        }

        public MultiMapSet<string, string> GetTagsWithDocuments()
        {
            return ai_tags_with_documents.Clone();
        }

        public HashSet<string> GetDocumentsWithTag(string tag)
        {
            HashSet<string> results = new HashSet<string>(ai_tags_with_documents.Get(tag));
            return results;
        }

        public int GetTagCount(string tag)
        {
            return ai_tags_with_documents.Get(tag).Count;
        }

        public HashSet<string> GetTagsWithDocument(string document_id)
        {
            HashSet<string> results = new HashSet<string>(ai_documents_with_tags.Get(document_id));
            return results;
        }

        public MultiMapSet<string, string> GetTagsWithDocuments(HashSet<string> documents)
        {
            HashSet<string> relevant_tags = new HashSet<string>();
            foreach (string document in documents)
            {
                relevant_tags.UnionWith(ai_documents_with_tags.Get(document));
            }

            MultiMapSet<string, string> results = new MultiMapSet<string, string>();
            foreach (string relevant_tag in relevant_tags)
            {
                foreach (string relevant_document in ai_tags_with_documents.Get(relevant_tag))
                {
                    if (documents.Contains(relevant_document))
                    {
                        results.Add(relevant_tag, relevant_document);
                    }
                }
            }

            return results;
        }

        public bool IsGettingOld
        {
            get
            {
                return DateTime.UtcNow.Subtract(timestamp_generated).TotalDays > 30;
            }
        }

        public bool HaveNoTags
        {
            get
            {
                if (null == ai_tags_with_documents) return true;                
                return (0 == ai_tags_with_documents.Keys.Count);
            }
        }
    }
}
