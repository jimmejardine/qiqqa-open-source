using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryDBStuff
{
    /// <summary>
    /// Interaction logic for LibraryDBExplorer.xaml
    /// </summary>
    public partial class LibraryDBExplorer : UserControl
    {
        Library library = null;

        public LibraryDBExplorer()
        {
            InitializeComponent();

            TxtLibrary.IsReadOnly = true;
            TxtLibrary.PreviewMouseDown += TxtLibrary_PreviewMouseDown;
            TxtLibrary.ToolTip = "Click to choose a library to use for your Expedition.";
            TxtLibrary.Cursor = Cursors.Hand;

            ButtonGet.Caption = "Get";
            ButtonGet.Click += ButtonGet_Click;

            ButtonPut.Caption = "Put";
            ButtonPut.Click += ButtonPut_Click;
        }

        void ButtonGet_Click(object sender, RoutedEventArgs e)
        {
            if (null == library)
            {
                MessageBoxes.Error("You must choose a library...");
                return;
            }

            TxtData.Text = "";
            
            var items = library.LibraryDB.GetLibraryItems(TxtFingerprint.Text, TxtExtension.Text);
            if (0 == items.Count)
            {
                MessageBoxes.Warn("No entry was found.");
            }
            else if (1 == items.Count)
            {
                byte[] data = items[0].data;
                string json = Encoding.UTF8.GetString(data);
                TxtData.Text = json;
            }
            else
            {
                MessageBoxes.Warn("{0} entries were found, so not showing them...", items.Count);
            }
        }

        void ButtonPut_Click(object sender, RoutedEventArgs e)
        {
            if (null == library)
            {
                MessageBoxes.Error("You must choose a library...");
                return;
            }

            string json = TxtData.Text;            
            if (MessageBoxes.AskQuestion("Are you sure you want to write {0} characters to the database?", json.Length))
            {
                library.LibraryDB.PutString(TxtFingerprint.Text, TxtExtension.Text, json);
            }
        }

        void TxtLibrary_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.library = null;
            TxtLibrary.Text = "Click to choose a library.";

            // Pick a new library...
            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != web_library_detail)
            {
                this.library = web_library_detail.library;
                TxtLibrary.Text = web_library_detail.Title;
            }

            e.Handled = true;
        }
    }
}
