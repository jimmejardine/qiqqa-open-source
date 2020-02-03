using System;
using System.Collections.Generic;

namespace QiqqaLegacyFileFormats
{
    class WordInWordIndex
    {
        private string word;
        private int word_id;
        
        private List<int> document_ids = null;
        private List<int> document_ids_count = null;
        private int document_count;

        public DateTime last_accessed = DateTime.MinValue;
        public DateTime last_flushed = DateTime.MinValue;
        public bool needs_flushing = false;

        public WordInWordIndex(string word, int word_id)
        {
            this.word = word;
            this.word_id = word_id;

            this.document_ids = new List<int>();
            this.document_ids_count = new List<int>();
            this.document_count = 0;
        }

        public WordInWordIndex(string word, int word_id, int document_count)
        {
            this.word = word;
            this.word_id = word_id;

            this.document_ids = null;
            this.document_ids_count = null;
            this.document_count = document_count;
        }

        public string Word
        {
            get
            {
                return word;
            }
        }

        public int WordId
        {
            get
            {
                return word_id;
            }
        }
        
        public bool IsLoaded
        {
            get
            {
                return document_ids != null;
            }
        }

        internal void Purge()
        {
            document_ids = null;
            document_ids_count = null;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", word, document_count, IsLoaded ? "Loaded" : "");
        }

        public List<int> DocumentIds
        {
            get
            {
                return document_ids;
            }
        }

        public List<int> DocumentIdsCount
        {
            get
            {
                return document_ids_count;
            }
        }

        public int DocumentCount
        {
            get
            {
                return document_count;
            }
        }

        public void TallyDocId(int document_id)
        {
            if (document_ids.Count != document_ids_count.Count)
            {
                throw new Exception("DocumentIds out of sync");
            }

            for (int i = document_ids.Count-1; i >= 0; --i)
            {
                if (document_ids[i] > document_id)
                {
                    continue;
                }

                // If we already have this document_id, tally one up
                if (document_ids[i] == document_id)
                {
                    document_ids_count[i] += 1;
                    return;
                }

                // If we don't already have this document_id, create a new one
                if (document_ids[i] < document_id)
                {
                    document_ids.Insert(i+1, document_id);
                    document_ids_count.Insert(i+1, 1);
                    document_count = document_ids.Count;
                    return;
                }
            }

            // IF we get here, we have reached the end of the list, so add the smallest document_id
            document_ids.Insert(0, document_id);
            document_ids_count.Insert(0, 1);
            document_count = document_ids.Count;
        }

        public bool SetDocumentIds(List<int> document_ids, List<int> document_ids_count)
        {
            bool corruption_detected = false;

            if (null == document_ids)
            {
                Logging.Warn("document_ids should not be null");
                corruption_detected = true;
            }
            if (null == document_ids_count)
            {
                Logging.Warn("document_ids_count should not be null");
                corruption_detected = true;
            }

            this.document_ids = document_ids;
            this.document_ids_count = document_ids_count;

            if (this.document_ids.Count != this.document_ids_count.Count)
            {
                corruption_detected = true;
            }
            
            if (this.document_count != this.document_ids.Count)
            {
                this.document_count = this.document_ids.Count;
                corruption_detected = true;                
            }

            return corruption_detected;
        }
    }
}
