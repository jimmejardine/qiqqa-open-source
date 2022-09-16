using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using icons;
using Microsoft.Win32;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.Common.WebcastStuff;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.DocumentLibraryIndex;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Localisation;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Language.TextIndexing;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace Qiqqa.InCite
{
    /// <summary>
    /// Interaction logic for InCiteControl.xaml
    /// </summary>
    public partial class InCiteControl : UserControl
    {
        private WebLibraryDetail default_library = null;
        private LibraryControl library_control = null;

        public InCiteControl()
        {
            Theme.Initialize();

            InitializeComponent();

            // Tabs

            DualTabControlArea.Children.Clear();
            DualTabControlArea.AddContent("Library", "Cite papers from your library", null, false, false, TabLibrary);
            DualTabControlArea.AddContent("Recommendations", "Cite papers recommended by Qiqqa InCite", null, false, false, TabRecommendations);
            DualTabControlArea.AddContent("Cluster", "Edit current citation cluster", null, false, false, TabCitationClusterEditor);
            DualTabControlArea.MakeActive("Library");


            bool ADVANCED_MENUS = ConfigurationManager.Instance.ConfigurationRecord.GUI_AdvancedMenus;


            ButtonCitationSnippetToClipboard.Icon = Icons.GetAppIcon(Icons.InCiteCitationSnippet);
            if (!ADVANCED_MENUS) ButtonCitationSnippetToClipboard.Caption = LocalisationManager.Get("INCITE/CAP/NEW_SNIPPET");
            ButtonCitationSnippetToClipboard.ToolTip = LocalisationManager.Get("INCITE/TIP/NEW_SNIPPET");
            ButtonCitationSnippetToClipboard.Click += ButtonCitationSnippetToClipboard_Click;

            ButtonNewCitation.Icon = Icons.GetAppIcon(Icons.InCiteNewCitation);
            if (!ADVANCED_MENUS) ButtonNewCitation.Caption = LocalisationManager.Get("INCITE/CAP/NEW_CITATION");
            ButtonNewCitation.ToolTip = LocalisationManager.Get("INCITE/TIP/NEW_CITATION");
            ButtonNewCitation.Click += ButtonNewCitation_Click;

            ButtonNewCitationSeparateAuthorYear.Icon = Icons.GetAppIcon(Icons.InCiteNewCitation);
            if (!ADVANCED_MENUS) ButtonNewCitationSeparateAuthorYear.Caption = LocalisationManager.Get("INCITE/CAP/NEW_CITATION_SEPARATE_AUTHOR_YEAR");
            ButtonNewCitationSeparateAuthorYear.ToolTip = LocalisationManager.Get("INCITE/TIP/NEW_CITATION_SEPARATE_AUTHOR_YEAR");
            ButtonNewCitationSeparateAuthorYear.Click += ButtonNewCitationSeparateAuthorYear_Click;

            ButtonAddBibliography.Icon = Icons.GetAppIcon(Icons.InCiteAddBibliography);
            if (!ADVANCED_MENUS) ButtonAddBibliography.Caption = LocalisationManager.Get("INCITE/CAP/NEW_BIBLIOGRAPHY");
            ButtonAddBibliography.ToolTip = LocalisationManager.Get("INCITE/TIP/NEW_BIBLIOGRAPHY");
            ButtonAddBibliography.Click += ButtonAddBibliography_Click;

            ButtonRefresh.Icon = Icons.GetAppIcon(Icons.InCiteRefresh);
            if (!ADVANCED_MENUS) ButtonRefresh.Caption = LocalisationManager.Get("INCITE/CAP/REFORMAT");
            ButtonRefresh.ToolTip = LocalisationManager.Get("INCITE/TIP/REFORMAT");
            ButtonRefresh.Click += ButtonRefresh_Click;


            ButtonEditCSL.AttachPopup(ButtonEditCSLPopup);
            ButtonEditCSL.Icon = Icons.GetAppIcon(Icons.InCiteEditCSL);
            if (!ADVANCED_MENUS) ButtonEditCSL.Caption = LocalisationManager.Get("INCITE/CAP/CSL_OPTIONS");

            ButtonCSLStandard.Icon = Icons.GetAppIcon(Icons.InCiteCSLStandard);
            ButtonCSLStandard.Caption = LocalisationManager.Get("INCITE/CAP/CSL_STANDARD");
            ButtonCSLStandard.ToolTip = LocalisationManager.Get("INCITE/TIP/CSL_STANDARD");
            ButtonCSLStandard.Click += ButtonCSLStandard_Click;

            ButtonCSLDownload.Icon = Icons.GetAppIcon(Icons.InCiteCSLDownload);
            ButtonCSLDownload.Caption = LocalisationManager.Get("INCITE/CAP/CSL_BROWSE");
            ButtonCSLDownload.ToolTip = LocalisationManager.Get("INCITE/TIP/CSL_BROWSE");
            ButtonCSLDownload.Click += ButtonCSLDownload_Click;

            ButtonEditCSL_Web.Click += ButtonEditCSL_Web_Click;
            ButtonEditCSL_Web.Icon = Icons.GetAppIcon(Icons.InCiteEditCSL);
            ButtonEditCSL_Web.Caption = "Open Web CSL Editor (Beginner)";
            ButtonEditCSL_Web.ToolTip = LocalisationManager.Get("INCITE/TIP/CSL_EDIT");

            ButtonEditCSL_Internal.Click += ButtonEditCSL_Internal_Click;
            ButtonEditCSL_Internal.Icon = Icons.GetAppIcon(Icons.InCiteEditCSL);
            ButtonEditCSL_Internal.Caption = "Open Qiqqa CSL Editor (Advanced)";
            ButtonEditCSL_Internal.ToolTip = LocalisationManager.Get("INCITE/TIP/CSL_EDIT");


            ButtonTools.AttachPopup(ButtonToolsPopup);
            ButtonTools.Icon = Icons.GetAppIcon(Icons.ModuleConfiguration);
            if (!ADVANCED_MENUS) ButtonTools.Caption = LocalisationManager.Get("INCITE/CAP/TOOLS");

            ButtonFindUsedReferences.Icon = Icons.GetAppIcon(Icons.InCiteFindUsedReferences);
            ButtonFindUsedReferences.Caption = LocalisationManager.Get("INCITE/CAP/CSL_FIND");
            ButtonFindUsedReferences.ToolTip = LocalisationManager.Get("INCITE/TIP/CSL_FIND");
            ButtonFindUsedReferences.Click += ButtonFindUsedReferences_Click;

            ButtonAddCSLStats.Icon = Icons.GetAppIcon(Icons.InCiteAddCSLStats);
            ButtonAddCSLStats.Caption = LocalisationManager.Get("INCITE/CAP/ADD_CSL_STATS");
            ButtonAddCSLStats.ToolTip = LocalisationManager.Get("INCITE/TIP/ADD_CSL_STATS");
            ButtonAddCSLStats.Click += ButtonAddCSLStats_Click;

            ButtonUseAbbreviations.Icon = Icons.GetAppIcon(Icons.InCiteAbbreviations);
            ButtonUseAbbreviations.Caption = LocalisationManager.Get("INCITE/CAP/ABBREVIATIONS");
            ButtonUseAbbreviations.ToolTip = LocalisationManager.Get("INCITE/TIP/ABBREVIATIONS");
            ButtonUseAbbreviations.DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            ButtonCustomAbbreviationsFilename.Icon = Icons.GetAppIcon(Icons.InCiteAbbreviations);
            ButtonCustomAbbreviationsFilename.Caption = LocalisationManager.Get("INCITE/CAP/CUSTOM_ABBREVIATIONS_FILENAME");
            ButtonCustomAbbreviationsFilename.ToolTip = LocalisationManager.Get("INCITE/TIP/CUSTOM_ABBREVIATIONS_FILENAME");
            ButtonCustomAbbreviationsFilename.Click += ButtonCustomAbbreviationsFilename_Click;

            ButtonToggleWatcher.Icon = Icons.GetAppIcon(Icons.InCitePause);
            ButtonToggleWatcher.Caption = LocalisationManager.Get("INCITE/CAP/PAUSE");
            ButtonToggleWatcher.ToolTip = LocalisationManager.Get("INCITE/TIP/PAUSE");
            ButtonToggleWatcher.Click += ButtonToggleWatcher_Click;

            ButtonLaunchWord.Icon = Icons.GetAppIcon(Icons.ExportWord2007);
            ButtonLaunchWord.Caption = LocalisationManager.Get("INCITE/CAP/LAUNCHWORD");
            ButtonLaunchWord.ToolTip = LocalisationManager.Get("INCITE/TIP/LAUNCHWORD");
            ButtonLaunchWord.Click += ButtonLaunchWord_Click;

            ButtonInCitePopup.Icon = Icons.GetAppIcon(Icons.InCiteToolbarOpenPopup);
            ButtonInCitePopup.Caption = LocalisationManager.Get("INCITE/CAP/INCITE_POPUP");
            ButtonInCitePopup.ToolTip = LocalisationManager.Get("INCITE/TIP/INCITE_POPUP");
            ButtonInCitePopup.Click += ButtonInCitePopup_Click;


            ObjCitationClusterEditorControl.CitationClusterChanged += ObjCitationClusterEditorControl_CitationClusterChanged;
            ObjCitationClusterEditorControl.CitationClusterOpenPDFByReferenceKey += ObjCitationClusterEditorControl_CitationClusterOpenPDFByReferenceKey;

            Webcasts.FormatWebcastButton(ButtonWebcast, Webcasts.INCITE);
            if (!ADVANCED_MENUS) ButtonWebcast.Caption = "Tutorial\n";

            ButtonConnection.Icon = Icons.GetAppIcon(Icons.InCiteConnection);
            ButtonConnection.CaptionDock = Dock.Right;

            LblTextStyleFilename.Visibility = ADVANCED_MENUS ? Visibility.Collapsed : Visibility.Visible;
            TextStyleFilename.FontSize = ThemeColours.HEADER_FONT_SIZE;
            TextStyleFilename.FontFamily = ThemeTextStyles.FontFamily_Header;
            TextStyleFilename.TextTrimming = TextTrimming.CharacterEllipsis;
            TextStyleFilename.DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;
            TextStyleFilename.PreviewMouseDown += TextStyleFilename_PreviewMouseDown;
            TextStyleFilename.Cursor = Cursors.Hand;
            TextStyleFilename.ToolTip = "Click to choose a citation formatting style file saved somewhere on your computer.";

            LblTextLibraryForCitations.Visibility = ADVANCED_MENUS ? Visibility.Collapsed : Visibility.Visible;
            TextLibraryForCitations.FontSize = ThemeColours.HEADER_FONT_SIZE;
            TextLibraryForCitations.FontFamily = ThemeTextStyles.FontFamily_Header;
            TextLibraryForCitations.TextWrapping = TextWrapping.Wrap;
            TextLibraryForCitations.PreviewMouseDown += TextLibraryForCitations_PreviewMouseDown;
            TextLibraryForCitations.ToolTip = "Click to choose a library to search for citations.";
            TextLibraryForCitations.Cursor = Cursors.Hand;

            MatchPreviousWebLibraryDetail();

            ObjHyperlinkFixWordConnection.Click += ObjHyperlinkFixWordConnection_Click;

            WordConnector.Instance.ContextChanged += word_connector_ContextChanged;
            WordConnector.Instance.CitationClusterChanged += word_connector_CitationClusterChanged;

            // Initialise the buttons
            WordConnector.Instance.ReissueContextChanged();
        }

        private void ButtonCustomAbbreviationsFilename_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (Runtime.IsRunningInVisualStudioDesigner) return;
#endif

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files|*.txt" + "|" + "All files|*.*";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Title = "Please select your Custom Abbreviations file or press CANCEL to clear the last selected file.  It should have one abbreviation per line of the form: LONG_NAME<tab>ABBREVIATION.";
            if (true == dialog.ShowDialog())
            {
                ConfigurationManager.Instance.ConfigurationRecord.InCite_CustomAbbreviationsFilename = dialog.FileName;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.InCite_CustomAbbreviationsFilename));
            }
            else
            {
                ConfigurationManager.Instance.ConfigurationRecord.InCite_CustomAbbreviationsFilename = "";
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.InCite_CustomAbbreviationsFilename));
            }
        }

        private void ButtonLaunchWord_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (Runtime.IsRunningInVisualStudioDesigner) return;
