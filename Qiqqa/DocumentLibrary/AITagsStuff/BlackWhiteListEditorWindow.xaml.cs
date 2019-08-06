using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using icons;
using Qiqqa.Common.GUI;
using Utilities;

namespace Qiqqa.DocumentLibrary.AITagsStuff
{
    /// <summary>
    /// Interaction logic for BlackWhiteListEditorWindow.xaml
    /// </summary>
    public partial class BlackWhiteListEditorWindow : StandardWindow
    {
        Library library;
        List<BlackWhiteListEntry> entries;

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

        void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void CmdSave_Click(object sender, RoutedEventArgs e)
        {
            List<BlackWhiteListEntry> new_entries = ProcessNewUserEntries();
            library.BlackWhiteListManager.WriteList(new_entries);
            this.Close();
        }

        private List<BlackWhiteListEntry> ProcessNewUserEntries()
        {
            List<BlackWhiteListEntry> new_entries = new List<BlackWhiteListEntry>(entries);

            // Flag all the old ones as deleted
            foreach (var entry in new_entries)
            {
                entry.is_deleted = true;
            }

            // Process the two new lists entries
            ProcessNewUserEntries_AddEntries(new_entries, TxtWhite, BlackWhiteListEntry.ListType.White);
            ProcessNewUserEntries_AddEntries(new_entries, TxtBlack, BlackWhiteListEntry.ListType.Black);
            
            return new_entries;
        }

        private static void ProcessNewUserEntries_AddEntries(List<BlackWhiteListEntry> new_entries, TextBox TxtSource, BlackWhiteListEntry.ListType listType)
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

        public void SetLibrary(Library library_)
        {
            // Reset the screen
            this.library = null;
            this.entries = null;
            this.TxtWhite.Text = "";
            this.TxtBlack.Text = "";
            
            // Reflect the library
            if (null != library)
            {
                this.library = library_;
                entries = library_.BlackWhiteListManager.ReadList();
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
    }
}
