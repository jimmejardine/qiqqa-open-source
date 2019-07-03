using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Utilities.Reflection;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for ThemeNodeContentControl.xaml
    /// </summary>
    public partial class LibraryNodeContentControl : UserControl, IKeyPressableNodeContentControl
    {
        NodeControl node_control;
        AugmentedBindable<LibraryNodeContent> library_node_content;

        public LibraryNodeContentControl(NodeControl node_control, LibraryNodeContent library_node_content)
        {
            InitializeComponent();

            this.node_control = node_control;
            this.library_node_content = new AugmentedBindable<LibraryNodeContent>(library_node_content);

            this.DataContext = this.library_node_content;

            Focusable = true;

            this.ImageIcon.Source = Icons.GetAppIcon(Icons.BrainstormLibrary);

            TextBorder.CornerRadius = NodeThemes.corner_radius;
        }

        public void ProcessKeyPress(KeyEventArgs e)
        {
            if (false) { }
            else if (Key.S == e.Key)
            {
                //ExpandSpecificDocuments();
                //e.Handled = true;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}