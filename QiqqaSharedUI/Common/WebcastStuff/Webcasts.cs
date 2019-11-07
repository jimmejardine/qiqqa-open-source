using System.Windows;
using System.Windows.Documents;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;

namespace Qiqqa.Common.WebcastStuff
{
    internal class Webcasts
    {
        public static readonly Webcast INCITE = new Webcast("INCITE", "InCite&Word", "Watch the short tutorial to get up to speed quickly with formatting references with Qiqqa InCite and Microsoft Word.", "http://www.youtube.com/watch?v=8MqeYNXNwrg");
        public static readonly Webcast LIBRARY = new Webcast("LIBRARY", "Library", "Watch the short tutorial to get up to speed quickly with the Qiqqa Library screen.", "http://www.youtube.com/watch?v=A4tzyuRd3e8");
        public static readonly Webcast PDF_VIEWER = new Webcast("PDF_VIEWER", "PDFReader", "Watch the short tutorial to get up to speed quickly with the Qiqqa PDF viewer.", "http://www.youtube.com/watch?v=kgydEDbqhxw");
        public static readonly Webcast BIBTEX_SNIFFER = new Webcast("BIBTEX_SNIFFER", "BibTeX", "Watch the short tutorial to get up to speed quickly with populating your PDFs with their associated journal metadata.", "http://www.youtube.com/watch?v=-5tiHs2EqUU");
        public static readonly Webcast BRAINSTORM = new Webcast("BRAINSTORM", "Brainstorm", "Watch the short tutorial to get up to speed quickly with using Qiqqa's brainstorming capabilities.", "http://www.youtube.com/watch?v=6A8C0b03PAQ");
        public static readonly Webcast BRAINSTORM_THEMES = new Webcast("BRAINSTORM_THEMES", "Brainstorm", "Watch the short tutorial that shows you how to use the brainstorm to explore your library.", "http://www.youtube.com/watch?v=5QF-4DNGSHk#t=9m01s");
        public static readonly Webcast EXPEDITION = new Webcast("EXPEDITION", "Expedition", "Watch the short tutorial that tells you how Qiqqa Expedition helps you understand your field of research.", "http://www.youtube.com/watch?v=ttg0B1RFuaw");
        public static readonly Webcast PLAY = new Webcast("PLAY", "Play!", "Watch Oli take Qiqqa for a spin!", "http://www.youtube.com/watch?v=PHll7sGQdEE");
        public static readonly Webcast EXTERNAL_BASICS = new Webcast("EXTERNAL_BASICS", "Basics", "Watch a 30-minute tutorial by the McKillop Library covering the basic features of Qiqqa.", "http://www.youtube.com/watch?v=kYa9KzpVvn8");
        public static readonly Webcast IDEAS = new Webcast("IDEAS", "Ideas", "Watch a 3-minute tutorial describing some of the things you can do with Qiqqa.", "http://www.youtube.com/watch?v=JOx-Sq8ZM5M");
        public static readonly Webcast FIRST10MINS = new Webcast("FIRST10MINS", "Getting started", "Your first 10 minutes with Qiqqa!", "http://www.youtube.com/watch?v=b01GdtlyOmE");
        public static readonly Webcast EXPLORE = new Webcast("EXPLORE", "Explore", "Watch a 10-minute tutorial describing how you can explore your PDFs, from start to finish.", "http://www.youtube.com/watch?v=5QF-4DNGSHk");

        public static void FormatWebcastButton(AugmentedButton ButtonWebcast, Webcast webcast)
        {
            ButtonWebcast.Icon = Icons.GetAppIcon(Icons.Webcast);
            ButtonWebcast.ToolTip = webcast.description;
            ButtonWebcast.Tag = webcast;
            ButtonWebcast.Click += ButtonWebcast_Click;
        }

        public static void FormatWebcaseHyperlink(Hyperlink hyperlink, Webcast webcast)
        {
            hyperlink.Inlines.Add(webcast.title);
            hyperlink.ToolTip = webcast.description;
            hyperlink.Tag = webcast;
            hyperlink.Click += hyperlink_Click;
        }

        private static void hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink fe = sender as Hyperlink;
            Webcast webcast = fe.Tag as Webcast;
            OpenWebcast(webcast);

        }

        private static void ButtonWebcast_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            Webcast webcast = fe.Tag as Webcast;
            OpenWebcast(webcast);
        }

        private static void OpenWebcast(Webcast webcast)
        {
            FeatureTrackingManager.Instance.UseFeature(
                Features.Web_AddToLibrary,
                "KEY", webcast.key
                );

            WebsiteAccess.OpenOffsiteUrl(webcast.url);
        }
    }
}
