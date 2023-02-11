using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.GUI;
using Utilities.Shutdownable;

namespace Qiqqa.Common.SpeedRead
{
    /// <summary>
    /// Interaction logic for SpeedReadControl.xaml
    /// </summary>
    public partial class SpeedReadControl : UserControl, IDisposable
    {
        private List<string> words;
        private bool unitIsWPM;
        private double maximumWPM = 1000;

        public SpeedReadControl()
        {
            InitializeComponent();

            unitIsWPM = true;

            WordsPerMinute.IsChecked = true;
            CharactersPerMinute.IsChecked = false;

            SliderWPM.Maximum = maximumWPM;

            SliderLocation.ToolTip = "Current location.  Press PageUp/PageDown to jump 1000 words.";
            TxtWPM.ToolTip = SliderWPM.ToolTip = "Words per minute.  Press Plus/Minus to change this.";
            TextCurrentWord.ToolTip = TextCurrentWordLeft.ToolTip = TextCurrentWordRight.ToolTip = RenderArea.ToolTip = "Press 1-9 to change the font size.";

            ButtonRewind.Icon = Icons.GetAppIcon(Icons.SpeedRead_Backward);
            ButtonRewind.ToolTip = "Rewind 100 words.\n(Backspace)";
            ButtonPlayStop.Icon = Icons.GetAppIcon(Icons.SpeedRead_Play);
            ButtonPlayStop.ToolTip = "Toggle Play/Stop.\n(Space)";
            ButtonSlower.Icon = Icons.GetAppIcon(Icons.SpeedRead_Minus);
            ButtonSlower.ToolTip = "Slow down!\n(Minus)";
            ButtonFaster.Icon = Icons.GetAppIcon(Icons.SpeedRead_Plus);
            ButtonFaster.ToolTip = "Speed up!  Oh yeah!\n(Plus)";

            ButtonPlayStop.Click += ButtonPlayStop_Click;
            ButtonSlower.Click += ButtonSlower_Click;
            ButtonFaster.Click += ButtonFaster_Click;

            ButtonRewind.Click += ButtonRewind_Click;

            PreviewKeyDown += SpeedReadControl_KeyDown;

            CheckPreamble.IsChecked = true;
            CheckPostamble.IsChecked = true;

            WordsPerMinute.Checked += WordsPerMinute_Checked;
            CharactersPerMinute.Checked += CharactersPerMinute_Checked;

            DataContext = ConfigurationManager.Instance.ConfigurationRecord;

            TogglePlayPause();
        }

        private void WordsPerMinute_Checked(object sender, RoutedEventArgs e)
        {
            if (!unitIsWPM)
            {
                double new_value = SliderWPM.Value;
                SliderWPM.Value = Math.Round(new_value / 7);
                SliderWPM.Maximum = maximumWPM;
                TxtWPM.ToolTip = SliderWPM.ToolTip = "Words per minute.  Press Plus/Minus to change this.";
            }
            unitIsWPM = true;
        }

        private void CharactersPerMinute_Checked(object sender, RoutedEventArgs e)
        {
            if (unitIsWPM)
            {
                SliderWPM.Maximum = maximumWPM * 7;
                double new_value = SliderWPM.Value;
                SliderWPM.Value = new_value * 7;
                TxtWPM.ToolTip = SliderWPM.ToolTip = "Characters per minute.  Press Plus/Minus to change this.";
            }
            unitIsWPM = false;
        }

