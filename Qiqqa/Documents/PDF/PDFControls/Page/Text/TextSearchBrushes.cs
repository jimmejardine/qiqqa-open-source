using System.Collections.Generic;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Text
{
    public class TextSearchBrushes
    {
        public static TextSearchBrushes Instance = new TextSearchBrushes();

        List<TransparentBoxBrushPair> brushes = new List<TransparentBoxBrushPair>();

        TextSearchBrushes()
        {
            for (int i = 0; i < 7; ++i)
            {
                brushes.Add(new TransparentBoxBrushPair(ColorTools.GetRainbowColour(i)));
            }
        }

        public TransparentBoxBrushPair GetBrushPair(int i)
        {
            return brushes[i % brushes.Count];
        }
    }
}
