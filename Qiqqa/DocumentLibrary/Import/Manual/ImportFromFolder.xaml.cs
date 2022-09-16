using System;
using System.Windows;
using icons;
using Microsoft.WindowsAPICodePack.Dialogs;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.Reflection;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.Import.Manual
{
    /// <summary>
    /// Provides UI for importing documents from a folder.
    /// </summary>
    public partial class ImportFromFolder
    {
        public class Context
        {
            public PropertyDependencies property_dependencies;

            public Context()
            {
                property_dependencies = new PropertyDependencies();
                property_dependencies.Add(() => SelectedPath, () => IsSelectedFileValid);

                RecurseSubfolders = DefaultRecurseSubfolders;
                ImportTagsFromSubfolderNames = DefaultImportTagsFromSubfolderNames;
            }

            public string SelectedPath { get; set; }
            public bool RecurseSubfolders { get; set; }
            public bool ImportTagsFromSubfolderNames { get; set; }

            public bool IsSelectedFileValid
            {
                get
                {
                    var selected_path = SelectedPath;
                    return !string.IsNullOrEmpty(selected_path) && Directory.Exists(selected_path);
                }
            }

            internal string DefaultSelectedPath
            {
                get
                {
                    var last_folder_imported = ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderLastFolderImported;
                    return Directory.Exists(last_folder_imported) ? last_folder_imported : null;
                }
                set
                {
                    var new_value = value;
                    //  only update if the folder exists
                    if (Directory.Exists(new_value))
                    {
                        ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderLastFolderImported = value;
                        ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderLastFolderImported));
                    }
                }
            }

            internal bool DefaultRecurseSubfolders
            {
                get => ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderRecurseSubfolders;
                set
                {
                    ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderRecurseSubfolders = value;
                    ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderRecurseSubfolders));
                }
            }

            internal bool DefaultImportTagsFromSubfolderNames
            {
                get => ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderImportTagsFromSubfolderNames;
                set
                {
                    ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderImportTagsFromSubfolderNames = value;
                    ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.ImportFromFolderImportTagsFromSubfolderNames));
                }
            }
        }

        private readonly WebLibraryDetail web_library_detail;
        private readonly AugmentedBindable<Context> bindable;

        public ImportFromFolder(WebLibraryDetail web_library_detail) : this(web_library_detail, null)
        {
        }

        public ImportFromFolder(WebLibraryDetail web_library_detail, string folder_path)
        {
            this.web_library_detail = web_library_detail;

            InitializeComponent();

            btnCancel.Caption = "Cancel";
            btnCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            btnImport.Caption = "Import";
            btnImport.Icon = Icons.GetAppIcon(Icons.DocumentsAddToLibraryFromFolder);

            Header.Img = Icons.GetAppIcon(Icons.DocumentsAddToLibraryFromFolder);

            Context ctx = new Context();
            ctx.SelectedPath = folder_path;
            bindable = new AugmentedBindable<Context>(ctx, ctx.property_dependencies);

            DataContext = bindable;
        }

        private void FolderLocationButton_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "Please select a folder.  All the PDFs in the folder will be added to your document library.";

                string default_folder = bindable.Underlying.DefaultSelectedPath;
                if (default_folder != null)
                {
                    dialog.DefaultDirectory = default_folder;
                }

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    bindable.Underlying.SelectedPath = dialog.FileName;
                    bindable.NotifyPropertyChanged(nameof(bindable.Underlying.SelectedPath));
                    Logging.Debug特("User selected import folder path: " + bindable.Underlying.SelectedPath);
                }
                else
                {
                    Logging.Debug特("User canceled directory selection. Import folder path: " + bindable.Underlying.SelectedPath);
                }
            }
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnImport_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var root_folder = bindable.Underlying.SelectedPath;
                if (!Directory.Exists(root_folder)) return;

                // do the import
                ImportingIntoLibrary.AddNewPDFDocumentsToLibraryFromFolder_ASYNCHRONOUS(web_library_detail, root_folder, bindable.Underlying.RecurseSubfolders, bindable.Underlying.ImportTagsFromSubfolderNames, false);

                // remember settings for next time
                bindable.Underlying.DefaultSelectedPath = root_folder;
                bindable.Underlying.DefaultRecurseSubfolders = bindable.Underlying.RecurseSubfolders;
                bindable.Underlying.DefaultImportTagsFromSubfolderNames = bindable.Underlying.ImportTagsFromSubfolderNames;

                Close();
            }
            catch (Exception exception)
            {
                Logging.Error(exception, "Problem importing files from {0}", bindable.Underlying.SelectedPath);
            }
        }
    }
}
