using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter
{
    internal class LibraryFilterHelpers
    {
        public static HyperlinkTextBlock xxxGetClearHyperlink(string header, MouseButtonEventHandler on_click)
        {
            HyperlinkTextBlock hyperlink_clear = new HyperlinkTextBlock();
            hyperlink_clear.Text = header;
            hyperlink_clear.OnClick += on_click;
            return hyperlink_clear;
        }

        public static Inline GetClearImageInline(string header, MouseButtonEventHandler on_click)
        {
            InlineUIContainer uicont = new InlineUIContainer();
            uicont.BaselineAlignment = BaselineAlignment.Center;
            uicont.Background = Brushes.Transparent;
            Image image = GetClearImage(header, on_click);
            uicont.Child = image;
            return uicont;
        }

        public static Image GetClearImage(string header, MouseButtonEventHandler on_click)
        {
            Image image = new Image();
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            image.Width = 24;
            image.ToolTip = header;
            image.Cursor = Cursors.Hand;
            image.Source = Icons.GetAppIcon(Icons.Clear);
            image.MouseDown += on_click;
            return image;
        }
    }
}
