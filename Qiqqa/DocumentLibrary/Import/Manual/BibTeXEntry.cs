using System;
using System.Collections.Generic;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Reflection;

namespace Qiqqa.DocumentLibrary.Import.Manual
{
    public class BibTeXEntry
    {
        public string BibTeX { get; set; }

        public string EntryType { get; set; }

        public string Filename { get; set; }

        public string FileType { get; set; }

        public BibTexItem Item { get; set; }

        public string Id => Item.Key;

        /// <summary>
        /// Our Qiqqa Fingerprint. 
        /// </summary>
        public string Fingerprint { get; set; }

        public bool ExistsInLibrary { get; set; }

        private bool _selected;
        /// <summary>
        /// Selected for import. 
        /// </summary>
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                Bindable.NotifyPropertyChanged(() => Selected);
            }
        }

        public BibTeXEntry()
        {
        }

        private AugmentedBindable<BibTeXEntry> _bindable = null;
        public AugmentedBindable<BibTeXEntry> Bindable
        {
            get
            {
                if (null == _bindable)
                {
                    _bindable = new AugmentedBindable<BibTeXEntry>(this);
                }

                return _bindable;
            }
        }

        private string GetValue(string key)
        {
            return Item[key];
        }

        /// <summary>
        /// Used via reflection
        /// </summary>
        public virtual string Title => BibTexTools.GetTitle_SLOOOOOOW(BibTeX);

        /// <summary>
        /// Used via reflection
        /// </summary>
        public virtual string Author => BibTexTools.GetAuthor_SLOOOOOOW(BibTeX);

        private static void ExtractTagsFromBibTeXField(string bibtex, string TAG, ref HashSet<string> tags)
        {
            string vals = BibTexTools.GetField(bibtex, TAG);
            if (!String.IsNullOrEmpty(vals))
            {
                string[] ret = vals.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string tag in ret)
                {
                    tags.Add(tag.Trim());
                }
            }
        }

        public virtual HashSet<string> Tags
        {
            get
            {
                HashSet<string> tags = new HashSet<string>();
                ExtractTagsFromBibTeXField(BibTeX, "tag", ref tags);
                ExtractTagsFromBibTeXField(BibTeX, "tags", ref tags);
                ExtractTagsFromBibTeXField(BibTeX, "keyword", ref tags);
                ExtractTagsFromBibTeXField(BibTeX, "keywords", ref tags);
                return tags;
            }
        }

        public bool IsVanilla { get; set; }

        /// <summary>
        /// Used via reflection
        /// </summary>
        public bool HasPDF => !IsVanilla;

        public virtual string Notes { get; set; }
    }
}
