#if false

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
        private static string[] LANGUAGES = { "English", "Spanish", "German", "French", "Zulu", "Afrikaans", "Latin", "Italian" };

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        public LanguageDropdown()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            DropDownStyle = ComboBoxStyle.DropDownList;
            fillValues();
        }

        private void fillValues()
        {
            BeginUpdate();
            Items.Clear();
            foreach (String language in LANGUAGES)
            {
                Items.Add(language);
            }
            EndUpdate();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
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
            Name = "NoteDropdown";
        }
        #endregion
    }
}

#endif