#endif

            // Check if the user wants to override their version of Word.
            if (KeyboardTools.IsCTRLDown())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Microsoft Word|winword.exe" + "|" + "Executable files|*.exe" + "|" + "All files|*.*";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                if (true == dialog.ShowDialog())
                {
                    ConfigurationManager.Instance.ConfigurationRecord.InCite_WinWordLocation = dialog.FileName;
                    ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.InCite_WinWordLocation));
                }
                else
                {
                    ConfigurationManager.Instance.ConfigurationRecord.InCite_WinWordLocation = "";
                    ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.InCite_WinWordLocation));
                }
            }

            string app_to_launch = ConfigurationManager.Instance.ConfigurationRecord.InCite_WinWordLocation;
            if (String.IsNullOrEmpty(app_to_launch))
            {
                app_to_launch = "winword.exe";
            }

            try
            {
                Process.Start(app_to_launch);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem starting {0}", app_to_launch);
                MessageBoxes.Error("There has been a problem starting Word from the location '{0}'.  This is probably because Word is not in your PATH.  Please try again while holding down the CTRL button so that you can select Word manually.  You are looking for winword.exe.", app_to_launch);
            }
        }

        private void ObjHyperlinkFixWordConnection_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(PDFDocumentCitingTools.BASE_REGISTRY_FIXES_DIRECTORY);
            e.Handled = true;
        }

        private bool already_told_about_popup = false;

        private void ButtonInCitePopup_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_OpenPopupFromToolbar);

            if (!already_told_about_popup)
            {
                already_told_about_popup = true;
                MessageBoxes.Info("You can get to InCite Popup much faster by holding down the 'Windows key' and pressing 'Z'.\n\nYou definitely want to do this if you are in Microsoft Word and want to quickly add a citation and keep typing.");
            }

            PopupInCiteControl.Popup();
        }

        private void ButtonFindUsedReferences_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_OpenFindUsedReferences);
            UsedCitationsControl ucc = new UsedCitationsControl();
            MainWindowServiceDispatcher.Instance.OpenControl("UsedCitations", "Used Citations", ucc, Icons.GetAppIcon(Icons.InCiteFindUsedReferences));
            ucc.Refresh(DefaultLibrary);
        }

        private void ObjCitationClusterEditorControl_CitationClusterOpenPDFByReferenceKey(string reference_key)
        {
            CSLProcessorBibTeXFinder.MatchingBibTeXRecord matching_bibtex_record = CSLProcessorBibTeXFinder.LocateBibTexForCitationItem(reference_key, DefaultLibrary);
            if (null != matching_bibtex_record)
            {
                MainWindowServiceDispatcher.Instance.OpenDocument(matching_bibtex_record.pdf_document);
            }
        }

        private void ButtonEditCSL_Internal_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenCSLEditor();
        }

        private void ButtonEditCSL_Web_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenCSLWebEditor();
        }

        private bool CheckThatLibraryAndStyleHaveBeenSelected()
        {
            string style_filename = ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile;
            if (String.IsNullOrEmpty(style_filename))
            {
                MessageBoxes.Info("You need to please choose a bibliography style from the toolbar.");
                return false;
            }

            if (null == library_control)
            {
                MessageBoxes.Info("You need to please choose a library from the toolbar.  This library will be used to generate your citations.");
                return false;
            }

            return true;
        }

        private void ButtonCitationSnippetToClipboard_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitationSnippet);

            // Check that they have picked the two important things...
            if (!CheckThatLibraryAndStyleHaveBeenSelected())
            {
                return;
            }

            // Get the selected citations
            List<PDFDocument> selected_pdf_documents = library_control.ObjLibraryCatalogControl.SelectedPDFDocuments;
            if (0 == selected_pdf_documents.Count)
            {
                MessageBoxes.Warn("You have not selected any document(s) with which to create a citation snippet.");
                return;
            }

            string style_filename = GetStyleFilename();
            if (null != style_filename)
            {
                CSLProcessor.GenerateRtfCitationSnippet(false, selected_pdf_documents, style_filename, null);
            }

            e.Handled = true;
        }

        private void MatchPreviousWebLibraryDetail()
        {
            bool found_matching_library = false;

            // Attempt to match the last known library
            string last_library_name = ConfigurationManager.Instance.ConfigurationRecord.InCite_LastLibrary;
            foreach (WebLibraryDetail web_library_detail in WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries)
            {
                if (last_library_name == web_library_detail.Title)
                {
                    ChooseNewLibrary(web_library_detail);
                    found_matching_library = true;
                    break;
                }
            }

            // If we have not found a matching library, choose their most recent lib (which will be a fallback on guest if that's the only or top one)
            if (!found_matching_library)
            {
                List<WebLibraryDetail> web_libary_details = WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries;
                WebLibraryManager.Instance.SortWebLibraryDetailsByLastAccessed(web_libary_details);

                ASSERT.Test(web_libary_details.Count > 0);

                ChooseNewLibrary(web_libary_details[0]);
            }
        }

        private void TextLibraryForCitations_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_ChooseLibrary);

            // Pick a new library...
            WebLibraryDetail picked_web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != picked_web_library_detail)
            {
                ConfigurationManager.Instance.ConfigurationRecord.InCite_LastLibrary = picked_web_library_detail.Title;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.InCite_LastLibrary));

                ChooseNewLibrary(picked_web_library_detail);
            }

            e.Handled = true;
        }

        private void ChooseNewLibrary(WebLibraryDetail web_library_detail)
        {
            default_library = web_library_detail;
            library_control = null;
            TextLibraryForCitations.Text = "Click to choose a library.";
            ObjLibraryControlPlaceholderRegion.Children.Clear();

            if (null != web_library_detail)
            {
                TextLibraryForCitations.Text = web_library_detail.Title;

                library_control = new LibraryControl(web_library_detail);
                library_control.ObjToolBarTray.Visibility = Visibility.Collapsed;

                HolderForSearchBox.Children.Clear();
                HolderForSearchBox.Children.Add(library_control.DetachSearchBox());

                ObjLibraryControlPlaceholderRegion.Children.Add(library_control);
            }
        }

        private enum CSLFileSource
        {
            TEXTBOX,
            STANDARD
        }

        private void CorrectStyleFilenameForNewDirectoryLocation()
        {
            // Get the style filename that is in the text box
            string style_filename = ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile;

            string new_style_filename = PDFDocumentCitingTools.FindValidStyleFilename(style_filename);
            if (0 != String.Compare(new_style_filename, style_filename) && null != new_style_filename)
            {
                Logging.Info("Updating style filename from {0} to {1}", style_filename, new_style_filename);
                ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile = new_style_filename;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile));
            }
        }

        private void ChooseCSLFile(CSLFileSource csl_file_source)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSL style files|*.csl" + "|" + "All files|*.*";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;

            string filename = null;
            string directory = null;

            // Decide what we want to use as the CSL file location
            if (CSLFileSource.TEXTBOX == csl_file_source)
            {
                try
                {
                    CorrectStyleFilenameForNewDirectoryLocation();
                    directory = Path.GetDirectoryName(ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile);
                    filename = Path.GetFileName(ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile);

                    // If the directory no longer exists, kill our memory of it.  We will pick the default directory again.
                    if (!Directory.Exists(directory))
                    {
                        directory = null;
                    }
                }
                catch { }
            }
            if (null == directory)
            {
                directory = PDFDocumentCitingTools.BASE_STYLE_DIRECTORY;
            }

            // Set the dialog defaults
            dialog.FileName = filename;
            dialog.InitialDirectory = directory;

            // Get the user response
            if (true == dialog.ShowDialog())
            {
                ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile = dialog.FileName;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile));

                // Check if it is a dependent style
                string style_xml_filename = ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile;
                string root_style_filename = DependentStyleDetector.GetRootStyleFilename(style_xml_filename);
                if (root_style_filename != style_xml_filename)
                {
                    if (null != root_style_filename)
                    {
                        MessageBoxes.Info("This style is dependent on another style, so InCite will be using this file instead:\n" + root_style_filename);
                    }
                    else
                    {
                        MessageBoxes.Info("This style is dependent on another style that is not available on your computer.  Please download it before proceeding.");
                    }
                }
            }
        }

        private void TextStyleFilename_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
