using System.Windows;

namespace Utilities.GUI.DualTabbedLayoutStuff
{
    /// <summary>
    /// Implement this on a control if you want its drag-and-drop events to be poked when the tab item header is poked...
    /// </summary>
    public interface IDualTabbedLayoutDragDrop
    {
        void DualTabbedLayoutDragEnter(object sender, DragEventArgs e);
        void DualTabbedLayoutDragOver(object sender, DragEventArgs e);
        void DualTabbedLayoutDrop(object sender, DragEventArgs e);
    }
}
