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
        public object[] parameters;

        // for those features that accept parameters:
        internal Feature _(params object[] f_parameters)
        {
            return new Feature
            {
                Name = this.Name,
                Category = this.Category,
                Description = this.Description,
                RecordOnlyOncePerSession = false,
                parameters = f_parameters
            };
        }
    }
}
