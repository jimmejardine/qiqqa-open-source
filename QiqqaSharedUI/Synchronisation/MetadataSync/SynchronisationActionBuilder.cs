using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Utilities.Misc;

namespace Qiqqa.Synchronisation.MetadataSync
{
    internal class SynchronisationActionBuilder
    {
        internal static SynchronisationAction Build(Library library, SynchronisationStates synchronisation_states)
        {
            SynchronisationAction synchronisation_action = new SynchronisationAction();

            List<SynchronisationState> synchronisation_state_list = new List<SynchronisationState>(synchronisation_states.Values);
            for (int i = 0; i < synchronisation_state_list.Count; ++i)
            {
                SynchronisationState synchronisation_state = synchronisation_state_list[i];

                if (false
                    || (0 == i % 20 && synchronisation_state_list.Count < 100)
                    || (0 == i % 100 && synchronisation_state_list.Count < 1000)
                    || (0 == i % 500)
                    )
                {
                    StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(library), "Determining sync action", i, synchronisation_state_list.Count);
                }

                // NB: Ordering of these statements is important so dont reorder them!
                
                // Not local, not remote: something dodgy in the history file, ignore it
                if (null == synchronisation_state.md5_local && null == synchronisation_state.md5_remote)
                {
                    synchronisation_action.states_dodgy.Add(synchronisation_state);
                }

                // If we don't have it locally but we do remotely
                else if (null == synchronisation_state.md5_local && null != synchronisation_state.md5_remote)
                {
                    synchronisation_action.states_to_download.Add(synchronisation_state);
                }

                // If we don't have it remotely, but we do locally
                else if (null != synchronisation_state.md5_local && null == synchronisation_state.md5_remote)
                {
                    synchronisation_action.states_to_upload.Add(synchronisation_state);
                }

                // If local and remote match, do nothing
                else if (0 == synchronisation_state.md5_local.CompareTo(synchronisation_state.md5_remote))
                {
                    synchronisation_action.states_already_synced.Add(synchronisation_state);
                }

                // If local and remote don't match, but local has not changed
                else if (0 == synchronisation_state.md5_local.CompareTo(synchronisation_state.md5_previous))
                {
                    synchronisation_action.states_to_download.Add(synchronisation_state);
                }

                // If local and remote don't match, but remote has not changed
                else if (0 == synchronisation_state.md5_remote.CompareTo(synchronisation_state.md5_previous))
                {
                    synchronisation_action.states_to_upload.Add(synchronisation_state);
                }

                // If local and remote dont match and neither match the previous value, we have a merge conflict
                else
                {
                    synchronisation_action.states_to_merge.Add(synchronisation_state);
                }
            }

            return synchronisation_action;
        }
    }
}
