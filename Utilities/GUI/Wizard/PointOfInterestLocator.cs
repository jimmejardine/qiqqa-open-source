using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Utilities.GUI.Wizard
{
    public class PointOfInterestLocator
    {
        Dictionary<string, List<DependencyObject>> points_of_interest;

        public PointOfInterestLocator()
        {
            this.points_of_interest = new Dictionary<string, List<DependencyObject>>();

            Refresh();
        }

        public void Refresh()
        {
            this.points_of_interest.Clear();

            foreach (Window window in Application.Current.Windows)
            {
                Refresh(window, this.points_of_interest);
            }
        }

        private static void Refresh(DependencyObject root, Dictionary<string, List<DependencyObject>> points_of_interest)
        {
            if (null != root)
            {
                /*
                if (KeyboardTools.IsCTRLDown() && KeyboardTools.IsShiftDown() && KeyboardTools.IsALTDown())
                {
                    string type = root.GetType().ToString();
                    string interesting = "";
                    if (type.Contains("LibraryCatalogOverviewControl")) interesting = "***";
                    Logging.Info("{0} {1} {2}", interesting, indent, type);
                }
                */

                string poi = WizardDPs.GetPointOfInterest(root);
                if (!String.IsNullOrEmpty(poi))
                {
                    if (!points_of_interest.ContainsKey(poi))
                    {
                        points_of_interest[poi] = new List<DependencyObject>();
                    }
                    points_of_interest[poi].Add(root);
                }

                int child_count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < child_count; ++i)
                {
                    DependencyObject dep_obj = VisualTreeHelper.GetChild(root, i);
                    if (null != dep_obj)
                    {
                        Refresh(dep_obj, points_of_interest);
                    }
                }
            }
        }

        public List<DependencyObject> GetPOI(string poi)
        {
            List<DependencyObject> poi_list = null;
            points_of_interest.TryGetValue(poi, out poi_list);
            return poi_list;
        }

        public DependencyObject GetFirstPOI(string poi)
        {
            List<DependencyObject> poi_list = GetPOI(poi);
            if (null == poi_list) return null;
            return poi_list.FirstOrDefault();
        }
    }
}
