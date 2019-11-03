using System.Windows.Controls;
using System.Windows.Media;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for StringNodeContentControl.xaml
    /// </summary>
    public partial class EllipseNodeContentControl : UserControl
    {
        private static readonly Brush FILL_BRUSH = Brushes.LightBlue;
        private static readonly Brush STROKE_BRUSH = Brushes.DarkBlue;
        private const double STROKE_THICKNESS = 1;
        private EllipseNodeContent circle_node_content;

        public EllipseNodeContentControl(NodeControl node_control, EllipseNodeContent circle_node_content_)
        {
            InitializeComponent();

            Focusable = true;

            circle_node_content = circle_node_content_;
            ToolTip = circle_node_content_.text;

            ObjEllipse.Fill = FILL_BRUSH;
            ObjEllipse.Stroke = STROKE_BRUSH;
            ObjEllipse.StrokeThickness = STROKE_THICKNESS;
        }
    }
}
