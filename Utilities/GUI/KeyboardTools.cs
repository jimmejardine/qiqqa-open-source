using System.Windows.Input;

namespace Utilities.GUI
{
    public class KeyboardTools
    {
        public static bool IsCTRLDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        public static bool IsALTDown()
        {
            return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
        }

        public static bool IsShiftDown()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        public static bool IsWinDown()
        {
            return Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin);
        }

        public static bool isAlphaNumeric(int code)
        {
            if (code >= 'A' && code <= 'Z') return true;
            if (code >= '0' && code <= '9') return true;
            if (code == ' ') return true;

            return false;
        }

    }
}
