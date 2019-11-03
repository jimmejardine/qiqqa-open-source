using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Utilities.GUI
{
    public class HyperlinkTextBlock : TextBlock
    {
        private readonly Brush BRUSH_HOVER;
        private readonly Brush BRUSH_LIGHT;

        public event MouseButtonEventHandler OnClick;

        public HyperlinkTextBlock()
        {
            Theme.Initialize();

            BRUSH_HOVER = ThemeColours.Background_Brush_Blue_VeryDark;
            BRUSH_LIGHT = ThemeColours.Background_Brush_Blue_VeryVeryVeryDark;

            TextDecorations.Add(System.Windows.TextDecorations.Underline);
            Cursor = Cursors.Hand;
            Foreground = BRUSH_LIGHT;

            MouseEnter += HyperlinkTextBlock_MouseEnter;
            MouseLeave += HyperlinkTextBlock_MouseLeave;

            MouseLeftButtonDown += HyperlinkTextBlock_MouseLeftButtonDown;
            MouseLeftButtonUp += HyperlinkTextBlock_MouseLeftButtonUp;
        }

        private void HyperlinkTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null != OnClick)
            {
                e.Handled = true;
            }
        }

        private void HyperlinkTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null != OnClick)
            {
                OnClick(this, e);
                e.Handled = true;
            }
        }

        private void HyperlinkTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            Foreground = BRUSH_LIGHT;
        }

        private void HyperlinkTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Foreground = BRUSH_HOVER;
        }
    }
}
