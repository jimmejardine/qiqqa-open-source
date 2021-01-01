using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using icons;
using Qiqqa.Brainstorm.Common.Searching;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class IconNodeContent : ISearchable
    {
        private string icon_path;
        [NonSerialized]
        private BitmapImage bitmap_image = null;

        public IconNodeContent(string icon_path)
        {
            this.icon_path = icon_path;
        }

        public BitmapImage BitmapImage
        {
            get
            {
                if (null == bitmap_image)
                {
                    bitmap_image = Icons.GetByPath(icon_path);
                }
                return bitmap_image;
            }
        }

        public bool MatchesKeyword(string keyword)
        {
            return (null != icon_path) && icon_path.ToLower().Contains(keyword);
        }
    }
}
