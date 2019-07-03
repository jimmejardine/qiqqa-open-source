using System;
using System.Reflection;
using System.Windows.Media;
using Utilities.Misc;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF
{
    /// <summary>
    /// ******************* NB NB NB NB NB NB NB NB NB NB NB ********************************
    /// 
    /// ALL PROPERTIES STORED IN THE DICTIONARY MUST BE SIMPLE TYPES - string, int or double.  
    /// NO DATES, NO COLORS, NO STRUCTs.  
    /// If you want to store Color and DateTime, then there are helper methods on the DictionaryBasedObject to convert TO/FROM.  Use those!
    /// Otherwise platform independent serialisation will break!
    /// 
    /// ******************* NB NB NB NB NB NB NB NB NB NB NB ********************************
    /// </summary>

    [Serializable]
    [Obfuscation(Feature = "properties renaming")]
    public class PDFAnnotation : ICloneable
    {
        readonly DictionaryBasedObject dictionary = new DictionaryBasedObject();

        public PDFAnnotation(string document_fingerprint, int page /* 1 based */, Color color, string creator)
            : this(new DictionaryBasedObject(), true)
        {
            this.DocumentFingerprint = document_fingerprint;
            this.Page = page;
            this.DateCreated = DateTime.UtcNow;
            this.Deleted = false;
            this.Creator = creator;

            this.Color = color;
        }

        /// <summary>
        /// If new (e.g. cloning), generate a new guid.
        /// </summary>
        public PDFAnnotation(DictionaryBasedObject dictionary, bool is_new)
        {
            this.dictionary = dictionary;
            if (is_new)
            {
                Guid = System.Guid.NewGuid();
            }
        }

        internal DictionaryBasedObject Dictionary
        {
            get { return dictionary; }
        }

        public override string ToString()
        {
            return String.Format("Annotation {0} for document {1} on page {2}", Guid, DocumentFingerprint, Page);
        }

        #region --- Protected  properties -------------------------------------------------------------

        public string DocumentFingerprint
        {
            get { return dictionary["DocumentFingerprint"] as string; }
            protected set { dictionary["DocumentFingerprint"] = value as string; }
        }

        public Guid? Guid
        {
            get { return dictionary.GetNullableGuid("Guid"); }
            protected set { dictionary["Guid"] = value as Guid?; }
        }

        public bool Deleted
        {
            get { return (bool)(dictionary["Deleted"] ?? false); }
            set { dictionary["Deleted"] = value; }
        }

        /// <summary>
        /// Indicates whether this annotation has been imported from a legacy PDF annotation
        /// </summary>
        public bool Legacy
        {
            get { return (bool)(dictionary["Legacy"] ?? false); }
            set { dictionary["Legacy"] = value; }
        }

        public int Page /* 1 based */
        {
            get { return Convert.ToInt32(dictionary["Page"] ?? 0); }
            protected set { dictionary["Page"] = value as int?; }
        }

        public DateTime? DateCreated
        {
            get { return dictionary.GetDateTime("DateCreated"); }
            set { dictionary.SetDateTime("DateCreated", value); }
        }

        public string Creator
        {
            get { return dictionary["Creator"] as string; }
            set { dictionary["Creator"] = value as string; }
        }        

        #endregion

        #region --- Public properties -------------------------------------------------------------

        public double Left
        {
            get { return dictionary.GetDouble("Left"); }            
            set { dictionary["Left"] = value as double?; }
        }
        public double Top
        {
            get { return dictionary.GetDouble("Top"); }
            set { dictionary["Top"] = value as double?; }
        }
        public double Width
        {
            get { return dictionary.GetDouble("Width"); }
            set { dictionary["Width"] = value as double?; }
        }
        public double Height
        {
            get { return dictionary.GetDouble("Height"); }
            set { dictionary["Height"] = value as double?; }
        }

        public Color Color
        {
            get { return dictionary.GetColor("ColorWrapper"); }
            set { dictionary.SetColor("ColorWrapper", value); }
        }

        public string Text
        {
            get { return dictionary["Text"] as string; }
            set { dictionary["Text"] = value as string; }
        }

        public string Tags
        {
            get { return dictionary["Tags"] as string; }
            set { dictionary["Tags"] = value as string; }
        }

        public string Rating
        {
            get { return dictionary["Rating"] as string; }
            set { dictionary["Rating"] = value as string; }
        }
        
        public DateTime? FollowUpDate
        {
            get { return dictionary.GetDateTime("FollowUpDate"); }
            set { dictionary.SetDateTime("FollowUpDate", value); }
        }

        public bool AnnotationReportSuppressImage
        {
            get { return (bool)(dictionary["AnnotationReportSuppressImage"] ?? false); }
            set { dictionary["AnnotationReportSuppressImage"] = value as bool?; }
        }

        public bool AnnotationReportSuppressText
        {
            get { return (bool)(dictionary["AnnotationReportSuppressText"] ?? false); }
            set { dictionary["AnnotationReportSuppressText"] = value as bool?; }
        }

        public bool AnnotationTextAlwaysVisible
        {
            get { return (bool)(dictionary["AnnotationTextAlwaysVisible"] ?? false); }
            set { dictionary["AnnotationTextAlwaysVisible"] = value; }
        }

        #endregion --------------------------------------------------------------------------------

        [NonSerialized]
        AugmentedBindable<PDFAnnotation> bindable = null;
        public AugmentedBindable<PDFAnnotation> Bindable
        {
            get
            {
                if (null == bindable)
                {
                    bindable = new AugmentedBindable<PDFAnnotation>(this);
                }

                return bindable;
            }
        }

        /// <summary>
        /// Deep clone, clears Bindable.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new PDFAnnotation((DictionaryBasedObject) dictionary.Clone(), true);
        }
    }
}
