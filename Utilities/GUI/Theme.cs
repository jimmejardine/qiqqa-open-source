using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Utilities.GUI
{
    public class Theme
    {
        private static int theme_init_count = 0;

        public static void Initialize(Application application = null)
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
