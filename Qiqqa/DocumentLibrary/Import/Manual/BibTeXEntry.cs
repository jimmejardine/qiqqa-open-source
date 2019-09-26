using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Reflection;

namespace Qiqqa.DocumentLibrary.Import.Manual
{
    public class BibTeXEntry
    {
        public string Raw { get; set; }
        
        public string Filename { get; set; }

        public string FileType { get; set; }

        private BibTexItem _Parsed;
        public BibTexItem Parsed
        {
            get
            {
                if (null == _Parsed)
                {
                    _Parsed = BibTexParser.ParseOne(this.Raw, false);
                }
                return _Parsed;
            }
            set
            {
                if (!String.IsNullOrEmpty(Raw))
                {
                    throw new Exception("Internal failure: Cannot overwrite an entire RAW BibTeX record.");
                }
                _Parsed = value;
                Raw = value.ToBibTex();
            }
        }

        public string EntryType
        {
            get
            {
                return this.Parsed.Type;
            }
        }

        public string Id {
            get
            {
                return Parsed.Key;
            }
        }

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
            return this.Parsed[key];
        }

        /// <summary>
        /// Used via reflection
        /// </summary>
        public virtual string Title
        {
            get
            {
                return this.Parsed.GetTitle();
            }
        }

        /// <summary>
        /// Used via reflection
        /// </summary>
        public virtual string Author
        {
            get
            {
                return this.Parsed.GetAuthor();
            }
        }

        private void ExtractTagsFromBibTeXField(string tag, List<string> tags)
        {
            string vals = this.Parsed[tag];
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
                ExtractTagsFromBibTeXField("tag", tags);
                ExtractTagsFromBibTeXField("tags", tags);
                ExtractTagsFromBibTeXField("keyword", tags);
                ExtractTagsFromBibTeXField("keywords", tags);
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
