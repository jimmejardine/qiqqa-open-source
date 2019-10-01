using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.GUI;
using Utilities.Reflection;
using Qiqqa.Common.Configuration;
using Utilities.Strings;

namespace Qiqqa.Documents.BibTeXEditor
{
    /// <summary>
    /// Interaction logic for BibTeXEditorControl.xaml
    /// </summary>
    public partial class BibTeXEditorControl : UserControl
    {
        WeakDependencyPropertyChangeNotifier wdpcn;
        public static DependencyProperty BibTeXProperty = DependencyProperty.Register("BibTeX", typeof(string), typeof(BibTeXEditorControl), new PropertyMetadata());
        public string BibTeX
        {
            get
            {
                return (string)GetValue(BibTeXProperty);
            }
            set
            {
                SetValue(BibTeXProperty, value);
            }
        }

        [NonSerialized]
        AugmentedBindable<BibTeXEditorControl> bindable = null;
        public AugmentedBindable<BibTeXEditorControl> Bindable
        {
            get
            {
                if (null == bindable)
                {
                    bindable = new AugmentedBindable<BibTeXEditorControl>(this);                    
                }

                return bindable;
            }
        }

        private bool rebuilding_grid = false;
        private bool updating_from_grid = false;
        private bool updating_from_text = false;
        
        public BibTeXEditorControl()
        {
            Theme.Initialize();

            InitializeComponent();

            // The error panel
            ObjErrorPanel.Background = ThemeColours.Background_Brush_Warning;
            ObjErrorPanel.Opacity = .3;
            ObjErrorPanel.IsHitTestVisible = false;

            // Initial visibility
            ObjErrorPanel.Visibility = Visibility.Collapsed;

            ObjBibTeXGrid.Visibility = Visibility.Visible;
            ObjTextPanel.Visibility = Visibility.Collapsed;
            
            // Register for notifications of changes to the COMPONENT's bibtex
            wdpcn = new WeakDependencyPropertyChangeNotifier(this, BibTeXProperty);
            wdpcn.ValueChanged += OnBibTeXPropertyChanged;

            ComboRecordType.ItemsSource = EntryTypes.Instance.EntryTypeList;

            ObjBibTeXText.TextChanged += ObjBibTeXText_TextChanged;
            TxtRecordKey.TextChanged += OnGridTextChanged;
            
            ComboRecordType.SelectionChanged += ComboRecordType_SelectionChanged;
            ComboRecordType.KeyUp += ComboRecordType_KeyUp;

            ComboRecordTypeHeader.ToolTip =
                ComboRecordType.ToolTip = 
                    "Please select the type of reference this points to.\nThis affects the way Qiqqa InCite and BibTeX format the reference in your bibliographies.";

            TxtRecordKeyHeader.ToolTip =
                TxtRecordKey.ToolTip = 
                "Please enter a BibTeX key for this article.\nIt needs to be unique in your library as it is used to identify this reference when you use Qiqqa InCite or in LaTeX when you use the \\cite{KEY} command.";

            ImageBibTeXParseError.Visibility = Visibility.Collapsed;
            ImageBibTeXParseError.ToolTip = null;
            ImageBibTeXParseError.Source = Icons.GetAppIcon(Icons.BibTeXParseError);
            RenderOptions.SetBitmapScalingMode(ImageBibTeXParseError, BitmapScalingMode.HighQuality);

            ImageBibTeXModeToggle.Visibility = Visibility.Visible;
            ImageBibTeXModeToggle.ToolTip = "Toggle between a BibTeX grid and raw BibTeX text.";
            ImageBibTeXModeToggle.Source = Icons.GetAppIcon(Icons.Switch);
            RenderOptions.SetBitmapScalingMode(ImageBibTeXModeToggle, BitmapScalingMode.HighQuality);
            ImageBibTeXModeToggle.Cursor = Cursors.Hand;
            ImageBibTeXModeToggle.MouseDown += ImageBibTeXModeToggle_MouseDown;

            RebuidTextAndGrid();
        }

