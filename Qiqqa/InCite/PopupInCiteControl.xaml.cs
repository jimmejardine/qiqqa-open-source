using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using Utilities.Language.TextIndexing;

namespace Qiqqa.InCite
{
    /// <summary>
    /// Interaction logic for PopupInCiteControl.xaml
    /// </summary>
    public partial class PopupInCiteControl : UserControl
    {
        private static PopupInCiteControl instance = null;
        private static StandardWindow chw = null;

        WebLibraryDetail web_library_detail = null;

        public PopupInCiteControl()
        {
            InitializeComponent();

            ObjIcon.Icon = Icons.GetAppIcon(Icons.ModuleInCite);
            ObjIcon.IconWidth = 32;
            ObjIcon.IconHeight = 32;
            ObjIcon.Click += ObjIcon_Click;
            ObjIcon.ToolTip = "Click here to open InCite inside Qiqqa.";
            ObjIcon.Cursor = Cursors.Hand;

            ButtonRefresh.Icon = Icons.GetAppIcon(Icons.InCiteRefresh);
            ButtonRefresh.Caption = "Refresh\nBibliography <F5>";
            ButtonRefresh.CaptionDock = Dock.Right;
            ButtonRefresh.Click += ButtonRefresh_Click;
            
            ButtonCiteTogether.Icon = Icons.GetAppIcon(Icons.InCiteAppendCitation);
            ButtonCiteTogether.Caption = "(Author, Date)\n<ENTER>";
            ButtonCiteTogether.CaptionDock = Dock.Right;
            ButtonCiteTogether.Click += ButtonCiteTogether_Click;
            
            ButtonCiteApart.Icon = Icons.GetAppIcon(Icons.InCiteAppendCitation);
            ButtonCiteApart.Caption = "Author (Date)\nCtrl+<ENTER>";
            ButtonCiteApart.CaptionDock = Dock.Right;
            ButtonCiteApart.Click += ButtonCiteApart_Click;

            ButtonSnippet.Icon = Icons.GetAppIcon(Icons.InCiteCitationSnippet);
            ButtonSnippet.Caption = "Copy Snippet\nCtrl+S";
            ButtonSnippet.CaptionDock = Dock.Right;
            ButtonSnippet.Click += ButtonSnippet_Click;


            ButtonCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            ButtonCancel.Caption = "Close\n<ESC>";
            ButtonCancel.CaptionDock = Dock.Right;
            ButtonCancel.Click += ButtonCancel_Click;
            
            this.PreviewKeyDown += PopupInCiteControl_PreviewKeyDown;

            TxtSearchTerms.TextChanged += TxtSearchTerms_TextChanged;
            TxtSearchTerms.ToolTip = "Enter your search query here...";

            TextLibraryForCitations.IsReadOnly = true;
            TextLibraryForCitations.PreviewMouseDown += TextLibraryForCitations_PreviewMouseDown;
            TextLibraryForCitations.ToolTip = "Click to choose a library to search for citations.";
            TextLibraryForCitations.Cursor = Cursors.Hand;

            MatchPreviousWebLibraryDetail();
        }


        void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            PDFDocumentCitingTools.RefreshBibliography();
            chw.Close();
        }

        void ButtonSnippet_Click(object sender, RoutedEventArgs e)
        {
            CiteDocument(false, true);
        }

        void ButtonCiteTogether_Click(object sender, RoutedEventArgs e)
        {
            CiteDocument(false, false);
        }

        void ButtonCiteApart_Click(object sender, RoutedEventArgs e)
        {
            CiteDocument(true, false);
        }

