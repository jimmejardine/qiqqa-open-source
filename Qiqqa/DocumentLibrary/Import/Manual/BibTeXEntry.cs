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
        public string Filename { get; set; }

        public string FileType { get; set; }

        public BibTexItem BibTexRecord { get; set; }

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

        public string Notes { get; set; } 
    }
}
