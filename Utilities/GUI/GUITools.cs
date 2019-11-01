using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Utilities.GUI
{
    /// <summary>
    /// Contains a few random GUI tools
    /// </summary>
    public class GUITools
    {
        public static FrameworkElement WrapElementWithDropShadow(FrameworkElement fe)
        {
            Grid g = new Grid();
            g.Margin = new Thickness(5);

            {
                Border b = new Border();
                b.Background = Brushes.White;
                b.BorderThickness = new Thickness(1);
                b.CornerRadius = new CornerRadius(5);
                b.BorderBrush = Brushes.LightGray;
                {
                    var effect = new DropShadowEffect();
                    effect.Color = Colors.LightGray;
                    b.Effect = effect;
                }
                g.Children.Add(b);
            }

            {
                Grid gg = new Grid();
                gg.Margin = new Thickness(5);
                gg.Children.Add(fe);
                gg.ClipToBounds = true;
                g.Children.Add(gg);
            }

            return g;
        }

        public static Visibility GetMinimumVisibility(Visibility v1, Visibility v2)
        {
            if (Visibility.Collapsed == v1 || Visibility.Collapsed == v2) return Visibility.Collapsed;
            if (Visibility.Hidden == v1 || Visibility.Hidden == v2) return Visibility.Hidden;
            if (Visibility.Visible == v1 || Visibility.Visible == v2) return Visibility.Visible;

            // Should never get here...
            return Visibility.Visible;
        }

        public static Visibility GetMaximumVisibility(Visibility v1, Visibility v2)
        {
            if (Visibility.Visible == v1 || Visibility.Visible == v2) return Visibility.Visible;
            if (Visibility.Hidden == v1 || Visibility.Hidden == v2) return Visibility.Hidden;
            if (Visibility.Collapsed == v1 || Visibility.Collapsed == v2) return Visibility.Collapsed;

            // Should never get here...
            return Visibility.Collapsed;
        }


        public static bool IsSelectedItemClicked(ListView list_view, MouseButtonEventArgs e)
        {
            foreach (var selected_item in list_view.SelectedItems)
            {
                ListViewItem selected_item_container = (ListViewItem)list_view.ItemContainerGenerator.ContainerFromItem(selected_item);
                if (null != selected_item_container)
                {
                    Rect selected_item_container_bounds = VisualTreeHelper.GetDescendantBounds(selected_item_container);
                    Point clicked_position = e.GetPosition(selected_item_container);
                    if (selected_item_container_bounds.Contains(clicked_position))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void ScrollToTop(ListView listview)
        {
            try
            {
                if (listview.IsVisible)
                {
                    listview.UpdateLayout();

                    // you MAY encounter this crash:
                    // System.ArgumentOutOfRangeException: 'Specified index is out of range or child at index is null. 
                    // Do not call this method if VisualChildrenCount returns zero, indicating that the Visual 
                    // has no children.
                    // Parameter name: index
                    // Actual value was 0.'
                    // Unfortunately, listview.VisualChildrenCount is inaccessible due to its protection settings. 
                    // So that's a lot of help...
                    // Further debugging reveils Height=NaN, so we'll fly with that one...
                    Border b = VisualTreeHelper.GetChild(listview, 0) as Border;
                    if (b != null)
                    {
                        ScrollViewer v = b.Child as ScrollViewer;

                        if (v != null)
                        {
                            v.ScrollToVerticalOffset(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }

        public static void ScrollToTop(FlowDocumentScrollViewer container)
        {
            try
            {
                container.UpdateLayout();
                ScrollViewer sv = GetScrollViewer(container);
                if (null != sv)
                {
                    sv.ScrollToTop();
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }


        public static ScrollViewer GetScrollViewer(DependencyObject parent)
        {
            DependencyObject current = parent;

            while (VisualTreeHelper.GetChildrenCount(current) > 0)
            {
                ScrollViewer sv = current as ScrollViewer;
                if (null != sv) return sv;

                current = VisualTreeHelper.GetChild(current, 0);
            }

            // If we get this far, we didn't find a ScrollViewer
            return null;
        }

        public static bool IsDescendentOf(object child, object parent)
        {
            try
            {
                DependencyObject de = child as DependencyObject;
                if (null == de) return false;

                while (null != de)
                {
                    if (de == parent) return true;
                    de = GetParentObject(de);
                }
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem while trying to determine the family tree for parent={0} and child={1}", parent, child);
            }

            return false;
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T GetParentControl<T>(DependencyObject child)
            where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return GetParentControl<T>(parentObject);
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) return null;

            //handle content elements separately
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }



        public static BitmapImage RenderToBitmapImage(UIElement element)
        {
            var target = new RenderTargetBitmap((int)(element.RenderSize.Width), (int)(element.RenderSize.Height), 96, 96, PixelFormats.Pbgra32);
            target.Render(element);

            var frame = BitmapFrame.Create(target);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(frame);

            MemoryStream ms = new MemoryStream();    // <-- must be disposed by caller
            encoder.Save(ms);

            BitmapImage bitmap_image = new BitmapImage();
            bitmap_image.BeginInit();
            bitmap_image.CacheOption = BitmapCacheOption.OnLoad;
            bitmap_image.StreamSource = ms;
            bitmap_image.EndInit();
            bitmap_image.StreamSource.Close();
            bitmap_image.StreamSource = null;
            bitmap_image.Freeze();
            return bitmap_image;
        }
    }
}
