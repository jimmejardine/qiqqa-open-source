using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using icons;
using Qiqqa.Common.MessageBoxControls;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.LibraryDBStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.UpgradePaths.V031To033;
using Qiqqa.UtilisationTracking;
using Qiqqa.WebBrowsing.EZProxy;
using Utilities;
using Utilities.GUI;

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

            this.DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;
            this.Background = ThemeColours.Background_Brush_Blue_LightToDark;

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

        void ObjListEZProxy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Proxies.Proxy proxy = ObjListEZProxy.SelectedItem as Proxies.Proxy;
            if (null != proxy && !String.IsNullOrEmpty(proxy.url))
            {
                ConfigurationManager.Instance.ConfigurationRecord.Proxy_EZProxy = proxy.url;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.Proxy_EZProxy);
            }

            e.Handled = true;
        }

        void ObjUserAgent_XXX_Click(object sender, RoutedEventArgs e)
        {
            string user_agent = null;

            if (false) { }
            else if (sender == ObjUserAgent_Clear) user_agent = "";
            else if (sender == ObjUserAgent_IE) user_agent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            else if (sender == ObjUserAgent_Chrome) user_agent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1284.0 Safari/537.13";
            else if (sender == ObjUserAgent_Safari) user_agent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; tr-TR) AppleWebKit/533.20.25 (KHTML, like Gecko) Version/5.0.4 Safari/533.20.27";
            else user_agent = null;

            // Update the config
            ConfigurationManager.Instance.ConfigurationRecord.Web_UserAgentOverride = user_agent;
            ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.Web_UserAgentOverride);
        }

        void ButtonLibraryDBExplorer_Click(object sender, RoutedEventArgs e)
        {
            LibraryDBExplorer ldbe = new LibraryDBExplorer();
            MainWindowServiceDispatcher.Instance.OpenControl("LibraryDBExplorer", "LibraryDB Explorer", ldbe);
        }

        void CmdClearColour_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.App_ThemeColour);
            ThemeColours.ClearThemeColour();
        }

        void ThemeColorButton_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.App_ThemeColour);
            AugmentedButton button = sender as AugmentedButton;
            Color color = (Color)button.Tag;
            ThemeColours.SetThemeColour(color);
        }

        void ObjColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.App_ThemeColour);
            ThemeColours.SetThemeColour(ObjColorPicker.SelectedColor);
        }

        void ObjBlurrySnapToPixels_Unchecked(object sender, RoutedEventArgs e)
        {
            WriteBlurryStatus();
        }

        void ObjBlurrySnapToPixels_Checked(object sender, RoutedEventArgs e)
        {
            WriteBlurryStatus();
        }

        void WriteBlurryStatus()
        {
            RegistrySettings.Instance.Write(RegistrySettings.SnapToPixels, (ObjBlurrySnapToPixels.IsChecked ?? false) ? "yes" : "no");
        }

        void TextQiqqaBaseFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegistrySettings.Instance.Write(RegistrySettings.BaseDataDirectory, TextQiqqaBaseFolder.Text);
        }

        void ButtonSeeDebugStatistics_Click(object sender, RoutedEventArgs e)
        {
            UnhandledExceptionMessageBox.DisplayInfo("Qiqqa debug statistics", "Behold - your debug statistis.", false, null);
        }

        void ButtonZipLogs_Click(object sender, RoutedEventArgs e)
        {
            BundleLogs.DoBundle();
        }

        void ButtonClearAutoSuggests_Click(object sender, RoutedEventArgs e)
        {
            foreach (var x in WebLibraryManager.Instance.WebLibraryDetails_All_IncludingDeleted)
            {
                Library library = x.library;

                List<PDFDocument> pdf_documents = library.PDFDocuments_IncludingDeleted;
                foreach (PDFDocument pdf_document in pdf_documents)
                {
                    pdf_document.AutoSuggested_PDFMetadata = false;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.AutoSuggested_PDFMetadata);

                    pdf_document.AutoSuggested_OCRFrontPage = false;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.AutoSuggested_OCRFrontPage);

                    if (null != pdf_document.TitleSuggested)
                    {
                        pdf_document.TitleSuggested = null;
                        pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.TitleSuggested);
                    }

                    if (null != pdf_document.AuthorsSuggested)
                    {
                    pdf_document.AuthorsSuggested = null;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.AuthorsSuggested);
                    }

                    if (null != pdf_document.YearSuggested)
                    {
                        pdf_document.YearSuggested = null;
                        pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.YearSuggested);
                    }
                }
            }
        }

        void ButtonRebuildIndices_Click(object sender, RoutedEventArgs e)
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


        void ButtonPurgeDeletedPDFs_Click(object sender, RoutedEventArgs e)
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



        void ButtonGarbageCollect_Click(object sender, RoutedEventArgs e)
        {
            Logging.Info("+Before Garbage Collect: Memory load: {0} Bytes", GC.GetTotalMemory(false));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Logging.Info("-After Garbage Collect: Memory load: {0} Bytes", GC.GetTotalMemory(true));
        }

        void ButtonOpenDataDirectory_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ConfigurationManager.Instance.BaseDirectoryForQiqqa);
        }

        void ButtonOpenTempDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(ConfigurationManager.Instance.TempDirectoryForQiqqa))
            {
                Process.Start(ConfigurationManager.Instance.TempDirectoryForQiqqa);
            }
            else
            {
                MessageBoxes.Warn("Hmmmm.  Your computer doesn't seem to have a temp folder called '{0}'.  Please let us know at https://github.com/jimmejardine/qiqqa-open-source/issues if you are getting any strange Qiqqa behaviour as a result of this.", ConfigurationManager.Instance.TempDirectoryForQiqqa);
            }
        }
    }
}
