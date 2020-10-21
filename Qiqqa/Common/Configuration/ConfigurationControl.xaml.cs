using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using icons;
using Qiqqa.Common.MessageBoxControls;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.LibraryDBStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Qiqqa.WebBrowsing.EZProxy;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Common.Configuration
{
    /// <summary>
    /// Interaction logic for ConfigurationControl.xaml
    /// </summary>
    public partial class ConfigurationControl : UserControl
    {
        public ConfigurationControl()
        {
            Theme.Initialize();

            InitializeComponent();

            DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;
            Background = ThemeColours.Background_Brush_Blue_LightToDark;

            ButtonOpenDataDirectory.Caption = "Open Qiqqa data directory";
            ButtonOpenDataDirectory.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonOpenDataDirectory.Click += ButtonOpenDataDirectory_Click;

            ButtonOpenTempDirectory.Caption = "Open Qiqqa temp directory";
            ButtonOpenTempDirectory.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonOpenTempDirectory.Click += ButtonOpenTempDirectory_Click;

            ButtonGarbageCollect.Caption = "Run garbage collection";
            ButtonGarbageCollect.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonGarbageCollect.Click += ButtonGarbageCollect_Click;

            ButtonRebuildIndices.Caption = "Rebuild library search indices";
            ButtonRebuildIndices.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonRebuildIndices.Click += ButtonRebuildIndices_Click;

            ButtonPurgeDeletedPDFs.Caption = "Purge deleted PDFs";
            ButtonPurgeDeletedPDFs.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonPurgeDeletedPDFs.Click += ButtonPurgeDeletedPDFs_Click;

            ButtonClearAutoSuggests.Caption = "Clear out all the autosuggestions for title, author and year";
            ButtonClearAutoSuggests.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonClearAutoSuggests.Click += ButtonClearAutoSuggests_Click;

            ButtonSeeDebugStatistics.Caption = "See debug statistics";
            ButtonSeeDebugStatistics.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonSeeDebugStatistics.Click += ButtonSeeDebugStatistics_Click;

            ButtonZipLogs.Caption = "Bundle logs for Qiqqa support";
            ButtonZipLogs.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonZipLogs.Click += ButtonZipLogs_Click;

            ButtonLibraryDBExplorer.Caption = "LibraryDB Explorer";
            ButtonLibraryDBExplorer.Icon = Icons.GetAppIcon(Icons.GarbageCollect);
            ButtonLibraryDBExplorer.Click += ButtonLibraryDBExplorer_Click;

            ObjBlurrySnapToPixels.IsChecked = RegistrySettings.Instance.IsSet(RegistrySettings.SnapToPixels);
            ObjBlurrySnapToPixels.Checked += ObjBlurrySnapToPixels_Checked;
            ObjBlurrySnapToPixels.Unchecked += ObjBlurrySnapToPixels_Unchecked;

            TextQiqqaBaseFolder.Text = ConfigurationManager.Instance.BaseDirectoryForQiqqa;
            TextQiqqaBaseFolder.TextChanged += TextQiqqaBaseFolder_TextChanged;

            CmdClearColour.Click += CmdClearColour_Click;
            ObjColorPicker.SelectedColorChanged += ObjColorPicker_SelectedColorChanged;
            foreach (Color color in ThemeColours.THEMES)
            {
                AugmentedButton button = new AugmentedButton();
                button.Tag = color;
                button.Caption = "   ";
                button.Background = new SolidColorBrush(ColorTools.MakeDarkerColor(color, 1.2));
                button.Click += ThemeColorButton_Click;
                ObjColoursPanel.Children.Add(button);
            }

            ObjCheckAskOnExit.IsEnabled = true;
            ObjRestoreWindowsAtStartup.IsEnabled = true;
            ObjRestoreLocationAtStartup.IsEnabled = true;

            ObjUserAgent_Clear.Click += ObjUserAgent_XXX_Click;
            ObjUserAgent_IE.Click += ObjUserAgent_XXX_Click;
            ObjUserAgent_Chrome.Click += ObjUserAgent_XXX_Click;
            ObjUserAgent_Safari.Click += ObjUserAgent_XXX_Click;

            ObjListEZProxy.ItemsSource = Proxies.Instance.GetProxies();
            ObjListEZProxy.SelectionChanged += ObjListEZProxy_SelectionChanged;
        }

        private void ObjListEZProxy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Proxy proxy = ObjListEZProxy.SelectedItem as Proxy;
            if (null != proxy && !String.IsNullOrEmpty(proxy.url))
            {
                ConfigurationManager.Instance.ConfigurationRecord.Proxy_EZProxy = proxy.url;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.Proxy_EZProxy));
            }

            e.Handled = true;
        }

        private void ObjUserAgent_XXX_Click(object sender, RoutedEventArgs e)
        {
            string user_agent = null;

            if (sender == ObjUserAgent_Clear) user_agent = "";
            else if (sender == ObjUserAgent_IE) user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362";
            else if (sender == ObjUserAgent_Chrome) user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36";
            else if (sender == ObjUserAgent_Safari) user_agent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/603.3.8 (KHTML, like Gecko) Version/10.1.2 Safari/603.3.8";
            // Lynx:  Lynx/2.8.9dev.16 libwww-FM/2.14 SSL-MM/1.4.1 GNUTLS/3.5.17
            // Kindle: Mozilla/5.0 (Linux; U; Android 2.3.4; en-us; Kindle Fire Build/GINGERBREAD) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1
            // WaterFox: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0 Waterfox/56.2.14
            else user_agent = null;

            // Update the config
            ConfigurationManager.Instance.ConfigurationRecord.Web_UserAgentOverride = user_agent;
            ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.Web_UserAgentOverride));
        }

        private void ButtonLibraryDBExplorer_Click(object sender, RoutedEventArgs e)
        {
            LibraryDBExplorer ldbe = new LibraryDBExplorer();
            MainWindowServiceDispatcher.Instance.OpenControl("LibraryDBExplorer", "LibraryDB Explorer", ldbe);
        }

        private void CmdClearColour_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.App_ThemeColour);
            ThemeColours.ClearThemeColour();
        }

        private void ThemeColorButton_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.App_ThemeColour);
            AugmentedButton button = sender as AugmentedButton;
            Color color = (Color)button.Tag;
            ThemeColours.SetThemeColour(color);
        }

        private void ObjColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.App_ThemeColour);
            ThemeColours.SetThemeColour(ObjColorPicker.SelectedColor);
        }

        private void ObjBlurrySnapToPixels_Unchecked(object sender, RoutedEventArgs e)
        {
            WriteBlurryStatus();
        }

        private void ObjBlurrySnapToPixels_Checked(object sender, RoutedEventArgs e)
        {
            WriteBlurryStatus();
        }

        private void WriteBlurryStatus()
        {
            RegistrySettings.Instance.Write(RegistrySettings.SnapToPixels, (ObjBlurrySnapToPixels.IsChecked ?? false) ? "yes" : "no");
        }

        private void TextQiqqaBaseFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegistrySettings.Instance.Write(RegistrySettings.BaseDataDirectory, TextQiqqaBaseFolder.Text);
        }

        private void ButtonSeeDebugStatistics_Click(object sender, RoutedEventArgs e)
        {
            UnhandledExceptionMessageBox.DisplayInfo("Qiqqa debug statistics", "Behold - your debug statistics.", false, null);
        }

        private void ButtonZipLogs_Click(object sender, RoutedEventArgs e)
        {
            BundleLogs.DoBundle();
        }

        private void ButtonClearAutoSuggests_Click(object sender, RoutedEventArgs e)
        {
            foreach (var x in WebLibraryManager.Instance.WebLibraryDetails_All_IncludingDeleted)
            {
                Library library = x.library;

                List<PDFDocument> pdf_documents = library.PDFDocuments_IncludingDeleted;
                foreach (PDFDocument pdf_document in pdf_documents)
                {
                    pdf_document.AutoSuggested_PDFMetadata = false;
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.AutoSuggested_PDFMetadata));

                    pdf_document.AutoSuggested_OCRFrontPage = false;
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.AutoSuggested_OCRFrontPage));

                    if (null != pdf_document.TitleSuggested)
                    {
                        pdf_document.TitleSuggested = null;
                        pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.TitleSuggested));
                    }

                    if (null != pdf_document.AuthorsSuggested)
                    {
                        pdf_document.AuthorsSuggested = null;
                        pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.AuthorsSuggested));
                    }

                    if (null != pdf_document.YearSuggested)
                    {
                        pdf_document.YearSuggested = null;
                        pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.YearSuggested));
                    }
                }
            }
        }

        private void ButtonRebuildIndices_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxes.AskQuestion("Are you sure you wish to rebuild indices?\n\nYou should only need to do this if your indices have become corrupted by a tool like Dropbox, GoogleDrive or LiveDrive.  Please don't use them to try to sync Qiqqa's database."))
            {
                foreach (var x in WebLibraryManager.Instance.WebLibraryDetails_All_IncludingDeleted)
                {
                    x.library.LibraryIndex.InvalidateIndex();
                }

                MessageBoxes.Info("Please restart Qiqqa for your indices to be rebuilt.");
            }
        }

        private void ButtonPurgeDeletedPDFs_Click(object sender, RoutedEventArgs e)
        {
            HashSet<string> filenames_to_keep = new HashSet<string>();
            HashSet<string> filenames_to_delete = new HashSet<string>();

            // Get all the active and deleted files
            foreach (var x in WebLibraryManager.Instance.WebLibraryDetails_All_IncludingDeleted)
            {
                Library library = x.library;

                List<PDFDocument> pdf_documents = library.PDFDocuments_IncludingDeleted;
                foreach (PDFDocument pdf_document in pdf_documents)
                {
                    if (library.WebLibraryDetail.Deleted || pdf_document.Deleted)
                    {
                        if (pdf_document.DocumentExists)
                        {
                            filenames_to_delete.Add(pdf_document.DocumentPath);
                        }
                    }
                    else
                    {
                        filenames_to_keep.Add(pdf_document.DocumentPath);
                    }
                }
            }

            // Remove all the items that we want to keep
            filenames_to_delete.RemoveWhere(o => filenames_to_keep.Contains(o));

            // Ask the user
            if (MessageBoxes.AskQuestion("Are you sure you want to purge {0} PDF files?", filenames_to_delete.Count))
            {
                foreach (string filename in filenames_to_delete)
                {
                    Logging.Info("Purging {0}", filename);
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "There was a problem purging {0}", filename);
                    }
                }
            }
        }

        private void ButtonGarbageCollect_Click(object sender, RoutedEventArgs e)
        {
            Logging.Info("+Before Garbage Collect: Memory load: {0} Bytes", GC.GetTotalMemory(false));
            GC.Collect();
            //GC.WaitForPendingFinalizers();
            GC.Collect();
            Logging.Info("-After Garbage Collect: Memory load: {0} Bytes", GC.GetTotalMemory(true));
        }

        private void ButtonOpenDataDirectory_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ConfigurationManager.Instance.BaseDirectoryForQiqqa);
        }

        private void ButtonOpenTempDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(TempFile.TempDirectoryForQiqqa))
            {
                Process.Start(TempFile.TempDirectoryForQiqqa);
            }
            else
            {
                MessageBoxes.Warn("Hmmmm.  Your computer doesn't seem to have a temp folder called '{0}'.  Please let us know at https://github.com/jimmejardine/qiqqa-open-source/issues if you are getting any strange Qiqqa behaviour as a result of this.", TempFile.TempDirectoryForQiqqa);
            }
        }

        private void Issue225_Link_Click(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            Process.Start(link.NavigateUri.ToString());
        }
    }
}
