using System;
using System.Text;

namespace Utilities.Language
{
    /// <summary>
    /// Summary description for Comparing.
    /// </summary>
    public class Comparing
    {
        public static string ConvertInternationalToASCII(string international)
        {
            StringBuilder local = new StringBuilder();
            foreach (char c_international in international)
            {
                char c_local = ConvertInternationalToASCII(c_international);
                local.Append(c_local);
            }

            return local.ToString();
        }

        public static char ConvertInternationalToASCII(char international)
        {
            char c2 = Char.ToLower(international);

            if ("абвгдежЄ".IndexOf(c2) != -1) return 'a';
            if ("жийкл".IndexOf(c2) != -1) return 'e';
            if ("мноп".IndexOf(c2) != -1) return 'i';
            if ("ртуфхцш".IndexOf(c2) != -1) return 'o';
            if ("щъыь".IndexOf(c2) != -1) return 'u';
            if ("эя".IndexOf(c2) != -1) return 'y';
            if ("с".IndexOf(c2) != -1) return 'n';
            if ("з".IndexOf(c2) != -1) return 'c';

            return c2;
        }

        public static bool isKeyboardCompatible(char c1, char c2)
        {
            // Some straightforward tests
            if (c1 == c2) return true;

            // Case insensitive compare now
            c1 = Char.ToLower(c1);
            c2 = Char.ToLower(c2);
            if (c1 == c2) return true;

            c1 = ConvertInternationalToASCII(c1);
            c2 = ConvertInternationalToASCII(c2);
            if (c1 == c2) return true;

            // If nothing has matched so far, they must be different
            return false;
        }
    }
}
