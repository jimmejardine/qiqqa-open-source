using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Shutdownable;

namespace Qiqqa.Common.ReadOutLoud
{
    public class ReadOutLoudManager : IDisposable
    {
        public static ReadOutLoudManager Instance = new ReadOutLoudManager();

        // TODO
        //
        // Warning CA1001  Implement IDisposable on 'ReadOutLoudManager' because it creates members 
        // of the following IDisposable types: 'SpeechSynthesizer'. 
        // If 'ReadOutLoudManager' has previously shipped, adding new members that implement IDisposable 
        // to this type is considered a breaking change to existing consumers.

        private object read_out_loud_lock = new object();
        private SpeechSynthesizer speech_synthesizer;
        private Prompt current_prompt;
        private int current_prompt_length;
        private List<string> last_words = new List<string>();

        private ReadOutLoudManager()
        {
            speech_synthesizer = new SpeechSynthesizer();
            speech_synthesizer.SpeakProgress += speech_synthesizer_SpeakProgress;
            speech_synthesizer.SpeakCompleted += speech_synthesizer_SpeakCompleted;
            current_prompt = null;

            ShutdownableManager.Instance.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            Logging.Info("Shutting down ReadOutLoudManager");
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (read_out_loud_lock)
            {
                //l1_clk.LockPerfTimerStop();
                speech_synthesizer.SpeakAsyncCancelAll();
            }
        }

        private void speech_synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            // Add the current word to our list
            last_words.Add(e.Text);
            while (last_words.Count > 10)
            {
                last_words.RemoveAt(0);
                last_words.RemoveAt(0);
                last_words.RemoveAt(0);
                last_words.RemoveAt(0);
                last_words.RemoveAt(0);
            }

            string textwindow = ArrayFormatter.ListElements(last_words, " ");

            StatusManager.Instance.UpdateStatus("ReadOutAloud", textwindow, e.CharacterPosition, current_prompt_length);
        }

        private void speech_synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            StatusManager.Instance.UpdateStatus("ReadOutAloud", "Finished reading page");
        }

        public void Read(string text)
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (read_out_loud_lock)
            {
                //l1_clk.LockPerfTimerStop();
                if (null != current_prompt)
                {
                    speech_synthesizer.SpeakAsyncCancel(current_prompt);
                }

                current_prompt_length = text.Length;
                current_prompt = speech_synthesizer.SpeakAsync(text);
                speech_synthesizer.Resume();
            }
        }

        public void Pause()
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (read_out_loud_lock)
            {
                //l1_clk.LockPerfTimerStop();
                speech_synthesizer.Pause();
            }
        }

        public void Resume()
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (read_out_loud_lock)
            {
                //l1_clk.LockPerfTimerStop();
                speech_synthesizer.Resume();
            }
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~ReadOutLoudManager()
        {
            Logging.Debug("~ReadOutLoudManager()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing ReadOutLoudManager");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("ReadOutLoudManager::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources
                    speech_synthesizer?.Dispose();
                }
                speech_synthesizer = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                current_prompt = null;
                last_words?.Clear();
                last_words = null;
            });

            ++dispose_count;
        }

        #endregion
    }
}
