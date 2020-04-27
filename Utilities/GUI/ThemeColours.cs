using System;
using System.Windows;
using System.Windows.Media;
using Utilities.Misc;

namespace Utilities.GUI
{
    public class ThemeColours
    {
        private static readonly QuantisleUserRegistry qur = new QuantisleUserRegistry("Misc");

        public static int HEADER_FONT_SIZE = 16;

        public static readonly Color[] THEMES = new Color[]
        {
            Color.FromRgb(234, 234, 234),
            Color.FromRgb(238, 230, 234),
            Color.FromRgb(234, 238, 230),
            Color.FromRgb(230, 234, 238),
            Color.FromRgb(238, 234, 230),
            Color.FromRgb(230, 238, 234),
            Color.FromRgb(234, 230, 238),
        };

        private static readonly Color DEFAULT_THEME_COLOUR = THEMES[0];

        public static Color GetThemeColour()
        {
            try
            {
                string theme_colour_base = qur.Read("ThemeColourBase");
                if (!String.IsNullOrEmpty(theme_colour_base))
                {
                    return ColorTools.HEXToColor(theme_colour_base);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem loading the ThemeColours base.");
            }

            return DEFAULT_THEME_COLOUR;
        }

        public static void ClearThemeColour()
        {
            qur.Write("ThemeColourBase", "");
        }

        public static void SetThemeColour(Color theme_colour_base)
        {
            if (Colors.Black == theme_colour_base)
            {
                ClearThemeColour();
            }
            else
            {
                qur.Write("ThemeColourBase", ColorTools.ColorToHEX(theme_colour_base));
            }
        }

        public static readonly Color Background_Color_Blue_Dark = GetThemeColour();

        public static readonly Color Background_Color_Neutral_VeryLight = Color.FromRgb(240, 240, 240);
        public static readonly Color Background_Color_Neutral_Light = Color.FromRgb(220, 220, 220);
        public static readonly Color Background_Color_Neutral_Medium = Color.FromRgb(170, 170, 170);

        public static readonly Color Background_Color_Blue_VeryLight = ColorTools.MakeLighterColor(Background_Color_Blue_Dark, 1.1);
        public static readonly Color Background_Color_Blue_Light = ColorTools.MakeLighterColor(Background_Color_Blue_Dark, 1.05);
        public static readonly Color Background_Color_Blue_VeryDark = ColorTools.MakeDarkerColor(Background_Color_Blue_Dark, 1.2);
        public static readonly Color Background_Color_Blue_VeryVeryDark = ColorTools.MakeDarkerColor(Background_Color_Blue_Dark, 1.4);
        public static readonly Color Background_Color_Blue_VeryVeryVeryDark = ColorTools.MakeDarkerColor(Background_Color_Blue_Dark, 1.5);

        //Semantic colours. 
        public static readonly Color Background_Color_Warning_Light = Color.FromRgb(255, 194, 46);
        public static readonly Color Background_Color_Warning_Dark = Color.FromRgb(241, 182, 39);

        public static readonly Brush Background_Brush_Blue_Light = new SolidColorBrush(Background_Color_Blue_Light);
        public static readonly Brush Background_Brush_Blue_Dark = new SolidColorBrush(Background_Color_Blue_Dark);
        public static readonly Brush Background_Brush_Blue_VeryDark = new SolidColorBrush(Background_Color_Blue_VeryDark);
        public static readonly Brush Background_Brush_Blue_VeryVeryDark = new SolidColorBrush(Background_Color_Blue_VeryVeryDark);
        public static readonly Brush Background_Brush_Blue_VeryVeryVeryDark = new SolidColorBrush(Background_Color_Blue_VeryVeryVeryDark);
        public static readonly Brush Background_Brush_Blue_LightToVeryLight = new LinearGradientBrush(Background_Color_Blue_Light, Background_Color_Blue_VeryLight, 90);
        public static readonly Brush Background_Brush_Blue_LightToDark = new LinearGradientBrush(Background_Color_Blue_Light, Background_Color_Blue_Dark, 90);
        public static readonly Brush Background_Brush_Blue_DarkToLight = new LinearGradientBrush(Background_Color_Blue_Dark, Background_Color_Blue_Light, 90);
        public static readonly Brush Background_Brush_Blue_VeryDarkToDark = new LinearGradientBrush(Background_Color_Blue_VeryDark, Background_Color_Blue_Dark, 90);
        public static readonly Brush Background_Brush_Blue_VeryVeryDarkToVeryDark = new LinearGradientBrush(Background_Color_Blue_VeryVeryDark, Background_Color_Blue_VeryDark, 90);
        public static readonly Brush Background_Brush_Blue_LightToWhite = new LinearGradientBrush(Background_Color_Blue_Light, Colors.White, 90);
        public static readonly Brush Background_Brush_Blue_DarkToWhite = new LinearGradientBrush(Background_Color_Blue_Dark, Colors.White, 90);
        public static readonly Brush Background_Brush_Blue_VeryDarkToWhite = new LinearGradientBrush(Background_Color_Blue_VeryDark, Colors.White, 90);
        public static readonly Brush Background_Brush_Blue_VeryVeryDarkToWhite = new LinearGradientBrush(Background_Color_Blue_VeryVeryDark, Colors.White, 90);

        public static readonly Brush Background_Brush_Warning = new SolidColorBrush(Colors.Salmon);
        public static readonly Brush Background_Brush_Warning_Transparent = new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Salmon, 128));

