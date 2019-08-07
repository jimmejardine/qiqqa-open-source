using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Files;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.IntranetLibraryStuff
{
    /// <summary>
    /// Interaction logic for IntranetLibraryChooserControl.xaml
    /// </summary>
    public partial class IntranetLibraryChooserControl : StandardWindow
    {
        public IntranetLibraryChooserControl()
        {
            InitializeComponent();
            
            this.Title = 
            this.Header.Caption = 
                "Create/Join Intranet Library";

            this.Header.SubCaption = "Please confirm the details of the Intranet Library you wish to create or join.";
            this.Header.Img = Icons.GetAppIcon(Icons.WebLibrary_IntranetLibrary);

            this.ButtonCancel.Caption = "Cancel";
            this.ButtonCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            this.ButtonCancel.Click += ButtonCancel_Click;

            this.ButtonJoinCreate.Caption = "Create";
            this.ButtonJoinCreate.Icon = Icons.GetAppIcon(Icons.WebLibrary_IntranetLibrary);
            this.ButtonJoinCreate.Click += ButtonJoinCreate_Click;

            ObjButtonFolderChoose.Click += ObjButtonFolderChoose_Click;
            TxtPath.TextChanged += TxtPath_TextChanged;
        }

        void ButtonJoinCreate_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.StartPage_CreateIntranetLibrary);

            if (String.IsNullOrEmpty(TxtPath.Text))
            {
                MessageBoxes.Error("Please enter a path to your Intranet Library.");
                return;
            }

            bool validation_successful = EnsureIntranetLibraryExists();

            if (validation_successful)
            {
                this.Close();
            }
        }

        void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void TxtPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateFolder();
        }

        private void ValidateFolder()
        {
            try
            {
                this.ButtonJoinCreate.Caption = "Create";

                string base_path = TxtPath.Text;
                if (Directory.Exists(base_path))
                {
                    string library_detail_path = IntranetLibraryTools.GetLibraryDetailPath(base_path);                    
                    if (File.Exists(library_detail_path))
                    {
                        IntranetLibraryDetail library_detail = IntranetLibraryDetail.Read(library_detail_path);
                        TxtTitle.Text = library_detail.Title;
                        TxtDescription.Text = library_detail.Description;
                        this.ButtonJoinCreate.Caption = "Join";
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was an exception while validating the selected Intranet Library path.");
            }
        }

        private bool EnsureIntranetLibraryExists()
        {
            try
            {
                string base_path = TxtPath.Text;
                if (!Directory.Exists(base_path))
                {
                    DirectoryTools.CreateDirectory(base_path);
                }

                EnsureWarningFilesArePresent(base_path);

                // If the file exists, check that we don't need to update it's details
                string library_detail_path = IntranetLibraryTools.GetLibraryDetailPath(base_path);
                if (File.Exists(library_detail_path))
                {
                    try
                    {
                        IntranetLibraryDetail library_detail = IntranetLibraryDetail.Read(library_detail_path);
                        if (library_detail.Title != TxtTitle.Text || library_detail.Description != TxtDescription.Text)
                        {
                            library_detail.Title = TxtTitle.Text;
                            library_detail.Description = TxtDescription.Text;
                            IntranetLibraryDetail.Write(library_detail_path, library_detail);
                        }
                    }

                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was an error while updating an Intranet Library path, so will try to delete and recreate...");
                        FileTools.Delete(library_detail_path);
                    }
                }

                // If the file does not exists, create it from scratch
                if (!File.Exists(library_detail_path))
                {
                    IntranetLibraryDetail library_detail = new IntranetLibraryDetail();
                    library_detail.Id = IntranetLibraryDetail.GetRandomId();
                    library_detail.Title = TxtTitle.Text;
                    library_detail.Description = TxtDescription.Text;
                    IntranetLibraryDetail.Write(library_detail_path, library_detail);
                }

                // If the sync database does not exist, put one in place.
                IntranetLibraryDB db = new IntranetLibraryDB(base_path);

                // Notify the WebLibrearyManager
                WebLibraryManager.Instance.UpdateKnownWebLibraryFromIntranet(base_path);

                return true;
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "Problem accessing Intranet Library for the first time.");
                MessageBoxes.Error("There was a problem while trying to connect to this Intranet Library.  Are you sure you have permission to access this folder?  Your Network or System Administrator can grant you this permission.\n\nThe detailed error message is:\n" + ex.Message);

                return false;
            }
        }

        private void EnsureWarningFilesArePresent(string base_path)
        {
            IEnumerable<string> warning_files = Directory.EnumerateFiles(base_path, "---*");
            if (0 == warning_files.Count())
            {
                EnsureWarningFilesArePresent_TOUCH(base_path, "---0--- --------------------------------------------------------------------------");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---1--- THIS IS A QIQQA INTRANET LIBRARY SYNC FOLDER");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---2--- --------------------------------------------------------------------------");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---3--- PLEASE TREAT IT AS A BLACK BOX DATABASE");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---4--- DO NOT MANUALLY ADD-DELETE-ALTER ANY FILES");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---5--- ALL MODIFICATIONS TO THIS FOLDER SHOULD BE DONE THROUGH QIQQA");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---9--- --------------------------------------------------------------------------");
            }
        }

        private void EnsureWarningFilesArePresent_TOUCH(string base_path, string filename)
        {
            try
            {
                File.WriteAllText(base_path + '/' + filename, "");
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem writing Intranet Library mount point warnings");
            }
        }


        void ObjButtonFolderChoose_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog
                {
                    Description = "Please select the shared directory for your Intranet Library.",
                    SelectedPath = TxtPath.Text,
                    ShowNewFolderButton = true
                })
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TxtPath.Text = dlg.SelectedPath;
                }
            }
        }
    }
}
