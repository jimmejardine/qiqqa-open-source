using System.ComponentModel;
using System.Windows.Forms;

namespace Utilities.GUI.Charting
{
    /// <summary>
    /// Summary description for SingleControlForm.
    /// </summary>
    public class SingleControlForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        public SingleControlForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public SingleControlForm(string title) : this()
        {
            Text = title;
        }

        public void setControl(Control control)
        {
            control.Dock = DockStyle.Fill;
            Controls.Clear();
            Controls.Add(control);
        }

        private int dispose_count = 0;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Logging.Debug("GenericChartForm::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                components?.Dispose();
            }

            components = null;

            base.Dispose(disposing);

            ++dispose_count;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SingleControlForm));
            // 
            // SingleControlForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(900, 600);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Name = "SingleControlForm";

        }
        #endregion
    }
}
