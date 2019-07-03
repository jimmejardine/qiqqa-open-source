using System;
using Qiqqa.Brainstorm.Common;
using Qiqqa.Brainstorm.Common.Searching;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class ThemeNodeContent : Searchable, RecurrentNodeContent
    {
        public string tags;
        public string library_fingerprint;

        public ThemeNodeContent(string tags, string library_fingerprint)
        {
            this.tags = tags;
            this.library_fingerprint = library_fingerprint;
        }

        public bool MatchesKeyword(string keyword)
        {
            return
                false
                || (null != tags) && tags.ToLower().Contains(keyword)
                ;
        }

        public override bool Equals(object obj)
        {
            ThemeNodeContent other = obj as ThemeNodeContent;
            if (null == other) return false;

            if (this.tags != other.tags) return false;
            if (this.library_fingerprint != other.library_fingerprint) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + this.tags.GetHashCode();
            hash = hash * 37 + this.library_fingerprint.GetHashCode();
            return hash;
        }


        public string Tags
        {
            get
            {
                return this.tags;
            }

            set
            {
                this.tags = value;
            }
        }
    }
}
