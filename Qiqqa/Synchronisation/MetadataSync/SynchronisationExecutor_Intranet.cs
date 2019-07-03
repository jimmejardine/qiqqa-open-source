using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;

namespace Qiqqa.Synchronisation.MetadataSync
{
    class SynchronisationExecutor_Intranet
    {
        internal static void DoUpload(Library library, SynchronisationState ss)
        {
            IntranetLibraryDB db = new IntranetLibraryDB(library.WebLibraryDetail.IntranetPath);
            db.PutBlob(ss.filename, ss.library_item.data);
        }

        internal static StoredUserFile DoDownload(Library library, SynchronisationState ss)
        {
            IntranetLibraryDB db = new IntranetLibraryDB(library.WebLibraryDetail.IntranetPath);
            IntranetLibraryDB.IntranetLibraryItem item = db.GetIntranetLibraryItem(ss.filename);

            // *** TODO: Change this to use the not WEB SERVICE class, but rather a dedicated response class
            StoredUserFile suf = new StoredUserFile();
            suf.Key = item.filename;
            suf.Md5 = item.md5;
            suf.Content = item.data;
            return suf;
        }
    }
}
