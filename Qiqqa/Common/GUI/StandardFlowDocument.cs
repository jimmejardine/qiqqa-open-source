using System.Windows.Documents;
using Utilities.GUI;

namespace Qiqqa.Common.GUI
{
    public class StandardFlowDocument : FlowDocument
    {
        public StandardFlowDocument()
        {
            Theme.Initialize();

            Background = ThemeColours.Background_Brush_Blue_Light;
            FontSize = 13;
            FontFamily = ThemeTextStyles.FontFamily_Standard;
        }
    }
}
