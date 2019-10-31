using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Utilities.GUI
{
    public class HyperlinkTextBlock : TextBlock
    {
        readonly Brush BRUSH_HOVER;
        readonly Brush BRUSH_LIGHT;

        public event MouseButtonEventHandler OnClick;
        
        public HyperlinkTextBlock()
        {
            Theme.Initialize();

            BRUSH_HOVER = ThemeColours.Background_Brush_Blue_VeryDark;
            BRUSH_LIGHT = ThemeColours.Background_Brush_Blue_VeryVeryVeryDark;

            this.TextDecorations.Add(System.Windows.TextDecorations.Underline);
            this.Cursor = Cursors.Hand;
            this.Foreground = BRUSH_LIGHT;

            this.MouseEnter += HyperlinkTextBlock_MouseEnter;
            this.MouseLeave += HyperlinkTextBlock_MouseLeave;

            this.MouseLeftButtonDown += HyperlinkTextBlock_MouseLeftButtonDown;
            this.MouseLeftButtonUp += HyperlinkTextBlock_MouseLeftButtonUp;
        }

        void HyperlinkTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null != OnClick)
            {
                e.Handled = true;
            }
        }

        void HyperlinkTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null != OnClick)
            {
                OnClick(this, e);
                e.Handled = true;
            }
        }

        void HyperlinkTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Foreground = BRUSH_LIGHT;   
        }

        void HyperlinkTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Foreground = BRUSH_HOVER;
        }
    }
}
