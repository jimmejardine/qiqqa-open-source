using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Misc
{
    public class DataChangedTracker
    {
        // Use this so we can *guarantee* unique marker codes for any update.
        private long ticker = 0;

        private long last_updated = 0;

        // Updater should call this API to mark any update as 'done':
        public void MarkAsUpdated()
        {
            Interlocked.Increment(ref ticker);

            last_updated = ticker;
        }

        // Observers should poll this API, or ...
        public long GetLastUpdateMarker()
        {
            return last_updated;
        }

        // ... poll for updates using this API.
        public bool HasBeenUpdated(ref long previous_marker)
        {
            long new_marker = last_updated;
            bool rv = new_marker != previous_marker;
            previous_marker = new_marker;
            return rv;
        }
    }
}
