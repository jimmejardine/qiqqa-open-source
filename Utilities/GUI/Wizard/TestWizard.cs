using System.Windows;
using System.Windows.Controls;

namespace Utilities.GUI.Wizard
{
    internal class TestWizard
    {
        public static Route GetRoute()
        {
            return new Route(
                "First steps...",
                new Step[]
                {
                    new Step
                    {
                        Instructions = "Welcome to the Wizard.  Press Next to get going...",
                        PointOfInterests = new string[] {  },
                        PostCondition = (point_of_interest_locator) => { return true; }
                    },

                    new Step
                    {
                        Instructions = "The first thing you need to do is make the text disappear by clicking the radio button...",
                        PointOfInterests = new string[] { "TB","Wizard:1:CheckBox" },
                        PostCondition = (point_of_interest_locator) =>
                        {
                            TextBlock tb = point_of_interest_locator.GetFirstPOI("TB") as TextBlock;
                            if (null == tb) return false;
                            return (tb.Visibility == Visibility.Collapsed);
                        }
                    },

                    new Step
                    {
                        Instructions = "Now you need to open a window by pressing the button...",
                        PointOfInterests = new string[] { "Wizard:1:Button" },
                        PostCondition = (point_of_interest_locator) =>
                        {
                            Window window = point_of_interest_locator.GetFirstPOI("Wizard:1:TargetWindow") as Window;
                            if (null == window) return false;
                            return true;
                        }
                    },
                },
                "Congratulations.  You are now up and running! With the longest message in the longest message in the longest message in the longest message in the longest message in the longest message in the longest message in the longest message in the longest message in the world."
            );
        }
    }
}
