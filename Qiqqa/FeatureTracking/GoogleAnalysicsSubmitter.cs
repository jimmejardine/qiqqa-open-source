#if false

using System;
using System.Net;
using System.Text;
using Qiqqa.Common.Configuration;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Misc;


namespace Qiqqa.FeatureTracking
{
    internal class GoogleAnalysicsSubmitter
    {
        private static readonly string TRACKING_ID = "UA-16059803-8";
        private static readonly string URL = WebsiteAccess.Url_GoogleAnalyticsTracker4Qiqqa;

        private static DateTime last_ga_failure_time = DateTime.MinValue;


        public static void Submit(Feature feature)
        {
            // Do not track if we have been asked not to
            if (!ConfigurationManager.Instance.ConfigurationRecord.Feedback_UtilisationInfo) return;

            // Do not try if we have failed recently
            if (15 > DateTime.UtcNow.Subtract(last_ga_failure_time).TotalMinutes) return;

            // Queue in background
            SafeThreadPool.QueueUserWorkItem(o => Submit_BACKGROUND(feature));
        }

        private static void Submit_BACKGROUND(Feature feature)
        {
            string request = "???";

            try
            {
                // Build up the request
                StringBuilder sb = new StringBuilder();
                {
                    sb.Append("v=1");
                    sb.AppendFormat("&tid={0}", TRACKING_ID);
                    sb.AppendFormat("&cid={0}", ConfigurationManager.Instance.ConfigurationRecord.Feedback_GATrackingCode);
                    sb.AppendFormat("&t={0}", "pageview");
                    sb.AppendFormat("&dp=/Client/Windows/Feature/{0}", feature.Name);
                }
                request = sb.ToString();

                // Send
                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = ConfigurationManager.Instance.Proxy;
                    string response = wc.UploadString(URL, request);

                    Logging.Info("Google Analytics Submitted: REQUEST:\n{0}\n\nRESPONSE:\n{1}", request, response);
                }
            }
            catch (Exception ex)
            {
                last_ga_failure_time = DateTime.UtcNow;
                Logging.Error(ex, "Google Analytics submission failure for feature {0}: URI: {1}, REQUEST: {2}", feature.Name, URL, request);
            }
        }
    }
}

#endif
