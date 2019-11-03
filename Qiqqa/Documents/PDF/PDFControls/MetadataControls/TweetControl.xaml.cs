using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.InCite;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Language;
using Utilities.Random;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for TweetControl.xaml
    /// </summary>
    public partial class TweetControl : UserControl
    {
        public TweetControl()
        {
            InitializeComponent();

            ToolTip =
                "Love OpenSource Qiqqa?  Want to help us spread the word?";

            ButtonSubmit.Icon = Icons.GetAppIcon(Icons.Tweet);
            ButtonSubmit.IconWidth = 32;
            ButtonSubmit.CaptionDock = Dock.Right;

            ButtonSubmit.Click += ButtonSubmit_Click;

            DataContextChanged += TweetControl_DataContextChanged;
        }

        private void ButtonSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string url = String.Format(WebsiteAccess.Url_TwitterTweetSubmit, Uri.EscapeDataString(TxtTweet.Text));
            Process.Start(url);
        }

        private void TweetControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            GenerateTweet();
        }

        private string GetRandomTweet()
        {
            string[] TWEET_SELECTION = new string[]
            {
                    "@Qiqqa rocks. Kudos! http://qiqqa.com",
                    "I'm loving reading my PDFs using @Qiqqa http://qiqqa.com",
                    "Reading another paper today with @Qiqqa. http://qiqqa.com",
                    "I saved hours on my research today using @Qiqqa http://qiqqa.com",
                    "I've just got Premium for free. Check out @Qiqqa at http://qiqqa.com",
                    "Highlighting and annotating like a boss! with @Qiqqa http://qiqqa.com",
                    "My research could never be this organised without @Qiqqa http://qiqqa.com",
                    "Check out @Qiqqa, the world's most advanced reference manager. http://qiqqa.com",
                    "My research project is going to be done in a flash with @Qiqqa's help! http://qiqqa.com",
                    "AutoBibTeX gets meta data for my papers automatically! Nice work @Qiqqa http://qiqqa.com",
                    "With @Qiqqa I will never lose that important piece of information again! http://qiqqa.com",
                    "One paper down, 500 to go, no wait, make that 10...thanks @Qiqqa Expedition! http://qiqqa.com",
                    "@Qiqqa's InCite suggests papers to cite that I haven't even read yet... Magic! http://qiqqa.com",
                    "You absolutely have to try out @Qiqqa to make your research management a breeze! http://qiqqa.com",
                    "Store your papers in @Qiqqa, sync them to lab, home and your Android. Brilliant! http://qiqqa.com",
                    "@Qiqqa really has you covered for the forgotten half of research...quite literally! http://qiqqa.com",
                    "Tagging my papers with @Qiqqa is powerful but its Autotagging of my papers is amazing! http://qiqqa.com",
                    "Want to save time reading so you can do more researching? Check out @Qiqqa's speed reader! http://qiqqa.com",
                    "@Qiqqa InCite, the best referencer manager there is. Practically writes your paper for you! http://qiqqa.com",
                    "@Qiqqa's Annotation Report saves me so much time by summarising all my highlights and annotations! http://qiqqa.com",
                    "Exploring my papers and their connections is powerful and fun with @Qiqqa expedition and brainstorms! http://qiqqa.com",
                    "I will never print another PDF...@Qiqqa annotation report recalls all the annotations I've forgotten about. http://qiqqa.com",
            };

            return TWEET_SELECTION[RandomAugmented.Instance.NextIntExclusive(TWEET_SELECTION.Length)];
        }

        private string CreatePaperTweet()
        {
            var pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable) return null;

            PDFDocument pdf_document = pdf_document_bindable.Underlying;

            BibTexItem bibtex_item = pdf_document.BibTexItem;

            if (!BibTexTools.HasTitle(bibtex_item)) return null;

            if (!BibTexTools.HasAuthor(bibtex_item)) return null;
            List<NameTools.Name> names = NameTools.SplitAuthors(BibTexTools.GetAuthor(bibtex_item));
            if (0 == names.Count) return null;

            string tweet = String.Format("I'm reading {1}'s '{0}' with @Qiqqa http://qiqqa.com", BibTexTools.GetTitle(bibtex_item), names[0].last_name);
            if (140 < tweet.Length) return null;

            return tweet;
        }

        private void GenerateTweet()
        {
            string tweet_paper = CreatePaperTweet();
            string tweet_random = GetRandomTweet();

            if (null == tweet_paper)
            {
                TxtTweet.Text = tweet_random;
            }
            else
            {
                // Give a 2 in 3 chance to the paper tweet
                if (0 != RandomAugmented.Instance.NextIntExclusive(3))
                {
                    TxtTweet.Text = tweet_paper;
                }
                else
                {
                    TxtTweet.Text = tweet_random;
                }
            }
        }

        private void CreatePaperBibTeXTweet()
        {
            var pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable) return;

            PDFDocument pdf_document = pdf_document_bindable.Underlying;

            TxtTweet.Text = "I'm loving reading my PDFs using @Qiqqa http://qiqqa.com";
            if (!String.IsNullOrEmpty(pdf_document.TitleCombined))
            {
                TxtTweet.Text = String.Format("I'm busy reading {0} using @Qiqqa http://qiqqa.com", pdf_document.TitleCombined);
            }

            PDFDocumentCitingTools.CiteSnippetPDFDocument(true, pdf_document, GenerateRtfCitationSnippet_OnBibliographyReady);
        }

        private void GenerateRtfCitationSnippet_OnBibliographyReady(CSLProcessorOutputConsumer ip)
        {
            if (ip.success)
            {
                using (System.Windows.Forms.RichTextBox rich_text_box = new System.Windows.Forms.RichTextBox())
                {
                    rich_text_box.Rtf = ip.GetRtf();
                    string text = rich_text_box.Text;

                    text = text.Trim();
                    text = text.Replace(";", ",");
                    if (text.StartsWith("1.\t")) text = text.Substring(3);
                    if (text.StartsWith("1 \t")) text = text.Substring(3);
                    if (text.StartsWith("1\t")) text = text.Substring(2);

                    if (text.Length > 32)
                    {
                        string POSTAMBLE_QIQQA_TWITTER = " @Qiqqa";
                        string POSTAMBLE_QIQQA_URL = " http://www.qiqqa.com";
                        int POSTAMBLE_QIQQA_URL_SHORTENED_LENGTH = 22;
                        string PREAMBLE_CHECK_OUT = "Check out ";

                        if (text.Length < 140 - POSTAMBLE_QIQQA_TWITTER.Length) text = text + POSTAMBLE_QIQQA_TWITTER;
                        if (text.Length < 140 - POSTAMBLE_QIQQA_URL_SHORTENED_LENGTH) text = text + POSTAMBLE_QIQQA_URL;
                        if (text.Length < 140 - PREAMBLE_CHECK_OUT.Length) text = PREAMBLE_CHECK_OUT + text;

                        TxtTweet.Text = text;
                    }
                }
            }
            else
            {
                Logging.Warn("There was a problem generating a citation snippet to tweet, so ignoring...");
            }
        }
    }
}
