using System;

namespace Qiqqa.UtilisationTracking
{
    [Serializable]
    public class Feature
    {
        public string Name;
        public string Category;
        public string Description;
        public bool RecordOnlyOncePerSession;
    }
}