        void ImageBibTeXModeToggle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RebuidTextAndGrid();

            if (Visibility.Visible != ObjBibTeXGrid.Visibility)
            {
                ObjBibTeXGrid.Visibility = Visibility.Visible;
                ObjTextPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                ObjBibTeXGrid.Visibility = Visibility.Collapsed;
                ObjTextPanel.Visibility = Visibility.Visible;
            }
        }

        void OnBibTeXPropertyChanged(object sender, EventArgs e)
        {
            RebuidTextAndGrid();
        }

        public bool ForceHideNoBibTeXInstructions { get; set; }
        
        void RebuidTextAndGrid()
        {
            string bibtex = BibTeX;

            if (String.IsNullOrEmpty(bibtex) && !ForceHideNoBibTeXInstructions)
            {
                ObjNoBibTeXInstructions.Visibility = Visibility.Visible;
            }
            else
            {
                ObjNoBibTeXInstructions.Visibility = Visibility.Collapsed;
            }

            BibTexItem bibtex_item = BibTexParser.ParseOne(bibtex, true);

            if (null == bibtex_item)
            {
                bibtex_item = new BibTexItem();
            }

            // If there were any exceptions, go pink and jump to the text editor
            if (bibtex_item.Exceptions.Count > 0 || bibtex_item.Warnings.Count > 0)
            {
                TextBlock tb = new TextBlock();
                tb.FontFamily = new FontFamily("Courier New");
                tb.Text = bibtex_item.GetExceptionsAndMessagesString();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.MaxWidth = 400;

                ImageBibTeXParseError.ToolTip = tb;
                ImageBibTeXParseError.Visibility = ObjErrorPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ObjErrorPanel.ToolTip = null;
                ImageBibTeXParseError.Visibility = ObjErrorPanel.Visibility = Visibility.Collapsed;
            }

            BuildGridFromBibTeX(bibtex, bibtex_item);
            BuildTextFromBibTeX(bibtex, bibtex_item);
        }

