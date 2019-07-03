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
			InitializeComponent();
		}

		public SingleControlForm(string title) : this()
		{
			this.Text = title;
		}

		public void setControl(Control control)
		{
			control.Dock = DockStyle.Fill;
			this.Controls.Clear();
			this.Controls.Add(control);
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
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(900, 600);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SingleControlForm";

		}
		#endregion
	}
}
