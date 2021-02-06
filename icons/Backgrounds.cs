using System;
using System.Windows.Media.Imaging;

namespace icons
{
    public static class Backgrounds
    {
        private static readonly string module_name = typeof(Icons).Assembly.GetName().Name;

        public static readonly string ExceptionDialogBackground = "ExceptionDialogBackground.jpg";

        public static readonly string PageRenderingDisabled = "green-dice-a4.jpg";

        public static BitmapImage GetBackground(string file)
        {
            string resource_location = string.Format("pack://application:,,,/{0};component/Backgrounds/{1}", module_name, file);
            return new BitmapImage(new Uri(resource_location));
        }
    }
}
