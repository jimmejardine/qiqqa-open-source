using System.Collections.Generic;
using System.Windows;

namespace Utilities.GUI.Wizard
{
    public class WizardTools
    {
        public static bool AreAnyPOIsVisible(PointOfInterestLocator point_of_interest_locator, params string[] pois)
        {
            foreach (string poi in pois)
            {
                List<DependencyObject> poi_list = point_of_interest_locator.GetPOI(poi);
                if (null != poi_list)
                {
                    foreach (DependencyObject dep_obj in poi_list)
                    {
                        FrameworkElement fe = dep_obj as FrameworkElement;
                        if (null != fe)
                        {
                            if (fe.Visibility == Visibility.Visible)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