        void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            instance.ObjCheckKeepOpen.IsChecked = false;
            chw.Close();
        }

        void ObjIcon_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenInCite();
        }

        void PopupInCiteControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (false) { }
            else if (Key.Enter == e.Key)
            {
                if (KeyboardTools.IsCTRLDown()) 
                {
                    ButtonCiteApart_Click(null, null);
                }
                else
                {
                    ButtonCiteTogether_Click(null, null);
                }

                e.Handled = true;
            }
            else if (Key.S == e.Key && KeyboardTools.IsCTRLDown())
            {
                ButtonSnippet_Click(null, null);

                e.Handled = true;
            }
            else if (Key.F5 == e.Key)
            {
                ButtonRefresh_Click(null, null);

                e.Handled = true;
            }
            else if (Key.Escape == e.Key)
            {
                instance.ObjCheckKeepOpen.IsChecked = false;
                chw.Close();
                e.Handled = true;
            }
            else if (Key.Up == e.Key)
            {
                try
                {
                    --ObjPDFDocuments.SelectedIndex;
                    ObjPDFDocuments.ScrollIntoView(ObjPDFDocuments.SelectedItem);
                    e.Handled = true;
                }
                catch (Exception) { }
            }
            else if (Key.Down == e.Key)
            {
                try
                {
                    ++ObjPDFDocuments.SelectedIndex;
                    ObjPDFDocuments.ScrollIntoView(ObjPDFDocuments.SelectedItem);
                    e.Handled = true;
                }
                catch (Exception) { }
            }
        }

        private void CiteDocument(bool separate_author_and_date, bool as_snippet)
        {
            TextBlock text_block = ObjPDFDocuments.SelectedItem as TextBlock;
            if (null != text_block)
            {
                ListFormattingTools.DocumentTextBlockTag tag = (ListFormattingTools.DocumentTextBlockTag)text_block.Tag;

                if (as_snippet)
                {
                    PDFDocumentCitingTools.CiteSnippetPDFDocument(false, tag.pdf_document);
                }
                else
                {
                    PDFDocumentCitingTools.CitePDFDocument(tag.pdf_document, separate_author_and_date);
                }
                chw.Close();
            }
            else
            {
                MessageBoxes.Info("Please select a citation to cite.");
            }
        }

        void TextLibraryForCitations_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_Popup_ChooseLibrary);

            // Pick a new library...
            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != web_library_detail)
            {
                ConfigurationManager.Instance.ConfigurationRecord.InCite_LastLibrary = web_library_detail.Title;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.InCite_LastLibrary);

                ChooseNewLibrary(web_library_detail);
            }

            e.Handled = true;
        }

        private void MatchPreviousWebLibraryDetail()
        {
            ChooseNewLibrary(null);

            // Attempt to match the last known library
            string last_library_name = ConfigurationManager.Instance.ConfigurationRecord.InCite_LastLibrary;
            foreach (WebLibraryDetail web_library_detail in WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries_All)
            {
                if (last_library_name == web_library_detail.Title)
                {
                    ChooseNewLibrary(web_library_detail);
                }
            }
        }

        private void ChooseNewLibrary(WebLibraryDetail web_library_detail)
        {
            this.web_library_detail = null;
            TextLibraryForCitations.Text = "Click to choose a library.";

            if (null != web_library_detail)
            {
                this.web_library_detail = web_library_detail;
                TextLibraryForCitations.Text = web_library_detail.Title;
            }

            ReSearch();
        }


        void TxtSearchTerms_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReSearch();
            e.Handled = true;
        }

        private void ReSearch()
        {
            int MAX_DOCUMENTS = 20;

            ObjPDFDocuments.ItemsSource = null;

            if (null != this.web_library_detail)
            {
                string query = TxtSearchTerms.Text;
                if (!String.IsNullOrEmpty(query))
                {
                    List<IndexResult> matches = this.web_library_detail.library.LibraryIndex.GetFingerprintsForQuery(query);
                    List<TextBlock> text_blocks = new List<TextBlock>();
                    bool alternator = false;
                    for (int i = 0; i < MAX_DOCUMENTS && i < matches.Count; ++i)
                    {
                        PDFDocument pdf_document = this.web_library_detail.library.GetDocumentByFingerprint(matches[i].fingerprint);
                        if (pdf_document.Deleted) continue;

                        string prefix = String.Format("{0:0%} - ", matches[i].score);
                        TextBlock text_block = ListFormattingTools.GetDocumentTextBlock(pdf_document, ref alternator, null, VOID_MouseButtonEventHandler, prefix, null);
                        text_blocks.Add(text_block);
                    }

                    ObjPDFDocuments.ItemsSource = text_blocks;
                    if (0 < text_blocks.Count)
                    {
                        ObjPDFDocuments.SelectedIndex = 0;
                    }
                }
            }
        }

        void VOID_MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public static void Popup()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_OpenPopup);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Popup_DISPATCHER();
            }
            ));
        }

        private static void Popup_DISPATCHER()
        {
            if (null == instance)
            {
                instance = new PopupInCiteControl();

                chw = new StandardWindow();
                chw.Title = "Qiqqa InCite";
                chw.Content = instance;
                chw.Closing += chw_Closing;
                chw.Width = 700;
                chw.Height = 340;
                chw.Topmost = true;
                chw.WindowStyle = WindowStyle.ToolWindow;
                chw.ShowInTaskbar = true;
                //chw.AllowsTransparency = true;
                //chw.Background = Brushes.Transparent;
            }

            // Show the window
            chw.Show();
            Keyboard.Focus(instance.TxtSearchTerms);
            instance.TxtSearchTerms.SelectAll();

            // Attempt to get focus
            {
                IntPtr window = Interop.GetWindowHandle(chw);
                Interop.SetForegroundWindow(window);
            }
        }


        static void chw_Closing(object sender, CancelEventArgs e)
        {
            if (instance.ObjCheckKeepOpen.IsChecked ?? false)
            {
            }
            else
            {
                chw.Hide();
            }



            
            e.Cancel = true;
        }

        public class Interop
        {
            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            public static IntPtr GetWindowHandle(Window window)
            {
                return new WindowInteropHelper(window).Handle;
            }
        }
    }
}
