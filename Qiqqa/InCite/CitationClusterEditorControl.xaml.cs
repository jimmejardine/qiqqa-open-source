using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities.GUI;

namespace Qiqqa.InCite
{
    /// <summary>
    /// Interaction logic for CitationClusterEditorControl.xaml
    /// </summary>
    public partial class CitationClusterEditorControl : UserControl
    {
        CitationCluster current_citation_cluster = null;

        public delegate void CitationClusterChangedDelegate(CitationCluster citation_cluster);
        public event CitationClusterChangedDelegate CitationClusterChanged;

        public delegate void CitationClusterOpenPDFByReferenceKeyDelegate(string reference_key);
        public event CitationClusterOpenPDFByReferenceKeyDelegate CitationClusterOpenPDFByReferenceKey;

        public CitationClusterEditorControl()
        {
            InitializeComponent();

            CmdApply.Caption = "Apply";
            CmdRevert.Caption = "Revert";

            ObjSpecifierType.ItemsSource = CitationItem.OPTIONS_SPECIFIER_TYPE;
            ObjCitationsInCluster.KeyDown += ObjSpecifierType_KeyDown;

            ObjSpecifierLocation.ToolTip = "Set the specifier (page, section, chapter, etc) range for each of your references.\nIf you have more than one reference in this cluster, separate these ranges with semicolons.\nAt the moment, all references have to reference the same speficier type.";
            ObjPrefix.ToolTip = "Set the prefix for each of your references.\nIf you have more than one reference in this cluster, separate these prefixes with semicolons.";
            ObjSuffix.ToolTip = "Set the suffix for each of your references.\nIf you have more than one reference in this cluster, separate these suffixes with semicolons.";

            CmdRevert.Click += CmdRevert_Click;
            CmdApply.Click += CmdApply_Click;

            SetCitationCluster(null);
        }

        void ObjSpecifierType_KeyDown(object sender, KeyEventArgs e)
        {
            if (false) { }

            // Delete the selected item...
            else if (e.Key == Key.Delete)
            {
                ObservableCollection<string> citation_item_keys = (ObservableCollection<string>)ObjCitationsInCluster.ItemsSource;
                if (1 < citation_item_keys.Count)
                {
                    citation_item_keys.Remove((string)ObjCitationsInCluster.SelectedItem);
                }
                else
                {
                    MessageBoxes.Warn("You can't delete the last citation from a cluster.\nIf you want to delete the entire cluster, just delete it in Word.");
                }
                e.Handled = true;
            }

                // Open the selected item
            else if (e.Key == Key.Enter)
            {
                ObservableCollection<string> citation_item_keys = (ObservableCollection<string>)ObjCitationsInCluster.ItemsSource;
                if (0 < citation_item_keys.Count)
                {
                    string selected_item_reference_key = (string)ObjCitationsInCluster.SelectedItem;

                    if (null != selected_item_reference_key)
                    {
                        if (null != CitationClusterOpenPDFByReferenceKey)
                        {
                            CitationClusterOpenPDFByReferenceKey(selected_item_reference_key);
                        }
                    }

                }

                e.Handled = true;
            }
        }

        void CmdApply_Click(object sender, RoutedEventArgs e)
        {
            CitationCluster citation_cluster = current_citation_cluster;
            if (null != citation_cluster)
            {
                // Make our stored citation cluster look like the GUI choices
                {
                    // Delete any items that were removed in the GUI
                    ObservableCollection<string> citation_item_keys = (ObservableCollection<string>)ObjCitationsInCluster.ItemsSource;
                    for (int i = citation_cluster.citation_items.Count-1; i >= 0; --i)
                    {
                        if (!citation_item_keys.Contains(citation_cluster.citation_items[i].reference_key))
                        {
                            citation_cluster.citation_items.RemoveAt(i);
                        }
                    }
                    
                    // And apply the other settings
                    List<string> specifier_locations = new List<string>(ObjSpecifierLocation.Text.Split(';'));
                    while (specifier_locations.Count < citation_cluster.citation_items.Count)
                    {
                        specifier_locations.Add("");
                    }
                    List<string> prefixes = new List<string>(ObjPrefix.Text.Split(';'));
                    while (prefixes.Count < citation_cluster.citation_items.Count)
                    {
                        prefixes.Add("");
                    }
                    List<string> suffixes = new List<string>(ObjSuffix.Text.Split(';'));
                    while (suffixes.Count < citation_cluster.citation_items.Count)
                    {
                        suffixes.Add("");
                    }

                    for (int i = 0; i < citation_cluster.citation_items.Count; ++i)                    
                    {
                        var citation_item = citation_cluster.citation_items[i];

                        // Separate author date?
                        citation_item.SeparateAuthorsAndDate(ObjCheckSeparateAuthorDate.IsChecked ?? false);

                        // Locator on page?
                        citation_item.SetParameter(CitationItem.PARAM_SPECIFIER_TYPE, ObjSpecifierType.Text);
                        citation_item.SetParameter(CitationItem.PARAM_SPECIFIER_LOCATION, specifier_locations[i]);

                        // Prefix/suffix?
                        citation_item.SetParameter(CitationItem.PARAM_PREFIX, prefixes[i]);
                        citation_item.SetParameter(CitationItem.PARAM_SUFFIX, suffixes[i]);
                    }
                }

                // And apply the modified cluster back to Word
                {
                    if (null != CitationClusterChanged)
                    {
                        CitationClusterChanged(citation_cluster);
                    }
                }
            }
        }

