using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Utilities.GUI
{
    [ContentProperty("ClientContent")]
    public class AugmentedBorder : Border
    {
        public static readonly double CORNER_RADIUS = 3;
        private Grid GridContent;

        public AugmentedBorder()
        {
            Theme.Initialize();

            CornerRadius = new CornerRadius(CORNER_RADIUS);
            BorderBrush = ThemeColours.Background_Brush_Blue_VeryDark;
            BorderThickness = new Thickness(1);

            GridContent = new Grid();
            GridContent.Margin = new Thickness(CORNER_RADIUS);
            Child = GridContent;
        }

        [Bindable(true)]
        public FrameworkElement ClientContent
        {
            set
            {
                GridContent.Children.Clear();
                GridContent.Children.Add(value);
            }

            get
            {
                if (GridContent.Children.Count > 0)
                {
                    return (FrameworkElement)GridContent.Children[0];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
