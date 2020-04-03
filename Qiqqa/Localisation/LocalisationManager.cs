using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.Files;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Localisation
{
    public class LocalisationManager
    {
        public static readonly string DEFAULT_LOCALE = "en";

        private static readonly Lazy<string> BASE_PATH = new Lazy<string>(() => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"Localisation")));
        private static readonly Lazy<string> TEMP_BASE_PATH = new Lazy<string>(() => Path.GetFullPath(Path.Combine(TempFile.TempDirectoryForQiqqa, @"Localisation")));

        private static string GetFilenameForLocale(string locale)
        {
            return Path.GetFullPath(Path.Combine(BASE_PATH.Value, string.Format("{0}.qiqqa.txt", locale)));
        }

        private static string GetFilenameForTempLocale(string locale)
        {
            return Path.GetFullPath(Path.Combine(TEMP_BASE_PATH.Value, string.Format("{0}.qiqqa.txt", locale)));
        }

        public static LocalisationManager Instance = new LocalisationManager();

        private Dictionary<string, LocaleTable> locale_tables = new Dictionary<string, LocaleTable>();

        public LocaleTable LoadTempLocaleTable(string locale)
        {
            // Temp locale
            try
            {
                LocaleTable locale_table = LocaleTable.Load(GetFilenameForTempLocale(locale));
                if (null != locale_table)
                {
                    return locale_table;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem loading temp locale.");
            }

            return null;
        }

        public void SaveTempLocaleTable(string locale, LocaleTable locale_table)
        {
            string filename = GetFilenameForTempLocale(locale);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            locale_table.Save(filename);
            Logging.Info("Saved locale {0}", locale);
        }

        public void BrowseTempLocaleTable(string locale)
        {
            string filename = GetFilenameForTempLocale(locale);
            FileTools.BrowseToFileInExplorer(filename);
        }

        public LocaleTable LoadLocaleTable(string locale)
        {
            // Temp locale
            {
                LocaleTable locale_table = LoadTempLocaleTable(locale);
                if (null != locale_table)
                {
                    Logging.Info("Loaded TEMP locale for {0} with {1} items.", locale_table, locale_table.Count);
                    return locale_table;
                }
            }

            // Real locale
            try
            {
                LocaleTable locale_table = LocaleTable.Load(GetFilenameForLocale(locale));
                if (null != locale_table)
                {
                    Logging.Info("Loaded REAL locale for {0} with {1} items.", locale_table, locale_table.Count);
                    return locale_table;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem loading locale.");
            }

            // Give up
            return null;
        }

        private List<string> last_locale_order = new List<string>();
        public List<string> GetLocaleOrder()
        {
            // Pick the override or current UI culture
            string locale = null;
            if (String.IsNullOrEmpty(locale)) locale = ConfigurationManager.Instance.ConfigurationRecord.Localisation_ForcedLocale;
            if (String.IsNullOrEmpty(locale)) locale = CultureInfo.CurrentUICulture.Name;
            locale = locale.ToLower();

            string locale_short = locale;
            if (locale_short.Length > 2)
            {
                locale_short = locale_short.Substring(0, 2);
            }

            string locale_en = DEFAULT_LOCALE;

            if (!last_locale_order.Contains(locale) || !last_locale_order.Contains(locale_short) || !last_locale_order.Contains(locale_en))
            {
                last_locale_order.Clear();
                if (!last_locale_order.Contains(locale)) last_locale_order.Add(locale);
                if (!last_locale_order.Contains(locale_short)) last_locale_order.Add(locale_short);
                if (!last_locale_order.Contains(locale_en)) last_locale_order.Add(locale_en);
            }

            return last_locale_order;
        }

        private LocaleTable GetLocaleTable(string locale)
        {
            if (!locale_tables.ContainsKey(locale))
            {
                locale_tables[locale] = LoadLocaleTable(locale);
            }

            return locale_tables[locale];
        }

        public string GetString(string key)
        {
            if (key.Contains("/TIP/"))
            {
                return GetString_TIP(key);
            }
            else
            {
                return GetString_NORMAL(key);
            }
        }

        public string GetString_NORMAL(string key)
        {
            foreach (string locale in GetLocaleOrder())
            {
                LocaleTable locale_table = GetLocaleTable(locale);
                if (null != locale_table)
                {
                    string content = null;
                    if (locale_table.TryGetValue(key, out content))
                    {
                        return content;
                    }
                }
            }

            return null;
        }

        public string GetString_TIP(string key)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string locale in GetLocaleOrder())
            {
                LocaleTable locale_table = GetLocaleTable(locale);
                if (null != locale_table)
                {
                    string content = null;
                    if (locale_table.TryGetValue(key, out content))
                    {
                        sb.AppendLine(content);
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString().TrimEnd();
        }

        // ------------------------------------------------------------------------------------------------------------

        public static string Get(string key)
        {
            return Instance.GetString(key);
        }
    }
}