#if DEBUG
            if (Runtime.IsRunningInVisualStudioDesigner) return;
#endif

            FeatureTrackingManager.Instance.UseFeature(Features.InCite_ChooseOwnCSL);

            ChooseCSLFile(CSLFileSource.TEXTBOX);
            e.Handled = true;
        }

        private void ButtonCSLStandard_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (Runtime.IsRunningInVisualStudioDesigner) return;
#endif

            FeatureTrackingManager.Instance.UseFeature(Features.InCite_ChooseStandardCSL);

            ChooseCSLFile(CSLFileSource.STANDARD);
            e.Handled = true;
        }

        private void ButtonCSLDownload_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_BrowseZoteroCSL);
            MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_ZoteroCSLRepository, true);
        }

        private void word_connector_CitationClusterChanged(CitationCluster context_citation_cluster)
        {
            Logging.Debug特("InCite: CitationClusterChanged: {0}", context_citation_cluster);

            WPFDoEvents.InvokeAsyncInUIThread(() =>
            {
                ObjCitationClusterEditorControl.SetCitationCluster(context_citation_cluster);
            }
            );
        }

        private void ButtonToggleWatcher_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_ToggleInCite);

            WordConnector.Instance.SetPaused(ButtonToggleWatcher.IsChecked ?? false);
        }

        private object context_thread_lock = new object();
        private bool context_thread_running = false;
        private string context_thread_next_context = null;

        private void word_connector_ContextChanged(string context_word, string context_backward, string context_surround)
        {
            Logging.Debug特("InCite: ContextChanged");

            WPFDoEvents.InvokeAsyncInUIThread(() =>
            {
                word_connector_ContextChanged_BACKGROUND_UpdateButtonEnabledness(context_word, context_backward, context_surround);
            }, DispatcherPriority.Background);

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (context_thread_lock)
            {
                // l1_clk.LockPerfTimerStop();
                context_thread_next_context = context_backward;
                if (!context_thread_running)
                {
                    context_thread_running = true;
                    SafeThreadPool.QueueUserWorkItem(o => word_connector_ContextChanged_BACKGROUND_FindRecommendations());
                }
            }
        }

        private void word_connector_ContextChanged_BACKGROUND_UpdateButtonEnabledness(string context_word, string context_backward, string context_surround)
        {
            // If there is a null context, lets assume the buttons won't work...
            if (null == context_word)
            {
                ButtonAddBibliography.IsEnabled =
                    ButtonAddCSLStats.IsEnabled =
                    ButtonNewCitation.IsEnabled =
                    ButtonNewCitationSeparateAuthorYear.IsEnabled =
                    ButtonRefresh.IsEnabled =
                    false;

                ObjCantConnectToWord.Visibility = Visibility.Visible;
            }
            else
            {
                ButtonAddBibliography.IsEnabled =
                    ButtonAddCSLStats.IsEnabled =
                    ButtonNewCitation.IsEnabled =
                    ButtonNewCitationSeparateAuthorYear.IsEnabled =
                    ButtonRefresh.IsEnabled =
                    true;

                ObjCantConnectToWord.Visibility = Visibility.Collapsed;
            }
        }

        private void word_connector_ContextChanged_BACKGROUND_FindRecommendations()
        {
            while (true)
            {
                // Get the next context to search for, and if there is none, then exit the background thread
                string context;

                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (context_thread_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    context = context_thread_next_context;
                    context_thread_next_context = null;

                    if (null == context)
                    {
                        context_thread_running = false;
                        break;
                    }
                }

                // Now that we have the context, do the query
                List<PDFDocument> context_pdf_documents = new List<PDFDocument>();
                {
                    WebLibraryDetail web_library_detail = this.default_library;
                    if (null != web_library_detail)
                    {
                        string context_search_string = PolishContextForLucene(context);
                        context_search_string = context_search_string.Trim();
                        if (!String.IsNullOrEmpty(context_search_string))
                        {
                            List<IndexResult> fingerprints = LibrarySearcher.FindAllFingerprintsMatchingQuery(web_library_detail, context_search_string);
                            if (null != fingerprints && 0 != fingerprints.Count)
                            {
                                foreach (var fingerprint in fingerprints)
                                {
                                    if (20 <= context_pdf_documents.Count)
                                    {
                                        break;
                                    }

                                    PDFDocument pdf_document = web_library_detail.Xlibrary.GetDocumentByFingerprint(fingerprint.fingerprint);
                                    if (null != pdf_document)
                                    {
                                        context_pdf_documents.Add(pdf_document);
                                    }
                                }
                            }
                        }
                    }
                }

                // And get the GUI to update with the results
                WPFDoEvents.InvokeAsyncInUIThread(() =>
                {
                    word_connector_ContextChanged_BACKGROUND_PopulateRecommendations(context_pdf_documents);
                }, DispatcherPriority.Background);
            }
        }


        private void word_connector_ContextChanged_BACKGROUND_PopulateRecommendations(List<PDFDocument> context_pdf_documents)
        {
            // Out with the old...
            ObjRecommendedCitationsList.Children.Clear();

            // In with the new
            bool added_recommendation = false;
            bool alternator = false;
            foreach (PDFDocument pdf_document in context_pdf_documents)
            {
                added_recommendation = true;

                TextBlock text_doc = ListFormattingTools.GetDocumentTextBlock(pdf_document, ref alternator, null, RecommendedCitationsMouseDown);
                ObjRecommendedCitationsList.Children.Add(text_doc);
            }

            ObjGridNoRecommendationsInstructions.Visibility = added_recommendation ? Visibility.Collapsed : Visibility.Visible;
        }

        private static string PolishContextForLucene(string context)
        {
            // Kill the magic chars
            context = context.Replace("[", "");
            context = context.Replace("]", "");
            context = context.Replace("(", "");
            context = context.Replace(")", "");
            context = context.Replace("-", "");
            context = context.Replace(":", "");
            context = context.Replace("*", "");
            context = context.Replace("~", "");
            context = context.Replace("^", "");
            context = context.Replace("?", "");

            // Fluff the whitespace
            context = context.Replace("\t", " ");
            context = context.Replace("\r", " ");
            context = context.Replace("\n", " ");

            return context;
        }

        private void RecommendedCitationsMouseDown(object sender, MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_ClickRecommended);

            TextBlock text_block = (TextBlock)sender;
            ListFormattingTools.DocumentTextBlockTag tag = (ListFormattingTools.DocumentTextBlockTag)text_block.Tag;
            library_control.ObjLibraryCatalogControl.FocusOnDocument(tag.pdf_document);
            e.Handled = true;
        }

        private CitationCluster GenerateCitationClusterFromCurrentSelection()
        {
            List<PDFDocument> selected_pdf_documents = library_control.ObjLibraryCatalogControl.SelectedPDFDocuments;

            return PDFDocumentCitingTools.GenerateCitationClusterFromPDFDocuments(selected_pdf_documents);
        }

        private void ButtonNewCitation_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitation);

            // Check that they have picked the two important things...
            if (!CheckThatLibraryAndStyleHaveBeenSelected())
            {
                return;
            }

            CitationCluster citation_cluster = GenerateCitationClusterFromCurrentSelection();
            if (null != citation_cluster && 0 < citation_cluster.citation_items.Count)
            {
                citation_cluster.citation_items[0].SeparateAuthorsAndDate(false);
                WordConnector.Instance.AppendCitation(citation_cluster);
            }
            e.Handled = true;
        }

        private void ButtonNewCitationSeparateAuthorYear_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitation);

            // Check that they have picked the two important things...
            if (!CheckThatLibraryAndStyleHaveBeenSelected())
            {
                return;
            }

            CitationCluster citation_cluster = GenerateCitationClusterFromCurrentSelection();
            if (null != citation_cluster)
            {
                citation_cluster.citation_items[0].SeparateAuthorsAndDate(true);
                WordConnector.Instance.AppendCitation(citation_cluster);
            }
            e.Handled = true;
        }

        private void ObjCitationClusterEditorControl_CitationClusterChanged(CitationCluster citation_cluster)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_EditCitationCluster);

            WordConnector.Instance.ModifyCitation(citation_cluster);
        }

        private void ButtonAddBibliography_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewBibliography);

            WordConnector.Instance.AddBibliography();

            e.Handled = true;
        }

        private void ButtonAddCSLStats_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCSLStats);

            WordConnector.Instance.AddCSLStats();

            e.Handled = true;
        }

        private string GetStyleFilename()
        {
            CorrectStyleFilenameForNewDirectoryLocation();

            // Get the style filename that is in the text box
            string style_filename = ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile;
            if (!File.Exists(style_filename))
            {
                MessageBoxes.Warn("The CSL style file you were using seems to have been moved or deleted.\nPlease select a new CSL style file.\n\nThe current missing CSL style file is:\n  {0}", style_filename);
                return null;
            }


            string root_style_filename = DependentStyleDetector.GetRootStyleFilename(style_filename);
            if (null == root_style_filename)
            {
                MessageBoxes.Info("This style is dependent on another style that is not available on your computer.  Please download it before proceeding.");
                return null;
            }

            if (root_style_filename != style_filename)
            {
                Logging.Info("We are using the root style {0} instead of the selected style {1}", root_style_filename, style_filename);
            }

            return style_filename;
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_Refresh);

            // Check that they have picked the two important things...
            if (!CheckThatLibraryAndStyleHaveBeenSelected())
            {
                return;
            }

            string style_filename = GetStyleFilename();
            if (null != style_filename)
            {
                CSLProcessor.RefreshDocument(WordConnector.Instance, style_filename, DefaultLibrary);
            }

            e.Handled = true;
        }

        public WebLibraryDetail DefaultLibrary
        {
            get
            {
                return default_library;
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            InCiteControl icc = new InCiteControl();
            ControlHostingWindow w = new ControlHostingWindow("InCite", icc);
            w.Width = 1024;
            w.Height= 800;
            w.Show();
        }
#endif

        #endregion
    }
}

