using System.Runtime.InteropServices;
using Autoclicker.Core.Abstractions;
using Autoclicker.Core.Models;

namespace Autoclicker.Core.Services
{
    /// SRP: only steps; DIP: depends on IKeyboard.
    public sealed class SequenceRunner : ISequenceRunner, IDisposable
    {
        private readonly IKeyboard _kbd;
        private CancellationTokenSource? _cts;
        public bool IsRunning => _cts != null;
        public event Action<string>? Logged;
        private void Log(string m) => Logged?.Invoke(m);

        [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int vKey);

        public SequenceRunner(IKeyboard keyboard) => _kbd = keyboard;

        public async Task RunAsync(IReadOnlyList<KeyStep> steps, bool loop, bool dryRun, Keys stopKey)
        {
            if (_cts != null) return;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            Log($"Start: {steps.Count} step(s). Loop={loop}, DryRun={dryRun}");

            _ = RunStopWatcher(stopKey, token);
            try
            {
                do
                {
                    var started = DateTime.UtcNow;
                    for (int i = 0; i < steps.Count; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        var s = steps[i];
                        await Task.Delay(s.DelayMs, token);
                        if (dryRun) { Log($"[dry] Step {i + 1}/{steps.Count}: would press {s.KeyName} for {s.HoldMs} ms after {s.DelayMs} ms"); continue; }

                        if (!_kbd.KeyDown(s.VKey, out var errDown)) { Log($"[WARN] KeyDown failed for: {s.KeyName} (GetLastError={errDown})"); continue; }
                        int hold = Math.Max(1, s.HoldMs);
                        await Task.Delay(hold, token);
                        if (!_kbd.KeyUp(s.VKey, out var errUp)) Log($"[WARN] KeyUp failed for: {s.KeyName} (GetLastError={errUp})");
                        else Log($"Pressed: {s.KeyName} held {hold} ms");
                    }
                    Log($"Sequence iteration finished. Duration: {(DateTime.UtcNow - started).TotalSeconds:F2}s");
                }
                while (loop && !token.IsCancellationRequested);
            }
            catch (TaskCanceledException) { Log("Stopped by user"); }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        public void Stop() => _cts?.Cancel();

        private Task RunStopWatcher(Keys stopKey, CancellationToken token) => Task.Run(async () =>
        {
            bool wasDown = false;
            while (!token.IsCancellationRequested)
            {
                bool isDown = (GetAsyncKeyState((int)stopKey) & 0x8000) != 0;
                if (isDown && !wasDown) Stop();
                wasDown = isDown;
                try { await Task.Delay(25, token); } catch { }
            }
        }, token);

        public void Dispose() { _cts?.Cancel(); _cts?.Dispose(); _cts = null; }
    }
}