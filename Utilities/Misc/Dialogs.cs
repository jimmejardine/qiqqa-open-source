#if false

using System.Windows.Forms;

namespace Utilities.Misc
{
    public class Dialogs
    {
        public static bool AskConfirmation(Form owner, string question)
        {
            return DialogResult.Yes == MessageBox.Show(owner, question, "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}

#endif
