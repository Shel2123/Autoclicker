using Autoclicker.Core.Abstractions;
using Autoclicker.Core.Models;

namespace Autoclicker.Core.Services
{
    /// SRP: only validation and translation StepRow → KeyStep
    public sealed class SequenceBuilder : ISequenceBuilder
    {
        private readonly IKeyParser _keys;
        public SequenceBuilder(IKeyParser keys) => _keys = keys;

        public bool TryBuild(IList<StepRow> rows, out List<KeyStep> steps, out string error)
        {
            steps = new();
            error = string.Empty;
            if (rows.Count == 0) { error = "The step list is empty"; return false; }

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                if (string.IsNullOrWhiteSpace(r.Key)) { error = $"Row {i + 1}: Key is empty"; return false; }
                if (r.DelaySeconds < 0) { error = $"Row {i + 1}: Delay (sec) cannot be negative"; return false; }
                if (r.HoldSeconds < 0) { error = $"Row {i + 1}: Input length (sec) cannot be negative"; return false; }

                if (!_keys.TryParseKey(r.Key.Trim(), out ushort vkey, out string normalized))
                { error = $"Row {i + 1}: unknown key \"{r.Key}\""; return false; }

                int delayMs = (int)Math.Round(r.DelaySeconds * 1000.0);
                int holdMs  = (int)Math.Round(r.HoldSeconds  * 1000.0);
                steps.Add(new KeyStep(normalized, vkey, delayMs, holdMs));
            }
            return true;
        }
    }
}