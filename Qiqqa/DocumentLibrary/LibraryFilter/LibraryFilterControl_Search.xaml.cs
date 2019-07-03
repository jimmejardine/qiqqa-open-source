using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.DocumentLibraryIndex;
using Qiqqa.UtilisationTracking;
using Utilities.GUI.Wizard;
using Utilities.Language.TextIndexing;

namespace Qiqqa.DocumentLibrary.LibraryFilter
{
    /// <summary>
    /// Interaction logic for LibraryFilterControl_Search.xaml
    /// </summary>
    public partial class LibraryFilterControl_Search : UserControl
    {
        internal LibraryFilterControl library_filter_control = null;

        public LibraryFilterControl_Search()
        {
            InitializeComponent();

            WizardDPs.SetPointOfInterest(SearchQuick, "LibrarySearchQuickTextBox");
            WizardDPs.SetPointOfInterest(SearchQuickHelpButton, "LibrarySearchQuickHelpButton");

            SearchQuick.SearchHistoryItemSource = ConfigurationManager.Instance.SearchHistory;
            SearchQuick.OnHardSearch += SearchQuick_OnHardSearch;

            SearchQuick.GotKeyboardFocus += SearchQuick_GotKeyboardFocus;
        }

        void SearchQuick_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Start the Wizard if necessary
            if (!ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenSearchWizard)
            {
                ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenSearchWizard = true;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenSearchWizard);
                MainWindowServiceDispatcher.Instance.OpenSearchWizard();
            }
        }

        void SearchQuick_OnHardSearch()
        {
            library_filter_control.ObjLibraryFilterControl_Sort.SetSortToSearchScore();
            
            ExecuteSearchQuick();            
            library_filter_control.ReviewParameters();
        }

        internal void ExecuteSearchQuick()
        {
            string query = SearchQuick.Text;

            if (!String.IsNullOrEmpty(query))
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Library_KeywordFilter);

                List<IndexResult> index_results = LibrarySearcher.FindAllFingerprintsMatchingQuery(library_filter_control.library, query);

                library_filter_control.search_quick_query = query;
                library_filter_control.search_quick_scores = new Dictionary<string, double>();
                library_filter_control.search_quick_fingerprints = new HashSet<string>();
                foreach (var index_result in index_results)
                {
                    library_filter_control.search_quick_fingerprints.Add(index_result.fingerprint);
                    library_filter_control.search_quick_scores[index_result.fingerprint] = index_result.score;
                }
            }
            else
            {
                library_filter_control.search_quick_query = null;
                library_filter_control.search_quick_fingerprints = null;
                library_filter_control.search_quick_scores = null;
            }

            // Create the feedback
            library_filter_control.search_quick_fingerprints_span = new Span();
            Bold bold = new Bold();
            bold.Inlines.Add("Search");
            library_filter_control.search_quick_fingerprints_span.Inlines.Add(bold);
            library_filter_control.search_quick_fingerprints_span.Inlines.Add(" (click search score for details)");
            library_filter_control.search_quick_fingerprints_span.Inlines.Add(": ");
            library_filter_control.search_quick_fingerprints_span.Inlines.Add("'");
            library_filter_control.search_quick_fingerprints_span.Inlines.Add(query);
            library_filter_control.search_quick_fingerprints_span.Inlines.Add("'");
            library_filter_control.search_quick_fingerprints_span.Inlines.Add(LibraryFilterHelpers.GetClearImageInline("Clear this filter.", hyperlink_search_quick_fingerprints_span_OnClick));
        }

        void hyperlink_search_quick_fingerprints_span_OnClick(object sender, MouseButtonEventArgs e)
        {
            SearchQuick.Clear();
        }

        public void ResetFilters()
        {
            SearchQuick.Clear();
        }
    }
}