        private void SpeedReadControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Space == e.Key)
            {
                TogglePlayPause();
                e.Handled = true;
            }
            else if (Key.OemPlus == e.Key || Key.Add == e.Key)
            {
                ButtonFaster_Click(null, null);
                e.Handled = true;
            }
            else if (Key.OemMinus == e.Key || Key.Subtract == e.Key)
            {
                ButtonSlower_Click(null, null);
                e.Handled = true;
            }
            else if (Key.Back == e.Key)
            {
                ButtonRewind_Click(null, null);
                e.Handled = true;
            }
            else if (Key.PageDown == e.Key)
            {
                ChangeLocation(+1000);
                e.Handled = true;
            }
            else if (Key.PageUp == e.Key)
            {
                ChangeLocation(-1000);
                e.Handled = true;
            }
            else if (Key.D1 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 22;
                e.Handled = true;
            }
            else if (Key.D2 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 32;
                e.Handled = true;
            }
            else if (Key.D3 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 42;
                e.Handled = true;
            }
            else if (Key.D4 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 52;
                e.Handled = true;
            }
            else if (Key.D5 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 62;
                e.Handled = true;
            }
            else if (Key.D6 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 72;
                e.Handled = true;
            }
            else if (Key.D7 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 82;
                e.Handled = true;
            }
            else if (Key.D8 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 92;
                e.Handled = true;
            }
            else if (Key.D9 == e.Key)
            {
                TextCurrentWord.FontSize = TextCurrentWordLeft.FontSize = TextCurrentWordRight.FontSize = 102;
                e.Handled = true;
            }
        }

        private void ChangeLocation(int delta)
        {
            double new_value = SliderLocation.Value + delta;
            new_value = Math.Min(SliderLocation.Maximum, new_value);
            new_value = Math.Max(SliderLocation.Minimum, new_value);

            SliderLocation.Value = new_value;
        }

        private void ButtonRewind_Click(object sender, RoutedEventArgs e)
        {
            ChangeLocation(-100);
        }

        private void ButtonFaster_Click(object sender, RoutedEventArgs e)
        {
            ChangeWPM(+20);
        }

        private void ButtonSlower_Click(object sender, RoutedEventArgs e)
        {
            ChangeWPM(-20);
        }

        private void ChangeWPM(int delta)
        {
            double new_value = SliderWPM.Value + delta;
            new_value = Math.Min(SliderWPM.Maximum, new_value);
            new_value = Math.Max(SliderWPM.Minimum, new_value);

            SliderWPM.Value = new_value;
        }

        private void ButtonPlayStop_Click(object sender, RoutedEventArgs e)
        {
            TogglePlayPause();
        }

        private bool playing = true;
        public void TogglePlayPause(bool force_stop = false)
        {
            playing = !playing;

            if (force_stop)
            {
                playing = false;
            }

            if (playing)
            {
                ButtonPlayStop.Icon = Icons.GetAppIcon(Icons.SpeedRead_Stop);
                KickOffPlayingThread();
            }
            else
            {
                ButtonPlayStop.Icon = Icons.GetAppIcon(Icons.SpeedRead_Play);
            }
        }

        private Daemon thread = null;

        private void KickOffPlayingThread()
        {
            bool playing_old = playing;

            if (null != thread)
            {
                playing = false;
                thread.Join();
            }

            playing = playing_old;

            thread = new Daemon(daemon_name: "SpeedReader:Player");
            //thread.IsBackground = true;
            //thread.Priority = ThreadPriority.Lowest;
            thread.Start(BackgroundThread, thread);
        }

        private void BackgroundThread(object arg)
        {
            Daemon daemon = (Daemon)arg;

            try
            {
                // NOTE: while one might wonder why `playing` is not protected by a lock, while it
                // is used by two threads: this is by design.
                //
                // `playing` is only ever changed by a single thread (Main) and any temporary collision
                // between reading `playing` while the boolean is being changed by the main thread
                // is NOT A PROBLEM.
                // Yes, an Exception MAY be thrown then, but we have to reckon with failures inside
                // the loop anyway (https://github.com/jimmejardine/qiqqa-open-source/issues/42) as
                // the `playing = false` STOP instruction MAY occur while Dispose()-ing this class
                // instance: *that* action MAY very well happen while we're *somewhere* inside the
                // play loop executing stuff... which will surely CRASH with an Exception, e.g.
                // https://github.com/jimmejardine/qiqqa-open-source/issues/42
                // hence we NEED the Exception handling anyway and any unsafe checking of `playing`
                // is but one potential failure mode, all of whom will be correctly caught by the
                // outer `try...catch`, hence no critical section lock coding effort for `playing`:
                while (playing && !ShutdownableManager.Instance.IsShuttingDown)
                {
                    int current_wpm = 1;

                    // Interrogate the GUI
                    WPFDoEvents.InvokeInUIThread(() =>
                        {
                            current_wpm = Math.Max(1, (int)SliderWPM.Value);
                        }
                    );

                    int current_position = 0;
                    int current_maximum = 0;
                    // Interrogate the GUI
                    WPFDoEvents.InvokeInUIThread(() =>
                        {
                            current_position = (int)SliderLocation.Value;
                            current_maximum = (int)SliderLocation.Maximum;
                        }
                    );

                    // Can we move onto the next word?
                    if (current_position < current_maximum)
                    {
                        string current_word = words[current_position];
                        string current_word_left = "";
                        string current_word_right = "";
                        for (int i = 1; i <= 3; ++i)
                        {
                            if (current_position - i >= 0 && current_position - i < words.Count)
                            {
                                current_word_left += words[current_position - i] + " ";
                            }
                            if (current_position + i >= 0 && current_position + i < words.Count)
                            {
                                current_word_right += " " + words[current_position + i];
                            }
                        }

                        ++current_position;

                        WPFDoEvents.InvokeInUIThread(() =>
                            {
                                SliderLocation.Value = current_position;
                                TextCurrentWord.Text = current_word;
                                TextCurrentWordLeft.Text = current_word_left;
                                TextCurrentWordRight.Text = current_word_right;
                            }
                        );

                        // Sleep a bit to reflect the WPM
                        int sleep_time_ms;
                        if (unitIsWPM)
                        {
                            sleep_time_ms = 60 * 1000 / current_wpm;
                        }
                        else
                        {
                            sleep_time_ms = current_word.Length * 60 * 1000 / current_wpm;
                        }
                        daemon.Sleep(sleep_time_ms);
                    }
                    else
                    {
                        playing = false;
                        WPFDoEvents.InvokeAsyncInUIThread(() =>
                            {
                                TogglePlayPause(force_stop: true);
                            }
                        );
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // all sorts of nasty stuff can happen. If it happens while `Dispose()`
                // was invoked on the mainline, it's hunky-dory. So we merely rate this
                // DEBUG level diagnostics.
                Logging.Error(ex, "Crash in SpeedReader Player.");
            }
        }

        public void UseText(string text)
        {
            char[] separators = { ' ', '\t', '\r', '\n' };

            string[] extracted_words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            UseText(new List<string>(extracted_words));
        }

        public void UseText(List<string> words_)
        {
            words = words_;

            SliderLocation.Minimum = 0;
            SliderLocation.Maximum = words_.Count;
            SliderLocation.Value = 0;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            SpeedReadControl src = new SpeedReadControl();
            ControlHostingWindow window = new ControlHostingWindow("SpeedRead!", src);

            window.Width = 640;
            window.Height= 480;
            window.Show();

            string text = File.ReadAllText(@"c:\temp\alice-en1.txt");
            src.UseText(text);
        }
#endif

        #endregion


        #region --- IDisposable ------------------------------------------------------------------------

        ~SpeedReadControl()
        {
            // Get rid of managed resources and background threads
            playing = false;

            Logging.Debug("~SpeedReadControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            // Get rid of managed resources and background threads
            playing = false;

            Logging.Debug("Disposing SpeedReadControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            // Get rid of managed resources and background threads
            playing = false;

            Logging.Debug("SpeedReadControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    // Get rid of managed resources and background threads
                    playing = false;

                    if (thread != null)
                    {
                        thread.Join();
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    words.Clear();
                });

                WPFDoEvents.SafeExec(() =>
                {
                    DataContext = null;
                });

                ++dispose_count;
            });
        }

        #endregion

    }
}
