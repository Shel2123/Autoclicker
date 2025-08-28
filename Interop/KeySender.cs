using System.Runtime.InteropServices;
using Autoclicker.Core.Abstractions;

namespace Autoclicker.Interop
{
    internal sealed class KeySender : IKeyboard, IKeyParser
    {
        [StructLayout(LayoutKind.Sequential)] struct INPUT { public uint type; public InputUnion U; }
        [StructLayout(LayoutKind.Explicit)] struct InputUnion { [FieldOffset(0)] public MOUSEINPUT mi; [FieldOffset(0)] public KEYBDINPUT ki; [FieldOffset(0)] public HARDWAREINPUT hi; }
        [StructLayout(LayoutKind.Sequential)] struct MOUSEINPUT { public int dx; public int dy; public uint mouseData; public uint dwFlags; public uint time; public nint dwExtraInfo; }
        [StructLayout(LayoutKind.Sequential)] struct KEYBDINPUT { public ushort wVk; public ushort wScan; public uint dwFlags; public uint time; public nint dwExtraInfo; }
        [StructLayout(LayoutKind.Sequential)] struct HARDWAREINPUT { public uint uMsg; public ushort wParamL; public ushort wParamH; }

        [DllImport("user32.dll", SetLastError = true)] static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        [DllImport("user32.dll")] static extern uint MapVirtualKey(uint uCode, uint uMapType); // 0 = VK->ScanCode

        const uint INPUT_KEYBOARD = 1; const uint KEYEVENTF_KEYUP = 0x0002; const uint KEYEVENTF_SCANCODE = 0x0008; const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        static readonly HashSet<ushort> _extended = new() { 0x21,0x22,0x23,0x24, 0x25,0x26,0x27,0x28, 0x2D,0x2E, 0x6F, 0xA3,0xA5 };
        private static readonly Dictionary<string, ushort> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["A"] = 0x41,["B"] = 0x42,["C"] = 0x43,["D"] = 0x44,["E"] = 0x45,["F"] = 0x46,["G"] = 0x47,["H"] = 0x48,["I"] = 0x49,["J"] = 0x4A,["K"] = 0x4B,["L"] = 0x4C,["M"] = 0x4D,["N"] = 0x4E,["O"] = 0x4F,["P"] = 0x50,["Q"] = 0x51,["R"] = 0x52,["S"] = 0x53,["T"] = 0x54,["U"] = 0x55,["V"] = 0x56,["W"] = 0x57,["X"] = 0x58,["Y"] = 0x59,["Z"] = 0x5A,
            ["0"] = 0x30,["1"] = 0x31,["2"] = 0x32,["3"] = 0x33,["4"] = 0x34,["5"] = 0x35,["6"] = 0x36,["7"] = 0x37,["8"] = 0x38,["9"] = 0x39,
            ["Space"] = 0x20,["Enter"] = 0x0D,["Tab"] = 0x09,["Esc"] = 0x1B,["Escape"] = 0x1B,["Backspace"] = 0x08,["Delete"] = 0x2E,["Insert"] = 0x2D,
            ["Left"] = 0x25,["Up"] = 0x26,["Right"] = 0x27,["Down"] = 0x28,["Home"] = 0x24,["End"] = 0x23,["PageUp"] = 0x21,["PageDown"] = 0x22,
            ["F1"] = 0x70,["F2"] = 0x71,["F3"] = 0x72,["F4"] = 0x73,["F5"] = 0x74,["F6"] = 0x75,["F7"] = 0x76,["F8"] = 0x77,["F9"] = 0x78,["F10"] = 0x79,["F11"] = 0x7A,["F12"] = 0x7B,
            ["Shift"] = 0x10,["Ctrl"] = 0x11,["Control"] = 0x11,["Alt"] = 0x12,
        };

        private static bool TrySendInputs(INPUT[] inputs, out int lastError)
        {
            uint sent = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());
            if (sent == inputs.Length) { lastError = 0; return true; }
            lastError = Marshal.GetLastWin32Error(); return false;
        }

        private static INPUT BuildKeyDown(ushort vkey, ushort scan, bool ext)
            => scan != 0
               ? new INPUT { type = INPUT_KEYBOARD, U = new InputUnion { ki = new KEYBDINPUT { wVk = 0, wScan = scan, dwFlags = KEYEVENTF_SCANCODE | (ext ? KEYEVENTF_EXTENDEDKEY : 0) } } }
               : new INPUT { type = INPUT_KEYBOARD, U = new InputUnion { ki = new KEYBDINPUT { wVk = vkey, wScan = 0, dwFlags = 0 } } };

        private static INPUT BuildKeyUp(ushort vkey, ushort scan, bool ext)
            => scan != 0
               ? new INPUT { type = INPUT_KEYBOARD, U = new InputUnion { ki = new KEYBDINPUT { wVk = 0, wScan = scan, dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP | (ext ? KEYEVENTF_EXTENDEDKEY : 0) } } }
               : new INPUT { type = INPUT_KEYBOARD, U = new InputUnion { ki = new KEYBDINPUT { wVk = vkey, wScan = 0, dwFlags = KEYEVENTF_KEYUP } } };

        public bool TryParseKey(string name, out ushort vkey, out string normalized)
        {
            if (_map.TryGetValue(name, out vkey)) { normalized = Normalize(name); return true; }
            if (name.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && ushort.TryParse(name.AsSpan(2), System.Globalization.NumberStyles.HexNumber, provider: null, out vkey))
            { normalized = $"VK_{vkey:X2}"; return true; }
            normalized = name; vkey = 0; return false;
        }

        public bool KeyDown(ushort vkey, out int lastError)
        {
            ushort scan = (ushort)MapVirtualKey(vkey, 0); bool ext = _extended.Contains(vkey);
            var down = BuildKeyDown(vkey, scan, ext); if (TrySendInputs(new[] { down }, out lastError)) return true;
            var downVk = BuildKeyDown(vkey, 0, false); return TrySendInputs(new[] { downVk }, out lastError);
        }

        public bool KeyUp(ushort vkey, out int lastError)
        {
            ushort scan = (ushort)MapVirtualKey(vkey, 0); bool ext = _extended.Contains(vkey);
            var up = BuildKeyUp(vkey, scan, ext); if (TrySendInputs(new[] { up }, out lastError)) return true;
            var upVk = BuildKeyUp(vkey, 0, false); return TrySendInputs(new[] { upVk }, out lastError);
        }

        private static string Normalize(string s) => s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s[1..].ToLowerInvariant();
    }
}
