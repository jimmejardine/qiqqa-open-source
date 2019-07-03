using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Utilities.Language
{
	/// <summary>
	/// Summary description for NoteDropdown.
	/// </summary>
	public class LanguageDropdown : ComboBox
	{
		static string[] LANGUAGES = { "English", "Spanish", "German", "French", "Zulu", "Afrikaans", "Latin", "Italian" };

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public LanguageDropdown()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.DropDownStyle = ComboBoxStyle.DropDownList;
			fillValues();
		}

		private void fillValues()
		{
			this.BeginUpdate();
			this.Items.Clear();
			foreach (String language in LANGUAGES)
			{
				Items.Add(language);
			}
			this.EndUpdate();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// NoteDropdown
			// 
			this.Name = "NoteDropdown";
		}
		#endregion
	}
}
