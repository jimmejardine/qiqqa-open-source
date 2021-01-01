using System;
using System.Globalization;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class PDFAuthorNodeContent : ISearchable, IRecurrentNodeContent
    {
        private string library_id;
        private string surname;
        private string initial;

#if false
        [NonSerialized]
        private WebLibraryDetail web_library_detail = null;
        public WebLibraryDetail LibraryRef
        {
            get
            {
                if (null == web_library_detail)
                {
                    web_library_detail = WebLibraryManager.Instance.GetLibrary(library_id);
                }

                return web_library_detail;
            }
        }
#endif

        public PDFAuthorNodeContent(string library_id, string surname, string initial)
        {
            this.library_id = library_id;
            this.surname = surname;
            this.initial = initial;
        }

        public bool MatchesKeyword(string keyword)
        {
            if ((null != surname) && surname.ToLower().Contains(keyword)) return true;
            if ((null != initial) && initial.ToLower().Contains(keyword)) return true;
            return false;
        }

        #region  --- Binding properties ------------------------------------

        public string Surname => surname;

        public string Initial => initial;

        public string SurnameAndInitial
        {
            get
            {
                if (String.IsNullOrEmpty(initial))
                {
                    return surname;
                }
                else
                {
                    return String.Format("{0}, {1}", surname, initial);
                }
            }
        }


        #endregion

        public override bool Equals(object obj)
        {
            PDFAuthorNodeContent other = obj as PDFAuthorNodeContent;
            if (null == other) return false;

            if (library_id != other.library_id) return false;
            if (surname != other.surname) return false;
            if (initial != other.initial) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + library_id.GetHashCode();
            hash = hash * 37 + surname.GetHashCode();
            if (null != initial) hash = hash * 37 + initial.GetHashCode();
            return hash;
        }
    }
}
