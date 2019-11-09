#if !HAS_NO_GUI

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using icons;

namespace Utilities.GUI.DualTabbedLayoutStuff
{
    public class DualTabbedLayout : Grid
    {
        private TabControl tab_control_left = new TabControl();
        private TabControl tab_control_right = new TabControl();
        private TabControl tab_control_bottom = new TabControl();
        private List<Window> floating_windows = new List<Window>();

        private Window owner_window = null;
        private BitmapSource window_icon = null;

        public delegate void OnActiveItemChangedDelegate(FrameworkElement content);
        public event OnActiveItemChangedDelegate OnActiveItemChanged;

        private DualTabbedLayoutItem.Location last_location = DualTabbedLayoutItem.Location.Left;
        private List<DualTabbedLayoutItem> recently_used_items = new List<DualTabbedLayoutItem>();

        public DualTabbedLayout()
        {
            Theme.Initialize();

            KeyboardNavigation.SetDirectionalNavigation(tab_control_left, KeyboardNavigationMode.None);
            KeyboardNavigation.SetDirectionalNavigation(tab_control_right, KeyboardNavigationMode.None);
            KeyboardNavigation.SetDirectionalNavigation(tab_control_bottom, KeyboardNavigationMode.None);

            tab_control_left.Background = Brushes.Transparent;
            tab_control_right.Background = Brushes.Transparent;
            tab_control_bottom.Background = Brushes.Transparent;

            tab_control_left.SelectionChanged += tab_control_SelectionChanged;
            tab_control_right.SelectionChanged += tab_control_SelectionChanged;
            tab_control_bottom.SelectionChanged += tab_control_SelectionChanged;

            tab_control_left.Padding = new Thickness(0);
            tab_control_right.Padding = new Thickness(0);
            tab_control_bottom.Padding = new Thickness(0);

            ReevaluateLayout();
        }

        public enum TabPositions
        {
            TopBottom,
            Sides
        }

        public TabPositions TabPosition
        {
            set
            {
                switch (value)
                {
                    case TabPositions.TopBottom:
                        tab_control_left.TabStripPlacement = Dock.Top;
                        tab_control_right.TabStripPlacement = Dock.Top;
                        tab_control_bottom.TabStripPlacement = Dock.Top;
                        break;

                    case TabPositions.Sides:
                        tab_control_left.LayoutTransform = new RotateTransform(+90);
                        foreach (TabItem sti in tab_control_left.Items)
                        {
                            FrameworkElement fe = (FrameworkElement)sti.Content;
                            fe.LayoutTransform = new RotateTransform(-90);
                        }

                        // NB: THERE IS A LOT OF WORK STILL TO BE DONE TO GET ALL THE NEW ITEMS TO BE ROTATED...
                        break;

                    default:
                        Logging.Warn("Unknown TabPosition " + value);
                        break;
                }
            }
        }

        private void tab_control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This hack is to let the combobox autocomplete not select the Tab...
            if (e.OriginalSource.GetType() != typeof(TabControl))
            {
                return;
            }

            TabControl tab_control = sender as TabControl;
            if (null != tab_control)
            {
                TabItem tab_item = tab_control.SelectedItem as TabItem;
                if (null != tab_item)
                {
                    DualTabbedLayoutItem item = (DualTabbedLayoutItem)tab_item.Tag;
                    //Logging.Info("Newly selected DualTabbedLayoutItem is {0}", item);

                    tab_item.Focus();

                    item.MarkAsRecentlyUsed();

                    if (OnActiveItemChanged != null)
                    {
                        OnActiveItemChanged(item.Content);
                    }
                }
            }

            e.Handled = true;
        }

        public Window OwnerWindow
        {
            get => owner_window;
            set
            {
                if (null != owner_window)
                {
                    owner_window.StateChanged -= owner_window_StateChanged;
                    owner_window.Closing -= owner_window_Closing;
                }

                owner_window = value;
                if (null != owner_window)
                {
                    owner_window.StateChanged += owner_window_StateChanged;
                    owner_window.Closing += owner_window_Closing;
                }

            }
        }

