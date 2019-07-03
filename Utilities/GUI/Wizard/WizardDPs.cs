using System.Windows;

namespace Utilities.GUI.Wizard
{
    public class WizardDPs
    {
        // --------------------------------------------------------------------------------------------------------------------------------------------------------

        public static readonly DependencyProperty PointOfInterestProperty = DependencyProperty.RegisterAttached("PointOfInterest", typeof(string), typeof(WizardDPs), new PropertyMetadata(""));
        public static string GetPointOfInterest(DependencyObject d)
        {
            return (string)d.GetValue(PointOfInterestProperty);
        }
        public static void SetPointOfInterest(DependencyObject d, string value)
        {
            d.SetValue(PointOfInterestProperty, value);
        }
        public static void ClearPointOfInterest(DependencyObject d)
        {
            d.ClearValue(PointOfInterestProperty);            
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}
