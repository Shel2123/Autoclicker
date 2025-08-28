using System.ComponentModel;
using System.Runtime.InteropServices;
using Autoclicker.Core.Abstractions;

namespace Autoclicker.Infrastructure
{
    internal sealed class HotkeyManager : NativeWindow, IHotkeyService
    {
        [DllImport("user32.dll", SetLastError = true)] static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll", SetLastError = true)] static extern bool UnregisterHotKey(nint hWnd, int id);
        private const int WM_HOTKEY = 0x0312; private const int ERROR_HOTKEY_ALREADY_REGISTERED = 1409;

        private sealed class Reg { public int id; public Keys key; public uint mods; public required Action action; public bool isRegistered; }
        private readonly List<Reg> _regs = new();
        private int _nextId = 1;

        public HotkeyManager(Form host)
        {
            host.HandleCreated += (_, __) => { AssignHandle(host.Handle); foreach (var r in _regs) TryRegister(r); };
            host.HandleDestroyed += (_, __) => { foreach (var r in _regs) { if (r.isRegistered && Handle != 0) { UnregisterHotKey(Handle, r.id); r.isRegistered = false; } } ReleaseHandle(); };
            if (host.IsHandleCreated) AssignHandle(host.Handle);
        }

        public int Register(Keys key, Action action, uint modifiers = 0)
        {
            var existing = _regs.Find(r => r.key == key && r.mods == modifiers);
            if (existing != null) { existing.action = action; return existing.id; }
            var reg = new Reg { id = _nextId++, key = key, mods = modifiers, action = action, isRegistered = false };
            _regs.Add(reg); if (Handle != 0) TryRegister(reg); return reg.id;
        }

        public void Unregister(int id)
        {
            var r = _regs.Find(x => x.id == id); if (r == null) return;
            if (r.isRegistered && Handle != 0) UnregisterHotKey(Handle, r.id);
            _regs.Remove(r);
        }

        private void TryRegister(Reg r)
        {
            if (r.isRegistered || Handle == 0) return;
            if (!RegisterHotKey(Handle, r.id, r.mods, (uint)r.key))
            {
                int code = Marshal.GetLastWin32Error();
                if (code == ERROR_HOTKEY_ALREADY_REGISTERED)
                    throw new InvalidOperationException($"Hotkey is already in use by the OS or another program: {Describe(r.mods, r.key)}");
                ThrowWin32("RegisterHotKey", r.key, code);
            }
            r.isRegistered = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                var r = _regs.Find(t => t.id == id);
                if (r != null) { try { r.action?.Invoke(); } catch { } return; }
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            foreach (var r in _regs) { if (r.isRegistered && Handle != 0) UnregisterHotKey(Handle, r.id); r.isRegistered = false; }
            _regs.Clear();
        }

        private static void ThrowWin32(string where, Keys key, int code) => throw new Win32Exception(code, $"{where} failed for {key}, error {code}");
        private static string Describe(uint mods, Keys key)
        {
            var parts = new List<string>();
            if ((mods & 0x0001) != 0) parts.Add("Alt");
            if ((mods & 0x0002) != 0) parts.Add("Ctrl");
            if ((mods & 0x0004) != 0) parts.Add("Shift");
            if ((mods & 0x0008) != 0) parts.Add("Win");
            parts.Add(key.ToString());
            return string.Join("+", parts);
        }
    }
}
