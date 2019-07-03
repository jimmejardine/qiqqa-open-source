using System.Windows.Controls;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for ThemeNodeContentEditor.xaml
    /// </summary>
    public partial class ThemeNodeContentEditor : UserControl
    {
        NodeControl node_control;
        ThemeNodeContent theme_node_content;

        public ThemeNodeContentEditor(NodeControl node_control, ThemeNodeContent theme_node_content)
        {
            this.node_control = node_control;
            this.theme_node_content = theme_node_content;
            this.DataContext = theme_node_content;

            InitializeComponent();
        }
    }
}
