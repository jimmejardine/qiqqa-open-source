using System.Collections.Generic;

namespace Utilities.Collections
{
    public class SetTools
    {
        public static HashSet<string> FoldInSet_Intersection(HashSet<string> total, HashSet<string> individual)
        {
            // If both sets are empty, then return empty
            if (null == total && null == individual)
            {
                return null;
            }

            // If the total set is empty, but we have a child, start a total with a clone of the child
            if (null == total)
            {
                return new HashSet<string>(individual);
            }

            // If the child is empty, do nothing
            if (null == individual)
            {
                return total;
            }

            // Otherwise interset the two sets
            total.IntersectWith(individual);
            return total;
        }

        public static HashSet<string> FoldInSet_Intersection(List<HashSet<string>> individuals)
        {
            HashSet<string> intersection = null;
            foreach (var individual in individuals)
            {
                intersection = FoldInSet_Intersection(intersection, individual);
            }
            return intersection;
        }

        public static HashSet<string> FoldInSet_Union(HashSet<string> total, HashSet<string> individual)
        {
            // If both sets are empty, then return empty
            if (null == total && null == individual)
            {
                return null;
            }

            // If the total set is empty, but we have a child, start a total with a clone of the child
            if (null == total)
            {
                return new HashSet<string>(individual);
            }

            // If the child is empty, do nothing
            if (null == individual)
            {
                return total;
            }

            // Otherwise interset the two sets
            total.UnionWith(individual);
            return total;
        }

    }
}