        private void owner_window_StateChanged(object sender, EventArgs e)
        {
            Window owner_window = (Window)sender;

            if (owner_window.WindowState == WindowState.Minimized)
            {
                foreach (Window window in floating_windows)
                {
                    window.WindowState = owner_window.WindowState;
                    window.ShowInTaskbar = false;
                }
            }
            else if (owner_window.WindowState == WindowState.Normal)
            {
                foreach (Window window in floating_windows)
                {
                    window.WindowState = owner_window.WindowState;
                    window.ShowInTaskbar = true;
                }
            }
        }

        private void owner_window_Closing(object sender, CancelEventArgs e)
        {
            List<Window> windows = new List<Window>(floating_windows);
            foreach (Window owner_window in windows)
            {
                DualTabbedLayoutItem item = (DualTabbedLayoutItem)owner_window.Tag;
                item.WantsLeft();
            }
        }

        public BitmapSource WindowIcon
        {
            set => window_icon = value;
        }


        private void ReevaluateLayout()
        {
            Children.Clear();
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();

            // Which panels do we need
            bool have_left = (0 < tab_control_left.Items.Count);
            bool have_right = (0 < tab_control_right.Items.Count);
            bool have_left_right = have_left && have_right;
            bool have_bottom = (0 < tab_control_bottom.Items.Count);
            bool have_top_bottom = (have_left || have_right) && have_bottom;

            Grid grid_top = this;
            Grid grid_bottom = this;
            Grid grid_left = this;
            Grid grid_right = this;

            // Create our grid ensemble
            if (have_top_bottom)
            {
                Grid parent_grid = new Grid();

                parent_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                parent_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                parent_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                grid_top = grid_left = grid_right = new Grid();
                SetRow(grid_top, 0);
                parent_grid.Children.Add(grid_top);
                GridSplitter grid_splitter = CreateSplitterHorizontal();
                SetRow(grid_splitter, 1);
                parent_grid.Children.Add(grid_splitter);
                grid_bottom = new Grid();
                SetRow(grid_bottom, 2);
                parent_grid.Children.Add(grid_bottom);

                Children.Add(parent_grid);
            }

            if (have_left_right)
            {
                Grid parent_grid = new Grid();

                parent_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                parent_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                parent_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                grid_left = new Grid();
                SetColumn(grid_left, 0);
                parent_grid.Children.Add(grid_left);
                GridSplitter grid_splitter = CreateSplitterVertical();
                SetColumn(grid_splitter, 1);
                parent_grid.Children.Add(grid_splitter);
                grid_right = new Grid();
                SetColumn(grid_right, 2);
                parent_grid.Children.Add(grid_right);

                grid_top.Children.Add(parent_grid);
            }

            if (have_left)
            {
                ClearTabControlParentGrid(tab_control_left);
                grid_left.Children.Add(tab_control_left);
            }
            if (have_right)
            {
                ClearTabControlParentGrid(tab_control_right);
                grid_right.Children.Add(tab_control_right);
            }
            if (have_bottom)
            {
                ClearTabControlParentGrid(tab_control_bottom);
                grid_bottom.Children.Add(tab_control_bottom);
            }
        }

        private static void ClearTabControlParentGrid(TabControl tab_control)
        {
            Grid grid = tab_control.Parent as Grid;
            if (null != grid)
            {
                grid.Children.Remove(tab_control);
            }
        }

        private static GridSplitter CreateSplitterHorizontal()
        {
            GridSplitter grid_splitter_horizontal = new GridSplitter();
            grid_splitter_horizontal.Height = 3;
            grid_splitter_horizontal.VerticalAlignment = VerticalAlignment.Center;
            grid_splitter_horizontal.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid_splitter_horizontal.ShowsPreview = true;
            return grid_splitter_horizontal;
        }

