using System.Windows;

namespace Utilities.GUI
{
    public class Theme
    {
        private static int theme_init_count = 0;

        /// <summary>
        /// This baby has been introduced as a stopgap for preventing Microsoft Visual Studio Designer View crashes/failures
        /// to render a given XAML-based control.
        /// 
        /// Invoke this API as part of every control's initialization and you should be good to go in MSVS's Designer View.
        /// </summary>
        public static void Initialize()
        {
            // NB NB NB NB: You CANT USE ANYTHING IN THE USER CONFIG AT THIS POINT - it is not yet decided until LOGIN has completed...

            ++theme_init_count;

            if (theme_init_count == 1)
            {
                Application app = Application.Current;

                ThemeColours.AddToApplicationResources(app);
                ThemeTextStyles.AddToApplicationResources(app);
                ThemeScrollbar.AddToApplicationResources(app);
                ThemeTabItem.AddToApplicationResources(app);
            }

            // NB NB NB NB: You CANT USE ANYTHING IN THE USER CONFIG AT THIS POINT - it is not yet decided until LOGIN has completed...
        }
    }
}
