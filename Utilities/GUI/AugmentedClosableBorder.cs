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

            this.Unloaded += AugmentedClosableBorder_Unloaded;

            //
            // Summary:
            //     Occurs when either the System.Windows.FrameworkElement.ActualHeight or the System.Windows.FrameworkElement.ActualWidth
            //     properties change value on this element.
            this.SizeChanged += AugmentedClosableBorder_SizeChanged;

            //
            // Summary:
            //     Occurs when System.Windows.FrameworkElement.BringIntoView(System.Windows.Rect)
            //     is called on this element.
            this.RequestBringIntoView += AugmentedClosableBorder_RequestBringIntoView;

            //
            // Summary:
            //     Occurs when the element is laid out, rendered, and ready for interaction.
            this.Loaded += AugmentedClosableBorder_Loaded;

            //
            // Summary:
            //     Occurs when this System.Windows.FrameworkElement is initialized. This event coincides
            //     with cases where the value of the System.Windows.FrameworkElement.IsInitialized
            //     property changes from false (or undefined) to true.
            this.Initialized += AugmentedClosableBorder_Initialized;

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
            if (has_been_force_closed_by_user)
            {
                IdPath = GetIdPath();
                this.Visibility = Visibility.Collapsed;
            }
            IdPath = GetIdPath();
        }

        void image_close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.has_been_force_closed_by_user = true;
            this.Visibility = Visibility.Collapsed;
            IdPath = GetIdPath();

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

        // -----------------------------------------------------------------------------

        //
        // Summary:
        //     When overridden in a derived class, returns an alternative user interface (UI)
        //     parent for this element if no visual parent exists.
        //
        // Returns:
        //     An object, if implementation of a derived class has an alternate parent connection
        //     to report.
        protected override DependencyObject GetUIParentCore()
        {
            return base.GetUIParentCore();
        }

        //
        // Summary:
        //     Called when the parent of the visual object is changed.
        //
        // Parameters:
        //   oldParent:
        //     A value of type System.Windows.DependencyObject that represents the previous
        //     parent of the System.Windows.Media.Visual object. If the System.Windows.Media.Visual
        //     object did not have a previous parent, the value of the parameter is null.
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
        }

    private void AugmentedClosableBorder_Unloaded(object sender, RoutedEventArgs e)
    {
            Logging.Debug("not implemented");
        }

        private void AugmentedClosableBorder_SizeChanged(object sender, SizeChangedEventArgs e)
    {
            Logging.Debug("not implemented");
            IdPath = GetIdPath();
        }

        private void AugmentedClosableBorder_Initialized(object sender, System.EventArgs e)
        {
            Logging.Debug("not implemented");
        }

        string IdPath;

        private void AugmentedClosableBorder_Loaded(object sender, RoutedEventArgs e)
        {
            Logging.Debug("not implemented");

            IdPath = GetIdPath();
        }

        private void AugmentedClosableBorder_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            Logging.Debug("not implemented");
        }


        public string GetIdPath()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return null;
            }
            if (null == this.Parent)
            {
                return null;
            }
            string rv = this.Name;
            var p = this.Parent;
            while (null != p)
            {
                FrameworkElement el = p as FrameworkElement;
                if (null != el)
                {
                    if (!string.IsNullOrEmpty(el.Name))
                    {
                        rv = el.Name + "/" + rv;
                    }
                    p = el.Parent;
                }
                else
                {
                    p = null;
                }
            }

            return rv;
        }

    }
}







//
// Summary:
//     Occurs when the element is removed from within an element tree of loaded elements.
