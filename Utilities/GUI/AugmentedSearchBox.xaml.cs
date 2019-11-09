using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;

namespace Utilities.GUI
{
    /// <summary>
    /// Interaction logic for AugmentedSearchBox.xaml
    /// </summary>
    public partial class AugmentedSearchBox : UserControl
    {
        public delegate void OnSearchDelegate();
        public event OnSearchDelegate OnSoftSearch;
        public event OnSearchDelegate OnHardSearch;

        public AugmentedSearchBox()
        {
            InitializeComponent();

            ImageClear.Source = Icons.GetAppIcon(Icons.Clear);
            ImageClear.MouseLeftButtonUp += ImageClear_MouseLeftButtonUp;
            ImageClear.Cursor = Cursors.Hand;

            TextSearch.FilterMode = AutoCompleteFilterMode.Contains;
            TextSearch.TextChanged += TextSearch_TextChanged;
            TextSearch.KeyUp += TextSearch_KeyUp;
            TextSearch.GotKeyboardFocus += TextSearch_GotKeyboardFocus;

            TextPrompt.Text = "Enter search query...";
            TextPrompt.Foreground = Brushes.Gray;
            TextPrompt.Focusable = false;

            ReconsiderEmptyText();
        }

        public void SelectAll()
        {
        }

        public AutoCompleteBox AutoCompleteBox => TextSearch;

        private HashSet<string> search_items = new HashSet<string>();
        public HashSet<string> SearchHistoryItemSource
        {
            get => search_items;
            set
            {
                search_items = value;

                // If no search items have been provided, use our own
                if (null == search_items)
                {
                    search_items = new HashSet<string>();
                }

                TextSearch.ItemsSource = search_items;
            }
        }

        private void TextSearch_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextSearch.ItemsSource = null;
            TextSearch.ItemsSource = search_items;
        }


        public string EmptyTextPrompt
        {
            get => TextPrompt.Text;
            set
            {
                TextPrompt.Text = value;
                ToolTip = value;
            }
        }

        public string Text
        {
            get => TextSearch.Text;
            set => TextSearch.Text = value;
        }

        public void FocusSearchArea()
        {
            TextBox text_box = TextSearch.Template.FindName("Text", TextSearch) as TextBox;
            if (null != text_box)
            {
                Keyboard.Focus(text_box);
                text_box.Focus();
            }
        }

        private void TextSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
            {
                FireOnHardSearch();
                SelectAll();
                e.Handled = true;
            }
            else if (Key.Escape == e.Key)
            {
                Text = "";
                FireOnHardSearch();
                SelectAll();
                e.Handled = true;
            }
        }

        private void ReconsiderEmptyText()
        {
            if (0 == TextSearch.Text.Length)
            {
                TextSearch.Background = Brushes.Transparent;
            }
            else
            {
                TextSearch.Background = Brushes.White;
            }
        }

        private void TextSearch_TextChanged(object sender, RoutedEventArgs e)
        {
            ReconsiderEmptyText();
            FireOnSoftSearch();
        }

        private void ImageClear_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            TextSearch.Text = "";
            FireOnHardSearch();
        }

        private void FireOnSoftSearch()
        {
            if (null != OnSoftSearch)
            {
                OnSoftSearch();
            }
        }

        private void RefillDropdown()
        {
            TextSearch.ItemsSource = null;
            TextSearch.ItemsSource = search_items;
        }

        private void FireOnHardSearch()
        {
            search_items.Remove(TextSearch.Text);
            search_items.Add(TextSearch.Text);
            RefillDropdown();

            if (null != OnHardSearch)
            {
                OnHardSearch();
            }
        }
    }
}
