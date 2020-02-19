using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.GUI;
using Utilities.Reflection;
using Utilities.Strings;

namespace Qiqqa.Documents.BibTeXEditor
{
    /// <summary>
    /// Interaction logic for BibTeXEditorControl.xaml
    /// </summary>
    public partial class BibTeXEditorControl : UserControl, IDisposable
    {
        // These three buttons are (optionally) provided by the parent control.
        // Note the use of WeakReferences to help ensure there's no cyclic dependency
        // in the UI that prevents the GC from cleaning up once we're done.
        private WeakReference<FrameworkElement> BibTeXParseErrorButtonRef;
        private WeakReference<FrameworkElement> BibTeXModeToggleButtonRef;
        private WeakReference<FrameworkElement> BibTeXUndoEditButtonRef;
        private WeakDependencyPropertyChangeNotifier wdpcn;

        public static DependencyProperty BibTeXProperty = DependencyProperty.Register("BibTeX", typeof(string), typeof(BibTeXEditorControl), new PropertyMetadata());
        public string BibTeX
        {
            get => (string)GetValue(BibTeXProperty);
            set => SetValue(BibTeXProperty, value);
        }

        private AugmentedBindable<BibTeXEditorControl> bindable = null;
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

            SizeChanged += BibTeXEditorControl_SizeChanged;

            // The error panel
            //ObjErrorPanel.Background = ThemeColours.Background_Brush_Warning;
            //ObjErrorPanel.Opacity = .3;
            ObjErrorPanel.IsHitTestVisible = false;

            ObjBibTeXErrorText.Background = ThemeColours.Background_Brush_Warning;
            //ObjBibTeXErrorText.Background.Opacity = 1.0;

            // Initial visibility
            //
            // For the three panels use `Hidden` instead of `Collapsed` to ensure their
            // dimensions are kept intact, irrespective in which 'edit mode' the user
            // is right now.
            // 
            ObjErrorPanel.Visibility = Visibility.Hidden;
            ObjBibTeXGrid.Visibility = Visibility.Hidden;
            ObjTextPanel.Visibility = Visibility.Visible;

            Grid.SetZIndex(ObjTextPanel, 3);
            Grid.SetZIndex(ObjBibTeXGrid, 5);
            Grid.SetZIndex(ObjErrorPanel, 2);

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

            RebuidTextAndGrid();
        }

        private void BibTeXEditorControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double table_height1 = ObjGridPanel.ActualHeight;
            //double table_height2 = ObjHeaderGrid.ActualHeight;
            //double table_height3 = ObjBibTeXGrid.ActualHeight;

            //double rawtxt_height1 = ObjTextPanel.ActualHeight;
            //double rawtxt_height2 = ObjBibTeXTextScrollViewer.ActualHeight;
            //double rawtxt_height3 = ObjBibTeXText.ActualHeight;

            //double errtxt_height1 = ObjErrorPanel.ActualHeight;
            //double errtxt_height2 = ObjBibTeXErrorScrollViewer.ActualHeight;
            //double errtxt_height3 = ObjBibTeXErrorText.ActualHeight;

            const double THRESHOLD = 100;

