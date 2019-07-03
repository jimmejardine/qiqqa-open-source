using System.Drawing;
using System.IO;

namespace Utilities.Images
{
    public class ImageLoader
    {
        public static Image Load(string filename)
        {
            using (FileStream fs = File.OpenRead(filename))
            {
                // .NET requires the stream to still be open if you want to save the loaded image
                // A hack is to make a copy of the image...
                Image result = new Bitmap(Image.FromStream(fs));
                return result;
            }
        }
    }
}
