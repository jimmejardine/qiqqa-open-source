using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace icons
{
    public static class Backgrounds
    {
        private static readonly string module_name = typeof(Icons).Assembly.GetName().Name;

        public static readonly string ExceptionDialogBackground = "ExceptionDialogBackground.jpg";

        public static readonly string PageRenderingDisabled = "green-dice-a4.jpg";
        public static readonly string PageRenderingFailed_Relax = "green-dice-for-fail.jpg";
        public static readonly string PageRenderingFailed_ClassicNews = "qiqqa-page-render-fail-news-page.jpg";
        public static readonly string PageRenderingFailed_Poster = "qiqqa-page-render-fail-poster.jpg";
        public static readonly string PageRenderingPending_1 = "golden-toad-for-pending-1.jpg";
        public static readonly string PageRenderingPending_2 = "flask-for-pending-2.jpg";

        public static BitmapImage GetBackground(string file)
        {
            string resource_location = string.Format("pack://application:,,,/{0};component/Backgrounds/{1}", module_name, file);
            return new BitmapImage(new Uri(resource_location));
        }

        public static byte[] GetBackgroundAsByteArray(string file)
        {
            string resource_location = string.Format("pack://application:,,,/{0};component/Backgrounds/{1}", module_name, file);
            var info = Application.GetResourceStream(new Uri(resource_location));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                info.Stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
