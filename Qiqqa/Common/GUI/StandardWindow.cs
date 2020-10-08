using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using icons;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Common.GUI
{
    public static class ScreenSize
    {
        private static Rect? screen_bounds = null;

        public static Rect Bounds
        {
            get
            {
                if (null == screen_bounds)
                {
                    //Application app = Application.Current;
                    //Window win = app.MainWindow;
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

    // Left/Top/Width/Height/State:
    public class StoredLocation
    {
        public Rect position;
        public WindowState state;

        public StoredLocation(double left, double top, double width, double height, WindowState s)
        {
            position = StoredLocation.ClipBoundsToScreen(left, top, width, height);
            state = s;
        }

        private static double ClipValueToRange(double v, double min, double max)
        {
            // any out-of-range value will be set to NaN to signal 'not available value'
            if (Double.IsNaN(v))
            {
                return Double.NaN;
            }
            if (v < min)
            {
                return Double.NaN;
            }
            if (v > max)
            {
                return Double.NaN;
            }
            return v;
        }

        private static Rect ClipBoundsToScreen(double x, double y, double w, double h)
        {
            Rect screen = ScreenSize.Bounds;
            Rect rv = new Rect();

            rv.X = ClipValueToRange(x, screen.X, screen.X + screen.Width);
            rv.Y = ClipValueToRange(y, screen.Y, screen.Y + screen.Height);
            rv.Width = ClipValueToRange(w, 0, screen.Width);
            rv.Height = ClipValueToRange(h, 0, screen.Height);
            return rv;
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

            position = ClipBoundsToScreen(left, top, w, h);
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

            // Set dimensions
            double width = Math.Max(800, SystemParameters.FullPrimaryScreenWidth - 20);
            double height = Math.Max(600, SystemParameters.FullPrimaryScreenHeight - 20);
            Width = width;
            Height = height;
            Top = 10;
            Left = 10;

            UseLayoutRounding = true;

            if (RegistrySettings.Instance.IsSet(RegistrySettings.SnapToPixels))
            {
                Logging.Info("Snapping to device pixels.");
                SnapsToDevicePixels = true;
            }

            Closed += StandardWindow_Closed;
        }

        private void StandardWindow_Closed(object sender, EventArgs e)
        {
            // Save window dimensions and position as persistent preference:
            SaveWindowDimensions();

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
            else if (IsMeasureValid && IsInitialized)
            {
                string name_to_find = Name;

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
                // Exception to the rule for backwards compatibility: first record is for main window and has no name.
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

        public virtual bool SetupConfiguredDimensions()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            // Format: Name=X|Y|W|H|M::...
            // Exception to the rule for backwards compatibility: first record is for main window and has no name.
            string cfg = Configuration.ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup_Position ?? "";
            List<string> cfgarr = new List<string>(cfg.Split(new string[] { "::" }, StringSplitOptions.None));
            string position = String.Empty;
            string name_to_find = Name;

            if (String.IsNullOrEmpty(name_to_find))
            {
                return false;
            }
            if (name_to_find == "QiqqaMainWindow")
            {
                name_to_find = "!";
            }

            for (int i = 0; i < cfgarr.Count; i++)
            {
                string[] wincfg = cfgarr[i].Split('=');
                if (0 == i)
                {
                    wincfg = new string[] { "!" /* Main */, wincfg[0] };
                }

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
                return false;
            }

            try
            {
                StoredLocation loc = new StoredLocation(position);

                Logging.Info("Window {5}: Screen position and window size restoring to x={0},y={1},w={2},h={3},mode={4}", loc.position.X, loc.position.Y, loc.position.Width, loc.position.Height, loc.state, Name);

                Rect pos = loc.position;

                bool done = false;

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

                return done;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem restoring screen position to {0} for window {1}", position, Name);
                return false;
            }
        }
    }
}
