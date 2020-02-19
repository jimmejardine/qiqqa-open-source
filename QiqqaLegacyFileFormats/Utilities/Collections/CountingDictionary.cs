using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QiqqaLegacyFileFormats          // namespace Utilities.Collections
{
    [Serializable]
    public class CountingDictionary<KEY> : Dictionary<KEY, int>
    {
        public CountingDictionary()
        {
        }

        protected CountingDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
