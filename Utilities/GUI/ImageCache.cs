using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Utilities.GUI
{
    public class ImageCache
    {
        public static ImageCache Instance = new ImageCache();

        Dictionary<string, BitmapImage> images = new Dictionary<string, BitmapImage>();
        
        private ImageCache()
        {
        }

        public BitmapImage this[string image_name]
        {
            get
            {
                image_name = image_name.ToLower(CultureInfo.CurrentCulture);

                BitmapImage image;

                // If it's already in our cache, return it
                if (images.TryGetValue(image_name, out image))
                {
                    return image;
                }

                // Otherwise load it
                Uri uri = new Uri("file://C:/personal/development/icons/papers/" + image_name);
                image = new BitmapImage(uri);
                images[image_name] = image;
                return image;
            }
        }
    }
}
