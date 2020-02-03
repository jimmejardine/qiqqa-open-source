using System.Collections.Generic;
using System.Reflection;
using ProtoBuf;

namespace QiqqaLegacyFileFormats
{
    [ProtoContract]
    class KnownWebLibrariesFile
    {
        [ProtoMember(1)]
        internal List<WebLibraryDetail> web_library_details;
    }
}
