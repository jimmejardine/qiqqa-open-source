using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.DocumentLibrary.LibraryFilter.GenericLibraryExplorerStuff;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.TagExplorerStuff
{
    /// <summary>
    /// Interaction logic for TabExplorerItemControl.xaml
    /// </summary>
    public partial class GenericLibraryExplorerItemControl : DockPanel
    {
        public GenericLibraryExplorerItem item;

        public GenericLibraryExplorerItemControl()
        {
            Theme.Initialize();

            InitializeComponent();

            DataContextChanged += GenericLibraryExplorerItemControl_DataContextChanged;

            ObjCaption.DragOver += TagExplorerControl_DragOver;
            ObjCaption.Drop += TagExplorerControl_Drop;

            ObjCaption.MouseLeftButtonUp += GenericLibraryExplorerItemControl_MouseLeftButtonUp;
            ObjChecked.Click += ObjChecked_Click;

            ObjCaption.MouseRightButtonUp += TagExplorerItemControl_MouseRightButtonUp;
        }

        private void GenericLibraryExplorerItemControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            item = e.NewValue as GenericLibraryExplorerItem;

            if (null != item)
            {
                ObjCaption.Text = String.Format("{0} ({1})", item.tag, item.fingerprints.Count);
                ObjCaption.Background = item.IsSelected ? ThemeColours.Background_Brush_Blue_VeryDarkToDark : Brushes.Transparent;
                ObjCaption.Foreground = item.IsSelected ? Brushes.Black : Brushes.Black;
                ObjChecked.IsChecked = item.IsSelected;
            }
            else
            {
                ObjCaption.Text = null;
                ObjCaption.Background = Brushes.Transparent;
                ObjCaption.Foreground = Brushes.Black;
                ObjChecked.IsChecked = false;
            }
        }

        private void GenericLibraryExplorerItemControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null != item)
            {
                item.GenericLibraryExplorerControl.ToggleSelectItem(item.tag, KeyboardTools.IsCTRLDown() || KeyboardTools.IsShiftDown());
            }

            e.Handled = true;
        }

        private void ObjChecked_Click(object sender, RoutedEventArgs e)
        {
            if (null != item)
            {
                item.GenericLibraryExplorerControl.ToggleSelectItem(item.tag, true);
            }

            e.Handled = true;
        }

        private void TagExplorerItemControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null != item && null != item.OnItemPopup)
            {
                item.OnItemPopup(item.library, item.tag);
            }

            e.Handled = true;
        }

        private void TagExplorerControl_DragOver(object sender, DragEventArgs e)
        {
            if (null != item && null != item.OnItemDragOver)
            {
                item.OnItemDragOver(item.library, item.tag, e);
            }
        }

        private void TagExplorerControl_Drop(object sender, DragEventArgs e)
        {
            if (null != item && null != item.OnItemDrop)
            {
                item.OnItemDrop(item.library, item.tag, e);
            }
        }
    }
}
