using System.Collections.Generic;
using Qiqqa.Synchronisation.BusinessLogic;

namespace Qiqqa.Synchronisation.GUI
{
    internal class SyncControlGridItemSet
    {
        internal LibrarySyncManager.SyncRequest sync_request;
        internal GlobalSyncDetail global_sync_detail;
        internal List<SyncControlGridItem> grid_items;

        public SyncControlGridItemSet(LibrarySyncManager.SyncRequest sync_request, GlobalSyncDetail global_sync_detail)
        {
            this.sync_request = sync_request;
            this.global_sync_detail = global_sync_detail;

            // Populate the grid datasource
            grid_items = new List<SyncControlGridItem>();
            foreach (var library_sync_detail in global_sync_detail.library_sync_details)
            {
                var grid_item = new SyncControlGridItem(library_sync_detail);

                grid_items.Add(grid_item);
            }
        }

        /// <summary>
        /// Goes through the libraries and automatically ticks those items that can be automatically synced
        /// </summary>
        public void AutoTick()
        {
            // Preset some of the values that the user might have wanted
            foreach (SyncControlGridItem sync_control_grid_item in grid_items)
            {
                bool tick_this_library = (0 == sync_request.libraries_to_sync.Count && !sync_control_grid_item.library_sync_detail.web_library_detail.IsReadOnlyLibrary) || sync_request.libraries_to_sync.Contains(sync_control_grid_item.library_sync_detail.web_library_detail);
                bool tick_this_documents = sync_control_grid_item.CanSyncLibrary;

                sync_control_grid_item.UserRequestedSync = tick_this_library;
                sync_control_grid_item.SyncLibrary = tick_this_library && tick_this_documents && sync_control_grid_item.library_sync_detail.web_library_detail.IsIntranetLibrary;
            }
        }

        public bool CanRunWithoutIntervention()
        {
            // IF the user has explicitly asked for intervention...
            if (sync_request.wants_user_intervention) return false;

            bool nothing_to_do = true;
            bool needs_intervention = false;

            foreach (SyncControlGridItem sync_control_grid_item in grid_items)
            {
                // Check if there is anything preventing us
                if (sync_control_grid_item.UserRequestedSync && !sync_control_grid_item.library_sync_detail.sync_decision.can_sync)
                {
                    needs_intervention = true;
                }

                // Check that there is something to do
                if (sync_control_grid_item.SyncLibrary)
                {
                    nothing_to_do = false;
                }
            }

            return !(needs_intervention || nothing_to_do);
        }
    }
}
