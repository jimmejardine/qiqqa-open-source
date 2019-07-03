using System.Windows.Controls;
using System.Windows.Media;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{
    /// <summary>
    /// Interaction logic for IconNodeContentControl.xaml
    /// </summary>
    public partial class IconNodeContentControl : Grid
    {
        IconNodeContent icon_node_content;

        public IconNodeContentControl(NodeControl node_control, IconNodeContent icon_node_content)
        {
            this.icon_node_content = icon_node_content;

            InitializeComponent();

            this.Focusable = true;

            this.Image.Stretch = Stretch.Fill;
            this.Image.Source = icon_node_content.BitmapImage;
        }
    }
}
