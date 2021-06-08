using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using icons;
using Utilities;
using Utilities.GUI;
using Utilities.Shutdownable;
using WpfScreenHelper;

namespace Qiqqa.Common.GUI
{
#if false
    public static class ScreenSize
    {
        private static Rect? screen_bounds = null;

        public static Rect Bounds
        {
            get
            {
                if (null == screen_bounds)
                {
                    System.Windows.Forms.Screen[] user_hw_screens = System.Windows.Forms.Screen.AllScreens;
                    System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);

                    foreach (System.Windows.Forms.Screen scr in user_hw_screens)
                    {
                        System.Drawing.Rectangle b = scr.WorkingArea;
                        bounds = System.Drawing.Rectangle.Union(bounds, b);
                    }

                    System.Windows.Media.Matrix matrix;
                    using (HwndSource src = new HwndSource(new HwndSourceParameters()))
                    {
                        matrix = src.CompositionTarget.TransformToDevice;
                    }

                    double x = bounds.X / matrix.M11;
                    double y = bounds.Y / matrix.M22;
                    double w = bounds.Width / matrix.M11;
                    double h = bounds.Height / matrix.M22;

                    screen_bounds = new Rect(x, y, w, h);
                }
                return (Rect)screen_bounds;
            }
        }
    }
#endif

    // Left/Top/Width/Height/State:
    public class StoredLocation
    {
        public Rect position;
        public WindowState state;

        public StoredLocation(double left, double top, double width, double height, WindowState s)
        {
            position = new Rect(left, top, width, height);
            state = s;
        }

        public StoredLocation(string location_spec)
        {
            string[] positions = location_spec.Split('|');

            double left = Double.Parse(positions[0]);
            double top = Double.Parse(positions[1]);
            double w = Double.NaN;
            double h = Double.NaN;

            state = WindowState.Normal;

            if (positions.Length >= 4)
            {
                w = Double.Parse(positions[2]);
                h = Double.Parse(positions[3]);

                if (positions.Length == 5)
                {
                    WindowState s = WindowState.Normal;
                    if (Enum.TryParse(positions[4], out s))
                    {
                        state = s;
                    }
                    else
                    {
                        Logging.Error("Invalid WindowState: {0} in screen position spec {1}", positions[4], location_spec);
                    }
                }
            }

            Logging.Debug特("Screen position and window size parsing {4} to {0},{1},{2},{3} ...", left, top, w, h, location_spec);

            position = new Rect(left, top, w, h);
        }
    }

    public class StandardWindow : Window
    {
        public StandardWindow()
        {
            Theme.Initialize();

            Background = ThemeColours.Background_Brush_Blue_LightToDark;
            FontFamily = ThemeTextStyles.FontFamily_Standard;
            Icon = Icons.GetAppIconICO(Icons.Qiqqa);

#if false
            // Set dimensions
            double w = Width;
            double h = Height;
            double width = Math.Max(800, SystemParameters.FullPrimaryScreenWidth - 20);
            double height = Math.Max(600, SystemParameters.FullPrimaryScreenHeight - 20);
            Width = width;
            Height = height;
            Top = 10;
            Left = 10;
#endif

            UseLayoutRounding = true;

            if (SnapToPixels)
            {
                Logging.Info("Snapping to device pixels.");
                SnapsToDevicePixels = true;
            }

            this.ContentRendered += StandardWindow_ContentRendered;
            this.Initialized += StandardWindow_Initialized;
            this.Loaded += StandardWindow_Loaded;
            Closed += StandardWindow_Closed;
            Closing += StandardWindow_Closing;

            // keep a weak reference to this window instance to prevent GC hold-up and thus memory leaks:
            StandardWindowShutdownCbInstance.RegisterShutdownHandler(new StandardWindowShutdownCbInstance(this));
        }

        private Size xaml_preferred_dimensions;

        private void StandardWindow_Initialized(object sender, EventArgs e)
        {
            // when we get here, the XAML has been loaded, but there's been no rendering or positioning yet.
            // That means Width and Height are NaN **unless** the designer has set a preferred width and/or height
            // in the XAML resource itself!
            Logging.Info($"Window: {Name} @ {Left}/{Top}/{Width}/{Height}");
            xaml_preferred_dimensions = new Size(Width, Height);
        }

        private void StandardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // when we get here, basic positioning has been done. Time to overide any default positioning
            // with the settings stored by the user during a previous run:
            Logging.Info($"Window: {Name} @ {Left}/{Top}/{Width}/{Height}");
            SetupConfiguredDimensions(xaml_preferred_dimensions);
        }

        private void StandardWindow_ContentRendered(object sender, EventArgs e)
        {
            Logging.Info($"Window: {Name} @ {Left}/{Top}/{Width}/{Height}");
        }

        public virtual void ShuttingDown()
        {
            Logging.Info($"Window '{Name}' is saving its position/size at shutdown");
            SaveWindowDimensions();
        }

        private void StandardWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save window dimensions and position as persistent preference:
            SaveWindowDimensions();
        }

        private void StandardWindow_Closed(object sender, EventArgs e)
        {
            // weak reference, so not to worry about this:
            //ShutdownableManager.Instance.UnRegister(sd.HandleShutDownForWindow);

            IDisposable disposable = Content as IDisposable;

            disposable?.Dispose();

            Content = null;
        }

        public void SaveWindowDimensions()
        {
            if (!Configuration.ConfigurationManager.IsInitialized)
            {
                // silently ignore: we haven't started up properly yet.
            }
            else if (IsMeasureValid && IsInitialized && IsLoaded)
            {
                string name_to_find = Name;
                Debug.Assert(!String.IsNullOrEmpty(name_to_find));

                if (String.IsNullOrEmpty(name_to_find))
                {
                    return;
                }

                Rect rc = Rect.Empty;

                if (WindowState != WindowState.Normal)
                {
                    rc = RestoreBounds;
                }
                if (rc.IsEmpty)
                {
                    rc = new Rect(Left, Top, Width, Height);
                }
                string position = String.Format("{0}|{1}|{2}|{3}|{4}", rc.X, rc.Y, rc.Width, rc.Height, WindowState);

                // Format: Name=X|Y|W|H|M::...
                string cfg = Configuration.ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup_Position;

                bool got_it = false;

                List<string> cfgarr;
                if (!String.IsNullOrWhiteSpace(cfg))
                {
                    cfgarr = new List<string>(cfg.Split(new string[] { "::" }, StringSplitOptions.None));

                    for (int i = 0; i < cfgarr.Count; i++)
                    {
                        string[] wincfg = cfgarr[i].Split('=');

                        if (wincfg[0] == name_to_find)
                        {
                            // replace this part with the new data:
                            cfgarr[i] = String.Format("{0}={1}", name_to_find, position);
                            got_it = true;
                        }
                        // validation of other records:
                        else if (0 != wincfg[0].IndexOfAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray()))
                        {
                            // bad record: nuke it!
                            cfgarr[i] = "";
                        }
                    }
                }
                else
                {
                    cfgarr = new List<string>();
                }

                // filter out bad/empty records:
                {
                    var cfgarr2 = new List<string>();

                    for (int i = 0; i < cfgarr.Count; i++)
                    {
                        if (!String.IsNullOrWhiteSpace(cfgarr[i]))
                        {
                            cfgarr2.Add(cfgarr[i]);
                        }
                    }
                    cfgarr = cfgarr2;
                }

                if (!got_it)
                {
                    cfgarr.Add(String.Format("{0}={1}", name_to_find, position));
                }
                // and sort + re-bundle the config string:
                cfgarr.Sort();
                StringBuilder rv = new StringBuilder();
                for (int i = 0; i < cfgarr.Count; i++)
                {
                    rv.AppendFormat("::{0}", cfgarr[i]);
                }

                Configuration.ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup_Position = rv.ToString();
                Logging.Info("Screen position for window {0} stored as {1}", Name, position);
            }
            else
            {
                Logging.Info("Screen position not valid yet; not stored in the configuration to prevent corruption");
            }
        }

        public virtual bool SetupConfiguredDimensions(Size preferred_dimensions)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            string name_to_find = Name;
            Debug.Assert(!String.IsNullOrEmpty(name_to_find));
            bool done = false;

            if (!String.IsNullOrEmpty(name_to_find))
            {
                if (Configuration.ConfigurationManager.IsInitialized)
                {
                    if (Configuration.ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup)
                    {
                        // Format: Name=X|Y|W|H|M::...
                        string cfg = Configuration.ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup_Position ?? "";
                        List<string> cfgarr = new List<string>(cfg.Split(new string[] { "::" }, StringSplitOptions.None));
                        string position = String.Empty;

                        for (int i = 0; i < cfgarr.Count; i++)
                        {
                            string[] wincfg = cfgarr[i].Split('=');

                            if (wincfg[0] == name_to_find)
                            {
                                position = wincfg[1];
                                break;
                            }
                        }

                        // https://stackoverflow.com/questions/16100/convert-a-string-to-an-enum-in-c-sharp
                        if (String.IsNullOrEmpty(position))
                        {
                            Logging.Warn("Can't restore screen position for window {0} from empty remembered location.", Name);
                        }
                        else
                        {
                            try
                            {
                                StoredLocation loc = new StoredLocation(position);

                                Logging.Info("Window {5}: Screen position and window size restoring to x={0},y={1},w={2},h={3},mode={4}", loc.position.X, loc.position.Y, loc.position.Width, loc.position.Height, loc.state, Name);

                                Rect pos = loc.position;

                                // when we have a valid corner coordinate, do we restore window *position*
                                // (and possibly also *size*):
                                if (!Double.IsNaN(pos.X) && !Double.IsNaN(pos.Y))
                                {
                                    Left = pos.X;
                                    Top = pos.Y;

                                    if (!Double.IsNaN(pos.Width) && !Double.IsNaN(pos.Height))
                                    {
                                        Width = pos.Width;
                                        Height = pos.Height;
                                    }
                                    done = true;

                                    // only restore WindowState when the (restore-)coordinates are valid too!
                                    WindowState = loc.state;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logging.Warn(ex, "There was a problem restoring screen position to {0} for window {1}", position, Name);
                                done = false;
                            }
                        }
                    }
                }
            }

            // Check that we actually are fitting on the user's display area
            //
            // https://stackoverflow.com/questions/1317235/c-get-complete-desktop-size
            // https://stackoverflow.com/questions/2704887/is-there-a-wpf-equaivalent-to-system-windows-forms-screen/2707176#2707176
            // https://stackoverflow.com/questions/1918877/how-can-i-get-the-dpi-in-wpf
            // https://github.com/micdenny/WpfScreenHelper
            List<Screen> scrns = Screen.AllScreens;
            Rect bounds = scrns[0].Bounds;
            scrns.RemoveAt(0);

            foreach (Screen screen in scrns)
            {
                bounds = Rect.Union(bounds, screen.Bounds);
            }

            if (Width > SystemParameters.FullPrimaryScreenWidth)
            {
                Width = SystemParameters.FullPrimaryScreenWidth;
            }
            if (Height > SystemParameters.FullPrimaryScreenHeight)
            {
                Height = SystemParameters.FullPrimaryScreenHeight;
            }

            // may be PARTLY positioned outside the visible area:
            double MINIMUM_VISIBLE_PART = Math.Min(100, Math.Min(Width, Height));
            if (Left + MINIMUM_VISIBLE_PART > bounds.Width)
            {
                Left = bounds.Width - MINIMUM_VISIBLE_PART;
            }
            if (Left + Width - MINIMUM_VISIBLE_PART < 0)
            {
                Left = MINIMUM_VISIBLE_PART - Width;
            }
            if (Top + MINIMUM_VISIBLE_PART > bounds.Height)
            {
                Top = bounds.Height - MINIMUM_VISIBLE_PART;
            }
            if (Top + Height - MINIMUM_VISIBLE_PART < 0)
            {
                Top = MINIMUM_VISIBLE_PART - Height;
            }

            return done;
        }

        //private static bool _SnapToPixels = true;
        public static bool SnapToPixels
        {
            get
            {
                if (!Configuration.ConfigurationManager.IsInitialized)
                {
                    // silently ignore: we haven't started up properly yet.
                    return true;
                }
                return Configuration.ConfigurationManager.Instance.ConfigurationRecord.SnapToPixels;
            }
            set
            {
                if (!Configuration.ConfigurationManager.IsInitialized)
                {
                    // silently ignore: we haven't started up properly yet.
                }
                Configuration.ConfigurationManager.Instance.ConfigurationRecord.SnapToPixels = value;
            }
        }
    }

    public class StandardWindowShutdownCbInstance
    {
        private WeakReference<StandardWindow> obj = null;

        public StandardWindowShutdownCbInstance(StandardWindow w)
        {
            obj = new WeakReference<StandardWindow>(w);
        }

        public void HandleShutDownForWindow()
        {
            if (obj.TryGetTarget(out StandardWindow w) && w != null)
            {
                w.ShuttingDown();
            }
        }

        public static void RegisterShutdownHandler(StandardWindowShutdownCbInstance self)
        {
            ShutdownableManager.Instance.Register(self.HandleShutDownForWindow);
        }
    }

    internal class StandardWindowReference
    {
        public WeakReference<StandardWindow> window;
        public string type;
    }

    public delegate StandardWindow StandardWindowCreatorFunction();

    public class StandardWindowFactory
    {
        private static List<StandardWindowReference> windows;
        private static object windows_lock = new object();

        static StandardWindowFactory()
        {
            windows = new List<StandardWindowReference>();
        }

        // parameters: nameof(class)
        internal static StandardWindowReference Spot(string name)
        {
            StandardWindowReference spot = null;

            lock (windows_lock)
            {
                foreach (var el in windows)
                {
                    if (el.type == name)
                    {
                        spot = el;
                        break;
                    }
                }
            }

            return spot;
        }

        public static bool Has(string name)
        {
            StandardWindowReference spot = Spot(name);

            StandardWindow instance;
            return (spot != null && spot.window.TryGetTarget(out instance) && instance != null);
        }

        public static StandardWindow Create(string name, StandardWindowCreatorFunction maker)
        {
            StandardWindowReference spot = Spot(name);

            StandardWindow instance;
            if (spot != null && spot.window.TryGetTarget(out instance) && instance != null)
            {
                return instance;
            }
            else
            {
                // got to create an instance (and store it!)
                instance = maker();
                if (spot == null)
                {
                    spot = new StandardWindowReference()
                    {
                        window = new WeakReference<StandardWindow>(instance),
                        type = name
                    };

                    lock (windows_lock)
                    {
                        windows.Add(spot);
                    }
                }
                else
                {
                    // update spot:
                    lock (windows_lock)
                    {
                        spot.window.SetTarget(instance);
                    }
                }
                return instance;
            }
        }
    }
}

