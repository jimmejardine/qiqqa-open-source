using System;

namespace Utilities.Misc
{
    public class TypedWeakReference<T> : WeakReference
    {
        public TypedWeakReference(T target) : base(target)
        {
        }

        public T TypedTarget
        {
            get
            {
                return (T)Target;
            }
            set
            {
                Target = value;
            }
        }
    }
}
