using System;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class LibraryNodeContent : ISearchable, IRecurrentNodeContent
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
            return title?.ToLower().Contains(keyword) ?? false;
        }

        public override bool Equals(object obj)
        {
            LibraryNodeContent other = obj as LibraryNodeContent;
            if (null == other) return false;

            if (library_fingerprint != other.library_fingerprint) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + library_fingerprint.GetHashCode();
            return hash;
        }

        public string Title
        {
            get => title;

            set => title = value;
        }
    }
}
