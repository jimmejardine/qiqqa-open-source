using System.Windows;
using System.Windows.Media;
using Utilities.GUI;

namespace Qiqqa.Common.GUI
{
    /// <summary>
    /// Standard header control for any pop up pages that need a title and subtitle, with image to prettify it up. 
    /// </summary>
    public partial class StandardPageHeader
    {
        public StandardPageHeader()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(0, 0, 0, 1);
            this.BorderBrush = new SolidColorBrush(ThemeColours.Background_Color_Neutral_Light);

            RenderOptions.SetBitmapScalingMode(HeaderImage, BitmapScalingMode.HighQuality);
        }

        public ImageSource Img
        {
            get { return HeaderImage.Source; }
            set
            {
                HeaderImage.Source = value;
            }
        }

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(StandardPageHeader));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty SubCaptionProperty = DependencyProperty.Register("SubCaption", typeof(string), typeof(StandardPageHeader));
        public string SubCaption
        {
            get { return (string)GetValue(SubCaptionProperty); }
            set { SetValue(SubCaptionProperty, value); }
        }
    }
}
