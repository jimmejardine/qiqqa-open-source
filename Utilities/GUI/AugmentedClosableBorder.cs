using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using icons;

namespace Utilities.GUI
{
    [ContentProperty("ClientContent")]
    public class AugmentedClosableBorder : Border
    {
        Grid GridBackground;
        Grid GridContent;
        // TODO: add SuperExpert Mode where these messages are not shown - at least most of 'em: once closed stays closed forever.
        private bool has_been_force_closed_by_user = false;

        public AugmentedClosableBorder()
        {
            Theme.Initialize();

            this.CornerRadius = new CornerRadius(3);
            this.BorderBrush = ThemeColours.Background_Brush_Blue_VeryDark;
            this.BorderThickness = new Thickness(1);

            this.GridBackground = new Grid();

            this.GridContent = new Grid();
            this.GridContent.Margin = new Thickness(3);
            this.GridBackground.Children.Add(GridContent);

            this.IsVisibleChanged += AugmentedClosableBorder_IsVisibleChanged;

            // And the overlayed button panel
            Image image_close = new Image();
            image_close.Source = Icons.GetAppIcon(Icons.AdvertClose);
            image_close.HorizontalAlignment = HorizontalAlignment.Right;
            image_close.VerticalAlignment = VerticalAlignment.Top;
            image_close.Margin = new Thickness(5);
            image_close.Width = 20;
            image_close.Cursor = Cursors.Hand;
            image_close.MouseLeftButtonUp += image_close_MouseLeftButtonUp;

            this.GridBackground.Children.Add(image_close);

            this.Child = GridBackground;            
        }

        void AugmentedClosableBorder_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (has_been_force_closed_by_user) this.Visibility = Visibility.Collapsed;
        }

        void image_close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.has_been_force_closed_by_user = true;
            this.Visibility = Visibility.Collapsed;
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
