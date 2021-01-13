using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.DateTimeTools;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Shutdownable;

namespace Qiqqa.Common.MessageBoxControls
{
    /// <summary>
    /// Interaction logic for UnhandledExceptionMessageBox.xaml
    /// </summary>
    public partial class UnhandledExceptionMessageBox : StandardWindow
    {
        private UnhandledExceptionMessageBox()
        {
            InitializeComponent();

            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Normal;

            Icon = Icons.GetAppIconICO(Icons.Qiqqa);

            ObjImage.Stretch = Stretch.Fill;
            ObjImage.Source = Backgrounds.GetBackground(Backgrounds.ExceptionDialogBackground);

            TextComments.Focus();
        }

        public static void DisplayException(Exception ex)
        {
            // When we're looking at an OutOfMem exception, there's nothing we can do but abort everything!
            if (ex is System.OutOfMemoryException)
            {
                throw new OutOfMemoryException("Out of memory", ex);
            }

            string useful_text_heading = "Something unexpected has happened, but it's okay. " + ex.Message;
            string useful_text_subheading = "You can continue working, but we would appreciate it if you would send us some feedback on what you were doing when this happened.";

            // should we display a better message
            // TODO: make this a bit neater
            if (ex != null)
            {
                //  very broad descriptions based on the exceptions we caught
                if (ex.GetType() == typeof(PathTooLongException))
                {
                    useful_text_heading = "Maximum path length exceeded";
                    useful_text_subheading = "A very long path was entered (more than 260 characters).  If you were importing documents, please move the documents into a folder with a shorter path before retrying the import.";
                }
            }

            // Check if this is an exception we know about and want to be a little more friendly
            UsefulTextException ute = ex as UsefulTextException;
            if (null != ute)
            {
                useful_text_heading = ute.header;
                useful_text_subheading = ute.body;
            }

            Logging.Error(ex, "Unhandled Exception Handler: {0} - {1}", useful_text_heading, useful_text_subheading);

            // make sure we're not in the process of shutting down Qiqqa for then the next code chunk will cause a (recursive) CRASH:
            if (ShutdownableManager.Instance.IsShuttingDown)
            {
                Logging.Error(ex, "Unhandled Exception Handler: detected Qiqqa shutting down.");
            }
            else
            {
                // Record this exception at server so that we know about it
                {
                    string message = ex.Message;
                    string stack_trace = ex.StackTrace;
                    int MAX_CHARS = 512;
                    if (null != stack_trace && stack_trace.Length > MAX_CHARS)
                    {
                        stack_trace = stack_trace.Substring(0, MAX_CHARS - 3) + "...";
                    }

                    FeatureTrackingManager.Instance.UseFeature(
                        Features.Exception,
                        "Ver", ClientVersion.CurrentVersion,
                        "Message", message,
                        "Stack", stack_trace
                        );
                }

                Display("Unexpected problem in Qiqqa!", useful_text_heading, useful_text_subheading, null, true, false, ex);
            }
        }

        public static void DisplayInfo(string useful_text, string useful_text_subheading, bool display_faq_link, Exception ex)
        {
            // make sure we're not in the process of shutting down Qiqqa for then the next code chunk will cause a (recursive) CRASH:
            if (ShutdownableManager.Instance.IsShuttingDown)
            {
                Logging.Error(ex, "Unhandled Exception Handler: detected Qiqqa shutting down.");
            }
            else
            {
                Display(
                    "Qiqqa Information",
                    useful_text,
                    useful_text_subheading,
                    "",
                    false,
                    display_faq_link,
                    ex);
            }
        }

        private static void Display(string title, string useful_text_heading, string useful_text_subheading, string comments_label, bool display_exception_section, bool display_faq_link, Exception ex)
        {
            try
            {
                Logging.Info(string.Format("About to display client stats: {0}", useful_text_heading), ex);

                if (!ShutdownableManager.Instance.IsShuttingDown)
                {
                    WPFDoEvents.InvokeInUIThread(() =>
                    {
                        UnhandledExceptionMessageBox mb = new UnhandledExceptionMessageBox();
                        mb.Title = title;
                        mb.PopulateText(useful_text_heading, useful_text_subheading, comments_label, display_faq_link);
                        mb.PopulateException(ex, display_exception_section);
                        mb.PopulateLog();
                        mb.PopulateMachineStats();
                        mb.ShowDialog();
                    });
                }
                else
                {
                    throw new Exception(String.Format("Unhandled Exception Display while shutting down:\nHeading: {0}\nOriginal Exception: {1}", useful_text_heading, ex), ex);
                }
            }
            catch (Exception ex2)
            {
                Logging.Error(ex2, "There was an error while trying to display an UnhandledExceptionMessageBox.  Bailing so that we don't get an infinite-recurse!");
            }
        }

        private void PopulateText(string useful_text_heading, string useful_text_subheading, string comments_label, bool display_faq_link)
        {
            UsefulTextHeading.Text = useful_text_heading;
            UsefulTextSubheading.Text = useful_text_subheading;
            if (!string.IsNullOrEmpty(comments_label))
            {
                TextCommentsLabel.Text = comments_label;
            }
            if (display_faq_link)
            {
                FaqText.Visibility = Visibility.Visible;
            }
        }

        private void PopulateException(Exception ex, bool display_exception_section)
        {
            if (display_exception_section)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateFormatter.asYYYYMMDDHHMMSS(DateTime.UtcNow) + ":");

                Exception current_exception = ex;
                while (null != current_exception)
                {
                    sb.AppendLine(current_exception.ToString());
                    sb.AppendLine();
                    sb.AppendLine("--------------------------------------------");
                    sb.AppendLine();

                    current_exception = current_exception.InnerException;
                }
                TextExceptions.Text = sb.ToString();
                TextExceptionSummary.Text = ex.Message;
            }
            else
            {
                TextExceptionsRegion.Visibility = Visibility.Collapsed;
            }
        }

        private void PopulateLog()
        {
            string log_filename = Logging.GetLogFilename();

            try
            {
                List<string> log_lines = new List<string>();
                using (FileStream fs = new FileStream(log_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (true)
                        {
                            string log_line = sr.ReadLine();
                            if (null == log_line) break;
                            log_lines.Add(log_line);
                        }
                    }
                }

                int NUM_LINES_TO_USE = 200;
                int start_pos = Math.Max(0, log_lines.Count - NUM_LINES_TO_USE);

                StringBuilder sb = new StringBuilder();
                for (int i = start_pos; i < log_lines.Count; ++i)
                {
                    sb.AppendLine(log_lines[i]);
                }

                TextLogs.Text = sb.ToString();
                TextLogs.ScrollToEnd();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem opening the logfile for display in the UnhandledExceptionMessageBox");
            }
        }

        private void PopulateMachineStats()
        {
            TextMachineStats.Text = ComputerStatistics.GetCommonStatistics(ConfigurationManager.GetCurrentConfigInfos());
        }

        private string FaqUrl => WebsiteAccess.GetOurUrl(WebsiteAccess.OurSiteLinkKind.Faq);

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST_UI
        public static void Test()
        {
            try
            {
                try
                {
                    try
                    {
                        throw new Exception("Inner exception 1");
                    }
                    catch (Exception ex_inner1)
                    {
                        throw new Exception("Inner exception 2", ex_inner1);
                    }
                }
                catch (Exception ex_inner2)
                {
                    throw new Exception("Outer exception", ex_inner2);
                }
            }
            catch (Exception ex_outer)
            {
                 DisplayException(ex_outer);
            }
        }
#endif

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

        }
    }
}
