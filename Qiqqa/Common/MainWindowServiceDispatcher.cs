using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using icons;
using Qiqqa.AnnotationsReportBuilding;
using Qiqqa.Brainstorm.Nodes;
using Qiqqa.Brainstorm.SceneManager;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.Common.SpeedRead;
using Qiqqa.DocumentConversionStuff;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.BundleLibrary;
using Qiqqa.DocumentLibrary.BundleLibrary.LibraryBundleDownloading;
using Qiqqa.DocumentLibrary.CrossLibrarySearchStuff;
using Qiqqa.DocumentLibrary.LibraryPivotReport;
using Qiqqa.DocumentLibrary.OmnipatentsImport;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls;
using Qiqqa.Expedition;
using Qiqqa.InCite;
using Qiqqa.InCite.CSLEditorStuff;
using Qiqqa.Localisation;
using Qiqqa.Main;
using Qiqqa.StartPage;
using Qiqqa.UtilisationTracking;
using Qiqqa.WebBrowsing;
using Qiqqa.Wizards;
using Qiqqa.Wizards.AnnotationReport;
using Utilities;
using Utilities.GUI;
using Utilities.GUI.Wizard;
using Utilities.Internet;
using Utilities.Language;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Common
{
    public class MainWindowServiceDispatcher
    {
        public static MainWindowServiceDispatcher Instance = new MainWindowServiceDispatcher();

        private MainWindow main_window;

        public MainWindow MainWindow
        {
            set => main_window = value;
            get => main_window;
        }

        public ConfigurationControl OpenControlPanel()
        {
            const string window_key = "ControlPanel";
            ConfigurationControl existing_control = (ConfigurationControl)main_window.DockingManager.MakeActive(window_key);
            if (null != existing_control)
            {
                return existing_control;
            }
            else
            {
                ConfigurationControl configuration_control = new ConfigurationControl();
                main_window.DockingManager.AddContent(window_key, "Configuration (Shift-F1)", Icons.GetAppIcon(Icons.ModuleConfiguration), true, true, configuration_control);
                return configuration_control;
            }
        }

        public LibraryControl OpenLibrary(Library library)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            string window_key = "Library-" + library.WebLibraryDetail.Id;

            LibraryControl existing_control = (LibraryControl)main_window.DockingManager.MakeActive(window_key);
            if (null != existing_control)
            {
                Logging.Info("OpenLibrary: (ExistingControl) Library {0} has {1} documents loaded", library.WebLibraryDetail.Title, library.PDFDocuments_IncludingDeleted_Count);
                return existing_control;
            }
            else
            {
                LibraryControl library_control = new LibraryControl(library);
                main_window.DockingManager.AddContent(window_key, library.WebLibraryDetail.Title, Icons.GetAppIcon(Icons.ModuleDocumentLibrary), true, true, library_control);
                Logging.Info("OpenLibrary: Library {0} has {1} documents loaded", library.WebLibraryDetail.Title, library.PDFDocuments_IncludingDeleted_Count);
                return library_control;
            }
        }

        public InCiteControl OpenInCite()
        {
            const string window_key = "InCite";

            InCiteControl existing_control = (InCiteControl)main_window.DockingManager.MakeActive(window_key);
            if (null != existing_control)
            {
                return existing_control;
            }
            else
            {
                InCiteControl incite_control = new InCiteControl();
                main_window.DockingManager.AddContent(window_key, "InCite", Icons.GetAppIcon(Icons.ModuleInCite), true, true, incite_control);
                return incite_control;
            }
        }

        internal void OpenPopupInCite()
        {
            PopupInCiteControl.Popup();
        }

        public ExpeditionControl OpenExpedition(Library library)
        {
            return OpenExpedition(library, null);
        }

        public ExpeditionControl OpenExpedition(Library library, PDFDocument pdf_document)
        {
            ExpeditionControl expedition_control = new ExpeditionControl();

            // Set the optional selections
            if (null != library)
            {
                expedition_control.ChooseNewLibrary(library.WebLibraryDetail);
            }
            if (null != pdf_document)
            {
                expedition_control.ChooseNewPDFDocument(pdf_document);
            }

            main_window.DockingManager.AddContent("Expedition" + Guid.NewGuid(), "Expedition", Icons.GetAppIcon(Icons.ModuleExpedition), true, true, expedition_control);
            return expedition_control;
        }

        internal LibraryPivotReportControl OpenPivot(Library library, List<PDFDocument> pdf_documents)
        {
            LibraryPivotReportControl lprc = new LibraryPivotReportControl();
            lprc.Library = library;
            lprc.PDFDocuments = pdf_documents;

            main_window.DockingManager.AddContent("Pivot" + Guid.NewGuid(), "Pivot", Icons.GetAppIcon(Icons.LibraryPivot), true, true, lprc);
            return lprc;
        }


        public void OpenUserControl(string title, BitmapImage icon, UserControl control)
        {
            main_window.DockingManager.AddContent(title + "-" + Guid.NewGuid(), title, icon, true, true, control);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public PDFReadingControl OpenDocument(PDFDocument pdf_document, int? page = null, string search_terms = null, bool open_again = false)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            if (pdf_document.IsVanillaReference)
            {
                var dialog = new AssociatePDFWithVanillaReferenceWindow(pdf_document);
                dialog.ShowDialog();
                return null;
            }

            FeatureTrackingManager.Instance.UseFeature(
                Features.Document_Open,
                "DocumentFingerprint", pdf_document.Fingerprint,
                "LibraryId", pdf_document.Library.WebLibraryDetail.Id
            );

            // If the document doesn't exist, check if it has a url bibtex field.  If so, prompt to go there to find the doc
            if (!File.Exists(pdf_document.DocumentPath))
            {
                const string URL_FIELD = "url";
                if (null != pdf_document.BibTexItem && pdf_document.BibTexItem.ContainsField(URL_FIELD))
                {
                    if (MessageBoxes.AskQuestion("You do not have the PDF file associated with this document.  However the document metadata has a URL link.  Do you want to visit that web page to perhaps download it?"))
                    {
                        string url = pdf_document.BibTexItem[URL_FIELD];
                        Instance.OpenUrlInBrowser(url);
                    }
                }
                else
                {
                    MessageBoxes.Info("You do not have the PDF file associated with this document.  Perhaps you need to sync with your Web/Intranet Library to fetch it?");
                }

                return null;
            }

            // Mark as recently read
            pdf_document.Library.RecentlyReadManager.AddRecentlyRead(pdf_document);

            // Add to most recently used
            pdf_document.DateLastRead = DateTime.UtcNow;
            pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.DateLastRead));

            // Set the opening page, if necessary
            if (page.HasValue)
            {
                pdf_document.PageLastRead = page.Value;
            }

            // Cause all pages to be OCRed
            pdf_document.PDFRenderer.CauseAllPDFPagesToBeOCRed();

            // Create a title for the window
            string title = "PDF " + pdf_document.Fingerprint;
            {
                StringBuilder sb = new StringBuilder();

                List<NameTools.Name> names = NameTools.SplitAuthors(pdf_document.AuthorsCombined);
                if (0 < names.Count && names[0] != NameTools.Name.UNKNOWN_NAME)
                {
                    sb.Append(names[0].last_name);
                }

                if (!String.IsNullOrEmpty(pdf_document.YearCombined))
                {
                    sb.Append(pdf_document.YearCombined);
                }

                if (!String.IsNullOrEmpty(pdf_document.TitleCombined))
                {
                    if (0 < sb.Length) sb.Append(", ");
                    sb.Append(pdf_document.TitleCombined);
                }

                if (0 < sb.Length)
                {
                    title = sb.ToString();
                }
            }

            // Open the window
            PDFReadingControl pdf_reading_control = null;
            if (!open_again && main_window.DockingManager.Contains(pdf_document.UniqueId))
            {
                Logging.Info("Activating existing PDF viewer for '{0}' with fingerprint {1}", title, pdf_document.Fingerprint);
                FrameworkElement fe = main_window.DockingManager.MakeActive(pdf_document.UniqueId);
                pdf_reading_control = fe as PDFReadingControl;
            }
            else
            {
                Logging.Info("Opening new PDF viewer for {0}", title);
                pdf_reading_control = new PDFReadingControl(pdf_document);

                // Shall we colour the tab header?
                Color? header_color = pdf_document.Color;
                main_window.DockingManager.AddContent(pdf_document.UniqueId, title, Icons.GetAppIcon(Icons.ModulePDFViewer), true, true, pdf_reading_control, header_color);
            }

            // Select the current search terms
            if (null != pdf_reading_control)
            {
                if (!String.IsNullOrEmpty(search_terms))
                {
                    pdf_reading_control.SetSearchKeywords(search_terms);
                }

                if (page.HasValue)
                {
                    pdf_reading_control.SelectPage(page.Value);
                }
            }

            return pdf_reading_control;
        }

        public void GenerateAnnotationReport(Library library, List<PDFDocument> pdf_documents)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_AnnotationReport);

            AnnotationReportOptionsWindow arow = new AnnotationReportOptionsWindow();
            arow.ShowTagOptions(library, pdf_documents, OnShowTagOptionsComplete);
        }

        private void OnShowTagOptionsComplete(Library library, List<PDFDocument> pdf_documents, AnnotationReportOptions annotation_report_options)
        {
            var annotation_report = AsyncAnnotationReportBuilder.BuildReport(library, pdf_documents, annotation_report_options);
            ReportViewerControl report_view_control = new ReportViewerControl(annotation_report);
            string title = String.Format("Annotation report at {0}", DateTime.UtcNow.ToShortTimeString());
            OpenNewWindow(title, Icons.GetAppIcon(Icons.ModulePDFAnnotationReport), true, true, report_view_control);
        }

        public void OpenNewWindow(string header, BitmapImage icon, bool can_close, bool can_floating, FrameworkElement content)
        {
            if (null != main_window)
            {
                main_window.DockingManager.AddContent(null, header, icon, can_close, can_floating, content);
            }
            else
            {
                Logging.Debug特("MainWindow is not present, so spawning independent window");
                ControlHostingWindow window = new ControlHostingWindow(header, content);
                window.Title = header;
                window.Show();
            }
        }

        internal void ProcessCommandLineFile(string filename)
        {
            if (filename.EndsWith(DocumentLibrary.BundleLibrary.Common.EXT_BUNDLE_MANIFEST))
            {
                ShowBundleLibraryJoiningControl(filename);
            }
            else if (filename.EndsWith(DocumentLibrary.OmnipatentsImport.Common.EXT_IMPORT_FROM_OMNIPATENTS))
            {
                ImportManager.Go(filename);
            }
            else if (filename.StartsWith("qiqqa://"))
            {
                URLProtocolHandler.Go(filename);
            }
        }

        internal void ShowBundleLibraryJoiningControl()
        {
            BundleLibraryJoiningControl control = new BundleLibraryJoiningControl();
            control.Show();
        }

        internal void ShowBundleLibraryJoiningControl(string filename)
        {
            BundleLibraryJoiningControl control = new BundleLibraryJoiningControl();
            control.Show();
            control.FocusOnManifestFilename(filename);
        }

        internal void ShowBundleLibraryJoiningControl(BundleLibraryManifest manifest)
        {
            BundleLibraryJoiningControl control = new BundleLibraryJoiningControl();
            control.Show();
            control.FocusOnManifest(manifest, "Automatically downloaded manifest...");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        internal BrainstormControl OpenNewBrainstorm()
        {
            BrainstormControl brainstorm_control = new BrainstormControl();
            main_window.DockingManager.AddContent("Brainstorm" + Guid.NewGuid(), "Brainstorm", Icons.GetAppIcon(Icons.ModuleBrainstorm), true, true, brainstorm_control);
            return brainstorm_control;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        internal BrainstormControl OpenSampleBrainstorm()
        {
            BrainstormControl brainstorm_control = new BrainstormControl();
            brainstorm_control.OpenSample();
            main_window.DockingManager.AddContent("Brainstorm" + Guid.NewGuid(), "Brainstorm", Icons.GetAppIcon(Icons.ModuleBrainstorm), true, true, brainstorm_control);
            return brainstorm_control;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public WebBrowserHostControl OpenWebBrowser()
        {
            const string window_key = "WebBrowser";

            WebBrowserHostControl existing_control = (WebBrowserHostControl)main_window.DockingManager.MakeActive(window_key);
            if (null != existing_control)
            {
                return existing_control;
            }
            else
            {
                WebBrowserHostControl web_browser_control = new WebBrowserHostControl();
                main_window.DockingManager.AddContent(window_key, "Web Browser", Icons.GetAppIcon(Icons.ModuleWebBrowser), true, true, web_browser_control);
                return web_browser_control;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public void SearchDictionary(string query)
        {
            WebBrowserHostControl web_browser_control = OpenWebBrowser();
            web_browser_control.DoDictionarySearch(query);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public void SearchWeb(string query)
        {
            WebBrowserHostControl web_browser_control = OpenWebBrowser();
            web_browser_control.DoWebSearch(query);
        }

        public void OpenUrlInBrowser(string url)
        {
            OpenUrlInBrowser(url, false);
        }

        public void OpenUrlInBrowser(string url, bool force_external_browser)
        {
            if (force_external_browser || ConfigurationManager.Instance.ConfigurationRecord.System_UseExternalWebBrowser)
            {
                BrowserStarter.OpenBrowser(url);
            }
            else
            {
                WebBrowserHostControl web_browser_control = OpenWebBrowser();
                web_browser_control.OpenUrl(url);
            }
        }

        public void SearchLibrary(Library library, string query)
        {
            LibraryControl library_control = OpenLibrary(library);
            library_control.SearchLibrary(query);
        }

        public void ShutdownQiqqa(bool suppress_exit_warning)
        {
            if (main_window != null)
            {
                main_window.suppress_exit_warning = suppress_exit_warning;
                main_window.Close();
            }
            else
            {
                Logging.Error("Forcibly shutting down Qiqqa (no main window present yet)");
                Application app = Application.Current;
                Window win = app.MainWindow;
                if (win != null)
                {
                    win.Close();
                }
                app.Shutdown(9);
            }
        }

        internal void OpenQiqqaWebsite()
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Home);
        }

        internal void OpenHelp()
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Help);
        }

        internal AboutControl OpenAbout()
        {
            const string window_key = "About";
            AboutControl existing_control = (AboutControl)main_window.DockingManager.MakeActive(window_key);
            if (null != existing_control)
            {
                return existing_control;
            }
            else
            {
                AboutControl about_control = new AboutControl();
                main_window.DockingManager.AddContent(window_key, "About Qiqqa", Icons.GetAppIcon(Icons.About), true, true, about_control);
                return about_control;
            }
        }

        internal void OpenLicensesDirectory()
        {
            Process.Start(Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"licenses")));
        }


        internal void ExploreDocumentInBrainstorm(PDFDocument pdf_document)
        {
            List<PDFDocument> pdf_documents = new List<PDFDocument>();
            pdf_documents.Add(pdf_document);
            ExploreDocumentInBrainstorm(pdf_documents);
        }

        internal void ExploreDocumentInBrainstorm(List<PDFDocument> pdf_documents)
        {
            BrainstormControl brainstorm_control = Instance.OpenNewBrainstorm();
            List<object> contents = new List<object>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                PDFDocumentNodeContent content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.Library.WebLibraryDetail.Id);
                contents.Add(content);
            }
            List<NodeControl> node_controls = brainstorm_control.SceneRenderingControl.AddNewNodeControlsInScreenCentre(contents);
            brainstorm_control.AutoArrange(true);

            // Then expand the interesting documents
            {
                // Authors and themes
                brainstorm_control.SceneRenderingControl.SelectAll();
                brainstorm_control.SceneRenderingControl.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(brainstorm_control.SceneRenderingControl), 0, Key.A) { RoutedEvent = Keyboard.KeyDownEvent });
                brainstorm_control.SceneRenderingControl.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(brainstorm_control.SceneRenderingControl), 0, Key.M) { RoutedEvent = Keyboard.KeyDownEvent });

                // All attached docs
                brainstorm_control.SceneRenderingControl.SelectAll();
                brainstorm_control.SceneRenderingControl.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(brainstorm_control.SceneRenderingControl), 0, Key.D) { RoutedEvent = Keyboard.KeyDownEvent });
                brainstorm_control.SceneRenderingControl.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(brainstorm_control.SceneRenderingControl), 0, Key.S) { RoutedEvent = Keyboard.KeyDownEvent });
            }

            brainstorm_control.SceneRenderingControl.SetSelectedNodeControls(node_controls, false);


        }

        internal void ExploreTagInBrainstorm(string library_id, string tag)
        {
            BrainstormControl brainstorm_control = Instance.OpenNewBrainstorm();
            PDFTagNodeContent content = new PDFTagNodeContent(library_id, tag);
            NodeControl node_control = brainstorm_control.SceneRenderingControl.AddNewNodeControlInScreenCentre(content);
            brainstorm_control.AutoArrange(true);

            PDFTagNodeContentControl content_control = node_control.NodeContentControl as PDFTagNodeContentControl;
            content_control.ExpandDocuments();
        }

        internal void ExploreAutoTagInBrainstorm(string library_id, string tag)
        {
            BrainstormControl brainstorm_control = Instance.OpenNewBrainstorm();
            PDFAutoTagNodeContent content = new PDFAutoTagNodeContent(library_id, tag);
            NodeControl node_control = brainstorm_control.SceneRenderingControl.AddNewNodeControlInScreenCentre(content);
            brainstorm_control.AutoArrange(true);

            PDFAutoTagNodeContentControl content_control = node_control.NodeContentControl as PDFAutoTagNodeContentControl;
            content_control.ExpandDocuments();
        }


        internal void ExploreTopicInBrainstorm(Library library, int topic)
        {
            ExpeditionDataSource eds = library.ExpeditionManager.ExpeditionDataSource;
            string topic_name = eds.GetDescriptionForTopic(topic, include_topic_number: false, "\n");

            BrainstormControl brainstorm_control = Instance.OpenNewBrainstorm();
            ThemeNodeContent tnc = new ThemeNodeContent(topic_name, library.WebLibraryDetail.Id);
            NodeControl node_control = brainstorm_control.SceneRenderingControl.AddNewNodeControlInScreenCentre(tnc);
            brainstorm_control.AutoArrange(true);

            // Then expand the interesting documents - old style
            //ThemeNodeContentControl node_content_control = node_control.NodeContentControl as ThemeNodeContentControl;
            //node_content_control.ExpandSpecificDocuments();
            //node_content_control.ExpandInfluentialDocuments();

            // Then expand the interesting documents
            {
                // Thmeme docs
                brainstorm_control.SceneRenderingControl.SelectAll();
                brainstorm_control.SceneRenderingControl.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(brainstorm_control.SceneRenderingControl), 0, Key.D) { RoutedEvent = Keyboard.KeyDownEvent });
                brainstorm_control.SceneRenderingControl.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(brainstorm_control.SceneRenderingControl), 0, Key.S) { RoutedEvent = Keyboard.KeyDownEvent });

                // Authors
                brainstorm_control.SceneRenderingControl.SelectAll();
                brainstorm_control.SceneRenderingControl.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(brainstorm_control.SceneRenderingControl), 0, Key.A) { RoutedEvent = Keyboard.KeyDownEvent });

                // Their docs
                brainstorm_control.SceneRenderingControl.SelectAll();
                brainstorm_control.SceneRenderingControl.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(brainstorm_control.SceneRenderingControl), 0, Key.D) { RoutedEvent = Keyboard.KeyDownEvent });
            }

            brainstorm_control.SceneRenderingControl.SetSelectedNodeControl(node_control, false);
        }


        internal void ExploreLibraryInBrainstorm(Library library)
        {
            BrainstormControl brainstorm_control = Instance.OpenNewBrainstorm();

            int WIDTH = 320;
            int HEIGHT = 240;

            LibraryNodeContent content_library = new LibraryNodeContent(library.WebLibraryDetail.Title, library.WebLibraryDetail.Id);
            NodeControl node_library = brainstorm_control.SceneRenderingControl.AddNewNodeControl(content_library, 0, 0, WIDTH, HEIGHT);

            ExpeditionDataSource eds = library.ExpeditionManager.ExpeditionDataSource;
            if (null != eds)
            {
                for (int topic = 0; topic < eds.lda_sampler.NumTopics; ++topic)
                {
                    string topic_name = eds.GetDescriptionForTopic(topic, include_topic_number: false, "\n");
                    ThemeNodeContent tnc = new ThemeNodeContent(topic_name, library.WebLibraryDetail.Id);
                    NodeControlAddingByKeyboard.AddChildToNodeControl(node_library, tnc);
                }
            }
            else
            {
                {
                    StringNodeContent content_warning = new StringNodeContent("Please run Expedition on your library.");
                    NodeControl node_warning = brainstorm_control.SceneRenderingControl.AddNewNodeControl(content_warning, 0, -2 * HEIGHT);
                    brainstorm_control.SceneRenderingControl.AddNewConnectorControl(node_library, node_warning);
                }
                {
                    StringNodeContent content_warning = new StringNodeContent("Then you will get to\nexplore its themes.");
                    NodeControl node_warning = brainstorm_control.SceneRenderingControl.AddNewNodeControl(content_warning, -WIDTH, +2 * HEIGHT);
                    brainstorm_control.SceneRenderingControl.AddNewConnectorControl(node_library, node_warning);
                }
                {
                    StringNodeContent content_warning = new StringNodeContent("And you will get to\nexplore its documents.");
                    NodeControl node_warning = brainstorm_control.SceneRenderingControl.AddNewNodeControl(content_warning, +WIDTH, +2 * HEIGHT);
                    brainstorm_control.SceneRenderingControl.AddNewConnectorControl(node_library, node_warning);
                }
            }

            brainstorm_control.AutoArrange(true);
        }

        internal void OpenLocalisationEditing()
        {
            LocalisationEditingControl lec = new LocalisationEditingControl();
            main_window.DockingManager.AddContent("Localisation" + Guid.NewGuid(), "Localisation", Icons.GetAppIcon(Icons.ModuleLocalisation), true, true, lec);
        }

        internal void OpenCSLEditor()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_CSLEditorOpen);
            CSLEditorControl lec = new CSLEditorControl();
            main_window.DockingManager.AddContent("CSLEditor" + Guid.NewGuid(), "CSL Editor", Icons.GetAppIcon(Icons.InCiteEditCSL), true, true, lec);
        }

        internal void OpenCSLWebEditor()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_CSLEditorWebOpen);
            Instance.OpenUrlInBrowser(WebsiteAccess.Url_CSLAbout);
        }

        internal void OpenControl(string type, string title, UserControl control)
        {
            OpenControl(type, title, control, null);
        }

        internal void OpenControl(string type, string title, UserControl control, BitmapImage icon)
        {
            FeatureTrackingManager.Instance.UseFeature(
                Features.Framework_OpenGenericControl,
                "Type", type
            );

            if (null == icon)
            {
                icon = Icons.GetAppIcon(Icons.Qiqqa);
            }

            main_window.DockingManager.AddContent(type + Guid.NewGuid(), title, icon, true, true, control);
        }

        internal void ClosePDFReadingControl(PDFReadingControl pdf_reading_control)
        {
            main_window.DockingManager.CloseContent(pdf_reading_control);
        }

        internal void OpenDocumentConvert()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Tool_DocumentConvertWidget);

            DocumentConversionControl dcc = new DocumentConversionControl();
            string reference_key = "DocConvert" + Guid.NewGuid();
            main_window.DockingManager.AddContent(reference_key, "Qiqqa DOC->PDF", Icons.GetAppIcon(Icons.DocumentTypePdf), true, true, dcc);
            main_window.DockingManager.WantsFloating(reference_key);
        }

        internal void OpenCrossLibrarySearch(string query)
        {
            CrossLibrarySearchControl control = new CrossLibrarySearchControl();
            control.DoSearch(query);
            main_window.DockingManager.AddContent("CrossLibrarySearch" + Guid.NewGuid(), "Cross Library Search", Icons.GetAppIcon(Icons.Search), true, true, control);
        }


        internal SpeedReadControl OpenSpeedRead()
        {
            SpeedReadControl src = new SpeedReadControl();

            StandardWindow window = new StandardWindow();
            window.Title = "Qiqqa SpeedRead";
            window.Content = src;
            window.Width = 700;
            window.Height = 340;

            window.Show();

            return src;

        }

        internal void OpenStartPage()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            main_window.DockingManager.MakeActive(MainWindow.TITLE_START_PAGE);
        }


        internal void OpenWelcomeWizard()
        {
            PointOfInterestLocator poi_locator = new PointOfInterestLocator();
            Route route = AnnotationReportWizard.GetRoute();
            Player player = new Player(poi_locator, route);
        }

        internal void OpenSearchWizard()
        {
            PointOfInterestLocator poi_locator = new PointOfInterestLocator();
            Route route = SearchResultWizard.GetRoute();
            Player player = new Player(poi_locator, route);
        }

        internal void GoExpertMode()
        {
            if (MessageBoxes.AskQuestion(
                ""
                + "Qiqqa ships out of the box in 'Novice Mode' so that newcomers are not overwhelmed at once by the loads of features Qiqqa has to offer.  If you are comfortable with Qiqqa, you can switch to Expert Mode now.  You can always switch back at any time by going to the configuration screen.\n\n"
                + "Do you want to switch to Expert Mode now?"
                ))
            {
                ConfigurationManager.Instance.ConfigurationRecord.GUI_IsNovice = false;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.GUI_IsNovice));

                MessageBoxes.Info("Woohoo!  Qiqqa is now in Expert Mode.  You may need to reopen some of your screens to have access to the advanced features.");
            }
        }
    }
}
