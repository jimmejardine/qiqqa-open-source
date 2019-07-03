using System.Windows.Controls;

namespace Qiqqa.Brainstorm.Nodes
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
