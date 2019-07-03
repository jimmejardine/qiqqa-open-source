using System.Reflection;
using System.Windows.Documents;
using System.Windows.Media;

namespace Qiqqa.StartPage.Formatting
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = false)]
    class SectionHeader : Paragraph
    {
        static readonly Brush brush = new SolidColorBrush(Color.FromRgb(47, 104, 149));

        public SectionHeader()
        {
            this.FontSize = 30;
            this.Foreground = brush;
        }
    }
}
