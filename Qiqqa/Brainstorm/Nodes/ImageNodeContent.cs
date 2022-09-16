using System;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using Qiqqa.Brainstorm.Common.Searching;
using Utilities.Images;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class ImageNodeContent : ISearchable
    {
        private byte[] image_data;

        [NonSerialized]
        private BitmapSource bitmap_source = null;

        public ImageNodeContent(string image_path)
        {
            ImageNodeContentFromPath(image_path);
        }

        public ImageNodeContent(MemoryStream ms)
        {
            image_data = ms.ToArray();
            bitmap_source = null;
        }


        public void ImageNodeContentFromPath(string path)
        {
            image_data = File.ReadAllBytes(path);
            bitmap_source = null;
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
            string extension = Path.GetExtension(filename.ToLower());

            if (0 == extension.CompareTo(".jpg")) return true;
            if (0 == extension.CompareTo(".png")) return true;
            if (0 == extension.CompareTo(".jpeg")) return true;
            if (0 == extension.CompareTo(".gif")) return true;
            if (0 == extension.CompareTo(".bmp")) return true;

            return false;
        }
    }
}
