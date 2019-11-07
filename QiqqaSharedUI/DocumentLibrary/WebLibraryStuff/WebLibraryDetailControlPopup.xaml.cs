using System.Windows;
using System.Windows.Controls;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    /// <summary>
    /// Interaction logic for WebLibraryDetailControlPopup.xaml
    /// </summary>
    public partial class WebLibraryDetailControlPopup : UserControl
    {
        private WebLibraryDetailControl web_library_detail_control;
        private AugmentedPopup popup;


        public WebLibraryDetailControlPopup(WebLibraryDetailControl web_library_detail_control)
        {
            this.web_library_detail_control = web_library_detail_control;

            InitializeComponent();

            popup = new AugmentedPopup(this);

            MenuOpenLibrary.Click += MenuOpenLibrary_Click;
            MenuCustomiseIcon.Click += MenuCustomiseIcon_Click;
            MenuCustomiseBackground.Click += MenuCustomiseBackground_Click;
        }

        private void MenuCustomiseBackground_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();
            web_library_detail_control.CustomiseBackground();
            e.Handled = true;
        }

        private void MenuCustomiseIcon_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();
            web_library_detail_control.CustomiseIcon();
            e.Handled = true;
        }

        private void MenuOpenLibrary_Click(object sender, RoutedEventArgs e)
        {
            popup.Close();
            web_library_detail_control.OpenLibrary();
            e.Handled = true;
        }

        public void Open()
        {
            popup.IsOpen = true;
        }

    }
}
