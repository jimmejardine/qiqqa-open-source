#if false

using System;

namespace Qiqqa.UtilisationTracking
{
    [Serializable]
    class UsedFeature
    {
        DateTime timestamp;
        Feature feature;
        string parameters;

        internal UsedFeature(Feature feature, string parameters)
        {
            // Remove all linebreaks from the parameters
            if (null != parameters)
            {
                parameters = parameters.Replace("\r\n", " ");
                parameters = parameters.Replace("\n", " ");
                parameters = parameters.Replace("\r", " ");
            }

            this.timestamp = DateTime.UtcNow;
            this.feature = feature;
            this.parameters = parameters;
        }

        internal double DaysAge
        {
            get
            {
                return DateTime.UtcNow.Subtract(timestamp).TotalDays;
            }
        }

        public Feature Feature
        {
            get
            {
                return feature;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return timestamp;
            }
        }

        public override string ToString()
        {
            return timestamp.ToString("yyyyMMdd.HHmmss") + "." + feature.Name + "." + parameters;
        }
    }
}

#endif
