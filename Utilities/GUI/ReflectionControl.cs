//  --------------------------------
//  Code credited to
//  - Nir (http://www.nbdtech.com/blog/archive/2007/11/21/WPF-Reflection-Control.aspx)
//  ---------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utilities.GUI
{
    public class ReflectionControl : Decorator
    {
        #region Private Fields

        private readonly VisualBrush _reflection;
        private readonly LinearGradientBrush _opacityMask;

        #endregion

        #region Constructor

        public ReflectionControl()
        {
            // Set defaults for this control
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Center;

            // Create brushes were going to use
            _opacityMask = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 0.8)
            };

            _opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0));
            _opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0.5));
            _opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 0.8));
            _opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

            _reflection = new VisualBrush
            {
                Transform = new ScaleTransform(1, -1)
            };
        }

        #endregion

        #region Public Override

        protected override Size MeasureOverride(Size constraint)
        {
            // We need twice the space that our content needs
            if (Child == null) return new Size(0, 0);

            Child.Measure(constraint);
            return new Size(Child.DesiredSize.Width, Child.DesiredSize.Height * 2.6);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            // always put out content at the upper half of the control
            if (Child == null) return new Size(0, 0);

            Child.Arrange(new Rect(0, -(arrangeBounds.Height * 2), arrangeBounds.Width, arrangeBounds.Height * 2.6));
            return arrangeBounds;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // draw everything except the reflection
            base.OnRender(drawingContext);

            // set opacity
            drawingContext.PushOpacityMask(_opacityMask);
            drawingContext.PushOpacity(0.3);

            // set reflection parameters based on content size
            _reflection.Visual = Child;
            ((ScaleTransform)_reflection.Transform).CenterY = 4 * ActualHeight / 5;

            // draw the reflection
            drawingContext.DrawRectangle(_reflection, null, new Rect(0, ActualHeight / 2, ActualWidth, ActualHeight / 2));

            // cleanup
            drawingContext.Pop();
            drawingContext.Pop();
        }

        #endregion
    }
}
