using Qiqqa.Common.Configuration;
using Qiqqa.Main.LoginStuff;
using Qiqqa.UtilisationTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Utilities;
using Utilities.GUI;
using Utilities.Internet;

namespace Qiqqa.Chat
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl, IDisposable
    {
        private static readonly string BASE_URL = WebsiteAccess.Url_ChatAPI4Qiqqa;
        private const int MAX_SLEEP_BACKOFF_SECONDS = 16;

        private Timer timer;
        private string last_epoch = "";

        private DateTime next_autopoll_datetime = DateTime.MinValue;
        private int current_sleep_backoff_seconds = MAX_SLEEP_BACKOFF_SECONDS;

        ChatRecord previous_chat_record = null;
        Paragraph previous_paragraph = null;

        public ChatControl()
        {
            Theme.Initialize();

            InitializeComponent();

            TxtSubmission.PreviewKeyDown += TxtSubmission_PreviewKeyDown;
            TxtSubmission.IsKeyboardFocusedChanged += TxtSubmission_IsKeyboardFocusedChanged;
            TxtSubmissionEmpty.Background = null;
            TxtSubmissionEmpty.IsHitTestVisible = false;
            TxtSubmissionEmpty.Focusable = false;
            TxtSubmissionEmpty.IsEnabled = false;

#if false
            timer = new Timer(OnTimeTick);
            timer.Change(0, 500);
#endif
            }
        
        private string ChatUsername
        {
            get            
            {
                string massaged_username = ConfigurationManager.Instance.ConfigurationRecord.Account_Nickname;
                if (massaged_username.Contains('@'))
                {
                    massaged_username = massaged_username.Substring(0, massaged_username.IndexOf('@'));
                }
                return massaged_username;
            }
        }

        void ReevaluateTxtSubmissionEmptyVisibility()
        {
            System.Windows.Visibility visibility = System.Windows.Visibility.Visible;

            if (!String.IsNullOrEmpty(TxtSubmission.Text)) visibility = System.Windows.Visibility.Collapsed;
            if (TxtSubmission.IsKeyboardFocused) visibility = System.Windows.Visibility.Collapsed;

            TxtSubmissionEmpty.Visibility = visibility;
        }

        void TxtSubmission_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ReevaluateTxtSubmissionEmptyVisibility();
        }


        void TxtSubmission_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ReevaluateTxtSubmissionEmptyVisibility();

            if (Key.Enter == e.Key)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Chat_Submit);

                string comment = TxtSubmission.Text;
                TxtSubmission.Text = "";
                if (String.IsNullOrEmpty(comment)) return;

                // Send the new message
                string url = BASE_URL + "/submit?";
                if (!String.IsNullOrEmpty(last_epoch)) url = url + "epoch=" + last_epoch;
                url = url + "&user=" + Uri.EscapeDataString(ChatUsername);
                url = url + "&msg=" + Uri.EscapeDataString(comment);

                PerformRequest(url);

                e.Handled = true;
            }
        }

        private void PerformRequest(string url)
        {
            bool is_chat_available = false;

            try
            {
                using (MemoryStream ms = UrlDownloader.DownloadWithBlocking(url))
                {
                    ProcessDisplayResponse(ms);
                }

                is_chat_available = true;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem communicating with chat. URL: {0}", url);
                next_autopoll_datetime = DateTime.UtcNow.AddMinutes(1);

                is_chat_available = false;
            }

            // make sure we're not in the process of shutting down Qiqqa for then the next code chunk will cause a CRASH:
            if (null != Application.Current && !Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.IsEnabled = is_chat_available;
                    TxtChatUnavailable.Visibility = is_chat_available ? Visibility.Collapsed : Visibility.Visible;
                }));
            }
            else
            {
                Logging.Warn("Chat: detected Qiqqa shutting down.");
            }
        }

        private void OnTimeTick(object state)
        {
            if (next_autopoll_datetime < DateTime.UtcNow)
            {
                next_autopoll_datetime = DateTime.MaxValue;
                DoRefresh();
            }
        }

        private void DoRefresh()
        {
            string url = BASE_URL + "/?";
            if (!String.IsNullOrEmpty(last_epoch)) url = url + "epoch=" + last_epoch;

            PerformRequest(url);
        }

        class ChatRecord
        {
            public string Timestamp { get; set; }
            public string Forum { get; set; }
            public string Username { get; set; }
            public string Comment { get; set; }            
        }

        private void ProcessDisplayResponse(MemoryStream ms)
        {
            // Process the lines
            using (StreamReader sr = new StreamReader(ms))
			{
	            List<ChatRecord> chat_records = new List<ChatRecord>();
	            {
	                string line;
	                ChatRecord last_chat_record = null;
	                while (null != (line = sr.ReadLine()))
	                {
	                    string[] items = line.Split('\t');
	                    {
	                        last_chat_record = new ChatRecord();
	                        last_chat_record.Timestamp = items[0];
	                        last_chat_record.Forum = items[1];
	                        last_chat_record.Username = items[2];
	                        last_chat_record.Comment = items[3];
	                    }
	                    chat_records.Add(last_chat_record);
	                }

	                // Record the last epoch
	                if (null != last_chat_record)
	                {
	                    last_epoch = last_chat_record.Timestamp;
	                }
	            }

	            // If there has been new chat, poll frequently
	            if (0 < chat_records.Count)
	            {
	                current_sleep_backoff_seconds = 1;
	                next_autopoll_datetime = DateTime.UtcNow.AddSeconds(current_sleep_backoff_seconds);
	            }
	            else
	            {
	                current_sleep_backoff_seconds = Math.Min(MAX_SLEEP_BACKOFF_SECONDS, 2 * current_sleep_backoff_seconds);
	                next_autopoll_datetime = DateTime.UtcNow.AddSeconds(current_sleep_backoff_seconds);
	            }

	            // Update GUI
	            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
	            {
	                bool is_currently_at_scroll_bottom = ObjChatFlowDocumentScroll.VerticalOffset == ObjChatFlowDocumentScroll.ScrollableHeight;

	                foreach (ChatRecord chat_record in chat_records)
	                {
	                    // Is this a different user chatting?
	                    if (null == previous_chat_record || chat_record.Username != previous_chat_record.Username || null == previous_paragraph)
	                    {
	                        previous_paragraph = new Paragraph();                        
	                        ObjChatFlowDocument.Blocks.Add(previous_paragraph);

	                        previous_paragraph.TextAlignment = ToggleTextAlignment();

	                        Run username_run = new Run(chat_record.Username);
	                        username_run.ToolTip = String.Format("{0} ({1})", chat_record.Username, chat_record.Timestamp);
	                        username_run.FontSize = 12;
	                        username_run.FontStyle = FontStyles.Italic;
	                        username_run.Foreground = ThemeColours.Background_Brush_Blue_VeryVeryDark;
	                        username_run.Cursor = Cursors.Pen;
	                        username_run.MouseDown += username_run_MouseDown;                        

	                        previous_paragraph.Inlines.Add(username_run);
	                    }

	                    previous_paragraph.Inlines.Add(new LineBreak());
	                    previous_paragraph.Inlines.Add(new Run(chat_record.Comment));

	                    previous_chat_record = chat_record;
	                }

	                if (is_currently_at_scroll_bottom)
	                {
	                    ObjChatFlowDocumentScroll.ScrollToEnd();
	                }
	            }
	            ));
			}
        }

        void username_run_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Run run = sender as Run;
            TxtSubmission.Text = TxtSubmission.Text + "@" + run.Text + " ";
            TxtSubmission.SelectionStart = TxtSubmission.Text.Length;
            TxtSubmission.SelectionLength = 0;
            Keyboard.Focus(TxtSubmission);

            ReevaluateTxtSubmissionEmptyVisibility();

            e.Handled = true;
        }

        private TextAlignment previous_text_alignment = TextAlignment.Left;
        private TextAlignment ToggleTextAlignment()
        {
            if (TextAlignment.Left == previous_text_alignment)
            {
                previous_text_alignment = TextAlignment.Right;
            }
            else
            {
                previous_text_alignment = TextAlignment.Left;
            }

            return previous_text_alignment;
        }

#region --- IDisposable ------------------------------------------------------------------------

        ~ChatControl()
        {
            Logging.Debug("~ChatControl()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Debug("Disposing ChatControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("ChatControl::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                timer?.Dispose();
            }
            timer = null;

            ++dispose_count;
        }

#endregion

    }
}
