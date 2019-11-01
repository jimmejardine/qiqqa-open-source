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
                    "@Qiqqa rocks. Kudos!",
                    "I'm loving reading my PDFs using @Qiqqa",
                    "Reading another paper today with @Qiqqa.",
                    "I saved hours on my research today using @Qiqqa",
                    "I've just got Premium for free. Check out @Qiqqa at",
                    "Highlighting and annotating like a boss! with @Qiqqa",
                    "My research could never be this organised without @Qiqqa",
                    "Check out @Qiqqa, the world's most advanced reference manager.",
                    "My research project is going to be done in a flash with @Qiqqa's help!",
                    "AutoBibTeX gets meta data for my papers automatically! Nice work @Qiqqa",
                    "With @Qiqqa I will never lose that important piece of information again!",
                    "One paper down, 500 to go, no wait, make that 10...thanks @Qiqqa Expedition!",
                    "@Qiqqa's InCite suggests papers to cite that I haven't even read yet... Magic!",
                    "You absolutely have to try out @Qiqqa to make your research management a breeze!",
                    "Store your papers in @Qiqqa, sync them to lab, home and your Android. Brilliant!",
                    "@Qiqqa really has you covered for the forgotten half of research...quite literally!",
                    "Tagging my papers with @Qiqqa is powerful but its Autotagging of my papers is amazing!",
                    "Want to save time reading so you can do more researching? Check out @Qiqqa's speed reader!",
                    "@Qiqqa InCite, the best referencer manager there is. Practically writes your paper for you!",
                    "@Qiqqa's Annotation Report saves me so much time by summarising all my highlights and annotations!",
                    "Exploring my papers and their connections is powerful and fun with @Qiqqa expedition and brainstorms!",
                    "I will never print another PDF...@Qiqqa annotation report recalls all the annotations I've forgotten about.",
            };

            return TWEET_SELECTION[RandomAugmented.Instance.NextIntExclusive(TWEET_SELECTION.Length)];
        }

        private string CreatePaperTweet()
        {
            var pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable) return null;

            PDFDocument pdf_document = pdf_document_bindable.Underlying;

            BibTexItem bibtex_item = pdf_document.BibTexItem;

            if (!bibtex_item.HasTitle()) return null;

            if (!bibtex_item.HasAuthor()) return null;
            List<NameTools.Name> names = NameTools.SplitAuthors(bibtex_item.GetAuthor());
            if (0 == names.Count) return null;

            return String.Format("I'm reading {1}'s '{0}' with @Qiqqa", bibtex_item.GetTitle(), names[0].last_name);
        }

        private void GenerateTweet()
        {
            string POSTAMBLE_QIQQA_URL = " " + WebsiteAccess.Url_Documentation4Qiqqa;
            string tweet_paper = CreatePaperTweet() + POSTAMBLE_QIQQA_URL;
            string tweet_random = GetRandomTweet() + POSTAMBLE_QIQQA_URL;

            if (null != tweet_paper && tweet_paper.Length <= 140)
            {
                // Give a 2 in 3 chance to the paper tweet
                if (0 != RandomAugmented.Instance.NextIntExclusive(3))
                {
                    tweet_random = tweet_paper;
                }
            }
            TxtTweet.Text = tweet_random;
        }
    }
}
