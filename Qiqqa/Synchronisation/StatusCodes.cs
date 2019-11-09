using Qiqqa.DocumentLibrary;

namespace Qiqqa.Synchronisation
{
    internal class StatusCodes
    {
        internal static readonly string SYNC_META_GLOBAL = "SYNC_META";

        internal static string SYNC_META(Library library)
        {
            return "SYNC_META:" + library.WebLibraryDetail.Id;
        }

        internal static readonly string SYNC_DATA = "SYNC_DATA";
        internal static readonly string SYNC_DATA_UPLOAD = "SYNC_DATA_UPLOAD";
        internal static readonly string SYNC_DATA_DOWNLOAD = "SYNC_DATA_DOWNLOAD";
    }
}
