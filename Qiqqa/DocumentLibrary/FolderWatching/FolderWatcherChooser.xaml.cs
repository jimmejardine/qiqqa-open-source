using System;
using System.ComponentModel;
using System.Windows;
using icons;
using Microsoft.WindowsAPICodePack.Dialogs;
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
        private WebLibraryDetail web_library_detail;
        public FolderWatcherChooser(WebLibraryDetail library)
        {
            web_library_detail = library;

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

            TxtFolders.Text = web_library_detail.FolderToWatch;
            TxtFolders.TextChanged += TxtFolders_TextChanged;
        }

        private void TxtFolders_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string[] rows = TxtFolders.Text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        }

        private void CmdResetHistory_Click(object sender, RoutedEventArgs e)
        {
            web_library_detail.Xlibrary.FolderWatcherManager.ResetHistory();
            MessageBoxes.Info("Your folder watching history has been reset.");
        }

        private void CmdAddFolder_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "Please select the folder you want to watch for new PDFs.";
                CommonFileDialogResult result = dialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                {
                    Logging.Info("The user starting watching folder {0}", dialog.FileName);

                    if (string.IsNullOrEmpty(TxtFolders.Text))
                    {
                        TxtFolders.Text = dialog.FileName;
                    }
                    else
                    {
                        TxtFolders.Text += "\r\n" + dialog.FileName;
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
            web_library_detail.FolderToWatch = TxtFolders.Text;
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

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            web_library_detail = null;
        }
    }
}
