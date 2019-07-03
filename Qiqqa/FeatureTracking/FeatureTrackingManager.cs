using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GeneralTaskDaemonStuff;
using Utilities;
using Utilities.Files;
using Utilities.Shutdownable;
using Qiqqa.FeatureTracking;

namespace Qiqqa.UtilisationTracking
{
    public class FeatureTrackingManager
    {
        public static readonly FeatureTrackingManager Instance = new FeatureTrackingManager();

        HashSet<string> used_once_off_per_session_features = new HashSet<string>();

        private FeatureTrackingManager()
        {
            Logging.Info("Starting the FeatureTrackingManager");
        }

        public void UseFeature(Feature feature, params object[] parameters)
        {
            // Check that we are not meant to be storing this feature only once...
            if (feature.RecordOnlyOncePerSession)
            {
                lock (used_once_off_per_session_features)
                {
                    if (used_once_off_per_session_features.Contains(feature.Name))
                    {
                        return;
                    }
                    else
                    {
                        used_once_off_per_session_features.Add(feature.Name);
                    }
                }
            }

            // Send to GA
            GoogleAnalysicsSubmitter.Submit(feature);
        }
    }
}
