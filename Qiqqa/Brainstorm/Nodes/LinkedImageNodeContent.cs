using System;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using Qiqqa.Brainstorm.Common.Searching;
using Utilities.Images;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class LinkedImageNodeContent : ISearchable
    {
        private string image_path;
        [NonSerialized]
        private BitmapSource bitmap_source = null;

        public LinkedImageNodeContent(string image_path)
        {
            this.image_path = image_path;
        }

        public string ImagePath
        {
            get => image_path;
            set
            {
                image_path = value;
                bitmap_source = null;
            }
        }

        public BitmapSource BitmapSource
        {
            get
            {
                if (null == bitmap_source)
                {
                    bitmap_source = BitmapImageTools.LoadFromFile(image_path);
                }
                return bitmap_source;
            }
        }

        public bool MatchesKeyword(string keyword)
        {
            return (null != image_path) && image_path.ToLower(CultureInfo.CurrentCulture).Contains(keyword);
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
