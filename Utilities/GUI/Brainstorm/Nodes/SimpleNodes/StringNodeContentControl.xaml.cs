using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{    
    /// <summary>
    /// Interaction logic for StringNodeContentControl.xaml
    /// </summary>
    public partial class StringNodeContentControl : UserControl, IEditableNodeContentControl
    {
        public StringNodeContentControl(NodeControl node_control, StringNodeContent string_node_content)
        {
            InitializeComponent();

            Focusable = true;

            this.DataContext = string_node_content.Bindable;            

            ExitEditMode();
            
            this.MouseDoubleClick += StringNodeContentControl_MouseDoubleClick;
            this.KeyDown += StringNodeContentControl_KeyDown;

            TxtEdit.LostFocus += edit_text_box_LostFocus;
            TxtEdit.PreviewKeyDown += edit_text_box_PreviewKeyDown;

            new BackgroundFader(this);
        }

        void StringNodeContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EnterEditMode();
            e.Handled = true;
        }

        void StringNodeContentControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (true
                && e.Key != Key.Delete
                && e.Key != Key.LeftShift
                && e.Key != Key.RightShift
                && e.Key != Key.LeftCtrl
                && e.Key != Key.RightCtrl
                && e.Key != Key.LeftAlt
                && e.Key != Key.RightAlt
                && e.Key != Key.Escape
                )
            {
                EnterEditMode(e);
            }
        }

        void edit_text_box_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (false) { }
            else if (e.Key == Key.Escape)
            {
                ExitEditMode();
                e.Handled = true;
            }
        }

        void edit_text_box_LostFocus(object sender, RoutedEventArgs e)
        {
            ExitEditMode();
        }

        public void EnterEditMode()
        {
            EnterEditMode(null);
        }

        private void EnterEditMode(KeyEventArgs e)
        {
            if (TxtEdit.Visibility != Visibility.Visible)
            {
                TxtEdit.Visibility = Visibility.Visible;
                TxtOverview.Visibility = Visibility.Hidden;
                TxtEdit.Focus();
                Keyboard.Focus(TxtEdit);
                TxtEdit.SelectionStart = TxtEdit.Text.Length;

                // If it is the default text, select it all in a bid to allow the user to delete it with one keypress...
                if (TxtEdit.Text == StringNodeContent.DEFAULT_NODE_CONTENT)
                {
                    TxtEdit.SelectAll();
                }
            }
        }

        void ExitEditMode()
        {
            TxtEdit.Visibility = Visibility.Collapsed;
            TxtOverview.Visibility = Visibility.Visible;
            this.Focus();
        }
    }
}
