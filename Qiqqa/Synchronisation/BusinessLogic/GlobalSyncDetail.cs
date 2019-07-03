using System;
using System.Collections.Generic;

namespace Qiqqa.Synchronisation.BusinessLogic
{
    [Serializable]
    class GlobalSyncDetail
    {
        public List<LibrarySyncDetail> library_sync_details = new List<LibrarySyncDetail>();
    }
}
