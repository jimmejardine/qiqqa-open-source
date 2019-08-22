using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.InfoBarStuff.PDFDocumentTagCloudStuff
{
    /// <summary>
    /// Interaction logic for TagCloudRendererControl.xaml
    /// </summary>
    public partial class TagCloudRendererControl : UserControl
    {
        public delegate void TagClickDelegate(List<string> tags);
        public event TagClickDelegate TagClick;

        static readonly Thickness TEXT_BLOCK_PADDING = new Thickness(2, 1, 2, 1);
        static readonly int TEXT_BLOCK_FONT_SIZE = 24;
        static readonly int TEXT_BLOCK_FONT_SIZE_MINIMUM = 12;
        
        public TagCloudRendererControl()
        {
            Theme.Initialize();

            InitializeComponent();
        }

        List<TagCloudEntry> entries;
        public List<TagCloudEntry> Entries
        {
            set
            {
                entries = value;

                TagPanel.Children.Clear();

                if (0 != entries.Count)
                {
                    double largest_prob = entries[0].importance;
                    if (0 == largest_prob) largest_prob = 1;
                    
                    for (int i = 0; i < 30 && i < entries.Count; ++i)
                    {
                        TextBlock text_block_word = new TextBlock();                        
                        text_block_word.Padding = TEXT_BLOCK_PADDING;
                        text_block_word.FontSize = Math.Max(TEXT_BLOCK_FONT_SIZE * entries[i].importance / largest_prob, TEXT_BLOCK_FONT_SIZE_MINIMUM);
                        text_block_word.Tag = entries[i];
                        text_block_word.Text = entries[i].word;
                        text_block_word.Cursor = Cursors.Hand;
                        text_block_word.ToolTip = String.Format("'{0}' appears {1} time(s) in this paper and in {2} paper(s) in your library.", entries[i].word, entries[i].word_count, entries[i].document_count);
                        text_block_word.MouseUp += text_block_word_MouseUp;

                        text_block_word.MouseEnter += text_block_word_MouseEnter;
                        text_block_word.MouseLeave += text_block_word_MouseLeave;

                        TagPanel.Children.Add(text_block_word);
                    }
                }
                else
                {
                    string message = "Could not generate a tag cloud for this document.  Please make sure you have built your AutoTags recently...";

                    foreach (string word in message.Split(' '))
                    {
                        TextBlock text_block_word = new TextBlock();
                        text_block_word.Padding = TEXT_BLOCK_PADDING;
                        text_block_word.FontSize = 0.5 * TEXT_BLOCK_FONT_SIZE;
                        text_block_word.Text = word;
                        TagPanel.Children.Add(text_block_word);
                    }
                }
            }
        }

        void text_block_word_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock) sender;
            tb.Background = Brushes.Transparent;
        }

        void text_block_word_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Background = ThemeColours.Background_Brush_Blue_LightToVeryLight;
        }

        void text_block_word_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (null != TagClick)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Document_TagCloud);

                // If they are not holding down CTRL, clear down the already selected items
                if (!KeyboardTools.IsCTRLDown())
                {
                    foreach (var tag in entries)
                    {
                        tag.selected = false;
                    }

                    foreach (TextBlock text_block_word in TagPanel.Children)
                    {
                        text_block_word.FontWeight = FontWeights.Normal;
                    }
                }

                {
                    // Find the clicked tag
                    TextBlock text_block_word = (TextBlock)sender;
                    TagCloudEntry entry = (TagCloudEntry)text_block_word.Tag;
                    entry.selected = !entry.selected;
                    text_block_word.FontWeight = entry.selected ? FontWeights.Bold : FontWeights.Normal;
                }

                // Get all the selected tags
                List<string> tags_selected = new List<string>();
                {                    
                    if (null != entries)
                    {
                        foreach (var tag in entries)
                        {
                            if (tag.selected)
                            {
                                tags_selected.Add(tag.word);
                            }
                        }
                    }
                }

                TagClick(tags_selected);
            }
        }
    }
}
