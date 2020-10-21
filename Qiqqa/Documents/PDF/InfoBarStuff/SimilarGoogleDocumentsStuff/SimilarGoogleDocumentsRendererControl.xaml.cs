using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Qiqqa.Common;
using Qiqqa.Common.GUI;
using Utilities;
using Utilities.GUI;
using Utilities.Internet.GoogleScholar;
using Utilities.Internet.HTMLToXAML;

namespace Qiqqa.Documents.PDF.InfoBarStuff.SimilarGoogleDocumentsStuff
{
    /// <summary>
    /// Interaction logic for SimilarGoogleDocumentsRendererControl.xaml
    /// </summary>
    public partial class SimilarGoogleDocumentsRendererControl : UserControl
    {
        public SimilarGoogleDocumentsRendererControl()
        {
            InitializeComponent();
        }

        public void SpecifyPaperSet(GoogleScholarScrapePaperSet gssps)
        {
            DocsPanel.Children.Clear();

            {
                // Add the header
                StackPanel sp = new StackPanel();
                HyperlinkTextBlock tb_gs = new HyperlinkTextBlock();
                tb_gs.HorizontalAlignment = HorizontalAlignment.Center;
                tb_gs.Text = "Go to Google Scholar";
                tb_gs.FontWeight = FontWeights.Bold;
                tb_gs.Tag = gssps.Url;
                tb_gs.OnClick += OpenUrlInTagOfTextBlock;
                sp.Children.Add(tb_gs);
                DocsPanel.Children.Add(sp);
                DocsPanel.Children.Add(new AugmentedSpacer());
            }

            bool alternate = false;
            foreach (var gssp in gssps.Papers)
            {
                StackPanel sp = new StackPanel();

                try
                {
                    string html = gssp.abstract_html;
                    if (html == null) html = "";
                    html = html.Replace("<br>", " ");
                    string xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, true);
                    FlowDocument fd = (FlowDocument)XamlReader.Parse(xaml);
                    RichTextBox rtb = new RichTextBox(fd);
                    rtb.Width = 320;
                    rtb.Height = 200;
                    sp.ToolTip = rtb;
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "Problem parsing GS HTML abstract");
                }

                if (null != gssp.source_url)
                {
                    HyperlinkTextBlock tb_title = new HyperlinkTextBlock();
                    tb_title.Text = gssp.title;
                    tb_title.Tag = gssp.source_url;
                    tb_title.OnClick += OpenUrlInTagOfTextBlock;
                    sp.Children.Add(tb_title);
                }
                else
                {
                    TextBlock tb_title = new TextBlock();
                    tb_title.Text = gssp.title;
                    tb_title.FontWeight = FontWeights.Bold;
                    sp.Children.Add(tb_title);
                }

                TextBlock tb_authors = new TextBlock();
                tb_authors.Text = gssp.authors;
                sp.Children.Add(tb_authors);

                TextBlock tb_citedby = new TextBlock();
                tb_citedby.Text = gssp.cited_by_header;
                sp.Children.Add(tb_citedby);

                if (0 < DocsPanel.Children.Count)
                {
                    DocsPanel.Children.Add(new AugmentedSpacer());
                }

                alternate = !alternate;
                ListFormattingTools.AddGlowingHoverEffect(sp);

                DocsPanel.Children.Add(sp);
            }
        }

        private void OpenUrlInTagOfTextBlock(object sender, MouseButtonEventArgs e)
        {
            HyperlinkTextBlock tb = (HyperlinkTextBlock)sender;
            string url = (string)tb.Tag;
            MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(url);
            e.Handled = true;
        }
    }
}
