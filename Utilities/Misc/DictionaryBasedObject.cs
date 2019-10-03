using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities.Cloning;
using Utilities.DateTimeTools;
using Utilities.GUI;
using Utilities.Serialization;

namespace Utilities.Misc
{
    [Serializable]
    public class DictionaryBasedObject : ICloneable
    {
        protected Dictionary<string, object> attributes;

        public DictionaryBasedObject()
        {
            attributes = new Dictionary<string, object>();
        }
        
        public DictionaryBasedObject(Dictionary<string, object> attributes)
        {
            this.attributes = attributes;
        }

        public Dictionary<string, object> Attributes
        {
            get
            {
                return attributes;
            }
        }

        //NKS - need this for my deserialization server side. 
        public List<string> Keys
        {
            get
            {
                return attributes.Keys.ToList();
            }
        }

        public object this[string key]
        {
            get
            {
                object o;
                if (attributes.TryGetValue(key, out o))
                {
                    return o;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                attributes[key] = value;
            }
        }

        public DateTime? GetDateTime(string key)
        {
            object obj = this[key];

            string datetime_string = obj as string;
            if (null != datetime_string)
            {
                return DateFormatter.FromYYYYMMDDHHMMSSMMM(datetime_string);
            }

            DateTime? datetime = obj as DateTime?;
            if (null != datetime)
            {
                Logging.Info("Doing a legacy DateTime upgrade for key {0}", key);
                SetDateTime(key, datetime);
                return datetime;
            }
            
            return null;
        }

        public void SetDateTime(string key, DateTime? value)
        {
            this[key] = DateFormatter.ToYYYYMMDDHHMMSSMMM(value);
        }

        public Guid? GetNullableGuid(string key)
        {
            object o = this[key];

            if (null == o) return null;

            Guid? o_nullable_guid = o as Guid?;
            if (null != o_nullable_guid) return o_nullable_guid;

            string o_string = o as string;
            if (null != o_string)
            {
                return new Guid(o_string);
            }

            Logging.Warn("SHOULD THIS GUID BE NULL IN THE ANNOTATION JSON?");
            return null;            
        }

        public int? GetNullableInt32(string key)
        {
            object o = this[key];
            
            if (null == o) return null;

            int? o_nullable_int = o as int?;
            if (null != o_nullable_int) return o_nullable_int;

            return new int?(Convert.ToInt32(o));
        }

        public void SetColor(string key, Color color)
        {
            string color_string = ColorTools.ColorToHEX(color);
            this[key] = color_string;
        }

        public double GetDouble(string key)
        {
            // Is this fast enough if most of the time it is returning doubles anyway?
            // It has to cope with JSON containing 0 - which is returned as a boxed long and then the type conversion fails...
            return Convert.ToDouble(this[key] ?? 0.0);
        }

        public Color GetColor(string key)
        {
            object obj = this[key];
            
            string color_string = obj as string;
            if (null != color_string)
            {                
                return ColorTools.HEXToColor(color_string);
            }

            ColorWrapper color_wrapper = obj as ColorWrapper;
            if (null != color_wrapper)
            {
                Logging.Info("Doing a legacy Color upgrade for key " + key);
                SetColor(key, color_wrapper.Color);
                return color_wrapper.Color;
            }

            // Not a good place to be, but we will try our best :-)
            // This means that we somehow didn't write a correct color to the streamm...
            JContainer color_json = obj as JContainer;
            if (null != color_json)
            {
                ColorWrapper cw = JsonConvert.DeserializeObject<ColorWrapper>(color_json.ToString());
                return cw.Color;
            }  

            // If we get here there is no color!
            return Colors.Transparent;
        }

        /// <summary>
        /// Deep clone by serialising and deserialising.
        /// </summary>
        public object Clone()
        {
            return DeepObjectCloner.DeepClone(this);
        }
    }
}
