using System.Collections.Generic;
using Utilities;

namespace Qiqqa.UtilisationTracking
{
    public class FeatureTrackingManager
    {
        private static FeatureTrackingManager __instance = null;
        public static FeatureTrackingManager Instance
        {
            get
            {
                if (null == __instance)
                {
                    __instance = new FeatureTrackingManager();
                }
                return __instance;
            }
        }

        private HashSet<string> used_once_off_per_session_features = new HashSet<string>();
        private object used_once_off_per_session_features_lock = new object();

        private FeatureTrackingManager()
        {
            Logging.Info("Starting the FeatureTrackingManager");
        }

        public void UseFeature(Feature feature, params object[] parameters)
        {
            // when there are parameters, create a feature instance which includes them for serialization:
            if (parameters.Length > 0)
            {
                feature = feature._(parameters);
            }

            // Check that we are not meant to be storing this feature only once...
            if (feature.RecordOnlyOncePerSession)
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (used_once_off_per_session_features_lock)
                {
                    l1_clk.LockPerfTimerStop();
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

#if false
            // Send to GA
            GoogleAnalysicsSubmitter.Submit(feature);
#endif
        }
    }
}
