#if false

using System.Windows.Forms;

namespace Utilities.GUI
{
    public class FormTools
    {
        public static void HideFormOnUserClose(object sender, FormClosingEventArgs e)
        {
            if (CloseReason.UserClosing == e.CloseReason)
            {
                Form form = sender as Form;
                form.Hide();
                e.Cancel = true;
            }
        }
    }
}

#endif
