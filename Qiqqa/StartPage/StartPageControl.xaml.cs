using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Backups;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Localisation;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.GUI.DualTabbedLayoutStuff;

namespace Qiqqa.StartPage
{
    /// <summary>
    /// Interaction logic for StartPageControl.xaml
    /// </summary>
    public partial class StartPageControl : UserControl, IDisposable
    {
        public StartPageControl()
        {
            InitializeComponent();

            KeyDown += StartPageControl_KeyDown;

            bool ADVANCED_MENUS = ConfigurationManager.Instance.ConfigurationRecord.GUI_AdvancedMenus;
            if (ADVANCED_MENUS)
            {
                ButtonLibraries.Caption = "";
                ButtonTools.Caption = "";
                ButtonNewBrainstorm.Caption = "";
                ButtonInCite.Caption = "";
                ButtonExpedition.Caption = "";
                ButtonExpertMode.Caption = "";
            }

            // Connect the dropdowns
            ButtonLibraries.AttachPopup(ButtonLibrariesPopup);
            ButtonTools.AttachPopup(ButtonToolsPopup);
            ButtonHelp.AttachPopup(ButtonHelpPopup);

            // Then the buttons
            ButtonSync.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonJoinBundleLibrary.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonNewBrainstorm.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonNewBrowser.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonExpedition.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonExpertMode.Visibility = ConfigurationManager.Instance.ConfigurationRecord.GUI_IsNovice ? Visibility.Visible : Visibility.Collapsed;
            ButtonDocumentConvert.Visibility = ConfigurationManager.Instance.NoviceVisibility;

            ObjSearch.SearchHistoryItemSource = ConfigurationManager.Instance.SearchHistory;
            ObjGlobalSearchPanel.Visibility = ConfigurationManager.Instance.NoviceVisibility;

            ButtonToggleOCR.ToolTipOpening += ButtonToggleOCR_ToolTipOpening;
            ObjSearch.OnHardSearch += ObjSearch_OnHardSearch;

            DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;
        }

        private void StartPageControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.F == e.Key && KeyboardTools.IsCTRLDown())
            {
                ObjSearch.FocusSearchArea();
                e.Handled = true;
            }
        }

        private void ButtonBackupRestore_Click(object sender, RoutedEventArgs e)
        {
            ButtonToolsPopup.Close();
            BackingUp.DoBackupRestoreInstructions();
        }

        private void ObjSearch_OnHardSearch()
        {
            string query = ObjSearch.Text;
            if (!String.IsNullOrEmpty(query))
            {
                MainWindowServiceDispatcher.Instance.OpenCrossLibrarySearch(query);
            }
        }

        private void ButtonCreateIntranetLibrary_Click(object sender, RoutedEventArgs e)
        {
            ButtonLibrariesPopup.Close();

            IntranetLibraryChooserControl control = new IntranetLibraryChooserControl();
            control.Show();
        }

        private void ButtonJoinBundleLibrary_Click(object sender, RoutedEventArgs e)
        {
            ButtonLibrariesPopup.Close();
            MainWindowServiceDispatcher.Instance.ShowBundleLibraryJoiningControl();
        }

        private void ButtonOpenLibrary_Click(object sender, RoutedEventArgs e)
        {
            ButtonLibrariesPopup.Close();

            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != web_library_detail)
            {
                MainWindowServiceDispatcher.Instance.OpenLibrary(web_library_detail.library);
            }
            e.Handled = true;
        }

        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            ButtonLibrariesPopup.Close();

            ObjWebLibraries.DoSync();
            e.Handled = true;
        }

        private void ButtonNewBrainstorm_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenNewBrainstorm();
        }

        private void ButtonNewBrowser_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenWebBrowser();
        }

        private void ButtonNewConfig_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenControlPanel();
        }

        private void ButtonNewHelp_Click(object sender, RoutedEventArgs e)
        {
            ButtonHelpPopup.Close();
            MainWindowServiceDispatcher.Instance.OpenHelp();
        }

        private void ButtonInCite_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenInCite();
        }

        private void ButtonExpedition_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Expedition_Open_StartPage);
            MainWindowServiceDispatcher.Instance.OpenExpedition(null, null);
        }

        private void ButtonToggleOCR_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Logging.Info("Library_OCRDisabled = {0}", ConfigurationManager.Instance.ConfigurationRecord.Library_OCRDisabled);
            if (ConfigurationManager.Instance.ConfigurationRecord.Library_OCRDisabled)
            {
                ButtonToggleOCR.ToolTip = LocalisationManager.Get("START/TIP/OCR_OFF");
            }
            else
            {
                ButtonToggleOCR.ToolTip = LocalisationManager.Get("START/TIP/OCR_ON");
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        internal void OpenBrowser(string url)
        {
            WebsiteAccess.OpenWebsite(url);
        }

        internal void OpenFeedback()
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Feedback);
        }

        internal void OpenWwwQiqqaCom()
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Home);
        }

        private void ButtonNewManual_Click(object sender, RoutedEventArgs e)
        {
            ButtonHelpPopup.Close();
            PDFDocument pdf_document = QiqqaManualTools.AddManualsToLibrary(WebLibraryManager.Instance.WebLibraryDetails_Guest.library);
            MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
        }

        private void ButtonWelcomeWizard_Click(object sender, RoutedEventArgs e)
        {
            ButtonHelpPopup.Close();
            MainWindowServiceDispatcher.Instance.OpenWelcomeWizard();
        }

        private void ButtonNewAbout_Click(object sender, RoutedEventArgs e)
        {
            ButtonHelpPopup.Close();
            MainWindowServiceDispatcher.Instance.OpenAbout();
        }

        private void ButtonZipLogs_Click(object sender, RoutedEventArgs e)
        {
            ButtonToolsPopup.Close();
            BundleLogs.DoBundle();
        }

        private void ButtonDocumentConvert_Click(object sender, RoutedEventArgs e)
        {
            ButtonToolsPopup.Close();
            MainWindowServiceDispatcher.Instance.OpenDocumentConvert();
        }

        private void ButtonExpertMode_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.GoExpertMode();
        }

        ~StartPageControl()
        {
            Logging.Debug("~StartPageControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing StartPageControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("StartPageControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                DataContext = null;
            });

            ++dispose_count;
        }
    }
}
