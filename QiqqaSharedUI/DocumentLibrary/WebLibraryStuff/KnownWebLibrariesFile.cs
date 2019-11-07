using System.Collections.Generic;
using ProtoBuf;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    [ProtoContract]
    internal class KnownWebLibrariesFile
    {
        [ProtoMember(1)]
        internal List<WebLibraryDetail> web_library_details;
    }
}
