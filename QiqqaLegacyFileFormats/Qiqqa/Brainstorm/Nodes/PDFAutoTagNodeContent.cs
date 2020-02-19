using System;
using System.Globalization;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class PDFAutoTagNodeContent : ISearchable, IRecurrentNodeContent
    {
        private string library_id;
        private string tag;

#if false
        [NonSerialized]
        private Library library = null;
        public Library Library
        {
            get
            {
                if (null == library)
                {
                    library = WebLibraryManager.Instance.GetLibrary(library_id);
                }

                return library;
            }
        }
#endif

        public PDFAutoTagNodeContent(string library_id, string tag)
        {
            this.library_id = library_id;
            this.tag = tag;
        }

        public bool MatchesKeyword(string keyword)
        {
            if ((null != tag) && tag.ToLower(CultureInfo.CurrentCulture).Contains(keyword)) return true;
            return false;
        }

#region  --- Binding properties ------------------------------------

        public string Tag => tag;

#endregion

        public override bool Equals(object obj)
        {
            PDFAutoTagNodeContent other = obj as PDFAutoTagNodeContent;
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
