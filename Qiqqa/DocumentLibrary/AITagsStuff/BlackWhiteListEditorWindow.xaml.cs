using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;

namespace Qiqqa.DocumentLibrary.AITagsStuff
{
    /// <summary>
    /// Interaction logic for BlackWhiteListEditorWindow.xaml
    /// </summary>
    public partial class BlackWhiteListEditorWindow : StandardWindow
    {
        private WebLibraryDetail web_library_detail;
        private List<BlackWhiteListEntry> entries;

        public BlackWhiteListEditorWindow()
        {
            InitializeComponent();

            ObjHeader.Img = Icons.GetAppIcon(Icons.LibraryAutoTagsBlackWhiteLists);
            ObjHeader.Caption = "AutoTag black/whitelists";
            ObjHeader.SubCaption = "Edit the black/whitelists for the AutoTags for your library.";

            CmdSave.Icon = Icons.GetAppIcon(Icons.Save);
            CmdSave.Caption = "Save lists and close";
            CmdSave.ToolTip = "Save these changes for the next time you generate AutoTags.";
            CmdSave.Click += CmdSave_Click;
            CmdCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            CmdCancel.Caption = "Cancel and close";
            CmdCancel.ToolTip = "Cancel and lose all changes.";
            CmdCancel.Click += CmdCancel_Click;
        }

        private void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CmdSave_Click(object sender, RoutedEventArgs e)
        {
            List<BlackWhiteListEntry> new_entries = ProcessNewUserEntries();
            web_library_detail.Xlibrary.BlackWhiteListManager.WriteList(new_entries);
            Close();
        }

        private List<BlackWhiteListEntry> ProcessNewUserEntries()
        {
            List<BlackWhiteListEntry> new_entries = (entries == null ? new List<BlackWhiteListEntry>() : new List<BlackWhiteListEntry>(entries));

            // Flag all the old ones as deleted
            foreach (var entry in new_entries)
            {
                entry.is_deleted = true;
            }

            // Process the two new lists entries
            ProcessNewUserEntries_AddEntries(ref new_entries, TxtWhite, BlackWhiteListEntry.ListType.White);
            ProcessNewUserEntries_AddEntries(ref new_entries, TxtBlack, BlackWhiteListEntry.ListType.Black);

            return new_entries;
        }

        private static void ProcessNewUserEntries_AddEntries(ref List<BlackWhiteListEntry> new_entries, TextBox TxtSource, BlackWhiteListEntry.ListType listType)
        {
            string[] lines = TxtSource.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string line_test = line.Trim();
                bool found_entry_to_update = false;
                foreach (var entry in new_entries)
                {
                    if (0 == entry.word.CompareTo(line_test))
                    {
                        entry.list_type = listType;
                        entry.is_deleted = false;
                        found_entry_to_update = true;
                    }
                }
                if (!found_entry_to_update)
                {
                    var entry = new BlackWhiteListEntry(line_test, listType);
                    new_entries.Add(entry);
                }
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            BlackWhiteListEditorWindow w = new BlackWhiteListEditorWindow();
            w.SetLibrary(Library.GuestInstance);
            w.Show();
        }
#endif

        #endregion

        public void SetLibrary(WebLibraryDetail library_)
        {
            // Reset the screen
            web_library_detail = library_;
            entries = null;
            TxtWhite.Text = "";
            TxtBlack.Text = "";

            // Reflect the library
            if (null != web_library_detail)
            {
                entries = web_library_detail.Xlibrary.BlackWhiteListManager.ReadList();
                foreach (var entry in entries)
                {
                    if (entry.is_deleted)
                    {
                        continue;
                    }

                    switch (entry.list_type)
                    {
                        case BlackWhiteListEntry.ListType.White:
                            TxtWhite.Text += entry.word;
                            TxtWhite.Text += "\n";
                            break;
                        case BlackWhiteListEntry.ListType.Black:
                            TxtBlack.Text += entry.word;
                            TxtBlack.Text += "\n";
                            break;
                        default:
                            Logging.Warn("Unknown entry list type: " + entry.list_type);
                            break;
                    }
                }
            }
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
            entries?.Clear();
            entries = null;
        }
    }
}
