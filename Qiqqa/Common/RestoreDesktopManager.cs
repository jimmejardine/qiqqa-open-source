using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls;
using Qiqqa.Main;
using Utilities;

namespace Qiqqa.Common
{
    class RestoreDesktopManager
    {
        public static void SaveDesktop()
        {
            List<string> restore_settings = new List<string>();

            // Get the remembrances
            List<FrameworkElement> framework_elements = MainWindowServiceDispatcher.Instance.MainWindow.DockingManager.GetAllFrameworkElements();
            foreach (FrameworkElement framework_element in framework_elements)
            {
                {
                    LibraryControl library_control = framework_element as LibraryControl;
                    if (null != library_control)
                    {
                        Logging.Info("Remembering a library control " + library_control.Library.WebLibraryDetail.Id);
                        restore_settings.Add(String.Format("PDF_LIBRARY,{0}", library_control.Library.WebLibraryDetail.Id));
                    }
                }

                {
                    PDFReadingControl pdf_reading_control = framework_element as PDFReadingControl;
                    if (null != pdf_reading_control)
                    {
                        Logging.Info("Remembering a PDF reader " + pdf_reading_control.PDFRendererControlStats.pdf_document.Fingerprint);
                        restore_settings.Add(String.Format("PDF_DOCUMENT,{0},{1}", pdf_reading_control.PDFRendererControlStats.pdf_document.Library.WebLibraryDetail.Id, pdf_reading_control.PDFRendererControlStats.pdf_document.Fingerprint));
                    }
                }
            }

            // Store the remembrances
            File.WriteAllLines(Filename, restore_settings);
        }

        public static void RestoreDesktop()
        {
            try
            {
                // Get the remembrances
                if (File.Exists(Filename))
                {
                    string[] restore_settings = File.ReadAllLines(Filename);
                    foreach (string restore_setting in restore_settings)
                    {
                        try
                        {
                            if (false) { }

                            else if (restore_setting.StartsWith("PDF_LIBRARY"))
                            {
                                string[] parts = restore_setting.Split(',');
                                string library_id = parts[1];

                                Library library = WebLibraryManager.Instance.GetLibrary(library_id);
                                MainWindowServiceDispatcher.Instance.OpenLibrary(library);
                            }
                            else if (restore_setting.StartsWith("PDF_DOCUMENT"))
                            {
                                string[] parts = restore_setting.Split(',');
                                string library_id = parts[1];
                                string document_fingerprint = parts[2];

                                Library library = WebLibraryManager.Instance.GetLibrary(library_id);
                                PDFDocument pdf_document = library.GetDocumentByFingerprint(document_fingerprint);
                                MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
                            }
                        }

                        catch (Exception ex)
                        {
                            Logging.Warn(ex, "There was an problem restoring desktop with state {0}", restore_setting);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem restoring the saved desktop state.");
            }
        }

        private static string Filename
        {
            get
            {
                return ConfigurationManager.Instance.BaseDirectoryForUser + @"\Qiqqa.restore_desktop";
            }
        }

        public class ScreenSize
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
        internal class StoredLocation
        {
            internal Rect position;
            internal WindowState state;

            internal StoredLocation(double left, double top, double width, double height, WindowState s)
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
                rv.Y = ClipValueToRange(x, screen.X, screen.X + screen.Width);
                rv.Width = ClipValueToRange(w, 0, screen.Width);
                rv.Height = ClipValueToRange(h, 0, screen.Height);
                return rv;
            }

            internal StoredLocation(string location_spec)
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

                Logging.Debug("Screen position and window size parsing {4} to {0},{1},{2},{3} ...", left, top, w, h, location_spec);

                position = ClipBoundsToScreen(left, top, w, h);
            }
        }

        static StoredLocation stored_location;

        internal static void RestoreLocation(MainWindow main_window)
        {
            // https://stackoverflow.com/questions/16100/convert-a-string-to-an-enum-in-c-sharp
            string position = ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup_Position;
            if (String.IsNullOrEmpty(position))
            {
                Logging.Warn("Can't restore screen position from empty remembered location.");
                return;
            }

            try
            {
                StoredLocation loc = new StoredLocation(position);

                Logging.Info("Screen position and window size restoring to x={0},y={1},w={2},h={3},mode={4}", loc.position.X, loc.position.Y, loc.position.Width, loc.position.Height, loc.state);

                Rect pos = loc.position;

                // when we have a valid corner coordinate, do we restore window *position* 
                // (and possibly also *size*):
                if (!Double.IsNaN(pos.X) && !Double.IsNaN(pos.Y))
                {
                    main_window.Left = pos.X;
                    main_window.Top = pos.Y;

                    if (!Double.IsNaN(pos.Width) && !Double.IsNaN(pos.Height))
                    {
                        main_window.Width = pos.Width;
                        main_window.Height = pos.Height;
                    }
                }

                main_window.WindowState = loc.state;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem restoring screen position to {0}", position);
            }
        }

        internal static void SaveLocation(MainWindow main_window)
        {
            if (main_window.IsMeasureValid && main_window.IsInitialized)
            {
                Rect rc = new Rect();

                if (main_window.WindowState != WindowState.Normal)
                {
                    rc = main_window.RestoreBounds;
                }
                else
                {
                    rc.X = main_window.Left;
                    rc.Y = main_window.Top;
                    rc.Width = main_window.ActualWidth;
                    rc.Height = main_window.ActualHeight;
                }
                string position = String.Format("{0}|{1}|{2}|{3}|{4}", rc.X, rc.Y, rc.Width, rc.Height, main_window.WindowState);
                ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup_Position = position;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup_Position);
                Logging.Info("Screen position stored as {0}", position);
            }
            else
            {
                Logging.Info("Screen position not valid yet; not stored in the configuration to prevent corruption");
            }
        }
    }
}
