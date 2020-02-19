using System;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class ThemeNodeContent : ISearchable, IRecurrentNodeContent
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
            return tags?.ToLower().Contains(keyword) ?? false;
        }

        public override bool Equals(object obj)
        {
            ThemeNodeContent other = obj as ThemeNodeContent;
            if (null == other) return false;

            if (tags != other.tags) return false;
            if (library_fingerprint != other.library_fingerprint) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + tags.GetHashCode();
            hash = hash * 37 + library_fingerprint.GetHashCode();
            return hash;
        }


        public string Tags
        {
            get => tags;

            set => tags = value;
        }
    }
}
