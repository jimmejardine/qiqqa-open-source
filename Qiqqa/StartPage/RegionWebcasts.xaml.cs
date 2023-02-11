using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.WebcastStuff;
using Utilities.GUI;

namespace Qiqqa.StartPage
{
    /// <summary>
    /// Interaction logic for RegionWhatsNew.xaml
    /// </summary>
    public partial class RegionWebcasts : UserControl
    {
        public RegionWebcasts()
        {
            InitializeComponent();

            Webcasts.FormatWebcaseHyperlink(LnkFirst10Mins, Webcasts.FIRST10MINS);
            Webcasts.FormatWebcaseHyperlink(LnkIdeas, Webcasts.IDEAS);
            Webcasts.FormatWebcaseHyperlink(LnkExplore, Webcasts.EXPLORE);
            Webcasts.FormatWebcaseHyperlink(LnkExternalIntro, Webcasts.EXTERNAL_BASICS);
            Webcasts.FormatWebcaseHyperlink(LnkLibrary, Webcasts.LIBRARY);
            Webcasts.FormatWebcaseHyperlink(LnkPDFViewer, Webcasts.PDF_VIEWER);
            Webcasts.FormatWebcaseHyperlink(LnkBrainstorm, Webcasts.BRAINSTORM);
            Webcasts.FormatWebcaseHyperlink(LnkExpedition, Webcasts.EXPEDITION);
            Webcasts.FormatWebcaseHyperlink(LnkBibTeXSniffer, Webcasts.BIBTEX_SNIFFER);
            Webcasts.FormatWebcaseHyperlink(LnkInCite, Webcasts.INCITE);
            Webcasts.FormatWebcaseHyperlink(LnkPlay, Webcasts.PLAY);

            ImageHelpers.Source = Icons.GetAppIcon(Icons.GlossHelpers, "jpg");
            //RenderOptions.SetBitmapScalingMode(ImageHelpers, BitmapScalingMode.HighQuality);
            MouseWheelDisabler.DisableMouseWheelForControl(ImageHelpers);

            HyperlinkForums.Click += HyperlinkForums_Click;
        }

        private void HyperlinkForums_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.Url_Forums);
        }
    }
}