        void CmdRevert_Click(object sender, RoutedEventArgs e)
        {
            ReflectCitationCluster(current_citation_cluster);
        }

        internal void SetCitationCluster(CitationCluster citation_cluster)
        {
            current_citation_cluster = citation_cluster;
            ReflectCitationCluster(current_citation_cluster);
        }


        private void ReflectCitationCluster(CitationCluster current_citation_cluster)
        {
            if (null == current_citation_cluster)
            {
                this.IsEnabled = false;
                this.ObjGridNoCitationSelectedInstructions.Visibility = Visibility.Visible;
                this.ObjGridNoCitationSelectedInstructions.Background = ThemeColours.Background_Brush_Blue_Dark;
                //this.ObjGridCitationSelectedPanel.Visibility = Visibility.Collapsed;

                ObjCitationsInCluster.ItemsSource = null;
                ObjCheckSeparateAuthorDate.IsChecked = false;
                ObjSpecifierType.Text = "";
                ObjSpecifierLocation.Text = "";
                ObjPrefix.Text = "";
                ObjSuffix.Text = "";
            }
            else
            {
                this.IsEnabled = true;
                this.ObjGridNoCitationSelectedInstructions.Visibility = Visibility.Collapsed;
                //this.ObjGridCitationSelectedPanel.Visibility = Visibility.Visible;

                // Populate the list of items
                ObservableCollection<string> citation_item_keys = new ObservableCollection<string>();
                foreach (var citation_item in current_citation_cluster.citation_items)
                {
                    citation_item_keys.Add(citation_item.reference_key);
                }
                ObjCitationsInCluster.ItemsSource = citation_item_keys;

                // Populate the options - we only use the first item's stuff...
                if (0 < current_citation_cluster.citation_items.Count)
                {
                    CitationItem citation_item = current_citation_cluster.citation_items[0];

                    // Separate author date?
                    string separate_author_date = citation_item.GetParameter(CitationItem.PARAM_SEPARATE_AUTHOR_DATE);
                    ObjCheckSeparateAuthorDate.IsChecked = (CitationItem.OPTION_SEPARATE_AUTHOR_DATE_TRUE == separate_author_date);

                    // Locator type?
                    string specifier_type = citation_item.GetParameter(CitationItem.PARAM_SPECIFIER_TYPE);
                    ObjSpecifierType.Text = specifier_type;
                    
                    // For the actual specifier, lets allow multiple selections
                    string specifier_locations = "";
                    string prefixes = "";
                    string suffixes = "";
                    for (int i = 0; i < current_citation_cluster.citation_items.Count; ++i)
                    {
                        specifier_locations = specifier_locations + current_citation_cluster.citation_items[i].GetParameter(CitationItem.PARAM_SPECIFIER_LOCATION) + ";";
                        prefixes = prefixes + current_citation_cluster.citation_items[i].GetParameter(CitationItem.PARAM_PREFIX) + ";";
                        suffixes = suffixes + current_citation_cluster.citation_items[i].GetParameter(CitationItem.PARAM_SUFFIX) + ";";
                    }
                    specifier_locations = specifier_locations.TrimEnd(';');
                    prefixes = prefixes.TrimEnd(';');
                    suffixes = suffixes.TrimEnd(';');
                    ObjSpecifierLocation.Text = specifier_locations;
                    ObjPrefix.Text = prefixes;
                    ObjSuffix.Text = suffixes;
                }
            }
        }
    }
}
