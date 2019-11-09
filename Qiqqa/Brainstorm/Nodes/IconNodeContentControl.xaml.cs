using System.Windows.Controls;
using System.Windows.Media;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for IconNodeContentControl.xaml
    /// </summary>
    public partial class IconNodeContentControl : Grid
    {
        private IconNodeContent icon_node_content;

        public IconNodeContentControl(NodeControl node_control, IconNodeContent icon_node_content)
        {
            this.icon_node_content = icon_node_content;

            InitializeComponent();

            Focusable = true;

            Image.Stretch = Stretch.Fill;
            Image.Source = icon_node_content.BitmapImage;
        }
    }
}
