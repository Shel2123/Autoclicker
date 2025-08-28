using System.Text.Json;
using Autoclicker.Core.Abstractions;
using Autoclicker.Core.Dto;
using Autoclicker.Core.Models;

namespace Autoclicker.Core.Services
{
    /// SRP: only save/load JSON.
    public sealed class SequencePersistence : ISequencePersistence
    {
        public void Save(string filePath, bool loop, IEnumerable<StepRow> rows)
        {
            var dto = new SequenceDto { Loop = loop };
            foreach (var r in rows)
            {
                dto.Steps.Add(new StepDto { Key = r.Key, DelaySeconds = r.DelaySeconds, HoldSeconds = r.HoldSeconds });
            }
            File.WriteAllText(filePath, JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true }));
        }

        public (bool Loop, List<StepRow> Rows) Load(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var dto = JsonSerializer.Deserialize<SequenceDto>(text) ?? throw new InvalidDataException("Invalid file format");
            var rows = new List<StepRow>();
            foreach (var s in dto.Steps)
            {
                var hold = s.HoldSeconds <= 0 ? 0.12 : s.HoldSeconds; // бэкомпат
                rows.Add(new StepRow { Key = s.Key, DelaySeconds = s.DelaySeconds, HoldSeconds = hold });
            }
            return (dto.Loop, rows);
        }
    }
}