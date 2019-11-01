using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Utilities.Collections;

namespace Utilities.Reflection
{
    public class PropertyDependencies
    {
        private MultiMap<string, string> dependencies = new MultiMap<string, string>(true);
        private List<string> EMPTY = new List<string>();

        public PropertyDependencies()
        {
        }

        public void Add<T, U>(Expression<Func<T>> changer, Expression<Func<U>> changed)
        {
            string changer_name = PropertyNames.Get<T>(changer);
            string changed_name = PropertyNames.Get<U>(changed);
            dependencies.Add(changer_name, changed_name);
        }

        public List<string> GetDependencies(string changer)
        {
            List<string> changeds;
            if (dependencies.TryGetValue(changer, out changeds))
            {
                return changeds;
            }
            else
            {
                return EMPTY;
            }
        }
    }
}
