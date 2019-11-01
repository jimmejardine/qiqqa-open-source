using System;
using System.Globalization;
using Qiqqa.Brainstorm.Common;
using Qiqqa.Brainstorm.Common.Searching;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class PDFAuthorNodeContent : ISearchable, IRecurrentNodeContent
    {
        private string library_id;
        private string surname;
        private string initial;


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


        public PDFAuthorNodeContent(string library_id, string surname, string initial)
        {
            this.library_id = library_id;
            this.surname = surname;
            this.initial = initial;
        }

        public bool MatchesKeyword(string keyword)
        {
            if ((null != surname) && surname.ToLower(CultureInfo.CurrentCulture).Contains(keyword)) return true;
            if ((null != initial) && initial.ToLower(CultureInfo.CurrentCulture).Contains(keyword)) return true;
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
