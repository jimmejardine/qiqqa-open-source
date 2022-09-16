using System.IO;
using System.Windows.Media.Imaging;

namespace Utilities.Images
{
    public class ImageSaver
    {
        public static void SaveAsPng(string filename, BitmapSource source)
        {
            if (null == source) return;

            PngBitmapEncoder p = new PngBitmapEncoder();
            p.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = File.OpenWrite(filename))
            {
                p.Save(fs);
            }
        }
    }
}
