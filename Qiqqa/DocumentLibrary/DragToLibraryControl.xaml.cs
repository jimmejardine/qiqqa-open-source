using System.Windows.Controls;
using System.Windows.Media;
using icons;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary
{
    /// <summary>
    /// Interaction logic for DragToLibraryControl.xaml
    /// </summary>
    public partial class DragToLibraryControl : UserControl
    {
        private DragToLibraryManager drag_to_library_manager;

        public DragToLibraryControl()
        {
            InitializeComponent();

            ButtonDragToLibrary.Icon = Icons.GetAppIcon(Icons.WebDragToLibrary);
            ButtonDragToLibrary.Caption = "Drop PDF to library";
            ButtonDragToLibrary.ToolTip = "If you drag-and-drop a URL to a PDF onto this area, the PDF will automatically be downloaded and added to your library.";
            ButtonDragToLibrary.Background = new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.GreenYellow, 128));
            ButtonDragToLibrary.CaptionDock = Dock.Right;

            drag_to_library_manager = new DragToLibraryManager(null);
            drag_to_library_manager.RegisterControl(ButtonDragToLibrary);
        }
    }
}