        static ThemeColours()
        {
            Background_Brush_Blue_Light.Freeze();
            Background_Brush_Blue_Dark.Freeze();
            Background_Brush_Blue_VeryDark.Freeze();
            Background_Brush_Blue_VeryVeryDark.Freeze();
            Background_Brush_Blue_VeryVeryVeryDark.Freeze();

            Background_Brush_Blue_LightToVeryLight.Freeze();
            Background_Brush_Blue_LightToDark.Freeze();
            Background_Brush_Blue_DarkToLight.Freeze();
            Background_Brush_Blue_VeryDarkToDark.Freeze();
            Background_Brush_Blue_VeryVeryDarkToVeryDark.Freeze();

            Background_Brush_Blue_LightToWhite.Freeze();
            Background_Brush_Blue_DarkToWhite.Freeze();
            Background_Brush_Blue_VeryDarkToWhite.Freeze();
            Background_Brush_Blue_VeryVeryDarkToWhite.Freeze();

            Background_Brush_Warning.Freeze();
            Background_Brush_Warning_Transparent.Freeze();
        }

        public static void AddToApplicationResources(Application application)
        {
            if (!application.Resources.Contains("Background_Color_Neutral_VeryLight")) application.Resources.Add("Background_Color_Neutral_VeryLight", Background_Color_Neutral_VeryLight);
            if (!application.Resources.Contains("Background_Color_Neutral_Light")) application.Resources.Add("Background_Color_Neutral_Light", Background_Color_Neutral_Light);
            if (!application.Resources.Contains("Background_Color_Neutral_Medium")) application.Resources.Add("Background_Color_Neutral_Medium", Background_Color_Neutral_Medium);

            if (!application.Resources.Contains("Background_Color_Blue_VeryLight")) application.Resources.Add("Background_Color_Blue_VeryLight", Background_Color_Blue_VeryLight);
            if (!application.Resources.Contains("Background_Color_Blue_Light")) application.Resources.Add("Background_Color_Blue_Light", Background_Color_Blue_Light);
            if (!application.Resources.Contains("Background_Color_Blue_Dark")) application.Resources.Add("Background_Color_Blue_Dark", Background_Color_Blue_Dark);
            if (!application.Resources.Contains("Background_Color_Blue_VeryDark")) application.Resources.Add("Background_Color_Blue_VeryDark", Background_Color_Blue_VeryDark);
            if (!application.Resources.Contains("Background_Color_Blue_VeryVeryDark")) application.Resources.Add("Background_Color_Blue_VeryVeryDark", Background_Color_Blue_VeryVeryDark);
            if (!application.Resources.Contains("Background_Color_Blue_VeryVeryVeryDark")) application.Resources.Add("Background_Color_Blue_VeryVeryVeryDark", Background_Color_Blue_VeryVeryVeryDark);

            if (!application.Resources.Contains("Background_Color_Warning_Light")) application.Resources.Add("Background_Color_Warning_Light", Background_Color_Warning_Light);
            if (!application.Resources.Contains("Background_Color_Warning_Dark")) application.Resources.Add("Background_Color_Warning_Dark", Background_Color_Warning_Dark);

            if (!application.Resources.Contains("Background_Brush_Blue_Light")) application.Resources.Add("Background_Brush_Blue_Light", Background_Brush_Blue_Light);
            if (!application.Resources.Contains("Background_Brush_Blue_Dark")) application.Resources.Add("Background_Brush_Blue_Dark", Background_Brush_Blue_Dark);
            if (!application.Resources.Contains("Background_Brush_Blue_VeryDark")) application.Resources.Add("Background_Brush_Blue_VeryDark", Background_Brush_Blue_VeryDark);
            if (!application.Resources.Contains("Background_Brush_Blue_VeryVeryDark")) application.Resources.Add("Background_Brush_Blue_VeryVeryDark", Background_Brush_Blue_VeryVeryDark);
            if (!application.Resources.Contains("Background_Brush_Blue_VeryVeryVeryDark")) application.Resources.Add("Background_Brush_Blue_VeryVeryVeryDark", Background_Brush_Blue_VeryVeryVeryDark);

            if (!application.Resources.Contains("Background_Brush_Blue_LightToVeryLight")) application.Resources.Add("Background_Brush_Blue_LightToVeryLight", Background_Brush_Blue_LightToVeryLight);
            if (!application.Resources.Contains("Background_Brush_Blue_LightToDark")) application.Resources.Add("Background_Brush_Blue_LightToDark", Background_Brush_Blue_LightToDark);
            if (!application.Resources.Contains("Background_Brush_Blue_DarkToLight")) application.Resources.Add("Background_Brush_Blue_DarkToLight", Background_Brush_Blue_DarkToLight);
            if (!application.Resources.Contains("Background_Brush_Blue_VeryDarkToDark")) application.Resources.Add("Background_Brush_Blue_VeryDarkToDark", Background_Brush_Blue_VeryDarkToDark);
            if (!application.Resources.Contains("Background_Brush_Blue_VeryVeryDarkToVeryDark")) application.Resources.Add("Background_Brush_Blue_VeryVeryDarkToVeryDark", Background_Brush_Blue_VeryVeryDarkToVeryDark);
            if (!application.Resources.Contains("Background_Brush_Blue_LightToWhite")) application.Resources.Add("Background_Brush_Blue_LightToWhite", Background_Brush_Blue_LightToWhite);
            if (!application.Resources.Contains("Background_Brush_Blue_DarkToWhite")) application.Resources.Add("Background_Brush_Blue_DarkToWhite", Background_Brush_Blue_DarkToWhite);
            if (!application.Resources.Contains("Background_Brush_Blue_VeryDarkToWhite")) application.Resources.Add("Background_Brush_Blue_VeryDarkToWhite", Background_Brush_Blue_VeryDarkToWhite);
            if (!application.Resources.Contains("Background_Brush_Blue_VeryVeryDarkToWhite")) application.Resources.Add("Background_Brush_Blue_VeryVeryDarkToWhite", Background_Brush_Blue_VeryVeryDarkToWhite);

            if (!application.Resources.Contains("Background_Brush_Warning")) application.Resources.Add("Background_Brush_Warning", Background_Brush_Warning);
            if (!application.Resources.Contains("Background_Brush_Warning_Transparent")) application.Resources.Add("Background_Brush_Warning_Transparent", Background_Brush_Warning_Transparent);

            // And then the associated XAML file
            string MODULE_NAME = typeof(ThemeScrollbar).Assembly.GetName().Name;
            string resource_location = string.Format("/{0};component/GUI/ThemeColours.xaml", MODULE_NAME);
            ResourceDictionary rd = Application.LoadComponent(new Uri(resource_location, UriKind.Relative)) as ResourceDictionary;
            application.Resources.MergedDictionaries.Add(rd);
        }
    }
}
