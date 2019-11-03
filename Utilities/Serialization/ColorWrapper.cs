using System;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Utilities.Serialization
{
    [Serializable]
    public class ColorWrapper
    {
        [JsonProperty("A")]
        private float a;
        [JsonProperty("R")]
        private float r;
        [JsonProperty("G")]
        private float g;
        [JsonProperty("B")]
        private float b;

        public ColorWrapper(Color color)
        {
            Set(color);
        }

        public void Set(Color color)
        {
            a = color.ScA;
            r = color.ScR;
            g = color.ScG;
            b = color.ScB;
        }

        [JsonIgnore]
        public Color Color => Color.FromScRgb(a, r, g, b);

        public override string ToString()
        {
            return Color.ToString();
        }
    }
}