        private static GridSplitter CreateSplitterVertical()
        {
            GridSplitter grid_splitter_vertical = new GridSplitter();
            grid_splitter_vertical.Width = 3;
            grid_splitter_vertical.VerticalAlignment = VerticalAlignment.Stretch;
            grid_splitter_vertical.HorizontalAlignment = HorizontalAlignment.Center;
            grid_splitter_vertical.ShowsPreview = true;
            return grid_splitter_vertical;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference_key">Is used as a unique id for when you want to bring the window to the front, check for uniqueness, etc</param>
        /// <param name="header">The title on the tab header or window</param>
        /// <param name="icon"></param>
        /// <param name="can_close"></param>
        /// <param name="can_floating"></param>
        /// <param name="content"></param>
        public void AddContent(string reference_key, string header, BitmapSource icon, bool can_close, bool can_floating, FrameworkElement content, Color? background_color = null)
        {
            DualTabbedLayoutItem item = new DualTabbedLayoutItem(reference_key, header, icon, can_close, can_floating, content, this, background_color);
            item.WantsLocation(last_location);
        }

        public void CloseAllContent()
        {
            List<DualTabbedLayoutItem> items = GetAllTabItems();
            foreach (var item in items)
            {
                WantsClose(item);
            }
        }

        public void CloseContent(FrameworkElement fe)
        {
            List<DualTabbedLayoutItem> items = GetAllTabItems();
            foreach (var item in items)
            {
                if (fe == item.content)
                {
                    WantsClose(item);
                }
            }
        }

        public FrameworkElement MakeActive(string reference_key)
        {
            DualTabbedLayoutItem item = Find(reference_key, true);
            if (null != item)
            {
                return item.content;
            }
            else
            {
                return null;
            }
        }

        public bool Contains(string reference_key)
        {
            DualTabbedLayoutItem item = Find(reference_key, false);
            return null != item;
        }

        private DualTabbedLayoutItem Find(string reference_key, bool make_visible)
        {
            DualTabbedLayoutItem item = null;
            if (null == item) item = FindTabItem(reference_key, make_visible);
            if (null == item) item = FindFloating(reference_key, make_visible);
            return item;
        }

        internal void WantsLeft(DualTabbedLayoutItem item)
        {
            last_location = DualTabbedLayoutItem.Location.Left;

            ClearTabItem(item, tab_control_right);
            ClearTabItem(item, tab_control_bottom);
            ClearFloating(item);
            BuildTabItem(item, tab_control_left);
        }

        internal void WantsRight(DualTabbedLayoutItem item)
        {
            last_location = DualTabbedLayoutItem.Location.Right;

            ClearTabItem(item, tab_control_left);
            ClearTabItem(item, tab_control_bottom);
            ClearFloating(item);
            BuildTabItem(item, tab_control_right);
        }

        internal void WantsBottom(DualTabbedLayoutItem item)
        {
            last_location = DualTabbedLayoutItem.Location.Bottom;

            ClearTabItem(item, tab_control_left);
            ClearTabItem(item, tab_control_right);
            ClearFloating(item);
            BuildTabItem(item, tab_control_bottom);
        }

        public void WantsFloating(string reference_key)
        {
            WantsFloating(Find(reference_key, false), false);
        }

        internal void WantsFloating(DualTabbedLayoutItem item)
        {
            WantsFloating(item, true);
        }

        internal void WantsFloating(DualTabbedLayoutItem item, bool set_last_location)
        {
            if (set_last_location)
            {
                last_location = DualTabbedLayoutItem.Location.Floating;
            }

            ClearTabItem(item, tab_control_left);
            ClearTabItem(item, tab_control_right);
            ClearTabItem(item, tab_control_bottom);

            Window window = BuildFloating(item);
            floating_windows.Add(window);
            window.Show();
        }

        internal void WantsClose(DualTabbedLayoutItem item)
        {
            ClearTabItem(item, tab_control_left);
            ClearTabItem(item, tab_control_right);
            ClearTabItem(item, tab_control_bottom);
            ClearFloating(item);

            // Remove the closing tab item from the recently used list
            for (int i = recently_used_items.Count - 1; i >= 0; --i)
            {
                if (recently_used_items[i] == item)
                {
                    recently_used_items.RemoveAt(i);
                }
            }

            // Dispose the TabItem
            IDisposable disposable = item.content as IDisposable;
            if (null != disposable)
            {
                Logging.Info("Tabbed manager is disposing of window {0}", item.header);
                disposable.Dispose();
            }
            item.content = null;

            // Make the last recently used the current focus
            if (recently_used_items.Any())
            {
                DualTabbedLayoutItem recent_item = recently_used_items.Last();

                Find(recent_item.header, true);
            }
        }

        private void BuildTabItem(DualTabbedLayoutItem item, TabControl tab_control)
        {
            TabItem tab_item = new TabItem();
            tab_item.Tag = item;
            tab_item.Header = new ItemHeader(item);
            tab_item.Content = item.content;
            tab_item.Background = ThemeColours.Background_Brush_Blue_Light;
            tab_item.KeyDown += tab_item_KeyDown;

            tab_item.AllowDrop = true;
            tab_item.DragEnter += tab_item_DragEnter;
            tab_item.DragOver += tab_item_DragOver;
            tab_item.Drop += tab_item_Drop;

            tab_control.Items.Add(tab_item);
            if (1 == tab_control.Items.Count)
            {
                ReevaluateLayout();
            }

            tab_item.IsSelected = true;
        }

#region --- Drag bring to front -------------------------------------------------

        private object potential_drag_to_front_target = null;
        private DateTime potential_drag_to_front_start_time = DateTime.MinValue;

        private void tab_item_DragEnter(object sender, DragEventArgs e)
        {
            TabItem tab_item = (TabItem)sender;
            DualTabbedLayoutItem item = (DualTabbedLayoutItem)tab_item.Tag;

            potential_drag_to_front_target = sender;
            potential_drag_to_front_start_time = DateTime.UtcNow;

            // Pass on to IDualTabbedLayoutDragDrop
            IDualTabbedLayoutDragDrop idtldd = item.Content as IDualTabbedLayoutDragDrop;
            if (null != idtldd)
            {
                idtldd.DualTabbedLayoutDragEnter(sender, e);
            }
        }

        private void tab_item_DragOver(object sender, DragEventArgs e)
        {
            TabItem tab_item = (TabItem)sender;
            DualTabbedLayoutItem item = (DualTabbedLayoutItem)tab_item.Tag;

            if (sender == potential_drag_to_front_target)
            {
                if (DateTime.UtcNow.Subtract(potential_drag_to_front_start_time).TotalMilliseconds > 300)
                {
                    MarkAsRecentlyUsed(item);
                    tab_item.IsSelected = true;
                }
            }
            else
            {
                sender = potential_drag_to_front_target;
                potential_drag_to_front_start_time = DateTime.UtcNow;
            }

            // Pass on to IDualTabbedLayoutDragDrop
            IDualTabbedLayoutDragDrop idtldd = item.Content as IDualTabbedLayoutDragDrop;
            if (null != idtldd)
            {
                idtldd.DualTabbedLayoutDragOver(sender, e);
            }
        }

        private void tab_item_Drop(object sender, DragEventArgs e)
        {
            TabItem tab_item = (TabItem)sender;
            DualTabbedLayoutItem item = (DualTabbedLayoutItem)tab_item.Tag;

            // Pass on to IDualTabbedLayoutDragDrop
            IDualTabbedLayoutDragDrop idtldd = item.Content as IDualTabbedLayoutDragDrop;
            if (null != idtldd)
            {
                idtldd.DualTabbedLayoutDrop(sender, e);
            }
        }


#endregion

        private void tab_item_KeyDown(object sender, KeyEventArgs e)
        {
            TabItem tab_item = (TabItem)sender;
            DualTabbedLayoutItem item = (DualTabbedLayoutItem)tab_item.Tag;

            if (e.Key == Key.F4 && KeyboardTools.IsCTRLDown())
            {
                if (item.can_close)
                {
                    item.WantsClose();
                }
                e.Handled = true;
            }

            e.Handled = false;
        }

        private void ClearTabItem(DualTabbedLayoutItem item, TabControl tab_control)
        {
            TabItem tab_item = null;

            foreach (TabItem t in tab_control.Items.OfType<TabItem>())
            {
                if (t.Tag == item)
                {
                    tab_item = t;
                }
            }

            if (null != tab_item)
            {
                tab_control.Items.Remove(tab_item);
                tab_item.Content = null;
                tab_item.Header = null;

                if (0 == tab_control.Items.Count)
                {
                    ReevaluateLayout();
                }
            }
        }

        public List<FrameworkElement> GetAllFrameworkElements()
        {
            List<FrameworkElement> results = new List<FrameworkElement>();
            GetAllTabItems().ForEach(o => results.Add(o.Content));
            return results;
        }

        private List<DualTabbedLayoutItem> GetAllTabItems()
        {
            List<DualTabbedLayoutItem> result = new List<DualTabbedLayoutItem>();
            result.AddRange(GetAllTabItems(tab_control_left));
            result.AddRange(GetAllTabItems(tab_control_right));
            result.AddRange(GetAllTabItems(tab_control_bottom));

            foreach (Window w in floating_windows)
            {
                DualTabbedLayoutItem item = (DualTabbedLayoutItem)w.Tag;
                result.Add(item);
            }

            return result;
        }

        private List<DualTabbedLayoutItem> GetAllTabItems(TabControl tab_control)
        {
            List<DualTabbedLayoutItem> result = new List<DualTabbedLayoutItem>();

            foreach (TabItem tab_item in tab_control.Items.OfType<TabItem>())
            {
                DualTabbedLayoutItem item = (DualTabbedLayoutItem)tab_item.Tag;
                result.Add(item);
            }

            return result;
        }

        private DualTabbedLayoutItem FindTabItem(string reference_key, bool make_visible)
        {
            DualTabbedLayoutItem item = null;
            if (null == item) item = FindTabItem(reference_key, tab_control_left, make_visible);
            if (null == item) item = FindTabItem(reference_key, tab_control_right, make_visible);
            if (null == item) item = FindTabItem(reference_key, tab_control_bottom, make_visible);
            return item;
        }

        private DualTabbedLayoutItem FindTabItem(string reference_key, TabControl tab_control, bool make_visible)
        {
            foreach (TabItem tab_item in tab_control.Items.OfType<TabItem>())
            {
                DualTabbedLayoutItem item = (DualTabbedLayoutItem)tab_item.Tag;

                if (null != reference_key && 0 == reference_key.CompareTo(item.reference_key))
                {
                    if (make_visible)
                    {
                        MarkAsRecentlyUsed(item);
                        tab_item.IsSelected = true;
                    }
                    return item;
                }
            }
            return null;
        }

        public delegate Window GetWindowOverrideDelegate();
        private static GetWindowOverrideDelegate _GetWindowOverride = null;
        public static GetWindowOverrideDelegate GetWindowOverride
        {
            get => _GetWindowOverride;
            set
            {
                if (null != _GetWindowOverride)
                {
                    Logging.Warn("Setting the DualTabbedLayout.GetWindowOverride more than once...");
                }

                _GetWindowOverride = value;
            }
        }

        private Window BuildFloating(DualTabbedLayoutItem item)
        {
            // Get a new window for this
            Window window = (null == GetWindowOverride) ? new Window() : GetWindowOverride();

            // Set the window parameters
            window.AllowsTransparency = false;
            window.WindowStyle = WindowStyle.None;
            window.Tag = item;
            window.Title = item.header;
            window.Icon = window_icon;
            window.Closing += window_Closing;
            window.KeyDown += window_KeyDown;

            DockPanel dock_panel = new DockPanel();

            AugmentedSpacer spacer = new AugmentedSpacer();
            ItemHeader item_header = new ItemHeader(item);
            WindowControlsHeader controls_header = new WindowControlsHeader(item, window);

            DockPanel title_bar_central_space = new DockPanel();
            title_bar_central_space.Background = Brushes.Transparent;
            title_bar_central_space.Tag = window;
            title_bar_central_space.MouseDown += dock_panel_title_bar_MouseDown;

            DockPanel dock_panel_title_bar = new DockPanel();
            dock_panel_title_bar.Height = 30;
            dock_panel_title_bar.Background = ThemeColours.Background_Brush_Blue_VeryDark;

            DockPanel.SetDock(spacer, Dock.Left);
            DockPanel.SetDock(item_header, Dock.Left);
            DockPanel.SetDock(controls_header, Dock.Right);

            dock_panel_title_bar.Children.Add(spacer);
            dock_panel_title_bar.Children.Add(item_header);
            dock_panel_title_bar.Children.Add(controls_header);
            dock_panel_title_bar.Children.Add(title_bar_central_space);

            DockPanel.SetDock(dock_panel_title_bar, Dock.Top);
            dock_panel.Children.Add(dock_panel_title_bar);
            dock_panel.Children.Add(item.content);

            window.Content = dock_panel;
            return window;
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            Window window = (Window)sender;
            DualTabbedLayoutItem item = (DualTabbedLayoutItem)window.Tag;
            if (e.Key == Key.F4 && (KeyboardTools.IsCTRLDown() || KeyboardTools.IsALTDown()))
            {
                if (item.can_close)
                {
                    item.WantsClose();
                }
                e.Handled = true;
            }
        }

        private void window_Closing(object sender, CancelEventArgs e)
        {
            // Only allow the user to close it from our X buttons
            Window window = (Window)sender;
            if (null != window.Tag)
            {
                e.Cancel = true;
            }
        }

        private void ClearFloating(DualTabbedLayoutItem item)
        {
            Window window = null;

            foreach (Window w in floating_windows)
            {
                if (w.Tag == item)
                {
                    window = w;
                }
            }

            if (null != window)
            {
                DockPanel sp = (DockPanel)window.Content;

                floating_windows.Remove(window);
                window.Tag = null;
                window.Close();

                if (null != sp)
                {
                    sp.Children.Clear();
                }
            }
        }

        private DualTabbedLayoutItem FindFloating(string reference_key, bool make_visible)
        {
            foreach (Window w in floating_windows)
            {
                DualTabbedLayoutItem item = (DualTabbedLayoutItem)w.Tag;
                if (null != reference_key && 0 == reference_key.CompareTo(item.reference_key))
                {
                    if (make_visible)
                    {
                        MarkAsRecentlyUsed(item);
                        if (w.WindowState == WindowState.Minimized) w.WindowState = WindowState.Normal;
                        w.Focus();
                    }
                    return item;
                }
            }
            return null;
        }

        private static void dock_panel_title_bar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement item_header = (FrameworkElement)sender;
            Window window = (Window)item_header.Tag;
            WindowTools.CauseWindowToBeDragged(window);
        }


