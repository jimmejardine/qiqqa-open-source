using System;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Utilities.Serialization
{
    [Serializable]
    public class ColorWrapper
    {
        [JsonProperty("A")]
        float a;
        [JsonProperty("R")]
        float r;
        [JsonProperty("G")]
        float g;
        [JsonProperty("B")]
        float b;

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
        public Color Color
        {
            get
            {
                return Color.FromScRgb(a, r, g, b);
            }
        }

        public override string ToString()
        {
            return Color.ToString();
        }
    }
}
