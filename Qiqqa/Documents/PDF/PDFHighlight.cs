using System;
using System.Reflection;
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
            this.Page = page;

            this.Left = word.Left;
            this.Top = word.Top;
            this.Width = word.Width;
            this.Height = word.Height;

            this.Color = colourNumber;
        }

        [JsonIgnore]
        public double Right
        {
            get
            {
                return Left + Width;
            }
        }

        [JsonIgnore]
        public double Bottom
        {
            get
            {
                return Top + Height;
            }
        }

        public bool Contains(int page, double left, double top)
        {
            return (this.Page == page && left >= Left && left <= Right && top >= Top && top <= Bottom);
        }

        public override bool Equals(object obj)
        {
            PDFHighlight other = obj as PDFHighlight;
            if (null == other) return false;

            return true
                && this.Page == other.Page
                && this.Left == other.Left
                && this.Top == other.Top
                && this.Width == other.Width
                && this.Height == other.Height
                ;
        }

        public override int GetHashCode()
        {            
            int hash = 23;
            hash = hash * 37 + this.Page.GetHashCode();
            hash = hash * 37 + this.Left.GetHashCode();
            hash = hash * 37 + this.Top.GetHashCode();
            hash = hash * 37 + this.Width.GetHashCode();
            hash = hash * 37 + this.Height.GetHashCode();
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
