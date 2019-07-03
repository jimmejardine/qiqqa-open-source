using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.DocumentLibrary;

namespace Qiqqa.Common.TagManagement
{
    /// <summary>
    /// Interaction logic for TagEditorItemControl.xaml
    /// </summary>
    public partial class TagEditorAddControl : Grid
    {
        public delegate void OnNewTagDelegate(string tag);
        public event OnNewTagDelegate OnNewTag;

        public TagEditorAddControl()
        {
            InitializeComponent();

            this.Margin = new Thickness(2, 1, 2, 1);
            this.MinWidth = 10;
            this.MinHeight = 10;

            ComboBoxNewTag.VerticalAlignment = VerticalAlignment.Center;
            ComboBoxNewTag.Padding = new Thickness(8, 4, 4, 4);

            RebuildComboBoxNewTag();

            ComboBoxNewTag.PreviewKeyUp += ComboBoxNewTag_KeyUp;
            ComboBoxNewTag.FilterMode = AutoCompleteFilterMode.Contains;
            ComboBoxNewTag.IsTextCompletionEnabled = false;
            ComboBoxNewTag.ToolTip = "Enter a tag here and press <ENTER> to attach it to this reference.";

            ComboBoxNewTag.PreviewMouseDown += ComboBoxNewTag_PreviewMouseDown;
            ComboBoxNewTag.LostFocus += ComboBoxNewTag_LostFocus;
        }

        DateTime last_frikken_mousedown_to_suppress_lostfocus_bug = DateTime.MinValue;

        void ComboBoxNewTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            last_frikken_mousedown_to_suppress_lostfocus_bug = DateTime.UtcNow;            
        }

        void ComboBoxNewTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DateTime.UtcNow.Subtract(last_frikken_mousedown_to_suppress_lostfocus_bug).TotalMilliseconds < 100)
            {
                return;
            }

            AttachNewTag();
        }

        void ComboBoxNewTag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttachNewTag();
            }
        }

        void AttachNewTag()
        {
            string tag = ComboBoxNewTag.Text.Trim();
                
            if (!String.IsNullOrEmpty(tag))
            {
                if (null != OnNewTag)
                {
                    OnNewTag(tag);
                }
            }

            ComboBoxNewTag.Text = "";

            // Rebuild the autosuggests
            RebuildComboBoxNewTag();
        }

        private void RebuildComboBoxNewTag()
        {
            // Rebind so that it updates
            ComboBoxNewTag.ItemsSource = null;
            ComboBoxNewTag.ItemsSource = TagManager.Instance.SortedTags;
        }
    }
}
