using System.Collections.Generic;

namespace Qiqqa.Synchronisation.MetadataSync
{
    public class SynchronisationAction
    {

        #region --- Action lists --------------------------------------------------------------------------------

        public List<SynchronisationState> states_to_download = new List<SynchronisationState>();
        public List<SynchronisationState> states_to_upload = new List<SynchronisationState>();
        public List<SynchronisationState> states_to_merge = new List<SynchronisationState>();

        #endregion

        #region --- Informational lists --------------------------------------------------------------------------------

        public List<SynchronisationState> states_dodgy = new List<SynchronisationState>();
        public List<SynchronisationState> states_already_synced = new List<SynchronisationState>();

        #endregion

    }
}
