using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.LibraryFilter;
using Qiqqa.DocumentLibrary.LibraryFilter.GenericLibraryExplorerStuff;
using Qiqqa.UtilisationTracking;
using Syncfusion.Windows.Chart;
using Utilities.Collections;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Strings;

namespace Qiqqa.DocumentLibrary.TagExplorerStuff
{
    /// <summary>
    /// Interaction logic for TagExplorerControl.xaml
    /// </summary>
    public partial class GenericLibraryExplorerControl : UserControl
    {
        public static readonly string YOU_CAN_FILTER_TOOLTIP = "\n\nYou can filter documents that contain them by clicking on one or more of the checkboxes to the left.\nIf you have selected 'AND' above, then a document must have all the items to be shown.\nIf you have selected 'OR', then a document must have any of the items to be shown.\nIf 'NOT' is selected, then the inverse of the filtered documents are shown.";

        private string description_title = "";
        private Library library = null;

        public delegate void OnTagSelectionChangedDelegate(HashSet<string> fingerprints, Span descriptive_span);
        public event OnTagSelectionChangedDelegate OnTagSelectionChanged;

        public delegate MultiMapSet<string, string> GetNodeItemsDelegate(Library library, HashSet<string> parent_fingerprints);
        public delegate void OnItemPopupDelegate(Library library, string item_tag);
        public delegate void OnItemDragOverDelegate(Library library, string item_tag, DragEventArgs e);
        public delegate void OnItemDropDelegate(Library library, string item_tag, DragEventArgs e);

        public GetNodeItemsDelegate GetNodeItems;
        public OnItemPopupDelegate OnItemPopup;
        public OnItemDragOverDelegate OnItemDragOver;
        public OnItemDropDelegate OnItemDrop;
        private List<string> selected_tags = new List<string>();

        public GenericLibraryExplorerControl()
        {
            Theme.Initialize();

            InitializeComponent();

            TreeSearchTerms.Background = ThemeColours.Background_Brush_Blue_LightToDark;

            ObjImageRefresh.Source = Icons.GetAppIcon(Icons.Refresh);
            RenderOptions.SetBitmapScalingMode(ObjImageRefresh, BitmapScalingMode.HighQuality);
            ObjImageRefresh.ToolTip = "Refresh this list to reflect your latest documents and annotations.";
            ObjImageRefresh.Cursor = Cursors.Hand;
            ObjImageRefresh.MouseUp += ObjImageRefresh_MouseUp;

            ObjBooleanAnd.ToolTip = "Choose this to display the documents that contain ALL the tags you have selected (more and more exclusive).\nYou can select/deselect multiple tags by toggling the checkbox or holding down SHIFT or CTRL while you click additional tags.";
            ObjBooleanOr.ToolTip = "Choose this to display the documents that contain ANY of the tags you have selected (more and more inclusive).\nYou can select/deselect multiple tags by toggling the checkbox or holding down SHIFT or CTRL while you click additional tags.";
            ObjBooleanNot.ToolTip = "Tick this to show instead the documents that do not meet the AND or OR criteria.  This is the logical NOT, which allows you to express selections such as:\n   show me documents that have neither X nor Y (using NOT and OR); or\n   show me documents that don't have both X and Y (using NOT and AND)";

            ObjBooleanAnd.Click += ObjBoolean_Click;
            ObjBooleanOr.Click += ObjBoolean_Click;
            ObjBooleanNot.Click += ObjBoolean_Click;

            ObjSort.Click += ObjSort_Click;

            CmdExport.Caption = "Export";
            CmdExport.CenteredMode = true;
            CmdExport.MinWidth = 0;
            CmdExport.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            CmdExport.Click += CmdExport_Click;

            TxtSearchTermsFilter.Visibility = ConfigurationManager.Instance.NoviceVisibility;

            ObjBooleanAnd.IsChecked = true;
            TreeSearchTerms.SelectionMode = SelectionMode.Single;

            TxtSearchTermsFilter.OnHardSearch += TxtSearchTermsFilter_OnSoftSearch;

            TreeSearchTerms.KeyUp += TreeSearchTerms_KeyUp;

            AllowDrop = true;

            ObjSeries.MouseClick += ObjSeries_MouseClick;

            // Start with the chart collapsed
            Loaded += GenericLibraryExplorerControl_Loaded;
        }

        private void CmdExport_Click(object sender, RoutedEventArgs e)
        {
            DateTime start_time = DateTime.Now;

            MultiMapSet<string, string> tags_with_fingerprints_ALL = GetNodeItems(library, null);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendFormat("{0} report\r\n", description_title);
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine("Generated by Qiqqa (http://www.qiqqa.com)");
            sb.AppendLine(String.Format("On {0} {1}", start_time.ToLongDateString(), start_time.ToLongTimeString()));
            sb.AppendLine("------------------------------------------------------------------------");

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendFormat("{0}:\r\n", description_title);
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine();

            foreach (var pair in tags_with_fingerprints_ALL)
            {
                sb.AppendLine(pair.Key);
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendFormat("{0} with associated fingerprints:\r\n", description_title);
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine();
            foreach (var pair in tags_with_fingerprints_ALL)
            {
                foreach (var value in pair.Value)
                {
                    sb.AppendFormat("{0}\t{1}\r\n", pair.Key, value);
                }
            }

            string filename = TempFile.GenerateTempFilename("txt");
            File.WriteAllText(filename, sb.ToString());
            Process.Start(filename);
        }

        private void ObjImageRefresh_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PopulateItems();
        }

