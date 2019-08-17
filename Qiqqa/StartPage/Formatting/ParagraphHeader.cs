using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Qiqqa.StartPage.Formatting
{
    class ParagraphHeader : Paragraph
    {
        static readonly Brush brush = new SolidColorBrush(Color.FromRgb(47, 104, 149));
        
        public ParagraphHeader()
        {
            this.FontWeight = FontWeights.Bold;
            this.FontSize = 14;
            this.Foreground = brush;
            this.Margin = new Thickness(0,24,0,0);  //bit of seperation from other content
        }
    }
}
