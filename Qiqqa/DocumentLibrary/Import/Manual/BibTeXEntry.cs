using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Reflection;

namespace Qiqqa.DocumentLibrary.Import.Manual
{
    [Obfuscation(Feature = "properties renaming")]
    public class BibTeXEntry
    {
        public string BibTeX { get; set; }
        
        public string EntryType { get; set; }
        
        public string Filename { get; set; }
        public string FileType { get; set; }


        public BibTexItem Item { get; set; }

        public string Id { get { return Item.Key; } }

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
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                this.Bindable.NotifyPropertyChanged(() => this.Selected);
            }
        }
        
        public BibTeXEntry()
        {
        }

        AugmentedBindable<BibTeXEntry> _bindable = null;
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
        public virtual string Title
        {
            get
            {
                return BibTexTools.GetTitle_SLOOOOOOW(this.BibTeX);
            }
        }

        /// <summary>
        /// Used via reflection
        /// </summary>
        public virtual string Author
        {
            get
            {
                return BibTexTools.GetAuthor_SLOOOOOOW(this.BibTeX);
            }
        }

        private static void ExtractTagsFromBibTeXField(string bibtex, string TAG, List<string> tags)
        {
            string vals = BibTexTools.GetField(bibtex, TAG);
            if (!String.IsNullOrEmpty(vals))
            {
                string[] ret = vals.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                tags.AddRange(ret.Select(x => x.Trim()));
            }
        }

        public virtual List<string> Tags
        {
            get
            {
                List<string> tags = new List<string>();
                ExtractTagsFromBibTeXField(this.BibTeX, "tag", tags);
                ExtractTagsFromBibTeXField(this.BibTeX, "tags", tags);
                ExtractTagsFromBibTeXField(this.BibTeX, "keyword", tags);
                ExtractTagsFromBibTeXField(this.BibTeX, "keywords", tags);
                return tags;
            }
        }

        public bool IsVanilla { get; set; }
        
        /// <summary>
        /// Used via reflection
        /// </summary>
        public bool HasPDF
        {
            get
            {
                return !IsVanilla;
            }
        }

        public virtual string Notes { get; set; } 
    }
}