        private void GenericLibraryExplorerControl_Loaded(object sender, RoutedEventArgs e)
        {
            ObjChartRegion.Collapse();
        }

        private void ObjSort_Click(object sender, RoutedEventArgs e)
        {
            PopulateItems();
        }

        private void ObjBoolean_Click(object sender, RoutedEventArgs e)
        {
            selected_tags.Clear();
            PopulateItems();
        }

        private void TreeSearchTerms_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                List<string> items = new List<string>();
                foreach (GenericLibraryExplorerItem item in TreeSearchTerms.SelectedItems)
                {
                    items.Add(item.tag);
                }

                ToggleSelectItems(items, KeyboardTools.IsCTRLDown() || KeyboardTools.IsShiftDown());
            }
        }

        public void ToggleSelectItem(string item, bool inclusive_selection)
        {
            ToggleSelectItems(new List<string>(new string[] { item }), inclusive_selection);
        }

        public void ToggleSelectItems(List<string> items, bool inclusive_selection)
        {
            foreach (var item in items)
            {
                if (inclusive_selection)
                {
                    if (selected_tags.Contains(item))
                    {
                        selected_tags.Remove(item);
                    }
                    else
                    {
                        selected_tags.Add(item);
                    }
                }
                else
                {
                    if (selected_tags.Contains(item))
                    {
                        selected_tags.Clear();
                    }
                    else
                    {
                        selected_tags.Clear();
                        selected_tags.Add(item);
                    }
                }
            }

            PopulateItems();
        }

        private void hyperlink_clear_all_OnClick(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        public string DescriptionTitle
        {
            get => description_title;
            set => description_title = value;
        }

        public Library Library
        {
            get => library;
            set
            {
                library = value;
                Reset();
            }
        }

        private void TxtSearchTermsFilter_OnSoftSearch()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_GenericExplorer_Filter);
            PopulateItems();
        }

        internal void Reset()
        {
            selected_tags.Clear();
            PopulateItems();
        }

        private void PopulateItems()
        {
            bool exclusive = ObjBooleanAnd.IsChecked ?? true;
            bool is_negated = ObjBooleanNot.IsChecked ?? false;

            MultiMapSet<string, string> tags_with_fingerprints_ALL = GetNodeItems(library, null);
            MultiMapSet<string, string> tags_with_fingerprints = tags_with_fingerprints_ALL;

            // If this is exclusive mode, we want to constrain the list of items in the tree
            if (!is_negated && exclusive && 0 < selected_tags.Count)
            {
                bool first_set = true;
                HashSet<string> exclusive_fingerprints = new HashSet<string>();
                foreach (string selected_tag in selected_tags)
                {
                    if (first_set)
                    {
                        first_set = false;
                        exclusive_fingerprints.UnionWith(tags_with_fingerprints_ALL.Get(selected_tag));
                    }
                    else
                    {
                        exclusive_fingerprints.IntersectWith(tags_with_fingerprints_ALL.Get(selected_tag));
                    }
                }

                tags_with_fingerprints = GetNodeItems(library, exclusive_fingerprints);
            }

            // Filter them by the user filter
            List<string> tags_eligible = null;
            if (!String.IsNullOrEmpty(TxtSearchTermsFilter.Text))
            {
                string filter_set = TxtSearchTermsFilter.Text.ToLower();
                string[] filters = filter_set.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < filters.Length; ++i)
                {
                    filters[i] = filters[i].Trim();
                }

                tags_eligible = new List<string>();
                foreach (string tag in tags_with_fingerprints.Keys)
                {
                    string tag_lower = tag.ToLower();
                    foreach (string filter in filters)
                    {
                        if (tag_lower.Contains(filter))
                        {
                            tags_eligible.Add(tag);
                            break;
                        }
                    }
                }
            }
            else
            {
                tags_eligible = new List<string>(tags_with_fingerprints.Keys);
            }

            // Sort the tags
            List<string> tags_sorted = new List<string>(tags_eligible);
            if (ObjSort.IsChecked ?? false)
            {
                // sort tags by number of occurrences for each tag: more is better
                tags_sorted.Sort(delegate (string tag1, string tag2)
                {
                    return tags_with_fingerprints[tag2].Count - tags_with_fingerprints[tag1].Count;
                });
            }
            else
            {
                // sort tags alphabetically - default
                tags_sorted.Sort();
            }

            // Create the tag list to bind to
            List<GenericLibraryExplorerItem> displayed_items = new List<GenericLibraryExplorerItem>();
            foreach (string tag in tags_sorted)
            {
                GenericLibraryExplorerItem item = new GenericLibraryExplorerItem
                {
                    GenericLibraryExplorerControl = this,
                    library = library,

                    tag = tag,
                    fingerprints = tags_with_fingerprints[tag],

                    OnItemDragOver = OnItemDragOver,
                    OnItemDrop = OnItemDrop,
                    OnItemPopup = OnItemPopup,

                    IsSelected = selected_tags.Contains(tag)
                };

                displayed_items.Add(item);
            }

            // Bind baby bind - tag list
            TreeSearchTerms.DataContext = displayed_items;

            // Populate the chart
            PopulateChart(tags_with_fingerprints);

            // Then we have to list the associated documents
            HashSet<string> fingerprints = new HashSet<string>();
            if (0 < selected_tags.Count)
            {
                if (exclusive)
                {
                    bool first_set = true;
                    foreach (string selected_tag in selected_tags)
                    {
                        if (first_set)
                        {
                            fingerprints.UnionWith(tags_with_fingerprints.Get(selected_tag));
                        }
                        else
                        {
                            fingerprints.IntersectWith(tags_with_fingerprints.Get(selected_tag));
                        }
                    }
                }
                else
                {
                    foreach (string selected_tag in selected_tags)
                    {
                        fingerprints.UnionWith(tags_with_fingerprints.Get(selected_tag));
                    }
                }
            }

            // Implement the NEGATION
            if (is_negated && 0 < fingerprints.Count)
            {
                HashSet<string> negated_fingerprints = library.GetAllDocumentFingerprints();
                fingerprints = new HashSet<string>(negated_fingerprints.Except(fingerprints));
            }

            // And build a description of them
            // Build up the descriptive span
            Span descriptive_span = new Span();
            if (!String.IsNullOrEmpty(description_title))
            {
                Bold bold = new Bold();
                bold.Inlines.Add(description_title);
                descriptive_span.Inlines.Add(bold);
                descriptive_span.Inlines.Add(": ");
            }
            string separator = exclusive ? " AND " : " OR ";
            if (is_negated)
            {
                descriptive_span.Inlines.Add("NOT [ ");
            }
            descriptive_span.Inlines.Add(StringTools.ConcatenateStrings(selected_tags, separator, 0));
            if (is_negated)
            {
                descriptive_span.Inlines.Add(" ] ");
            }
            descriptive_span.Inlines.Add(" ");
            descriptive_span.Inlines.Add(LibraryFilterHelpers.GetClearImageInline("Clear this filter.", hyperlink_clear_all_OnClick));

            OnTagSelectionChanged?.Invoke(fingerprints, descriptive_span);
        }

        #region --- Charting methods ------------------------------------------------------------------------------------------------------------------

        private void PopulateChart(MultiMapSet<string, string> tags_with_fingerprints)
        {
            int N = 20;

            List<KeyValuePair<string, HashSet<string>>> top_n = tags_with_fingerprints.GetTopN(N);

            List<ChartItem> chart_items = new List<ChartItem>();
            for (int i = 0; i < top_n.Count; ++i)
            {
                if ("(none)" != top_n[i].Key && "<Untagged>" != top_n[i].Key)
                {
                    chart_items.Add(
                        new ChartItem
                        {
                            X = i,
                            Caption = top_n[i].Key,
                            Count = top_n[i].Value.Count
                        }
                        );
                }
            }

            ChartSearchTerms.ToolTip = String.Format("Top {0} {1} in your library.", N, description_title);
            ObjChartArea.PrimaryAxis.AxisVisibility = Visibility.Collapsed;
            ObjChartArea.SecondaryAxis.AxisVisibility = Visibility.Collapsed;

            ObjSeries.DataSource = chart_items;
            ObjSeries.BindingPathX = "ID";
            ObjSeries.BindingPathsY = new string[] { "Count" };
        }

        private void ObjSeries_MouseClick(object sender, ChartMouseEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_GenericExplorer_ChartItem);

            ChartSegment chart_segment = e.Segment;
            IList data_source = (IList)chart_segment.Series.DataSource;
            ChartItem chart_item = (ChartItem)data_source[chart_segment.CorrespondingPoints[0].Index];
            ToggleSelectItem(chart_item.Caption, KeyboardTools.IsCTRLDown() || KeyboardTools.IsShiftDown());
        }

        #endregion

    }


    #region --- Useful ancilliary classes ------------

    public class ChartItem
    {
        public int X { get; set; }
        public string Caption { get; set; }
        public int Count { get; set; }
    }

    public class ToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
            {
                return "";
            }

            ChartSegment chart_segment = value as ChartSegment;
            IList data_source = (IList)chart_segment.Series.DataSource;
            ChartItem chart_item = (ChartItem)data_source[chart_segment.CorrespondingPoints[0].Index];
            return String.Format("{0} ({1})", chart_item.Caption, chart_item.Count);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
