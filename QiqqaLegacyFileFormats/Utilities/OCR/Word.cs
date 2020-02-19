using System;

namespace QiqqaLegacyFileFormats          // namespace Utilities.OCR
{
    [Serializable]
    public class Word
    {
        public string Text;
        public double Confidence;
        public double Left;
        public double Top;
        public double Width;
        public double Height;

        public override string ToString()
        {
            return String.Format("{0} - {1} ({2},{3} x {4},{5})",
                Text,
                Confidence,
                Left,
                Top,
                Width,
                Height);
        }

        public double Right => Left + Width;

        public double Bottom => Top + Height;

        public bool Contains(double left, double top)
        {
            return (left >= Left && left <= Right && top >= Top && top <= Bottom);
        }

        // Determines whether the supplied box is inside this word bounding box
        public bool Contains(double left, double top, double width, double height)
        {
            double right = left + width;
            double bottom = top + height;
            return
                true
                && Right >= left
                && Bottom >= top
                && Left <= right
                && Top <= bottom
                ;
        }

        // Determines whether most of the supplied box is inside this word bounding box
        public bool ContainsMajority(double left, double top, double width, double height)
        {
            return
                true
                && Right >= left + width / 2
                && Bottom >= top + height / 2
                && Left <= left + width / 2
                && Top <= top + height / 2
                ;
        }


        // Determines whether this word bounding box is inside the supplied box
        public bool IsContained(double left, double top, double width, double height)
        {
            double right = left + width;
            double bottom = top + height;

            return
                true
                && right >= Right
                && bottom >= Bottom
                && left <= Left
                && top <= Top
                ;
        }
    }
}
