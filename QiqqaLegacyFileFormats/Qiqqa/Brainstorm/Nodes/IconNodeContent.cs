using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
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
#if SAMPLE_LOAD_CODE
                if (null == bitmap_image)
                {
                    bitmap_image = Icons.GetByPath(icon_path);
                }
#endif
                return bitmap_image;
            }
        }

        public bool MatchesKeyword(string keyword)
        {
            return (null != icon_path) && icon_path.ToLower().Contains(keyword);
        }
    }
}
