using System;
using Newtonsoft.Json;
using ProtoBuf;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF
{
    [ProtoContract]
    public class PDFHighlight : ICloneable
    {
        [ProtoMember(1)]
        [JsonProperty("P")]
        public int Page;

        [ProtoMember(2)]
        [JsonProperty("L")]
        public double Left;

        [ProtoMember(3)]
        [JsonProperty("T")]
        public double Top;

        [ProtoMember(4)]
        [JsonProperty("W")]
        public double Width;

        [ProtoMember(5)]
        [JsonProperty("H")]
        public double Height;

        [ProtoMember(6)]
        [JsonProperty("C")]
        public int Color;

        public PDFHighlight()
        {
        }

        public PDFHighlight(int page, Word word, int colourNumber)
        {
            Page = page;

            Left = word.Left;
            Top = word.Top;
            Width = word.Width;
            Height = word.Height;

            Color = colourNumber;
        }

        [JsonIgnore]
        public double Right => Left + Width;

        [JsonIgnore]
        public double Bottom => Top + Height;

        public bool Contains(int page, double left, double top)
        {
            return (Page == page && left >= Left && left <= Right && top >= Top && top <= Bottom);
        }

        public override bool Equals(object obj)
        {
            PDFHighlight other = obj as PDFHighlight;
            if (null == other) return false;

            return true
                && Page == other.Page
                && Left == other.Left
                && Top == other.Top
                && Width == other.Width
                && Height == other.Height
                ;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + Page.GetHashCode();
            hash = hash * 37 + Left.GetHashCode();
            hash = hash * 37 + Top.GetHashCode();
            hash = hash * 37 + Width.GetHashCode();
            hash = hash * 37 + Height.GetHashCode();
            return hash;
        }

        /// <summary>
        /// No need to deep clone this.
        /// </summary>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
