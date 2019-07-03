using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using icons;
using Utilities.GUI.Brainstorm.Common.Searching;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{
    [Serializable]
    public class IconNodeContent : Searchable
    {
        string icon_path;
        [NonSerialized]
        BitmapImage bitmap_image = null;

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
            return (null != icon_path) && icon_path.ToLower(CultureInfo.CurrentCulture).Contains(keyword);
        }
    }
}
