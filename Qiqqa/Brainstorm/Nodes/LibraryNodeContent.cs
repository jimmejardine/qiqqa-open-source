using System;
using Qiqqa.Brainstorm.Common;
using Qiqqa.Brainstorm.Common.Searching;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class LibraryNodeContent : Searchable, RecurrentNodeContent
    {
        public string title;
        public string library_fingerprint;

        public LibraryNodeContent(string title, string library_fingerprint)
        {
            this.title = title;
            this.library_fingerprint = library_fingerprint;
        }

        public bool MatchesKeyword(string keyword)
        {
            return
                false
                || (null != title) && title.ToLower().Contains(keyword)
                ;
        }

        public override bool Equals(object obj)
        {
            LibraryNodeContent other = obj as LibraryNodeContent;
            if (null == other) return false;

            if (this.library_fingerprint != other.library_fingerprint) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + this.library_fingerprint.GetHashCode();
            return hash;
        }

        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
            }
        }
    }
}
