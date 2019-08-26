using System;
using System.Windows;

namespace Utilities.GUI
{
    public class AugmentedSplashScreen : SplashScreen
    {
        public AugmentedSplashScreen(string filename) :
            base(filename)
        {
        }

        public void Close()
        {
            // A bug in .NET3.5 causes the splashscreen not to close if the app doesnt have focus
            for (int i = 0; i < 5; ++i)
            {
                try
                {
                    Logging.Info("Closing splash screen");
                    Close(TimeSpan.FromSeconds(0));
                    Logging.Info("Closed splash screen");
                    break;
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Exception closing splash screen");

                    try
                    {
                        Application.Current.MainWindow.Focus();
                    }
                    catch (Exception ex2)
                    {
                        Logging.Error(ex2);
                    }
                }
            }
        }
    }
}
