using System;

namespace QiqqaLegacyFileFormats          // namespace Utilities.Language.Buzzwords
{
    [Serializable]
    public class NGram : IComparable<NGram>
    {
        public int n;
        public string text;
        public bool is_acronym;

        public NGram()
        {
        }

        public NGram(int n, string text, bool is_acronym)
        {
            this.n = n;
            this.text = text;
            this.is_acronym = is_acronym;
        }

        public override string ToString()
        {
            return text;
        }

        /// <summary>
        /// Overridden so that NGram behaves just just its underlying text string in dictionaries
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return text.GetHashCode();
        }

        /// <summary>
        /// Overridden so that NGram behaves just just its underlying text string in dictionaries
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            NGram other = obj as NGram;
            if (null == other) return false;
            return text.Equals(other.text);
        }

        public int CompareTo(NGram other)
        {
            return text.CompareTo(other.text);
        }
    }
}
