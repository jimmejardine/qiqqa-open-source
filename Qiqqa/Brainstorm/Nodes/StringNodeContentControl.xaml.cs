using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.Common.Common;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Brainstorm.Nodes
{    
    /// <summary>
    /// Interaction logic for StringNodeContentControl.xaml
    /// </summary>
    public partial class StringNodeContentControl : UserControl, IEditableNodeContentControl, IDisposable
    {
        BackgroundFader fader;

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

            this.fader = new BackgroundFader(this);
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
            if (e.Key == Key.Escape)
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

        #region --- IDisposable ------------------------------------------------------------------------

        ~StringNodeContentControl()
        {
            Logging.Debug("~StringNodeContentControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing StringNodeContentControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("StringNodeContentControl::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                // Get rid of managed resources / get rid of cyclic references:
                this.fader?.Dispose();
            }
            this.fader = null;

            this.DataContext = null;

            this.MouseDoubleClick -= StringNodeContentControl_MouseDoubleClick;
            this.KeyDown -= StringNodeContentControl_KeyDown;

            TxtEdit.LostFocus -= edit_text_box_LostFocus;
            TxtEdit.PreviewKeyDown -= edit_text_box_PreviewKeyDown;

            ++dispose_count;
        }

        #endregion

    }
}
