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

        public static KeyPressCode Key2KeyPressCode(Key k)
        {
            KeyPressCode rv = (KeyPressCode)k;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                rv |= KeyPressCode.LeftCtrl;
            }
            if (Keyboard.IsKeyDown(Key.RightCtrl))
            {
                rv |= KeyPressCode.RightShift;
            }

            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                rv |= KeyPressCode.LeftAlt;
            }
            if (Keyboard.IsKeyDown(Key.RightAlt))
            {
                rv |= KeyPressCode.RightAlt;
            }

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                rv |= KeyPressCode.LeftShift;
            }
            if (Keyboard.IsKeyDown(Key.RightShift))
            {
                rv |= KeyPressCode.RightShift;
            }

            if (Keyboard.IsKeyDown(Key.LWin))
            {
                rv |= KeyPressCode.LeftWin;
            }
            if (Keyboard.IsKeyDown(Key.RWin))
            {
                rv |= KeyPressCode.RightWin;
            }

            if (Keyboard.IsKeyDown(Key.Apps))
            {
                rv |= KeyPressCode.Apps;
            }

            return rv;
        }

        public static KeyPressCode GetCleanKeyPressCode(KeyPressCode k)
        {
            return k & KeyPressCode.KeyCodeMask;
        }

        public static KeyPressCode GetKeyPressMetaBits(KeyPressCode k)
        {
            return k & KeyPressCode.MetaKeysMask;
        }
    }
}
