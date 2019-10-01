using System;
using System.Collections.Generic;
using System.Linq;
using Qiqqa.DocumentLibrary;
using Utilities;
using Utilities.Misc;

namespace Qiqqa.Synchronisation.MetadataSync
{
    class SynchronisationStateBuilder
    {
        /// <summary>
        /// Builds a map of SynchronisationState objects of the form
        ///     filename -> SynchronisationState
        /// </summary>
        /// <returns></returns>
        internal static SynchronisationStates Build(Library library, Dictionary<string, string> historical_sync_file)
        {
            SynchronisationStates synchronisation_states = new SynchronisationStates();

            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(library), "Building sync state from local history", 0, 1);
            BuildFromHistoricalSyncFile(historical_sync_file, ref synchronisation_states);

            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(library), "Building sync state from local files", 0, 1);
            BuildFromLocal(library, ref synchronisation_states);

            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(library), "Building sync state from Web/Intranet Library", 0, 1);
            BuildFromRemote(library, ref synchronisation_states);

            FilterSynchronisationStates(ref synchronisation_states);

            return synchronisation_states;
        }


        private static void FilterSynchronisationStates(ref SynchronisationStates synchronisation_states)
        {
            HashSet<string> ineligible_keys = new HashSet<string>();

            foreach (var pair in synchronisation_states)
            {
                if (!SynchronisationFileTypes.extensions.Contains(pair.Value.extension))
                {
                    Logging.Warn("Not syncing unsupported extension of {0} in {1}", pair.Value.extension, pair.Value.filename);
                    ineligible_keys.Add(pair.Key);
                }
            }

            foreach (string key in ineligible_keys)
            {
                synchronisation_states.Remove(key);
            }
        }

        private static void BuildFromHistoricalSyncFile(Dictionary<string, string> historical_sync_file, ref SynchronisationStates synchronisation_states)
        {            
            foreach (var pair in historical_sync_file)
            {
                synchronisation_states[pair.Key].md5_previous = pair.Value;
            }
        }

        private static void BuildFromLocal(Library library, ref SynchronisationStates synchronisation_states)
        {
            List<LibraryDB.LibraryItem> library_items = library.LibraryDB.GetLibraryItems(null, null);
            foreach (LibraryDB.LibraryItem library_item in library_items)
            {
                string short_filename = library_item.ToFileNameFormat();
                SynchronisationState synchronisation_state = synchronisation_states[short_filename];
                synchronisation_state.md5_local = library_item.md5.ToUpper();
                synchronisation_state.library_item = library_item;
            }
        }

        static void BuildFromRemote(Library library, ref SynchronisationStates synchronisation_states)
        {
            // --- TODO: Replace this with a pretty interface class ------------------------------------------------
            if (library.WebLibraryDetail.IsIntranetLibrary)
            {
                SynchronisationStateBuilder_Intranet.BuildFromRemote(library, ref synchronisation_states);
            }
            else
            {
                throw new Exception(String.Format("Did not understand how to build from remote for library '{0}' (Type: {1})", library.WebLibraryDetail.Title, library.WebLibraryDetail.LibraryType()));
            }
            // -----------------------------------------------------------------------------------------------------
        }
    }
}
