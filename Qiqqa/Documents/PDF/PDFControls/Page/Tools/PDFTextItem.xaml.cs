using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Utilities;
using Utilities.GUI;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    /// <summary>
    /// Interaction logic for PDFTextItem.xaml
    /// </summary>
    public partial class PDFTextItem : Shape
    {
        static Color color_normal = Colors.LightGreen;
        static Color color_normal_transparent = ColorTools.MakeTransparentColor(color_normal, 64);
        static Brush brush_normal_border = new SolidColorBrush(color_normal);
        static Brush brush_normal_fill = new SolidColorBrush(color_normal_transparent);

        static Color color_highlighted = Colors.LightPink;
        static Color color_highlighted_transparent = Color.FromArgb(64, color_highlighted.R, color_highlighted.G, color_highlighted.B);
        static Brush brush_highlighted_border = new SolidColorBrush(color_highlighted);
        static Brush brush_highlighted_fill = new SolidColorBrush(color_highlighted_transparent);

        public Word word;
        public bool is_inside = false;

        RectangleGeometry geometry = new RectangleGeometry();

        public PDFTextItem()
            : this(null)
        {
        }

        public PDFTextItem(Word word)
        {
            this.word = word;

            InitializeComponent();

            this.IsHitTestVisible = true;

            MouseEnter += PDFTextItem_MouseEnter;
        }

        public Word Word
        {
            get
            {
                return word;
            }
            set
            {
                this.word = value;
            }
        }

        void PDFTextItem_MouseEnter(object sender, MouseEventArgs e)
        {
            Logging.Info(word.ToString());
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return geometry;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            geometry = new RectangleGeometry(rect);            
            return base.ArrangeOverride(finalSize);
        }

        internal void SetAppearance(TransparentBoxBrushPair transparent_box_brush_pair)
        {
            Stroke = transparent_box_brush_pair.BorderBrush;
            Fill = transparent_box_brush_pair.FillBrush;
        }
        
        internal void SetHighlightedAppearance(bool is_inside)
        {
            this.is_inside = is_inside;

            if (is_inside)
            {
                Stroke = brush_highlighted_border;
                Fill = brush_highlighted_fill;
            }
            else
            {
                Stroke = brush_normal_border;
                Fill = brush_normal_fill;
            }
        }
    }
}
