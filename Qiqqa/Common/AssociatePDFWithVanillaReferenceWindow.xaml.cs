using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Qiqqa.WebBrowsing.GeckoStuff;

namespace Qiqqa.Common
{
    /// <summary>
    /// Interaction logic for AssociatePDFWithVanillaReferenceWindow.xaml
    /// </summary>
    public partial class AssociatePDFWithVanillaReferenceWindow : StandardWindow
    {
        private PDFDocument pdf_document;

        public AssociatePDFWithVanillaReferenceWindow(PDFDocument pdf_document)
        {
            this.pdf_document = pdf_document;

            InitializeComponent();

            ObjHeader.Img = Icons.GetAppIcon(Icons.LibraryCatalogOpenVanillaReference);
            ObjHeader.Caption = "Vanilla References";
            ObjHeader.SubCaption = "Associate a PDF with your Vanilla Reference";

            CmdLocal.Caption = "Use Local";
            CmdLocal.Icon = Icons.GetAppIcon(Icons.ModuleDocumentLibrary);
            CmdLocal.CaptionDock = Dock.Bottom;
            CmdLocal.Click += CmdLocal_Click;

            CmdWeb.Caption = "Use Web";
            CmdWeb.Icon = Icons.GetAppIcon(Icons.ModuleWebBrowser);
            CmdWeb.CaptionDock = Dock.Bottom;
            CmdWeb.Click += CmdWeb_Click;

            CmdCancel.Caption = "Cancel";
            CmdCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            CmdCancel.CaptionDock = Dock.Bottom;
            CmdCancel.Click += CmdCancel_Click;
        }

        private void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CmdWeb_Click(object sender, RoutedEventArgs e)
        {
            PDFInterceptor.Instance.PotentialAttachmentPDFDocument = pdf_document;
            MainWindowServiceDispatcher.Instance.SearchWeb(pdf_document.TitleCombined);
            Close();
        }

        private void CmdLocal_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "PDF Files|*.pdf",
                Multiselect = false,
                Title = "Select the PDF document you wish to associate with this Vanilla Reference."
            })
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FeatureTrackingManager.Instance.UseFeature(Features.Library_AttachToVanilla_Local);
                    pdf_document.AssociatePDFWithVanillaReference(dlg.FileName);
                }
            }

            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            pdf_document = null;
        }
    }
}
