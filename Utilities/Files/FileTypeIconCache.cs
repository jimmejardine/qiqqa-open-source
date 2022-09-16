using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Utilities.Files
{
    public class FileTypeIconCache
    {
        public static FileTypeIconCache Instance = new FileTypeIconCache();
        private Dictionary<string, BitmapSource> icons = new Dictionary<string, BitmapSource>();

        private FileTypeIconCache()
        {
        }

        public BitmapSource this[string path]
        {
            get
            {
                string extension = Path.GetExtension(path).ToLower();

                BitmapSource result;
                if (!icons.TryGetValue(extension, out result) || IsUncacheableExtension(extension))
                {
                    Logging.Info("FileTypeIconCache has not seen a {0} before, so fetching from OS", extension);

                    using (Icon sysicon = Icon.ExtractAssociatedIcon(path))
                    {
                        result = Imaging.CreateBitmapSourceFromHIcon(sysicon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(64, 64));
                        icons[extension] = result;
                    }
                }

                return result;
            }
        }

        private static readonly string[] uncacheables = new string[]
            {
                ".exe", ".com", ".lnk"
            };

        private static bool IsUncacheableExtension(string extension)
        {

            foreach (string uncacheable in uncacheables)
            {
                if (0 == uncacheable.CompareTo(extension)) return true;
            }

            return false;
        }
    }
}
