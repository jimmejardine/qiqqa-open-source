using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Utilities.Reflection
{
    public class PropertyNames
    {
        public static string Get<T>(Expression<Func<T>> property)
        {
            PropertyInfo property_info = (property.Body as MemberExpression).Member as PropertyInfo;
            if (property_info == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            return property_info.Name;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            T t = new T();
            t.Name = "Test";
        }

        class T
        {
            string name;
            public string Name
            {
                get { return name; }
                set
                {
                    name = value;
                    Logging.Info(Get(() => Name));
                }
            }
        }
#endif

        #endregion
    }
}
