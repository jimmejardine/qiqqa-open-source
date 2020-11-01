using Qiqqa.DocumentLibrary.WebLibraryStuff;

namespace Qiqqa.Synchronisation
{
    internal class StatusCodes
    {
        internal static readonly string SYNC_META_GLOBAL = "SYNC_META";

        internal static string SYNC_META(WebLibraryDetail web_library_detail)
        {
            return "SYNC_META:" + web_library_detail.Id;
        }

        internal static readonly string SYNC_DATA = "SYNC_DATA";
        internal static readonly string SYNC_DATA_UPLOAD = "SYNC_DATA_UPLOAD";
        internal static readonly string SYNC_DATA_DOWNLOAD = "SYNC_DATA_DOWNLOAD";
    }
}
