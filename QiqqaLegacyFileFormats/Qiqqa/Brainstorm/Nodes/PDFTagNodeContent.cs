using System;
using System.Globalization;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class PDFTagNodeContent : ISearchable, IRecurrentNodeContent
    {
        private string library_id;
        private string tag;

        public PDFTagNodeContent(string library_id, string tag)
        {
            this.library_id = library_id;
            this.tag = tag;
        }

        public bool MatchesKeyword(string keyword)
        {
            if ((null != tag) && tag.ToLower().Contains(keyword)) return true;
            return false;
        }

        #region  --- Binding properties ------------------------------------

        public string Tag => tag;

        #endregion

        public override bool Equals(object obj)
        {
            PDFTagNodeContent other = obj as PDFTagNodeContent;
            if (null == other) return false;

            if (library_id != other.library_id) return false;
            if (tag != other.tag) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + library_id.GetHashCode();
            hash = hash * 37 + tag.GetHashCode();
            return hash;
        }

    }
}
