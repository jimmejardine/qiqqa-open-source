using System.Collections.Generic;
using System.Speech.Synthesis;
using Utilities;
using Utilities.Collections;
using Utilities.Misc;
using Utilities.Shutdownable;

namespace Qiqqa.Common.ReadOutLoud
{
    public class ReadOutLoudManager
    {
        public static ReadOutLoudManager Instance = new ReadOutLoudManager();

        object read_out_loud_lock = new object();
        SpeechSynthesizer speech_synthesizer;
        Prompt current_prompt;
        int current_prompt_length;

        List<string> last_words = new List<string>();

        private ReadOutLoudManager()
        {
            speech_synthesizer = new SpeechSynthesizer();
            speech_synthesizer.SpeakProgress += speech_synthesizer_SpeakProgress;
            speech_synthesizer.SpeakCompleted += speech_synthesizer_SpeakCompleted;
            current_prompt = null;

            ShutdownableManager.Instance.Register(OnShutdown);
        }

        void OnShutdown()
        {
            Logging.Info("Shutting down ReadOutLoudManager");
            lock (read_out_loud_lock)
            {
                speech_synthesizer.SpeakAsyncCancelAll();
            }
        }

        void speech_synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
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

            string window = ArrayFormatter.ListElements(last_words, " ");

            StatusManager.Instance.UpdateStatus("ReadOutAloud", window, e.CharacterPosition, current_prompt_length);            
        }

        void speech_synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            StatusManager.Instance.UpdateStatus("ReadOutAloud", "Finished reading page");
        }

        public void Read(string text)
        {
            lock (read_out_loud_lock)
            {
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
            lock (read_out_loud_lock)
            {
                speech_synthesizer.Pause();
            }
        }

        public void Resume()
        {
            lock (read_out_loud_lock)
            {
                speech_synthesizer.Resume();
            }
        }
    }
}
