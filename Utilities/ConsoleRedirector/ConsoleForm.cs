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

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public void setText(string t)
		{
			this.objText.Text = t;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.objText = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// objText
			// 
			this.objText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.objText.Location = new System.Drawing.Point(0, 0);
			this.objText.Name = "objText";
			this.objText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.objText.Size = new System.Drawing.Size(400, 390);
			this.objText.TabIndex = 0;
			this.objText.Text = "";
			this.objText.WordWrap = false;
			// 
			// ConsoleForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(400, 390);
			this.Controls.Add(this.objText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "ConsoleForm";
			this.Text = "Console";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
