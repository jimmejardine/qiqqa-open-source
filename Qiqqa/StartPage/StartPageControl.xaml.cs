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

            DualTabWhatsNew.Children.Clear();
            DualTabWhatsNew.AddContent("Webcasts", "Tutorials & Help", null, false, false, TabWebcasts);

            DualTabWhatsNew.TabPosition = DualTabbedLayout.TabPositions.Sides;

            KeyDown += StartPageControl_KeyDown;

            bool ADVANCED_MENUS = ConfigurationManager.Instance.ConfigurationRecord.GUI_AdvancedMenus;

            // Connect the dropdowns
            ButtonLibraries.AttachPopup(ButtonLibrariesPopup);
            ButtonLibraries.Icon = Icons.GetAppIcon(Icons.ModuleDocumentLibrary);
            if (!ADVANCED_MENUS) ButtonLibraries.Caption = LocalisationManager.Get("START/CAP/POPUP_LIBRARIES");
            ButtonLibraries.ToolTip = LocalisationManager.Get("START/TIP/POPUP_LIBRARIES");

            ButtonTools.AttachPopup(ButtonToolsPopup);
            ButtonTools.Icon = Icons.GetAppIcon(Icons.ModuleConfiguration);
            if (!ADVANCED_MENUS) ButtonTools.Caption = LocalisationManager.Get("START/CAP/POPUP_TOOLS");
            ButtonTools.ToolTip = LocalisationManager.Get("START/TIP/POPUP_TOOLS");

            ButtonHelp.AttachPopup(ButtonHelpPopup);
            ButtonHelp.Icon = Icons.GetAppIcon(Icons.ModuleHelp);
            if (!ADVANCED_MENUS) ButtonHelp.Caption = LocalisationManager.Get("START/CAP/POPUP_HELP");
            ButtonHelp.ToolTip = LocalisationManager.Get("START/TIP/POPUP_HELP");

            // Then the buttons
            ButtonSync.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonSync.Icon = Icons.GetAppIcon(Icons.WebLibrarySyncAll);
            ButtonSync.Caption = LocalisationManager.Get("START/CAP/SYNC_LIBRARIES");
            ButtonSync.ToolTip = LocalisationManager.Get("START/TIP/SYNC_LIBRARIES");
            ButtonSync.Click += ButtonSync_Click;

            ButtonCreateIntranetLibrary.Icon = Icons.GetAppIcon(Icons.WebLibrary_IntranetLibrary);
            ButtonCreateIntranetLibrary.Caption = LocalisationManager.Get("START/CAP/CREATE_INTRANET_LIBRARY");
            ButtonCreateIntranetLibrary.ToolTip = LocalisationManager.Get("START/TIP/CREATE_INTRANET_LIBRARY");
            ButtonCreateIntranetLibrary.Click += ButtonCreateIntranetLibrary_Click;

            ButtonJoinBundleLibrary.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonJoinBundleLibrary.Icon = Icons.GetAppIcon(Icons.WebLibrary_BundleLibrary);
            ButtonJoinBundleLibrary.Caption = LocalisationManager.Get("START/CAP/JOIN_BUNDLE_LIBRARY");
            ButtonJoinBundleLibrary.ToolTip = LocalisationManager.Get("START/TIP/JOIN_BUNDLE_LIBRARY");
            ButtonJoinBundleLibrary.Click += ButtonJoinBundleLibrary_Click;

            ButtonNewBrainstorm.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonNewBrainstorm.Icon = Icons.GetAppIcon(Icons.ModuleBrainstorm);
            if (!ADVANCED_MENUS) ButtonNewBrainstorm.Caption = LocalisationManager.Get("START/CAP/BRAINSTORM");
            ButtonNewBrainstorm.ToolTip = LocalisationManager.Get("START/TIP/BRAINSTORM");
            ButtonNewBrainstorm.Click += ButtonNewBrainstorm_Click;

            ButtonNewBrowser.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonNewBrowser.Icon = Icons.GetAppIcon(Icons.ModuleWebBrowser);
            if (!ADVANCED_MENUS) ButtonNewBrowser.Caption = LocalisationManager.Get("START/CAP/BROWSER");
            ButtonNewBrowser.ToolTip = LocalisationManager.Get("START/TIP/BROWSER");
            ButtonNewBrowser.Click += ButtonNewBrowser_Click;

            ButtonOpenLibrary.Icon = Icons.GetAppIcon(Icons.ModuleDocumentLibrary);
            ButtonOpenLibrary.Caption = LocalisationManager.Get("START/CAP/OPEN_LIBRARY");
            ButtonOpenLibrary.ToolTip = LocalisationManager.Get("START/TIP/OPEN_LIBRARY");
            ButtonOpenLibrary.Click += ButtonOpenLibrary_Click;

            ButtonInCite.Icon = Icons.GetAppIcon(Icons.ModuleInCite);
            if (!ADVANCED_MENUS) ButtonInCite.Caption = LocalisationManager.Get("START/CAP/INCITE");
            ButtonInCite.ToolTip = LocalisationManager.Get("START/TIP/INCITE");
            ButtonInCite.Click += ButtonInCite_Click;

            ButtonExpedition.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonExpedition.Icon = Icons.GetAppIcon(Icons.ModuleExpedition);
            if (!ADVANCED_MENUS) ButtonExpedition.Caption = LocalisationManager.Get("START/CAP/EXPEDITION"); ;
            ButtonExpedition.ToolTip = LocalisationManager.Get("START/TIP/EXPEDITION");
            ButtonExpedition.Click += ButtonExpedition_Click;

            ButtonToggleOCR.Icon = Icons.GetAppIcon(Icons.LibraryDoOCR);
            ButtonToggleOCR.Caption = LocalisationManager.Get("START/CAP/OCR"); ;
            ButtonToggleOCR.ToolTip = LocalisationManager.Get("START/TIP/OCR_ON");
            ButtonToggleOCR.ToolTipOpening += ButtonToggleOCR_ToolTipOpening;

            ButtonNewConfig.Icon = Icons.GetAppIcon(Icons.ModuleConfiguration);
            ButtonNewConfig.Caption = LocalisationManager.Get("START/CAP/CONFIG");
            ButtonNewConfig.ToolTip = LocalisationManager.Get("START/TIP/CONFIG");
            ButtonNewConfig.Click += ButtonNewConfig_Click;

            ButtonExpertMode.Icon = Icons.GetAppIcon(Icons.BibTeXSnifferWizard);
            if (!ADVANCED_MENUS) ButtonExpertMode.Caption = LocalisationManager.Get("START/CAP/EXPERT_MODE");
            ButtonExpertMode.ToolTip = LocalisationManager.Get("START/TIP/EXPERT_MODE");
            ButtonExpertMode.Click += ButtonExpertMode_Click;
            ButtonExpertMode.Visibility = ConfigurationManager.Instance.ConfigurationRecord.GUI_IsNovice ? Visibility.Visible : Visibility.Collapsed;

            ButtonNewHelp.Icon = Icons.GetAppIcon(Icons.ModuleHelp);
            ButtonNewHelp.Caption = LocalisationManager.Get("START/CAP/HELP");
            ButtonNewHelp.ToolTip = LocalisationManager.Get("START/TIP/HELP");
            ButtonNewHelp.Click += ButtonNewHelp_Click;

            ButtonNewManual.Icon = Icons.GetAppIcon(Icons.Manual);
            ButtonNewManual.Caption = LocalisationManager.Get("START/CAP/MANUAL");
            ButtonNewManual.ToolTip = LocalisationManager.Get("START/TIP/MANUAL");
            ButtonNewManual.Click += ButtonNewManual_Click;

            ButtonWelcomeWizard.Icon = Icons.GetAppIcon(Icons.BibTeXSnifferWizard);
            ButtonWelcomeWizard.Caption = LocalisationManager.Get("START/CAP/WELCOME_WIZARD");
            ButtonWelcomeWizard.ToolTip = LocalisationManager.Get("START/TIP/WELCOME_WIZARD");
            ButtonWelcomeWizard.Click += ButtonWelcomeWizard_Click;

            ButtonBackupRestore.Icon = Icons.GetAppIcon(Icons.Backup);
            ButtonBackupRestore.Caption = LocalisationManager.Get("START/CAP/BACKUPRESTORE");
            ButtonBackupRestore.ToolTip = LocalisationManager.Get("START/TIP/BACKUPRESTORE");
            ButtonBackupRestore.Click += ButtonBackupRestore_Click;

            ButtonNewAbout.Icon = Icons.GetAppIcon(Icons.About);
            ButtonNewAbout.Caption = LocalisationManager.Get("START/CAP/ABOUT");
            ButtonNewAbout.ToolTip = LocalisationManager.Get("START/TIP/ABOUT");
            ButtonNewAbout.Click += ButtonNewAbout_Click;

            ButtonTranslate.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonTranslate.Icon = Icons.GetAppIcon(Icons.ModuleLocalisation);
            ButtonTranslate.Caption = LocalisationManager.Get("START/CAP/LOCALISE");
            ButtonTranslate.ToolTip = LocalisationManager.Get("START/TIP/LOCALISE");
            ButtonTranslate.Click += ButtonTranslate_Click;

            ButtonZipLogs.Icon = Icons.GetAppIcon(Icons.ZipLogs);
            ButtonZipLogs.Caption = LocalisationManager.Get("START/CAP/ZIPLOGS");
            ButtonZipLogs.ToolTip = LocalisationManager.Get("START/TIP/ZIPLOGS");
            ButtonZipLogs.Click += ButtonZipLogs_Click;

            ButtonDocumentConvert.Visibility = ConfigurationManager.Instance.NoviceVisibility;
            ButtonDocumentConvert.Icon = Icons.GetAppIcon(Icons.DocumentTypePdf);
            ButtonDocumentConvert.Caption = LocalisationManager.Get("START/CAP/DOCUMENT_CONVERT");
            ButtonDocumentConvert.ToolTip = LocalisationManager.Get("START/TIP/DOCUMENT_CONVERT");
            ButtonDocumentConvert.Click += ButtonDocumentConvert_Click;

            ObjSearch.OnHardSearch += ObjSearch_OnHardSearch;
            ObjSearch.SearchHistoryItemSource = ConfigurationManager.Instance.SearchHistory;

            DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            ObjGlobalSearchPanel.Visibility = ConfigurationManager.Instance.NoviceVisibility;
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

        private void ButtonTranslate_Click(object sender, RoutedEventArgs e)
        {
            ButtonToolsPopup.Close();
            MainWindowServiceDispatcher.Instance.OpenLocalisationEditing();
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
