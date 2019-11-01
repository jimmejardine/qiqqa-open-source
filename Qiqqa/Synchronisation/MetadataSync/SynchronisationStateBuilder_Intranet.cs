using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;

namespace Qiqqa.Synchronisation.MetadataSync
{
    internal class SynchronisationStateBuilder_Intranet
    {
        internal static void BuildFromRemote(Library library, ref SynchronisationStates synchronisation_states)
        {
            IntranetLibraryDB db = new IntranetLibraryDB(library.WebLibraryDetail.IntranetPath);
            List<IntranetLibraryDB.IntranetLibraryItem> items = db.GetIntranetLibraryItemsSummary();
            foreach (var item in items)
            {
                synchronisation_states[item.filename].md5_remote = item.md5;
            }
        }
    }
}
