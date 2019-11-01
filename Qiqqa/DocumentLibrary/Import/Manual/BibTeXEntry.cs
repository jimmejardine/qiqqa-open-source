using System;
using System.Collections.Generic;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Reflection;

namespace Qiqqa.DocumentLibrary.Import.Manual
{
    public class BibTeXEntry
    {
        public string Filename { get; set; }

        public string FileType { get; set; }

        public string FileURI { get; set; }

        public string OriginalFilename { get; set; }

        public string SuggestedDownloadSourceURI { get; set; }

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

        // HasPDF = !IsVanilla
        public bool IsVanilla { get; set; }

        public string Notes { get; set; } 
    }
}