        void ObjBibTeXText_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFromText();
        }

        void ComboRecordType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 < e.AddedItems.Count)
            {
                ComboRecordType.Text = e.AddedItems[0].ToString();
            }
            
            UpdateFromGrid(true);
        }

        void ComboRecordType_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateFromGrid(true);
        }

        void OnGridTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFromGrid(false);
        }

        // ------------------------------------------

        HashSet<AutoCompleteBox> first_text_change_suppression_set = new HashSet<AutoCompleteBox>();        
        void tb_key_TextChanged(object sender, RoutedEventArgs e)
        {
            // Sigh - this crappy control seems to set a text change just once after initialisation - even if we don't change the text.
            AutoCompleteBox sender_ac = (AutoCompleteBox)sender;
            if (first_text_change_suppression_set.Contains(sender_ac))
            {
                first_text_change_suppression_set.Remove(sender_ac);
            }
            else
            {
                UpdateFromGrid(false);
            }
        }

        private void UpdateFromText()
        {
            updating_from_text = true;
            BibTeX = ObjBibTeXText.Text;
            updating_from_text = false;
        }

        private void UpdateFromGrid(bool do_reparse)
        {
            if (rebuilding_grid) return;

            BibTexItem bibtex_item = new BibTexItem();
            bibtex_item.Type = ComboRecordType.Text;
            bibtex_item.Key = TxtRecordKey.Text;

            foreach (object child_obj in ObjBibTeXGrid.Children)
            {
                FrameworkElement child = child_obj as FrameworkElement;
                if (null != child)
                {
                    GridPair gp = child.Tag as GridPair;
                    if (null != gp)
                    {
                        if (gp.key == child)
                        {
                            string key = null;
                            { TextBlock tb = gp.key as TextBlock; if (null != tb) key = tb.Text; }
                            { AutoCompleteBox tb = gp.key as AutoCompleteBox; if (null != tb) key = tb.Text; }
                            string value = null;
                            { TextBox tb = gp.value as TextBox; if (null != tb) value = tb.Text; }

                            if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value))
                            {                                
                                bibtex_item.SetIfHasValue(key, value);
                            }
                        }
                    }
                }
            }
            
            // Check that the BibTeX is not completely empty
            string bibtex = bibtex_item.ToBibTex();
            if (bibtex_item.IsEmpty())
            {
                bibtex = "";
            }

            // Update the bibtex field
            updating_from_grid = !do_reparse;
            BibTeX = bibtex;
            updating_from_grid = false;
        }

        class GridPair
        {
            public FrameworkElement key;
            public FrameworkElement value;
        }

        private void BuildTextFromBibTeX(string bibtex, BibTexItem bibtex_item)
        {
            if (updating_from_text) return;

            ObjBibTeXText.Text = bibtex;
        }

        private void BuildGridFromBibTeX(string bibtex, BibTexItem bibtex_item)
        {
            if (updating_from_grid) return;

                // Refill the headers
                rebuilding_grid = true;
                ComboRecordType.Text = bibtex_item.Type;
                TxtRecordKey.Text = bibtex_item.Key;
                rebuilding_grid = false;

                // Reset the grid            
                ObjBibTeXGrid.Children.Clear();                

                int row = 0;
                EntryType entry_type = EntryTypes.Instance.GetEntryType(bibtex_item.Type);
                HashSet<string> processed_fields = new HashSet<string>();

                // First the POPULATED required ones
                foreach (string key in entry_type.requireds)
                {
                    string value = bibtex_item[key];
                    if (!String.IsNullOrEmpty(value))
                    {
                        processed_fields.Add(key);
                        BuildGridFromBibTeX_AddGridPair(row, key, value, false, true);
                        ++row;
                    }
                }

                // Then the POPULATED optional ones
                foreach (string key in entry_type.optionals)
                {
                    string value = bibtex_item[key];
                    if (!String.IsNullOrEmpty(value))
                    {
                        processed_fields.Add(key);
                        BuildGridFromBibTeX_AddGridPair(row, key, value, false, false);
                        ++row;
                    }
                }

                // Then the filled in ones
                foreach (var field in bibtex_item.Fields)
                {
                    if (!processed_fields.Contains(field.Key))
                    {
                        processed_fields.Add(field.Key);
                        BuildGridFromBibTeX_AddGridPair(row, field.Key, field.Value, true, false);
                        ++row;
                    }
                }

                // Then the UNPOPULATED required ones
                foreach (string key in entry_type.requireds)
                {
                    string value = bibtex_item[key];
                    if (!processed_fields.Contains(key))
                    {
                        processed_fields.Add(key);
                        BuildGridFromBibTeX_AddGridPair(row, key, value, false, true);
                        ++row;
                    }
                }


                // Then the UNPOPULATED optionals
                foreach (string key in entry_type.optionals)
                {
                    string value = bibtex_item[key];
                    if (!processed_fields.Contains(key))
                    {
                        processed_fields.Add(key);
                        BuildGridFromBibTeX_AddGridPair(row, key, value, false, false);
                        ++row;
                    }
                }

                // Then the UNPOPULATED user-specifieds
                {
                    var user_specified_keys = ConfigurationManager.Instance.ConfigurationRecord.Metadata_UserDefinedBibTeXFields;
                    if (!String.IsNullOrEmpty(user_specified_keys))
                    {
                        foreach (string key in StringTools.splitAtNewline(user_specified_keys))
                        {
                            string value = bibtex_item[key];
                            if (!processed_fields.Contains(key))
                            {
                                processed_fields.Add(key);
                                BuildGridFromBibTeX_AddGridPair(row, key, value, false, false);
                                ++row;
                            }
                        }
                    }
                }

                // And then a few extras
                for (int i = 0; i < 3; ++i)
                {
                    BuildGridFromBibTeX_AddGridPair(row, "", "", true, false);
                    ++row;
                }
        }

        private void BuildGridFromBibTeX_AddGridPair(int row, string key, string value, bool isEditableKey, bool isBoldKey)
        {
            // Check that we have enough rowdefinitions
            while (ObjBibTeXGrid.RowDefinitions.Count < row)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = GridLength.Auto;
                ObjBibTeXGrid.RowDefinitions.Add(rd);
            }

            GridPair gp = new GridPair();

            // The KEY column
                if (!isEditableKey)
                {
                    TextBlock tb_key = new TextBlock();
                    tb_key.VerticalAlignment = VerticalAlignment.Center;                    
                    if (isBoldKey)
                    {
                        tb_key.FontWeight = FontWeights.Bold;
                        tb_key.ToolTip = key + " - this is a recommended field for the article type you have selected.";
                    }
                    else
                    {
                        tb_key.ToolTip = key + " - this is an optional field for the article type you have selected.";
                    }
                    tb_key.Text = key;                    
                    Grid.SetColumn(tb_key, 0);
                    Grid.SetRow(tb_key, row);
                    ObjBibTeXGrid.Children.Add(tb_key);
                    gp.key = tb_key;
                    tb_key.Tag = gp;
                }
                else
                {
                    AutoCompleteBox tb_key = new AutoCompleteBox();
                    tb_key.Background = ThemeColours.Background_Brush_Blue_VeryDark;
                    tb_key.Background = Brushes.Transparent;
                    tb_key.ToolTip = "You can add any number of your own BibTeX fields here.  Just click and enter the field name and value.\nYou can use these to powerfully restrict your searches, or use them in your bibliographies if you have a CSL style that understands these specific fields.";
                    tb_key.ItemsSource = EntryTypes.Instance.FieldTypeList;                    
                    tb_key.Margin = new Thickness(1);
                    tb_key.VerticalAlignment = VerticalAlignment.Center;                    
                    if (isBoldKey) tb_key.FontWeight = FontWeights.Bold;
                    tb_key.Text = key;
                    Grid.SetColumn(tb_key, 0);
                    Grid.SetRow(tb_key, row);
                    ObjBibTeXGrid.Children.Add(tb_key);
                    gp.key = tb_key;
                    tb_key.Tag = gp;
                    tb_key.TextChanged += tb_key_TextChanged;
                    first_text_change_suppression_set.Add(tb_key);
                }

            // The VALUE column 
            {
                TextBox tb_value = new TextBox();                
                tb_value.Text = value;
                tb_value.VerticalContentAlignment = VerticalAlignment.Center;
                tb_value.TextWrapping = TextWrapping.Wrap;
                Grid.SetColumn(tb_value, 2);
                Grid.SetRow(tb_value, row);
                ObjBibTeXGrid.Children.Add(tb_value);
                gp.value = tb_value;
                tb_value.BorderThickness = new Thickness(0,1,0,0);
                tb_value.Tag = gp;
                tb_value.KeyDown += tb_value_KeyDown;
                tb_value.TextChanged += OnGridTextChanged;

                tb_value.ToolTip = "Enter the value associated with the field name to the left.\nHint: pressing CTRL and ; simultaneously will insert today's date.  That should help you populate those 'Accessed' fields...";
            }
        }

        void tb_value_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyboardTools.IsCTRLDown() && Key.OemSemicolon == e.Key)
            {
                TextBox tb = (TextBox)sender;
                tb.Text = DateTime.Now.ToString("d MMM yyyy");
                e.Handled = true;
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            BibTeXEditorControl bec = new BibTeXEditorControl();
            
            ControlHostingWindow window = new ControlHostingWindow("BibTeX editor", bec);
            window.Width = 500;
            window.Height = 300;
            window.Show();
        }
#endif

        #endregion
    }
}
