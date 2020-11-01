using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;

namespace Qiqqa.Synchronisation.MetadataSync
{
    internal class SynchronisationExecutor_Intranet
    {
        internal static void DoUpload(WebLibraryDetail web_library_detail, SynchronisationState ss)
        {
            IntranetLibraryDB db = new IntranetLibraryDB(web_library_detail.IntranetPath);
            db.PutBlob(ss.filename, ss.library_item.data);
        }

        internal static StoredUserFile DoDownload(WebLibraryDetail web_library_detail, SynchronisationState ss)
        {
            IntranetLibraryDB db = new IntranetLibraryDB(web_library_detail.IntranetPath);
            IntranetLibraryDB.IntranetLibraryItem item = db.GetIntranetLibraryItem(ss.filename);

            // TODO: Change this to use the not WEB SERVICE class, but rather a dedicated response class
            StoredUserFile suf = new StoredUserFile();
            suf.Key = item.filename;
            suf.Md5 = item.md5;
            suf.Content = item.data;
            return suf;
        }
    }
}
