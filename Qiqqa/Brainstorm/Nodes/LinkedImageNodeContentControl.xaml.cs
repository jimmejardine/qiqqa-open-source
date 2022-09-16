using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Utilities.Misc;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for ImageNodeContentControl.xaml
    /// </summary>
    public partial class LinkedImageNodeContentControl : Grid
    {
        private LinkedImageNodeContent linked_image_node_content;

        public LinkedImageNodeContentControl(NodeControl node_control, LinkedImageNodeContent linked_image_node_content)
        {
            this.linked_image_node_content = linked_image_node_content;

            InitializeComponent();

            Focusable = true;

            Image.Stretch = Stretch.Fill;
            Image.Source = linked_image_node_content.BitmapSource;
            ToolTip = linked_image_node_content.ImagePath;

            MouseDown += ImageNodeContentControl_MouseDown;
        }

        private void ImageNodeContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (2 == e.ClickCount)
            {
#if DEBUG
                if (Runtime.IsRunningInVisualStudioDesigner) return;
#endif

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files|*.jpeg;*.jpg;*.png;*.gif;*.bmp" + "|" + "All files|*.*";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                if (true == dialog.ShowDialog())
                {
                    linked_image_node_content.ImagePath = dialog.FileName;
                    Image.Source = linked_image_node_content.BitmapSource;
                    ToolTip = linked_image_node_content.ImagePath;
                }

                e.Handled = true;
            }
        }
    }
}
