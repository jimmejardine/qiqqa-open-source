using System;
using System.Text;
using System.Windows;
using Utilities;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Text
{
    internal class TextSelectionManager
    {
        private static readonly WordList WORDS_EMPTY = new WordList();
        private TextLayerSelectionMode text_layer_selection_mode;
        private WordList words;
        private double actual_width;
        private double actual_height;
        private WordList last_selected_words = WORDS_EMPTY;

        public WordList OnDragStarted(TextLayerSelectionMode text_layer_selection_mode, WordList words, double actual_width, double actual_height, bool button_left_pressed, bool button_right_pressed, Point mouse_down_point)
        {
            this.text_layer_selection_mode = text_layer_selection_mode;
            this.words = words;
            this.actual_width = actual_width;
            this.actual_height = actual_height;

            if (null == words)
            {
                Logging.Info("OCR has not been done so no text to select");
                return WORDS_EMPTY;
            }

            if (button_left_pressed)
            {
                last_selected_words = WORDS_EMPTY;
                WordList selected_words = new WordList();

                // Find any clicked word
                double mouse_down_left = mouse_down_point.X / actual_width;
                double mouse_down_top = mouse_down_point.Y / actual_height;

                foreach (Word word in words)
                {
                    if (word.Contains(mouse_down_left, mouse_down_top))
                    {
                        selected_words.Add(word);
                    }
                }

                return selected_words;
            }
            else
            {
                return WORDS_EMPTY;
            }
        }
        public WordList OnDragInProgress(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_move_point)
        {
            if (button_left_pressed)
            {
                return DoTextSentenceItemSelection(mouse_down_point, mouse_move_point);
            }
            else
            {
                return WORDS_EMPTY;
            }
        }

        public WordList OnDragComplete(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point)
        {
            if (button_left_pressed)
            {
                last_selected_words = DoTextSentenceItemSelection(mouse_down_point, mouse_up_point);
                return last_selected_words;
            }
            else
            {
                return last_selected_words;
            }
        }

        public WordList GetLastSelectedWords()
        {
            return last_selected_words;
        }

        public string GetLastSelectedWordsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Word word in last_selected_words)
            {
                sb.Append(word.Text);
                sb.Append(' ');
            }

            string selected_text = sb.ToString().TrimEnd(' ');
            return selected_text;
        }

        private WordList DoTextSentenceItemSelection(Point mouse_down_point, Point mouse_move_point)
        {
            if (null == words)
            {
                Logging.Info("OCR has not been done so no text to select");
                return WORDS_EMPTY;
            }

            switch (text_layer_selection_mode)
            {
                case TextLayerSelectionMode.Sentence:
                    return DoTextSentenceItemSelection_Sentence(mouse_down_point, mouse_move_point);
                case TextLayerSelectionMode.Block:
                    return DoTextItemSelection_Block(mouse_down_point, mouse_move_point);
                default:
                    Logging.Error("Unknown mode " + text_layer_selection_mode);
                    return WORDS_EMPTY;
            }
        }

        private WordList DoTextItemSelection_Block(Point mouse_down_point, Point mouse_move_point)
        {
            WordList selected_words = new WordList();

            double left = Math.Min(mouse_move_point.X, mouse_down_point.X) / actual_width;
            double top = Math.Min(mouse_move_point.Y, mouse_down_point.Y) / actual_height;
            double width = Math.Abs(mouse_move_point.X - mouse_down_point.X) / actual_width;
            double height = Math.Abs(mouse_move_point.Y - mouse_down_point.Y) / actual_height;

            foreach (Word word in words)
            {
                bool is_inside = true;
                is_inside = is_inside && word.Left >= left;
                is_inside = is_inside && word.Left + word.Width <= left + width;
                is_inside = is_inside && word.Top >= top;
                is_inside = is_inside && word.Top + word.Height <= top + height;

                if (is_inside)
                {
                    selected_words.Add(word);
                }
            }

            return selected_words;
        }

        private WordList DoTextSentenceItemSelection_Sentence(Point mouse_down_point, Point mouse_move_point)
        {
            WordList selected_words = new WordList();

            double down_left = mouse_down_point.X / actual_width;
            double down_top = mouse_down_point.Y / actual_height;
            double move_left = mouse_move_point.X / actual_width;
            double move_top = mouse_move_point.Y / actual_height;

            Word word_down = null;
            Word word_move = null;

            // Find the boundary words
            foreach (Word word in words)
            {
                if (word.Left <= down_left && word.Right >= down_left && word.Top <= down_top && word.Bottom >= down_top)
                {
                    word_down = word;
                    break;
                }
            }
            foreach (Word word in words)
            {
                if (word.Left <= move_left && word.Right >= move_left && word.Top <= move_top && word.Bottom >= move_top)
                {
                    word_move = word;
                    break;
                }
            }

            // If they didnt click directly on a boundary word, look for the nearest word            
            if (null == word_down)
            {
                word_down = FindNearestWord(words, down_left, down_top);
            }
            if (null == word_move)
            {
                word_move = FindNearestWord(words, move_left, move_top);
            }

            // If we finally have anchor words, get the text in between them
            if (null != word_down && null != word_move)
            {
                bool is_inside = false;

                foreach (Word word in words)
                {
                    if (word_down == word) is_inside = !is_inside;
                    if (word_move == word) is_inside = !is_inside;

                    if (is_inside || word == word_down || word == word_move)
                    {
                        selected_words.Add(word);
                    }
                }
            }

            return selected_words;
        }

        private static Word FindNearestWord(WordList words, double left, double top)
        {
            Word nearest_word = null;
            double nearest_distance = Double.PositiveInfinity;

            foreach (Word word in words)
            {
                double distance = DistanceSquaredToWord(left, top, word);
                if (distance < nearest_distance)
                {
                    nearest_distance = distance;
                    nearest_word = word;
                }
            }

            return nearest_word;
        }

        // NB: Not true distance.  Only an approximation...
        private static double DistanceSquaredToWord(double left, double top, Word word)
        {
            double distance = Double.PositiveInfinity;
            distance = Math.Min(distance, DistanceSquared(left, top, word.Left, word.Top));
            distance = Math.Min(distance, DistanceSquared(left, top, word.Right, word.Top));
            distance = Math.Min(distance, DistanceSquared(left, top, word.Left, word.Bottom));
            distance = Math.Min(distance, DistanceSquared(left, top, word.Right, word.Bottom));
            distance = Math.Min(distance, DistanceSquared(left, top, (word.Left + word.Right) / 2, (word.Top + word.Bottom) / 2));
            return distance;
        }

        private static double DistanceSquared(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }
    }
}
