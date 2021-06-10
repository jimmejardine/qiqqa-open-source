using System.IO;
using System.Windows.Media.Imaging;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


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
