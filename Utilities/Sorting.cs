namespace Utilities
{
    public class Sorting
    {
        public static int CompareLengths(string a, string b)
        {
            if (null == a) a = "";
            if (null == b) b = "";

            return Compare(a.Length, b.Length);
        }

        public static int Compare(string a, string b)
        {
            return string.Compare(a, b);
        }

        public static int Compare(int a, int b)
        {
            if (a < b) return -1;
            if (a > b) return +1;
            return 0;
        }

        public static int Compare(double a, double b)
        {
            if (a < b) return -1;
            if (a > b) return +1;
            return 0;
        }

        public static int Compare(bool? a, bool? b)
        {
            if (!a.HasValue && !b.HasValue) return 0;
            if (!a.HasValue) return +1;
            if (!b.HasValue) return -1;
            return Compare(a.Value, b.Value);
        }

        public static int Compare(bool a, bool b)
        {
            int ai = a ? 1 : 0;
            int bi = b ? 1 : 0;
            return Compare(ai, bi);
        }

        public static int Compare(System.DateTime? a, System.DateTime? b)
        {
            if (!a.HasValue && !b.HasValue) return 0;
            if (!a.HasValue) return +1;
            if (!b.HasValue) return -1;
            return Compare(a.Value, b.Value);
        }

        public static int Compare(System.DateTime a, System.DateTime b)
        {
            if (a < b) return -1;
            if (b < a) return +1;
            return 0;
        }
    }
}
