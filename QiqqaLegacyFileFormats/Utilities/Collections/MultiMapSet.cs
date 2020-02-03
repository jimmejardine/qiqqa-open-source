#if !HAS_NO_PROTOBUF

using System;
using System.Collections.Generic;
using ProtoBuf;

namespace QiqqaLegacyFileFormats          // namespace Utilities.Collections
{
    /// <summary>
    /// Provides a multiple map, where a given key points to several items.  Note that the items will be unique.
    /// </summary>
    /// <typeparam name="KEY"></typeparam>
    /// <typeparam name="VALUE"></typeparam>
    [Serializable]
    [ProtoContract]
    public class MultiMapSet<KEY, VALUE>
    {
        private static HashSet<VALUE> EMPTY_VALUE_SET = new HashSet<VALUE>();

        [ProtoMember(1)]
        private Dictionary<KEY, HashSet<VALUE>> data = new Dictionary<KEY, HashSet<VALUE>>();

        public MultiMapSet()
        {
        }
    }
}

#endif
