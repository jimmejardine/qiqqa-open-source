using System;
using System.Collections.Generic;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Synchronisation.MetadataSync
{
    [Serializable]
    public class SynchronisationStates : Dictionary<string, SynchronisationState>
    {
        public new SynchronisationState this[string key]
        {
            get
            {
                SynchronisationState synchronisation_state;
                if (!TryGetValue(key, out synchronisation_state))
                {
                    // Get the fingerprint and extension
                    string[] key_split = key.Split('.');
                    string fingerprint = key_split[0];
                    string extension = key_split[1];

                    synchronisation_state = new SynchronisationState();
                    synchronisation_state.filename = key;
                    synchronisation_state.fingerprint = fingerprint;
                    synchronisation_state.extension = extension;

                    this[key] = synchronisation_state;
                }

                return synchronisation_state;
            }

            protected set => base[key] = value;
        }
    }
}
