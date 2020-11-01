using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;

namespace Qiqqa.Synchronisation.MetadataSync
{
    internal class SynchronisationStateBuilder_Intranet
    {
        internal static void BuildFromRemote(WebLibraryDetail web_library_detail, ref SynchronisationStates synchronisation_states)
        {
            IntranetLibraryDB db = new IntranetLibraryDB(web_library_detail.IntranetPath);
            List<IntranetLibraryDB.IntranetLibraryItem> items = db.GetIntranetLibraryItemsSummary();
            foreach (var item in items)
            {
                synchronisation_states[item.filename].md5_remote = item.md5;
            }
        }
    }
}
