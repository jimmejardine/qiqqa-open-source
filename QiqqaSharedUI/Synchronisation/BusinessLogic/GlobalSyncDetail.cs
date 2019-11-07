using System;
using System.Collections.Generic;

namespace Qiqqa.Synchronisation.BusinessLogic
{
    [Serializable]
    internal class GlobalSyncDetail
    {
        public List<LibrarySyncDetail> library_sync_details = new List<LibrarySyncDetail>();
    }
}
