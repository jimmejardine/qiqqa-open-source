using System;
using System.Windows.Forms;

namespace Utilities.GUI
{
    public class MessageBoxes
    {
        public static void Error(Exception ex, string msg, params object[] args)
        {
            string message = Logging.Error(ex, msg, args);
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Error(string msg, params object[] args)
        {
            string message = Logging.Error(msg, args);
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Warn(string msg, params object[] args)
        {
            string message = Logging.Warn(msg, args);
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Info(string msg, params object[] args)
        {
            string message = Logging.Info(msg, args);
            MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool AskQuestion(string msg, params object[] args)
        {
            return AskErrorQuestion(msg, false, args);
        }

        public static bool AskErrorQuestion(string msg, bool isError, params object[] args)
        {
            string message = String.Format(msg, args);
            DialogResult dialog_result = MessageBox.Show(message, isError? "Problem" :  "Question", MessageBoxButtons.YesNo, 
                isError? MessageBoxIcon.Error : MessageBoxIcon.Question);
            return (dialog_result == DialogResult.Yes);
        }
    }
}
