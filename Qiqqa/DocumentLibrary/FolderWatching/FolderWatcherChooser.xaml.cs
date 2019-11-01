using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.FolderWatching
{
    /// <summary>
    /// Interaction logic for FolderWatcherChooser.xaml
    /// </summary>
    public partial class FolderWatcherChooser : StandardWindow
    {
        private Library _library;
        public FolderWatcherChooser(Library library)
        {
            _library = library;

            InitializeComponent();

            btnOk.Click += btnOk_Click;
            btnCancel.Click += btnCancel_Click;
            CmdResetHistory.Click += CmdResetHistory_Click;

            CmdAddFolder.Click += CmdAddFolder_Click;

            Title = "Watch Folders";

            Header.Caption = "Watch Folders";
            Header.SubCaption = "When you save a PDF into any of the specified folders, it will automatically be added to your library.  You can optionally automatically associate tags with files from each folder.";
            Header.Img = Icons.GetAppIcon(Icons.DocumentsWatchFolder);

            btnOk.Icon = Icons.GetAppIcon(Icons.Save);
            btnCancel.Icon = Icons.GetAppIcon(Icons.Cancel);

            TxtFolders.Text = _library.WebLibraryDetail.FolderToWatch;
            TxtFolders.TextChanged += TxtFolders_TextChanged;
        }

        private void TxtFolders_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string[] rows = TxtFolders.Text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        }

        private void CmdResetHistory_Click(object sender, RoutedEventArgs e)
        {
            _library.FolderWatcherManager.ResetHistory();
            MessageBoxes.Info("Your folder watching history has been reset.");
        }

        private void CmdAddFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog
            {
                Description = "Please select the folder you want to watch for new PDFs.",
                ShowNewFolderButton = true
            })
            {
                if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
                {
                    Logging.Info("The user starting watching folder {0}", dlg.SelectedPath);

                    if (string.IsNullOrEmpty(TxtFolders.Text))
                    {
                        TxtFolders.Text = dlg.SelectedPath;
                    }
                    else
                    {
                        TxtFolders.Text += "\r\n" + dlg.SelectedPath;
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            _library.WebLibraryDetail.FolderToWatch = TxtFolders.Text;
            WebLibraryManager.Instance.NotifyOfChangeToWebLibraryDetail();

            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this calss Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            _library = null;
        }
    }
}
