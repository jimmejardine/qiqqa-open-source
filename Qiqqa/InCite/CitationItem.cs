using System;
using System.Collections.Generic;

namespace Qiqqa.InCite
{
    public class CitationItem
    {
        public static readonly string PARAM_SEPARATE_AUTHOR_DATE = "SEPARATE_AUTHOR_DATE";
        public static readonly string OPTION_SEPARATE_AUTHOR_DATE_TRUE = "TRUE";

        public static readonly string PARAM_SPECIFIER_TYPE = "PARAM_SPECIFIER_TYPE";
        public static readonly string[] OPTIONS_SPECIFIER_TYPE = new string[] 
        { 
            "", "page", "section", "chapter",
            "", 
            "book",
            "chapter",
            "column",
            "figure",
            "folio",
            "issue",
            "line",
            "note",
            "opus",
            "page",
            "paragraph",
            "part",
            "section",
            "sub verbo",
            "verse",
            "volume"
        };
        public static readonly string PARAM_SPECIFIER_LOCATION = "PARAM_SPECIFIER_LOCATION";
        public static readonly string PARAM_PREFIX = "PARAM_PREFIX";
        public static readonly string PARAM_SUFFIX = "PARAM_SUFFIX";

        public string reference_key;
        public string reference_library_hint;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();

        public CitationItem(string reference_key, string reference_library_hint)
            : this(reference_key, reference_library_hint, new Dictionary<string, string>())
        {
        }

        public CitationItem(string reference_key, string reference_library_hint, Dictionary<string, string> parameters)
        {
            this.reference_key = reference_key;
            this.reference_library_hint = reference_library_hint;
            this.parameters = parameters;
        }

        public string GetParameter(string key)
        {
            string value;
            if (parameters.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return "";
            }
        }

        public void SetParameter(string key, string value)
        {
            if (String.IsNullOrEmpty(key))
            {
                parameters.Remove(key);
            }
            else
            {
                parameters[key] = value;
            }
        }

        public void SeparateAuthorsAndDate(bool separate_author_and_date)
        {
            if (separate_author_and_date)
            {
                this.SetParameter(PARAM_SEPARATE_AUTHOR_DATE, OPTION_SEPARATE_AUTHOR_DATE_TRUE);
            }
            else
            {
                this.SetParameter(PARAM_SEPARATE_AUTHOR_DATE, "");
            }
        }
    }
}
