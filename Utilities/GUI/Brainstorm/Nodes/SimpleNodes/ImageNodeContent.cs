using System;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using Utilities.GUI.Brainstorm.Common.Searching;
using Utilities.Images;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{
    [Serializable]
    public class ImageNodeContent : Searchable
    {
        byte[] image_data;

        [NonSerialized]
        BitmapSource bitmap_source = null;

        public ImageNodeContent(string image_path)
        {
            ImagePath = image_path;
        }

        public ImageNodeContent(MemoryStream ms)
        {
            this.image_data = ms.ToArray();
            bitmap_source = null;
        }


        public string ImagePath
        {
            set
            {
                this.image_data = File.ReadAllBytes(value);
                bitmap_source = null;
            }
        }

        public BitmapSource BitmapSource
        {
            get
            {
                if (null == bitmap_source)
                {
                    bitmap_source = BitmapImageTools.LoadFromBytes(image_data);
                }
                return bitmap_source;
            }
        }

        public bool MatchesKeyword(string keyword)
        {
            return false;
        }

        internal static bool IsSupportedImagePath(string filename)
        {
            string extension = Path.GetExtension(filename.ToLower(CultureInfo.CurrentCulture));

            if (0 == extension.CompareTo(".jpg")) return true;
            if (0 == extension.CompareTo(".png")) return true;
            if (0 == extension.CompareTo(".jpeg")) return true;
            if (0 == extension.CompareTo(".gif")) return true;
            if (0 == extension.CompareTo(".bmp")) return true;

            return false;
        }
    }
}
