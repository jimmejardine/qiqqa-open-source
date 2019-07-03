using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{
    /// <summary>
    /// Interaction logic for ImageNodeContentControl.xaml
    /// </summary>
    public partial class LinkedImageNodeContentControl : Grid
    {
        LinkedImageNodeContent linked_image_node_content;

        public LinkedImageNodeContentControl(NodeControl node_control, LinkedImageNodeContent linked_image_node_content)
        {
            this.linked_image_node_content = linked_image_node_content;

            InitializeComponent();

            this.Focusable = true;

            this.Image.Stretch = Stretch.Fill;
            this.Image.Source = linked_image_node_content.BitmapSource;
            this.ToolTip = linked_image_node_content.ImagePath;

            MouseDown += ImageNodeContentControl_MouseDown;
        }

        void ImageNodeContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (2 == e.ClickCount)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files|*.jpeg;*.jpg;*.png;*.gif;*.bmp" + "|" + "All files|*.*";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                if (true == dialog.ShowDialog())
                {
                    this.linked_image_node_content.ImagePath = dialog.FileName;
                    this.Image.Source = linked_image_node_content.BitmapSource;
                    this.ToolTip = linked_image_node_content.ImagePath;
                }

                e.Handled = true;
            }
        }
    }
}
