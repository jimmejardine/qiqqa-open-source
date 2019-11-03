namespace Utilities
{
    public class Swap
    {
        public static void swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }


        public static void swap(ref int a, ref int b)
        {
            int t = a;
            a = b;
            b = t;
        }

        public static void swap(ref char a, ref char b)
        {
            char t = a;
            a = b;
            b = t;
        }

        public static void swap(ref double a, ref double b)
        {
            double t = a;
            a = b;
            b = t;
        }

        public static void swap(ref string a, ref string b)
        {
            string t = a;
            a = b;
            b = t;
        }
    }

    public class Swap<TYPE>
    {
        public static void swap(ref TYPE a, ref TYPE b)
        {
            TYPE temp = a;
            a = b;
            b = temp;
        }
    }
}