            if (table_height1 > THRESHOLD)
            {
                double maxh1 = ObjBibTeXTextScrollViewer.MaxHeight;
                double maxh2 = ObjBibTeXErrorScrollViewer.MaxHeight;

                // tweak the control so the Parsed View gives us the master MaxHeight:
                ObjBibTeXTextScrollViewer.MaxHeight = THRESHOLD;
                ObjBibTeXErrorScrollViewer.MaxHeight = THRESHOLD;
                UpdateLayout();

                table_height1 = ObjGridPanel.ActualHeight;

                if (table_height1 > THRESHOLD)
                {
                    ObjBibTeXTextScrollViewer.MaxHeight = table_height1;
                    ObjBibTeXErrorScrollViewer.MaxHeight = table_height1;
                }
                else
                {
                    ObjBibTeXTextScrollViewer.MaxHeight = double.PositiveInfinity;
                    ObjBibTeXErrorScrollViewer.MaxHeight = double.PositiveInfinity;
                }

                if (Math.Abs(maxh1 - ObjBibTeXTextScrollViewer.MaxHeight) > 0.25)
                {
                    UpdateLayout();
                }
            }
        }

        public void RegisterOverlayButtons(FrameworkElement BibTeXParseErrorButton, FrameworkElement BibTeXModeToggleButton, FrameworkElement BibTeXUndoEditButton, double IconHeight = double.NaN)
        {
            BibTeXParseErrorButtonRef = new WeakReference<FrameworkElement>(BibTeXParseErrorButton);
            BibTeXModeToggleButtonRef = new WeakReference<FrameworkElement>(BibTeXModeToggleButton);
            BibTeXUndoEditButtonRef = new WeakReference<FrameworkElement>(BibTeXUndoEditButton);

            // button references MAY be NULL when this method is invoked on dialog close as part of the cleanup:
            if (BibTeXParseErrorButton != null)
            {
                if (BibTeXParseErrorButton is AugmentedButton)
                {
                    AugmentedButton btn = BibTeXParseErrorButton as AugmentedButton;
                    btn.Caption = "View parse errors";
                    btn.Icon = Icons.GetAppIcon(Icons.BibTeXParseError2);
                    if (IconHeight > 0)
                    {
                        btn.IconHeight = IconHeight;
                    }
                }
                BibTeXParseErrorButton.Visibility = Visibility.Visible;
                BibTeXParseErrorButton.IsEnabled = false;
                BibTeXParseErrorButton.Opacity = 0.3;
                BibTeXParseErrorButton.ToolTip = "Shows BibTeX errors when they occur";
                if (BibTeXParseErrorButton is Image)
                {
                    Image imgBtn = BibTeXParseErrorButton as Image;
                    imgBtn.Source = Icons.GetAppIcon(Icons.BibTeXParseError2);
                    RenderOptions.SetBitmapScalingMode(imgBtn, BitmapScalingMode.HighQuality);
                }
            }

            if (BibTeXModeToggleButton != null)
            {
                if (BibTeXModeToggleButton is AugmentedButton)
                {
                    AugmentedButton btn = BibTeXModeToggleButton as AugmentedButton;
                    btn.Caption = "Toggle edit mode";
                    btn.Icon = Icons.GetAppIcon(Icons.BibTeXEditToggleMode1);
                    if (IconHeight > 0)
                    {
                        btn.IconHeight = IconHeight;
                    }
                }
                BibTeXModeToggleButton.Visibility = Visibility.Visible;
                BibTeXModeToggleButton.ToolTip = "Toggle between a BibTeX grid ('parsed mode') and raw BibTeX text ('raw/unparssed mode').";
                if (BibTeXModeToggleButton is Image)
                {
                    Image imgBtn = BibTeXModeToggleButton as Image;
                    imgBtn.Source = Icons.GetAppIcon(Icons.BibTeXEditToggleMode1);
                    RenderOptions.SetBitmapScalingMode(imgBtn, BitmapScalingMode.HighQuality);
                }
                BibTeXModeToggleButton.Cursor = Cursors.Hand;
#if false
                BibTeXModeToggleButton.MouseDown += ToggleBibTeXMode;
#endif
            }

            if (BibTeXUndoEditButton != null)
            {
                if (BibTeXUndoEditButton is AugmentedButton)
                {
                    AugmentedButton btn = BibTeXUndoEditButton as AugmentedButton;
                    btn.Caption = "Undo";
                    btn.Icon = Icons.GetAppIcon(Icons.Previous2);
                    if (IconHeight > 0)
                    {
                        btn.IconHeight = IconHeight;
                    }
                }
                BibTeXModeToggleButton.Visibility = Visibility.Visible;
                BibTeXModeToggleButton.ToolTip = null;
                BibTeXUndoEditButton.IsEnabled = false;
                BibTeXUndoEditButton.Opacity = 0.3;
                if (BibTeXUndoEditButton is Image)
                {
                    Image imgBtn = BibTeXUndoEditButton as Image;
                    imgBtn.Source = Icons.GetAppIcon(Icons.Previous2);
                    RenderOptions.SetBitmapScalingMode(imgBtn, BitmapScalingMode.HighQuality);
                }
            }
        }

        //void ImageBibTeXModeToggle_MouseDown(object sender, MouseButtonEventArgs e)
        public void ToggleBibTeXMode(TriState state)
        {
            RebuidTextAndGrid();

            if (TriState.Arbitrary == state)
            {
                // toggle view mode:
                state = (Visibility.Visible != ObjBibTeXGrid.Visibility ? TriState.On : TriState.Off);
            }

            if (TriState.On == state)
            {
                ObjBibTeXGrid.Visibility = Visibility.Visible;
                ObjTextPanel.Visibility = Visibility.Hidden;
                ObjErrorPanel.Visibility = Visibility.Hidden;

                Grid.SetZIndex(ObjTextPanel, 3);
                Grid.SetZIndex(ObjBibTeXGrid, 5);
                Grid.SetZIndex(ObjErrorPanel, 2);
            }
            else
            {
                ObjBibTeXGrid.Visibility = Visibility.Hidden;
                ObjTextPanel.Visibility = Visibility.Visible;
                ObjErrorPanel.Visibility = Visibility.Hidden;

                Grid.SetZIndex(ObjTextPanel, 5);
                Grid.SetZIndex(ObjBibTeXGrid, 3);
                Grid.SetZIndex(ObjErrorPanel, 2);
            }
        }

        public void ToggleBibTeXErrorView()
        {
            if (!String.IsNullOrEmpty(ObjBibTeXErrorText.Text))
            {
                ObjErrorPanel.Visibility = (ObjErrorPanel.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible);

                Grid.SetZIndex(ObjErrorPanel, ObjErrorPanel.Visibility == Visibility.Visible ? 6 : 2);
            }
            else
            {
                ObjErrorPanel.Visibility = Visibility.Hidden;

                Grid.SetZIndex(ObjErrorPanel, 2);
            }
        }

        private void OnBibTeXPropertyChanged(object sender, EventArgs e)
        {
            RebuidTextAndGrid();
        }

        public bool ForceHideNoBibTeXInstructions { get; set; }

        private void RebuidTextAndGrid()
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
                string error_msg = bibtex_item.GetExceptionsAndMessagesString().Trim();

                ObjBibTeXErrorText.Text = error_msg;

                ObjErrorPanel.ToolTip = error_msg;
                ObjErrorPanel.Visibility = Visibility.Visible;

                FrameworkElement errBtn = null;
                if (BibTeXParseErrorButtonRef?.TryGetTarget(out errBtn) ?? false)
                {
                    errBtn.ToolTip = error_msg;
                    errBtn.Visibility = Visibility.Visible;
                    errBtn.IsEnabled = true;
                    errBtn.Opacity = 1.0;
                }
            }
            else
            {
                ObjBibTeXErrorText.Text = "";

                ObjErrorPanel.ToolTip = null;
                ObjErrorPanel.Visibility = Visibility.Hidden;

                FrameworkElement errBtn = null;
                if (BibTeXParseErrorButtonRef?.TryGetTarget(out errBtn) ?? false)
                {
                    errBtn.ToolTip = "Shows BibTeX errors when they occur";
                    //errBtn.Visibility = Visibility.Collapsed;
                    errBtn.IsEnabled = false;
                    errBtn.Opacity = 0.3;
                }
            }

            BuildGridFromBibTeX(bibtex, bibtex_item);
            BuildTextFromBibTeX(bibtex, bibtex_item);
        }

        private void ObjBibTeXText_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFromText();
        }

        private void ComboRecordType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 < e.AddedItems.Count)
            {
                ComboRecordType.Text = e.AddedItems[0].ToString();
            }

            UpdateFromGrid(true);
        }

        private void ComboRecordType_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateFromGrid(true);
        }

        private void OnGridTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFromGrid(false);
        }

        // ------------------------------------------

        private HashSet<AutoCompleteBox> first_text_change_suppression_set = new HashSet<AutoCompleteBox>();

        private void tb_key_TextChanged(object sender, RoutedEventArgs e)
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
                                bibtex_item[key] = value;
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

        private class GridPair
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
                    BuildGridFromBibTeX_AddGridPair(ref row, key, value, false, true);
                }
            }

            // Then the POPULATED optional ones
            foreach (string key in entry_type.optionals)
            {
                string value = bibtex_item[key];
                if (!String.IsNullOrEmpty(value))
                {
                    processed_fields.Add(key);
                    BuildGridFromBibTeX_AddGridPair(ref row, key, value, false, false);
                }
            }

            // Then the filled in ones
            foreach (var field in bibtex_item.Fields)
            {
                if (!processed_fields.Contains(field.Key))
                {
                    processed_fields.Add(field.Key);
                    BuildGridFromBibTeX_AddGridPair(ref row, field.Key, field.Value, true, false);
                }
            }

            // Then the UNPOPULATED required ones
            foreach (string key in entry_type.requireds)
            {
                string value = bibtex_item[key];
                if (!processed_fields.Contains(key))
                {
                    processed_fields.Add(key);
                    BuildGridFromBibTeX_AddGridPair(ref row, key, value, false, true);
                }
            }


            // Then the UNPOPULATED optionals
            foreach (string key in entry_type.optionals)
            {
                string value = bibtex_item[key];
                if (!processed_fields.Contains(key))
                {
                    processed_fields.Add(key);
                    BuildGridFromBibTeX_AddGridPair(ref row, key, value, false, false);
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
                            BuildGridFromBibTeX_AddGridPair(ref row, key, value, false, false);
                        }
                    }
                }
            }

            // And then a few extras
            for (int i = 0; i < 6; ++i)
            {
                BuildGridFromBibTeX_AddGridPair(ref row, "", "", true, false);
            }
        }

        private void BuildGridFromBibTeX_AddGridPair(ref int row, string key, string value, bool isEditableKey, bool isBoldKey)
        {
            // Check that we have enough rowdefinitions
            while (ObjBibTeXGrid.RowDefinitions.Count <= row)
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
                tb_value.BorderThickness = new Thickness(0, 1, 0, 0);
                tb_value.Tag = gp;
                tb_value.KeyDown += tb_value_KeyDown;
                tb_value.TextChanged += OnGridTextChanged;

                tb_value.ToolTip = "Enter the value associated with the field name to the left.\nHint: pressing CTRL and ; simultaneously will insert today's date.  That should help you populate those 'Accessed' fields...";
            }

            ++row;
        }

        private void tb_value_KeyDown(object sender, KeyEventArgs e)
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

        #region --- IDisposable ------------------------------------------------------------------------

        ~BibTeXEditorControl()
        {
            Logging.Debug("~BibTeXEditorControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing BibTeXEditorControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("BibTeXEditorControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                // *Nobody* gets any updates from us anymore, so we can delete cached content etc. in peace. (https://github.com/jimmejardine/qiqqa-open-source/issues/121)
                BindingOperations.ClearBinding(this, BibTeXProperty);
            }, must_exec_in_UI_thread: true);

            WPFDoEvents.SafeExec(() =>
            {
                // Get rid of managed resources / get rid of cyclic references:
                if (null != wdpcn)
                {
                    wdpcn.ValueChanged -= OnBibTeXPropertyChanged;
                }
            }, must_exec_in_UI_thread: true);

            WPFDoEvents.SafeExec(() =>
            {
                // discard all references which might otherwise potentially cause memleaks due to (potential) references cycles:
                BibTeXParseErrorButtonRef?.SetTarget(null);
                BibTeXModeToggleButtonRef?.SetTarget(null);
                BibTeXUndoEditButtonRef?.SetTarget(null);
            }, must_exec_in_UI_thread: true);

            WPFDoEvents.SafeExec(() =>
            {
                bindable = null;
                // BibTeX = "";  <-- forbidden to reset as that MAY trigger a dependency update! (https://github.com/jimmejardine/qiqqa-open-source/issues/121)
            });

            WPFDoEvents.SafeExec(() =>
            {
                // Get rid of managed resources / get rid of cyclic references:
                wdpcn?.Dispose();
            });

            WPFDoEvents.SafeExec(() =>
            {
                ObjBibTeXText.TextChanged -= ObjBibTeXText_TextChanged;
                TxtRecordKey.TextChanged -= OnGridTextChanged;

                ComboRecordType.SelectionChanged -= ComboRecordType_SelectionChanged;
                ComboRecordType.KeyUp -= ComboRecordType_KeyUp;
            }, must_exec_in_UI_thread: true);

            WPFDoEvents.SafeExec(() =>
            {
                // Clear the references for sanity's sake
                BibTeXParseErrorButtonRef = null;
                BibTeXModeToggleButtonRef = null;
                BibTeXUndoEditButtonRef = null;

                wdpcn = null;
                bindable = null;
            });

            ++dispose_count;
        }

        #endregion

    }
}
