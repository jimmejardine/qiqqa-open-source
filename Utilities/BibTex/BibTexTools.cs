using System;
using System.Text;
using Utilities.BibTex.Parsing;
using Utilities.Random;

namespace Utilities.BibTex
{
    public class BibTexTools
    {
        // ----------------------------------------------------------------------------------------

        public static string GenerateRandomBibTeXKey(string seed = null)
        {
            if (String.IsNullOrEmpty(seed))
            {
                seed = "QIQQA";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(seed + "-");
            for (int i = 0; i < 5; ++i)
            {                
                sb.Append(Convert.ToChar((byte)(65 + RandomAugmented.Instance.NextIntExclusive(26))));
            }
            return sb.ToString();
        }

        public static string GetEmptyArticleBibTeXTemplate()
        {
            string key = GenerateRandomBibTeXKey();

            return
                "@article{" + GenerateRandomBibTeXKey() + "," +
                " author = {}," +
                " title = {}," +
                " year = {}," +
                " publisher = {}" +
                "}"
                ;
        }

        
        // ----------------------------------------------------------------------------------------

        public static string GetTitle_SLOOOOOOW(string bibtex)
        {
            return GetField(bibtex, "title");
        }

        public static string GetTitle(BibTexItem bibtex_item)
        {
            return GetField(bibtex_item, "title");
        }

        public static string SetTitle(string bibtex, string title)
        {
            return SetField(bibtex, "title", title);
        }

        public static string GetAuthor_SLOOOOOOW(string bibtex)
        {
            return GetField(bibtex, "author");
        }

        public static bool HasTitle(BibTexItem bibtex_item)
        {
            return HasField(bibtex_item, "title");
        }
        
        public static bool HasAuthor(BibTexItem bibtex_item)
        {
            return HasField(bibtex_item, "author");
        }

        public static bool HasField(BibTexItem bibtex_item, string field)
        {
            if (null == bibtex_item) return false;
            return bibtex_item.ContainsField(field);
        }

        public static string GetAuthor(BibTexItem bibtex_item)
        {
            return GetField(bibtex_item, "author");
        }

        public static string SetAuthor(string bibtex, string author)
        {
            return SetField(bibtex, "author", author);
        }

        //public static string GetYear(string bibtex)
        //{
        //    return GetField(bibtex, "year");
        //}

        public static string GetYear(BibTexItem bibtex_item)
        {
            return GetField(bibtex_item, "year");
        }

        public static string SetYear(string bibtex, string year)
        {
            return SetField(bibtex, "year", year);
        }

        public static string GetGenericPublication(BibTexItem bibtex_item)
        {
            string generic_publication = null;

            generic_publication = GetField(bibtex_item, "journal");
            if (!String.IsNullOrEmpty(generic_publication)) return generic_publication;

            generic_publication = GetField(bibtex_item, "booktitle");
            if (!String.IsNullOrEmpty(generic_publication)) return generic_publication;

            generic_publication = GetField(bibtex_item, "container-title");
            if (!String.IsNullOrEmpty(generic_publication)) return generic_publication;

            generic_publication = GetField(bibtex_item, "publisher");
            if (!String.IsNullOrEmpty(generic_publication)) return generic_publication;

            return null;
        }

        public static void SetGenericPublication(BibTexItem bibtex_item, string generic_publication)
        {
            if (null == bibtex_item) return;

            bool set_a_field = false;

            if (bibtex_item.ContainsField("journal"))
            {
                set_a_field = true;
                bibtex_item["journal"] = generic_publication;
            }

            if (bibtex_item.ContainsField("booktitle"))
            {
                set_a_field = true;
                bibtex_item["booktitle"] = generic_publication;
            }

            if (bibtex_item.ContainsField("container-title"))
            {
                set_a_field = true;
                bibtex_item["container-title"] = generic_publication;
            }

            if (bibtex_item.ContainsField("publisher"))
            {
                set_a_field = true;
                bibtex_item["publisher"] = generic_publication;
            }

            // If no field was ever set, insert a new field.
            // NB: This could get smarter in that we don't always want to insert journal, depending on bibtex type
            if (!set_a_field)
            {
                bibtex_item["journal"] = generic_publication;
            }
        }

        // ----------------------------------------------------------------------------------------

        public static string GetField(BibTexItem bibtex_item, string field)
        {
            if (null != bibtex_item)
            {
                return bibtex_item[field];
            }
            else
            {
                return "";
            }
        }
            
        public static string GetField(string bibtex, string field)
        {
            try
            {
                BibTexItem bibtex_item = BibTexParser.ParseOne(bibtex, false);
                return GetField(bibtex_item, field);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem extracting from the BibTeX");
                return "";
            }
        }

        /// <summary>
        /// After setting the field, returns the WHOLE bibtex again
        /// </summary>
        /// <param name="bibtex"></param>
        /// <param name="field"></param>
        /// <param name="field_value"></param>
        /// <returns></returns>
        public static string SetField(string bibtex, string field, string field_value)
        {
            try
            {
                BibTexItem item = BibTexParser.ParseOne(bibtex, false);
                if (null != item)
                {
                    item[field] = field_value;
                    return item.ToBibTex();
                }
                else
                {
                    return bibtex;
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem setting a field in BibTeX:\n  field: {0}\n  value: {1}\n  BibTeX:\n  {2}", field, field_value, bibtex);
                return null;
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            string sample_bibtext = @"@conference{kamp1984theory,title =       {{A theory of truth and semantic representation}},author =       {Kamp, H.},booktitle={Truth, Interpretation and Information: Selected Papers from the Third Amsterdam Colloquium},pages={1--41},year={1984}";
            
            Logging.Info("BibTex is:\n" + sample_bibtext);

            BibTexItem bibtex_item = BibTexParser.ParseOne(sample_bibtext, false);

            Logging.Info("Title is: " + GetTitle(bibtex_item));
            Logging.Info("Author is: " + GetAuthor(bibtex_item));
            Logging.Info("Year is: " + GetYear(bibtex_item));

            string replaced_bibtex = sample_bibtext;
            replaced_bibtex = SetTitle(replaced_bibtex, "New title");
            replaced_bibtex = SetAuthor(replaced_bibtex, "New author");
            replaced_bibtex = SetYear(replaced_bibtex, "New year");

            Logging.Info("Replaced BibTex is:\n" + replaced_bibtex);
        }
#endif

        #endregion
    }
}
