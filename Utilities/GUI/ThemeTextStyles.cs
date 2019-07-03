using System.Windows;
using System.Windows.Media;

namespace Utilities.GUI
{
    /// <summary>
    /// Intention here is to provide standardized bits for text presentation.
    /// </summary>
    public class ThemeTextStyles
    {
        public static readonly FontFamily FontFamily_Standard = new FontFamily("Segoe UI, Arial, Trebuchet MS");
        public static readonly FontFamily FontFamily_Header = new FontFamily("Segoe UI Light, Segoe UI, Arial, Trebuchet MS");

        public static readonly Color Color_Neutral_Medium = ThemeColours.Background_Color_Neutral_Medium;
        public static readonly Brush Brush_Neutral_Medium = new SolidColorBrush(Color_Neutral_Medium);
        
        
        public static void AddToApplicationResources(Application application)
        {
            application.Resources.Add("FontFamily_Standard", FontFamily_Standard);
            application.Resources.Add("FontFamily_Header", FontFamily_Header);
            application.Resources.Add("Color_Neutral_Medium", Color_Neutral_Medium);
            application.Resources.Add("Brush_Neutral_Medium", Brush_Neutral_Medium);            
        }
    }
}
