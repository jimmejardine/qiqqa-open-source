using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Utilities.GUI;

namespace Qiqqa.Common.TagManagement
{
    /// <summary>
    /// Interaction logic for TagEditorItemControl.xaml
    /// </summary>
    public partial class TagEditorItemControl : Grid
    {
        public delegate void TagRemovedDelegate(string tag);

        public TagEditorItemControl()
        {
            Theme.Initialize();

            InitializeComponent();
        }

        string tag;
        TagRemovedDelegate tag_removed_delegate;

        public TagEditorItemControl(string tag, TagRemovedDelegate tag_removed_delegate)
        {
            this.tag = tag;
            this.tag_removed_delegate = tag_removed_delegate;

            InitializeComponent();

            this.Margin = new Thickness(2, 1, 2, 1);            
            this.MinWidth = 10;
            this.MinHeight = 10;
            
            TextTag.Text = tag;
            TextTag.ToolTip = tag;
            TextTag.VerticalAlignment = VerticalAlignment.Center;
            TextTag.Padding = new Thickness(8, 4, 4, 4);

            ImageClear.Source = Icons.GetAppIcon(Icons.Clear);
            ImageClear.IsHitTestVisible = true;
            ImageClear.MouseDown += ImageClear_MouseDown;

            ImageClear.Cursor = Cursors.Hand;
            ImageClear.VerticalAlignment = VerticalAlignment.Center;
            ImageClear.ToolTip = "Click here to remove this tag.";
        }

        void ImageClear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tag_removed_delegate(tag);
            e.Handled = true;
        }
    }
}
