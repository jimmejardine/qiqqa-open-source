#if !HAS_NO_GUI

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Clipboard = System.Windows.Clipboard;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using TextDataFormat = System.Windows.TextDataFormat;

namespace Utilities
{
    public class ClipboardTools
    {
        [DllImport("user32.dll")]
        extern static IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll")]
        extern static int GetWindowText(int hwnd, StringBuilder text, int count);

        public static void SetText(string selected_text)
        {
            SetText(selected_text, TextDataFormat.UnicodeText);
        }

        public static void SetText(string selected_text, TextDataFormat textDataFormat)
        {
            for (int i = 0; i < 3; ++i)
            {
                try
                {
                    // Try set the text - and if we succeed, exit
                    Clipboard.Clear();
                    Clipboard.SetText(selected_text, textDataFormat);
                    return;
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem setting the clipboard text, so trying again.");
                    Thread.Sleep(250);
                }
            }

            // If we get here, report who has locked the clipboard...
            try
            {
                IntPtr hwnd = GetOpenClipboardWindow();
                StringBuilder sb = new StringBuilder(501);
                GetWindowText(hwnd.ToInt32(), sb, 500);
                string msg = String.Format("Process '{0}' has locked the clipboard and won't release it.", sb.ToString());
                Logging.Warn(msg);
                throw new Exception(msg);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Something has locked the clipboard, and there was a problem find out what...");
            }
        }

        public static void SetRtf(string text_rtf)
        {

            // Also convert the RTF to normal text
            using (RichTextBox rich_text_box = new RichTextBox())
            {
                rich_text_box.Rtf = text_rtf;
                string text_plain = rich_text_box.Text;

                DataObject data = new DataObject();
                data.SetData(DataFormats.Rtf, text_rtf);
                data.SetData(DataFormats.Text, text_plain);

                for (int i = 0; i < 3; ++i)
                {
                    try
                    {
                        // Try set the text - and if we succeed, exit
                        Clipboard.Clear();
                        Clipboard.SetDataObject(data, true);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "There was a problem setting the clipboard text, so trying again.");
                        Thread.Sleep(250);
                    }
                }

                // If we get here, try one last time!
                Clipboard.Clear();
                Clipboard.SetDataObject(data, true);
            }
        }

        public static string GetText()
        {
            return Clipboard.GetText();
        }
    }
}

#endif
