using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using icons;
using Utilities.GUI;

namespace Qiqqa.Localisation
{
    /// <summary>
    /// Interaction logic for LocalisationEditingControl.xaml
    /// </summary>
    public partial class LocalisationEditingControl : UserControl
    {
        public class LocalisationRow
        {
            public string Id { get; set; }
            public string Default { get; set; }
            public string Translation { get; set; }
        }

        private string current_locale = null;

        public LocalisationEditingControl()
        {
            Theme.Initialize();

            InitializeComponent();

            TxtNewLocale.EmptyTextPrompt = "Type a new locale to edit here and press <ENTER>";
            TxtWorkingLocale.Text = "Please select a locale...";

            TxtNewLocale.OnHardSearch += TxtNewLocale_OnHardSearch;
            GridEditor.SelectedCellsChanged += GridEditor_SelectedCellsChanged;
            CmdSendToQiqqa.Click += CmdSendToQiqqa_Click;
            CmdSendToQiqqa.Icon = Icons.GetAppIcon(Icons.ModuleLocalisation);
            CmdSendToQiqqa.CaptionDock = Dock.Right;

            TxtCurrentLocale.Inlines.Clear();
            TxtCurrentLocale.Inlines.Add(CultureInfo.CurrentUICulture.Name);
        }

        private void CmdSendToQiqqa_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(current_locale))
            {
                MessageBoxes.Error("You have to pick a locale first.");
                return;
            }

            DoFlush(true);
            MessageBoxes.Info("Please remember to ZIP the language file before sending it to Qiqqa.");
            LocalisationManager.Instance.BrowseTempLocaleTable(current_locale);
        }

        private void GridEditor_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DoFlush(false);
        }

        private DateTime last_flush_time = DateTime.MinValue;
        private void DoFlush(bool force)
        {
            if (!force && DateTime.UtcNow.Subtract(last_flush_time).TotalSeconds < 3)
            {
                return;
            }

            last_flush_time = DateTime.UtcNow;

            List<LocalisationRow> rows = (List<LocalisationRow>)GridEditor.ItemsSource;
            LocaleTable locale_table = new LocaleTable();
            foreach (LocalisationRow row in rows)
            {
                if (!String.IsNullOrEmpty(row.Translation))
                {
                    locale_table[row.Id] = row.Translation.Replace(Environment.NewLine, "\\n");
                }
            }

            // Write to disk...
            LocalisationManager.Instance.SaveTempLocaleTable(current_locale, locale_table);
        }

        private void TxtNewLocale_OnHardSearch()
        {
            if (!String.IsNullOrEmpty(TxtNewLocale.Text))
            {
                current_locale = TxtNewLocale.Text;

                TxtNewLocale.Clear();
                TxtWorkingLocale.Text = "You are working on locale: " + current_locale;
            };

            LoadLocale(current_locale);
        }

        public void LoadLocale(string locale)
        {
            LocaleTable locale_table_en = LocalisationManager.Instance.LoadLocaleTable(LocalisationManager.DEFAULT_LOCALE);

            LocaleTable locale_table = LocalisationManager.Instance.LoadLocaleTable(locale);
            if (null == locale_table)
            {
                locale_table = new LocaleTable();
            }

            List<LocalisationRow> rows = new List<LocalisationRow>();
            foreach (var pair in locale_table_en)
            {
                LocalisationRow row = new LocalisationRow();
                row.Id = pair.Key;
                row.Default = pair.Value;
                if (locale_table.ContainsKey(pair.Key))
                {
                    row.Translation = locale_table[pair.Key];
                }

                rows.Add(row);
            }

            GridEditor.ItemsSource = rows;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()        
        {
            LocalisationEditingControl lec = new LocalisationEditingControl();
            ControlHostingWindow chw = new ControlHostingWindow("Localisation", lec);
            chw.Show();
        }
#endif

        #endregion
    }
}
