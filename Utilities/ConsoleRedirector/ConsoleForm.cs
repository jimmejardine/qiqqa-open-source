using System.ComponentModel;
using System.Windows.Forms;

namespace Utilities.ConsoleRedirector
{
    /// <summary>
    /// Summary description for ConsoleForm.
    /// </summary>
    public class ConsoleForm : Form
    {
        private RichTextBox objText;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        public ConsoleForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private int dispose_count = 0;
        protected override void Dispose(bool disposing)
        {
            Logging.Debug("ConsoleForm::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                components?.Dispose();
                objText?.Dispose();
            }

            components = null;
            objText = null;

            base.Dispose(disposing);

            ++dispose_count;
        }

        public void setText(string t)
        {
            objText.Text = t;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            objText = new System.Windows.Forms.RichTextBox();
            SuspendLayout();
            // 
            // objText
            // 
            objText.Dock = System.Windows.Forms.DockStyle.Fill;
            objText.Location = new System.Drawing.Point(0, 0);
            objText.Name = "objText";
            objText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            objText.Size = new System.Drawing.Size(400, 390);
            objText.TabIndex = 0;
            objText.Text = "";
            objText.WordWrap = false;
            // 
            // ConsoleForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(400, 390);
            Controls.Add(objText);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Name = "ConsoleForm";
            Text = "Console";
            ResumeLayout(false);

        }
        #endregion
    }
}
