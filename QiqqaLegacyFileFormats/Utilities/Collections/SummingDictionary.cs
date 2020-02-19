using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QiqqaLegacyFileFormats          // namespace Utilities.Collections
{
    [Serializable]
    public class SummingDictionary<KEY> : Dictionary<KEY, double>
    {
        public SummingDictionary()
        {
        }

        protected SummingDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
