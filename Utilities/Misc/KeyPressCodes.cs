using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Utilities
{
    // Note https://stackoverflow.com/questions/757684/enum-inheritance alas.   :'-(

    /// <summary>
    ///     Specifies the possible key values on a keyboard.
    /// </summary>
    public enum KeyPressCode
    {
        //
        // Summary:
        //     No key pressed.
        None = Key.None,
        //
        // Summary:
        //     The Cancel key.
        Cancel = Key.Cancel,
        //
        // Summary:
        //     The Backspace key.
        Back = Key.Back,
        //
        // Summary:
        //     The Tab key.
        Tab = Key.Tab,
        //
        // Summary:
        //     The Linefeed key.
        LineFeed = Key.LineFeed,
        //
        // Summary:
        //     The Clear key.
        Clear = Key.Clear,
        //
        // Summary:
        //     The Return key.
        Return = Key.Return,
        //
        // Summary:
        //     The Enter key.
        Enter = Key.Enter,
        //
        // Summary:
        //     The Pause key.
        Pause = Key.Pause,
        //
        // Summary:
        //     The Caps Lock key.
        Capital = Key.Capital,
        //
        // Summary:
        //     The Caps Lock key.
        CapsLock = Key.CapsLock,
        //
        // Summary:
        //     The IME Kana mode key.
        KanaMode = Key.KanaMode,
        //
        // Summary:
        //     The IME Hangul mode key.
        HangulMode = Key.HangulMode,
        //
        // Summary:
        //     The IME Junja mode key.
        JunjaMode = Key.JunjaMode,
        //
        // Summary:
        //     The IME Final mode key.
        FinalMode = Key.FinalMode,
        //
        // Summary:
        //     The IME Hanja mode key.
        HanjaMode = Key.HanjaMode,
        //
        // Summary:
        //     The IME Kanji mode key.
        KanjiMode = Key.KanjiMode,
        //
        // Summary:
        //     The ESC key.
        Escape = Key.Escape,
        //
        // Summary:
        //     The IME Convert key.
        ImeConvert = Key.ImeConvert,
        //
        // Summary:
        //     The IME NonConvert key.
        ImeNonConvert = Key.ImeNonConvert,
        //
        // Summary:
        //     The IME Accept key.
        ImeAccept = Key.ImeAccept,
        //
        // Summary:
        //     The IME Mode change request.
        ImeModeChange = Key.ImeModeChange,
        //
        // Summary:
        //     The Spacebar key.
        Space = Key.Space,
        //
        // Summary:
        //     The Page Up key.
        Prior = Key.Prior,
        //
        // Summary:
        //     The Page Up key.
        PageUp = Key.PageUp,
        //
        // Summary:
        //     The Page Down key.
        Next = Key.Next,
        //
        // Summary:
        //     The Page Down key.
        PageDown = Key.PageDown,
        //
        // Summary:
        //     The End key.
        End = Key.End,
        //
        // Summary:
        //     The Home key.
        Home = Key.Home,
        //
        // Summary:
        //     The Left Arrow key.
        Left = Key.Left,
        //
        // Summary:
        //     The Up Arrow key.
        Up = Key.Up,
        //
        // Summary:
        //     The Right Arrow key.
        Right = Key.Right,
        //
        // Summary:
        //     The Down Arrow key.
        Down = Key.Down,
        //
        // Summary:
        //     The Select key.
        Select = Key.Select,
        //
        // Summary:
        //     The Print key.
        Print = Key.Print,
        //
        // Summary:
        //     The Execute key.
        Execute = Key.Execute,
        //
        // Summary:
        //     The Print Screen key.
        Snapshot = Key.Snapshot,
        //
        // Summary:
        //     The Print Screen key.
        PrintScreen = Key.PrintScreen,
        //
        // Summary:
        //     The Insert key.
        Insert = Key.Insert,
        //
        // Summary:
        //     The Delete key.
        Delete = Key.Delete,
        //
        // Summary:
        //     The Help key.
        Help = Key.Help,
        //
        // Summary:
        //     The 0 (zero) key.
        D0 = Key.D0,
        //
        // Summary:
        //     The 1 (one) key.
        D1 = Key.D1,
        //
        // Summary:
        //     The 2 key.
        D2 = Key.D2,
        //
        // Summary:
        //     The 3 key.
        D3 = Key.D3,
        //
        // Summary:
        //     The 4 key.
        D4 = Key.D4,
        //
        // Summary:
        //     The 5 key.
        D5 = Key.D5,
        //
        // Summary:
        //     The 6 key.
        D6 = Key.D6,
        //
        // Summary:
        //     The 7 key.
        D7 = Key.D7,
        //
        // Summary:
        //     The 8 key.
        D8 = Key.D8,
        //
        // Summary:
        //     The 9 key.
        D9 = Key.D9,
        //
        // Summary:
        //     The A key.
        A = Key.A,
        //
        // Summary:
        //     The B key.
        B = Key.B,
        //
        // Summary:
        //     The C key.
        C = Key.C,
        //
        // Summary:
        //     The D key.
        D = Key.D,
        //
        // Summary:
        //     The E key.
        E = Key.E,
        //
        // Summary:
        //     The F key.
        F = Key.F,
        //
        // Summary:
        //     The G key.
        G = Key.G,
        //
        // Summary:
        //     The H key.
        H = Key.H,
        //
        // Summary:
        //     The I key.
        I = Key.I,
        //
        // Summary:
        //     The J key.
        J = Key.J,
        //
        // Summary:
        //     The K key.
        K = Key.K,
        //
        // Summary:
        //     The L key.
        L = Key.L,
        //
        // Summary:
        //     The M key.
        M = Key.M,
        //
        // Summary:
        //     The N key.
        N = Key.N,
        //
        // Summary:
        //     The O key.
        O = Key.O,
        //
        // Summary:
        //     The P key.
        P = Key.P,
        //
        // Summary:
        //     The Q key.
        Q = Key.Q,
        //
        // Summary:
        //     The R key.
        R = Key.R,
        //
        // Summary:
        //     The S key.
        S = Key.S,
        //
        // Summary:
        //     The T key.
        T = Key.T,
        //
        // Summary:
        //     The U key.
        U = Key.U,
        //
        // Summary:
        //     The V key.
        V = Key.V,
        //
        // Summary:
        //     The W key.
        W = Key.W,
        //
        // Summary:
        //     The X key.
        X = Key.X,
        //
        // Summary:
        //     The Y key.
        Y = Key.Y,
        //
        // Summary:
        //     The Z key.
        Z = Key.Z,
        //
        // Summary:
        //     The left Windows logo key (Microsoft Natural Keyboard).
        LWinKey = Key.LWin,
        //
        // Summary:
        //     The right Windows logo key (Microsoft Natural Keyboard).
        RWinKey = Key.RWin,
        //
        // Summary:
        //     The Application key (Microsoft Natural Keyboard).
        AppsKey = Key.Apps,
        //
        // Summary:
        //     The Computer Sleep key.
        Sleep = Key.Sleep,
        //
        // Summary:
        //     The 0 key on the numeric keypad.
        NumPad0 = Key.NumPad0,
        //
        // Summary:
        //     The 1 key on the numeric keypad.
        NumPad1 = Key.NumPad1,
        //
        // Summary:
        //     The 2 key on the numeric keypad.
        NumPad2 = Key.NumPad2,
        //
        // Summary:
        //     The 3 key on the numeric keypad.
        NumPad3 = Key.NumPad3,
        //
        // Summary:
        //     The 4 key on the numeric keypad.
        NumPad4 = Key.NumPad4,
        //
        // Summary:
        //     The 5 key on the numeric keypad.
        NumPad5 = Key.NumPad5,
        //
        // Summary:
        //     The 6 key on the numeric keypad.
        NumPad6 = Key.NumPad6,
        //
        // Summary:
        //     The 7 key on the numeric keypad.
        NumPad7 = Key.NumPad7,
        //
        // Summary:
        //     The 8 key on the numeric keypad.
        NumPad8 = Key.NumPad8,
        //
        // Summary:
        //     The 9 key on the numeric keypad.
        NumPad9 = Key.NumPad9,
        //
        // Summary:
        //     The Multiply key.
        Multiply = Key.Multiply,
        //
        // Summary:
        //     The Add key.
        Add = Key.Add,
        //
        // Summary:
        //     The Separator key.
        Separator = Key.Separator,
        //
        // Summary:
        //     The Subtract key.
        Subtract = Key.Subtract,
        //
        // Summary:
        //     The Decimal key.
        Decimal = Key.Decimal,
        //
        // Summary:
        //     The Divide key.
        Divide = Key.Divide,
        //
        // Summary:
        //     The F1 key.
        F1 = Key.F1,
        //
        // Summary:
        //     The F2 key.
        F2 = Key.F2,
        //
        // Summary:
        //     The F3 key.
        F3 = Key.F3,
        //
        // Summary:
        //     The F4 key.
        F4 = Key.F4,
        //
        // Summary:
        //     The F5 key.
        F5 = Key.F5,
        //
        // Summary:
        //     The F6 key.
        F6 = Key.F6,
        //
        // Summary:
        //     The F7 key.
        F7 = Key.F7,
        //
        // Summary:
        //     The F8 key.
        F8 = Key.F8,
        //
        // Summary:
        //     The F9 key.
        F9 = Key.F9,
        //
        // Summary:
        //     The F10 key.
        F10 = Key.F10,
        //
        // Summary:
        //     The F11 key.
        F11 = Key.F11,
        //
        // Summary:
        //     The F12 key.
        F12 = Key.F12,
        //
        // Summary:
        //     The F13 key.
        F13 = Key.F13,
        //
        // Summary:
        //     The F14 key.
        F14 = Key.F14,
        //
        // Summary:
        //     The F15 key.
        F15 = Key.F15,
        //
        // Summary:
        //     The F16 key.
        F16 = Key.F16,
        //
        // Summary:
        //     The F17 key.
        F17 = Key.F17,
        //
        // Summary:
        //     The F18 key.
        F18 = Key.F18,
        //
        // Summary:
        //     The F19 key.
        F19 = Key.F19,
        //
        // Summary:
        //     The F20 key.
        F20 = Key.F20,
        //
        // Summary:
        //     The F21 key.
        F21 = Key.F21,
        //
        // Summary:
        //     The F22 key.
        F22 = Key.F22,
        //
        // Summary:
        //     The F23 key.
        F23 = Key.F23,
        //
        // Summary:
        //     The F24 key.
        F24 = Key.F24,
        //
        // Summary:
        //     The Num Lock key.
        NumLock = Key.NumLock,
        //
        // Summary:
        //     The Scroll Lock key.
        Scroll = Key.Scroll,
        //
        // Summary:
        //     The left Shift key.
        LeftShiftKey = Key.LeftShift,
        //
        // Summary:
        //     The right Shift key.
        RightShiftKey = Key.RightShift,
        //
        // Summary:
        //     The left CTRL key.
        LeftCtrlKey = Key.LeftCtrl,
        //
        // Summary:
        //     The right CTRL key.
        RightCtrlKey = Key.RightCtrl,
        //
        // Summary:
        //     The left ALT key.
        LeftAltKey = Key.LeftAlt,
        //
        // Summary:
        //     The right ALT key.
        RightAltKey = Key.RightAlt,
        //
        // Summary:
        //     The Browser Back key.
        BrowserBack = Key.BrowserBack,
        //
        // Summary:
        //     The Browser Forward key.
        BrowserForward = Key.BrowserForward,
        //
        // Summary:
        //     The Browser Refresh key.
        BrowserRefresh = Key.BrowserRefresh,
        //
        // Summary:
        //     The Browser Stop key.
        BrowserStop = Key.BrowserStop,
        //
        // Summary:
        //     The Browser Search key.
        BrowserSearch = Key.BrowserSearch,
        //
        // Summary:
        //     The Browser Favorites key.
        BrowserFavorites = Key.BrowserFavorites,
        //
        // Summary:
        //     The Browser Home key.
        BrowserHome = Key.BrowserHome,
        //
        // Summary:
        //     The Volume Mute key.
        VolumeMute = Key.VolumeMute,
        //
        // Summary:
        //     The Volume Down key.
        VolumeDown = Key.VolumeDown,
        //
        // Summary:
        //     The Volume Up key.
        VolumeUp = Key.VolumeUp,
        //
        // Summary:
        //     The Media Next Track key.
        MediaNextTrack = Key.MediaNextTrack,
        //
        // Summary:
        //     The Media Previous Track key.
        MediaPreviousTrack = Key.MediaPreviousTrack,
        //
        // Summary:
        //     The Media Stop key.
        MediaStop = Key.MediaStop,
        //
        // Summary:
        //     The Media Play Pause key.
        MediaPlayPause = Key.MediaPlayPause,
        //
        // Summary:
        //     The Launch Mail key.
        LaunchMail = Key.LaunchMail,
        //
        // Summary:
        //     The Select Media key.
        SelectMedia = Key.SelectMedia,
        //
        // Summary:
        //     The Launch Application1 key.
        LaunchApplication1 = Key.LaunchApplication1,
        //
        // Summary:
        //     The Launch Application2 key.
        LaunchApplication2 = Key.LaunchApplication2,
        //
        // Summary:
        //     The OEM 1 key.
        Oem1 = Key.Oem1,
        //
        // Summary:
        //     The OEM Semicolon key.
        OemSemicolon = Key.OemSemicolon,
        //
        // Summary:
        //     The OEM Addition key.
        OemPlus = Key.OemPlus,
        //
        // Summary:
        //     The OEM Comma key.
        OemComma = Key.OemComma,
        //
        // Summary:
        //     The OEM Minus key.
        OemMinus = Key.OemMinus,
        //
        // Summary:
        //     The OEM Period key.
        OemPeriod = Key.OemPeriod,
        //
        // Summary:
        //     The OEM 2 key.
        Oem2 = Key.Oem2,
        //
        // Summary:
        //     The OEM Question key.
        OemQuestion = Key.OemQuestion,
        //
        // Summary:
        //     The OEM 3 key.
        Oem3 = Key.Oem3,
        //
        // Summary:
        //     The OEM Tilde key.
        OemTilde = Key.OemTilde,
        //
        // Summary:
        //     The ABNT_C1 (Brazilian) key.
        AbntC1 = Key.AbntC1,
        //
        // Summary:
        //     The ABNT_C2 (Brazilian) key.
        AbntC2 = Key.AbntC2,
        //
        // Summary:
        //     The OEM 4 key.
        Oem4 = Key.Oem4,
        //
        // Summary:
        //     The OEM Open Brackets key.
        OemOpenBrackets = Key.OemOpenBrackets,
        //
        // Summary:
        //     The OEM 5 key.
        Oem5 = Key.Oem5,
        //
        // Summary:
        //     The OEM Pipe key.
        OemPipe = Key.OemPipe,
        //
        // Summary:
        //     The OEM 6 key.
        Oem6 = Key.Oem6,
        //
        // Summary:
        //     The OEM Close Brackets key.
        OemCloseBrackets = Key.OemCloseBrackets,
        //
        // Summary:
        //     The OEM 7 key.
        Oem7 = Key.Oem7,
        //
        // Summary:
        //     The OEM Quotes key.
        OemQuotes = Key.OemQuotes,
        //
        // Summary:
        //     The OEM 8 key.
        Oem8 = Key.Oem8,
        //
        // Summary:
        //     The OEM 102 key.
        Oem102 = Key.Oem102,
        //
        // Summary:
        //     The OEM Backslash key.
        OemBackslash = Key.OemBackslash,
        //
        // Summary:
        //     A special key masking the real key being processed by an IME.
        ImeProcessed = Key.ImeProcessed,
        //
        // Summary:
        //     A special key masking the real key being processed as a system key.
        System = Key.System,
        //
        // Summary:
        //     The OEM ATTN key.
        OemAttn = Key.OemAttn,
        //
        // Summary:
        //     The DBE_ALPHANUMERIC key.
        DbeAlphanumeric = Key.DbeAlphanumeric,
        //
        // Summary:
        //     The OEM FINISH key.
        OemFinish = Key.OemFinish,
        //
        // Summary:
        //     The DBE_KATAKANA key.
        DbeKatakana = Key.DbeKatakana,
        //
        // Summary:
        //     The OEM COPY key.
        OemCopy = Key.OemCopy,
        //
        // Summary:
        //     The DBE_HIRAGANA key.
        DbeHiragana = Key.DbeHiragana,
        //
        // Summary:
        //     The OEM AUTO key.
        OemAuto = Key.OemAuto,
        //
        // Summary:
        //     The DBE_SBCSCHAR key.
        DbeSbcsChar = Key.DbeSbcsChar,
        //
        // Summary:
        //     The OEM ENLW key.
        OemEnlw = Key.OemEnlw,
        //
        // Summary:
        //     The DBE_DBCSCHAR key.
        DbeDbcsChar = Key.DbeDbcsChar,
        //
        // Summary:
        //     The OEM BACKTAB key.
        OemBackTab = Key.OemBackTab,
        //
        // Summary:
        //     The DBE_ROMAN key.
        DbeRoman = Key.DbeRoman,
        //
        // Summary:
        //     The ATTN key.
        Attn = Key.Attn,
        //
        // Summary:
        //     The DBE_NOROMAN key.
        DbeNoRoman = Key.DbeNoRoman,
        //
        // Summary:
        //     The CRSEL key.
        CrSel = Key.CrSel,
        //
        // Summary:
        //     The DBE_ENTERWORDREGISTERMODE key.
        DbeEnterWordRegisterMode = Key.DbeEnterWordRegisterMode,
        //
        // Summary:
        //     The EXSEL key.
        ExSel = Key.ExSel,
        //
        // Summary:
        //     The DBE_ENTERIMECONFIGMODE key.
        DbeEnterImeConfigureMode = Key.DbeEnterImeConfigureMode,
        //
        // Summary:
        //     The ERASE EOF key.
        EraseEof = Key.EraseEof,
        //
        // Summary:
        //     The DBE_FLUSHSTRING key.
        DbeFlushString = Key.DbeFlushString,
        //
        // Summary:
        //     The PLAY key.
        Play = Key.Play,
        //
        // Summary:
        //     The DBE_CODEINPUT key.
        DbeCodeInput = Key.DbeCodeInput,
        //
        // Summary:
        //     The ZOOM key.
        Zoom = Key.Zoom,
        //
        // Summary:
        //     The DBE_NOCODEINPUT key.
        DbeNoCodeInput = Key.DbeNoCodeInput,
        //
        // Summary:
        //     A constant reserved for future use.
        NoName = Key.NoName,
        //
        // Summary:
        //     The DBE_DETERMINESTRING key.
        DbeDetermineString = Key.DbeDetermineString,
        //
        // Summary:
        //     The PA1 key.
        Pa1 = Key.Pa1,
        //
        // Summary:
        //     The DBE_ENTERDLGCONVERSIONMODE key.
        DbeEnterDialogConversionMode = Key.DbeEnterDialogConversionMode,
        //
        // Summary:
        //     The OEM Clear key.
        OemClear = Key.OemClear,
        //
        // Summary:
        //     The key is used with another key to create a single combined character.
        DeadCharProcessed = Key.DeadCharProcessed,

        // additional meta keys as bitmaskable values:
        LeftCtrl = 0x1000,
        RightCtrl = 0x2000,
        Ctrl = LeftCtrl | RightCtrl,
        LeftAlt = 0x4000,
        RightAlt = 0x8000,
        Alt = LeftAlt | RightAlt,
        LeftShift = 0x10000,
        RightShift = 0x20000,
        Shift = LeftShift | RightShift,
        LeftWin = 0x40000,
        RightWin = 0x80000,
        Windows = LeftWin | RightWin,
        Apps = 0x100000,

        MetaKeysMask = 0x1FF000,
        KeyCodeMask = 0x00FF,
    }
}
