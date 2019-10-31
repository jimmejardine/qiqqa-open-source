using System.Reflection;
using System.Windows.Documents;
using Utilities.GUI;

namespace Qiqqa.Common.GUI
{
    public class StandardFlowDocument : FlowDocument
    {
        public StandardFlowDocument()
        {
            Theme.Initialize();

            this.Background = ThemeColours.Background_Brush_Blue_Light;
            this.FontSize = 13;
            this.FontFamily = ThemeTextStyles.FontFamily_Standard;
        }
    }
}
