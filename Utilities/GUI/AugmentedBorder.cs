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

        Grid GridContent;

        public AugmentedBorder()
        {
            this.CornerRadius = new CornerRadius(CORNER_RADIUS);
            this.BorderBrush = ThemeColours.Background_Brush_Blue_VeryDark;
            this.BorderThickness = new Thickness(1);

            this.GridContent = new Grid();
            this.GridContent.Margin = new Thickness(CORNER_RADIUS);
            this.Child = GridContent;            
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