        internal void MarkAsRecentlyUsed(DualTabbedLayoutItem item)
        {
            // only add item when it isn't the last one already:
            var top = (recently_used_items.Any() ? recently_used_items.Last() : null);

            if (top != item)
            {
                recently_used_items.Add(item);

                if (recently_used_items.Count > 100)
                {
                    recently_used_items.RemoveAt(0);
                }
            }
        }

        public FrameworkElement CurrentActiveTabItem
        {
            get
            {
                if (recently_used_items.Any())
                {
                    DualTabbedLayoutItem item = recently_used_items.Last();
                    return item.Content;
                }
                else
                {
                    return null;
                }
            }
        }

#region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            DualTabbedLayout dtl = new DualTabbedLayout();

            AugmentedButton button = new AugmentedButton();
            button.Tag = dtl;
            button.Caption = "Create";
            button.Click += button_Click;
            button.DragOver += button_DragOver;

            dtl.AddContent(null, "Forward", Icons.GetAppIcon(Icons.Forward), false, true, button);
            dtl.AddContent(null, "Backward", Icons.GetAppIcon(Icons.Back), true, true, new TextBlock { Text = "Backwards is the way!" });

            UserControl control = new UserControl();
            control.Content = dtl;
            ControlHostingWindow window = new ControlHostingWindow("DualTabbedLayout", control);
            dtl.OwnerWindow = window;
            dtl.WindowIcon = Icons.GetAppIconICO(Icons.Qiqqa);
            window.Show();
        }
#endif

#endregion

        private static void button_DragOver(object sender, DragEventArgs e)
        {
            Logging.Debug特("BUTTON DRAG OVER");

            e.Handled = true;
        }

        private static void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DualTabbedLayout dtl = (DualTabbedLayout)button.Tag;
            dtl.AddContent(null, "Child", Icons.GetAppIcon(Icons.Camera), true, true, new TextBlock { Text = "Child!" });
        }
    }
}

#endif
