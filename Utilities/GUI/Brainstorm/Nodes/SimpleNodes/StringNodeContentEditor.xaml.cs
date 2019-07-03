using System.Windows.Controls;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{    
    /// <summary>
    /// Interaction logic for StringNodeContentControl.xaml
    /// </summary>
    public partial class StringNodeContentEditor : UserControl
    {
        public StringNodeContentEditor(NodeControl node_control, StringNodeContent string_node_content)
        {
            InitializeComponent();

            this.DataContext = string_node_content.Bindable;
        }
    }
}
