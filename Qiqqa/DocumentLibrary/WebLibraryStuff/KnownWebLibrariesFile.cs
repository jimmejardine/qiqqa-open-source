using System.Collections.Generic;
using System.Reflection;
using ProtoBuf;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    [ProtoContract]
    [Obfuscation(Feature = "properties renaming")]
    class KnownWebLibrariesFile
    {
        [ProtoMember(1)]
        internal List<WebLibraryDetail> web_library_details;
    }
}
